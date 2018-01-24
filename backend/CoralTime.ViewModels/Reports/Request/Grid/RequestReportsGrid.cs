namespace CoralTime.ViewModels.Reports.Request.Grid
{
    public class RequestReportsGrid
    {
        public int DateFormatId { get; set; }

        public int? FileTypeId { get; set; }

        public RequestReportsSettings ValuesSaved { get; set; }
    }
}