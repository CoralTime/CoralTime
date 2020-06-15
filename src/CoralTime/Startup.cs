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
using Microsoft.AspNet.OData.Formatter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OData.Edm;
using NLog.Extensions.Logging;
using NLog.Web;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CoralTime.BL.Services.Notifications;
using CoralTime.ViewModels.MemberActions;
using Microsoft.IdentityModel.Tokens;
using static CoralTime.Common.Constants.Constants.Routes.OData;
using Microsoft.IdentityModel.Logging;
using CoralTime.ViewModels.Vsts;
using Microsoft.AspNetCore.Mvc;

namespace CoralTime
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            bool.TryParse(Configuration["UseMySql"], out var useMySql);
            if (useMySql)
            {
                // Add MySQL support (At first create DB on MySQL server.)
                services.AddDbContextPool<AppDbContext>(options =>
                    options.UseMySql(Configuration.GetConnectionString("DefaultConnectionMySQL"),
                    b => b.MigrationsAssembly("CoralTime.MySqlMigrations")));
            }
            else
            {
                // Sql Server
                services.AddDbContextPool<AppDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            }

            IdentityModelEventSource.ShowPII = true; 
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2); 

            // Add OData
            services.AddOData();
            services.AddMvcCore(options =>
            {
                foreach (var outputFormatter in options.OutputFormatters.OfType<ODataOutputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    outputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }
                foreach (var inputFormatter in options.InputFormatters.OfType<ODataInputFormatter>().Where(_ => _.SupportedMediaTypes.Count == 0))
                {
                    inputFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("application/prs.odatatestxx-odata"));
                }

                options.EnableEndpointRouting = false; // TODO: Remove when OData does not causes exceptions anymore
            })
            .AddJsonOptions(options =>
            {
                options.SerializerSettings.Converters.Insert(0, new TrimmingStringConverter());
            });

            SetupIdentity(services);
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info { Title = "CoralTime", Version = "v1" });
            });
            
            return services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //add NLog to ASP.NET Core
            loggerFactory.AddNLog();

            // Configure NLog
            env.ConfigureNLog("nlog.config");
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }

            // Disable ApplicationInsights messages if it isn't configured
            var isApplicationInsights = Configuration.GetValue<string>("ApplicationInsights:InstrumentationKey") != null;
            if (!isApplicationInsights)
            {
                var configuration = app.ApplicationServices.GetService<Microsoft.ApplicationInsights.Extensibility.TelemetryConfiguration>();
                configuration.DisableTelemetry = true;
            }

            SetupAngularRouting(app);

            app.UseDefaultFiles();

            // Uses static file for the current path.
            app.UseStaticFiles();

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Environment.CurrentDirectory, "StaticFiles")),
                RequestPath = "/StaticFiles"
            });

            app.UseIdentityServer();

            // Add middleware exceptions
            app.UseMiddleware(typeof(ErrorHandlingMiddleware));

            // Add OData
            var edmModel = SetupODataEntities(app.ApplicationServices);

            //Make sure you add app.UseCors before app.UseMvc otherwise the request will be finished before the CORS middleware is applied
            app.UseCors("AllowAllOrigins");

            app.UseMvc();

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.Count().Filter().OrderBy().Expand().Select().MaxTop(null);
                routeBuilder.MapODataServiceRoute("ODataRoute", BaseODataRoute, edmModel);
                routeBuilder.EnableDependencyInjection();
            });

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoralTime V1");
            });

            Constants.EnvName = env.EnvironmentName;

            CombineFileWkhtmltopdf(env);

            AppDbContext.InitializeFirstTimeDataBaseAsync(app.ApplicationServices, Configuration).Wait();
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
            services.AddScoped<IMemberProjectRoleService, MemberProjectRoleService>();
            services.AddScoped<IMemberService, MemberService>();
            services.AddScoped<INotificationService, NotificationsService>();
            services.AddScoped<IProfileService, ProfileService>();
            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITasksService, TasksService>();
            services.AddScoped<ITimeEntryService, TimeEntryService>();
            services.AddScoped<IReportsService, ReportsService>();
            services.AddScoped<IReportExportService, ReportsExportService>();
            services.AddScoped<IReportsSettingsService, ReportsSettingsService>();
            services.AddScoped<IImageService, ImageService>();
            services.AddScoped<CheckSecureHeaderServiceFilter>();
            services.AddScoped<CheckSecureHeaderNotificationFilter>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IMemberActionService, MemberActionService>();
            services.AddScoped<IVstsService, VstsService>();
            services.AddScoped<IVstsAdminService, VstsService>();
        }

        private static void SetupAngularRouting(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.HasValue && null != Constants.AngularRoutes.FirstOrDefault(ar => context.Request.Path.Value.StartsWith(ar, StringComparison.OrdinalIgnoreCase)))
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
            var isDemo = bool.Parse(Configuration["DemoSiteMode"]);

            // Identity options.
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                if (isDemo)
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                }
                else
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                }

                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            });



            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Configuration["Authority"],
                ValidateAudience = true,
                ValidAudience = Constants.Authorization.WebApiScope,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
            var clients = Config.GetClients(Configuration);

            if (isDemo)
            {
                services.AddIdentityServer()
                    .AddDeveloperSigningCredential()
                    .AddInMemoryIdentityResources(Config.GetIdentityResources())
                    .AddInMemoryApiResources(Config.GetApiResources())
                    .AddInMemoryClients(clients)
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
                    .AddInMemoryClients(clients)
                    .AddAspNetIdentity<ApplicationUser>()
                    .AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                    .AddSigningCredential(cert)
                    .AddProfileService<IdentityWithAdditionalClaimsProfileService>()
                    .AddOperationalStore<AppDbContext>(options =>
                        {
                            options.EnableTokenCleanup = true;
                        }
                    );
                var key = new X509SecurityKey(cert);
                tokenValidationParameters.IssuerSigningKey = key;
                tokenValidationParameters.ValidateIssuerSigningKey = true;
            }

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Constants.Authorization.AuthenticateScheme;
                options.DefaultChallengeScheme = Constants.Authorization.AuthenticateScheme;
                options.DefaultForbidScheme = "Identity.Application";
            }).AddJwtBearer(options =>
                {
                    // name of the API resource
                    options.Audience = Constants.Authorization.WebApiScope;
                    options.Authority = Configuration["Authority"];
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = tokenValidationParameters;
                });

            services.AddAuthorization(Config.CreateAuthorizationOptions);
        }

        private static IEdmModel SetupODataEntities(IServiceProvider serviceProvider)
        {
            var builder = new ODataConventionModelBuilder(serviceProvider);
            builder.EntitySet<ClientView>("Clients");
            builder.EntitySet<ProjectView>("Projects");
            builder.EntitySet<MemberView>("Members");
            builder.EntitySet<MemberProjectRoleView>("MemberProjectRoles");
            builder.EntitySet<ProjectRoleView>("ProjectRoles");
            builder.EntitySet<TaskTypeView>("Tasks");
            builder.EntitySet<ErrorODataView>("Errors");
            builder.EntitySet<SettingsView>("Settings");
            builder.EntitySet<ManagerProjectsView>("ManagerProjects");
            builder.EntitySet<ProjectNameView>("ProjectsNames");
            builder.EntitySet<MemberActionView>("MemberActions");
            builder.EntitySet<VstsProjectIntegrationView>("VstsProjectIntegration");
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