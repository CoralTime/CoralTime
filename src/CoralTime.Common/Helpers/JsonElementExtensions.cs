using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CoralTime.Common.Helpers
{
    public static class JsonElementExtensions
    {
        public static object GetValue(this JsonElement json, Type type)
        {
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                if (json.HasValue())
                {
                    return json.GetValue(underlyingType);
                }
                else
                {
                    return null;
                }
            }
            else
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        return json.GetBoolean();
                    case TypeCode.Byte:
                        return json.GetByte();
                    case TypeCode.DateTime:
                        return json.GetDateTime();
                    case TypeCode.Decimal:
                        return json.GetDecimal();
                    case TypeCode.Double:
                        return json.GetDouble();
                    case TypeCode.Int16:
                        return json.GetInt16();
                    case TypeCode.Int32:
                        return json.GetInt32();
                    case TypeCode.Int64:
                        return json.GetInt64();
                    case TypeCode.SByte:
                        return json.GetSByte();
                    case TypeCode.Single:
                        return json.GetSingle();
                    case TypeCode.String:
                        return json.GetString();
                    case TypeCode.UInt16:
                        return json.GetUInt16();
                    case TypeCode.UInt32:
                        return json.GetUInt32();
                    case TypeCode.UInt64:
                        return json.GetUInt64();
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        public static JsonElement? GetNullableProperty(this JsonElement json, string propertyName)
        {
            return json.TryGetProperty(propertyName, out JsonElement propertyValue) ? (JsonElement?)propertyValue : null;
        }

        public static int? GetNullableInt32(this JsonElement json)
        {
            return json.HasValue() ? (int?)json.GetInt32() : null;
        }

        public static bool? GetNullableBoolean(this JsonElement json)
        {         
            return json.HasValue() ? (bool?)json.GetBoolean() : null;
        }

        public static bool HasValue(this JsonElement json)
        {
            var valueKind = json.ValueKind;
            return valueKind != JsonValueKind.Undefined && valueKind != JsonValueKind.Null;
        }
    }
}
