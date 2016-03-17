﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalara.Avatax.Rest.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Avatax;
using System.ServiceModel;
using System.Text.RegularExpressions;
using System.Threading;
using FluentValidation.Validators;

namespace VitalChoice.Business.Services.Bronto
{
    public class BrontoService
    {
        private readonly BrontoSettings _brontoSettings;
        private readonly ILogger _logger;

        //TODO: should be removed after rc2 release(reason MessageHeaderAttribute)
#if !DOTNET5_4
        private readonly BrontoSoapPortTypeClient _client;
#endif

        public BrontoService(IOptions<AppOptions> options,
            ILoggerProviderExtended loggerProvider)
        {
            _brontoSettings = options.Value.Bronto;
            _logger = loggerProvider.CreateLoggerDefault();

#if !DOTNET5_4
            BasicHttpBinding binding = new BasicHttpBinding();
            binding.Security.Mode = BasicHttpSecurityMode.Transport;
            EndpointAddress endpoint = new EndpointAddress(_brontoSettings.ApiUrl);
            _client = new BrontoSoapPortTypeClient(binding, endpoint);
#endif
        }

        public async Task<bool> Send(string email)
        {
            EmailValidator emailValidator = new EmailValidator();
            var emailRegex = new Regex(emailValidator.Expression, RegexOptions.IgnoreCase);
            if (!emailRegex.IsMatch(email))
                return false;

            try
            {
                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create(_brontoSettings.PublicFormUrl);
                string toSend =
                    _brontoSettings.PublicFormSendData +
                    email;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (var stream = await request.GetRequestStreamAsync())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(toSend);
                    }

                    WebResponse response = await request.GetResponseAsync();
                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        return true;
                    }
                }
            }
            catch (WebException e)
            {
            }
            return false;
        }

        public async Task<bool> Subscribe(string email)
        {
            EmailValidator emailValidator = new EmailValidator();
            var emailRegex = new Regex(emailValidator.Expression, RegexOptions.IgnoreCase);
            if (!emailRegex.IsMatch(email))
                return false;

            try
            {
                HttpWebRequest request =
                    (HttpWebRequest)WebRequest.Create(_brontoSettings.PublicFormUrl);
                string toSend =
                    _brontoSettings.PublicFormSubscribeData +
                    email;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                using (var stream = await request.GetRequestStreamAsync())
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(toSend);
                    }

                    WebResponse response = await request.GetResponseAsync();
                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        return true;
                    }
                }
            }
            catch (WebException e)
            {
            }
            return false;
        }

        public bool Unsubscribe(string email)
        {
#if !DOTNET5_4
            List<contactObject> result = new List<contactObject>();
            int pageNumber = 1;
            contactObject[] lists;
            var brontoEmailAddress = new stringValue { value = email };
            var sessionHeader = new sessionHeader { sessionId = _client.login(_brontoSettings.ApiKey) };
            do
            {
                lists = _client.readContacts(sessionHeader, new readContacts
                {
                    pageNumber = pageNumber,
                    filter = new contactFilter
                    {
                        email = new[] { brontoEmailAddress }
                    },
                    includeLists = false,
                    includeSMSKeywords = false
                });
                if (lists != null)
                {
                    result.AddRange(lists);
                    pageNumber++;
                }
            } while (lists != null && lists.Length > 0);
            var contactsToUpdate = result.Where(c => c.status.ToLower() == "active" || c.status.ToLower() == "onboarding").Select(c => new contactObject
            {
                id = c.id,
                status = "unsub"
            }).ToArray();
            if (contactsToUpdate.Length > 0)
            {
                var results = _client.updateContacts(sessionHeader, contactsToUpdate).results;
                return results.All(s => !s.isError);
            }
#endif
            return true;
        }

        public bool GetIsUnsubscribed(string email)
        {
#if !DOTNET5_4
            List<contactObject> result = new List<contactObject>();
            int pageNumber = 1;
            contactObject[] lists;
            var brontoEmailAddress = new stringValue { value = email };
            var sessionHeader = new sessionHeader { sessionId = _client.login(_brontoSettings.ApiKey) };
            do
            {
                lists = _client.readContacts(sessionHeader, new readContacts
                {
                    pageNumber = pageNumber,
                    filter = new contactFilter
                    {
                        email = new[] { brontoEmailAddress }
                    },
                    includeLists = false,
                    includeSMSKeywords = false
                });
                if (lists != null)
                {
                    result.AddRange(lists);
                    pageNumber++;
                }
            } while (lists != null && lists.Length > 0);
            return result.All(c => c.status == "unsub");
#endif
            return true;
        }

#if !DOTNET5_4
        public contactObject[] GetAllActiveContacts()
        {
            List<contactObject> result = new List<contactObject>();
            int pageNumber = 1;
            contactObject[] lists;
            var sessionHeader = new sessionHeader { sessionId = _client.login(_brontoSettings.ApiKey) };
            do
            {
                lists = _client.readContacts(sessionHeader, new readContacts
                {
                    pageNumber = pageNumber,
                    filter = new contactFilter
                    {
                        status = new[] { "active" },
                    },
                    includeLists = false,
                    includeSMSKeywords = false
                });
                if (lists != null)
                {
                    result.AddRange(lists);
                    pageNumber++;
                }
            } while (lists != null && lists.Length > 0);
            return result.ToArray();
        }

        public int GetAddressCount(contactObject[] info, DateTime startDate, DateTime endDate)
        {
            return info.Count(c => c.created > startDate && c.created < endDate.AddDays(1));
        }

        public deliveryObject[] GetInfo()
        {
            int pageNumber = 1;
            deliveryObject[] deliveries;
            List<deliveryObject> result = new List<deliveryObject>();
            var sessionHeader = new sessionHeader { sessionId = _client.login(_brontoSettings.ApiKey) };
            do
            {
                deliveries = _client.readDeliveries(sessionHeader, new readDeliveries
                {
                    filter = new deliveryFilter(),
                    includeContent = false,
                    includeRecipients = false,
                    pageNumber = pageNumber
                });
                if (deliveries != null)
                {
                    result.AddRange(deliveries);
                    pageNumber++;
                }
            } while (deliveries != null && deliveries.Length > 0);
            return result.ToArray();
        }

        public double GetOpenRate(deliveryObject[] info, DateTime startDate, DateTime endDate)
        {
            var filtered = info.Where(d => d.start > startDate && d.start < endDate.AddDays(1)).ToArray();
            long sent = filtered.Sum(d => d.numSends);
            long opened = filtered.Sum(d => d.uniqOpens);
            return sent == 0 ? 0 : (opened / (double)sent);
        }
#endif
    }
}