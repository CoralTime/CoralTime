using CoralTime.BL.ServicesInterfaces.Reports.DDAndGrid;
using CoralTime.Common.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1.Reports.DDAndGrid
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsGroupByController : _BaseController<ReportsGroupByController, IReportService>
    {  
        public ReportsGroupByController(IReportService service, ILogger<ReportsGroupByController> logger)
            : base(logger, service) { }

        [HttpGet]
        public IActionResult GetReportsDropDownsGroupBy()
        {
            try
            {
                return new JsonResult(_service.GetReportsDropDownGroupBy());
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetReportsDropDowns method {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }
    }
}
