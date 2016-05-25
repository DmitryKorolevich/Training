using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Customers
{
    public class CustomerLastOrder : Entity
    {
        public DateTime LastOrderDate { get; set; }
    }
}
