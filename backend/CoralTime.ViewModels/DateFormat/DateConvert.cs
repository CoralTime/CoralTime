using Newtonsoft.Json;

namespace CoralTime.ViewModels.DateFormat
{
    public class DateConvert
    {
        public int DateFormatId { get; set; }

        public string DateFormat { get; set; }

        [JsonIgnore]
        public string DateFormatDotNet { get; set; }

        [JsonIgnore]
        public string DateFormatDotNetShort { get; set; }
    }
}
