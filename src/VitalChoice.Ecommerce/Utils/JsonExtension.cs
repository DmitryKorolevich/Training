using System;
using System.IO;
using Newtonsoft.Json;

namespace VitalChoice.Ecommerce.Utils
{
    public static class JsonExtension
    {
        static JsonExtension()
        {
            var settings = new JsonSerializerSettings
            {
                StringEscapeHandling = StringEscapeHandling.EscapeHtml,
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = DateParseHandling.DateTime,
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Include
            };
            Serializer = JsonSerializer.Create(settings);
        }

        private static readonly JsonSerializer Serializer;

        public static string ToJson(this object obj)
        {
            using (var writer = new StringWriter())
            {
                Serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        public static T FromJson<T>(this string jsonString)
        {
            using (var reader = new StringReader(jsonString))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return Serializer.Deserialize<T>(jsonReader);
                }
            }
        }

        public static object FromJson(this string jsonString, Type type)
        {
            using (var reader = new StringReader(jsonString))
            {
                using (var jsonReader = new JsonTextReader(reader))
                {
                    return Serializer.Deserialize(jsonReader, type);
                }
            }
        }
    }
}