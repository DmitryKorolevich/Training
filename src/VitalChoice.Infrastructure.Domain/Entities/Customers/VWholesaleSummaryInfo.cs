using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.Customers
{
    public class VWholesaleSummaryInfo : Entity
    {
        public DateTime DateCreated { get; set; }

        public bool NewCustomer { get; set; }

        public int? TradeClass { get; set; }

        public bool OrdersExist { get; set; }
    }
}
