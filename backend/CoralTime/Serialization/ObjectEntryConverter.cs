using CoralTime.Common.Helpers;
using Microsoft.OData.Core;
using System;
using System.Collections.Generic;

namespace CoralTime.Serialization
{
    
    public class ObjectEntryConverter
    {
        public static ODataEntry Convert(ODataEntry entry)
        {
            //replace PascalCase in properties names to camelCase
            List<ODataProperty> new_properties = new List<ODataProperty>();

            foreach (ODataProperty odata_property in entry.Properties)
            {
                object value = odata_property.Value;
                if (null != value)
                {
                    if (value is DateTimeOffset)
                        value = ((DateTimeOffset)value).DateTime;
                    ODataProperty new_property = ODataUtils.CreateProperty(StringHandler.ToLowerCamelCase(odata_property.Name), value);
                    new_properties.Add(new_property);

                }
            }

            entry.Properties = new_properties;

            return entry;
        }
    }
}
