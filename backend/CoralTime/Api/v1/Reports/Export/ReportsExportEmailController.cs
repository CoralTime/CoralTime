using CoralTime.BL.Interfaces.Reports;
using CoralTime.ViewModels.Reports.Request.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CoralTime.Api.v1.Reports.Export
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsExportEmailController : BaseController<ReportsExportEmailController, IReportExportService>
    {
        public ReportsExportEmailController(IReportExportService service, ILogger<ReportsExportEmailController> logger)
            : base(logger, service) { }

        [HttpPost]
        public async Task<IActionResult> ReportsExportEmail([FromBody] ReportsExportEmailView reportsGridData)
        {
            var result = await _service.ExportEmailGroupedByType(reportsGridData);

            if (result == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok();
            }
        }
    }
}
