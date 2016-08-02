using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class KPIReportDBSaleRawItem : Entity
    {
        public decimal Total { get; set; }

        public decimal NewCustomers { get; set; }

        public decimal Wholesales { get; set; }

        public decimal NewWholesales { get; set; }

        public decimal Affiliates { get; set; }

        public decimal NewAffiliates { get; set; }
    }
}
