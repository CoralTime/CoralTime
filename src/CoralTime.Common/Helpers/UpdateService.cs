using System;
using System.Text.Json;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Helpers
{
    public class UpdateService<T>
    {
        public static T UpdateObject(JsonElement delta, T currentObject)
        {
            var propertyInfoes = typeof(T).GetProperties();

            foreach (var propertyInfo in propertyInfoes)
            {
                var jsonPropertyName = StringHandler.ToLowerCamelCase(propertyInfo.Name);
                if (jsonPropertyName != "id")
                {
                    JsonElement jsonProperty;
                    if (delta.TryGetProperty(jsonPropertyName, out jsonProperty))
                    {
                        propertyInfo.SetValue(currentObject, jsonProperty.GetValue(propertyInfo.PropertyType));
                    }
                }
            }

            return currentObject;
        }
    }
}