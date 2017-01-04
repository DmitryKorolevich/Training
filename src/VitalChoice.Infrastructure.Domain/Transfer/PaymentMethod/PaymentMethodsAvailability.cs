using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.PaymentMethod
{
    public enum MarketingPromotionType
    {
        Promotion = 1,
        Donation = 2,
        DonationtoNonProfit = 4,
        CorporateGiftfromVc = 5,
        MarketingGiftfromVc = 6,
        CompanyEvent = 7,
        VendorEventRelationship = 8,
        ProductDevelopment = 9,
        TradeshowConfSeminar = 10
    }

    public class PaymentMethodsAvailability
    {
	    public int Id { get; set; }
	    public IList<int> CustomerTypes { get; set; }

        public PaymentMethodsAvailability()
        {
            CustomerTypes = new List<int>();
        }
    }
}
