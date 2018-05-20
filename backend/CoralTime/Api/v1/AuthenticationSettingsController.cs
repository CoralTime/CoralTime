using CoralTime.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using static CoralTime.Common.Constants.Constants.Routes;

namespace CoralTime.Api.v1
{
    [Route(BaseControllerRoute)]
    public class AuthenticationSettingsController : BaseController<AuthenticationSettingsController, object>
    {
        private readonly IConfiguration _config;

        public AuthenticationSettingsController(ILogger<AuthenticationSettingsController> logger, IConfiguration config) : base (logger)
        {
            _config = config;
        }

        // GET api/v1/AuthenticationSettings
        [HttpGet]
        public ActionResult GetSettings()
        {
            try
            {
                bool.TryParse(_config["Authentication:EnableAzure"], out bool enableAzure);
                var settings = new AuthenticationSettings { EnableAzure = enableAzure };
                if (enableAzure)
                {
                    var authSection = _config.GetSection("Authentication:AzureAd");
                    settings.AzureSettings = new AzureSettings
                    {
                        ClientId = authSection["ClientId"],
                        RedirectUrl = authSection["CallbackPath"],
                        Tenant = authSection["TenantId"]
                    };
                }
                return new JsonResult(settings);
            }
            catch (Exception e)
            {
                _logger.LogWarning($"Method GET - {e}");
                return BadRequest();
            }
        }
    }
}
