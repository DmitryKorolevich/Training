using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Healthwise
{
    public class HealthwisePeriod : Entity
	{
	    public int IdCustomer { get; set; }

        public Customer Customer { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

	    public decimal? PaidAmount { get; set; }

        public DateTime? PaidDate { get; set; }

        public ICollection<HealthwiseOrder> HealthwiseOrders { get; set; }
    }
}
