using System.Linq;
using AutoMapper;
using CoralTime.Common.Constants;
using CoralTime.DAL.Models;
using TimeEntryView = CoralTime.ViewModels.TimeEntries.TimeEntryView;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        private static TimeEntryView GetView(this TimeEntry timeEntry, IMapper mapper)
        {
            var tEntry = mapper.Map<TimeEntry, TimeEntryView>(timeEntry);
            return tEntry;
        }

        public static TimeEntryView GetView(this TimeEntry timeEntry, string userName, IMapper mapper)
        {
            var tEntryView = timeEntry.GetView(mapper);

            if (timeEntry.Project != null)
            {
                tEntryView.IsUserManagerOnProject = timeEntry.Project.MemberProjectRoles == null
                    ? false
                    : timeEntry.Project.MemberProjectRoles.Any(r => r.Member?.User.UserName == userName && r.Role?.Name == Constants.ProjectRoleManager);
            }

            return tEntryView;
        }
    }
}