using CoralTime.DAL.Models;
using CoralTime.ViewModels.Reports;
using CoralTime.ViewModels.Reports.Responce.ReportsGrid;

namespace CoralTime.Data.Converters.ConvertTo
{
    public static partial class ConvertTo
    {
        private static ReportsGridItemsView MapToReportsGridItemView(TimeEntry timeEntry, Member member)
        {
            return new IReportsGridItemsView
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
