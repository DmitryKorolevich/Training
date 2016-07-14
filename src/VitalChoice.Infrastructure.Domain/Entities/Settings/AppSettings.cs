using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.Infrastructure.Domain.Entities.Settings
{
    public class AppSettings
    {
        [Map]
        public decimal? GlobalPerishableThreshold { get; set; }

        [Map]
        public bool CreditCardAuthorizations { get; set; }

        [Map]
        public int HealthwisePeriodMaxItemsCount { get; set; }

        [Map]
        public string ProductOutOfStockEmailTemplate { get; set; }

        [Map]
        public string AffiliateEmailTemplate { get; set; }
    }
}
