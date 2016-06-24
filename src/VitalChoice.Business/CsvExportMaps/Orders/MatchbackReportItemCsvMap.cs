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
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class MatchbackReportItemCsvMap : CsvClassMap<MatchbackReportItem>
    {
        public MatchbackReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.IdOrder).Name("Order #").Index(0);
            Map(m => m.OrderSource).Name("Order Source").Index(1);
            Map(m => m.IdCustomer).Name("Customer #").Index(2);
            Map(m => m.DateCreated).Name("Order Date").Index(3).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");

            Map(m => m.BillingFirstName).Name("First Name").Index(4);
            Map(m => m.BillingLastName).Name("Last Name").Index(5);
            Map(m => m.BillingAddress1).Name("Address1").Index(6);
            Map(m => m.BillingAddress2).Name("Address2").Index(7);
            Map(m => m.BillingCountyCode).Name("Country").Index(8);
            Map(m => m.BillingStateCode).Name("State").Index(9);
            Map(m => m.BillingCity).Name("City").Index(10);
            Map(m => m.BillingZip).Name("Zip").Index(11);

            Map(m => m.KeyCode).Name("Key Code").Index(12);
            Map(m => m.Source).Name("\"Heard Of\"").Index(13);
            Map(m => m.DiscountCode).Name("Discount Code").Index(14);
            Map(m => m.ProductsSubtotal).Name("Products Total").Index(15).TypeConverterOption("c");
            Map(m => m.ShippingTotal).Name("Shipping Total").Index(16).TypeConverterOption("c");
            Map(m => m.Total).Name("Total").Index(17).TypeConverterOption("c");
        }
    }
}
