namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class AffiliatesSummaryModel
    {
        public int AllAffiliates { get; set; }

        public int EngagedAffiliates { get; set; }

        public decimal EngagedPercent { get; set; }

        public int AffiliateCustomers { get; set; }
    }
}