using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public enum CustomerStatus
    {
		NotActive = 1,
		Active = 2,
		Deleted = 3,
		Suspended = 4
	}
}
