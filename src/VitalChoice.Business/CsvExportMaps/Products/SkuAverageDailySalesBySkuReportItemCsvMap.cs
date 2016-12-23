using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.CsvExportMaps.Products
{
    public class SkuAverageDailySalesBySkuReportItemCsvMap : CsvClassMap<SkuAverageDailySalesBySkuReportItem>
    {
        public SkuAverageDailySalesBySkuReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Code).Name("SKU").Index(0);
            Map(m => m.SkuName).Name("SKU Description").Index(1);
            Map(m => m.IdProduct).Name("Product #").Index(2);
            Map(m => m.ProductName).Name("Product Page Description").Index(3);
            Map(m => m.StatusCode).Name("Active").Index(4).TypeConverter<YesNoRecordStatusConverter>();
            Map(m => m.CurrentOOS).Name("Currently OOS").Index(5);
            Map(m => m.DaysOOS).Name("Days OOS").Index(6);
            Map(m => m.AverageDailyAmount).Name("Avg. Daily Sales").Index(7).TypeConverterOption("c");
            Map(m => m.AverageDailyQuantity).Name("Avg. Daily Units").Index(8).TypeConverterOption("N2");
            Map(m => m.TotalAmount).Name("Total Sales").Index(9).TypeConverterOption("c");
            Map(m => m.TotalOOSImpactAmount).Name("OOS Sales Impact").Index(11).TypeConverterOption("c");
            Map(m => m.ProductCategories).Name("Product Categories").Index(12);
        }
    }
}
