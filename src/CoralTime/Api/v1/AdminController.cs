using CoralTime.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using static CoralTime.Common.Constants.Constants;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1
{
    [Route(BaseControllerRoute)]
    [Authorize(Roles = ApplicationRoleAdmin)]
    public class AdminController : BaseController<AdminController, IAdminService>
    {
        private readonly IMemberProjectRoleService _roleService;
        private readonly IImageService _avatarService;
        private readonly IMemberService _memberService;
        private readonly INotificationService _notificationService;
        private readonly IVstsAdminService _vstsAdminService;

        public AdminController(
            ILogger<AdminController> logger,
            IAdminService service,
            IMemberProjectRoleService roleService,
            IImageService avatarService,
            IMemberService memberService,
            INotificationService notificationService,
            IVstsAdminService vstsAdminService)
            : base(logger, service)
        {
            _roleService = roleService;
            _avatarService = avatarService;
            _memberService = memberService;
            _notificationService = notificationService;
            _vstsAdminService = vstsAdminService;
        }

        // GET api/v1/Admin/UpdateManagerRoles
        [HttpGet]
        [Route(UpdateManagerRolesRoute)]
        public ActionResult UpdateManagerRoles()
        {
            var result = _roleService.FixAllManagerRoles();
            return Ok(result);
        }

        // GET api/v1/Admin/ResetCache
        [HttpGet]
        [Route(ResetCacheRoute)]
        public ActionResult ResetCache()
        {
            _service.ResetCache();
            return Ok(true);
        }

        // GET api/v1/Admin/UpdateClaims
        [HttpGet]
        [Route(UpdateClaimsRoute)]
        public ActionResult UpdateClaims()
        {
            _memberService.UpdateUsersClaims();
            return Ok(true);
        }

        // GET api/v1/Admin/SaveImagesFromDbToStaticFiles
        [HttpGet]
        [Route(SaveImagesFromDbToStaticFilesRoute)]
        public ActionResult SaveImagesFromDbToStaticFiles()
        {
            _avatarService.SaveImagesFromDbToFolder();
            return Ok();
        }

        #region NotificationsByProjectSettings

        // GET api/v1/Admin/NotificationsByProjectSettings
        [HttpGet]
        [Route(NotificationsByProjectSettingsRoute)]
        public ActionResult NotificationsByProjectSettingsMembers() => Ok(_notificationService.GetMembersWithProjectsNotification());

        // POST api/v1/Admin/NotificationsByProjectSettings
        //        [HttpPost] [Route(NotificationsByProjectSettingsRoute)]
        //        public async Task<IActionResult> NotificationsByProjectSettings(string todayDate, [FromBody]List<MemberWithProjectsIdsView> memberWithProjectsIds = null)
        //        {
        //            await _notificationService.SendToMemberNotificationsByProjectsSettingsAsync(ConvertStringDateToDateTime(todayDate), GetBaseUrl(), memberWithProjectsIds);
        //            return Ok();
        //        }

        #endregion NotificationsByProjectSettings

        #region NotificationsWeeklyMembers

        // GET api/v1/Admin/NotificationsWeekly
        [HttpGet]
        [Route(NotificationsWeeklyRoute)]
        public ActionResult NotificationsWeeklyMembers() => Ok(_memberService.GetMembersWithWeeklyNotifications());

        // POST api/v1/Admin/NotificationsWeekly
        //        [HttpPost] [Route(NotificationsWeeklyRoute)]
        //        public async Task<IActionResult> NotificationsWeekly([FromBody] int[] memberIds)
        //        {
        //            await _notificationService.SendWeeklyNotificationsForMembers(GetBaseUrl(), memberIds);
        //            return Ok();
        //        }

        #endregion NotificationsWeeklyMembers

        #region VSTS

        // GET api/v1/Admin/UpdateVstsProjects
        [HttpGet]
        [Route(UpdateVstsProjects)]
        public ActionResult UpdateAllVstsProjects()
        {
            _vstsAdminService.UpdateVstsProjects();
            return Ok();
        }

        // GET api/v1/Admin/UpdateVstsUsers
        [HttpGet]
        [Route(UpdateVstsUsers)]
        public ActionResult UpdateAllVstsUsers()
        {
            _vstsAdminService.UpdateVstsUsers();
            return Ok();
        }

        #endregion VSTS

        private static DateTime ConvertStringDateToDateTime(string todayDate) => todayDate == null ? DateTime.Now : DateTime.Parse(todayDate);
    }
}