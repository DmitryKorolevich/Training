using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Avalara.Avatax.Rest.Utility;
using Microsoft.Framework.OptionsModel;
using Newtonsoft.Json;
using VitalChoice.Domain.Avatax;
using VitalChoice.Domain.Entities.Options;

namespace Avalara.Avatax.Rest.Services
{
    public class TaxService : ITaxService
    {
        private readonly string _accountNumber;
        private readonly string _license;
        private readonly string _svcUrl;
        private readonly JsonSerializer _serializer;

        public TaxService(IOptions<AppOptions> options)
        {
            _accountNumber = options.Options.Avatax.AccountNumber;
            _license = options.Options.Avatax.LicenseKey;
            _svcUrl = options.Options.Avatax.ServiceUrl.TrimEnd('/') + "/1.0/";
            _serializer =
                JsonSerializer.Create(new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateParseHandling = DateParseHandling.DateTime,
                    DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
                });
        }

        public async Task<GetTaxResult> GetTax(GetTaxRequest req)
        {
            Uri address = new Uri(_svcUrl + "tax/get");
            var request = address.CreateRequest(HttpMethod.Post, _accountNumber, _license);
            using (Stream newStream = await request.GetRequestStreamAsync())
            {
                _serializer.Serialize(new JsonTextWriter(new StreamWriter(newStream)), req);
                GetTaxResult result;
                try
                {
                    WebResponse response = await request.GetResponseAsync();
                    result = _serializer.ProcessResponse<GetTaxResult>(response);
                }
                catch (WebException ex)
                {
                    result = _serializer.ProcessResponse<GetTaxResult>(ex.Response);
                }
                return result;
            }
        }

        public async Task<GeoTaxResult> EstimateTax(decimal latitude, decimal longitude, decimal saleAmount)
        {
            Uri address = new Uri($"{_svcUrl}tax/{latitude},{longitude}/get.xml?saleamount={saleAmount}");
            var request = address.CreateRequest(HttpMethod.Get, _accountNumber, _license);

            GeoTaxResult result;
            try
            {
                WebResponse response = await request.GetResponseAsync();
                result = _serializer.ProcessResponse<GeoTaxResult>(response);
            }
            catch (WebException ex)
            {
                result = _serializer.ProcessResponse<GeoTaxResult>(ex.Response);
            }

            return result;
        }

        public async Task<GeoTaxResult> Ping()
        {
            return await EstimateTax((decimal) 47.627935, (decimal) -122.51702, 10);
        }

        public async Task<CancelTaxResult> CancelTax(CancelTaxRequest cancelTaxRequest)
        {
            Uri address = new Uri(_svcUrl + "tax/cancel");
            var request = address.CreateRequest(HttpMethod.Post, _accountNumber, _license);
            using (Stream newStream = await request.GetRequestStreamAsync())
            {
                _serializer.Serialize(new JsonTextWriter(new StreamWriter(newStream)), cancelTaxRequest);
                CancelTaxResponse cancelResponse;
                try
                {
                    WebResponse response = await request.GetResponseAsync();
                    cancelResponse = _serializer.ProcessResponse<CancelTaxResponse>(response);
                }
                catch (WebException ex)
                {
                    cancelResponse = _serializer.ProcessResponse<CancelTaxResponse>(ex.Response);

                    if (cancelResponse.ResultCode.Equals(SeverityLevel.Error))
                    {
                        cancelResponse.CancelTaxResult = new CancelTaxResult
                        {
                            ResultCode = cancelResponse.ResultCode,
                            Messages = cancelResponse.Messages
                        };
                    }
                }
                return cancelResponse.CancelTaxResult;
            }
        }
    }
}