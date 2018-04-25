using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Projects;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using static CoralTime.Common.Constants.Constants;
using static CoralTime.Common.Constants.Constants.Routes;
using static CoralTime.Common.Constants.Constants.Routes.OData;

namespace CoralTime.Api.v1.Odata.Projects
{
    [Route(BaseODataControllerRoute)]
    [Authorize]
    public class ProjectsController : BaseODataController<ProjectsController, IProjectService>
    {
        public ProjectsController(IProjectService service, ILogger<ProjectsController> logger)
            : base(logger, service) { }

        // GET: api/v1/odata/Projects
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_service.TimeTrackerAllProjects());
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // GET api/v1/odata/Projects(2)
        [ODataRoute(ProjectsRouteWithMembers)]
        [HttpGet(IdRouteWithMembers)]
        public IActionResult GetMembers([FromODataUri] int id)
        {
            try
            {
                return Ok(_service.GetMembers(id));
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // GET api/v1/odata/Projects(2)
        [ODataRoute(ProjectsWithIdRoute)]
        [HttpGet(IdRoute)]
        public IActionResult GetById([FromODataUri]  int id)
        {
            try
            {
                var result = _service.GetById(id);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // POST api/v1/odata/Projects
        [Authorize(Roles = ApplicationRoleAdmin)]
        [HttpPost]
        public IActionResult Create([FromBody]ProjectView projectData)
        {
            if (!ModelState.IsValid)
            {
                SendInvalidModelResponse();
            }

            try
            {
                var result = _service.Create(projectData);
                var locationUri = $"{Request.Host}/{BaseODataRoute}/Projects({result.Id})";

                return Created(locationUri, result);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // PUT api/v1/odata/Projects(1)
        [ODataRoute(ProjectsWithIdRoute)]
        [HttpPut(IdRoute)]
        public IActionResult Update([FromODataUri] int id, [FromBody]dynamic project)
        {
            if (!ModelState.IsValid)
            {
                SendInvalidModelResponse();
            }

            project.Id = id;

            try
            {
                var result = _service.Update(project);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // PATCH api/v1/odata/Projects(1)
        [ODataRoute(ProjectsWithIdRoute)]
        [HttpPatch(IdRoute)]
        public IActionResult Patch([FromODataUri] int id, [FromBody]dynamic project)
        {
            if (!ModelState.IsValid)
            {
                SendInvalidModelResponse();
            }

            project.Id = id;

            try
            {
                var result = _service.Patch(project);
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // DELETE api/v1/odata/Projects(1)
        [Authorize(Roles = ApplicationRoleAdmin)]
        [ODataRoute(ProjectsWithIdRoute)]
        [HttpDelete(IdRoute)]
        public IActionResult Delete([FromODataUri] int id)
        {
            return BadRequest($"Can't delete the project with Id - {id}");
        }
    }
}