using System.Collections.Generic;

namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportProjectView
    {
        public ReportProjectView()
        {
            UsersDetails = new List<ReportUsersView>();
        }

        public bool IsUserManagerOnProject { get; set; }

        public string ProjectName { get; set; }

        public int ProjectId { get; set; }

        public int RoleId { get; set; }

        public bool IsProjectActive { get; set; }

        public List<ReportUsersView> UsersDetails { get; set; }
    }
}