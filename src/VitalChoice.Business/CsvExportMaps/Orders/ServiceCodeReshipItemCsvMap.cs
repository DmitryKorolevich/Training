using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class ServiceCodeReshipItemCsvMap : CsvClassMap<ServiceCodeReshipItem>
    {
        public ServiceCodeReshipItemCsvMap()
        {
            Map(m => m.Id).Name("Order Number").Index(0);
            Map(m => m.DateCreated).Name("Order Date").Index(1).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.IdOrderSource).Name("Original Order Number").Index(2);
            Map(m => m.ShipDate).Name("Ship Date").Index(3).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.Warehouse).Name("Warehouse").Index(5);
            Map(m => m.ShippingStateCode).Name("Dest. State").Index(6);
            Map(m => m.Carrier).Name("Carrier").Index(7);
            Map(m => m.ShipService).Name("Ship Service").Index(8);
            Map(m => m.Total).Name("Amount").Index(9).TypeConverterOption("c");
            Map(m => m.SkuCodesLine).Name("Issue Skus").Index(10);
            Map(m => m.OrderNotes).Name("Comments").Index(11);
        }
    }
}
