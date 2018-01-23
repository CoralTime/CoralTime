using CoralTime.Common.Models;
using CoralTime.ViewModels.TimeEntries;
using System;
using System.Collections.Generic;

namespace CoralTime.BL.Interfaces
{
    public interface ITimeEntryService
    {
        IEnumerable<TimeEntryView> GetAllTimeEntries(string userName, DateTime dateSart, DateTime dateEnd);

        TimeEntryView GetById(int id, string userName);

        TimeEntryView Create(TimeEntryView timeEntryData, string userName);

        TimeEntryView Update(TimeEntryView timeEntryData, string userName);

        TimeEntryView Patch(TimeEntryTime timeEntryData, string userName);

        void Delete(int timeEntryId, string userName);
    }
}