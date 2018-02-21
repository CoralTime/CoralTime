namespace CoralTime.ViewModels.Reports.Request.Grid
{
    public class ReportsGridView
    {
        public int DateFormatId { get; set; }

        public int? FileTypeId { get; set; }

        public ReportsSettingsView CurrentQuery { get; set; }
    }
}