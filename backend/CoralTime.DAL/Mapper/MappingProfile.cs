using AutoMapper;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Clients;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.MemberProjectRoles;
using CoralTime.ViewModels.ProjectRole;
using CoralTime.ViewModels.Projects;
using CoralTime.ViewModels.Reports.Responce.DropDowns.Filters;
using CoralTime.ViewModels.Settings;
using CoralTime.ViewModels.Tasks;
using CoralTime.ViewModels.TimeEntries;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.DAL.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Member, MemberView>().ConvertUsing(new MemberToMemberViewConverter());
            CreateMap<MemberView, Member>().ConvertUsing(new MemberViewToMemberConverter());

            CreateMap<MemberProjectRoleView, MemberProjectRole>();
            CreateMap<MemberProjectRole, MemberProjectRoleView>().ConvertUsing(new MemberProjectRoleToMemberProjectRoleViewConverter());

            CreateMap<Member, ReportUsersView>()
                .ForMember(x => x.IsUserActive, x => x.MapFrom(m => m.User.IsActive))
                .ForMember(x => x.UserId, x => x.MapFrom(m => m.Id))
                .ForMember(x => x.UserFullName, x => x.MapFrom(m => m.FullName));

            CreateMap<ProjectRoleView, ProjectRole>();
            CreateMap<ProjectRole, ProjectRoleView>();
            CreateMap<Project, ProjectNameView>();
            CreateMap<Project, ProjectView>()
                .ForMember(x => x.ClientName, x => x.MapFrom(m => m.Client.Name))
                .ForMember(x => x.ClientIsActive, x => x.MapFrom(m => m.Client.IsActive))
                .ForMember(x => x.LockPeriod, x => x.MapFrom(m => (int)m.LockPeriod));
            CreateMap<ProjectView, Project>()
                .ForMember(x => x.LockPeriod, x => x.MapFrom(m => (LockTimePeriod)m.LockPeriod));

            CreateMap<Client, ClientView>();
            CreateMap<ClientView, Client>();

            CreateMap<TimeEntry, TimeEntryView>()
                .ForMember(x => x.Date, x => x.MapFrom(m => m.Date))
                .ForMember(x => x.Color, x => x.MapFrom(m => m.Project.Color))
                .ForMember(x => x.ProjectName, x => x.MapFrom(m => m.Project.Name))
                .ForMember(x => x.MemberName, x => x.MapFrom(m => m.Member.FullName))
                .ForMember(x => x.TaskName, x => x.MapFrom(m => m.TaskType.Name))
                .ForMember(x => x.IsTaskTypeActive, x => x.MapFrom(m => m.TaskType.IsActive))
                .ForMember(x => x.IsProjectActive, x => x.MapFrom(m => m.Project.IsActive))
                
                .ForPath(x => x.TimeValues.TimeActual, x => x.MapFrom(m => m.TimeActual))
                .ForPath(x => x.TimeValues.TimeEstimated, x => x.MapFrom(m => m.TimeEstimated))
                .ForPath(x => x.TimeValues.TimeFrom, x => x.MapFrom(m => m.TimeFrom))
                .ForPath(x => x.TimeValues.TimeTo, x => x.MapFrom(m => m.TimeTo))
                .ForPath(x => x.TimeOptions.IsFromToShow, x => x.MapFrom(m => m.IsFromToShow))
                .ForPath(x => x.TimeOptions.TimeTimerStart, x => x.MapFrom(m => m.TimeTimerStart));

            CreateMap<Member, ProjectMembersView>()
                .ForMember(x => x.MemberId, x => x.MapFrom(z => z.Id))
                .ForMember(x => x.MemberName, x => x.MapFrom(z => z.FullName));

            CreateMap<SettingsView, Setting>();
            CreateMap<Setting, SettingsView>();

            CreateMap<TaskTypeView, TaskType>();
            CreateMap<TaskType, TaskTypeView>();

            CreateMap<ReportsSettings, ReportsSettings>();
        }

        public class MemberToMemberViewConverter : ITypeConverter<Member, MemberView>
        {
            public MemberView Convert(Member source, MemberView destination, ResolutionContext context)
            {
                return new MemberView
                {
                    UserName = source.User.UserName,
                    Email = source.User.Email,
                    Id = source.Id,
                    IsActive = source.User.IsActive,
                    DefaultProjectId = source.DefaultProjectId,
                    IsManager = source.User.IsManager,
                    IsAdmin = source.User.IsAdmin,
                    DefaultTaskId = source.DefaultTaskId,
                    FullName = source.FullName,
                    IsWeeklyTimeEntryUpdatesSend = source.IsWeeklyTimeEntryUpdatesSend,
                    DateFormatId = source.DateFormatId,
                    DateFormat = new GetDateFormat().GetDateFormatById(source.DateFormatId),
                    SendEmailDays = ConverterBitMask.DayOfWeekIntToString(source.SendEmailDays),
                    TimeFormat = source.TimeFormat,
                    SendEmailTime = source.SendEmailTime,
                    WeekStart =  (int)source.WeekStart
                };
            }
        }

        public class MemberViewToMemberConverter : ITypeConverter<MemberView, Member>
        {
            public Member Convert(MemberView source, Member destination, ResolutionContext context)
            {
                return new Member
                {
                    DefaultTaskId = source.DefaultTaskId,
                    DefaultProjectId = source.DefaultProjectId,
                    FullName = source.FullName,
                    DateFormatId = source.DateFormatId,
                    WeekStart = (WeekStart)source.WeekStart,
                    IsWeeklyTimeEntryUpdatesSend = source.IsWeeklyTimeEntryUpdatesSend,
                    SendEmailDays = ConverterBitMask.DayOfWeekStringToInt(source.SendEmailDays),
                    SendEmailTime = source.SendEmailTime,
                    TimeFormat = source.TimeFormat,
                };
            }
        }

        public class MemberProjectRoleToMemberProjectRoleViewConverter : ITypeConverter<MemberProjectRole, MemberProjectRoleView>
        {
            public MemberProjectRoleView Convert(MemberProjectRole source, MemberProjectRoleView destination, ResolutionContext context)
            {
                return new MemberProjectRoleView
                {
                    Id = source.Id,
                    MemberId = source.Member.Id,
                    MemberName = source.Member.FullName,
                    ProjectId = source.Project.Id,
                    ProjectName = source.Project.Name,
                    RoleId = source.Role.Id,
                    RoleName = source.Role.Name,
                    MemberEmail = source.Member.User.Email,
                    MemberUserName = source.Member.User.UserName,
                    IsProjectActive = source.Project.IsActive,
                    IsProjectPrivate = source.Project.IsPrivate,
                    IsMemberActive = source.Member.User.IsActive
                };
            }
        }
    }
}