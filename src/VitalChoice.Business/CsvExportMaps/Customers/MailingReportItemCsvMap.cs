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

namespace VitalChoice.Business.CsvExportMaps.Customers
{
    public class MailingReportItemCsvMap : CsvClassMap<MailingReportItem>
    {
        public MailingReportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Id).Name("Customer #").Index(0);
            Map(m => m.FirstName).Name("First Name").Index(1);
            Map(m => m.LastName).Name("Last Name").Index(2);
            Map(m => m.Address1).Name("Address1").Index(3);
            Map(m => m.Address2).Name("Address2").Index(4);
            Map(m => m.CountryCode).Name("Country").Index(5);
            Map(m => m.StateCode).Name("State").Index(6);
            Map(m => m.City).Name("City").Index(7);
            Map(m => m.Zip).Name("Zip").Index(8);
            Map(m => m.Phone).Name("Phone").Index(9);
            Map(m => m.FirstDiscountCode).Name("First Discount Code").Index(10);
            Map(m => m.CustomerIdObjectType).Name("Type").Index(11).TypeConverter<CustomerTypeConverter>();
            Map(m => m.Email).Name("Email").Index(12);
            Map(m => m.LastOrderDateCreated).Name("Last Order Date").Index(13).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.LastOrderTotal).Name("Last Order Total").Index(14).TypeConverterOption("c");
            Map(m => m.FirstOrderDateCreated).Name("Entry Date").Index(15).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.OrdersCount).Name("Order Frequency").Index(16);
            Map(m => m.OrdersTotal).Name("Total Of Sales").Index(17).TypeConverterOption("c");
            Map(m => m.CustomerOrderSource).Name("First Source").Index(18);
            Map(m => m.FirstKeyCode).Name("First KeyCode").Index(19);
            Map(m => m.DoNotMail).Name("DNM").Index(20);
            Map(m => m.DoNotRent).Name("DNR").Index(21);
        }
    }
}
