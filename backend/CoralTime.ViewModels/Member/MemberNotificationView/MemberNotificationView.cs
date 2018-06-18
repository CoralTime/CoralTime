using Newtonsoft.Json;

namespace CoralTime.ViewModels.Member.MemberNotificationView
{
    public class MemberNotificationView : IMemberNotificationView
    {
        [JsonIgnore]
        public int Id { get; set; }

        public int SendEmailTime { get; set; }

        public string SendEmailDays { get; set; }
    }
}