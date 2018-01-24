using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1.Odata.Projects
{
    [Route("api/v1/odata/[controller]")]
    [EnableQuery]
    [Authorize]
    public class ProjectsNamesController : BaseController<ProjectsNamesController, IProjectService>
    {
        public ProjectsNamesController(IProjectService service, ILogger<ProjectsNamesController> logger) : base(logger, service)
        {
        }

        // GET: api/v1/odata/Projects
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_service.GetProjectsNames());
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Get method {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }
    }
}
