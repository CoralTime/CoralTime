using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using CoralTime.Services;
using CoralTime.ViewModels.Member.MemberNotificationView;
using CoralTime.ViewModels.Member.MemberPersonalInfoView;
using CoralTime.ViewModels.Member.MemberPreferencesView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ProfileController : BaseController<ProfileController, IProfileService>
    {
        public ProfileController(IProfileService service, ILogger<ProfileController> logger)
            : base(logger, service) { }

        [HttpGet("Projects")]
        public ActionResult GetMemberProjects()
        {
            try
            {
                return new JsonResult(_service.GetMemberProjects(this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetMemberProjects method {e}");
                var errors = ExceptionsChecker.CheckProfileException(e);
                return BadRequest(errors);
            }
        }

        [HttpGet("DateFormats")]
        public IActionResult GetDateFormats()
        {
            try
            {
                var result = _service.GetDateFormats();
                return new JsonResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetDateFormats method {e}");
                var errors = ExceptionsChecker.CheckProfileException(e);
                return BadRequest(errors);
            }
        }

        [HttpGet("ProjectMembers/{projectId}")]
        public ActionResult GetProjectMembers(int projectId)
        {
            try
            {
                var result = _service.GetProjectMembers(projectId, this.GetUserNameWithImpersonation());
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetProjectMembers method with parameters ({projectId});\n {e}");
                var errors = ExceptionsChecker.CheckProfileException(e);
                return BadRequest(errors);
            }
        }

        [HttpPut]
        public IActionResult UpdateMemberAvatar(IFormFile file)
        {
            if (file != null)
            {
                try
                {
                    var result = _service.SetUpdateMemberAvatar(file, this.GetUserNameWithImpersonation());
                    return Ok(result);
                }
                catch (Exception e)
                {
                    _logger.LogWarning($"UpdateMemberAvatar method {e}");
                    var errors = ExceptionsChecker.CheckProfileException(e);
                    return BadRequest(errors);
                }
            }

            return BadRequest("File is empty");
        }


        [HttpGet("Avatar/{memberId}")]
        public IActionResult GetMemberAvatar(int memberId)
        {
            try
            {
                var result = _service.GetMemberAvatar(this.GetUserNameWithImpersonation(), memberId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetMemberAvatar method with parameters ({memberId});\n {e}");
                var errors = ExceptionsChecker.CheckProfileException(e);
                return BadRequest(errors);
            }
        }

        [HttpGet("Icon/{memberId}")]
        public IActionResult GetMemberIcon(int memberId)
        {
            try
            {
                var result = _service.GetMemberIcon(this.GetUserNameWithImpersonation(), memberId);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetMemberIcon method with parameters ({memberId});\n {e}");
                var errors = ExceptionsChecker.CheckProfileException(e);
                return BadRequest(errors);
            }
        }

        // PATCH: api/v1/Profile/Member(3066)/Notifications
        [HttpPatch("Member({id})/Notifications")]
        public IActionResult Patch(int id, [FromBody]MemberNotificationView memberNotificationView)
        {
            memberNotificationView.Id = id;

            try
            {
                return Ok(_service.PatchNotifications(this.GetUserNameWithImpersonation(), memberNotificationView));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Patch method with parameters ({id}, {memberNotificationView});\n {e}");
                var errors = ExceptionsChecker.CheckTimeEntriesException(e);
                return BadRequest(errors);
            }
        }

        // PATCH: api/v1/Profile/Member(3066)/Preferences
        [HttpPatch("Member({id})/Preferences")]
        public IActionResult Patch(int id, [FromBody]MemberPreferencesView memberPreferencesView)
        {
            memberPreferencesView.Id = id;

            try
            {
                return Ok(_service.PatchPreferences(this.GetUserNameWithImpersonation(), memberPreferencesView));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Patch method with parameters ({id}, {memberPreferencesView});\n {e}");
                var errors = ExceptionsChecker.CheckTimeEntriesException(e);
                return BadRequest(errors);
            }
        }

        // PATCH: api/v1/Profile/Member(3066)/PersonalInfo
        [HttpPatch("Member({id})/PersonalInfo")]
        public IActionResult Patch(int id, [FromBody]MemberPersonalInfoView memberPersonalInfoView)
        {
            memberPersonalInfoView.Id = id;

            try
            {
                var waitResult = _service.PatchPersonalInfo(this.GetUserNameWithImpersonation(), memberPersonalInfoView);
                return Ok(waitResult);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Patch method with parameters ({id}, {memberPersonalInfoView});\n {e}");
                var errors = ExceptionsChecker.CheckProfileException(e);
                return BadRequest(errors);
            }
        }
    }
}
