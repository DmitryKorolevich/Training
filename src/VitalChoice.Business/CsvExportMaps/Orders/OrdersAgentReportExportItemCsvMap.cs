using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class OrdersAgentReportExportItemCsvMap : CsvClassMap<OrdersAgentReportExportItem>
    {
        public OrdersAgentReportExportItemCsvMap()
        {
            Map(m => m.Agent).Index(0);
            Map(m => m.OrdersCount).Index(1);
            Map(m => m.TotalOrdersAmount).Index(2);
            Map(m => m.AverageOrdersAmount).Index(3);
            Map(m => m.LowestOrderAmount).Index(4);
            Map(m => m.HighestOrderAmount).Index(5);
            Map(m => m.RefundsCount).Index(6);
            Map(m => m.ReshipsCount).Index(7);
        }
    }
}
