﻿using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.CsvExportMaps.Products
{
    public class InventorySkuUsageReportItemForExportCsvMap : CsvClassMap<InventorySkuUsageReportItemForExport>
    {
        public InventorySkuUsageReportItemForExportCsvMap()
        {
            Map(m => m.SkuCode).Name("Product SKU").Index(0);
            Map(m => m.TotalSkuQuantity).Name("Quantity").Index(1);
            Map(m => m.Assemble).Name("Assemble").Index(2);
            Map(m => m.InventorySkuChannel).Name("Channel").Index(3);
            Map(m => m.BornDate).Name("Born Date").Index(4).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");

            Map(m => m.InvSkuCode).Name("Inventory SKU").Index(5);
            Map(m => m.TotalInvQuantityWithInvCorrection).Name("Inventory Quantity").Index(6);
            Map(m => m.InvDescription).Name("Inventory Description").Index(7);
            Map(m => m.UnitOfMeasure).Name("Inventory Unit of Measure").Index(8);
            Map(m => m.TotalUnitOfMeasureAmount).Name("Inventory Unit Amount").Index(9);
            Map(m => m.PurchaseUnitOfMeasure).Name("Purchase Unit of Measure").Index(10);
            Map(m => m.ProductSource).Name("Product Source").Index(11);
            Map(m => m.InventorySkuCategory).Name("Category").Index(12);
        }
    }
}
