using CoralTime.BL.Interfaces.Reports;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1.Reports
{
    [Authorize]
    [Route(BaseControllerRoute)]
    public class ReportsSettingsController : BaseController<ReportsSettingsController, IReportsSettingsService>
    {
        public ReportsSettingsController(IReportsSettingsService service, ILogger<ReportsSettingsController> logger)
            : base(logger, service) { }

        [HttpPost]
        [Route(CustomQueryRoute)]
        public IActionResult SaveCustomQuery([FromBody] ReportsGridView reportsGridView)
        {
            _service.SaveCustomQuery(reportsGridView.CurrentQuery);

            return Ok();
        }

        [HttpDelete(CustomQueryWithIdRoute)]
        public IActionResult DeleteCustomQuery(int id)
        {
            _service.DeleteCustomQuery(id);

            return Ok();
        }
    }
}
