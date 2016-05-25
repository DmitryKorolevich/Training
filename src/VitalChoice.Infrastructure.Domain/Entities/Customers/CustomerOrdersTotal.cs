using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Customers
{
    public class CustomerOrdersTotal : Entity
    {
        public decimal Total { get; set; }
    }
}
