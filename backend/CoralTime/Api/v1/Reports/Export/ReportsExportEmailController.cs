using CoralTime.BL.Interfaces.Reports;
using CoralTime.Common.Middlewares;
using CoralTime.ViewModels.Reports.Request.Emails;
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
    public class ReportsExportEmailController : BaseController<ReportsExportEmailController, IReportExportService>
    {
        public ReportsExportEmailController(IReportExportService service, ILogger<ReportsExportEmailController> logger)
            : base(logger, service) { }

        [HttpPost]
        public async Task<IActionResult> ReportsExportEmail([FromBody]ReportsExportEmailView reportsGridData)
        {
            try
            {
                switch (reportsGridData.CurrentQuery.GroupById)
                {
                    case (int) ReportsGroupBy.Project:
                    {
                        await _service.ExportEmailGroupByProjects(reportsGridData);
                        break;
                    }

                    case (int) ReportsGroupBy.User:
                    {
                        await _service.ExportEmailGroupByUsers(reportsGridData);
                        break;
                    }

                    case (int) ReportsGroupBy.Date:
                    {
                        await _service.ExportEmailGroupByDates(reportsGridData);
                        break;
                    }

                    case (int) ReportsGroupBy.Client:
                    {
                        await _service.ExportEmailGroupByClients(reportsGridData);
                        break;
                    }

                    default:
                    {
                        await _service.ExportEmailGroupByNone(reportsGridData);
                        break;
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogWarning($"ReportsExportEmail method with parameters ({reportsGridData});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }
    }
}
