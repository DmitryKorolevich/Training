using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;

namespace VitalChoice.Interfaces.Services.InventorySkus
{
	public interface IInventorySkuService : IDynamicServiceAsync<InventorySkuDynamic, InventorySku>
    {
        Task<PagedList<InventorySkuListItemModel>> GetInventorySkusAsync(InventorySkuFilter filter);

        Task<Dictionary<int, List<SkuToInventorySku>>> GetAssignedInventorySkusAsync(ICollection<int> skuIds);
    }
}
