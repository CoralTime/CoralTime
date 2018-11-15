﻿using CoralTime.DAL.Models.Member;
using CoralTime.ViewModels.TimeEntries;
using System;
using System.Collections.Generic;

namespace CoralTime.BL.Interfaces
{
    public interface ITimeEntryService
    {
        IEnumerable<TimeEntryView> GetAllTimeEntries(DateTimeOffset dateSart, DateTimeOffset dateEnd);

        TimeEntryView GetById(int id);

        TimeEntryView Create(TimeEntryView timeEntryData, Member member = null);

        TimeEntryView Update(TimeEntryView timeEntryData);

        void Delete(int timeEntryId);

        TimerView GetTimeEntryTimer(DateTime? date);
    }
}