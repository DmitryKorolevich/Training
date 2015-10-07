using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Avalara.Avatax.Rest.Utility;
using Microsoft.Framework.OptionsModel;
using Newtonsoft.Json;
using VitalChoice.Domain.Avatax;
using VitalChoice.Domain.Entities.Options;

namespace Avalara.Avatax.Rest.Services
{
    public class AddressService : IAddressService
    {
        private static string _accountNumber;
        private static string _license;
        private static string _svcUrl;
        private readonly JsonSerializer _serializer;

        public AddressService(IOptions<AppOptions> options)
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

        public async Task<ValidateResult> Validate(Address address)
        {
            List<string> parameters = new List<string>();
            if (address.Line1 != null)
            {
                parameters.Add("Line1=" + address.Line1.Replace(" ", "+"));
            }
            if (address.Line2 != null)
            {
                parameters.Add("Line2=" + address.Line2.Replace(" ", "+"));
            }
            if (address.Line3 != null)
            {
                parameters.Add("Line3=" + address.Line3.Replace(" ", "+"));
            }
            if (address.City != null)
            {
                parameters.Add("City=" + address.City.Replace(" ", "+"));
            }
            if (address.Region != null)
            {
                parameters.Add("Region=" + address.Region.Replace(" ", "+"));
            }
            if (address.PostalCode != null)
            {
                parameters.Add("PostalCode=" + address.PostalCode.Replace(" ", "+"));
            }
            if (address.Country != null)
            {
                parameters.Add("Country=" + address.Country.Replace(" ", "+"));
            }
            var querystring = string.Join("&", parameters);

            Uri webAddress = new Uri(_svcUrl + "address/validate.xml?" + querystring);
            var request = webAddress.CreateRequest(HttpMethod.Get, _accountNumber, _license);

            ValidateResult result;
            try
            {
                WebResponse response = await request.GetResponseAsync();
                result = _serializer.ProcessResponse<ValidateResult>(response);
            }
            catch (WebException ex)
            {
                result = _serializer.ProcessResponse<ValidateResult>(ex.Response);
            }

            return result;
        }
    }
}