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
    public class SkuAverageDailySalesByProductReportItemCsvMap : CsvClassMap<SkuAverageDailySalesByProductReportItem>
    {
        public SkuAverageDailySalesByProductReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.ProductCategories).Name("Product Categories").Index(0);
            Map(m => m.IdProduct).Name("Product #").Index(1);
            Map(m => m.ProductName).Name("Description").Index(2);
            Map(m => m.CurrentOOS).Name("Currently OOS").Index(3);
            Map(m => m.DaysOOS).Name("Days OOS").Index(4);
            Map(m => m.AverageDailyAmount).Name("Avg Daily Sales").Index(5).TypeConverterOption("c");
            Map(m => m.TotalAmount).Name("Total Sales").Index(3).TypeConverterOption("c");
            Map(m => m.TotalOOSImpactAmount).Name("OOS Sales Impact").Index(6).TypeConverterOption("c");
            Map(m => m.SkusLine).Name("Included SKUs").Index(7);
        }
    }
}
