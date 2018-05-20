using CoralTime.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using static CoralTime.Common.Constants.Constants.Routes;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Api.v1
{
    [Route(BaseControllerRoute)]
    public class TestController : Controller
    {
        private readonly UnitOfWork _uow;

        public TestController(UnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/v1/test/ping
        [HttpGet]
        [Route(PingRoute)]
        public IActionResult Ping()
        {
            return Ok("I'm alive!");
        }

        // GET: api/v1/test/pingdatabase
        [HttpGet]
        [Route(PingdatabaseRoute)]
        public IActionResult PingDataBase()
        {
            try
            {
                var member = _uow.MemberRepository.GetQueryWithIncludes().FirstOrDefault();
                if (member != null)
                    return Ok("DB is alive !!!");
                return BadRequest("DB is dead!!");
            }
            catch (System.Exception)
            {
                return BadRequest("DB is dead!!");
            }
        }

        // GET: api/v1/test/unauthorize
        [HttpGet]
        [Route(UnauthorizeRoute)]
        public IActionResult GetUnauthorize()
        {
            return Ok("Unauthorize - success");
        }

        // GET: api/v1/test/authorize
        [Authorize]
        [HttpGet]
        [Route(AuthorizeRoute)]
        public IActionResult GetAuthorize()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        // GET: api/v1/test/authorize/ActiveUser
        [Authorize]
        [HttpGet]
        [Route(AuthorizeActiveUserRoute)]
        public IActionResult GetActiveUser()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        // GET: api/v1/test/authorize/admin
        [Authorize(Policy = ApplicationRoleAdmin)]
        [HttpGet]
        [Route(AuthorizeAdminRoute)]
        public IActionResult GetAuthorizeAdmin()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        // GET: api/v1/test/authorize/user
        [Authorize(Policy = ApplicationRoleUser)]
        [HttpGet]
        [Route(AuthorizeUserRoute)]
        public IActionResult GetAuthorizeUser()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
