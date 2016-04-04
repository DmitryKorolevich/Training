namespace VitalChoice.Infrastructure.Domain.Entities.Settings
{
    public class AppSettings
    {
        public decimal? GlobalPerishableThreshold { get; set; }

        public bool CreditCardAuthorizations { get; set; }

        public int HealthwisePeriodMaxItemsCount { get; set; }
    }
}
