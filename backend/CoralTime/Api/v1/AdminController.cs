using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Notifications.ByProjectSettings.Request.MemberWithProjectsLightIds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1
{
    [Route(BaseControllerRoute)]
    [Authorize(Roles = ApplicationRoleAdmin)]
    public class AdminController : BaseController<AdminController, IAdminService>
    {
        private readonly IMemberProjectRoleService _roleService;
        private readonly IRefreshDataBaseService _refreshDataBaseService;
        private readonly IImageService _avatarService;
        private readonly IMemberService _memberService;
        private readonly INotificationService _notificationService;

        public AdminController(ILogger<AdminController> logger, IAdminService service,
            IMemberProjectRoleService roleService, IRefreshDataBaseService refreshDataBaseService,IImageService avatarService, IMemberService memberService, INotificationService notificationService)
            : base(logger, service)
        {
            _roleService = roleService;
            _refreshDataBaseService = refreshDataBaseService;
            _avatarService = avatarService;
            _memberService = memberService;
            _notificationService = notificationService;
        }

        // GET api/v1/Admin/UpdateManagerRoles
        [HttpGet] [Route(UpdateManagerRolesRoute)]
        public ActionResult UpdateManagerRoles()
        {
            var result = _roleService.FixAllManagerRoles();
            return Ok(result);
        }

        // GET api/v1/Admin/ResetCache
        [HttpGet] [Route(ResetCacheRoute)]
        public ActionResult ResetCache()
        {
            _service.ResetCache();
            return Ok(true);
        }

        // GET api/v1/Admin/UpdateClaims
        [HttpGet] [Route(UpdateClaimsRoute)]
        public ActionResult UpdateClaims()
        {
            _memberService.UpdateUsersClaims();
            return Ok(true);
        }

        // GET api/v1/Admin/RefreshDataBase
        [HttpGet] [Route(RefreshDataBaseRoute)]
        public async Task<ActionResult>RefreshDataBase()
        {
            await _refreshDataBaseService.RefreshDataBase();
            return Ok(true);
        }

        // GET api/v1/Admin/SaveImagesFromDbToStaticFiles
        [HttpGet] [Route(SaveImagesFromDbToStaticFilesRoute)]
        public ActionResult SaveImagesFromDbToStaticFiles()
        {
            _avatarService.SaveImagesFromDbToFolder();
            return Ok();
        }

        #region NotificationsByProjectSettings

        // GET api/v1/Admin/NotificationsByProjectSettings
        [HttpGet] [Route(NotificationsByProjectSettingsRoute)]
        public ActionResult NotificationsByProjectSettingsMembers() => Ok(_notificationService.GetMembersWithProjectsNotification());

        // POST api/v1/Admin/NotificationsByProjectSettings
        [HttpPost] [Route(NotificationsByProjectSettingsRoute)]
        public async Task<IActionResult> NotificationsByProjectSettings(string todayDate, [FromBody]List<MemberWithProjectsIdsView> memberWithProjectsIds = null)
        {
            await _notificationService.SendToMemberNotificationsByProjectsSettingsAsync(ConvertStringDateToDateTime(todayDate), GetBaseUrl(), memberWithProjectsIds);
            return Ok();
        }

        #endregion

        #region NotificationsWeeklyMembers

        // GET api/v1/Admin/NotificationsWeekly
        [HttpGet] [Route(NotificationsWeeklyRoute)]
        public ActionResult NotificationsWeeklyMembers() => Ok(_memberService.GetMembersWithWeeklyNotifications());

        // POST api/v1/Admin/NotificationsWeekly
        [HttpPost] [Route(NotificationsWeeklyRoute)]
        public async Task<IActionResult> NotificationsWeekly(string todayDate, [FromBody] int[] memberIds)
        {
            await _notificationService.SendWeeklyNotificationsForMembers(GetBaseUrl(), ConvertStringDateToDateTime(todayDate), memberIds);
            return Ok();
        }

        private DateTime ConvertStringDateToDateTime(string todayDate) => todayDate == null ? DateTime.Now : DateTime.Parse(todayDate);

        #endregion
    }
}