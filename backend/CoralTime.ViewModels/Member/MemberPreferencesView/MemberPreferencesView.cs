using Newtonsoft.Json;

namespace CoralTime.ViewModels.Member.MemberPreferencesView
{
    public class MemberPreferencesView : IMemberPreferencesView
    {
        [JsonIgnore]
        public int Id { get; set; }

        public int DefaultProjectId { get; set; }

        public int DefaultTaskId { get; set; }

        public string TimeZone { get; set; }

        public int DateFormatId { get; set; }

        public int TimeFormat { get; set; }

        public int WeekStart { get; set; }

        public bool IsWeeklyTimeEntryUpdatesSend { get; set; }
    }
}