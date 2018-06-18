using System.Linq;
using AutoMapper;
using CoralTime.Common.Constants;
using CoralTime.DAL.Models;

namespace CoralTime.DAL.ConvertModelToView
{
    public static partial class ConvertModelToView
    {
        private static ViewModels.TimeEntries.TimeEntryView GetView(this TimeEntry timeEntry, IMapper _mapper)
        {
            var tEntry = _mapper.Map<TimeEntry, ViewModels.TimeEntries.TimeEntryView>(timeEntry);
            return tEntry;
        }

        public static ViewModels.TimeEntries.TimeEntryView GetView(this TimeEntry timeEntry, string userName, IMapper _mapper)
        {
            var tEntryView = timeEntry.GetView(_mapper);

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