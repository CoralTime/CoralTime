using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Projects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace CoralTime.Api.v1.Odata.Projects
{
    [Route("api/v1/odata/[controller]")]
    [Authorize]
    public class ManagerProjectsController : BaseODataController<ManagerProjectsController, IProjectService>
    {
        public ManagerProjectsController(IProjectService service, IMapper mapper, ILogger<ManagerProjectsController> logger)
            : base(logger, mapper, service) { }

        // GET api/v1/odata/ManagerProjects
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var projects = _service.ManageProjectsOfManager().Select(_mapper.Map<ProjectView, ManagerProjectsView>);

                return Ok(projects);
            }
            catch (Exception e)
            {
                return SendErrorODataResponse(e);
            }
        }
    }
}