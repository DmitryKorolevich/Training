using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class KPICacheItem : Entity
    {
        public DateTime DateCreated { get; set; }

        public string Data { get; set; }
    }
}
