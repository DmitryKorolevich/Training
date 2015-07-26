using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Business.Helpers
{
    public static class EnumHelper
    {
        public static string GetContentItemStatusName(CustomerTypeCode code, RecordStatusCode status)
        {
            string toReturn = null;
            if (status == RecordStatusCode.NotActive)
            {
                toReturn = "Draft";
            }
            if (status == RecordStatusCode.Active)
            {
                if (code == CustomerTypeCode.All)
                {
                    toReturn = "Published - All";
                }
                if (code == CustomerTypeCode.Wholesale)
                {
                    toReturn = "Published - Wholesale Only";
                }
            }
            return toReturn;
        }

        public static Dictionary<string, string> GetContentItemStatusNames()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add($"{(int)RecordStatusCode.NotActive}:{(int)CustomerTypeCode.All}", "Draft");
            toReturn.Add($"{(int)RecordStatusCode.Active}:{(int)CustomerTypeCode.All}", "Published - All");
            toReturn.Add($"{(int)RecordStatusCode.Active}:{(int)CustomerTypeCode.Wholesale}", "Published - Wholesale Only");
            return toReturn;
        }

        public static Dictionary<string, string> GetProductCategoryStatusNames()
        {
            Dictionary<string, string> toReturn = new Dictionary<string, string>();
            toReturn.Add($"{(int)RecordStatusCode.NotActive}:{(int)CustomerTypeCode.All}", "Hide within storefront");
            toReturn.Add($"{(int)RecordStatusCode.Active}:{(int)CustomerTypeCode.All}", "Active - All Customers");
            toReturn.Add($"{(int)RecordStatusCode.Active}:{(int)CustomerTypeCode.Wholesale}", "Active - Wholesale Customers Only");
            return toReturn;
        }

        public static string GetGCTypeName(GCType type)
        {
            string toReturn = null;
            if (type == GCType.ManualGC)
            {
                toReturn = "Manually Created Gift Certificate";
            }
            return toReturn;
        }

        public static Dictionary<int, string> GetGCTypeNames()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();
            toReturn.Add((int)GCType.ManualGC, "Manually Created Gift Certificate");
            return toReturn;
        }

        public static Dictionary<int, string> GetRecordStatuses()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) RecordStatusCode.Active, "Active"},
                {(int) RecordStatusCode.NotActive, "Not Active"},
                {(int) RecordStatusCode.Deleted, "Deleted"}
            };
            return toReturn;
        }

        public static Dictionary<int, string> GetProductTypes()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) ProductType.Perishable, "Perishable"},
                {(int) ProductType.NonPerishable, "Non Perishable"},
                {(int) ProductType.Gc, "Gift Certificate"},
                {(int) ProductType.EGс, "E Gift Certificate"}
            };
            return toReturn;
        }

        public static Dictionary<int, string> GetDiscountTypes()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) DiscountType.PercentDiscount, "Percent"},
                {(int) DiscountType.PriceDiscount, "Price"},
                {(int) DiscountType.FreeShipping, "Free Shipping"},
                {(int) DiscountType.Threshold, "Threshold"},
                {(int) DiscountType.Tiered, "Tiered"},
            };
            return toReturn;
        }

        public static string GetDiscountTypeName(DiscountType type)
        {
            string toReturn = null;
            switch (type)
            {
                case DiscountType.PriceDiscount:
                    toReturn = "Price";
                    break;
                case DiscountType.PercentDiscount:
                    toReturn = "Percent";
                    break;
                case DiscountType.FreeShipping:
                    toReturn = "Free Shipping";
                    break;
                case DiscountType.Threshold:
                    toReturn = "Threshold";
                    break;
                case DiscountType.Tiered:
                    toReturn = "Tiered";
                    break;
            }
            return toReturn;
        }

        public static Dictionary<int, string> GetAssignedCustomerTypes()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) CustomerTypeCode.All, "All"},
                {(int) CustomerTypeCode.Retail, "Retail Only"},
                {(int) CustomerTypeCode.Wholesale, "Wholesale Only"},
            };
            return toReturn;
        }

        public static string GetAssignedCustomerTypeName(CustomerTypeCode type)
        {
            string toReturn = null;
            switch (type)
            {
                case CustomerTypeCode.All:
                    toReturn = "All";
                    break;
                case CustomerTypeCode.Retail:
                    toReturn = "Retail Only";
                    break;
                case CustomerTypeCode.Wholesale:
                    toReturn = "Wholesale Only";
                    break;
            }
            return toReturn;
        }

        public static Dictionary<int, string> GetActiveFilterOptions()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) -1, "All" },
                {(int) RecordStatusCode.Active, "Active"},
                {(int) RecordStatusCode.NotActive, "Not Active"}
            };
            return toReturn;
        }

        public static Dictionary<int, string> GetCreditCardTypes()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) CreditCardType.MasterCard, "MasterCard"},
                {(int) CreditCardType.Visa, "Visa"},
                {(int) CreditCardType.AmericanExpress, "American Express"},
                {(int) CreditCardType.Discover, "Discover"}
            };
            return toReturn;
        }
    }
}