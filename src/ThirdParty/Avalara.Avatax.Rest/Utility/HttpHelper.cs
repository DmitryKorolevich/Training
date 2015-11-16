using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Avalara.Avatax.Rest.Utility
{
    public static class HttpHelper
    {
        internal static HttpWebRequest CreateRequest(this Uri address, HttpMethod method, string accountNumber, string license)
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(address);
            request.Headers = new WebHeaderCollection
            {
                [HttpRequestHeader.Authorization] =
                    "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(accountNumber + ":" + license))
            };
            if (method.Method == HttpMethod.Post.Method || method.Method == HttpMethod.Put.Method)
            {
                request.ContentType = "application/json";
            }
            request.Accept = "application/json";
            request.Method = method.Method;
            return request;
        }

        internal static T ProcessResponse<T>(this JsonSerializer serializer, WebResponse response)
        {
            using (var responseStream = response.GetResponseStream())
            {
                if (responseStream != null)
                    return serializer.Deserialize<T>(new JsonTextReader(new StreamReader(responseStream)));
                return default(T);
            }
        }

        internal static void WriteTo(this JsonSerializer serializer, object obj, Stream newStream)
        {
            using (var streamWriter = new StreamWriter(newStream))
            {
                using (var jsonWriter = new JsonTextWriter(streamWriter))
                {
                    serializer.Serialize(jsonWriter, obj);
                    jsonWriter.Flush();
                }
            }
        }
    }
}