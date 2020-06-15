using CoralTime.DAL.Models;
using System;

namespace CoralTime.BL.Helpers
{
    public static partial class BLHelpers
    {
        public static void StopTimer(this TimeEntry timeEntry)
        {
            if (timeEntry.TimeTimerStart <= 0) return;

            var timerTime = (int)new TimeSpan(0, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second).TotalSeconds - timeEntry.TimeTimerStart;

            if (timerTime < 0)
            {
                timerTime = 0;
            }

            timeEntry.TimeActual += timerTime;
            timeEntry.TimeTimerStart = -1;
            timeEntry.TimeFrom = 0;
            timeEntry.TimeTo = timeEntry.TimeActual;
        }
    }
}