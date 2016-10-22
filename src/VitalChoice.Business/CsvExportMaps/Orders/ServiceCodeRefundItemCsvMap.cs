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
            Map(m => m.OrderSourceDateCreated).Name("Original Order Date").Index(3).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.AddedByAgentId).Name("Original Agent ID").Index(5);
            Map(m => m.Warehouse).Name("Warehouse").Index(6);
            Map(m => m.PWarehouse).Name("P Warehouse").Index(7);
            Map(m => m.NPWarehouse).Name("NP Warehouse").Index(8);
            Map(m => m.Carrier).Name("Carrier").Index(9);
            Map(m => m.PCarrier).Name("P Carrier").Index(10);
            Map(m => m.NPCarrier).Name("NP Carrier").Index(11);
            Map(m => m.Total).Name("Amount").Index(12).TypeConverterOption("c");
            Map(m => m.SkuCodesLine).Name("Issue Skus").Index(13);
            Map(m => m.OrderNotes).Name("Comments").Index(14);
        }
    }
}
