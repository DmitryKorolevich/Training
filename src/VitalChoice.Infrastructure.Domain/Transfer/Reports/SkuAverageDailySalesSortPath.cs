using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public static class SkuAverageDailySalesSortPath
    {
        public const string Id = "Id";
        public const string IdProduct = "IdProduct";
        public const string Code = "Code";
        public const string InStock = "InStock";
        public const string DaysOOS = "DaysOOS";
        public const string TotalAmount = "TotalAmount";
        public const string AverageDailyAmount = "AverageDailyAmount";
        public const string AverageDailyQuantity = "AverageDailyQuantity";
        public const string TotalOOSImpactAmount = "TotalOOSImpactAmount";
    }
}