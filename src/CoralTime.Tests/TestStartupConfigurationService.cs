using CoralTime.Common.Constants;
using CoralTime.DAL;
using GeekLearning.Testavior.Environment;
using IdentityModel;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Security.Claims;

namespace CoralTime.Tests
{
    //for tests
    public class TestStartupConfigurationService : TestStartupConfigurationService<AppDbContext>
    {
        protected override ClaimsIdentity ConfigureIdentity()
        {
            var claims = new ClaimsIdentity(new Claim[]
            {
                new Claim(JwtClaimTypes.Name, "Admin"),
                new Claim(JwtClaimTypes.Role, Constants.ApplicationRoleAdmin),
                new Claim(JwtClaimTypes.Id,"3066"),
            }, "test");
            return claims;
        }
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IConfigurationRoot configuration)
        {
            base.Configure(app, env, loggerFactory, configuration);
        }

        public override void ConfigureEnvironment(IHostingEnvironment env)
        {
            if (Directory.Exists(env.ContentRootPath + "\\backend\\CoralTime"))
                env.ContentRootPath += "\\backend\\CoralTime";
            else if (Directory.Exists(env.ContentRootPath + "\\workspace\\git\\backend\\CoralTime"))
                env.ContentRootPath += "\\workspace\\git\\backend\\CoralTime";
            base.ConfigureEnvironment(env);
        }

        public override void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("TestConnection")));
            base.ConfigureServices(services, configuration);
        }
    }
}