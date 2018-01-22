namespace CoralTime.Common.Models.Reports.Responce.DropDowns.Filters
{
    public class ReportsUserDetails
    {
        public string CurrentUserFullName { get; set; }

        public int CurrentUserId { get; set; }

        public bool IsManagerCurrentUser { get; set; }

        public bool IsAdminCurrentUser { get; set; }
    }
}
