using System;

namespace CoralTime.ViewModels.Reports.Request.Grid
{
    public class ReportsGridView
    {
        public int DateFormatId { get; set; }

        public int? FileTypeId { get; set; }

        public DateTimeOffset? Date { get; set; }

        public ReportsSettingsView.ReportsSettingsView CurrentQuery { get; set; }

        public DateTime? GetTodayDate => Date?.Date;
    }
}