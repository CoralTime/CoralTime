using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.ReportsDropwDowns
{
    public class ReportDropDownView 
    {
        public ReportDropDownView()
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