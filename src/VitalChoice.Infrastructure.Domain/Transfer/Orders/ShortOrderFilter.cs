namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class ShortOrderFilter : FilterBase
    {
        public string Id { get; set; }

	    public int? IdCustomer { get; set; }
    }
}