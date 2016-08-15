using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Avalara.Avatax.Rest.Services;
using Microsoft.Extensions.Logging;
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
using Microsoft.Extensions.Options;
using VitalChoice.Business.Helpers;
using VitalChoice.Infrastructure.Domain;

namespace VitalChoice.Business.Services.Bronto
{
    public class BrontoService
    {
        private readonly BrontoSettings _brontoSettings;
        private readonly ILogger _logger;

        private readonly Lazy<BrontoSoapPortTypeClient> _lclient;

        private BrontoSoapPortTypeClient Client => _lclient.Value;

        public BrontoService(IOptions<AppOptions> options,
            ILoggerProviderExtended loggerProvider)
        {
            _brontoSettings = options.Value.Bronto;
            _logger = loggerProvider.CreateLogger<BrontoService>();

            BasicHttpBinding binding = new BasicHttpBinding {Security = {Mode = BasicHttpSecurityMode.Transport}};
            binding.MaxReceivedMessageSize = int.MaxValue;
            binding.SendTimeout =new TimeSpan(0,5,0);
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            EndpointAddress endpoint = new EndpointAddress(_brontoSettings.ApiUrl);
            _lclient = new Lazy<BrontoSoapPortTypeClient>(() => new BrontoSoapPortTypeClient(binding, endpoint));
        }

        public void PushSubscribe(string email, bool subscribe)
        {
            Task.Run(() =>
            {
                var unsubscribed = GetIsUnsubscribed(email).GetAwaiter().GetResult();
                if (subscribe && (unsubscribed ?? true))
                {
                    Subscribe(email).GetAwaiter().GetResult();
                }
                if (!subscribe)
                {
                    if (!unsubscribed.HasValue)
                    {
                        //Resolve issue with showing the default value only the first time
                        Subscribe(email).GetAwaiter().GetResult();
                        Unsubscribe(email).GetAwaiter().GetResult();
                    }
                    else if (!unsubscribed.Value)
                    {
                        Unsubscribe(email).GetAwaiter().GetResult();
                    }
                }
            }).ConfigureAwait(false);
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
                        await writer.WriteAsync(toSend);
                    }

                    WebResponse response = await request.GetResponseAsync();
                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            await reader.ReadToEndAsync();
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return false;
        }

        public async Task<bool> Subscribe(string email)
        {
            if (!email.IsValidEmail())
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
                        await writer.WriteAsync(toSend);
                    }

                    WebResponse response = await request.GetResponseAsync();
                    var responseStream = response.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (var reader = new StreamReader(responseStream))
                        {
                            await reader.ReadToEndAsync();
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
            return false;
        }

        public async Task<bool> Unsubscribe(string email)
        {
            List<contactObject> result = new List<contactObject>();
            int pageNumber = 1;
            contactObject[] lists;
            var brontoEmailAddress = new stringValue {value = email};
            var sessionHeader = new sessionHeader {sessionId = Client.login(_brontoSettings.ApiKey)};
            do
            {
                lists = (await Client.readContactsAsync(sessionHeader, new readContacts
                {
                    pageNumber = pageNumber,
                    filter = new contactFilter
                    {
                        email = new[] {brontoEmailAddress}
                    },
                    includeLists = false,
                    includeSMSKeywords = false
                })).@return;
                if (lists != null)
                {
                    result.AddRange(lists);
                    pageNumber++;
                }
            } while (lists != null && lists.Length > 0);
            var contactsToUpdate =
                result.Where(c => c.status.ToLower() == "active" || c.status.ToLower() == "onboarding").Select(c => new contactObject
                {
                    id = c.id,
                    status = "unsub"
                }).ToArray();
            if (contactsToUpdate.Length > 0)
            {
                var results = (await Client.updateContactsAsync(sessionHeader, contactsToUpdate)).@return.results;
                return results.All(s => !s.isError);
            }
            return false;
        }

        public async Task<bool?> GetIsUnsubscribed(string email)
        {
            List<contactObject> result = new List<contactObject>();
            int pageNumber = 1;
            contactObject[] lists;
            var brontoEmailAddress = new stringValue { value = email };
            var sessionHeader = new sessionHeader { sessionId = Client.login(_brontoSettings.ApiKey) };
            do
            {
                lists = (await Client.readContactsAsync(sessionHeader, new readContacts
                {
                    pageNumber = pageNumber,
                    filter = new contactFilter
                    {
                        email = new[] {brontoEmailAddress}
                    },
                    includeLists = false,
                    includeSMSKeywords = false
                })).@return;
                if (lists != null)
                {
                    result.AddRange(lists);
                    pageNumber++;
                }
            } while (lists != null && lists.Length > 0);
            
            return result.Count==0 ? (bool?)null : result.All(c => c.status == "unsub");
        }


        public async Task<contactObject[]> GetAllActiveContacts(DateTime from)
        {
            List<contactObject> result = new List<contactObject>();
            int pageNumber = 1;
            contactObject[] lists;
            var sessionHeader = new sessionHeader { sessionId = Client.login(_brontoSettings.ApiKey) };
            do
            {
                lists = (await Client.readContactsAsync(sessionHeader, new readContacts
                {
                    pageNumber = pageNumber,
                    filter = new contactFilter
                    {
                        status = new[] { "active" },
                        created = new dateValue[1]
                        {
                            new dateValue()
                            {
                                @operator = filterOperator.After,
                                value = from,
                                operatorSpecified = true,
                                valueSpecified = true
                            }
                        }
                    },
                    includeLists = false,
                    includeSMSKeywords = false
                })).@return;
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

        public async Task<deliveryObject[]> GetInfo(DateTime from)
        {
            int pageNumber = 1;
            deliveryObject[] deliveries;
            List<deliveryObject> result = new List<deliveryObject>();
            var sessionHeader = new sessionHeader { sessionId = Client.login(_brontoSettings.ApiKey) };
            do
            {
                deliveries = (await Client.readDeliveriesAsync(sessionHeader, new readDeliveries
                {
                    filter = new deliveryFilter()
                    {
                        start = new dateValue[1]
                        {
                            new dateValue()
                            {
                                @operator = filterOperator.After,
                                value = from,
                                operatorSpecified = true,
                                valueSpecified = true
                            }
                        }
                    },
                    includeContent = false,
                    includeRecipients = false,
                    pageNumber = pageNumber
                })).@return;
                if (deliveries != null)
                {
                    result.AddRange(deliveries);
                    pageNumber++;
                }
            } while (deliveries != null && deliveries.Length > 0);
            return result.ToArray();
        }

        public decimal GetOpenRate(deliveryObject[] info, DateTime startDate, DateTime endDate)
        {
            var filtered = info.Where(d => d.start > startDate && d.start < endDate.AddDays(1)).ToArray();
            long sent = filtered.Sum(d => d.numSends);
            long opened = filtered.Sum(d => d.uniqOpens);
            return sent == 0 ? 0 : (opened / (decimal)sent);
        }
    }
}