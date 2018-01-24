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
    public class ProjectRolesController : BaseController<ProjectRolesController, IMemberProjectRolesService>
    {
        public ProjectRolesController(IMemberProjectRolesService service, ILogger<ProjectRolesController> logger)
            : base(logger, service) { }

        // GET: api/v1/odata/ProjectRoles
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var result = _service.GetProjectRoles();

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Get method {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }
    }
}
