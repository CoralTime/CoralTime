using System.Linq;
using CoralTime.ViewModels.DateFormat;

namespace CoralTime.Common.Helpers
{
    public static class DateFormatsStorage
    {
        public static string GetDateFormatById(int dateFormatId) => GetDateFormat(dateFormatId)?.DateFormat;

        public static string GetDateFormaDotNetById(int dateFormatId) => GetDateFormat(dateFormatId)?.DateFormatDotNet;

        public static string GetDateFormatDotNetShortById(int dateFormatId) => GetDateFormat(dateFormatId)?.DateFormatDotNetShort;

        private static DateConvert GetDateFormat(int dateFormatId) => Constants.Constants.DateFormats.FirstOrDefault(x => x.DateFormatId == dateFormatId);
    }
}
