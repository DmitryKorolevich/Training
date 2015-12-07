using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class VOrderWithRegionInfoItem : Entity
    {
        public DateTime DateCreated { get; set; }

        public int IdCustomer { get; set; }

        public string Zip { get; set; }

        public string Region { get; set; }

        public string City { get; set; }

        public decimal Total { get; set; }

        public int IdCustomerType { get; set; }

        public int? OrderType { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public int CustomerOrdersCount { get; set; }
    }
}
