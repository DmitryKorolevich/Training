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
    public class SkuInventoriesInfoItemCsvMap : CsvClassMap<SkuInventoriesInfoItem>
    {
        public SkuInventoriesInfoItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Code).Name("SKU").Index(0);
            Map(m => m.StatusCode).Name("Active").Index(1).TypeConverter<YesNoRecordStatusConverter>();
            Map(m => m.ProductStatusCode).Name("Product Active").Index(2).TypeConverter<YesNoRecordStatusConverter>();
            Map(m => m.InventoriesLine).Name("Parts").Index(3);
        }
    }
}
