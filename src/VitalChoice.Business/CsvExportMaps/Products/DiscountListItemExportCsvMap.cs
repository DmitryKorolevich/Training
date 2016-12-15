using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities.Reports;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Discounts;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.CsvExportMaps.Products
{
    public class DiscountListItemExportCsvMap : CsvClassMap<DiscountListItemModel>
    {
        public DiscountListItemExportCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Code).Name("Code").Index(1);
            Map(m => m.Description).Name("Description").Index(2);
            Map(m => m.DiscountType).Name("Type").Index(3).TypeConverter<DiscountTypeNameConverter>();
            Map(m => m.Assigned).Name("Customer").Index(4).TypeConverter<CustomerTypeConverter>();
            Map(m => m.StatusCode).Name("Active").Index(5).TypeConverter<RecordStatusCodeConverter>();
            Map(m => m.StartDate).Name("Valid From").Index(6).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.ExpirationDate).Name("Valid To").Index(7).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.DateStatus).Name("Status").Index(8).TypeConverter<DateStatusConverter>();
            Map(m => m.DateCreated).Name("Created").Index(9).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy hh:mm tt");
            Map(m => m.AddedByAgentId).Name("Created By").Index(10);
            Map(m => m.DateEdited).Name("Updated").Index(11).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy hh:mm tt");
            Map(m => m.EditedByAgentId).Name("Updated By").Index(12);
        }
    }
}
