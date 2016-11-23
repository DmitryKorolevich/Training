using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuAverageDailySalesBySkuReportItem
    {
        public int Id { get; set; }

        public int IdProduct { get; set; }

        public string ProductCategories { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public string Code { get; set; }

        public string SkuName { get; set; }

        public string ProductName { get; set; }

        public bool InStock { get; set; }

        public bool CurrentOOS { get; set; }

        public int DaysOOS { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal AverageDailyAmount { get; set; }

        public decimal TotalOOSImpactAmount { get; set; }
    }
}