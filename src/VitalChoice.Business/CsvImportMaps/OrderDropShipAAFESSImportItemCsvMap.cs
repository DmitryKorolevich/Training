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

        }
    }
}
