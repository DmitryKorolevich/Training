using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
//using LinqToTwitter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog.Internal;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class TwitterService
    {
        private readonly string _baseUrl = "https://api.twitter.com/";
        private readonly string _twitterConsumerKey;
        private readonly string _twitterConsumerSecret;

        private readonly ILogger _logger;

        public TwitterService(
            IOptions<AppOptions> options,
            ILoggerFactory loggerProvider)
        {
            _twitterConsumerKey = options.Value.TwitterSettings.ConsumerKey;
            _twitterConsumerSecret = options.Value.TwitterSettings.ConsumerSecret;
            _logger = loggerProvider.CreateLogger<TwitterService>();
        }

        private string GetToken()
        {
            if (string.IsNullOrWhiteSpace(_twitterConsumerKey) ||
                string.IsNullOrWhiteSpace(_twitterConsumerSecret))
            {
                return null;
            }

            string toReturn = null;

            var url = $"{_baseUrl}oauth2/token";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            var key = $"{_twitterConsumerKey}:{_twitterConsumerSecret}";
            key = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(key));
            request.Headers.Add("Authorization", key);
            request.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
            var body = "grant_type=client_credentials";
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(Encoding.UTF8.GetBytes(body), 0, body.Length);
            }
            using (var response = request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            var data = JObject.Parse(reader.ReadToEnd());
                            toReturn = data.GetValue("access_token").Value<string>();

                        }
                    }
                }
            }

            return toReturn;
        }

        public int GetFollowersCount()
        {
            var token = GetToken();

            try
            {
                var url = $"{_baseUrl}1.1/users/lookup.json?screen_name=vitalchoice";
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                var key = "Bearer " + token;
                request.Headers.Add("Authorization", key);

                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                var result = JArray.Parse(reader.ReadToEnd());
                                return result.Children<JObject>().First().GetValue("followers_count").Value<int>();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("KPI report generating error - twitter");
                _logger.LogError(e.ToString());
                return 0;
            }

            return 0;
        }
    }
}
