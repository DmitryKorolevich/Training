using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class ProductQualitySalesReportItemSortPath
    {
        public const string Sales = "Sales";
        public const string Issues = "Issues";
        public const string SalesPerIssue = "SalesPerIssue";
    }
}
