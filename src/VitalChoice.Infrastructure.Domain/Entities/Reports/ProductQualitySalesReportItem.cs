using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class ProductQualitySalesReportItem : Entity
    { 
        public string Code { get; set; }

        public int Sales { get; set; }

        public int Issues { get; set; }

        public decimal SalesPerIssue { get; set; }
    }
}
