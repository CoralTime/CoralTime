using CoralTime.DAL.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace CoralTime.Api.v1
{
    [Route("api/v1/[controller]")]
    public class TestController : Controller
    {
        private readonly UnitOfWork _uow;

        public TestController(UnitOfWork uow)
        {
            _uow = uow;
        }

        // GET: api/v1/test/ping
        [HttpGet]
        [Route("ping")]
        public IActionResult Ping()
        {
            return Ok("I'm alive!");
        }

        // GET: api/v1/test/pingdatabase
        [HttpGet]
        [Route("pingdatabase")]
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
        [Route("unauthorize")]
        public IActionResult GetUnauthorize()
        {
            return Ok("Unauthorize - success");
        }

        // GET: api/v1/test/authorize
        [Authorize]
        [HttpGet]
        [Route("authorize")]
        public IActionResult GetAuthorize()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        // GET: api/v1/test/authorize/ActiveUser
        [Authorize]
        [HttpGet]
        [Route("authorize/activeuser")]
        public IActionResult GetActiveUser()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        // GET: api/v1/test/authorize/admin
        [Authorize(Policy = "admin")]
        [HttpGet]
        [Route("authorize/admin")]
        public IActionResult GetAuthorizeAdmin()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        // GET: api/v1/test/authorize/user
        [Authorize(Policy = "user")]
        [HttpGet]
        [Route("authorize/user")]
        public IActionResult GetAuthorizeUser()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }
    }
}
