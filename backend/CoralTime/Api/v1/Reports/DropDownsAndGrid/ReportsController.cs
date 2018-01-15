using System;
using CoralTime.BL.ServicesInterfaces.Reports.DDAndGrid;
using CoralTime.Common.Constants;
using CoralTime.Common.Middlewares;
using CoralTime.Services;
using CoralTime.ViewModels.Reports.Request.ReportsGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoralTime.Api.v1.Reports.DropDownsAndGrid
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsController : _BaseController<ReportsController, IReportService>
    {
        public ReportsController(IReportService service, ILogger<ReportsController> logger)
            : base(logger, service) { }

        [HttpGet]
        public IActionResult GetReportsDropdowns()
        {
            try
            {
                return new JsonResult(_service.GetReportsDropDowns(this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogError($"GetReportsDropdowns method {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        [HttpPost]
        public IActionResult PostReportsGrid([FromBody]RequestReportsGrid reportsGridData)
        {
            try
            {
                var userName = this.GetUserNameWithImpersonation();

                // 0 - Default(none), 1 - Projects, 2 - Users, 3 - Dates, 4 - Clients.
                switch (reportsGridData.GroupById)
                {
                    case (int)Constants.ReportsGroupBy.Project:
                        return new JsonResult(_service.GroupByProjects(userName, reportsGridData));

                    case (int)Constants.ReportsGroupBy.User:
                        return new JsonResult(_service.GroupByUsers(userName, reportsGridData));

                    case (int)Constants.ReportsGroupBy.Date:
                        return new JsonResult(_service.GroupByDates(userName, reportsGridData));

                    case (int)Constants.ReportsGroupBy.Client:
                        return new JsonResult(_service.GroupByClients(userName, reportsGridData));

                    default:
                        return new JsonResult(_service.GroupByNone(userName, reportsGridData));
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning($"PostReportsGrid method with parameters ({reportsGridData});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }
    }
}
