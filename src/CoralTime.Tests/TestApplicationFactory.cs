using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using IdentityModel;
using CoralTime.Common.Constants;

namespace CoralTime.Tests
{
    public class TestApplicationFactory : WebApplicationFactory<CoralTime.Startup>
    {
        const string TestScheme = "Test";

        protected override void ConfigureClient(HttpClient client)
        {
            base.ConfigureClient(client);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestScheme);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                var integrationConfig = new ConfigurationBuilder()
                    .AddJsonFile("integrationsettings.json")
                    .Build();

                config.AddConfiguration(integrationConfig);
            });

            builder.ConfigureTestServices(services =>
            {
                services.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = TestScheme;
                    o.DefaultChallengeScheme = TestScheme;
                    o.DefaultScheme = TestScheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestScheme, options => { });
            });
         }

        class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
        {
            public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
                : base(options, logger, encoder, clock)
            {
            }

            protected override Task<AuthenticateResult> HandleAuthenticateAsync()
            {
                var claims = new[] { new Claim(JwtClaimTypes.Name, "admin"), new Claim(JwtClaimTypes.Role, Constants.ApplicationRoleAdmin), new Claim(JwtClaimTypes.Id, "3066") };
                var identity = new ClaimsIdentity(claims, TestScheme);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, TestScheme);

                var result = AuthenticateResult.Success(ticket);

                return Task.FromResult(result);
            }
        }
    }
}
