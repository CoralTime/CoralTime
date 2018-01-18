using CoralTime.BL.Interfaces.Reports.DDAndGrid;
using CoralTime.Common.Constants;
using CoralTime.Common.Middlewares;
using CoralTime.Services;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1.Reports.DropDownsAndGrid
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsController : _BaseController<ReportsController, IReportService>
    {
        public ReportsController(IReportService service, ILogger<ReportsController> logger)
            : base(logger, service) { }

        [HttpGet]
        public IActionResult ReportsDropdowns()
        {
            try
            {
                return new JsonResult(_service.ReportsDropDowns(this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogError($"GetReportsDropdowns method {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        [HttpPost]
        public IActionResult ReportsGrid([FromBody]RequestReportsGrid reportsGridData)
        {
            try
            {
                var userName = this.GetUserNameWithImpersonation();

                // 0 - Default(none), 1 - Projects, 2 - Users, 3 - Dates, 4 - Clients.
                switch (reportsGridData.GroupById)
                {
                    case (int) Constants.ReportsGroupBy.Project:
                    {
                        return new JsonResult(_service.ReportsGridGroupByProjects(userName, reportsGridData));
                    }

                    case (int) Constants.ReportsGroupBy.User:
                    {
                        return new JsonResult(_service.ReportsGridGroupByUsers(userName, reportsGridData));
                    }

                    case (int) Constants.ReportsGroupBy.Date:
                    {
                        return new JsonResult(_service.ReportsGridGroupByDates(userName, reportsGridData));
                    }

                    case (int) Constants.ReportsGroupBy.Client:
                    {
                        return new JsonResult(_service.ReportsGridGroupByClients(userName, reportsGridData));
                    }

                    default:
                    {
                        return new JsonResult(_service.ReportsGridGroupByNone(userName, reportsGridData));
                    }
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
