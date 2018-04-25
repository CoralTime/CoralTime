using CoralTime.BL.Interfaces;
using CoralTime.Common.Attributes;
using CoralTime.ViewModels.Errors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
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

        //[HttpGet]
        //[Route("ByMemberSettings")]
        //public async Task<IActionResult> ByMemberSettings()
        //{
        //    try
        //    {
        //        await _service.ByMemberSettings();
        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogWarning($"CheckAsync method {e}");
        //        return BadRequest(new ErrorView { Source = "Other", Title = string.Empty, Details = e.Message });
        //    }
        //}

        [HttpGet]
        [Route(ByProjectSettingsRoute)]
        public async Task<IActionResult> ByProjectSettings()
        {
            await _service.ByProjectSettings(GetBaseUrl());
            return Ok();
        }
    }
}