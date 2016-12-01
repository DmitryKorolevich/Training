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
            Map(m => m.ProductCategories).Name("Product Categories").Index(0);
            Map(m => m.IdProduct).Name("Product #").Index(1);
            Map(m => m.ProductName).Name("Product Description").Index(2);
            Map(m => m.Code).Name("SKU").Index(3);
            Map(m => m.SkuName).Name("Description").Index(4);
            Map(m => m.StatusCode).Name("Active").Index(5).TypeConverter<YesNoRecordStatusConverter>();
            Map(m => m.CurrentOOS).Name("Currently OOS").Index(6);
            Map(m => m.DaysOOS).Name("Days OOS").Index(7);
            Map(m => m.AverageDailyAmount).Name("Avg Daily Sales").Index(8).TypeConverterOption("c");
            Map(m => m.TotalAmount).Name("Total Sales").Index(9).TypeConverterOption("c");
            Map(m => m.TotalOOSImpactAmount).Name("OOS Sales Impact").Index(10).TypeConverterOption("c");
        }
    }
}
