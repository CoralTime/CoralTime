using CoralTime.DAL;
using GeekLearning.Testavior.Configuration.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CoralTime
{
    //for tests
    public class StartupConfigurationService : DefaultStartupConfigurationService
    {
        public override void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IConfigurationRoot configuration)
        {
            base.Configure(app, env, loggerFactory, configuration);
        }

        public override void ConfigureEnvironment(IHostingEnvironment env)
        {
            base.ConfigureEnvironment(env);
        }

        public override void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration)
        {
            base.ConfigureServices(services, configuration);

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }
    }
}
