namespace CoralTime.DAL.Repositories
{
    public partial class UnitOfWork
    {
        #region ProjectRepository

        private ProjectRepository _projectRepository;

        public ProjectRepository ProjectRepository => _projectRepository ?? (_projectRepository = new ProjectRepository(_context, _memoryCache, _userId));

        #endregion

        #region ProjectRoleRepository

        private ProjectRoleRepository _projectRoleRepository;

        public ProjectRoleRepository ProjectRoleRepository => _projectRoleRepository ?? (_projectRoleRepository = new ProjectRoleRepository(_context, _memoryCache, _userId));

        #endregion

        #region TimeEntryRepository

        private TimeEntryRepository _timeEntryRepository;

        public TimeEntryRepository TimeEntryRepository => _timeEntryRepository ?? (_timeEntryRepository = new TimeEntryRepository(_context, _memoryCache, _userId));

        #endregion

        #region ClientRepository

        private ClientRepository _clientRepository;

        public ClientRepository ClientRepository => _clientRepository ?? (_clientRepository = new ClientRepository(_context, _memoryCache, _userId));

        #endregion

        #region TaskTypeRepository

        private TaskTypeRepository _taskTypeRepository;

        public TaskTypeRepository TaskTypeRepository => _taskTypeRepository ?? (_taskTypeRepository = new TaskTypeRepository(_context, _memoryCache, _userId));

        #endregion

        #region MemberRepository

        private MemberRepository _memberRepository;

        public MemberRepository MemberRepository => _memberRepository ?? (_memberRepository = new MemberRepository(_context, _memoryCache, _userId));

        #endregion 

        #region UserRepository

        private UserRepository _userRepository;

        public UserRepository UserRepository => _userRepository ?? (_userRepository = new UserRepository(_context, _memoryCache, _userId));

        #endregion

        #region MemberProjectRoleRepository

        private MemberProjectRoleRepository _memberProjectRoleRepository;

        public MemberProjectRoleRepository MemberProjectRoleRepository => _memberProjectRoleRepository ?? (_memberProjectRoleRepository = new MemberProjectRoleRepository(_context, _memoryCache, _userId));

        #endregion

        #region UserForgotPassRequestRepository

        private UserForgotPassRequestRepository _userForgotPassRequestRepository;

        public UserForgotPassRequestRepository UserForgotPassRequestRepository => _userForgotPassRequestRepository ?? (_userForgotPassRequestRepository = new UserForgotPassRequestRepository(_context, _memoryCache, _userId));

        #endregion

        #region MemberImageRepository

        private MemberImageRepository _memberImageRepository;

        public MemberImageRepository MemberImageRepository => _memberImageRepository ?? (_memberImageRepository = new MemberImageRepository(_context, _memoryCache, _userId));

        #endregion

        #region ReportsSettingsRepository

        private ReportsSettingsRepository _reportsSettingsRepository;

        public ReportsSettingsRepository ReportsSettingsRepository => _reportsSettingsRepository ?? (_reportsSettingsRepository = new ReportsSettingsRepository(_context, _memoryCache, _userId));

        #endregion
    }
}