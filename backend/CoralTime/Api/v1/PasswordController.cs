using CoralTime.BL.Interfaces;
using CoralTime.Common.Middlewares;
using CoralTime.ViewModels.Member;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace CoralTime.Api.v1
{
    [Route("api/v1/[controller]")]
    public class PasswordController : _BaseController<PasswordController, IMemberService>
    {
        public PasswordController(IMemberService service, ILogger<PasswordController> logger) : base(logger, service)
        {
        }

        //Put: api/v1/Password/2
        [Authorize]
        [HttpPut]
        [Route("{memberId}")]
        public async Task<IActionResult> ChangePasswordAsync(int memberId, [FromBody]MemberChangePasswordView member)
        {
            member.Id = memberId;

            try
            {
                await _service.ChangePassword(member);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogWarning($"ChangePasswordAsync method with parameters ({memberId}, {JsonConvert.SerializeObject(member)});\n {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        //Post: api/v1/Password/2
        [Authorize(Policy = "admin")]
        [HttpPost]
        [Route("{memberId}")]
        public async Task<IActionResult> ResetPasswordAsync(int memberId)
        {
            try
            {
                await _service.ResetPassword(memberId);
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogWarning($"ResetPasswordAsync method with parameters ({memberId});\n {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        // GET: api/v1/Password/sendforgotemail/email@email.com
        [HttpGet]
        [Route("sendforgotemail/{email}")]
        public async Task<IActionResult> ResetPasswordAsync(string email)
        {
            var serverUrl = GetBaseUrl();
            try
            {
                var result = await _service.SentForgotEmailAsync(email, serverUrl);
                return new JsonResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"ResetPasswordAsync method with parameters ({email});\n {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        // POST: api/v1/Password/changepasswordbytoken
        [HttpPost]
        [Route("changepasswordbytoken")]
        public async Task<IActionResult> ChangePasswordByTokenAsync([FromBody] MemberChangePasswordByTokenView model)
        {
            var serverUrl = GetBaseUrl();
            try
            {
                var result = await _service.ChangePasswordByTokenAsync(model);
                return new JsonResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"ChangePasswordByTokenAsync method with parameters ({JsonConvert.SerializeObject(model)});\n {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        // GET: api/v1/Password/checkforgotpasswordtoken
        [HttpGet]
        [Route("checkforgotpasswordtoken/{token}")]
        public async Task<IActionResult> CheckForgotPasswordToken(string token)
        {
            var serverUrl = GetBaseUrl();
            try
            {
                var result = await _service.CheckForgotPasswordTokenAsync(token);
                return new JsonResult(result);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"CheckForgotPasswordToken method with parameters ({token});\n {e}");
                var errors = ExceptionsChecker.CheckMembersException(e);
                return BadRequest(errors);
            }
        }

        private string GetBaseUrl()
        {
            var request = Request;

            var host = request.Host.ToUriComponent();

            return $"{request.Scheme}://{host}";
        }
    }
}
