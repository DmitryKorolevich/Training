using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Customers
{
    public class VOrderCountOnCustomer : Entity
    {
        public int IdCustomer { get; set; }

        public long Count { get; set; }
    }
}
