using System;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class VCustomerInAffiliate : Entity
    {
        public string Name { get; set; }

        public int Count { get; set; }
    }
}