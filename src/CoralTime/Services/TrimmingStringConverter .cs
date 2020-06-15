using Newtonsoft.Json;
using System;

namespace CoralTime.Services
{
    public class TrimmingStringConverter : JsonConverter
    {
        public override bool CanRead => true;
        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => objectType == typeof(string);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var path = reader.Path;

            if (reader.Value is string value && !path.Contains("password", StringComparison.InvariantCultureIgnoreCase))
            {
                return value.Trim();
            }

            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
