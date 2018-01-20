using System;
using CoralTime.BL.Interfaces.Reports.DropDownsAndGrid;
using CoralTime.Common.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoralTime.Api.v1.Reports.DropDownsAndGrid
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsGroupByController : _BaseController<ReportsGroupByController, IReportService>
    {
        public ReportsGroupByController(IReportService service, ILogger<ReportsGroupByController> logger)
            : base(logger, service) { }

        [HttpGet]
        public IActionResult ReportsDropDownsGroupBy()
        {
            try
            {
                return new JsonResult(_service.ReportsDropDownGroupBy());
            }
            catch (Exception e)
            {
                _logger.LogWarning($"ReportsDropDowns method {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }
    }
}
