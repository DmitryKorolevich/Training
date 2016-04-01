using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.SharedWeb.Helpers
{
    public static class ViewModelHelper
    {
        public static DateTime FindNextAutoShipDate(DateTime orderDate, int frequency)
        {
            if (frequency <= 0)
                throw new ApiException("Invalid auto-ship frequency");

            var difference = DateTime.Now - orderDate;
            var cyclesAdd = (int) (difference.Days/30.0/frequency);
            var next = orderDate.AddMonths(cyclesAdd*frequency);
            if (next < DateTime.Today)
            {
                next = next.AddMonths(frequency);
            }
            return next;
        }

        public static List<KeyValuePair<string, string>> PopulateBillingAddressDetails(this AddressDynamic billingAddress, ICollection<Country> countries, string email)
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(string.Empty, $"{billingAddress.Data.FirstName} {billingAddress.Data.LastName}"),
                new KeyValuePair<string, string>(string.Empty, billingAddress.SafeData.Company),
                new KeyValuePair<string, string>(string.Empty, billingAddress.Data.Address1),
                new KeyValuePair<string, string>(string.Empty, billingAddress.SafeData.Address2),
                new KeyValuePair<string, string>(string.Empty, $"{billingAddress.Data.City}, {BusinessHelper.ResolveStateOrCounty(countries, billingAddress)} {billingAddress.Data.Zip}"),
                new KeyValuePair<string, string>("Phone", billingAddress.SafeData.Phone!=null ? ((string)billingAddress.SafeData.Phone).FormatAsPhone(BaseAppConstants.BASE_PHONE_FORMAT) : String.Empty),
                new KeyValuePair<string, string>("Email", email),
            };
        }

        public static List<KeyValuePair<string, string>> PopulateCreditCardDetails(this OrderPaymentMethodDynamic paymentMethod, ReferenceData referenceData, bool withLabels=false)
        {
            if (paymentMethod.IdObjectType != (int)PaymentMethodType.CreditCard)
            {
                return new List<KeyValuePair<string, string>>();
            }

            if (withLabels)
            {
                return new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>("Credit Card",referenceData.CreditCardTypes.Single(z => z.Key == (int) paymentMethod.Data.CardType).Text),
                    new KeyValuePair<string, string>("Number", paymentMethod.Data.CardNumber),
                    new KeyValuePair<string, string>("Expiration",$"{paymentMethod.Data.ExpDate.Month}/{paymentMethod.Data.ExpDate.Year%2000}"),
                };
            }
            else
            {
                return new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(string.Empty,referenceData.CreditCardTypes.Single(z => z.Key == (int) paymentMethod.Data.CardType).Text),
                    new KeyValuePair<string, string>(string.Empty, paymentMethod.Data.CardNumber),
                    new KeyValuePair<string, string>(string.Empty,$"{paymentMethod.Data.ExpDate.Month}/{paymentMethod.Data.ExpDate.Year%2000}"),
                };
            }
        }

        public static List<KeyValuePair<string, string>> PopulateShippingAddressDetails(this AddressDynamic shippingAddress, ICollection<Country> countries)
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(string.Empty, $"{shippingAddress.Data.FirstName} {shippingAddress.Data.LastName}"),
                new KeyValuePair<string, string>(string.Empty, shippingAddress.SafeData.Company),
                new KeyValuePair<string, string>(string.Empty, shippingAddress.Data.Address1),
                new KeyValuePair<string, string>(string.Empty, shippingAddress.SafeData.Address2),
                new KeyValuePair<string, string>(string.Empty, $"{shippingAddress.Data.City}, {BusinessHelper.ResolveStateOrCounty(countries, shippingAddress)} {shippingAddress.Data.Zip}")
            };
        }
    }
}