using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.ViewModels.TimeEntries;
using CoralTime.ViewModels.Vsts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace CoralTime.Api.v1
{
    [Route(Constants.Routes.BaseControllerRoute)]
    public class VstsController : BaseController<VstsController, IVstsService>
    {
        public VstsController(IVstsService service, ILogger<VstsController> logger)
            : base(logger, service)
        {
        }

        // GET: api/v1/Vsts/Tasks
        [HttpGet(Constants.Routes.Tasks)]
        public IActionResult GetTasks(string projectId)
        {
            if (!ValidateVstsToken())
            {
                return Unauthorized();
            }
            return Ok(_service.GetTasksByProject(projectId));
        }

        // GET: api/v1/Vsts/TimeEnties
        [HttpGet(Constants.Routes.TimeEntries)]
        public IActionResult TimeEntries(string projectId, string workItemId)
        {
            if (!ValidateVstsToken())
            {
                return Unauthorized();
            }
            var timeEntries = _service.GetTimeEntriesByWorkItemId(projectId, workItemId);

            if (timeEntries == null)
            {
                return BadRequest();
            }
            return Ok(timeEntries);
        }

        // POST api/v1/Vsts/TimeEntries
        [HttpPost(Constants.Routes.TimeEntries)]
        public IActionResult CreateTimeEntry([FromBody] TimeEntryView vstsTimeEntry)
        {
            if (!ValidateVstsToken())
            {
                return Unauthorized();
            }
            var member = _service.GetVstsMember(GetToken());
            var result = _service.SaveTimeEntry(vstsTimeEntry, member);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        // POST api/v1/Vsts/Setup
        [HttpPost(Constants.Routes.Setup)]
        public IActionResult Setup([FromBody] VstsSetup vstsSetup)
        {
            if (!ValidateVstsToken())
            {
                return Unauthorized();
            }
            var result = _service.GetVstsSetupInfo(vstsSetup);

            if (result.Errors != null)
            {
                return new BadRequestObjectResult(result.Errors);
            }
            return Ok(result);
        }

        private bool ValidateVstsToken()
        {
            return _service.ValidateToken(GetToken()) != null;
        }

        private string GetToken()
        {
            return Request.Headers["VSTSToken"].ToArray().ElementAtOrDefault(0);
        }
    }
}