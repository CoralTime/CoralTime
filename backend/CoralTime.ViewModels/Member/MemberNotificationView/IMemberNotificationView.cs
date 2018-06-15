namespace CoralTime.ViewModels.Member.MemberNotificationView
{
    public interface IMemberNotificationView
    {
        int Id { get; set; }

        int SendEmailTime { get; set; }

        string SendEmailDays { get; set; }
    }
}