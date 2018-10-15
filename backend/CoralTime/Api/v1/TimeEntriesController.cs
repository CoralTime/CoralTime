using System;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.ViewModels.TimeEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoralTime.Api.v1
{
    [Authorize]
    [Route(Constants.Routes.BaseControllerRoute)]
    public class TimeEntriesController : BaseController<TimeEntriesController, ITimeEntryService>
    {
        public TimeEntriesController(ITimeEntryService service, ILogger<TimeEntriesController> logger)
            : base(logger, service) { }

        // GET: api/v1/TimeEntries
        [HttpGet]
        public IActionResult Get(DateTimeOffset dateBegin, DateTimeOffset dateEnd) => new JsonResult(_service.GetAllTimeEntries(dateBegin, dateEnd));

        // GET api/v1/TimeEntries/2
        [HttpGet(Constants.Routes.IdRoute)]
        public IActionResult GetById(int id) => new JsonResult(_service.GetById(id));

        // POST api/v1/TimeEntries
        [HttpPost]
        public IActionResult Create([FromBody] TimeEntryView timeEntryView) => new JsonResult(_service.Create(timeEntryView));

        // PUT api/v1/TimeEntries/2
        [HttpPut(Constants.Routes.IdRoute)]
        public IActionResult Update(int id, [FromBody]TimeEntryView timeEntryView)
        {
            timeEntryView.Id = id;
            return new JsonResult(_service.Update(timeEntryView));
        }

        // DELETE api/v1/TimeEntries/1
        [HttpDelete(Constants.Routes.IdRoute)]
        public IActionResult Delete(int id)
        {
            _service.Delete(id);
            return Ok();
        }
        
        // GET: api/v1/TimeEntries/TimeEntryTimer
        [HttpGet(Constants.Routes.TimeEntryTimer)]
        public IActionResult Get(DateTime? date) => new JsonResult(_service.GetTimeEntryTimer(date));
    }
}
