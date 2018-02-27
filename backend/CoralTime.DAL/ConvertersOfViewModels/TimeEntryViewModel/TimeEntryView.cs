using AutoMapper;
using CoralTime.Common.Constants;
using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.TimeEntries;
using System.Linq;

namespace CoralTime.DAL.ConvertersOfViewModels
{
    public static partial class ConvertersOfViewModels
    {
        public static TimeEntryView GetView(this TimeEntry timeEntry, IMapper _mapper)
        {
            var tEntry = _mapper.Map<TimeEntry, TimeEntryView>(timeEntry);
            return tEntry;
        }

        public static TimeEntryView GetViewTimeEntry(this TimeEntry timeEntry, string userName, IMapper _mapper)
        {
            var tEntry = timeEntry.GetView(_mapper);

            if (timeEntry.Project != null)
            {
                tEntry.IsUserManagerOnProject = timeEntry.Project.MemberProjectRoles == null
                    ? false
                    : timeEntry.Project.MemberProjectRoles.Any(r => r.Member?.User.UserName == userName && r.Role?.Name == Constants.ProjectRoleManager);
            }

            return tEntry;
        }

        public static ReportsGridItemsView GetViewReportsGridItem(this TimeEntry timeEntry, IMapper _mapper)
        {
            var tEntry = _mapper.Map<TimeEntry, ReportsGridItemsView>(timeEntry);
            return tEntry;
        }
    }
}