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
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Groove;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class GrooveService : IGrooveService
    {
        private const string _accessLink = "https://api.groovehq.com/v1/";
        private const string _accessTokenParam = "access_token";
        private readonly string _accessToken;
        private readonly string _serviceEmail;
        private readonly ILogger _logger;

        public GrooveService(IOptions<AppOptions> options,
            ILoggerFactory loggerProvider)
        {
            _accessToken = options.Value.GrooveSettings.AccessToken;
            _serviceEmail = options.Value.GrooveSettings.ServiceEmail;
            _logger = loggerProvider.CreateLogger<GrooveService>();
        }

        public Task<bool> AddHelpTicketAsync(AddTicketModel ticket)
        {
            try
            {
                ticket.to = _serviceEmail;
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_accessLink + $"tickets?{_accessTokenParam}={_accessToken}");
                request.Method = "POST";
                request.ContentType = "application/json";
                var body = JsonConvert.SerializeObject(ticket);
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
                            }
                        }
                    }
                }

                return Task.FromResult<bool>(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }

            return Task.FromResult<bool>(false);
        }
    }
}
