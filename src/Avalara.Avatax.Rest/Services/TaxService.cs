using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Avalara.Avatax.Rest.Utility;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace Avalara.Avatax.Rest.Services
{
    public class DateTimeAvalaraConverter : IsoDateTimeConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                DateTime date = (DateTime) value;
                writer.WriteValue(date.ToString("yyyy-MM-dd"));
            }
            else
            {
                base.WriteJson(writer, value, serializer);
            }
        }
    }

    public class TaxService : ITaxService
    {
        private readonly string _accountNumber;
        private readonly string _license;
        private readonly string _svcUrl;
        private readonly JsonSerializer _serializer;
        private readonly ILogger _logger;

        public TaxService(IOptions<AppOptions> options, ILoggerProviderExtended loggerProvider)
        {
            _logger = loggerProvider.CreateLoggerDefault();
            _accountNumber = options.Value.Avatax.AccountNumber;
            _license = options.Value.Avatax.LicenseKey;
            _svcUrl = options.Value.Avatax.ServiceUrl.TrimEnd('/') + "/1.0/";
            _serializer =
                JsonSerializer.Create(new JsonSerializerSettings
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateParseHandling = DateParseHandling.DateTime,
                    DateTimeZoneHandling = DateTimeZoneHandling.Unspecified,
                    NullValueHandling = NullValueHandling.Ignore,
                    Culture = new CultureInfo("en-us"),
                    Error = (sender, args) =>
                    {
                        args.ErrorContext.Handled = true;
                        _logger.LogWarning(0, $"Serialization issue on [{args.ErrorContext.Member?.GetType()}]",
                            args.ErrorContext.Error);
                    },
                    Converters = new List<JsonConverter>
                    {
                        new DateTimeAvalaraConverter()
                    }
                });
        }

        public async Task<GetTaxResult> GetTax(GetTaxRequest req)
        {
            Uri address = new Uri(_svcUrl + "tax/get");
            var request = address.CreateRequest(HttpMethod.Post, _accountNumber, _license);
            using (Stream newStream = await request.GetRequestStreamAsync())
            {
                _serializer.WriteTo(req, newStream);
                GetTaxResult result;
                try
                {
                    WebResponse response = await request.GetResponseAsync();
                    result = _serializer.ProcessResponse<GetTaxResult>(response);
                }
                catch (WebException ex)
                {
                    result = _serializer.ProcessResponse<GetTaxResult>(ex.Response);
                    using (var memoryStream = new MemoryStream())
                    {
                        using (var streamWriter = new StreamWriter(memoryStream))
                        {
                            using (var jsonWriter = new JsonTextWriter(streamWriter))
                            {
                                _serializer.Serialize(jsonWriter, req);
                                jsonWriter.Flush();
                                memoryStream.Seek(0, SeekOrigin.Begin);
                                StreamReader reader = new StreamReader(memoryStream);
                                result.DocCode = reader.ReadToEnd();
                            }
                        }
                    }
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
                _serializer.WriteTo(cancelTaxRequest, newStream);
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