using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.ReportsDropwDowns
{
    public class ReportsDropDownsView 
    {
        public ReportsDropDownsView()
        {
            ClientsDetails = new List<ReportClientView>();
        }

        public string CurrentUserFullName { get; set; }

        public int CurrentUserId { get; set; }

        public bool IsManagerCurrentUser { get; set; }

        public bool IsAdminCurrentUser { get; set; }

        public List<ReportClientView> ClientsDetails { get; set; }
    }
}