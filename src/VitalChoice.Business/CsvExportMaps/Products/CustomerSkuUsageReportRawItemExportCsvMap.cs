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
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.CsvExportMaps.Products
{
    public class CustomerSkuUsageReportRawItemExportCsvMap : CsvClassMap<CustomerSkuUsageReportRawItem>
    {
        public CustomerSkuUsageReportRawItemExportCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.CategoryNames).Name("Product Category").Index(0);
            Map(m => m.Code).Name("SKU").Index(1);
            Map(m => m.Quantity).Name("Quantity").Index(2);
            Map(m => m.LastOrderDateCreated).Name("Last Order Date").Index(3).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.IdCustomer).Name("Customer #").Index(4);
            Map(m => m.CustomerType).Name("Customer Type").Index(5).TypeConverter<CustomerTypeConverter>();
            Map(m => m.ShippingFirstName).Name("Ship to First Name").Index(6);
            Map(m => m.ShippingLastName).Name("Ship to Last Name").Index(7);
            Map(m => m.ShippingAddress1).Name("Ship to Address1").Index(8);
            Map(m => m.ShippingAddress2).Name("Ship to Address2").Index(9);
            Map(m => m.ShippingCity).Name("Ship to City").Index(10);
            Map(m => m.ShippingStateCode).Name("Ship to State").Index(11);
            Map(m => m.ShippingZip).Name("Ship to Zip").Index(12);
            Map(m => m.ShippingCountryCode).Name("Ship to Country").Index(13);
            Map(m => m.DoNotMail).Name("DNM flag").Index(14);
            Map(m => m.Email).Name("Email").Index(15);
        }
    }
}
