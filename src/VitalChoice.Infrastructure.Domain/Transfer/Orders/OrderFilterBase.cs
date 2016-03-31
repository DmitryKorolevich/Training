namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrderFilterBase : FilterBase
    {
        public string Id { get; set; }

	    public int? IdCustomer { get; set; }
    }
}