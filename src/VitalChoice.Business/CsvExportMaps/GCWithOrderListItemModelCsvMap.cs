using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;

namespace VitalChoice.Business.CsvExportMaps
{
    public class GCWithOrderListItemModelCsvMap : CsvClassMap<GCWithOrderListItemModel>
    {
        public GCWithOrderListItemModelCsvMap()
        {
            Map(m => m.Code).Name("Gift Certificate").Index(0);
            Map(m => m.Created).Name("Creation Date").Index(1).TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy hh:mm tt");
            Map(m => m.BillingLastName).Name("Billing Last Name").Index(2);
            Map(m => m.ShippingLastName).Name("Shipping Last Name").Index(3);
            Map(m => m.GCTypeName).Name("Type").Index(4);
            Map(m => m.StatusCodeName).Name("Status").Index(5);
            Map(m => m.Balance).Name("Current Balance").Index(6).TypeConverterOption("c");
        }
    }
}
