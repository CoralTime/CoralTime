namespace CoralTime.DAL.Repositories
{
    public partial class UnitOfWork
    {
        #region ProjectRepository

        private ProjectRepository _projectRepository;

        public ProjectRepository ProjectRepository => _projectRepository ?? (_projectRepository = new ProjectRepository(_context, _memoryCache, _userId));

        #endregion ProjectRepository

        #region ProjectRoleRepository

        private ProjectRoleRepository _projectRoleRepository;

        public ProjectRoleRepository ProjectRoleRepository => _projectRoleRepository ?? (_projectRoleRepository = new ProjectRoleRepository(_context, _memoryCache, _userId));

        #endregion ProjectRoleRepository

        #region TimeEntryRepository

        private TimeEntryRepository _timeEntryRepository;

        public TimeEntryRepository TimeEntryRepository => _timeEntryRepository ?? (_timeEntryRepository = new TimeEntryRepository(_context, _memoryCache, _userId));

        #endregion TimeEntryRepository

        #region ClientRepository

        private ClientRepository _clientRepository;

        public ClientRepository ClientRepository => _clientRepository ?? (_clientRepository = new ClientRepository(_context, _memoryCache, _userId));

        #endregion ClientRepository

        #region TaskTypeRepository

        private TaskTypeRepository _taskTypeRepository;

        public TaskTypeRepository TaskTypeRepository => _taskTypeRepository ?? (_taskTypeRepository = new TaskTypeRepository(_context, _memoryCache, _userId));

        #endregion TaskTypeRepository

        #region MemberRepository

        private MemberRepository _memberRepository;

        public MemberRepository MemberRepository => _memberRepository ?? (_memberRepository = new MemberRepository(_context, _userManager, _memoryCache, _userId));

        #endregion MemberRepository

        #region UserRepository

        private UserRepository _userRepository;

        public UserRepository UserRepository => _userRepository ?? (_userRepository = new UserRepository(_context, _userManager, _memoryCache, _userId));

        #endregion UserRepository

        #region MemberProjectRoleRepository

        private MemberProjectRoleRepository _memberProjectRoleRepository;

        public MemberProjectRoleRepository MemberProjectRoleRepository => _memberProjectRoleRepository ?? (_memberProjectRoleRepository = new MemberProjectRoleRepository(_context, _memoryCache, _userId));

        #endregion MemberProjectRoleRepository

        //#region SettingsRepository

        //private SettingsRepository _settingsRepository;

        //public SettingsRepository SettingsRepository => _settingsRepository ?? (_settingsRepository = new SettingsRepository(_context, _memoryCache, _userId));

        //#endregion SettingsRepository

        #region UserForgotPassRequestRepository

        private UserForgotPassRequestRepository _userForgotPassRequestRepository;

        public UserForgotPassRequestRepository UserForgotPassRequestRepository => _userForgotPassRequestRepository ?? (_userForgotPassRequestRepository = new UserForgotPassRequestRepository(_context, _memoryCache, _userId));

        #endregion UserForgotPassRequestRepository

        #region MemberPicRepository

        private MemberAvatarRepository _memberAvatarRepository;

        public MemberAvatarRepository MemberAvatarRepository => _memberAvatarRepository ?? (_memberAvatarRepository = new MemberAvatarRepository(_context, _memoryCache, _userId));

        #endregion MemberPicRepository

    }
}