using CoralTime.DAL.Models;
using System.Collections.Generic;
using System.Text;
using CoralTime.DAL.Models.Member;
using CoralTime.DAL.Models.ReportsSettings;

namespace CoralTime.DAL.Cache
{
    public static class CancelationTokenSources
    {
        private const string Prefix = "cts_";
        private static readonly string ProjectClassName = new StringBuilder(Prefix + typeof(Project).Name).ToString();
        private static readonly string ClientClassName = new StringBuilder(Prefix + typeof(Client).Name).ToString();
        private static readonly string MemberClassName = new StringBuilder(Prefix + typeof(Member).Name).ToString();
        private static readonly string TaskTypeClassName = new StringBuilder(Prefix + typeof(TaskType).Name).ToString();
        private static readonly string TimeEntryClassName = new StringBuilder(Prefix + typeof(TimeEntry).Name).ToString();
        private static readonly string MemberProjectRoleClassName = new StringBuilder(Prefix + typeof(MemberProjectRole).Name).ToString();
        private static readonly string ApplicationUserClassName = new StringBuilder(Prefix + typeof(ApplicationUser).Name).ToString();
        private static readonly string MemberAvatarClassName = new StringBuilder(Prefix + typeof(MemberImage).Name).ToString();
        private static readonly string ReportsSettingsClassName = new StringBuilder(Prefix + typeof(ReportsSettings).Name).ToString();

        public static List<string> GetCancelationTokenSourcesNames() => new List<string>
        {
            ProjectClassName,
            ClientClassName,
            MemberClassName,
            TaskTypeClassName,
            TimeEntryClassName,
            MemberProjectRoleClassName,
            ApplicationUserClassName,
            MemberAvatarClassName,
            ReportsSettingsClassName
        };
    
        public static List<string> GetNamesCancelationTokenSourcesForType<T>()
        {
            var names = new List<string>();
            var typeName = new StringBuilder(Prefix + typeof(T).Name).ToString();

            // Has relation with AppUser 
            if (typeName == ApplicationUserClassName)
            {
                names.AddRange(new List<string> { typeName, MemberClassName });
            }

            if (typeName == ProjectClassName)
            {
                names.AddRange(new List<string> { typeName, ApplicationUserClassName, ClientClassName, MemberProjectRoleClassName, MemberClassName, TaskTypeClassName });
            }

            if (typeName == MemberProjectRoleClassName)
            {
                names.AddRange(new List<string> { typeName, ProjectClassName, MemberClassName, ApplicationUserClassName });
            }

            // Has relation with Member 
            if (typeName == MemberClassName)
            {
                names.AddRange(new List<string> { typeName, ApplicationUserClassName });
            }

            if (typeName == TimeEntryClassName)
            {
                names.AddRange(new List<string> { typeName, ProjectClassName, TaskTypeClassName, MemberClassName, MemberProjectRoleClassName, ApplicationUserClassName });
            }

            if (typeName == MemberAvatarClassName)
            {
                names.AddRange(new List<string> { typeName, MemberClassName });
            }

            if (typeName == ReportsSettingsClassName)
            {
                names.AddRange(new List<string> { typeName, MemberClassName });
            }

            // Separate Entities
            if (typeName == TaskTypeClassName)
            {
                names.AddRange(new List<string> { typeName, ProjectClassName });
            }

            if (typeName == ClientClassName)
            {
                names.AddRange(new List<string> { typeName });
            }

            return names;
        }
    }
}