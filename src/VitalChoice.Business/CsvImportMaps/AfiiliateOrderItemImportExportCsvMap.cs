using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;

namespace VitalChoice.Business.CsvImportMaps
{
    public class AfiiliateOrderItemImportExportCsvMap : CsvClassMap<AfiiliateOrderItemImportExportModel>
    {
        public AfiiliateOrderItemImportExportCsvMap()
        {
            MapValues();
        }

        private void MapValues()
        {
            Map(m => m.IdOrder).Name("Order ID");
            Map(m => m.StatusMessage).Name("Order Status");
        }
    }
}
