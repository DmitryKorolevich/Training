using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly ILogger _logger;

        private readonly Lazy<AnalyticsService> _lclient;

        private AnalyticsService Client => _lclient.Value;

        public GoogleService(
            ILoggerProviderExtended loggerProvider)
        {
            _logger = loggerProvider.CreateLogger<GoogleService>();
            _lclient = new Lazy<AnalyticsService>(CreateAnalyticsService);
        }

        private AnalyticsService CreateAnalyticsService()
        {
            const string clientId = "481586578687@developer.gserviceaccount.com";

            const string keyFile = @"\5083717e58d4513a8e8ca2992840b54ee728d674-privatekey.p12";

            const string keyPass = "notasecret";

            var keyUrl = Directory.GetCurrentDirectory() + keyFile;

            var certificate = new X509Certificate2(keyUrl, keyPass, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(clientId)
               {
                   Scopes = new[] { AnalyticsService.Scope.AnalyticsReadonly }
               }.FromCertificate(certificate));

            BaseClientService.Initializer initializer = new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
            };
            return  new AnalyticsService(initializer);
        }

        public void Test()
        {
            //var accounts = Client.Management.AccountSummaries.List().Execute();
            //foreach (var account in accounts.Items)
            //{
            //    var profileId = account.WebProperties[0].Profiles[0].Id;
            //}

            var startDate = DateTime.Now.AddMonths(-2);
            var endDate = DateTime.Now.AddMonths(-1);
            var results = Client.Data.Ga.Get("ga:652942", startDate.ToString("yyyy-MM-dd"),
                                  endDate.ToString("yyyy-MM-dd"), "ga:percentNewVisits");
            results.Segment = "gaid::-1";
            var res = double.Parse(results.Execute().TotalsForAllResults["ga:percentNewVisits"]) / 100.0;
        }
    }
}
