using System;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrderRegionFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int? IdCustomerType { get; set; }

        public int? IdOrderType { get; set; }

        public string Region { get; set; }

        public string Zip { get; set; }
    }
}