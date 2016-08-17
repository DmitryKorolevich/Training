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
using Microsoft.Extensions.Options;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly string _accountId;
        private readonly string _clientId;
        private readonly string _keyFile;
        private readonly string _keyPass;
        private readonly Lazy<AnalyticsService> _lclient;
        private readonly ILogger _logger;

        private AnalyticsService Client => _lclient.Value;

        public GoogleService(
            IOptions<AppOptions> options,
            ILoggerFactory loggerProvider)
        {
            _accountId = options.Value.GoogleSettings.GAAccountId;
            _clientId = options.Value.GoogleSettings.ClientId;
            _keyFile = options.Value.GoogleSettings.KeyFile;
            _keyPass = options.Value.GoogleSettings.KeyPass;
            _logger = loggerProvider.CreateLogger<GoogleService>();
            _lclient = new Lazy<AnalyticsService>(CreateAnalyticsService);
        }

        private AnalyticsService CreateAnalyticsService()
        {
            var keyUrl = _keyFile;
            if (!Path.IsPathRooted(keyUrl))
            {
                keyUrl = Directory.GetCurrentDirectory() + @"\" + keyUrl;
            }

            var certificate = new X509Certificate2(keyUrl, _keyPass, X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

            ServiceAccountCredential credential = new ServiceAccountCredential(
               new ServiceAccountCredential.Initializer(_clientId)
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
            var results = Client.Data.Ga.Get(_accountId, startDate.ToString("yyyy -MM-dd"),
                                  endDate.ToString("yyyy-MM-dd"), "ga:percentNewVisits");
            results.Segment = "gaid::-1";
            var res = double.Parse(results.Execute().TotalsForAllResults["ga:percentNewVisits"]) / 100.0;
        }

        public async Task<decimal> GetCartAbandon(DateTime startDate, DateTime endDate)
        {
            var results = Client.Data.Ga.Get(_accountId, startDate.ToString("yyyy-MM-dd"),
                                              endDate.ToString("yyyy-MM-dd"), "ga:goalAbandonRateAll");
            results.MaxResults = 1;
            return decimal.Parse((await results.ExecuteAsync()).TotalsForAllResults["ga:goalAbandonRateAll"]) / 100;
        }

        public async Task<long> GetUniqueVisits(DateTime startDate, DateTime endDate)
        {
            var results = Client.Data.Ga.Get(_accountId, startDate.ToString("yyyy-MM-dd"),
                                              endDate.ToString("yyyy-MM-dd"), "ga:visitors");
            results.Dimensions = "ga:visitCount";
            results.MaxResults = 1;
            return long.Parse((await results.ExecuteAsync()).TotalsForAllResults["ga:visitors"]);
        }

        public async Task<decimal> GetNewWebVisitsPercent(DateTime startDate, DateTime endDate)
        {
            var results = Client.Data.Ga.Get(_accountId, startDate.ToString("yyyy-MM-dd"),
                                              endDate.ToString("yyyy-MM-dd"), "ga:percentNewVisits");
            results.Segment = "gaid::-1";
            return decimal.Parse((await results.ExecuteAsync()).TotalsForAllResults["ga:percentNewVisits"]) / 100;
        }

        public async Task<decimal> GetTransactionsRevenueOrganics(DateTime startDate, DateTime endDate)
        {
            var results = Client.Data.Ga.Get(_accountId, startDate.ToString("yyyy-MM-dd"),
                                              endDate.ToString("yyyy-MM-dd"), "ga:transactionRevenue");
            results.Segment = "gaid::-5";
            results.Dimensions = "ga:medium";
            return decimal.Parse((await results.ExecuteAsync()).TotalsForAllResults["ga:transactionRevenue"]);
        }

        public async Task<decimal> GetTransactionsRevenuePaid(DateTime startDate, DateTime endDate)
        {
            var results = Client.Data.Ga.Get(_accountId, startDate.ToString("yyyy-MM-dd"),
                                              endDate.ToString("yyyy-MM-dd"), "ga:transactionRevenue");
            results.Segment = "gaid::-4";
            results.Dimensions = "ga:medium";
            return decimal.Parse((await results.ExecuteAsync()).TotalsForAllResults["ga:transactionRevenue"]);
        }

        public async Task<decimal> GetBounceRate(DateTime startDate, DateTime endDate)
        {
            var results = Client.Data.Ga.Get(_accountId, startDate.ToString("yyyy-MM-dd"),
                                                 endDate.ToString("yyyy-MM-dd"), "ga:visitBounceRate");
            return decimal.Parse((await results.ExecuteAsync()).TotalsForAllResults["ga:visitBounceRate"]) / 100;
        }

        public async Task<decimal> GetConversionLevel(DateTime startDate, DateTime endDate)
        {
            var results = Client.Data.Ga.Get(_accountId, startDate.ToString("yyyy-MM-dd"),
                                                 endDate.ToString("yyyy-MM-dd"), "ga:transactionsPerVisit");
            return decimal.Parse((await results.ExecuteAsync()).TotalsForAllResults["ga:transactionsPerVisit"]) / 100;
        }

        public async Task<decimal> GetAov(DateTime startDate, DateTime endDate)
        {
            var results = Client.Data.Ga.Get(_accountId, startDate.ToString("yyyy-MM-dd"),
                                                 endDate.ToString("yyyy-MM-dd"), "ga:revenuePerTransaction");
            return decimal.Parse((await results.ExecuteAsync()).TotalsForAllResults["ga:revenuePerTransaction"]);
        }
    }
}
