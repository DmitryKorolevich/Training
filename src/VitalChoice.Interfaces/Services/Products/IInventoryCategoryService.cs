using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IInventoryCategoryService
    {
        Task<IList<InventoryCategory>> GetCategoriesTreeAsync(InventoryCategoryTreeFilter filter);
        /// <summary>
        /// Allow only update ordering categories in a tree, without ability to add or delete items
        /// render on the public part.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<bool> UpdateCategoriesTreeAsync(IList<InventoryCategory> categories);
        Task<InventoryCategory> GetCategoryAsync(int id);
        Task<InventoryCategory> UpdateCategoryAsync(InventoryCategory category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
