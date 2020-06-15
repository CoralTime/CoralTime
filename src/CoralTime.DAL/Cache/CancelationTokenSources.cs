using CoralTime.DAL.Models;
using System.Collections.Generic;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Models.ReportsSettings;

namespace CoralTime.DAL.Cache
{
    public static class CancelationTokenSources
    {
        private const string Prefix = "cts_";
        private const string ProjectClassName = Prefix + nameof(Project);
        private const string ClientClassName = Prefix + nameof(Client);
        private const string MemberClassName = Prefix + nameof(Member);
        private const string TaskTypeClassName = Prefix + nameof(TaskType);
        private const string TimeEntryClassName = Prefix + nameof(TimeEntry);
        private const string MemberProjectRoleClassName = Prefix + nameof(MemberProjectRole);
        private const string ApplicationUserClassName = Prefix + nameof(ApplicationUser);
        private const string ReportsSettingsClassName = Prefix + nameof(ReportsSettings);
        private const string MemberImageClassName = Prefix + nameof(MemberImage);
        private const string ProjectRoleClassName = Prefix + nameof(ProjectRole);

        public static List<string> GetCancelationTokenSourcesNames { get; } = new List<string>()
        {
            ProjectClassName,
            ClientClassName,
            MemberClassName,
            TaskTypeClassName,
            TimeEntryClassName,
            MemberProjectRoleClassName,
            ApplicationUserClassName,
            ReportsSettingsClassName,
            MemberImageClassName,
            ProjectRoleClassName
        }; 

        public static List<string> GetNamesCancelationTokenSourcesForType<T>()
        {
            var typeName = Prefix + typeof(T).Name;
            
            var names = new List<string> {typeName};
            
            switch (typeName)
            {
                   case ProjectClassName:
                       names.AddRange(new List<string> { ApplicationUserClassName, ClientClassName, MemberProjectRoleClassName, MemberClassName, TaskTypeClassName });
                       break;
                   
                   case MemberProjectRoleClassName:
                       names.AddRange(new List<string> { ProjectClassName, MemberClassName, ApplicationUserClassName, ClientClassName, TaskTypeClassName});
                       break;
                   
                   case MemberClassName:
                       names.Add(ApplicationUserClassName);
                       break;
                   
                   case TimeEntryClassName:
                       names.AddRange(new List<string> { ProjectClassName, TaskTypeClassName, MemberClassName, MemberProjectRoleClassName, ApplicationUserClassName });
                       break;
                   
                   case ReportsSettingsClassName:
                       names.Add(MemberClassName);
                       break;
                   
                   case TaskTypeClassName:
                       names.Add(ProjectClassName);
                       break;
            }

            return names;
        }
    }
}