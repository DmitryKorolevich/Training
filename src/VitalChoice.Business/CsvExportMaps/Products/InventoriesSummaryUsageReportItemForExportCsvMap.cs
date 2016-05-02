using System.Dynamic;
using System.Globalization;
using CsvHelper.Configuration;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Business.CsvExportMaps.Products
{
    public class InventoriesSummaryUsageReportItemForExportCsvMap : CsvClassMap<ExpandoObject>
    {
        public InventoriesSummaryUsageReportItemForExportCsvMap()
        {
        }
    }
}
