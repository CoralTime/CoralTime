using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Constants;
using CoralTime.Common.Middlewares;
using CoralTime.Services;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1.Reports
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsController : BaseController<ReportsController, IReportService>
    {
        public ReportsController(IReportService service, ILogger<ReportsController> logger)
            : base(logger, service) { }

        [HttpGet]
        public IActionResult GetDropdowns()
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
        public IActionResult GetGridAndSaveCurrentQuery([FromBody] ReportsGridView reportsGridView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Model");
            }

            try
            {
                var userName = this.GetUserNameWithImpersonation();

                _service.SaveCurrentQuery(reportsGridView.ValuesSaved, userName);

                // 0 - Default(none), 1 - Projects, 2 - Users, 3 - Dates, 4 - Clients.
                switch (reportsGridView.ValuesSaved.GroupById)
                {
                    case (int) Constants.ReportsGroupBy.Project:
                    {
                        return new JsonResult(_service.ReportsGridGroupByProjects(userName, reportsGridView));
                    }

                    case (int) Constants.ReportsGroupBy.User:
                    {
                        return new JsonResult(_service.ReportsGridGroupByUsers(userName, reportsGridView));
                    }

                    case (int) Constants.ReportsGroupBy.Date:
                    {
                        return new JsonResult(_service.ReportsGridGroupByDates(userName, reportsGridView));
                    }

                    case (int) Constants.ReportsGroupBy.Client:
                    {
                        return new JsonResult(_service.ReportsGridGroupByClients(userName, reportsGridView));
                    }

                    default:
                    {
                        return BadRequest("Invalid Grouping value");
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogWarning($"PostReportsGrid method with parameters ({reportsGridView});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        [HttpPost]
        [Route("CustomQuery")]
        public IActionResult SaveCustomQuery([FromBody] ReportsGridView reportsGridView)
        {
            _service.SaveCustomQuery(reportsGridView.ValuesSaved, this.GetUserNameWithImpersonation());

            return Ok();
        }

        [HttpDelete("{id}")]
        [Route("CustomQuery")]
        public IActionResult DeleteCustomQuery(int id)
        {
            _service.DeleteCustomQuery(id, this.GetUserNameWithImpersonation());

            return Ok();
        }
    }
}
