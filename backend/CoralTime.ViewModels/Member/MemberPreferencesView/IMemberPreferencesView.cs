namespace CoralTime.ViewModels.Member.MemberPreferencesView
{
    public interface IMemberPreferencesView
    {
        int Id { get; set; }

        int DefaultProjectId { get; set; }

        int DefaultTaskId { get; set; }

        string TimeZone { get; set; }

        int DateFormatId { get; set; }

        int TimeFormat { get; set; }

        int WeekStart { get; set; }

        bool IsWeeklyTimeEntryUpdatesSend { get; set; }
    }
}