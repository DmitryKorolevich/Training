using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.CsvExportMaps.Orders
{
    public class VOrderWithRegionInfoItemCsvMap : CsvClassMap<VOrderWithRegionInfoItem>
    {
        public VOrderWithRegionInfoItemCsvMap()
        {
            Map(m => m.Id).Name("Order #").Index(0);
            Map(m => m.DateCreated).Name("Order Date").Index(1).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy hh:mm tt");
            Map(m => m.Total).Name("Order Amount").Index(2).TypeConverterOption("c");
            Map(m => m.Region).Name("Region").Index(3);
            Map(m => m.City).Name("City").Index(4);
            Map(m => m.Zip).Name("Zip").Index(5);
            Map(m => m.IdCustomer).Name("Customer #").Index(6);
            Map(m => m.CustomerLastName).Name("Customer Last Name").Index(7);
            Map(m => m.CustomerOrdersCount).Name("Repeat").Index(8);
        }
    }
}
