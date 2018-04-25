using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Member;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoralTime.Api.v1.Odata.Members
{
    [Authorize]
    [Route("api/v1/odata/[controller]")]
    public class MembersController : BaseODataController<MembersController, IMemberService>
    {
        private readonly IImageService _avatarService;
        public MembersController(IMemberService service, ILogger<MembersController> logger, IMapper mapper, IImageService avatarService)
            : base(logger, mapper, service)
        {
            _avatarService = avatarService;
        }

        // GET: api/v1/odata/Members
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_service.GetAllMembers());
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // GET api/v1/odata/Members(5)
        [ODataRoute("Members({id})")]
        [HttpGet("{id}")]
        public IActionResult GetById([FromODataUri]int id)
        {
            try
            {
                return Ok(_service.GetById(id));
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // GET api/v1/odata/Members(2)/projects
        [ODataRoute("Members({id})/projects")]
        [HttpGet("{id}/projects")]
        public IActionResult GetProjects([FromODataUri]int id)
        {
            try
            {
                return Ok(_service.GetTimeTrackerAllProjects(id));
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        // POST: api/v1/odata/Members
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create([FromBody] MemberView memberView)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";

            var createdMemberView = await _service.CreateNewUser(memberView, baseUrl);

            var locationUri = $"{Request.Host}/api/v1/odata/Members/{memberView.Id}";

            return base.Created(locationUri, (object)createdMemberView);
        }

        // PUT: api/v1/odata/Members(1)
        [ODataRoute("Members({id})")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromODataUri]int id, [FromBody]MemberView memberView)
        {
            if (!ModelState.IsValid)
            {
                return SendInvalidModelResponse();
            }

            memberView.Id = id;
            var baseUrl = $"{Request.Scheme}://{Request.Host.Host}:{Request.Host.Port}";

            try
            {
                return Ok(await _service.Update(memberView, baseUrl));
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }

        //DELETE :api/v1/odata/Members(1)
        [ODataRoute("Members({id})")]
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete([FromODataUri]int id) => BadRequest($"Can't delete the member with Id - {id}");
    }
}