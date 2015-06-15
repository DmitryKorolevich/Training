using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Transfer.Order
{
    public class ExtendedOrderNote: OrderNote
    {
		public AdminProfile AdminProfile { get; set; }
	}
}
