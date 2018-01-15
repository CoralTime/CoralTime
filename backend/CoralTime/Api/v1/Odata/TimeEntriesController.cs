using CoralTime.BL.ServicesInterfaces;
using CoralTime.Common.Middlewares;
using CoralTime.Common.Models;
using CoralTime.Services;
using CoralTime.ViewModels.TimeEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CoralTime.Api.v1.Odata
{
    [Route("api/v1/odata/[controller]")]
    [Authorize]
    public class TimeEntriesController : _BaseController<TimeEntriesController, ITimeEntryService>
    {
        public TimeEntriesController(ITimeEntryService service, ILogger<TimeEntriesController> logger)
            : base(logger, service) { }

        // GET: api/v1/odata/TimeEntries
        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            try
            {
                // Get qry parameters (range dates for timeentries) from request string get.
                var queryStringOdata = HttpContext.Request.QueryString.Value;

                var regex = new Regex(@"\d{4}-\d{2}-\d{2}T\d{2}:\d{2}:\d{2}");
                var dates = new List<DateTime>();
                if (regex.Match(queryStringOdata).Success)
                {
                    foreach (var match in regex.Matches(queryStringOdata))
                    {
                        //DateTime dt = DateTime.ParseExact(m.Value, "yyyy-MM-ddThh:mm:ssZ", CultureInfo.InvariantCulture);
                        var dt = Convert.ToDateTime(match.ToString());
                        dates.Add(dt);
                    }

                    var dateStart = dates.Min(x => x).Date;
                    var dateEnd = dates.Max(x => x).Date.AddDays(1).AddMilliseconds(-1);

                    return Ok(_service.GetAllTimeEntries(this.GetUserNameWithImpersonation(), dateStart, dateEnd));
                }

                return BadRequest("No dates in query");
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
                return Ok(_service.GetById(id, this.GetUserNameWithImpersonation()));
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
                return Ok(_service.Create(timeEntryView, this.GetUserNameWithImpersonation()));
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
                return Ok(_service.Update(timeEntryView, this.GetUserNameWithImpersonation()));
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
                return Ok(_service.Patch(timeEntryTime, this.GetUserNameWithImpersonation()));
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
                return Ok();
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
