using System.Collections.Generic;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class VCustomerFavoritesFilter : FilterBase
    {
	    public int IdCustomer { get; set; }
    }
}