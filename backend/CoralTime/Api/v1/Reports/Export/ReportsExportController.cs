using CoralTime.BL.Interfaces.Reports.Export;
using CoralTime.Common.Middlewares;
using CoralTime.Services;
using CoralTime.ViewModels.Reports.Request.ReportsGrid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Api.v1.Reports.Export
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsExportController : _BaseController<ReportsExportController, IReportExportService>
    {
        public ReportsExportController(IReportExportService service, ILogger<ReportsExportController> logger)
            : base (logger, service) { }

        [HttpPost]
        public IActionResult PostReportsGrid([FromBody]RequestReportsGrid reportsGridData)
        {
            try
            {
                var userName = this.GetUserNameWithImpersonation();

                switch (reportsGridData.GroupById)
                {
                    case (int) ReportsGroupBy.Project:
                    {
                        return _service.ExportGroupByProjects(userName, reportsGridData, HttpContext);
                    }

                    case (int) ReportsGroupBy.User:
                    {
                        return _service.ExportGroupByUsers(userName, reportsGridData, HttpContext);
                    }

                    case (int) ReportsGroupBy.Date:
                    {
                        return _service.ExportGroupByDates(userName, reportsGridData, HttpContext);
                    }

                    case (int) ReportsGroupBy.Client:
                    {
                        return _service.ExportGroupByClients(userName, reportsGridData, HttpContext);
                    }

                    default:
                    {
                        return _service.ExportGroupByNone(userName, reportsGridData, HttpContext);
                    }
                }
            }
            catch (InvalidOperationException invalidOperationException)
            {
                _logger.LogWarning(invalidOperationException.Message + $"\n {invalidOperationException}");
                var errors = ExceptionsChecker.CheckRunMethodSetCommonValuesForExportException(invalidOperationException);
                return BadRequest(errors);
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
