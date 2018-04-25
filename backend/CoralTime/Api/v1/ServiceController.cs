using CoralTime.BL.Interfaces;
using CoralTime.Common.Attributes;
using CoralTime.Common.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CoralTime.Api.v1
{
    [Route("api/v1/[controller]")]
    [ServiceFilter(typeof(CheckSecureHeaderServiceFilter))]
    public class ServiceController : BaseController<ServiceController, IMemberService>
    {
        private readonly IMemberProjectRoleService _roleService;
        private readonly IRefreshDataBaseService _refreshDataBaseService;
        private readonly IImageService _avatarService;

        public ServiceController(
            IMemberService service, 
            IMemberProjectRoleService roleService, 
            IRefreshDataBaseService refreshDataBaseService, 
            IImageService avatarService,
            ILogger<ServiceController> logger) : base(logger, service)
        {
            _roleService = roleService;
            _refreshDataBaseService = refreshDataBaseService;
            _avatarService = avatarService;
        }

        // GET api/v1/Service/UpdateManagerRoles
        [HttpGet]
        [Route("UpdateManagerRoles")]
        public ActionResult UpdateManagerRoles()
        {
            var result = _roleService.FixAllManagerRoles();
            return Ok(result);
        }

        // GET api/v1/Service/UpdateClaims
        [HttpGet]
        [Route("UpdateClaims")]
        public ActionResult UpdateClaims()
        {
            _service.UpdateUsersClaims();
            return Ok(true);
        }

        [HttpGet]
        [Route("RefreshDataBase")]
        public async Task<ActionResult >RefreshDataBase()
        {
            await _refreshDataBaseService.RefreshDataBase();
            return Ok(true);
        }

        // GET api/v1/Service/SaveImagesFromDbToStaticFiles
        [HttpGet]
        [Route("SaveImagesFromDbToStaticFiles")]
        public ActionResult SaveImagesFromDbToStaticFiles()
        {
            _avatarService.SaveImagesFromDbToFolder();
            return Ok();
        }
    }
}