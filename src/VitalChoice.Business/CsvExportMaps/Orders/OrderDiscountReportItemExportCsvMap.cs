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
    public class OrderDiscountReportItemExportCsvMap : CsvClassMap<OrderDiscountReportItem>
    {
        public OrderDiscountReportItemExportCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Id).Name("Order #").Index(0);
            Map(m => m.OrderStatus).Name("Status").Index(1).TypeConverter<OrderStatusConverter>();
            Map(m => m.POrderStatus).Name("P Status").Index(2).TypeConverter<OrderStatusConverter>();
            Map(m => m.NPOrderStatus).Name("NP Status").Index(3).TypeConverter<OrderStatusConverter>();
            Map(m => m.DateCreated).Name("Order Date").Index(4).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.CustomerEmail).Name("Email").Index(5);
            Map(m => m.CustomerFirstName).Name("First Name").Index(6);
            Map(m => m.CustomerLastName).Name("Last Name").Index(7);
            Map(m => m.Total).Name("Order Total").Index(8).TypeConverterOption("c");
            Map(m => m.DiscountCode).Name("Discount Code").Index(9);
            Map(m => m.DiscountMessage).Name("Discount Type").Index(10);
            Map(m => m.DiscountInfo).Name("% / $").Index(10);
            Map(m => m.DiscountTotal).Name("Discount Amount").Index(11).TypeConverterOption("c");
        }
    }
}
