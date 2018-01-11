using CoralTime.BL.ServicesInterfaces;
using CoralTime.BL.ServicesInterfaces.MemberProjecRole;
using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.Common.Middlewares;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace CoralTime.Api.v1
{
    [Route("api/v1/[controller]")]
    [CheckSecureHeader(Constants.SecureHeaderService)]
    public class ServiceController : _BaseController<ServiceController, IMemberService>
    {
        private readonly IMemberProjectRolesService _roleService;

        public ServiceController(IMemberService service, IMemberProjectRolesService roleService, ILogger<ServiceController> logger) : base(logger, service)
        {
            _roleService = roleService;
        }

        // GET api/v1/Service/UpdateManagerRoles
        [HttpGet]
        [Route("UpdateManagerRoles")]
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
        [Route("UpdateClaims")]
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
    }
}
