using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Entities.eCommerce.Users
{
    public class User: Entity
    {
	    public Customer Customer { get; set; }

        public AdminProfile AdminProfile { get; set; }
    }
}