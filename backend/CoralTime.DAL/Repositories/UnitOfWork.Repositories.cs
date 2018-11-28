using CoralTime.DAL.Repositories.Member;
using CoralTime.DAL.Repositories.User;
using CoralTime.DAL.Repositories.Vsts;

namespace CoralTime.DAL.Repositories
{
    public partial class UnitOfWork
    {
        #region List of Repositories

        private ProjectRepository _projectRepository;

        public ProjectRepository ProjectRepository => _projectRepository ?? (_projectRepository = new ProjectRepository(AppDbContext, MemoryCache, UserId));

        private ProjectRoleRepository _projectRoleRepository;

        public ProjectRoleRepository ProjectRoleRepository => _projectRoleRepository ?? (_projectRoleRepository = new ProjectRoleRepository(AppDbContext, MemoryCache, UserId));

        private TimeEntryRepository _timeEntryRepository;

        public TimeEntryRepository TimeEntryRepository => _timeEntryRepository ?? (_timeEntryRepository = new TimeEntryRepository(AppDbContext, MemoryCache, UserId));

        private ClientRepository _clientRepository;

        public ClientRepository ClientRepository => _clientRepository ?? (_clientRepository = new ClientRepository(AppDbContext, MemoryCache, UserId));

        private TaskTypeRepository _taskTypeRepository;

        public TaskTypeRepository TaskTypeRepository => _taskTypeRepository ?? (_taskTypeRepository = new TaskTypeRepository(AppDbContext, MemoryCache, UserId));

        private MemberRepository _memberRepository;

        public MemberRepository MemberRepository => _memberRepository ?? (_memberRepository = new MemberRepository(AppDbContext, MemoryCache, UserId));

        private UserRepository _userRepository;

        public UserRepository UserRepository => _userRepository ?? (_userRepository = new UserRepository(AppDbContext, MemoryCache, UserId));

        private MemberProjectRoleRepository _memberProjectRoleRepository;

        public MemberProjectRoleRepository MemberProjectRoleRepository => _memberProjectRoleRepository ?? (_memberProjectRoleRepository = new MemberProjectRoleRepository(AppDbContext, MemoryCache, UserId));

        private UserForgotPassRequestRepository _userForgotPassRequestRepository;

        public UserForgotPassRequestRepository UserForgotPassRequestRepository => _userForgotPassRequestRepository ?? (_userForgotPassRequestRepository = new UserForgotPassRequestRepository(AppDbContext, MemoryCache, UserId));

        private MemberImageRepository _memberImageRepository;

        public MemberImageRepository MemberImageRepository => _memberImageRepository ?? (_memberImageRepository = new MemberImageRepository(AppDbContext, MemoryCache, UserId));

        private ReportsSettingsRepository _reportsSettingsRepository;

        public ReportsSettingsRepository ReportsSettingsRepository => _reportsSettingsRepository ?? (_reportsSettingsRepository = new ReportsSettingsRepository(AppDbContext, MemoryCache, UserId));

        private MemberActionRepository _memberActionRepository;

        public MemberActionRepository MemberActionRepository => _memberActionRepository ?? (_memberActionRepository = new MemberActionRepository(AppDbContext, MemoryCache, UserId));

        private VstsProjectRepository _vstsProjectRepository;

        public VstsProjectRepository VstsProjectRepository => _vstsProjectRepository ?? (_vstsProjectRepository = new VstsProjectRepository(AppDbContext, MemoryCache, UserId));

        private VstsUserRepository _vstsUserRepository;

        public VstsUserRepository VstsUserRepository => _vstsUserRepository ?? (_vstsUserRepository = new VstsUserRepository(AppDbContext, MemoryCache, UserId));

        private VstsProjectUserRepository _vstsProjectUserRepository;

        public VstsProjectUserRepository VstsProjectUserRepository => _vstsProjectUserRepository ?? (_vstsProjectUserRepository = new VstsProjectUserRepository(AppDbContext, MemoryCache, UserId));

        #endregion List of Repositories
    }
}