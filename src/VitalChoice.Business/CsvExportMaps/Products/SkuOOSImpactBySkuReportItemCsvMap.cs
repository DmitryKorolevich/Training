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
    public class SkuOOSImpactBySkuReportItemCsvMap : CsvClassMap<SkuAverageDailySalesBySkuReportItem>
    {
        public SkuOOSImpactBySkuReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.IdProduct).Name("Product #").Index(0);
            Map(m => m.Code).Name("SKU").Index(1);
            Map(m => m.SkuName).Name("Description").Index(2);
            Map(m => m.AverageDailyAmount).Name("Avg Daily Sales").Index(3);
            Map(m => m.TotalAmount).Name("Total Sales").Index(4);
            Map(m => m.DaysOOS).Name("Days OOS").Index(5);
        }
    }
}
