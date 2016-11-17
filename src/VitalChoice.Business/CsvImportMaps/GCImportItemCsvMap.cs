using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Business.CsvImportMaps
{
    public class GCImportItemCsvMap : CsvClassMap<GCImportItem>
    {
        public GCImportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Email).Name("Email");
            Map(m => m.FirstName).Name("First Name");
            Map(m => m.LastName).Name("Last Name");
            Map(m => m.Tag).Name("Tag");
        }
    }
}
