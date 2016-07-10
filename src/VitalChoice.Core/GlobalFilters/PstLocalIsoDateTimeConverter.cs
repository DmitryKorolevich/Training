using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Core.GlobalFilters
{
    public class PstLocalIsoDateTimeConverter : IsoDateTimeConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = base.ReadJson(reader, objectType, existingValue, serializer);
            if (obj is DateTime)
            {
                var dateTime = (DateTime) obj;
                return TimeZoneInfo.ConvertTime(dateTime, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local);
            }
            return obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                var dateTime = (DateTime) value;
                if (dateTime.Kind == DateTimeKind.Local)
                {
                    value = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, TimeZoneHelper.PstTimeZoneInfo);
                }
                else if (dateTime.Kind == DateTimeKind.Utc)
                {
                    value = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Utc, TimeZoneHelper.PstTimeZoneInfo);
                }
            }
            base.WriteJson(writer, value, serializer);
        }
    }
}