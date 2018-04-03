using CoralTime.Common.Constants;
using CoralTime.Common.Helpers;
using CoralTime.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoralTime.DAL
{
    public partial class AppDbContext
    {
        private static IConfiguration Configuration { get; set; }

        private static IServiceProvider ServiceProvider { get; set; }

        private static AppDbContext DbContext { get; set; }

        private static UserManager<ApplicationUser> UserManager { get; set; }

        private static RoleManager<IdentityRole> RoleManager { get; set; }

        private static int ProjectRoleManagerId { get; set; }

        public static async Task InitializeFirstTimeDataBaseAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            using (DbContext = serviceProvider.GetRequiredService<AppDbContext>())
            {
                var isExistDataBase = (DbContext.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists();

                if (!isExistDataBase)
                {
                    await InitializeDataBase(serviceProvider, configuration);
                }
            }
        }

        public static async Task InitializeDataBase(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            Configuration = configuration;
            ServiceProvider = serviceProvider;
            UserManager = ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager = ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var sqlDb = DbContext.Database;

            if (sqlDb != null)
            {
                //async method doesn't work for MySQL
                await DbContext.Database.MigrateAsync();

                await InitializeRoles();
                await InitializeProjectRoles();
                await InitializeSettings();

                if (bool.Parse(Configuration["AddTasks"]))
                {
                    await InitializeTaskTypes();
                }

                if (bool.Parse(Configuration["AddAdmins"]))
                {
                    await InitializeUsers(Constants.UserTypeAdmins, Constants.ApplicationRoleAdmin);
                }

                if (bool.Parse(Configuration["AddMembers"]))
                {
                    await InitializeUsers(Constants.UserTypeMembers, Constants.ApplicationRoleUser);
                }

                if (bool.Parse(Configuration["AddClients"]))
                {
                    await InitializeClients();
                }

                if (bool.Parse(Configuration["AddProjects"]))
                {
                    await InitializeProjects();
                }

                if (bool.Parse(Configuration["AddXRefProjectsClients"]))
                {
                    await InitializeXRefProjectsClients();
                }

                if (bool.Parse(Configuration["AddXRefMemberProjectRoles"]))
                {
                    await InitializeXRefMemberProjectRoles();
                }

                if (bool.Parse(Configuration["AddXRefTimeEntries"]))
                {
                    await InitializeXRefTimeEntries();
                }
            }
        }

        private static async Task InitializeRoles()
        {
            foreach (var aspNetRole in Constants.ApplicationRoles)
            {
                var aspNetRoleNew = new IdentityRole(aspNetRole);
                var aspNetRoleDb = await RoleManager.FindByNameAsync(aspNetRoleNew.Name);
                if (aspNetRoleDb == null)
                {
                    var aspNetRoleCreateResult =  await RoleManager.CreateAsync(aspNetRoleNew);
                    if (!aspNetRoleCreateResult.Succeeded)
                    {
                        continue;
                    }
                }
            }
        }

        private static async Task InitializeProjectRoles()
        {
            var projectRoleList = new List<ProjectRole>();

            foreach (var projectRole in Constants.ProjectRoles)
            {
                var projectRoleManager = await DbContext.ProjectRoles.FirstOrDefaultAsync(x => x.Name == projectRole);
                if (projectRoleManager == null)
                {
                    projectRoleList.Add(new ProjectRole {Name = projectRole});
                }
            }

            await InsertListOfEntitiesToDbAsync(DbContext.ProjectRoles, projectRoleList);
        }

        private static async Task InitializeSettings()
        {
            var settings = Configuration.GetSection("Settings").GetChildren();

            var settingsList = new List<Setting>();

            foreach (var setting in settings)
            {
                var settingName = setting["Name"];

                var settingsDb = await DbContext.Settings.FirstOrDefaultAsync(x => x.Name == settingName);
                if (settingsDb == null)
                {

                    var newSetting = new Setting();
                    if (setting["Name"] == "DefaulProjectRoleId")
                    {
                        var projectRoleMemberId = (await DbContext.ProjectRoles.FirstAsync(r => r.Name == Constants.ProjectRoleMember)).Id;
                        newSetting.Name = setting["Name"];
                        newSetting.Value = projectRoleMemberId.ToString();
                    }
                    else
                    {
                        newSetting.Name = setting["Name"];
                        newSetting.Value = setting["Value"];
                    }

                    settingsList.Add(newSetting);
                }
            }

            await InsertListOfEntitiesToDbAsync(DbContext.Settings, settingsList);
        }

        private static async Task InitializeTaskTypes()
        {
            var tasksList = await CreateListOfEntitiesFromConfigByNameAsync(DbContext.TaskTypes, "Tasks");

            await InsertListOfEntitiesToDbAsync(DbContext.TaskTypes, tasksList);
        }

        private static async Task InitializeUsers(string typeUser, string roleUser)
        {
            var users = Configuration.GetSection($"Users:{typeUser}").GetChildren();

            foreach (var user in users)
            {
                // Get Values
                var userName = user["UserName"];
                var userFullName = user["FullName"];
                var userPassword = user["Password"];
                if (string.IsNullOrWhiteSpace(userPassword))
                {
                    userPassword = "A" + Guid.NewGuid();
                }
                var userEmail = user["Email"];

                #region Create ApplicationUser, Roles, Claims, Member

                // Create ApplicationUser
                var applicationUser = new ApplicationUser
                {
                    UserName = userName,
                    Email = userEmail,
                    IsAdmin = roleUser == Constants.ApplicationRoleAdmin,
                    IsManager = false,
                    IsActive = bool.Parse(user["IsActive"])
                };

                // Create Member
                var member = new Member
                {
                    UserId = applicationUser.Id,
                    FullName = userFullName
                };

                #endregion

                #region Insert ApplicationUser, Roles, Claims

                // Insert ApplicationUser
                var userCreationResult = await UserManager.CreateAsync(applicationUser, userPassword);
                if (!userCreationResult.Succeeded)
                {
                    continue;
                }

                // Insert ApplicationUser Roles
                var roleForMemberResult = await UserManager.AddToRoleAsync(applicationUser, roleUser);
                if (!roleForMemberResult.Succeeded)
                {
                    continue;
                }

                #endregion

                #region Insert Member

                try
                {
                    // Insert Member
                    await DbContext.Members.AddAsync(member);

                    // Save DbContex
                    await DbContext.SaveChangesAsync();

                    // Create ApplicationUser claims 
                    var claims = ClaimsCreator.CreateUserClaims(applicationUser.UserName, userFullName, userEmail, roleUser, member.Id);

                    // Insert ApplicationUser claims 
                    var applicationUserClaimsResult = await UserManager.AddClaimsAsync(applicationUser, claims);
                    if (!applicationUserClaimsResult.Succeeded)
                    {
                        continue;
                    }

                }
                catch (Exception) { }

                #endregion
            }
        }

        private static async Task InitializeClients()
        {
            var clientsList = await CreateListOfEntitiesFromConfigByNameAsync(DbContext.Clients, "Clients");
             
            await InsertListOfEntitiesToDbAsync(DbContext.Clients, clientsList);
        }

        private static async Task InitializeProjects()
        {
            var projectList = await CreateListOfEntitiesFromConfigByNameAsync(DbContext.Projects, "Projects");
            
            await InsertListOfEntitiesToDbAsync(DbContext.Projects, projectList);
        }

        private static async Task InitializeXRefProjectsClients()
        {
            var projectsList = await GetProjectList();
            var clientsList = await GetClientList();

            var projectsWithClientsList = new List<Project>();
            var projectsWithClients = Configuration.GetSection("XRefProjectsClient").GetChildren();
            foreach (var projectWithClient in projectsWithClients)
            {
                var projectByName = projectsList.FirstOrDefault(x => x.Name == projectWithClient["ProjectName"]);
                var clientByName = clientsList.FirstOrDefault(x => x.Name == projectWithClient["ClientName"]);

                if (projectByName != null && clientByName != null)
                {
                    projectByName.ClientId = clientByName.Id;

                    projectsWithClientsList.Add(projectByName);
                }
            }

            await UpdateListOfEntitiesToDbAsync(DbContext.Projects, projectsWithClientsList);
        }

        private static async Task InitializeXRefMemberProjectRoles()
        {
            var allMembersList = await GetAllMembersList();

            var projectsList = await GetProjectList();
            var projectRolesList = await GetProjectRolesList();

            var memberProjectRoleList = new List<MemberProjectRole>();
            var memberProjectRoles = Configuration.GetSection("XRefMemberProjectRoles").GetChildren();
            foreach (var memberProjectRole in memberProjectRoles)
            {
                var mprUserByName = allMembersList.FirstOrDefault(x => x.User.UserName == memberProjectRole["UserName"]);
                var mprProjectByName = projectsList.FirstOrDefault(x => x.Name == memberProjectRole["ProjectName"]);
                var mprProjectRoleByName = projectRolesList.FirstOrDefault(x => x.Name == memberProjectRole["ProjectRoleName"]);

                if (mprUserByName != null && mprProjectByName != null && mprProjectRoleByName != null)
                {
                    var mpr = new MemberProjectRole
                    {
                        MemberId = mprUserByName.Id,
                        ProjectId = mprProjectByName.Id,
                        RoleId = mprProjectRoleByName.Id
                    };

                    memberProjectRoleList.Add(mpr);

                    UpdateIsManagerRoleForMember(ProjectRoleManagerId, memberProjectRoleList, mprUserByName, mprProjectRoleByName);
                }
            }

            await InsertListOfEntitiesToDbAsync(DbContext.MemberProjectRoles, memberProjectRoleList);
        }

        private static async Task InitializeXRefTimeEntries()
        {
            var allMembersList = await GetAllMembersList();
            var projectsList = await GetProjectList();
            var tasksList = await GetTaskTypesList();
            var memberProjectRolesList = await GetMemberProjectRoleList();

            var timeEntryList = new List<TimeEntry>();
            var timeEntries = Configuration.GetSection("XRefTimeEntries").GetChildren();
            foreach (var timeEntry in timeEntries)
            {
                var timeEntryMemberByName = allMembersList.FirstOrDefault(x => x.User.UserName == timeEntry["UserName"]);
                var timeEntryProjectByName = projectsList.FirstOrDefault(x => x.Name == timeEntry["ProjectName"]);
                var timeEntryTaskByName = tasksList.FirstOrDefault(x => x.Name == timeEntry["TaskTypeName"]);
                var timeEntryDate = CreateTimeEntryDate(int.Parse(timeEntry["DayNumberOfWeek"]));

                if (timeEntryMemberByName != null && timeEntryProjectByName != null && timeEntryTaskByName != null
                    && CanMemberCreateTimeEntry(memberProjectRolesList, timeEntryMemberByName)
                    && timeEntryDate != null)
                {
                    var newTimeEntry = new TimeEntry
                    {
                        MemberId = timeEntryMemberByName.Id,
                        ProjectId = timeEntryProjectByName.Id,
                        TaskTypesId = timeEntryTaskByName.Id,
                        Date = (DateTime) timeEntryDate,
                        TimeActual = int.Parse(timeEntry["TimeActual"]),
                        TimeEstimated = int.Parse(timeEntry["TimeEstimated"]),
                        Description = timeEntry["Description"]
                    };

                    timeEntryList.Add(newTimeEntry);
                }

                await InsertListOfEntitiesToDbAsync(DbContext.TimeEntries, timeEntryList);
            }
        }

        #region Initialize single entities by Name from config.

        private static async Task<List<TEntity>> CreateListOfEntitiesFromConfigByNameAsync<TEntity>(DbSet<TEntity> dbSet, string section) where TEntity : class, IInitializeByName, new()
        {
            var listOfEntities = new List<TEntity>();

            var entitiesFromConfig = Configuration.GetSection(section).GetChildren();

            foreach (var entity in entitiesFromConfig)
            {
                var entityFromDbByName = await dbSet.FirstOrDefaultAsync(x => x.Name == entity["Name"]);
                if (entityFromDbByName == null)
                {
                    listOfEntities.Add(CreateNewEntity<TEntity>(entity));
                }
            }

            return listOfEntities;
        }

        private static TEntity CreateNewEntity<TEntity>(IConfigurationSection entitiesFromConfig) where TEntity : class, new()
        {
            switch (new TEntity())
            {
                case Project project:
                {
                    project = new Project
                    {
                        Name = entitiesFromConfig["Name"],
                        IsActive = bool.Parse(entitiesFromConfig["IsActive"]),
                        IsPrivate = bool.Parse(entitiesFromConfig["IsPrivate"]),
                        Color = int.Parse(entitiesFromConfig["Color"])
                    };

                    return project as TEntity;
                }

                case Client client:
                {
                    client = new Client
                    {
                        Name = entitiesFromConfig["Name"],
                        Description = entitiesFromConfig["Description"],
                        IsActive = bool.Parse(entitiesFromConfig["IsActive"]),
                        Email = entitiesFromConfig["Email"],
                    };

                    return client as TEntity;
                }

                case TaskType task:
                {
                    task = new TaskType
                    {
                        Name = entitiesFromConfig["Name"],
                        IsActive = bool.Parse(entitiesFromConfig["IsActive"])
                    };

                    return task as TEntity;
                }
                default:
                {
                    return null;
                }
            }
        }

        private static async Task InsertListOfEntitiesToDbAsync<TEntity>(DbSet<TEntity> dbSet, List<TEntity> listOfEntities) where TEntity : class
        {
            try
            {
                if (listOfEntities.Count > 0)
                {
                    await dbSet.AddRangeAsync(listOfEntities);
                    await DbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                //todo log
            }
        }

        private static async Task UpdateListOfEntitiesToDbAsync<TEntity>(DbSet<TEntity> dbSet, List<TEntity> listOfEntities) where TEntity : class
        {
            try
            {
                if (listOfEntities.Count > 0)
                {
                    dbSet.UpdateRange(listOfEntities);
                    await DbContext.SaveChangesAsync();
                }
            }
            catch (Exception)
            {
                //todo log
            }
        }

        #endregion

        #region Get initialized list of entities methods

        private static async Task<List<Member>> GetAllMembersList()
        {
            var allMembersList = new List<Member>();

            allMembersList.AddRange(await GetUsersList(Constants.UserTypeAdmins));
            allMembersList.AddRange(await GetUsersList(Constants.UserTypeMembers));

            return allMembersList;
        }

        private static async Task<List<Member>> GetUsersList(string userType)
        {
            var membersList = new List<Member>();
            var members = Configuration.GetSection($"Users:{userType}").GetChildren();
            foreach (var member in members)
            {
                var getMemberByUserNameAsync = await DbContext.Members.FirstOrDefaultAsync(x => x.User.UserName == member["UserName"]);
                if (getMemberByUserNameAsync != null)
                {
                    membersList.Add(getMemberByUserNameAsync);
                }
            }

            return membersList;
        }

        private static async Task<List<Project>> GetProjectList()
        {
            var projectsList = new List<Project>();
            var projects = Configuration.GetSection("Projects").GetChildren();
            foreach (var project in projects)
            {
                var getProjectByNAmeAsync = await DbContext.Projects.FirstOrDefaultAsync(x => x.Name == project["Name"]);
                if (getProjectByNAmeAsync != null)
                {
                    projectsList.Add(getProjectByNAmeAsync);
                }
            }

            return projectsList;
        }

        private static async Task<List<Client>> GetClientList()
        {
            var clientsList = new List<Client>();
            var clients = Configuration.GetSection("Clients").GetChildren();
            foreach (var client in clients)
            {
                var getClientByNameAsync = await DbContext.Clients.FirstOrDefaultAsync(x => x.Name == client["Name"]);
                if (getClientByNameAsync != null)
                {
                    clientsList.Add(getClientByNameAsync);
                }
            }

            return clientsList;
        }

        private static async Task<List<ProjectRole>> GetProjectRolesList()
        {
            var projectRolesList = new List<ProjectRole>();
            foreach (var projectRole in Constants.ProjectRoles)
            {
                var projectRoleByNameAsync = await DbContext.ProjectRoles.FirstOrDefaultAsync(x => x.Name == projectRole);
                if (projectRoleByNameAsync != null)
                {
                    projectRolesList.Add(projectRoleByNameAsync);

                    if (projectRoleByNameAsync.Name == Constants.ProjectRoleManager)
                    {
                        ProjectRoleManagerId = projectRoleByNameAsync.Id;
                    }
                }
            }

            return projectRolesList;
        }

        private static async Task<List<TaskType>> GetTaskTypesList()
        {
            var tasksList = new List<TaskType>();
            var tasks = Configuration.GetSection("Tasks").GetChildren();
            foreach (var task in tasks)
            {
                var taskDb = await DbContext.TaskTypes.FirstOrDefaultAsync(x => x.Name == task["Name"]);
                if (taskDb != null)
                {
                    tasksList.Add(taskDb);
                }
            }

            return tasksList;
        }

        private static async Task<List<MemberProjectRole>> GetMemberProjectRoleList()
        {
            var memberProjectRolesList = new List<MemberProjectRole>();
            var memberProjectRoles = Configuration.GetSection("XRefMemberProjectRoles").GetChildren();
            foreach (var memberProjectRole in memberProjectRoles)
            {
                var mprDb = await DbContext.MemberProjectRoles.FirstOrDefaultAsync(x =>
                    x.Member.User.UserName == memberProjectRole["UserName"] &&
                    x.Project.Name == memberProjectRole["ProjectName"] &&
                    x.Role.Name == memberProjectRole["ProjectRoleName"]);

                if(mprDb != null)
                {
                    memberProjectRolesList.Add(mprDb);
                }
            }

            return memberProjectRolesList;
        }

        #endregion

        #region Additional methods 

        private static void UpdateIsManagerRoleForMember(int projectRoleManagerId, List<MemberProjectRole> memberProjectRoleList, Member mprUser, ProjectRole mprProjectRole)
        {
            var checkManagerRoleFromDb = memberProjectRoleList.Exists(x => x.MemberId == mprUser.Id && x.RoleId == projectRoleManagerId);
            var checkManagerRoleFromConfig = mprProjectRole.Name == Constants.ProjectRoleManager;

            mprUser.User.IsManager = checkManagerRoleFromDb || checkManagerRoleFromConfig;

            DbContext.Members.Update(mprUser);
        }

        private static bool CanMemberCreateTimeEntry(List<MemberProjectRole> memberProjectRolesList, Member memberByName)
        {
            var isMemberAssignAtProject = memberProjectRolesList.Exists(x => x.MemberId == memberByName.Id);

            return memberByName.User.IsAdmin || isMemberAssignAtProject;
        }

        private static DateTime? CreateTimeEntryDate(int timeEntryDAyOfWeek)
        {
            DateTime? timeEntryDate = null;

            if (0 <= timeEntryDAyOfWeek && timeEntryDAyOfWeek < 7)
            {
                timeEntryDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, ConvertNumberOfDayToDayOfThisWeek(timeEntryDAyOfWeek));
            }

            return timeEntryDate;
        }

        private static int ConvertNumberOfDayToDayOfThisWeek(int timeEntryDayNumberOfWeek)
        {
            CommonHelpers.SetRangeOfThisWeekByDate(out var weekByTodayFirstDate, out var weekByTodayLastDate, DateTime.Today);
            var dayOfThisWeek = weekByTodayFirstDate.AddDays(timeEntryDayNumberOfWeek).Day;
            return dayOfThisWeek;
        }

        #endregion
    }
}