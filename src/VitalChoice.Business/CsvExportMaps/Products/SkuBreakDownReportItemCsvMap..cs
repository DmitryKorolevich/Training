using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.CsvExportMaps.Products
{
    public class SkuBreakDownReportItemCsvMap : CsvClassMap<SkuBreakDownReportItem>
    {
        public SkuBreakDownReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Code).Name("SKU").Index(0);
            Map(m => m.TotalQuantity).Name("Quantity").Index(1);
            Map(m => m.TotalAmount).Name("Total").Index(2).TypeConverterOption("c");
            Map(m => m.RetailPercent).Name("Retail").Index(3).TypeConverter<PercentConverter>();
            Map(m => m.WholesalePercent).Name("Wholesale").Index(4).TypeConverter<PercentConverter>();
        }
    }
}
