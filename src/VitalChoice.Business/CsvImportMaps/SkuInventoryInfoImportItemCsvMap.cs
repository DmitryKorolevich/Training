using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Entities.InventorySkus;
using VitalChoice.Infrastructure.Domain.Entities.Orders;

namespace VitalChoice.Business.CsvImportMaps
{
    public class SkuInventoryInfoImportItemCsvMap : CsvClassMap<SkuInventoryInfoImportItem>
    {
        public SkuInventoryInfoImportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.SKU).Name("SKU");
            Map(m => m.Channel).Name("Channel");
            Map(m => m.Assemble).Name("Assemble");
            Map(m => m.Parts).Name("Parts");
        }
    }
}
