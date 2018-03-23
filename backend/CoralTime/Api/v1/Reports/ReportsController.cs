using CoralTime.BL.Interfaces.Reports;
using CoralTime.ViewModels.Reports.Request.Grid;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CoralTime.Api.v1.Reports
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class ReportsController : BaseController<ReportsController, IReportsService>
    {
        public ReportsController(IReportsService service, ILogger<ReportsController> logger)
            : base(logger, service) { }

        [HttpGet]
        public IActionResult GetReportsDropdowns() => new JsonResult(_service.ReportsDropDowns());

        [HttpPost]
        public IActionResult GetReportsGridAndSaveCurrentQuery([FromBody] ReportsGridView reportsGridView)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Model");
            }

            var jsonSerializatorSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            };

            var reportsGroupingBy = _service.GetReportsGroupingBy(reportsGridView);

            return new JsonResult(reportsGroupingBy, jsonSerializatorSettings);
        }
    }
}
