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
        public VstsController(IVstsService service, ILogger<VstsController> logger)
            : base(logger, service)
        {

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

        // POST api/v1/Vsts/TimeEntries
        [HttpPost(Constants.Routes.TimeEntries)]
        public IActionResult CreateTimeEntry([FromBody] VstsTimeEntry vstsTimeEntry)
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

        // POST api/v1/Vsts/Setup
        [HttpPost(Constants.Routes.Setup)]
        public IActionResult Setup([FromBody] VstsSetup vstsSetup)
        {
            if (!ValidateVstsToken())
            {
                return Unauthorized();
            }
            var result = _service.GetVstsSetupInfo(vstsSetup);

            if (result.Errors!= null)
            {
                return new BadRequestObjectResult(result.Errors);
            }
            return Ok(result);
        }

        private bool ValidateVstsToken()
        {
            //return _service.ValidateToken(GetToken()) != null;
            return true;
        }

        private bool ValidateVstsTokenAndUser(string userName)
        {
            return true;
            //return _service.GetVstsMember(token: GetToken(), null, userName: userName) != null;
        }

        private string GetToken()
        {
            return Request.Headers["VSTSToken"].ToArray().ElementAtOrDefault(0);
        }
    }
}