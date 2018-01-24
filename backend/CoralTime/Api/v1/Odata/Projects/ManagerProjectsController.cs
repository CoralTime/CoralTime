using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using CoralTime.Services;
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
    public class ManagerProjectsController : BaseController<ManagerProjectsController, IProjectService>
    {
        public ManagerProjectsController(IProjectService service, ILogger<ManagerProjectsController> logger)
            : base(logger, service) { }

        // GET api/v1/odata/ManagerProjects
        [HttpGet]
        public ActionResult ManageProjectsOfManager()
        {
            try
            {
                return Ok(_service.ManageProjectsOfManager(this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"ProjectsByManager method {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }
    }
}
