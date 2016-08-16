using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Entities.Orders;

namespace VitalChoice.Business.CsvImportMaps
{
    public class OrderDropShipAAFESSImportItemCsvMap : CsvClassMap<DropShipAAFESShipImportItem>
    {
        public OrderDropShipAAFESSImportItemCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.OrderNotes).Name("ORDER_NO");
            Map(m => m.PoNumber).Name("PO_NO");
            Map(m => m.Company).Name("SHIPTO_COMPANY");
            Map(m => m.Address1).Name("SHIPTO_REF1");
            Map(m => m.Address2).Name("SHIPTO_REF2");
            Map(m => m.City).Name("SHIPTO_CITY");
            Map(m => m.State).Name("SHIPTO_STATE");
            Map(m => m.Country).Name("SHIPTO_COUNTRY");
            Map(m => m.Zip).Name("SHIPTO_ZIP");
            Map(m => m.Phone).Name("SHIPTO_PHONE");
            Map(m => m.GiftMessage).Name("GREETING");
            Map(m => m.Sku).Name("VND_SKU");
            Map(m => m.QTY).Name("QTY");
        }
    }
}
