using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace CoralTime.DAL
{
    public partial class AppDbContext
    {
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            using (var db = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var sqlDb = db.Database;
                
                if (sqlDb != null)
                {
                    // async method doesn't work for MySQL
                    await db.Database.MigrateAsync();
                    
                    await InitializeRoles(serviceProvider);
                    await InitializeProjectRoles(db);
                    await InitializeSettings(serviceProvider, db);

                    if (bool.Parse(config["AddTasks"]))
                    {
                        await InitializeTaskTypes(serviceProvider, db);
                    }

                    if (bool.Parse(config["AddAdmins"]))
                    {
                        await InitializeAdmins(serviceProvider, db);
                    }

                    if (bool.Parse(config["AddMembers"]))
                    {
                        await InitializeMembers(serviceProvider, db);
                    }

                    if (bool.Parse(config["AddClients"]))
                    {
                        await InitializeClients(serviceProvider, db);
                    }

                    if (bool.Parse(config["AddProjects"]))
                    {
                        await InitializeProjects(serviceProvider, db);
                    }
                }
            }
        }

        private static async Task InitializeRoles(IServiceProvider provider)
        {
            var roleManager = provider.GetRequiredService<RoleManager<IdentityRole>>();

            var defaultRoles = Constants.GetDefaultRoles();
            foreach (var role in defaultRoles)
            {
                if (await roleManager.RoleExistsAsync(role))
                {
                    continue;
                }

                var newRole = new IdentityRole(role);
                await roleManager.CreateAsync(newRole);
            }
        }

        private static async Task InitializeProjectRoles(AppDbContext db)
        {
            if (!await db.ProjectRoles.AnyAsync())
            {
                var projectManagerRole = new ProjectRole {Name = Constants.ManagerRole};
                var projectUserRole = new ProjectRole {Name = Constants.MemberRole};

                await db.ProjectRoles.AddAsync(projectManagerRole);
                await db.ProjectRoles.AddAsync(projectUserRole);

                //db.ProjectRoles.Add(new ProjectRole { Name = Constants.MemberRole });
                //db.SaveChanges();

                await db.SaveChangesAsync();
            }
        }

        private static async Task InitializeSettings(IServiceProvider serviceProvider, AppDbContext db)
        {
            if (!await db.Settings.AnyAsync())
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var settings = config.GetSection("Settings").GetChildren();
                foreach (var setting in settings)
                {
                    var newSetting = new Setting();
                    if (setting["Name"] == "DefaulProjectRoleId")
                    {
                        var memberRoleId = (await db.ProjectRoles.FirstAsync(r => r.Name == Constants.MemberRole)).Id;
                        newSetting.Name = setting["Name"];
                        newSetting.Value = memberRoleId.ToString();
                    }
                    else
                    {
                        newSetting.Name = setting["Name"];
                        newSetting.Value = setting["Value"];
                    }
                    await db.Settings.AddAsync(newSetting);
                }

                await db.SaveChangesAsync();
            }
        }

        private static async Task InitializeTaskTypes(IServiceProvider serviceProvider, AppDbContext db)
        {
            if (!await db.TaskTypes.AnyAsync())
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var tasks = config.GetSection("Tasks").GetChildren();
                foreach (var task in tasks)
                {
                    var newTask = new TaskType
                    {
                        Name = task["Name"], 
                        IsActive = bool.Parse(task["IsActive"])
                    };

                    await db.TaskTypes.AddAsync(newTask);
                }

                await db.SaveChangesAsync();
            }
        }

        private static async Task InitializeAdmins(IServiceProvider serviceProvider, AppDbContext db)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var users = config.GetSection("Users:Admins").GetChildren();
            foreach (var user in users)
            {
                var userName = user["UserName"];
                var userPassword = user["Password"];
                if (string.IsNullOrWhiteSpace(userPassword))
                {
                    userPassword = "A" + Guid.NewGuid();
                }

                var userEmail = user["Email"];
                var userFullName = user["FullName"];
                var newUser = await userManager.FindByNameAsync(userName);

                if (newUser != null)
                {
                    continue;
                }

                newUser = new ApplicationUser
                {
                    UserName = userName,
                    Email = userEmail,
                    IsAdmin = true,
                    IsManager = false,
                    IsActive = true
                };

                var userCreationResult = await userManager.CreateAsync(newUser, userPassword);
                if (!userCreationResult.Succeeded)
                {
                    continue;
                }

                var adminUser = await userManager.FindByNameAsync(newUser.UserName);
                await userManager.AddToRoleAsync(adminUser, Constants.AdminRole);

                var newMember = new Member
                {
                    UserId = adminUser.Id,
                    FullName = userFullName
                };

                await db.Members.AddAsync(newMember);
                await db.SaveChangesAsync();

                // Assigns claims.
                var claims = ClaimsCreator.GetUserClaims(adminUser.UserName, userFullName, userEmail, Constants.AdminRole, newMember.Id);
                await userManager.AddClaimsAsync(adminUser, claims);
            }
        }

        private static async Task InitializeMembers(IServiceProvider serviceProvider, AppDbContext db)
        {
            var config = serviceProvider.GetRequiredService<IConfiguration>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var users = config.GetSection("Users:Members").GetChildren();
            foreach (var user in users)
            {
                var userName = user["UserName"];
                var userPass = user["Password"];
                if (string.IsNullOrWhiteSpace(userPass))
                {
                    userPass = "A" + Guid.NewGuid();
                }

                var userEmail = user["Email"];
                var userFullName = user["FullName"];
                var newUser = await userManager.FindByNameAsync(userName);

                if (newUser != null)
                {
                    continue;
                }

                newUser = new ApplicationUser
                {
                    UserName = userName,
                    Email = userEmail,
                    IsAdmin = false,
                    IsManager = false,
                    IsActive = true
                };

                var userCreationResult = await userManager.CreateAsync(newUser, userPass);
                if (!userCreationResult.Succeeded)
                {
                    continue;
                }

                var memberUser = await userManager.FindByNameAsync(newUser.UserName);
                await userManager.AddToRoleAsync(memberUser, Constants.UserRole);

                var newMember = new Member
                {
                    UserId = memberUser.Id,
                    FullName = userFullName
                };

                await db.Members.AddAsync(newMember);
                await db.SaveChangesAsync();

                // Assigns claims.
                var claims = ClaimsCreator.GetUserClaims(memberUser.UserName, userFullName, userEmail, Constants.UserRole, newMember.Id);
                await userManager.AddClaimsAsync(memberUser, claims);
            }
        }

        private static async Task InitializeClients(IServiceProvider serviceProvider, AppDbContext db)
        {
            if (!await db.Clients.AnyAsync())
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var clients = config.GetSection("Clients").GetChildren();
                foreach (var client in clients)
                {
                    var newClient = new Client
                    {
                        Name = client["Name"],
                        Description = client["Description"],
                        IsActive = bool.Parse(client["IsActive"]),
                        Email = client["Email"],
                    };
                    await db.Clients.AddAsync(newClient);
                }
                await db.SaveChangesAsync();
            }
        }

        private static async Task InitializeProjects(IServiceProvider serviceProvider, AppDbContext db)
        {
            if (!await db.Projects.AnyAsync())
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                var projects = config.GetSection("Projects").GetChildren();
                foreach (var project in projects)
                {
                    var newProject = new Project
                    {
                        Name = project["Name"],
                        CreationDate = Convert.ToDateTime(project["CreationDate"]),
                        LastUpdateDate = Convert.ToDateTime(project["LastUpdateDate"]),
                        DaysBeforeStopEditTimeEntries = int.Parse(project["DaysBeforeStopEditTimeEntries"]),
                        IsActive = bool.Parse(project["IsActive"]),
                        IsPrivate = bool.Parse(project["IsPrivate"]),
                        ClientId = int.Parse(project["ClientId"])
                    };

                    await db.Projects.AddAsync(newProject);
                }

                await db.SaveChangesAsync();
            }
        }
    }
}