using AutoMapper;
using CoralTime.BL.Interfaces;
using CoralTime.BL.Interfaces.Reports;
using CoralTime.BL.Services;
using CoralTime.BL.Services.Reports.DropDownsAndGrid;
using CoralTime.BL.Services.Reports.Export;
using CoralTime.Common.Attributes;
using CoralTime.Common.Constants;
using CoralTime.Common.Middlewares;
using CoralTime.DAL;
using CoralTime.DAL.Helpers;
using CoralTime.DAL.Models;
using CoralTime.DAL.Repositories;
using CoralTime.Services;
using CoralTime.ViewModels.Clients;
using CoralTime.ViewModels.Errors;
using CoralTime.ViewModels.Member;
using CoralTime.ViewModels.MemberProjectRoles;
using CoralTime.ViewModels.ProjectRole;
using CoralTime.ViewModels.Projects;
using CoralTime.ViewModels.Settings;
using CoralTime.ViewModels.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.Stores;
using IdentityServer4.Validation;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OData.Edm;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace CoralTime
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add MySQL support (At first create DB on MySQL server.)
            //var sqlConnectionString = (Configuration.GetConnectionString("DefaultConnectionMySQL"));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseMySQL(
            //        sqlConnectionString,
            //        b => b.MigrationsAssembly("CoralTime")));

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

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

            services.AddMemoryCache();

            services.AddAutoMapper();

            services.AddMvc();

            // Add OData
            // Comment this string for update swagger.json.
            services.AddOData();

            SetupIdentity(services);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "CoralTime", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
#if DEBUG
            loggerFactory.AddConsole();
#endif

            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();

            // Configure NLog
            env.ConfigureNLog("nlog.config");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            SetupAngularRouting(app);

            app.UseDefaultFiles();

            // Uses static file for the current path.
            app.UseStaticFiles();

            app.UseIdentityServer();

            // Add middleware exceptions
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            // Add OData
            var edmModel = SetupODataEntities(app.ApplicationServices);

            app.UseMvc();

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
                routeBuilder.MapODataServiceRoute("ODataRoute", "api/v1/odata", edmModel);

                // Work-around for #1175
                routeBuilder.EnableDependencyInjection();
            });

            app.UseCors("AllowAllOrigins");

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/api/v1/swagger", "CoralTime V1");
            });

            Constants.EnvName = env.EnvironmentName;

            CombineFileWkhtmltopdf(env);

            // Uncomment to Create DB
            //ApplicationDbContext.InitializeDatabaseAsync(app.ApplicationServices).Wait();
        }

        private void AddApplicationServices(IServiceCollection services)
        {
            // Add application services.
            services.AddSingleton<IConfiguration>(sp => Configuration);

            services.AddScoped<BaseService>();
            services.AddScoped<UnitOfWork>();
            services.AddScoped<IPersistedGrantDbContext, AppDbContext>();

            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator>();
            services.AddTransient<IdentityServer4.Services.IProfileService, IdentityWithAdditionalClaimsProfileService>();
            services.AddTransient<IExtensionGrantValidator, AzureGrant>();
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

            services.AddScoped<CheckServiceSecureHeaderFilter>();
            services.AddScoped<CheckNotificationSecureHeaderFilter>();
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

        private void SetupIdentity(IServiceCollection services)
        {
            // Identity options.
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;

                options.User.RequireUniqueEmail = true;
            });

            var accessTokenLifetime = int.Parse(Configuration["AccessTokenLifetime"]);
            var refreshTokenLifetime = int.Parse(Configuration["RefreshTokenLifetime"]);
            var isDemo = bool.Parse(Configuration["DemoSiteMode"]);

            if (isDemo)
            {
                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients(accessTokenLifetime, refreshTokenLifetime))
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                    .AddProfileService<IdentityWithAdditionalClaimsProfileService>();
            }
            else
            {
                var cert = new X509Certificate2("coraltime.pfx", "", X509KeyStorageFlags.MachineKeySet);

                services.AddIdentityServer()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(Config.GetClients(accessTokenLifetime, refreshTokenLifetime))
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                    .AddSigningCredential(cert).AddAppAuthRedirectUriValidator()
                    .AddProfileService<IdentityWithAdditionalClaimsProfileService>();
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "Bearer";
                options.DefaultChallengeScheme = "Bearer";
                options.DefaultForbidScheme = "Identity.Application";
            }).AddJwtBearer(options =>
                {
                    // name of the API resource
                    options.Audience = "WebAPI";
                    options.Authority = Configuration["Authority"];
                    options.RequireHttpsMetadata = false;
                });

            services.AddAuthorization(options =>
            {
                Config.CreateAuthorizatoinOptions(options);
            });
        }

        private static IEdmModel SetupODataEntities(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);
            builder.EntitySet<ClientView>("Clients");
            builder.EntitySet<ProjectView>("Projects");
            builder.EntitySet<MemberView>("Members");
            builder.EntitySet<MemberProjectRoleView>("MemberProjectRoles");
            builder.EntitySet<ProjectRoleView>("ProjectRoles");
            builder.EntitySet<TaskView>("Tasks");
            builder.EntitySet<ErrorView>("Errors");
            builder.EntitySet<SettingsView>("Settings");
            builder.EntitySet<ManagerProjectsView>("ManagerProjects");
            builder.EntitySet<ProjectNameView>("ProjectsNames");
            builder.EnableLowerCamelCase();
            return builder.GetEdmModel();
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

                var cmd = new ProcessStartInfo("cmd.exe", $@"/c copy /y /b {filePattern} {destFile}")
                {
                    WorkingDirectory = pathContentPDFSplitFile,
                    UseShellExecute = false
                };
                Process.Start(cmd);
            }
        }
    }
}