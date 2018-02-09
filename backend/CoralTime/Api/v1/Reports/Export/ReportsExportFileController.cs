using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Middlewares;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Api.v1.Reports.Export
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsExportFileController : BaseController<ReportsExportFileController, IReportExportService>
    {
        public ReportsExportFileController(IReportExportService service, ILogger<ReportsExportFileController> logger)
            : base(logger, service) { }

        [HttpPost]
        public async Task<IActionResult> ReportsExportFileAsync([FromBody]ReportsGridView reportsGridData)
        {
            try
            {
                switch (reportsGridData.ValuesSaved.GroupById)
                {
                    case (int) ReportsGroupBy.Project:
                    {
                        return await _service.ExportFileGroupByProjectsAsync(reportsGridData, HttpContext);
                    }

                    case (int) ReportsGroupBy.User:
                    {
                        return await _service.ExportFileGroupByUsersAsync(reportsGridData, HttpContext);
                    }

                    case (int) ReportsGroupBy.Date:
                    {
                        return await _service.ExportFileGroupByDatesAsync(reportsGridData, HttpContext);
                    }

                    case (int) ReportsGroupBy.Client:
                    {
                        return await _service.ExportFileGroupByClientsAsync(reportsGridData, HttpContext);
                    }

                    default:
                    {
                        return await _service.ExportFileGroupByNoneAsync(reportsGridData, HttpContext);
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