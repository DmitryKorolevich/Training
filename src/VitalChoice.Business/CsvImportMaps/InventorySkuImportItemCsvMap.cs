using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Entities.InventorySkus;
using VitalChoice.Infrastructure.Domain.Entities.Orders;

namespace VitalChoice.Business.CsvImportMaps
{
    public class InventorySkuImportItemCsvMap : CsvClassMap<InventorySkuImportItem>
    {
        public InventorySkuImportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.Code).Name("Code");
            Map(m => m.Active).Name("Active");
            Map(m => m.Description).Name("Description");
            Map(m => m.Source).Name("Source");
            Map(m => m.InvQty).Name("Inv Qty");
            Map(m => m.InvUOM).Name("Inv UOM");
            Map(m => m.InvUnitAmt).Name("Inv Unit Amt");
            Map(m => m.PurchaseUOM).Name("Purchase UOM");
            Map(m => m.UOMQty).Name("UOM Qty");
            Map(m => m.PartsCategory).Name("Parts Category");
        }
    }
}
