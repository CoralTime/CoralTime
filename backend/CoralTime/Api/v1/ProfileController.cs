using CoralTime.BL.Interfaces;
using CoralTime.BL.Services;
using CoralTime.ViewModels.Member.MemberNotificationView;
using CoralTime.ViewModels.Member.MemberPersonalInfoView;
using CoralTime.ViewModels.Member.MemberPreferencesView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoralTime.Api.v1
{
    [Route("api/v1/[controller]")]
    [Authorize]
    public class ProfileController : BaseController<ProfileController, IProfileService>
    {
        private readonly IImageService _imageService;

        public ProfileController(IProfileService service, ILogger<ProfileController> logger, IImageService imageService)
            : base(logger, service)
        {
            _imageService = imageService;
        }

        [HttpGet("Projects")]
        public ActionResult GetMemberProjects() => new JsonResult(_service.GetMemberProjects());

        [HttpGet("DateFormats")]
        public IActionResult GetDateFormats() => new JsonResult(_service.GetDateFormats());

        [HttpGet("ProjectMembers/{projectId}")]
        public ActionResult GetProjectMembers(int projectId) => Ok(_service.GetProjectMembers(projectId));

        #region Notifications Preferences PersonalInfo

        // PATCH: api/v1/Profile/Member(3066)/Notifications
        [HttpPatch("Member({id})/Notifications")]
        public IActionResult Patch(int id, [FromBody]MemberNotificationView memberNotificationView)
        {
            memberNotificationView.Id = id;

            return Ok(_service.PatchNotifications(memberNotificationView));
        }

        // PATCH: api/v1/Profile/Member(3066)/Preferences
        [HttpPatch("Member({id})/Preferences")]
        public IActionResult Patch(int id, [FromBody]MemberPreferencesView memberPreferencesView)
        {
            memberPreferencesView.Id = id;

            return Ok(_service.PatchPreferences(memberPreferencesView));
        }

        // PATCH: api/v1/Profile/Member(3066)/PersonalInfo
        [HttpPatch("Member({id})/PersonalInfo")]
        public IActionResult Patch(int id, [FromBody]MemberPersonalInfoView memberPersonalInfoView)
        {
            memberPersonalInfoView.Id = id;

            return Ok(_service.PatchPersonalInfo(memberPersonalInfoView));
        }

        #endregion

        #region Member Avatar

        [HttpGet("Member({id})/UrlAvatar")]
        public IActionResult GetUrlAvatar(int id) => Ok(_imageService.GetUrlAvatar(id));

        [HttpPut("UploadImage")]
        public IActionResult UploadImage(IFormFile file) => Ok(_imageService.UploadImage(file));

        #endregion
    }
}