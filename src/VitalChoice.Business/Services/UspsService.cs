using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Google.Apis.Analytics.v3;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class UspsService : IUspsService
    {
        private readonly ICountryNameCodeResolver _countryNameCodeResolver;
        private readonly string _uspsUserId;
        private readonly ILogger _logger;

        private const string USPS_ADDRESS_VALIDATION_URL_FORMAT = "http://production.shippingapis.com/ShippingAPI.dll?API=Verify&XML={0}";

        public UspsService(
            IOptions<AppOptions> appOptions,
            ICountryNameCodeResolver countryNameCodeResolver,
            ILoggerFactory loggerProvider)
        {
            _uspsUserId = appOptions.Value.UspsSettings.UserId;
            _countryNameCodeResolver = countryNameCodeResolver;
            _logger = loggerProvider.CreateLogger<UspsService>();
        }

        public async Task<bool> IsAddressValidAsync(AddressDynamic address)
        {
            if (address == null)
                return false;

            if (_countryNameCodeResolver.IsCountry(address, "us"))
            {
                string address1 = address.SafeData.Address1;
                string city = address.SafeData.City;
                string zip = address.SafeData.Zip;
                string stateCode = null;
                if (address.IdState.HasValue && address.IdCountry.HasValue)
                {
                    stateCode = _countryNameCodeResolver.GetStateCode(address.IdCountry.Value, address.IdState.Value);
                }
                if (string.IsNullOrEmpty(zip) || string.IsNullOrEmpty(address1) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(stateCode))
                {
                    return false;
                }

                var zip5 = zip.Length >= 5 ? zip.Substring(0, 5) : "00000".Substring(0, 5 - zip.Length) + zip;

                try
                {
                    var xml = $"<AddressValidateRequest USERID=\"{_uspsUserId}\"><IncludeOptionalElements>false</IncludeOptionalElements><ReturnCarrierRoute>false</ReturnCarrierRoute><Address ID=\"0\"><FirmName /><Address1 />"+
                        $"<Address2>{address1}</Address2><City>{city}</City><State>{stateCode}</State><Zip5></Zip5><Zip4></Zip4></Address></AddressValidateRequest>";
                    var url = string.Format(USPS_ADDRESS_VALIDATION_URL_FORMAT, xml);

                    HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
                    request.Method = "GET";
                    using (var response = request.GetResponse())
                    {
                        using (var stream = response.GetResponseStream())
                        {
                            if (stream != null)
                            {
                                using (StreamReader reader = new StreamReader(stream))
                                {
                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(await reader.ReadToEndAsync());
                                    if (doc.DocumentElement.ChildNodes.Count > 0)
                                    {
                                        var resultAddress = doc.DocumentElement.ChildNodes[0];
                                        var errors = resultAddress.SelectNodes("Error");
                                        if (errors != null && errors.Count > 0)
                                        {
                                            return false;
                                        }
                                        else
                                        {
                                            var resultZip5Node = resultAddress.SelectNodes("Zip5");
                                            if (resultZip5Node != null && resultZip5Node.Count > 0)
                                            {
                                                var resultZip5 = resultZip5Node[0].InnerText;

                                                return resultZip5 == zip5;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    return false;
                }

                return false;
            }

            return true;
        }
    }
}
