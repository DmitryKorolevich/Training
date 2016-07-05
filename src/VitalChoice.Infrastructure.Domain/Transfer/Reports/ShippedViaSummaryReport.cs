using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class ShippedViaSummaryReport
    {
        public ShippedViaSummaryReport()
        {
            Warehouses=new List<ShippedViaSummaryReportWarehouseItem>();
        }

        public ICollection<ShippedViaSummaryReportWarehouseItem> Warehouses { get; set; }
    }
}