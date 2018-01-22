using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using CoralTime.Services;
using CoralTime.ViewModels.MemberProjectRoles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;

namespace CoralTime.Api.v1.Odata.Members
{
    [Route("api/v1/odata/[controller]")]
    [EnableQuery]
    [Authorize]
    public class MemberProjectRolesController : BaseController<MemberProjectRolesController, IMemberProjectRolesService>
    {
        public MemberProjectRolesController(IMemberProjectRolesService service, ILogger<MemberProjectRolesController> logger)
            : base(logger, service) { }

        // GET: api/v1/odata/MemberProjectRoles
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(_service.GetAllProjectRoles(this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetAll method;\n {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }

        // GET api/v1/odata/MemberProjectRoles(2)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                return new ObjectResult(_service.GetById(id));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetById method with parameter ({id});\n {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }

        [HttpGet("{id}/Members")]
        public ActionResult GetNotAssignMembersAtProjByProjectId(int projectId)
        {
            try
            {
                return Ok(_service.GetNotAssignMembersAtProjByProjectId(projectId));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetNotAssignMembersByProjectId method with parameter ({projectId});\n {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }

        [HttpGet("{id}/Projects")]
        public IActionResult GetNotAssignMembersAtProjByMemberId(int memberId)
        {
            try
            {
                return Ok(_service.GetNotAssignMembersAtProjByMemberId(memberId));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetNotAssignMembersByMemberId method with parameter ({memberId});\n {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }

        // POST: api/v1/odata/MemberProjectRoles
        [HttpPost]
        public IActionResult Create([FromBody]MemberProjectRoleView projectRole)
        {
            try
            {
                var value = _service.Create(this.GetUserNameWithImpersonation(), projectRole);
                var locationUri = $"{Request.Host}/api/v1/odata/MemberProjectRoles({value.Id})";

                return Created(locationUri, value);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Create method with parameter ({JsonConvert.SerializeObject(projectRole)});\n {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }

        // PUT: api/v1/odata/MemberProjectRoles(2)
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody]dynamic projectRole)
        {
            projectRole.Id = id;
            try
            {
                var value = _service.Update(this.GetUserNameWithImpersonation(), projectRole);

                return new ObjectResult(value);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Update method with parameters ({id}, {projectRole});\n {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }

        // PATCH: api/v1/odata/MemberProjectRoles(2)
        [HttpPatch("{id}")]
        public IActionResult Patch(int id, [FromBody]MemberProjectRoleView projectRole)
        {
            projectRole.Id = id;

            try
            {
                var value = _service.Patch(this.GetUserNameWithImpersonation(), projectRole);
                return new ObjectResult(value);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Patch method with parameters ({id}, {projectRole});\n {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }

        //DELETE :api/v1/odata/MemberProjectRoles(1)
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _service.Delete(this.GetUserNameWithImpersonation(), id);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Delete method with parameter ({id});\n {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }
    }
}
