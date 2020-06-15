using System.Text.Json.Serialization;

namespace CoralTime.ViewModels.Member.MemberPersonalInfoView
{
    public class MemberPersonalInfoView : IMemberPersonalInfoView
    {
        [JsonIgnore]
        public int Id { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }
    }
}