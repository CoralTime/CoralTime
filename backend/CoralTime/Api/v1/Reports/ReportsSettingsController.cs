using CoralTime.BL.Interfaces.Reports;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoralTime.Api.v1.Reports
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsSettingsController : BaseController<ReportsSettingsController, IReportsSettingsService>
    {
        public ReportsSettingsController(IReportsSettingsService service, ILogger<ReportsSettingsController> logger)
            : base(logger, service) { }

        [HttpPost]
        [Route("CustomQuery")]
        public IActionResult SaveCustomQuery([FromBody] ReportsGridView reportsGridView)
        {
            _service.SaveCustomQuery(reportsGridView.CurrentQuery);

            return Ok();
        }

        [HttpDelete("CustomQuery/{id}")]
        public IActionResult DeleteCustomQuery(int id)
        {
            _service.DeleteCustomQuery(id);

            return Ok();
        }
    }
}
