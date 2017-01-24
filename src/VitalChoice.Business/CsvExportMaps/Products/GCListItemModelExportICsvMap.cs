using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Business.CsvExportMaps.Products
{
    public class GCListItemModelExportICsvMap : CsvClassMap<GCListItemModel>
    {
        public GCListItemModelExportICsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.ProductName).Name("Product Name");
            Map(m => m.Code).Name("GC Code");
            Map(m => m.Created).Name("Created").TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy hh:mm tt");
            Map(m => m.AgentId).Name("Created By");
            Map(m => m.IdOrder).Name("Order #");
            Map(m => m.RecipientName).Name("Recipient Name");
            Map(m => m.Email).Name("Recipient Email");
            Map(m => m.Balance).Name("Available").TypeConverterOption("c");
            Map(m => m.StatusCode).Name("Status").TypeConverter<RecordStatusCodeConverter>();
            Map(m => m.ExpirationDate).Name("Expiration Date").TypeConverterOption(CultureInfo.InvariantCulture).TypeConverterOption("MM/dd/yyyy");
            Map(m => m.Tag).Name("Tag");
        }
    }
}
