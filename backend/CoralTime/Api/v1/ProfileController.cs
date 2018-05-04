using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Member.MemberPersonalInfoView;
using CoralTime.ViewModels.Member.MemberPreferencesView;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1
{
    [Route(BaseControllerRoute)]
    [Authorize]
    public class ProfileController : BaseController<ProfileController, IProfileService>
    {
        private readonly IImageService _imageService;

        public ProfileController(IProfileService service, ILogger<ProfileController> logger, IImageService imageService)
            : base(logger, service)
        {
            _imageService = imageService;
        }

        [HttpGet(ProjectsRoute)]
        public ActionResult GetMemberProjects() => new JsonResult(_service.GetMemberProjects());

        [HttpGet(DateFormatsRoute)]
        public IActionResult GetDateFormats() => new JsonResult(_service.GetDateFormats());

        [HttpGet(ProjectMembersWithIdRoute)]
        public ActionResult GetProjectMembers(int id) => Ok(_service.GetProjectMembers(id));

        #region Notifications Preferences PersonalInfo

        // PATCH: api/v1/Profile/Member(3066)/Notifications
        //[HttpPatch(MemberRouteWithNotifications)]
        //public IActionResult Patch(int id, [FromBody]MemberNotificationView memberNotificationView)
        //{
        //    memberNotificationView.Id = id;

        //    return Ok(_service.PatchNotifications(memberNotificationView));
        //}

        // PATCH: api/v1/Profile/Member(3066)/Preferences
        [HttpPatch(MemberRouteWithPreferences)]
        public IActionResult Patch(int id, [FromBody]MemberPreferencesView memberPreferencesView)
        {
            memberPreferencesView.Id = id;

            return Ok(_service.PatchPreferences(memberPreferencesView));
        }

        // PATCH: api/v1/Profile/Member(3066)/PersonalInfo
        [HttpPatch(MemberRouteWithPersonalInfo)]
        public IActionResult Patch(int id, [FromBody]MemberPersonalInfoView memberPersonalInfoView)
        {
            memberPersonalInfoView.Id = id;

            return Ok(_service.PatchPersonalInfo(memberPersonalInfoView));
        }

        #endregion

        #region Member Avatar

        [HttpGet(MemberRouteWithUrlAvatar)]
        public IActionResult GetUrlAvatar(int id) => Ok(_imageService.GetUrlAvatar(id));

        [HttpPut(UploadImageRoute)]
        public IActionResult UploadImage(IFormFile file) => Ok(_imageService.UploadImage(file));

        #endregion
    }
}