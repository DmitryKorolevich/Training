using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerToPaymentMethod : Entity
	{
		public int IdCustomer { get; set; }

		public int IdPaymentMethod { get; set; }
	}
}
