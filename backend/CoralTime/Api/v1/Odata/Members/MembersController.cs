using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using CoralTime.DAL.Models;
using CoralTime.Services;
using CoralTime.ViewModels.Errors;
using CoralTime.ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoralTime.Api.v1.Odata.Members
{
    [Authorize]
    [EnableQuery]
    [Route("api/v1/odata/[controller]")]
    public class MembersController : BaseController<MembersController, IMemberService>
    {
        public MembersController(IMemberService service, ILogger<MembersController> logger, IMapper mapper)
            : base(logger, mapper, service) { }

        // GET: api/v1/odata/Members
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_service.GetAllMembers(this.GetUserNameWithImpersonation()));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Get method;\n {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        // GET api/v1/odata/Members(5)
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var value = _service.GetById(id);

                return new ObjectResult(_mapper.Map<Member, MemberView>(value));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetById method with parameters ({id});\n {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        // GET api/v1/odata/Members(2)
        [HttpGet("{id}/Projects")]
        public ActionResult GetProjects(int id)
        {
            try
            {
                var result = _service.GetTimeTrackerAllProjects(id);
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"GetProjects method with parameters({id});\n {e}");
                var errors = ExceptionsChecker.CheckProjectsException(e);
                return BadRequest(errors);
            }
        }

        // POST: api/v1/odata/Members
        [HttpPost]
        [Authorize(Policy = "admin")]
        public async Task<IActionResult> Create([FromBody]MemberView memberData)
        {
            if (!ModelState.IsValid)
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

            try
            {
                var createNewUserResult = await _service.CreateNewUser(memberData);

                if (memberData.SendInvitationEmail)
                {
                    var baseUrl = $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";
                    await _service.SentInvitationEmailAsync(memberData, baseUrl);
                }

                var locationUri = $"{Request.Host}/api/v1/odata/Members/{createNewUserResult.Id}";

                return Created(locationUri, _mapper.Map<Member, MemberView>(createNewUserResult));
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Create method with parameters ({JsonConvert.SerializeObject(memberData)});\n {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        // PUT: api/v1/odata/Members(1)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody]MemberView memberView)
        {
            if (!ModelState.IsValid)
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

            memberView.Id = id;

            try
            {
                var updatedMember = await _service.Update(this.GetUserName(), memberView);

                if (memberView.SendInvitationEmail)
                {
                    var baseUrl = $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";
                    await _service.SentUpdateAccountEmailAsync(updatedMember, baseUrl);
                }

                return Ok(updatedMember);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Update method with parameters ({id}, {memberView});\n {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        //DELETE :api/v1/odata/Members(1)
        [HttpDelete("{id}")]
        [Authorize(Policy = "admin")]
        public IActionResult Delete(int id)
        {
            return BadRequest($"Can't delete the member with Id - {id}");
        }
    }
}
