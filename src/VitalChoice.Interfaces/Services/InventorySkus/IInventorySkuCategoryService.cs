using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;
using VitalChoice.Infrastructure.Domain.Transfer.InventorySkus;

namespace VitalChoice.Interfaces.Services.InventorySkus
{
	public interface IInventorySkuCategoryService
    {
        Task<IList<InventorySkuCategory>> GetCategoriesTreeAsync(InventorySkuCategoryTreeFilter filter);
        Task<bool> UpdateCategoriesTreeAsync(IList<InventorySkuCategory> categories);
        Task<InventorySkuCategory> GetCategoryAsync(int id);
        Task<InventorySkuCategory> UpdateCategoryAsync(InventorySkuCategory category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
