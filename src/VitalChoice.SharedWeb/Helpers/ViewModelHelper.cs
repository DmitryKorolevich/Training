using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.SharedWeb.Helpers
{
    public static class ViewModelHelper
    {
        public static List<KeyValuePair<string, string>> PopulateBillingAddressDetails(this AddressDynamic billingAddress,
            ICountryNameCodeResolver nameCodeResolver, string email)
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(string.Empty, $"{billingAddress?.SafeData.FirstName} {billingAddress?.SafeData.LastName}"),
                new KeyValuePair<string, string>(string.Empty, billingAddress?.SafeData.Company),
                new KeyValuePair<string, string>(string.Empty, billingAddress?.SafeData.Address1),
                new KeyValuePair<string, string>(string.Empty, billingAddress?.SafeData.Address2),
                new KeyValuePair<string, string>(string.Empty,
                    $"{billingAddress?.SafeData.City}, {nameCodeResolver.GetRegionOrStateCode(billingAddress)} {billingAddress?.SafeData.Zip}"),
                new KeyValuePair<string, string>("Phone",
                    billingAddress?.SafeData.Phone != null
                        ? ((string) billingAddress.SafeData.Phone).FormatAsPhone(BaseAppConstants.BASE_PHONE_FORMAT)
                        : String.Empty),
                new KeyValuePair<string, string>("Email", email),
            };
        }

        public static List<KeyValuePair<string, string>> PopulateCreditCardDetails(this OrderPaymentMethodDynamic paymentMethod,
            ReferenceData referenceData, bool withLabels = false)
        {
            if (paymentMethod == null || paymentMethod.IdObjectType != (int) PaymentMethodType.CreditCard)
            {
                return new List<KeyValuePair<string, string>>();
            }

            if (withLabels)
            {
                return new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Credit Card",
                        referenceData.CreditCardTypes.FirstOrDefault(z => z.Key == (int?) paymentMethod.SafeData.CardType)?.Text),
                    new KeyValuePair<string, string>("Number", paymentMethod.SafeData.CardNumber),
                    new KeyValuePair<string, string>("Expiration",
                        $"{paymentMethod.SafeData.ExpDate?.Month}/{(paymentMethod.SafeData.ExpDate?.Year ?? 0)%2000}"),
                };
            }
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(string.Empty,
                    referenceData.CreditCardTypes.Single(z => z.Key == (int) paymentMethod.SafeData.CardType).Text),
                new KeyValuePair<string, string>(string.Empty, paymentMethod.SafeData.CardNumber),
                new KeyValuePair<string, string>(string.Empty,
                    $"{paymentMethod.SafeData.ExpDate?.Month}/{(paymentMethod.SafeData.ExpDate?.Year ?? 0)%2000}"),
            };
        }

        public static List<KeyValuePair<string, string>> PopulateShippingAddressDetails(this AddressDynamic shippingAddress,
            ICountryNameCodeResolver nameCodeResolver)
        {
            return new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>(string.Empty, $"{shippingAddress?.SafeData.FirstName} {shippingAddress?.SafeData.LastName}"),
                new KeyValuePair<string, string>(string.Empty, shippingAddress?.SafeData.Company),
                new KeyValuePair<string, string>(string.Empty, shippingAddress?.SafeData.Address1),
                new KeyValuePair<string, string>(string.Empty, shippingAddress?.SafeData.Address2),
                new KeyValuePair<string, string>(string.Empty,
                    $"{shippingAddress?.SafeData.City}, {nameCodeResolver.GetRegionOrStateCode(shippingAddress)} {shippingAddress?.SafeData.Zip}")
            };
        }
    }
}