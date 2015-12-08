using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers.Bases;
using Microsoft.Extensions.Logging;

namespace Authorize.Net.Util
{
    public static class HttpUtility
    {
        private static Uri GetPostUrl(Environment env)
        {
            var postUrl = new Uri(env.GetXmlBaseUrl() + "/xml/v1/request.api");
            return postUrl;
        }

        public static async Task<ANetApiResponse> PostData<TQ, TS>(Environment env, TQ request)
            where TQ : ANetApiRequest
            where TS : ANetApiResponse
        {
            ANetApiResponse response = null;
            if (null == request)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var postUrl = GetPostUrl(env);
            var webRequest = (HttpWebRequest) WebRequest.Create(postUrl);
            webRequest.Method = "POST";
            webRequest.ContentType = "text/xml";

            var requestType = typeof (TQ);
            var serializer = new XmlSerializer(requestType);
            using (var writer = new StreamWriter(await webRequest.GetRequestStreamAsync(), Encoding.UTF8))
            {
                serializer.Serialize(writer, request);
            }

            // Get the response
            string responseAsString = null;
            using (var webResponse = await webRequest.GetResponseAsync())
            {
                using (var responseStream = webResponse.GetResponseStream())
                {
                    if (null != responseStream)
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseAsString = reader.ReadToEnd();
                        }
                    }
                }
            }
            if (null != responseAsString)
            {
                using (var memoryStreamForResponseAsString = new MemoryStream(Encoding.UTF8.GetBytes(responseAsString)))
                {
                    var responseType = typeof (TS);
                    var deSerializer = new XmlSerializer(responseType);

                    object deSerializedObject;
                    try
                    {
                        // try deserializing to the expected response type
                        deSerializedObject = deSerializer.Deserialize(memoryStreamForResponseAsString);
                    }
                    catch (Exception)
                    {
                        // probably a bad response, try if this is an error response
                        memoryStreamForResponseAsString.Seek(0, SeekOrigin.Begin); //start from beginning of stream
                        var genericDeserializer = new XmlSerializer(typeof (ANetApiResponse));
                        deSerializedObject = genericDeserializer.Deserialize(memoryStreamForResponseAsString);
                    }

                    //if error response
                    if (deSerializedObject is ErrorResponse)
                    {
                        response = deSerializedObject as ErrorResponse;
                    }
                    else
                    {
                        //actual response of type expected
                        if (deSerializedObject is TS)
                        {
                            response = deSerializedObject as TS;
                        }
                        else if (deSerializedObject is ANetApiResponse) //generic response
                        {
                            response = deSerializedObject as ANetApiResponse;
                        }
                    }
                }
            }

            return response;
        }
    }
}