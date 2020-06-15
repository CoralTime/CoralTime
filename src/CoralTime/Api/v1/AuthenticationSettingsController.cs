using CoralTime.ViewModels.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
                var roles = new Dictionary<string, string>();
                var rolesSection = _config.GetSection("Roles");
                foreach (var roleItem in rolesSection.GetChildren())
                {
                    roles.Add(roleItem.Key, roleItem.Value);
                }                

                bool.TryParse(_config["Authentication:EnableAzure"], out var enableAzure);
                var settings = new AuthenticationSettings
                {
                    EnableAzure = enableAzure,
                    InstrumentationKey = _config["ApplicationInsights:InstrumentationKey"],
                    Roles = roles
                };
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
