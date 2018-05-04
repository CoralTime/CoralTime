using CoralTime.BL.Interfaces;
using CoralTime.Common.Attributes;
using CoralTime.Common.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1
{
    [Route(BaseControllerRoute)]
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
        [Route(UpdateManagerRolesRoute)]
        public ActionResult UpdateManagerRoles()
        {
            var result = _roleService.FixAllManagerRoles();
            return Ok(result);
        }

        // GET api/v1/Service/UpdateClaims
        [HttpGet]
        [Route(UpdateClaimsRoute)]
        public ActionResult UpdateClaims()
        {
            _service.UpdateUsersClaims();
            return Ok(true);
        }

        [HttpGet]
        [Route(RefreshDataBaseRoute)]
        public async Task<ActionResult>RefreshDataBase()
        {
            await _refreshDataBaseService.RefreshDataBase();
            return Ok(true);
        }

        // GET api/v1/Service/SaveImagesFromDbToStaticFiles
        [HttpGet]
        [Route(SaveImagesFromDbToStaticFilesRoute)]
        public ActionResult SaveImagesFromDbToStaticFiles()
        {
            _avatarService.SaveImagesFromDbToFolder();
            return Ok();
        }
    }
}