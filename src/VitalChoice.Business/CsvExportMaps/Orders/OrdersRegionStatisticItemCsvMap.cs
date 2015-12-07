using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class OrdersRegionStatisticItemCsvMap : CsvClassMap<OrdersRegionStatisticItem>
    {
        public OrdersRegionStatisticItemCsvMap()
        {
            Map(m => m.Region).Name("Region").Index(0);
            Map(m => m.Amount).Name("Sales").Index(1).TypeConverterOption("c");
            Map(m => m.Count).Name("Orders").Index(2);
        }
    }
}
