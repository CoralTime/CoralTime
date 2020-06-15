using CoralTime.BL.Interfaces.Reports;
using CoralTime.ViewModels.Reports.Request.Emails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1.Reports.Export
{
    [Authorize]
    [Route(BaseControllerRoute)]
    public class ReportsExportEmailController : BaseController<ReportsExportEmailController, IReportExportService>
    {
        public ReportsExportEmailController(IReportExportService service, ILogger<ReportsExportEmailController> logger)
            : base(logger, service) { }

        [HttpPost]
        public async Task<IActionResult> ReportsExportEmail([FromBody]ReportsExportEmailView reportsExportEmailView)
        {
            var result = await _service.ExportEmailGroupedByType(reportsExportEmailView);

            if (result == null)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
