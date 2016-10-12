using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;

namespace VitalChoice.Interfaces.Services.InventorySkus
{
	public interface IInventorySkuService : IDynamicServiceAsync<InventorySkuDynamic, InventorySku>
	{
	    Task ValidateInventorySkuAsync(InventorySkuDynamic model);

        Task<PagedList<InventorySkuListItemModel>> GetInventorySkusAsync(InventorySkuFilter filter);

        Task<Dictionary<int, List<SkuToInventorySku>>> GetAssignedInventorySkusAsync(IEnumerable<int> skuIds);

	    Task<ICollection<SkuInventoriesInfoItem>> GetSkuInventoriesInfoAsync(bool activeOnly, bool withInventories);

        Task<ICollection<InventorySkuUsageReportItem>> GetInventorySkuUsageReportAsync(InventorySkuUsageReportFilter filter);

        Task<ICollection<InventorySkuUsageReportItemForExport>> GetInventorySkuUsageReportForExportAsync(InventorySkuUsageReportFilter filter);

        Task<InventoriesSummaryUsageReport> GetInventoriesSummaryUsageReportAsync(InventoriesSummaryUsageReportFilter filter);

	    void ConvertInventoriesSummaryUsageReportForExport(InventoriesSummaryUsageReport report, int infoType,
            out IList<DynamicExportColumn> columns, out IList<ExpandoObject> items);

        Task<bool> ImportInventorySkusAsync(byte[] file, int userId);

        Task<bool> ImportSkuInventoryInfoAsync(byte[] file, int userId);
    }
}
