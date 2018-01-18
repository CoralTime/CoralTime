using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using CoralTime.Common.Models;
using CoralTime.Services;
using CoralTime.ViewModels.TimeEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Globalization;

namespace CoralTime.Api.v1.Odata
{
    [Authorize]
    [Route("api/v1/[controller]")]
    public class TimeEntriesController : _BaseController<TimeEntriesController, ITimeEntryService>
    {
        public TimeEntriesController(ITimeEntryService service, ILogger<TimeEntriesController> logger)
            : base(logger, service) { }

        // GET: api/v1/odata/TimeEntries
        [HttpGet]
        public IActionResult Get(string dateBegin, string dateEnd)
        {
            try
            {
                //DateTime dt = DateTime.ParseExact(dateEnd, "yyyy-MM-ddTHH:mm:ssZ", null);
                var dateBeginOffset = DateTimeOffset.Parse(dateBegin, null).Date;
                var dateEndOffset = DateTimeOffset.Parse(dateEnd, null).Date.AddDays(1).AddMilliseconds(-1);

                return new JsonResult(_service.GetAllTimeEntries(this.GetUserNameWithImpersonation(), dateBeginOffset, dateEndOffset));
            }
            catch (Exception e)
            {
                _logger.LogError($"Get method {e}");
                var errors = ExceptionsChecker.CheckTimeEntriesException(e);
                return BadRequest(errors);
            }
        }

        // GET api/v1/odata/TimeEntries(2)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                return new JsonResult(_service.GetById(id, this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetById method with parameters ({id});\n {e}");
                var errors = ExceptionsChecker.CheckTimeEntriesException(e);
                return BadRequest(errors);
            }
        }

        // POST api/v1/odata/TimeEntries
        [HttpPost]
        public IActionResult Create([FromBody]TimeEntryView timeEntryView)
        {
            try
            {
                return new JsonResult(_service.Create(timeEntryView, this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Create method with parameters ({timeEntryView});\n {e}");
                var errors = ExceptionsChecker.CheckTimeEntriesException(e);
                return BadRequest(errors);
            }
        }

        // PUT api/v1/odata/TimeEntries(2)
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]TimeEntryView timeEntryView)
        {
            timeEntryView.Id = id;

            try
            {
                return new JsonResult(_service.Update(timeEntryView, this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Update method with parameters ({id}, {timeEntryView});\n {e}");
                var errors = ExceptionsChecker.CheckTimeEntriesException(e);
                return BadRequest(errors);
            }
        }

        // PATCH: api/v1/odata/TimeEntries(2)
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody]TimeEntryTime timeEntryTime)
        {
            timeEntryTime.Id = id;

            try
            {
                return new JsonResult(_service.Patch(timeEntryTime, this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Patch method with parameters ({id}, {timeEntryTime});\n {e}");
                var errors = ExceptionsChecker.CheckTimeEntriesException(e);
                return BadRequest(errors);
            }
        }

        // DELETE api/v1/odata/TimeEntries(1)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var result = _service.Delete(id, this.GetUserNameWithImpersonation());
                return new JsonResult(string.Empty);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Delete method with parameters ({id});\n {e}");
                var errors = ExceptionsChecker.CheckTimeEntriesException(e);
                return BadRequest(errors);
            }
        }
    }
}
