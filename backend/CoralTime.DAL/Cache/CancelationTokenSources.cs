using CoralTime.DAL.Models;
using System.Collections.Generic;

namespace CoralTime.DAL.Cache
{
    public static class CancelationTokenSources
    {
        private const string Prefix = "cts_";
        private static readonly string ProjectClassName = Prefix + typeof(Project).Name;
        private static readonly string ClientClassName = Prefix + typeof(Client).Name;
        private static readonly string MemberClassName = Prefix + typeof(Member).Name;
        private static readonly string TaskTypeClassName = Prefix + typeof(TaskType).Name;
        private static readonly string TimeEntryClassName = Prefix + typeof(TimeEntry).Name;
        private static readonly string MemberProjectRoleClassName = Prefix + typeof(MemberProjectRole).Name;
        private static readonly string ApplicationUserClassName = Prefix + typeof(ApplicationUser).Name;
        private static readonly string MemberAvatarClassName = Prefix + typeof(MemberImage).Name;
        private static readonly string ReportsSettingsClassName = Prefix + typeof(ReportsSettings).Name;

        public static List<string> GetCancelationTokenSourcesNames()
        {
            return new List<string>
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
        }

        public static List<string> GetNamesCancelationTokenSourcesForType<T>()
        {
            var names = new List<string>();
            var typeName = Prefix + typeof(T).Name;

            if (typeName == ProjectClassName)
            {
                names.AddRange(new List<string> { typeName, ApplicationUserClassName, ClientClassName, MemberProjectRoleClassName, MemberClassName, TaskTypeClassName });
            }

            if (typeName == ClientClassName)
            {
                names.AddRange(new List<string> { typeName });
            }

            if (typeName == MemberClassName)
            {
                names.AddRange(new List<string> { typeName, ApplicationUserClassName });
            }

            if (typeName == TaskTypeClassName)
            {
                names.AddRange(new List<string> { typeName, ProjectClassName });
            }

            if (typeName == TimeEntryClassName)
            {
                names.AddRange(new List<string> { typeName, ProjectClassName, TaskTypeClassName, MemberClassName, MemberProjectRoleClassName, ApplicationUserClassName });
            }

            if (typeName == MemberProjectRoleClassName)
            {
                names.AddRange(new List<string> { typeName, ProjectClassName, MemberClassName, ApplicationUserClassName });
            }

            if (typeName == ApplicationUserClassName)
            {
                names.AddRange(new List<string> { typeName, MemberClassName });
            }

            if (typeName == MemberAvatarClassName)
            {
                names.AddRange(new List<string> { typeName, MemberClassName });
            }

            if (typeName == ReportsSettingsClassName)
            {
                names.AddRange(new List<string> { typeName, MemberClassName });
            }

            return names;
        }
    }
}