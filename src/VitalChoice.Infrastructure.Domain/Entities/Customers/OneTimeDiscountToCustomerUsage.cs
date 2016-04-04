using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Customers
{
    public class OneTimeDiscountToCustomerUsage : Entity
    {
        public int IdCustomer { get; set; }
        public int IdDiscount { get; set; }
        public int UsageCount { get; set; }
    }
}