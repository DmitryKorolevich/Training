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
    public class OrdersSummarySalesOrderItemCsvMap : CsvClassMap<OrdersSummarySalesOrderItem>
    {
        public OrdersSummarySalesOrderItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Id).Name("Order #").Index(0);
            Map(m => m.OrderStatus).Name("Status").Index(1).TypeConverter<OrderStatusConverter>();
            Map(m => m.POrderStatus).Name("P Status").Index(2).TypeConverter<OrderStatusConverter>();
            Map(m => m.NPOrderStatus).Name("NP Status").Index(3).TypeConverter<OrderStatusConverter>();
            Map(m => m.CustomerFirstName).Name("First Name").Index(4);
            Map(m => m.CustomerLastName).Name("Last Name").Index(5);
            Map(m => m.CustomerCompany).Name("Company").Index(6);
            Map(m => m.DateCreated).Name("Order Date").Index(7).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.ProductsSubtotal).Name("Product Total").Index(8).TypeConverterOption("c");
            Map(m => m.Total).Name("Total").Index(9).TypeConverterOption("c");
            Map(m => m.DiscountCode).Name("Discount Code").Index(10);
            Map(m => m.IdAffiliate).Name("Affiliate Id").Index(11);
            Map(m => m.KeyCode).Name("Key Code").Index(12);
            Map(m => m.SourceName).Name("\"Heard Of\"").Index(13);
            Map(m => m.SourceDetails).Name("Details").Index(14);
            Map(m => m.OrdersCount).Name("Repeat").Index(15);
            Map(m => m.FirstOrderDate).Name("Inception Date").Index(16).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
        }
    }
}
