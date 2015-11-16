namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class VAffiliateFilter : FilterBase
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Company { get; set; }

        public int? Tier { get; set; }

        public bool WithAvailablePayCommision { get; set; }
    }
}