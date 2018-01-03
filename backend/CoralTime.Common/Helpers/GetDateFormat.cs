using System.Linq;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Helpers
{
    public class GetDateFormat
    {
        public string GetDateFormatById(int dateFormatId)
        {
            return DateFormats.FirstOrDefault(x => x.DateFormatId == dateFormatId)?.DateFormat;
        }

        public string GetDateFormaDotNetById(int dateFormatId)
        {
            return DateFormats.FirstOrDefault(x => x.DateFormatId == dateFormatId)?.DateFormatDotNet;
        }

        public string GetDateFormaDotNetShortById(int dateFormatId)
        {
            return DateFormats.FirstOrDefault(x => x.DateFormatId == dateFormatId)?.DateFormatDotNetShort;
        }
    }
}
