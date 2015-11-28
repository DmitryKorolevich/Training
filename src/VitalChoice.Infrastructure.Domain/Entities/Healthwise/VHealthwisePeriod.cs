using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Healthwise
{
    public class VHealthwisePeriod : Entity
	{
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

	    public decimal? PaidAmount { get; set; }

        public DateTime? PaidDate { get; set; }

        public int IdCustomer { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public int OrdersCount { get; set; }

        public decimal OrderSubtotals { get; set; }

        public DateTime LastOrderDate { get; set; }
    }
}
