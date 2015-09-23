using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VitalChoice.Core.GlobalFilters
{
    public class PstLocalIsoDateTimeConverter : IsoDateTimeConverter
    {
        private readonly TimeZoneInfo _pstTimeZoneInfo;

        public PstLocalIsoDateTimeConverter()
        {
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var obj = base.ReadJson(reader, objectType, existingValue, serializer);
            if (obj is DateTime)
            {
                var dateTime = (DateTime) obj;
                return TimeZoneInfo.ConvertTime(dateTime, _pstTimeZoneInfo, TimeZoneInfo.Local);
            }
            return obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                var dateTime = (DateTime)value;
                value = TimeZoneInfo.ConvertTime(dateTime, TimeZoneInfo.Local, _pstTimeZoneInfo);
            }
            base.WriteJson(writer, value, serializer);
        }
    }
}