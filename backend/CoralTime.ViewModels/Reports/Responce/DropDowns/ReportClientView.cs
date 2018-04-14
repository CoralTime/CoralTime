using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportClientView
    {
        public ReportClientView()
        {
            ProjectsDetails = new List<ReportProjectView>();
        }

        public int ClientId { get; set; }

        public string ClientName { get; set; }

        public bool IsClientActive { get; set; }

        public List<ReportProjectView> ProjectsDetails { get; set; }
    }
}