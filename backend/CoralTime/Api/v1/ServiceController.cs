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
            try
            {
                var result = _roleService.FixAllManagerRoles();
                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"UpdateManagerRoles method {e}");
                var errors = ExceptionsChecker.CheckProjectRolesException(e);
                return BadRequest(errors);
            }
        }

        // GET api/v1/Service/UpdateClaims
        [HttpGet]
        [Route(UpdateClaimsRoute)]
        public ActionResult UpdateClaims()
        {
            try
            {
                _service.UpdateUsersClaims();
                return Ok(true);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"UpdateClaims method {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        [HttpGet]
        [Route(RefreshDataBaseRoute)]
        public async Task<ActionResult>RefreshDataBase()
        {
            try
            {
                await _refreshDataBaseService.RefreshDataBase();
                return Ok(true);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"RefreshDataBase method {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        // GET api/v1/Service/SaveImagesFromDbToStaticFiles
        [HttpGet]
        [Route(SaveImagesFromDbToStaticFilesRoute)]
        public ActionResult SaveImagesFromDbToStaticFiles()
        {
            try
            {
                _avatarService.SaveImagesFromDbToFolder();
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"SaveAllIconsAndAvatarsInStaticFiles method {e}");
                return BadRequest(e.Message);
            }
        }
    }
}