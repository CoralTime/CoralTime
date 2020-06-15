using System;
using static CoralTime.Common.Constants.Constants;

namespace CoralTime.Common.Helpers
{
    public class UpdateService<T>
    {
        public static T UpdateObject(dynamic delta, T currentObject)
        {
            var propertyInfoes = typeof(T).GetProperties();

            foreach (var propertyInfo in propertyInfoes)
            {
                if (StringHandler.ToLowerCamelCase(propertyInfo.Name)!="id" &&
                    HasField(delta, StringHandler.ToLowerCamelCase(propertyInfo.Name)))
                {
                    if (propertyInfo.PropertyType.Name.Contains("Nullable"))
                    {
                        int? value = delta[StringHandler.ToLowerCamelCase(propertyInfo.Name)];
                        propertyInfo.SetValue(currentObject, value);
                    }
                    else if (propertyInfo.PropertyType.Name.Contains("DateTime"))
                    {
                        DateTime value = delta[StringHandler.ToLowerCamelCase(propertyInfo.Name)];
                        propertyInfo.SetValue(currentObject, value);
                    }
                    else if (propertyInfo.PropertyType.Name.Contains("Double"))
                    {
                        double value = delta[StringHandler.ToLowerCamelCase(propertyInfo.Name)];
                        propertyInfo.SetValue(currentObject, value);
                    }
                    else if (propertyInfo.PropertyType.Name.Contains("LockTimePeriod"))
                    {
                        LockTimePeriod value = delta[StringHandler.ToLowerCamelCase(propertyInfo.Name)];
                        propertyInfo.SetValue(currentObject, value);
                    }
                    else
                    {
                        string value = delta[StringHandler.ToLowerCamelCase(propertyInfo.Name)];
                        propertyInfo.SetValue(currentObject, Convert.ChangeType(value, propertyInfo.PropertyType));
                    }
                }
            }

            return currentObject;
        }

        public static bool HasField(dynamic dynamicObject, string fieldName)
        {
            var field = dynamicObject[fieldName];
            try
            {
                var result = field.HasValues;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}