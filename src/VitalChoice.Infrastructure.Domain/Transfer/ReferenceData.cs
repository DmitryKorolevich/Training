using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Entities.Settings;

namespace VitalChoice.Infrastructure.Domain.Transfer
{
    public class ReferenceData
    {
        public AppSettings AppSettings { get; set; }

        public Ecommerce.Domain.Entities.Addresses.Country DefaultCountry { get; set; }

        public IList<LookupItem<int>> AdminRoles { get; set; }

        public IList<LookupItem<int>> CustomerRoles { get; set; }

        public IList<LookupItem<int>> AffiliateRoles { get; set; }

        public IList<LookupItem<byte>> UserStatuses { get; set; }

        public IList<LookupItem<int>> ContentTypes { get; set; }

        public IList<ContentProcessorEntity> ContentProcessors { get; set; }

        public IList<LookupItem<string>> Labels { get; set; }

        public IList<LookupItem<int>> Months { get; set; }

        public string PublicHost { get; set; }

        public IList<LookupItem<int?>> VisibleOptions { get; set; }

        public IList<LookupItem<string>> ContentItemStatusNames { get; set; }

        public IList<LookupItem<string>> ProductCategoryStatusNames { get; set; }

        public IList<LookupItem<int>> GCTypes { get; set; }

        public IList<LookupItem<int>> GCShortTypes { get; set; }

        public IList<LookupItem<int>> RecordStatuses { get; set; }

        public IList<LookupItem<int?>> PublicRecordStatuses { get; set; }
        
        public IList<LookupItem<int>> CustomerStatuses { get; set; }

        public IList<LookupItem<int>> AffiliateStatuses { get; set; }

        public IList<LookupItem<int>> ProductTypes { get; set; }

        public IList<LookupItem<int>> ShortProductTypes { get; set; }

        public IList<LookupItem<int>> DiscountTypes { get; set; }

        public IList<LookupItem<int>> AssignedCustomerTypes { get; set; }

        public IList<LookupItem<int?>> ActiveFilterOptions { get; set; }

        public IList<LookupItem<int>> CustomerTypes { get; set; }

        public IList<LookupItem<int>> ShortCustomerTypes { get; set; }

        public IList<LookupItem<int>> OrderStatuses { get; set; }

        public IList<LookupItem<int>> PaymentMethods { get;set; }

        public IList<LookupItem<int>> ShortPaymentMethods { get; set; }

        public IList<LookupItem<int>> OrderTypes { get; set; }

        public IList<LookupItem<int>> PublicOrderTypes { get; set; }

        public IList<LookupItem<int>> ShortOrderTypes { get; set; }

        public IList<LookupItem<int>> TaxExempts { get; set; }

	    public List<LookupItem<int>> Tiers { get; set; }

	    public List<LookupItem<int>> TradeClasses { get; set; }

	    public List<LookupItem<int>> CustomerNotePriorities { get; set; }
        public IList<LookupItem<int>> OacTerms { get; set; }
        public IList<LookupItem<int>> OacFob { get; set; }
        public IList<LookupItem<int>> CreditCardTypes { get; set; }
        public IList<LookupItem<int>> MarketingPromotionTypes { get; set; }
        public IList<LookupItem<int>> ProductSellers { get; set; }
        public IList<LookupItem<int>> GoogleCategories { get; set; }

        public IList<LookupItem<int>> OrderSources { get; set; }
        public IList<LookupItem<int>> OrderSourcesCelebrityHealthAdvocate { get; set; }
        public IList<LookupItem<int>> OrderPreferredShipMethod { get; set; }
        public IList<LookupItem<int>> OrderSourceTypes { get; set; }
        public IList<LookupItem<int>> POrderTypes { get; set; }
        public IList<LookupItem<int?>> FilterPNPOrderTypes { get; set; }
        public IList<LookupItem<int>> ServiceCodes { get; set; }

        public IList<LookupItem<int>> AffiliateProfessionalPractices { get; set; }
        public IList<LookupItem<int>> AffiliateMonthlyEmailsSentOptions { get; set; }
        public IList<LookupItem<int>> AffiliateTiers { get; set; }

        public IList<LookupItem<int>> TicketStatuses { get; set; }
        public IList<LookupItem<int>> BugTicketStatuses { get; set; }
        public IList<LookupItem<int>> Priorities { get; set; }

        public IList<LookupItem<int>> PromotionTypes { get; set; }

        public IList<LookupItem<int?>> ExpiredTypes { get; set; }
        public IList<LookupItem<int?>> DateStatuses { get; set; }

        public IList<LookupItem<int>> PromotionBuyTypes { get; set; }

        public IList<LookupItem<string>> PersonTitles { get; set; }

	    public IList<LookupItem<int>> CartShippingOptions { get; set; }

        public IList<LookupItem<int>> ShippingUpgrades { get; set; }

        public IList<LookupItem<int>> AutoShipOptions { get; set; }

        public IList<LookupItem<int>> RefundRedeemOptions { get; set; }
    }
}