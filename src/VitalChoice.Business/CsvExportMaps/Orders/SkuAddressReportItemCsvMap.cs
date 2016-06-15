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
    public class SkuAddressReportItemCsvMap : CsvClassMap<SkuAddressReportItem>
    {
        public SkuAddressReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.IdOrder).Name("Order #").Index(0);
            Map(m => m.DateCreated).Name("Order Date").Index(1).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.SkuCode).Name("SKU").Index(2);
            Map(m => m.Quantity).Name("Quantity").Index(3);
            Map(m => m.DateCreated).Name("SKU").Index(4);

            Map(m => m.BillingFirstName).Name("Bill to First Name").Index(5);
            Map(m => m.BillingLastName).Name("Bill to Last Name").Index(6);
            Map(m => m.BillingCompany).Name("Bill to Company").Index(7);
            Map(m => m.BillingAddress1).Name("Bill to Address1").Index(8);
            Map(m => m.BillingAddress2).Name("Bill to Address2").Index(9);
            Map(m => m.BillingCity).Name("Bill to City").Index(10);
            Map(m => m.BillingStateCode).Name("Bill to State").Index(11);
            Map(m => m.BillingZip).Name("Bill to Zip").Index(12);
            Map(m => m.BillingCountyCode).Name("Bill to Country").Index(13);
            Map(m => m.BillingPhone).Name("Bill to Phone").Index(14);

            Map(m => m.IdCustomer).Name("Customer #").Index(15);
            Map(m => m.DiscountCode).Name("Discount Code").Index(16);
            Map(m => m.Source).Name("Channel").Index(17);
            Map(m => m.Price).Name("Unit Price").Index(18).TypeConverterOption("c");
            Map(m => m.Amount).Name("Extended Price (Qty x Unit Price)").Index(19).TypeConverterOption("c");
            Map(m => m.Source).Name("DNM flag").Index(20);

            Map(m => m.ShippingFirstName).Name("Ship to First Name").Index(21);
            Map(m => m.ShippingLastName).Name("Ship to Last Name").Index(22);
            Map(m => m.ShippingCompany).Name("Ship to Company").Index(23);
            Map(m => m.ShippingAddress1).Name("Ship to Address1").Index(24);
            Map(m => m.ShippingAddress2).Name("Ship to Address2").Index(25);
            Map(m => m.ShippingCity).Name("Ship to City").Index(26);
            Map(m => m.ShippingStateCode).Name("Ship to State").Index(27);
            Map(m => m.ShippingZip).Name("Ship to Zip").Index(28);
            Map(m => m.ShippingCountyCode).Name("Ship to Country").Index(29);
            Map(m => m.ShippingPhone).Name("Ship to Phone").Index(30);
        }
    }
}
