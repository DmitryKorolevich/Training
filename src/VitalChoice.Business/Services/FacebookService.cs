using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class FacebookInfo
    {
        public string id { get; set; }
        public long likes { get; set; }
    }

    public class FacebookService
    {
        private readonly ILogger _logger;
        private readonly string _accessLink;

        public FacebookService(
            IOptions<AppOptions> options,
            ILoggerProviderExtended loggerProvider)
        {
            var accessToken = $"{options.Value.FacebookSettings.AppId}|{options.Value.FacebookSettings.AppSecret}";
            _accessLink = $"https://graph.facebook.com/{options.Value.FacebookSettings.Id}?fields=likes&access_token={accessToken}";
            _logger = loggerProvider.CreateLogger<FacebookService>();
        }

        public long GetLikesCount()
        {
            try
            {
                JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                });
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_accessLink);
                request.Method = "GET";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                        {
                            return
                                serializer.Deserialize<FacebookInfo>(new JsonTextReader(new StreamReader(stream))).likes;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError("KPI report generating error - facebook");
                _logger.LogError(e.ToString());
                return 0;
            }
            return 0;
        }
    }
}
