using CoralTime.BL.Interfaces;
using CoralTime.Common.Attributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1
{
    [Route(BaseControllerRoute)]
    [ServiceFilter(typeof(CheckSecureHeaderNotificationFilter))]
    public class NotificationsController : BaseController<NotificationsController, INotificationService>
    {
        public NotificationsController(INotificationService service, ILogger<NotificationsController> logger)
            : base(logger, service) { }

        [HttpGet]
        [Route(ByProjectSettingsRoute)]
        public async Task<IActionResult> ByProjectSettings()
        {
            await _service.ByProjectSettingsAsync(GetBaseUrl());
            return Ok();
        }

        [HttpGet]
        [Route(SendWeeklyTimeEntryUpdatesRoute)]
        public async Task<IActionResult> SendWeeklyTimeEntryUpdates()
        {
            await _service.SendWeeklyTimeEntryUpdatesAsync(GetBaseUrl());
            return Ok();
        }
    }
}