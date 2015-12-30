using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Transfer;

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
                {(int) CustomerStatus.NotActive, "Phone Only"},
                {(int) CustomerStatus.Deleted, "Deleted"},
                {(int) CustomerStatus.Suspended, "Suspended"},
            };
            return toReturn;
        }

        public static Dictionary<int, string> GetAffiliateStatuses()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) AffiliateStatus.Active, "Active"},
                {(int) AffiliateStatus.Pending, "Pending"},
                {(int) AffiliateStatus.NotActive, "Not Active"},
                {(int) AffiliateStatus.Deleted, "Deleted"},
                {(int) AffiliateStatus.Suspended, "Suspended"},
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

        public static Dictionary<int, string> GetShortProductTypes()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>
            {
                {(int) ProductType.Perishable, "P"},
                {(int) ProductType.NonPerishable, "NP"},
                {(int) ProductType.Gc, "GIFT"},
                {(int) ProductType.EGс, "EGIFT"}
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
                    case 6:
                        paymentMethod.Text = "WT";
                        break;
                    case 8:
                        paymentMethod.Text = "Employee";
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

        public static IList<LookupItem<int?>> GetExpiredTypes()
        {
            IList<LookupItem<int?>> toReturn = new List<LookupItem<int?>>
            {
                new LookupItem<int?>() {Key = (int?) null, Text="All" },
                new LookupItem<int?>() {Key = (int?) ExpiredType.NotExpired, Text="Not Expired" },
                new LookupItem<int?>() {Key = (int?) ExpiredType.Expired, Text="Expired" },
            };
            return toReturn;
        }

        public static IList<LookupItem<int?>> GetDateStatuses()
        {
            IList<LookupItem<int?>> toReturn = new List<LookupItem<int?>>
            {
                new LookupItem<int?>() {Key = (int?) null, Text="All" },
                new LookupItem<int?>() {Key = (int?) DateStatus.Live, Text="Live" },
                new LookupItem<int?>() {Key = (int?) DateStatus.Future, Text="Future" },
                new LookupItem<int?>() {Key = (int?) DateStatus.Expired, Text="Expired" },
            };
            return toReturn;
        }

        public static string GetOrderStatusName(OrderStatus type)
        {
            string toReturn = null;
            switch (type)
            {
                case OrderStatus.Incomplete:
                    toReturn = "Incomplete";
                    break;
                case OrderStatus.Processed:
                    toReturn = "Processed";
                    break;
                case OrderStatus.Shipped:
                    toReturn = "Shipped";
                    break;
                case OrderStatus.Cancelled:
                    toReturn = "Cancelled";
                    break;
                case OrderStatus.Exported:
                    toReturn = "Exported";
                    break;
                case OrderStatus.ShipDelayed:
                    toReturn = "Ship Delayed";
                    break;
                case OrderStatus.OnHold:
                    toReturn = "On Hold";
                    break;
            }
            return toReturn;
        }

        public static IList<LookupItem<string>> GetPersonTitles()
        {
            IList<LookupItem<string>> toReturn = new List<LookupItem<string>>
            {
                new LookupItem<string>() {Key = "Mr.", Text="Mr." },
                new LookupItem<string>() {Key = "Ms.", Text="Ms." },
                new LookupItem<string>() {Key = "Miss.", Text="Miss." },
                new LookupItem<string>() {Key = "Mrs.", Text="Mrs." },
                new LookupItem<string>() {Key = "Dr.", Text="Dr." },
            };
            return toReturn;
        }
    }
}