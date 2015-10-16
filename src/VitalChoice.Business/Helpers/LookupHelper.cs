using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Promotions;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Helpers
{
    public static class LookupHelper
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

        public static Dictionary<int, string> GetCustomerStatuses()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) CustomerStatus.Active, "Active"},
                {(int) CustomerStatus.NotActive, "Not Active"},
                {(int) CustomerStatus.Deleted, "Deleted"},
                {(int) CustomerStatus.Suspended, "Suspended"},
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

        public static IList<LookupItem<int>> GetShortPaymentMethods(IList<LookupItem<int>> paymentMethods)
        {
            foreach(var paymentMethod in paymentMethods)
            {
                switch (paymentMethod.Key)
                {
                    case 1:
                        paymentMethod.Text = "CC";
                        break;
                    case 2:
                        paymentMethod.Text = "OAC";
                        break;
                    case 3:
                        paymentMethod.Text = "Check";
                        break;
                    case 4:
                        paymentMethod.Text = "NC";
                        break;
                    case 5:
                        paymentMethod.Text = "PP";
                        break;
                }
            }
            return paymentMethods;
        }

        public static IList<LookupItem<int?>> GetVisibleOptions()
        {
            IList<LookupItem<int?>> toReturn = new List<LookupItem<int?>>
            {
                new LookupItem<int?>() {Key = (int?) CustomerTypeCode.All, Text="All" },
                new LookupItem<int?>() {Key = (int?) CustomerTypeCode.Wholesale, Text="Wholesale Only" },
                new LookupItem<int?>() {Key = (int?) CustomerTypeCode.Retail, Text="Retail Only" },
                new LookupItem<int?>() {Key =null, Text="Hidden" },
            };
            return toReturn;
        }

        public static Dictionary<int, string> GetTicketStatuses()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) RecordStatusCode.Active, "Open"},
                {(int) RecordStatusCode.NotActive, "Closed"}
            };
            return toReturn;
        }

        public static Dictionary<int, string> GetPriorities()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) TicketPriority.High, "High"},
                {(int) TicketPriority.Medium, "Medium"},
                {(int) TicketPriority.Low, "Low"},
            };
            return toReturn;
        }


        public static string GetPromotionTypeName(PromotionType type)
        {
            string toReturn = null;
            switch (type)
            {
                case PromotionType.BuyXGetY:
                    toReturn = "Buy X Get Y";
                    break;
                case PromotionType.CategoryDiscount:
                    toReturn = "Category Discount";
                    break;
            }
            return toReturn;
        }
    }
}