﻿namespace CoralTime.ViewModels.Reports.Responce.DropDowns
{
    public class ReportUserDetails
    {
        public string CurrentUserFullName { get; set; }

        public int CurrentUserId { get; set; }

        public bool IsManagerCurrentUser { get; set; }

        public bool IsAdminCurrentUser { get; set; }
    }
}
