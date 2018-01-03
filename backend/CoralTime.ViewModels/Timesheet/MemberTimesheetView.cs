using System;

namespace CoralTime.ViewModels.Timesheet
{
    public class MemberTimesheetView
    {
        public int MemberId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }
    }
}
