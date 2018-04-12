using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.TimeEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1.Odata
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class TimeEntriesController : BaseController<TimeEntriesController, ITimeEntryService>
    {
        public TimeEntriesController(ITimeEntryService service, ILogger<TimeEntriesController> logger)
            : base(logger, service) { }

        // GET: api/v1/odata/TimeEntries
        [HttpGet]
        public IActionResult Get(DateTimeOffset dateBegin, DateTimeOffset dateEnd) => new JsonResult(_service.GetAllTimeEntries(dateBegin, dateEnd));

        // GET api/v1/odata/TimeEntries(2)
        [HttpGet("{id}")]
        public IActionResult GetById(int id) => new JsonResult(_service.GetById(id));

        // POST api/v1/odata/TimeEntries
        [HttpPost]
        public IActionResult Create([FromBody] TimeEntryView timeEntryView) => new JsonResult(_service.Create(timeEntryView));

        // PUT api/v1/odata/TimeEntries(2)
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]TimeEntryView timeEntryView)
        {
            timeEntryView.Id = id;
            return new JsonResult(_service.Update(timeEntryView));
        }

        // DELETE api/v1/odata/TimeEntries(1)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok();
        }
    }
}
