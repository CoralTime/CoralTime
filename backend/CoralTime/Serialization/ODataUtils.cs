using Microsoft.OData.Core;
using System;

namespace CoralTime.Serialization
{
    public class ODataUtils
    {
       
        public static ODataProperty CreateProperty(string name, object value)
        {
            var property_value = value;
            if (value != null)
            {
                Type t = value.GetType();
                
                if (t == typeof(DateTime) || t == typeof(DateTime?))
                {
                    DateTime dt = (DateTime)value;
                    dt = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
                    DateTimeOffset dto = dt;
                    property_value = dto;
                }
            }
            ODataProperty new_property = new ODataProperty()
            {
                Name = name,
                Value = property_value
            };
            return new_property;
        }

    }
}
