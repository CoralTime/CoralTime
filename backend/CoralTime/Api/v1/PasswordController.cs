using CoralTime.BL.Interfaces;
using CoralTime.ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CoralTime.Api.v1
{
    [Route("api/v1/[controller]")]
    public class PasswordController : BaseController<PasswordController, IMemberService>
    {
        public PasswordController(IMemberService service, ILogger<PasswordController> logger) 
            : base(logger, service) { }

        //Put: api/v1/Password/2
        [Authorize]
        [HttpPut]
        [Route("{memberId}")]
        public async Task<IActionResult> ChangePasswordAsync(int memberId, [FromBody]MemberChangePasswordView member)
        {
            member.Id = memberId;

            await _service.ChangePassword(member);
            return Ok();
        }

        //Post: api/v1/Password/2
        [Authorize(Policy = "admin")]
        [HttpPost]
        [Route("{memberId}")]
        public async Task<IActionResult> ResetPasswordAsync(int memberId)
        {
            await _service.ResetPassword(memberId);
            return Ok();
        }

        // GET: api/v1/Password/sendforgotemail/email@email.com
        [HttpGet]
        [Route("sendforgotemail/{email}")]
        public async Task<IActionResult> ResetPasswordAsync(string email)
        {
            var serverUrl = GetBaseUrl();
            var result = await _service.SentForgotEmailAsync(email, serverUrl);

            return new JsonResult(result);
        }

        // POST: api/v1/Password/changepasswordbytoken
        [HttpPost]
        [Route("changepasswordbytoken")]
        public async Task<IActionResult> ChangePasswordByTokenAsync([FromBody] MemberChangePasswordByTokenView model)
        {
            var result = await _service.ChangePasswordByTokenAsync(model);

            return new JsonResult(result);
        }

        // GET: api/v1/Password/checkforgotpasswordtoken
        [HttpGet]
        [Route("checkforgotpasswordtoken/{token}")]
        public async Task<IActionResult> CheckForgotPasswordToken(string token)
        {
            var result = await _service.CheckForgotPasswordTokenAsync(token);
            return new JsonResult(result);
        }

        private string GetBaseUrl()
        {
            var request = Request;
            var host = request.Host.ToUriComponent();

            return $"{request.Scheme}://{host}";
        }
    }
}
