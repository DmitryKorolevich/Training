using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VitalChoice.Ecommerce.Utils
{
    public static class JsonExtension
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings()
        {
            StringEscapeHandling = StringEscapeHandling.EscapeHtml
        };

        public static string ToJson(this object obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }

        public static T FromJson<T>(this string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString, Settings);
        }
    }
}