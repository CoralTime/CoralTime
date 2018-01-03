using AutoMapper;
using CoralTime.BL.Helpers;
using CoralTime.BL.ServicesInterfaces;
using CoralTime.Common.Constants;
using CoralTime.Common.Exceptions;
using CoralTime.Common.Helpers;
using CoralTime.DAL.ConvertersViews.ExstensionsMethods;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.ViewModels.Errors;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.Projects;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.BL.Services
{
    public class MemberService : _BaseService, IMemberService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public MemberService(UnitOfWork uow, UserManager<ApplicationUser> userManager, IConfiguration configuration, IMapper mapper)
            : base(uow, mapper)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public IEnumerable<MemberView> GetAllMembers(string userName)
        {
            var globalActiveProjCount = Uow.ProjectRepository.LinkedCacheGetList().Where(x => !x.IsPrivate && x.IsActive).Select(x => x.Id).ToArray();

            var allMembers = GetAllMembersCommon(userName);
            var allMembersView = allMembers.Select(p => p.GetViewWithGlobalProjectsCount(globalActiveProjCount, Mapper));

            return allMembersView;
        }

        public Member GetById(int id)
        {
            var memberById = Uow.MemberRepository.LinkedCacheGetById(id);

            if (memberById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with id {id} not found.");
            }

            return memberById;
        }

        public IEnumerable<ProjectView> GetTimeTrackerAllProjects(int memberId)
        {
            return GetProjects(memberId).Select(p => p.GetViewTimeTrackerAllProjects(Mapper));
        }

        public async Task<Member> CreateNewUser(MemberView memberView)
        {
            if (! EmailChecker.IsValidEmail(memberView.Email))
            {
                throw new CoralTimeDangerException("Invalid email");
            }
            
            if (memberView.IsAdmin)
            {
                var newUserAdmin = new ApplicationUser
                {
                    UserName = memberView.UserName,
                    Email = memberView.Email,
                    IsAdmin = true,
                    IsManager = false,
                    IsActive = true
                };

                return await CreateNewUserCommon(memberView, newUserAdmin, AdminRole);
            }
            else
            {
                var newUserMember = new ApplicationUser
                {
                    UserName = memberView.UserName,
                    Email = memberView.Email,
                    IsAdmin = false,
                    IsManager = false,
                    IsActive = true
                };

                return await CreateNewUserCommon(memberView, newUserMember, UserRole);
            }
        }

        public async Task<MemberView> Update(string userName, MemberView memberView)
        {
            var memberByName = Uow.MemberRepository.GetQueryByUserName(userName);

            if (memberByName == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with userName {userName} not found.");
            }

            if (!memberByName.User.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"Member with userName {userName} is not active.");
            }

            var memberId = memberView.Id;

            if (memberByName.Id != memberId && !memberByName.User.IsAdmin)
            {
                throw new CoralTimeForbiddenException($"Member with userName {userName} can't change other user's data.");
            }

            if (! EmailChecker.IsValidEmail(memberView.Email))
            {
                throw new CoralTimeSafeEntityException("Invalid email");
            }
            
            if (memberByName.User.IsAdmin)
            {
                var newEmail = memberView.Email;
                var newUserName = memberView.UserName;
                var newIsActive = memberView.IsActive;
                var newIsAdmin = memberView.IsAdmin;

                var member = Uow.MemberRepository.GetQueryByMemberId(memberId);

                if (member.User.Email != newEmail || member.User.UserName != newUserName || member.User.IsActive != newIsActive || member.User.IsAdmin != newIsAdmin)
                {
                    member.User.Email = newEmail;
                    member.User.UserName = newUserName;

                    var updateResult = await _userManager.UpdateAsync(member.User);
                    if (updateResult.Succeeded)
                    {
                        var startRole = member.User.IsAdmin ? AdminRole : UserRole;

                        if (memberId != memberByName.Id)
                        {
                            member.User.IsActive = newIsActive;
                            member.User.IsAdmin = newIsAdmin;
                        }

                        var finishRole = member.User.IsAdmin ? AdminRole : UserRole;

                        try
                        {
                            Uow.MemberRepository.Update(member);
                            Uow.Save();

                            if (startRole != finishRole)
                            {
                                await _userManager.RemoveFromRoleAsync(member.User, startRole);
                                await _userManager.AddToRoleAsync(member.User, finishRole);
                            }

                            UpdateUserClaims(member.Id);
                            Uow.MemberRepository.LinkedCacheClear();
                        }
                        catch (Exception e)
                        {
                            throw new CoralTimeDangerException("An error occurred while updating member", e);
                        }
                    }
                    else
                    {
                        BLHelpers.CheckMembersErrors(updateResult.Errors.Select(e => new IdentityErrorView
                        {
                            Code = e.Code,
                            Description = e.Description
                        }));
                    }
                }
            }

            var memberById = Uow.MemberRepository.GetQueryWithIncludes().FirstOrDefault(y => y.Id == memberId);

            await ChangeEmailByUserAsync(memberById, memberView.Email);

            memberById.FullName = memberView.FullName;
            memberById.DefaultProjectId = memberView.DefaultProjectId;
            memberById.DefaultTaskId = memberView.DefaultTaskId;
            memberById.DateFormatId = memberView.DateFormatId;
            memberById.TimeZone = memberView.TimeZone;
            memberById.WeekStart = (WeekStart)memberView.WeekStart;
            memberById.IsWeeklyTimeEntryUpdatesSend =memberView.IsWeeklyTimeEntryUpdatesSend;
            memberById.TimeFormat = memberView.TimeFormat;
            memberById.SendEmailTime = memberView.SendEmailTime;
            memberById.SendEmailDays = ConverterBitMask.DayOfWeekStringToInt(memberView.SendEmailDays);

            try
            {
                Uow.MemberRepository.Update(memberById);

                if (Uow.Save() > 0)
                {
                    UpdateUserClaims(memberById.Id);
                }

                Uow.MemberRepository.LinkedCacheClear();
            }
            catch (Exception e)
            {
                Uow.MemberRepository.LinkedCacheClear();
                throw new CoralTimeDangerException("An error occurred while updating member", e);
            }

            var memberByIdResult = Uow.MemberRepository.LinkedCacheGetById(memberById.Id);
            var result = memberByIdResult.GetView(Mapper);

            return result;
        }

        #region Change Password.

        public async Task ChangePassword(MemberChangePasswordView member)
        {
            var user = await _userManager.FindByIdAsync(Uow.MemberRepository.GetById(member.Id).UserId);

            if (user == null)
            {
                throw new CoralTimeEntityNotFoundException($"user with id {Uow.MemberRepository.GetById(member.Id).UserId} not found.");
            }

            if (!user.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"user with id {Uow.MemberRepository.GetById(member.Id).UserId} is not active.");
            }

            var userUpdationResult = await _userManager.ChangePasswordAsync(user, member.OldPassword, member.NewPassword);

            if (!userUpdationResult.Succeeded)
            {
                BLHelpers.CheckMembersErrors(userUpdationResult.Errors.Select(e => new IdentityErrorView
                {
                    Code = e.Code,
                    Description = e.Description
                }));
            }
        }

        public async Task ResetPassword(int id)
        {
            var user = await _userManager.FindByIdAsync(Uow.MemberRepository.GetById(id).UserId);

            if (user == null)
            {
                throw new CoralTimeEntityNotFoundException($"user with id {Uow.MemberRepository.GetById(id).UserId} not found.");
            }

            if (!user.IsActive)
            {
                throw new CoralTimeEntityNotFoundException($"user with id {Uow.MemberRepository.GetById(id).UserId} is not active.");
            }

            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            var userResetPassword = await _userManager.ResetPasswordAsync(user, resetPasswordToken, _configuration["DefaultPassword"]);

            if (!userResetPassword.Succeeded)
            {
                BLHelpers.CheckMembersErrors(userResetPassword.Errors.Select(e => new IdentityErrorView
                {
                    Code = e.Code,
                    Description = e.Description
                }));
            }
        }

        public async Task<ChangePasswordResultView> ChangePasswordByTokenAsync(MemberChangePasswordByTokenView model)
        {
            var userForgotPassRequest = Uow.UserForgotPassRequestRepository.GetRequest(model.Token);

            if (userForgotPassRequest == null)
            {
                return new ChangePasswordResultView { IsChangedPassword = false, Message = (int)Constants.Errors.InvalidToken };
            }

            var user = await _userManager.FindByEmailAsync(userForgotPassRequest.Email);
            if (user == null)
            {
                return new ChangePasswordResultView { IsChangedPassword = false, Message = (int)Constants.Errors.EmailDoesntExist };
            }

            var result = await _userManager.ResetPasswordAsync(user, userForgotPassRequest.RefreshToken, model.NewPassword);

            if (result.Succeeded)
            {
                var tokenToDeleteIds = Uow.UserForgotPassRequestRepository.GetQueryWithIncludes()
                    .Where(x => x.Email == userForgotPassRequest.Email)
                    .Select(y => y.Id)
                    .ToList();

                tokenToDeleteIds.ForEach(id => Uow.UserForgotPassRequestRepository.Delete(id));
                Uow.Save();

                return new ChangePasswordResultView { IsChangedPassword = true, Message = (int)Constants.Errors.None };
            }

            var errors = string.Empty;
            foreach (var error in result.Errors)
            {
                errors += $"{error.Description} ";
            }
            var errorResult = (int)((errors.Contains("token")) ? Constants.Errors.InvalidToken : Constants.Errors.ErrorPassword);
            return new ChangePasswordResultView { IsChangedPassword = false, Message = errorResult };
        }

        #endregion

        #region Emails: Invitation, Update Account, Forgot, Check Forgot Password Token, Change Email.

        public async Task SentInvitationEmailAsync(MemberView member, string baseUrl)
        {
            var profileUrl = baseUrl + "/profile/settings";

            bool.TryParse(_configuration["Authentication:EnableAzure"], out bool enableAzure);

            var body = (enableAzure) ?
                 new TextPart("html")
                 {
                     Text = $"Dear { member.FullName },<br/><br/>" +
                            "You have been invited to join CoralTime time tracking tool.<br/><br/>" +
                            $"To get started, please click the link: <a href='{baseUrl}'>CoralTime</a> <br/>" +
                            "If the link doesn’t work, copy and past URL into your browser.<br/><br/>" +
                            "Best wishes,<br/>" +
                            "CoralTime Team"
                 } :
                new TextPart("html")
                {
                    Text = $"Dear { member.FullName },<br/><br/>" +
                            "You have been invited to join CoralTime time tracking tool.<br/><br/>" +
                            "Below are your login details:<br/><br/>" +
                            $"username: { member.UserName }<br/>password: { member.Password } <br/>" +
                            $"You can change your password at any time on your <a href='{ profileUrl }'>Profile page</a>.<br/><br/><br/>" +
                            $"To get started, please click the link: <a href='{ baseUrl }'>CoralTime</a> <br/>" +
                            "If the link doesn’t work, copy and past URL into your browser.<br/><br/>" +
                            "Best wishes,<br/>" +
                            "CoralTime Team"
                };

            var multipart = new Multipart { body };

            var emailSender = new EmailSender(_configuration);

            emailSender.CreateSimpleMessage(member.Email, multipart, "Invitation to join CoralTime");
            await emailSender.SendMessageAsync();
        }

        public async Task SentUpdateAccountEmailAsync(MemberView member, string baseUrl)
        {
            var profileUrl = baseUrl + "/profile/settings";

            var body = new TextPart("html")
            {
                Text = $"Dear { member.FullName },<br/><br/>" +
                            "Your account data have been changed in CoralTime time tracking tool.<br/><br/>" +
                            $"You can change your account data at any time on your <a href='{ profileUrl }'>Profile page</a>.<br/><br/>" +
                            "If the link doesn’t work, copy and past URL into your browser.<br/><br/>" +
                            "Best wishes,<br/>" +
                            "CoralTime Team"
            };

            var multipart = new Multipart { body };

            var emailSender = new EmailSender(_configuration);

            emailSender.CreateSimpleMessage(member.Email, multipart, "Your account data have been change in CoralTime");
            await emailSender.SendMessageAsync();
        }

        public async Task<PasswordForgotEmailResultView> SentForgotEmailAsync(string email, string url)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new PasswordForgotEmailResultView { IsSentEmail = false, Message = (int)Constants.Errors.EmailDoesntExist };
            }

            if (!user.IsActive)
            {
                return new PasswordForgotEmailResultView { IsSentEmail = false, Message = (int)Constants.Errors.UserIsArchived };
            }

            var token = await GetForgotToken(user);
            var passwordResetLinkValidForHrs = int.Parse(_configuration["PasswordResetLinkValidForHrs"]);
            var userForgotPassRequest = Uow.UserForgotPassRequestRepository.CreateUserForgotPassRequest(email, passwordResetLinkValidForHrs, token);

            if (userForgotPassRequest == null)
            {
                return new PasswordForgotEmailResultView { IsSentEmail = false, Message = (int)Constants.Errors.ErrorSendEmail };
            }

            var link = $"{url}/forgot-password/enter-new-password?restoreCode={userForgotPassRequest.UserForgotPassRequestUid}";
            var subject = "Reset your CoralTime password.";

            var body = new TextPart("html")
            {
                Text = $@"CoralTime received a request to reset your password.<br /><br />
                        To reset your password, click this link or copy the URL below and paste it into your web browser's navigation bar:<br />
                        {link}<br /><br />
                        Please note: This link will expire in {passwordResetLinkValidForHrs} hours. If it has already expired, please go back to {url} and click the Forgot Password?<br /><br />
                        Best wishes,<br />
                        the CoralTime team"
            };

            var multipart = new Multipart { body };

            var emailSender = new EmailSender(_configuration);

            emailSender.CreateSimpleMessage(email, multipart, subject);
            try
            {
                await emailSender.SendMessageAsync();
                return new PasswordForgotEmailResultView { IsSentEmail = true, Message = (int)Constants.Errors.None };
            }
            catch (Exception)
            {
                return new PasswordForgotEmailResultView { IsSentEmail = false, Message = (int)Constants.Errors.ErrorSendEmail };
            }
        }

        public async Task<CheckForgotPasswordTokenResultView> CheckForgotPasswordTokenAsync(string token)
        {
            var userForgotPassRequest = Uow.UserForgotPassRequestRepository.GetRequest(token);

            if (userForgotPassRequest == null)
            {
                return new CheckForgotPasswordTokenResultView { IsTokenValid = false, Message = (int)Constants.Errors.InvalidToken };
            }

            var user = await _userManager.FindByEmailAsync(userForgotPassRequest.Email);
            if (user == null)
            {
                return new CheckForgotPasswordTokenResultView { IsTokenValid = false, Message = (int)Constants.Errors.EmailDoesntExist };
            }

            if (!user.IsActive)
            {
                return new CheckForgotPasswordTokenResultView { IsTokenValid = false, Message = (int)Constants.Errors.EmailDoesntExist };
            }

            return new CheckForgotPasswordTokenResultView { IsTokenValid = true, Message = (int)Constants.Errors.None };
        }

        public async Task ChangeEmailByUserAsync(Member member, string newEmail)
        {
            if (!string.IsNullOrWhiteSpace(newEmail) && member.User.Email != newEmail)
            {
                member.User.Email = newEmail;
                var updateEmailResult = await _userManager.UpdateAsync(member.User);

                if (!updateEmailResult.Succeeded)
                {
                    BLHelpers.CheckMembersErrors(updateEmailResult.Errors.Select(e => new IdentityErrorView
                    {
                        Code = e.Code,
                        Description = e.Description
                    }));
                }
            }
        }

        #endregion

        #region Claims & Token.

        public void UpdateUsersClaims()
        {
            var memberIds = Uow.MemberRepository.LinkedCacheGetList().Select(m => m.Id).ToList();

            foreach (var memberId in memberIds)
            {
                try
                {
                    UpdateUserClaims(memberId);
                    Uow.MemberRepository.LinkedCacheClear();
                }
                catch (Exception e)
                {
                    Uow.MemberRepository.LinkedCacheClear();
                    throw new CoralTimeDangerException($"Error update member claims by MemberId = {memberId}.");
                }
            }
        }

        public void UpdateUserClaims(int memberId)
        {
            var memberById = Uow.MemberRepository.GetQueryWithIncludes().FirstOrDefault(m => m.Id == memberId);

            #region v1.

            //try
            //{
            //    memberById.User.Claims.Clear();
            //    Uow.MemberRepository.Update(memberById);
            //    Uow.Save();

            //    // Assigns claims.
            //    var claims = ClaimsCreator.GetUserClaims(memberById.User.UserName, memberById.FullName, memberById.User.Email, memberById.User.IsAdmin ? AdminRole : UserRole, memberId);

            //    _userManager.AddClaimsAsync(memberById.User, claims).GetAwaiter().GetResult();

            //    ClearCacheMembers();
            //}

            //catch (Exception) { }

            #endregion

            #region v2.

            //memberByName.User.Claims.Clear();
            //memberByName.User.Claims.Add(new IdentityUserClaim<string> { ClaimType = JwtClaimTypes.Email, ClaimValue = memberByName.User.Email, });
            //var updatememberByNameClaims = _userManager.UpdateAsync(memberByName.User).GetAwaiter().GetResult();
            //if (!updatememberByNameClaims.Succeeded)
            //{
            //    BLHelpers.CheckMembersErrors(updatememberByNameClaims.Errors.Select(e => new IdentityErrorView
            //    {
            //        Code = e.Code,
            //        Description = e.Description
            //    }));
            //}

            #endregion

            var memberByNameClaims = _userManager.GetClaimsAsync(memberById.User).GetAwaiter().GetResult();
            var removeClaimsForMember = _userManager.RemoveClaimsAsync(memberById.User, memberByNameClaims).GetAwaiter().GetResult();
            if (!removeClaimsForMember.Succeeded)
            {
                CheckIdentityResultErrors(removeClaimsForMember);
            }

            var claimsForMemberNew = ClaimsCreator.GetUserClaims(memberById.User.UserName, memberById.FullName, memberById.User.Email, memberById.User.IsAdmin ? AdminRole : UserRole, memberById.Id);
            var addClaimsForMember = _userManager.AddClaimsAsync(memberById.User, claimsForMemberNew).GetAwaiter().GetResult();
            if (!addClaimsForMember.Succeeded)
            {
                CheckIdentityResultErrors(addClaimsForMember);
            }
        }

        private async Task<string> GetForgotToken(ApplicationUser user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            return token;
        }

        #endregion

        #region Other Methods.

        private async Task<Member> CreateNewUserCommon(MemberView memberView, ApplicationUser newUser, string userRole)
        {
            var userByName = Uow.UserRepository.LinkedCacheGetByName(memberView.UserName);
            if (userByName != null)
            {
                throw new CoralTimeAlreadyExistsException($"User with userName {memberView.UserName} already exist");
            }

            // Identity #1. Create User in db.AspNetUsers.
            var userCreationResult = await _userManager.CreateAsync(newUser, memberView.Password);
            if (!userCreationResult.Succeeded)
            {
                CheckIdentityResultErrors(userCreationResult);
            }

            // Identity #2. Create role for user in db.AspNetUserRoles.
            var memberUser = await _userManager.FindByNameAsync(newUser.UserName);
            var userCreateRoleResult = await _userManager.AddToRoleAsync(memberUser, UserRole);
            if (!userCreateRoleResult.Succeeded)
            {
                CheckIdentityResultErrors(userCreateRoleResult);
            }

            #region Set UserId to new Member. Save to Db. Get Member from Db with related entity User by UserId.

            // 1. Convert MemberView to Member.
            var newMember = Mapper.Map<MemberView, Member>(memberView);

            // 2. Assign UserId to Member (After Save, when you try to get entity from Db, before assign UserId to entity then it has Related Entity User).
            newMember.UserId = memberUser.Id;

            // 3. Save in Db.
            Uow.MemberRepository.Insert(newMember);
            Uow.Save();

            // 4. Clear cache for Members.
            Uow.MemberRepository.LinkedCacheClear();

            // 5. Get From Db -> Cache New Member. (Get entity With new created related entity - User)
            var memberByName = Uow.MemberRepository.LinkedCacheGetByName(memberView.UserName);

            #endregion

            // Identity #3. Create claims. Add Claims for user in AspNetUserClaims.
            var claimsUser = ClaimsCreator.GetUserClaims(memberUser.UserName, memberView.FullName, memberView.Email, userRole, memberByName.Id);
            var claimsUserResult = await _userManager.AddClaimsAsync(memberUser, claimsUser);
            if (!claimsUserResult.Succeeded)
            {
                CheckIdentityResultErrors(userCreateRoleResult);
            }

            return memberByName;
        }

        private static void CheckIdentityResultErrors(IdentityResult userCreateRoleResult)
        {
            BLHelpers.CheckMembersErrors(userCreateRoleResult.Errors.Select(e => new IdentityErrorView
            {
                Code = e.Code,
                Description = e.Description
            }));
        }

        public IEnumerable<Member> GetAllMembersCommon(string userName)
        {
            var userByName = Uow.UserRepository.LinkedCacheGetByName(userName);
            if (userByName == null)
            {
                throw new CoralTimeEntityNotFoundException("User with userName " + $"{userName} not found");
            }

            if (userByName.IsAdmin || userByName.IsManager)
            {
                var membersAll = Uow.MemberRepository.LinkedCacheGetList();
                if (membersAll == null)
                {
                    throw new CoralTimeEntityNotFoundException($"Members not found.");
                }

                return membersAll;
            }

            var memberById = Uow.MemberRepository.LinkedCacheGetByUserName(userName);
            if (memberById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member by user id {userByName.Id} not found.");
            }

            return new List<Member> { memberById };
        }

        public IEnumerable<Project> GetProjects(int memberId)
        {
            var memberById = Uow.MemberRepository.GetById(memberId);
            if (memberById == null)
            {
                throw new CoralTimeEntityNotFoundException($"Member with id {memberId} not found.");
            }

            var memberProjectsRoles = Uow.MemberProjectRoleRepository.LinkedCacheGetList()
                .Where(r => r.MemberId == memberId)
                .ToList();

            var projects = memberProjectsRoles.Select(mpr => new Project
            {
                ClientId = mpr.Project.ClientId,
                Color = mpr.Project.Color,
                DaysBeforeStopEditTimeEntries = mpr.Project.DaysBeforeStopEditTimeEntries,
                Description = mpr.Project.Description,
                Id = mpr.Project.Id,
                IsActive = mpr.Project.IsActive,
                IsPrivate = mpr.Project.IsPrivate,
                IsTimeLockEnabled = mpr.Project.IsTimeLockEnabled,
                LockPeriod = mpr.Project.LockPeriod,
                Name = mpr.Project.Name,
                TaskTypes = Uow.TaskTypeRepository.LinkedCacheGetList().Where(t => (t.ProjectId == null || t.ProjectId == mpr.ProjectId) && t.IsActive).ToList(),
                MemberProjectRoles = Uow.MemberProjectRoleRepository.LinkedCacheGetList().Where(mr => mr.ProjectId == mpr.ProjectId).ToList()
            });

            return projects;
        }

        //private ApplicationUser GetRelatedUserByName(string userName)
        //{
        //    var relatedUserByName = Uow.UserRepository.LinkedCacheGetByName(userName);
        //    if (relatedUserByName == null)
        //    {
        //        throw new CoralTimeEntityNotFoundException($"User {userName} not found.");
        //    }

        //    if (!relatedUserByName.IsActive)
        //    {
        //        throw new CoralTimeEntityNotFoundException($"User {userName} is not active.");
        //    }

        //    return relatedUserByName;
        //}

        //private Member GetRelatedMemberByUserName(string userName)
        //{
        //    var relatedMemberByName = Uow.MemberRepository.LinkedCacheGetByName(userName);
        //    if (relatedMemberByName == null)
        //    {
        //        throw new CoralTimeEntityNotFoundException($"Member with userName {userName} not found.");
        //    }

        //    //var relatedMemberById = Uow.MemberRepository.LinkedCacheGetById(memberId);
        //    //if (relatedMemberById == null)
        //    //{
        //    //    throw new CoralTimeEntityNotFoundException($"Member with id = {memberId} not found.");
        //    //}

        //    return relatedMemberByName;
        //}

        #endregion
    }
}