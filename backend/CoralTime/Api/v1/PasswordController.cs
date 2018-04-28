using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static CoralTime.Common.Constants.Constants;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1
{
    [Route(BaseControllerRoute)]
    public class PasswordController : BaseController<PasswordController, IMemberService>
    {
        public PasswordController(IMemberService service, ILogger<PasswordController> logger) 
            : base(logger, service) { }

        //Put: api/v1/Password/2
        [Authorize]
        [HttpPut]
        [Route(IdRoute)]
        public async Task<IActionResult> ChangePasswordAsync(int id, [FromBody]MemberChangePasswordView member)
        {
            member.Id = id;
            await _service.ChangePassword(member);
            return Ok();
        }

        //Post: api/v1/Password/2
        [Authorize(Policy = ApplicationRoleAdmin)]
        [HttpPost]
        [Route(IdRoute)]
        public async Task<IActionResult> ResetPasswordAsync(int id)
        {
            await _service.ResetPassword(id);
            return Ok();
        }

        // GET: api/v1/Password/sendforgotemail/email@email.com
        [HttpGet]
        [Route(SendForgotEmailRoute)]
        public async Task<IActionResult> ResetPasswordAsync(string email)
        {
            var result = await _service.SentForgotEmailAsync(email, GetBaseUrl());
            return new JsonResult(result);
        }

        //[HttpGet]
        //// GET: api/v1/Password/create-password/email@email.com
        //[Route("create-password/{email}")]
        //public IActionResult SetNewPasswordAsync(string email) => new JsonResult(ResetPasswordAsync(email));

        // POST: api/v1/Password/changepasswordbytoken
        [HttpPost]
        [Route(ChangePasswordByTokenWithTokenRoute)]
        public async Task<IActionResult> ChangePasswordByTokenAsync([FromBody] MemberChangePasswordByTokenView model)
        {
            var result = await _service.ChangePasswordByTokenAsync(model);

            return new JsonResult(result);
        }

        // GET: api/v1/Password/checkforgotpasswordtoken
        [HttpGet]
        [Route(CheckPasswordByTokenWithTokenRoute)]
        public async Task<IActionResult> CheckForgotPasswordToken(string token)
        {
            var result = await _service.CheckForgotPasswordTokenAsync(token);
            return new JsonResult(result);
        }
    }
}
