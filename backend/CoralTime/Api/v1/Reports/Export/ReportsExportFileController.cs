using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Middlewares;
using CoralTime.Common.Models.Reports.Request.Grid;
using CoralTime.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Api.v1.Reports.Export
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsExportFileController : BaseController<ReportsExportFileController, IReportExportService>
    {
        public ReportsExportFileController(IReportExportService service, ILogger<ReportsExportFileController> logger)
            : base (logger, service) { }

        [HttpPost]
        public IActionResult ReportsExportFile([FromBody]RequestReportsGrid reportsGridData)
        {
            try
            {
                var userName = this.GetUserNameWithImpersonation();

                switch (reportsGridData.ValuesSaved.GroupById)
                {
                    case (int) ReportsGroupBy.Project:
                    {
                        return _service.ExportFileGroupByProjects(userName, reportsGridData, HttpContext);
                    }

                    case (int) ReportsGroupBy.User:
                    {
                        return _service.ExportFileGroupByUsers(userName, reportsGridData, HttpContext);
                    }

                    case (int) ReportsGroupBy.Date:
                    {
                        return _service.ExportFileGroupByDates(userName, reportsGridData, HttpContext);
                    }

                    case (int) ReportsGroupBy.Client:
                    {
                        return _service.ExportFileGroupByClients(userName, reportsGridData, HttpContext);
                    }

                    default:
                    {
                        return _service.ExportFileGroupByNone(userName, reportsGridData, HttpContext);
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
                _logger.LogWarning($"ReportsExportFile method with parameters ({reportsGridData});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }
    }
}
