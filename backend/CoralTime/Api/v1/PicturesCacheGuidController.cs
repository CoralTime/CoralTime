using System;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoralTime.Api.v1
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class PicturesCacheGuidController : _BaseController<PicturesCacheGuidController, IPicturesCacheGuid>
    {
        public PicturesCacheGuidController(IPicturesCacheGuid service, ILogger<PicturesCacheGuidController> logger)
            : base (logger, service) { }

        [HttpGet]
        public IActionResult GetPicturesCacheGuid()
        {
            try
            {
                return new JsonResult(_service.GetPicturesCacheGuid());
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetPicturesCacheGuid method {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }    
        }
    }
}
