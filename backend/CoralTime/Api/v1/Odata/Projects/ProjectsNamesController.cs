using CoralTime.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1.Odata.Projects
{
    [Route("api/v1/odata/[controller]")]
    [Authorize]
    public class ProjectsNamesController : BaseODataController<ProjectsNamesController, IProjectService>
    {
        public ProjectsNamesController(IProjectService service, ILogger<ProjectsNamesController> logger) : base(logger, service)
        {
        }

        // GET: api/v1/odata/ProjectsNames
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_service.GetProjectsNames());
            }
            catch (Exception e)
            {
                return SendErrorResponse(e);
            }
        }
    }
}