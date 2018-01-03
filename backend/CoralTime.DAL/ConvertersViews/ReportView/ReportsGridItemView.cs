using CoralTime.Data.Models;
using CoralTime.ViewModels.Reports.ReportsGrid;

namespace CoralTime.Data.Converters.ConvertTo
{
    public static partial class ConvertTo
    {
        private static ReportsGridItemView MapToReportsGridItemView(TimeEntry timeEntry, Member member)
        {
            return new ReportsGridItemView
            {
                ActualTime = timeEntry.Time,
                Date = timeEntry.Date,
                EstimatedTime = timeEntry.PlannedTime,
                IsMemberActive = member.User.IsActive,
                MemberId = member.Id,
                MemberName = member.FullName,
                TaskId = timeEntry.TaskTypesId,
                TaskName = timeEntry.TaskType.Name, 
                Description = timeEntry.Description
            };
        }

        public static ReportsGridItemView ReportsGridItemView(TimeEntry timeEntry, Member member)
        {
            var result = MapToReportsGridItemView(timeEntry, member);

            return result;
        }

    }
}
