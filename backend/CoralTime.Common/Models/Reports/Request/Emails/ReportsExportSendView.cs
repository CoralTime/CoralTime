using CoralTime.Common.Models.Reports.Request.Grid;

namespace CoralTime.ViewModels.Reports.Request.Emails
{
    public class ReportsExportSendView : RequestReportsGrid
    {
        public string ToEmail { get; set; }

        public string[] CcEmails { get; set; }

        public string[] BccEmails { get; set; }

        public string Subject { get; set; }

        public string Comment { get; set; }
    }
}
