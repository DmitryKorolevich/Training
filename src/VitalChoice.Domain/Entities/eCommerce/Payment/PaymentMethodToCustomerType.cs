﻿using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.Domain.Entities.eCommerce.Payment
{
    public class PaymentMethodToCustomerType:Entity
    {
		public PaymentMethod PaymentMethod { get; set; }

		public CustomerTypeEntity CustomerType { get; set; }

		public int IdPaymentMethod { get; set; }

		public int IdCustomerType { get; set; }
    }
}
