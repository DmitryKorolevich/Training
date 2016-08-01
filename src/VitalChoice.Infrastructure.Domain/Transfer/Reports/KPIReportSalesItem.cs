using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class KPIReportSalesItem
    {
        public decimal Total { get; set; }

        public decimal NewCustomers { get; set; }

        public decimal Wholesales { get; set; }

        public decimal NewWholesales { get; set; }

        public decimal Affiliates { get; set; }

        public decimal NewAffiliates { get; set; }

        public decimal OrganicSearch { get; set; }

        public decimal PaidSearch { get; set; }
    }
}