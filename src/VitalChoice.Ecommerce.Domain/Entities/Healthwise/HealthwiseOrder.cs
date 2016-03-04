using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Entities.Healthwise
{
    public class HealthwiseOrder : Entity
	{
        public Order Order { get; set; }

        public int IdHealthwisePeriod { get; set; }

        public HealthwisePeriod HealthwisePeriod { get; set; }
    }
}
