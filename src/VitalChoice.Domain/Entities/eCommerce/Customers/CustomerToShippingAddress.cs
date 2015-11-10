using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Addresses;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class CustomerToShippingAddress : Entity
    {
        public int IdCustomer { get; set; }

        public Customer Customer { get; set; }

        public int IdAddress { get; set; }

        public Address ShippingAddress { get; set; }
    }
}
