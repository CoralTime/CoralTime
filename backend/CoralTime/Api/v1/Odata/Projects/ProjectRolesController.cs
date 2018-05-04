using CoralTime.BL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using static CoralTime.Common.Constants.Constants.Routes.OData;

namespace CoralTime.Api.v1.Odata.Projects
{
    [Route(BaseODataControllerRoute)]
    [Authorize]
    public class ProjectRolesController : BaseODataController<ProjectRolesController, IMemberProjectRoleService>
    {
        public ProjectRolesController(IMemberProjectRoleService service, ILogger<ProjectRolesController> logger)
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
                return SendErrorODataResponse(e);
            }
        }
    }
}