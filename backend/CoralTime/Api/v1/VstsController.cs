using CoralTime.BL.Interfaces;
using CoralTime.Common.Constants;
using CoralTime.ViewModels.Vsts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace CoralTime.Api.v1
{
    [Route(Constants.Routes.BaseControllerRoute)]
    public class VstsController : BaseController<VstsController, IVstsService>
    {
        private readonly ITimeEntryService _timeEntryService;

        public VstsController(IVstsService service, ILogger<VstsController> logger, ITimeEntryService timeEntryService)
            : base(logger, service)
        {
            _timeEntryService = timeEntryService;
        }

        // GET: api/v1/Vsts/Tasks
        [HttpGet(Constants.Routes.Tasks)]
        public IActionResult GetTasks(string projectName)
        {
            if (!ValidateVstsToken())
            {
                return Unauthorized();
            }
            return Ok(_service.GetTasksByProject(projectName));
        }

        // POST api/v1/Vsts
        [HttpPost]
        public IActionResult Create([FromBody] VstsTimeEntry vstsTimeEntry)
        {
            if (!ValidateVstsTokenAndUser(vstsTimeEntry.UserName))
            {
                return Unauthorized();
            }
            var result = _service.SaveTimeEntry(vstsTimeEntry);
            if (result)
            {
                return Ok();
            }
            return BadRequest();
        }

        private bool ValidateVstsToken()
        {
            return _service.ValidateToken(GetToken()) != null;
        }

        private bool ValidateVstsTokenAndUser(string userName)
        {
            return _service.GetVstsMember(token: GetToken(), null, userName: userName) != null;
        }

        private string GetToken()
        {
            return Request.Headers["Authorization"].ToArray().ElementAtOrDefault(0);
        }
    }
}