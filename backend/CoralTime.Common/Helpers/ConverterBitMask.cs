using System.Collections.Generic;
using System.Linq;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Helpers
{
    public class ConverterBitMask
    {
        public static int? DayOfWeekStringToInt(string sendEmailDays)
        {
            if (!string.IsNullOrEmpty(sendEmailDays))
            {
                var daysOfWeek = sendEmailDays.Split(',').Select(short.Parse).ToList();

                if (daysOfWeek.Count > 0)
                {
                    short sendEmailDaysResult = 0;

                    foreach (var dayOfWeek in daysOfWeek)
                    {
                        var dayOfWeekById = daysOfWeekWithBinaryValues.FirstOrDefault(x => x.Id == dayOfWeek);

                        if (dayOfWeekById != null)
                        {
                            sendEmailDaysResult = (short) (sendEmailDaysResult | dayOfWeekById.ValueForBinary);
                        }
                    }

                    return sendEmailDaysResult;
                }
            }

            return null;
        }

        public static string DayOfWeekIntToString(int? sendEmailDays)
        {
            if (sendEmailDays != null)
            {
                var daysOfWeekResult = new List<short>();

                foreach (var dayOfWeek in daysOfWeekWithBinaryValues)
                {
                    var res = (short) sendEmailDays & dayOfWeek.ValueForBinary;

                    if (res > 0)
                    {
                        daysOfWeekResult.Add(dayOfWeek.Id);
                    }
                }

                if (daysOfWeekResult.Count > 0)
                {
                    var sendEmailDaysResult = string.Join(",", daysOfWeekResult);
                    return sendEmailDaysResult;
                }
            }

            return string.Empty;
        }

        public static List<DaysOfWeekAdaptive> DaysOfWeekIntToListDayOfWeekAdaptive(int? sendEmailDays)
        {
            if (sendEmailDays != null)
            {
                var daysOfWeekAdaptive = new List<DaysOfWeekAdaptive>();

                foreach (var dayOfWeek in daysOfWeekWithBinaryValues)
                {
                    var hasIdDayofWeekInArray = (short)sendEmailDays & dayOfWeek.ValueForBinary;

                    if (hasIdDayofWeekInArray > 0)
                    {
                        daysOfWeekAdaptive.Add(dayOfWeek);
                    }
                }

                return daysOfWeekAdaptive;
            }

            return new List<DaysOfWeekAdaptive>();
        }
    }
}
