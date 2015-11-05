using System;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Orders
{
    public class ShortOrderFilter : FilterBase
    {
        public string Id { get; set; }

	    public int? IdCustomer { get; set; }
    }
}