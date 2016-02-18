using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Helpers
{
    public static class DynamicViewHelper
    {
        public static List<KeyValuePair<string, string>> PopulateBillingAddressDetails(this AddressDynamic billingAddress, ICollection<Country> countries, string email)
        {
            return new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>(string.Empty, $"{billingAddress.Data.FirstName} {billingAddress.Data.LastName}"),
                new KeyValuePair<string, string>(string.Empty, billingAddress.SafeData.Company),
                new KeyValuePair<string, string>(string.Empty, billingAddress.Data.Address1),
                new KeyValuePair<string, string>(string.Empty, billingAddress.SafeData.Address2),
                new KeyValuePair<string, string>(string.Empty, $"{billingAddress.Data.City}, {ResolveStateOrCounty(countries, billingAddress)} {billingAddress.Data.Zip}"),
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
                new KeyValuePair<string, string>(string.Empty, $"{shippingAddress.Data.City}, {ResolveStateOrCounty(countries, shippingAddress)} {shippingAddress.Data.Zip}"),
                new KeyValuePair<string, string>("Phone", shippingAddress.SafeData.Phone!=null ? ((string)shippingAddress.SafeData.Phone).FormatAsPhone(BaseAppConstants.BASE_PHONE_FORMAT) : String.Empty),
            };
        }

        private static string ResolveStateOrCounty(ICollection<Country> countries, AddressDynamic address)
        {
            var target = countries.Single(x => x.Id == address.IdCountry);

            var stateOrCounty = address.IdState.HasValue ? target.States.Single(x => x.Id == address.IdState.Value).StateCode : address.County;

            return stateOrCounty;
        }

        public static string GetDiscountMessage(this DiscountDynamic discount, int? IdTier=null)
        {
            string toReturn = null;
            if (discount == null)
                return toReturn;
            switch (discount.IdObjectType)
            {
                case (int)DiscountType.FreeShipping:
                    toReturn = "Free Shipping Discount";
                    break;
                case (int)DiscountType.PercentDiscount:
                    if (discount.SafeData.Percent != null)
                    {
                        toReturn = $"Percent Discount ({(decimal)discount.SafeData.Percent / 100:P0})";
                    }
                    break;
                case (int)DiscountType.PriceDiscount:
                    if (discount.SafeData.Amount != null)
                    {
                        toReturn = $"Price Discount ({discount.SafeData.Amount:C})";
                    }
                    break;
                case (int)DiscountType.Threshold:
                    if (discount.SafeData.ProductSKU != null)
                    {
                        toReturn = $"Threshold Discount ({discount.SafeData.ProductSKU})";
                    }
                    break;
                case (int)DiscountType.Tiered:
                    if (IdTier.HasValue)
                    {
                        var neededTier = discount.DiscountTiers?.FirstOrDefault(p => p.Id == IdTier.Value);
                        if (neededTier != null)
                        {
                            switch (neededTier.IdDiscountType)
                            {
                                case DiscountType.PriceDiscount:
                                    toReturn =
                                        $"Tiered Discount, Tier from {neededTier.From:C} to {neededTier.To:C} ({neededTier.Amount ?? 0:C})";
                                    break;
                                case DiscountType.PercentDiscount:
                                    toReturn =
                                        $"Tiered Discount, Tier from {neededTier.From:C} to {neededTier.To:C} ({(neededTier.Percent ?? 0)/100:P0})";
                                    break;
                            }
                        }
                    }
                    break;
            }
            return toReturn;
        }
    }
}