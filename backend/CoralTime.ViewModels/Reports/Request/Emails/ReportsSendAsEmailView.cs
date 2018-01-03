using CoralTime.ViewModels.Reports.Request.ReportsGrid;

namespace CoralTime.ViewModels.Reports.Request.ReportsEmails
{
    public class ReportsSendAsEmailView : RequestReportsGrid
    {
        public string ToEmail { get; set; }

        public string[] CcEmails { get; set; }

        public string[] BccEmails { get; set; }

        public string Subject { get; set; }

        public string Comment { get; set; }
    }
}
