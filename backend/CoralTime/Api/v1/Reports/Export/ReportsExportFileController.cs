using CoralTime.BL.Interfaces.Reports;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1.Reports.Export
{
    [Authorize]
    [Route(BaseControllerRoute)]
    public class ReportsExportFileController : BaseController<ReportsExportFileController, IReportExportService>
    {
        public ReportsExportFileController(IReportExportService service, ILogger<ReportsExportFileController> logger)
            : base(logger, service) { }

        [HttpPost]
        public async Task<IActionResult> ReportsExportFileAsync([FromBody] ReportsGridView reportsGridData)
        {
            var result = await _service.ExportFileReportsGridAsync(reportsGridData, HttpContext);

            if (result == null)
            {
                return BadRequest();
            }

            return result;
        }
    }
}
