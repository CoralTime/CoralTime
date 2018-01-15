using AutoMapper;
using CoralTime.BL.Services;
using CoralTime.BL.Services.Reports.DDAndGrid;
using CoralTime.BL.Services.Reports.Export;
using CoralTime.BL.ServicesInterfaces;
using CoralTime.BL.ServicesInterfaces.MemberProjecRole;
using CoralTime.BL.ServicesInterfaces.Reports.DDAndGrid;
using CoralTime.BL.ServicesInterfaces.Reports.Export;
using CoralTime.Common.Constants;
using CoralTime.Common.Middlewares;
using CoralTime.DAL;
using CoralTime.DAL.Helpers;
using CoralTime.DAL.Models;
using CoralTime.DAL.OData;
using CoralTime.DAL.Repositories;
using CoralTime.Serialization;
using CoralTime.Services;
//using MySQL.Data.Entity.Extensions; for MySQL
using GeekLearning.Testavior.Configuration.Startup;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.OData.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CoralTime
{
    public class Startup
    {
        private IStartupConfigurationService externalStartupConfiguration;

        public Startup(IHostingEnvironment environment, IStartupConfigurationService externalStartupConfiguration)
        {
            this.externalStartupConfiguration = externalStartupConfiguration;
            this.externalStartupConfiguration.ConfigureEnvironment(environment);

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
                .AddJsonFile("defaultDbData.json", optional: true);
            if (environment.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                // builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            Constants.EnvName = environment.EnvironmentName;

            CombineFileWkhtmltopdf(environment);
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add MySQL support (At first create DB on MySQL server.)
            //var sqlConnectionString = (Configuration.GetConnectionString("DefaultConnectionMySQL"));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseMySQL(
            //        sqlConnectionString,
            //        b => b.MigrationsAssembly("CoralTime")));

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            AddApplicationServices(services);

            services.AddMvc();

            services.AddMemoryCache();

            services.AddAutoMapper();

            // Add OData. Comment this string for update swagger.json.
            services.AddOData<IOdataService>();

            services.ConfigureODataSerializerProvider<ODataSerializerProvider>();

            SetupIdentity(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "CoralTime", Version = "v1" });
            });

            this.externalStartupConfiguration.ConfigureServices(services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            this.externalStartupConfiguration.Configure(app, env, loggerFactory, Configuration);

//#if DEBUG
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();
//#endif
            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();
            //add NLog.Web
            app.AddNLogWeb();
            // Configure NLog
            env.ConfigureNLog("nlog.config");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            //app.UseStatusCodePagesWithReExecute("/");
            SetupAngularRouting(app);

            app.UseCors("AllowAllOrigins");

            // IdentityServer4.AccessTokenValidation: authentication middleware for the API.
            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = Configuration["Authority"],

                AllowedScopes = { "WebAPI" },

                RequireHttpsMetadata = false
            });

            // Add middleware exceptions
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            // Add OData
            app.UseOData("api/v1/odata");

            // Microsoft.AspNetCore.StaticFiles: API for starting the application from wwwroot.
            // Uses default files as index.html.
            app.UseDefaultFiles();

            // Uses static file for the current path.
            app.UseStaticFiles();

            app.UseMvc();

            app.UseIdentity();

            app.UseIdentityServer();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/v1/swagger", "CoralTime V1");
            });

            // Uncomment to Create DB
            //AppDbContext.InitializeDatabaseAsync(app.ApplicationServices).Wait();
        }

        private  void SetupIdentity(IServiceCollection services)
        {
            //var cert = new X509Certificate2("coraltime.pfx", "", X509KeyStorageFlags.MachineKeySet);

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            // Identity options.
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;

                options.User.RequireUniqueEmail = true;
            });

            services.AddAuthorization(options =>
            {
                Config.CreateAuthorizatoinOptions(options);
            });

            var accessTokenLifetime = int.Parse(Configuration["AccessTokenLifetime"]);
            var refreshTokenLifetime = int.Parse(Configuration["RefreshTokenLifetime"]);
            // Adds IdentityServer
            services.AddIdentityServer()
              //.AddTemporarySigningCredential()
              .AddDeveloperSigningCredential()
              .AddInMemoryIdentityResources(Config.GetIdentityResources())
              .AddInMemoryApiResources(Config.GetApiResources())
              .AddInMemoryClients(Config.GetClients(accessTokenLifetime, refreshTokenLifetime))
              .AddAspNetIdentity<ApplicationUser>()
              .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
              //.AddSigningCredential(cert)
              .AddProfileService<IdentityWithAdditionalClaimsProfileService>();
        }

        private void AddApplicationServices(IServiceCollection services)
        {
            // Add application services.
            services.AddSingleton<IConfiguration>(sp => Configuration);

            services.AddScoped<_BaseService>();
            services.AddScoped<UnitOfWork>();
            services.AddScoped<IPersistedGrantDbContext, AppDbContext>();

            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddTransient<IdentityServer4.Services.IProfileService, IdentityWithAdditionalClaimsProfileService>();
            services.AddTransient<IExtensionGrantValidator, AzureGrant> ();
            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>();

            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<IClientService, ClientService>();
            services.AddScoped<IMemberProjectRolesService, MemberProjectRolesService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<INotificationService, NotificationsService>();
            services.AddScoped<IPicturesCacheGuid, PicturesCacheGuidService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITasksService, TasksService>();
            services.AddScoped<ITimeEntryService, TimeEntryService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IReportExportService, ReportExportService>();
        }

        private static void SetupAngularRouting(IApplicationBuilder app)
        {
            //TODO: add all routes
            var angularRoutes = new[] {
                "/home",
                "/profile",
                "/projects",
                "/clients",
                "/about",
                "/login",
                "/tasks",
                "/users",
                "/reports",
                "/calendar",
                "/settings",
                "/help",
                "/forgot-password",
                "/signin-oidc"
            };

            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue && null != angularRoutes.FirstOrDefault(
                        ar => context.Request.Path.Value.StartsWith(ar, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Request.Path = new PathString("/");

                    context.Response.Headers.Add("Cache-Control", "no-cache, no-store");
                    context.Response.Headers.Add("Expires", "-1");
                }

                await next();
            });
        }

        private void CombineFileWkhtmltopdf(IHostingEnvironment environment)
        {
            var fileNameWkhtmltopdf = "wkhtmltopdf.exe";
            var patchContentRoot = environment.ContentRootPath;

            var pathContentPDF = $"{patchContentRoot}\\Content\\PDF";
            var pathContentPDFSplitFile = $"{pathContentPDF}\\SplitFileWkhtmltopdf";

            var fileNotExist = !File.Exists(pathContentPDF + "\\" + fileNameWkhtmltopdf);
            if (fileNotExist)
            {
                var filePattern = "*.0**";
                var destFile = $"\"../{fileNameWkhtmltopdf}\"";

                var cmd = new ProcessStartInfo("cmd.exe", $@"/c copy /y /b {filePattern} {destFile}");
                cmd.WorkingDirectory = pathContentPDFSplitFile;
                cmd.UseShellExecute = false;
                Process.Start(cmd);
            }
        }
    }
}
