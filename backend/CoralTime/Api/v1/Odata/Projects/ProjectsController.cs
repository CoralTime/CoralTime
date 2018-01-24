using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using CoralTime.Services;
using CoralTime.ViewModels.Errors;
using CoralTime.ViewModels.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace CoralTime.Api.v1.Odata.Projects
{
    [EnableQuery]
    [Route("api/v1/odata/[controller]")]
    [Authorize]
    public class ProjectsController : BaseController<ProjectsController, IProjectService>
    {
        public ProjectsController(IProjectService service, ILogger<ProjectsController> logger)
            : base(logger, service) { }

        // GET: api/v1/odata/Projects
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_service.TimeTrackerAllProjects(this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Get method {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        // GET api/v1/odata/Projects(2)
        [HttpGet("{id}/Members")]
        public ActionResult GetMembers(int id)
        {
            try
            {
                return Ok(_service.GetMembers(id));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetMembers method with parameters ({id});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        // GET api/v1/odata/Projects(2)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var result = _service.GetById(id, this.GetUserNameWithImpersonation());
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetById with parameters ({id});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        // POST api/v1/odata/Projects
        [Authorize(Policy = "admin")]
        [HttpPost]
        public IActionResult Create([FromBody]ProjectView projectData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestErrors();
            }

            try
            {
                var result = _service.Create(projectData, this.GetUserName());
                var locationUri = $"{Request.Host}/api/v1/odata/Projects({result.Id})";

                return Created(locationUri, result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Create methodwith parameters ({JsonConvert.SerializeObject(projectData)});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        // PUT api/v1/odata/Projects(1)
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]dynamic project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestErrors();
            }

            project.Id = id;

            try
            {
                var result = _service.Update(project, this.GetUserName());
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Update method with parameters ({id}, {project});\n {e} ");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        // PATCH api/v1/odata/Projects(1)
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody]dynamic project)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestErrors();
            }

            project.Id = id;

            try
            {
                var result = _service.Patch(project, this.GetUserName());
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Patch method with parameters ({id}, {project});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        private IActionResult BadRequestErrors()
        {
            return BadRequest(new List<ErrorView>
            {
                new ErrorView
                {
                    Source = "Other",
                    Title = "",
                    Details = "ModelState is invalid."
                }
            });
        }

        // DELETE api/v1/odata/Projects(1)
        [Authorize(Policy = "admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return BadRequest($"Can't delete the project with Id - {id}");
        }
    }
}
