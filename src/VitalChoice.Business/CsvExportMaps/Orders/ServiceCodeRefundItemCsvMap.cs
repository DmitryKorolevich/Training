using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class ServiceCodeRefundItemCsvMap : CsvClassMap<ServiceCodeRefundItem>
    {
        public ServiceCodeRefundItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Id).Name("Order Number").Index(0);
            Map(m => m.DateCreated).Name("Order Date").Index(1).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.IdOrderSource).Name("Original Order Number").Index(2);
            Map(m => m.Total).Name("Amount").Index(3).TypeConverterOption("c");
            Map(m => m.SkuCodesLine).Name("Issue Skus").Index(4);
            Map(m => m.OrderNotes).Name("Comments").Index(5);
        }
    }
}
