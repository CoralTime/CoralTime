namespace CoralTime.ViewModels.Reports.Responce.DropDowns.Filters
{
    public class ReportsDropDownValues : ReportsDropDowns
    {
        public ReportsDropDownValues()
        {
            UserDetails = new ReportsUserDetails();
        }

        public ReportsUserDetails UserDetails { get; set; }
    }
}
