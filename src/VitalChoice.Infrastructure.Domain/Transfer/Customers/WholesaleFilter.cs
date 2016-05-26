namespace VitalChoice.Infrastructure.Domain.Transfer.Customers
{
    public class WholesaleFilter : FilterBase
    {
        public int? IdTradeClass { get; set; }

        public int? IdTier { get; set; }

        public bool OnlyActive { get; set; }
	}
}