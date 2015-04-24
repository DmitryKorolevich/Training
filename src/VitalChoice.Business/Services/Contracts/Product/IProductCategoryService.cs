using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Domain.Transfer.Product;

namespace VitalChoice.Business.Services.Contracts.Product
{
	public interface IProductCategoryService
    {
        Task<ProductCategory> GetCategoriesTreeAsync(ProductCategoryTreeFilter filter);
        /// <summary>
        /// Allow only update ordering categories in a tree, without ability to add or delete items. Not active status is ignored for this method, but not active categories willn't 
        /// render on the public part.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<bool> UpdateCategoriesTreeAsync(ProductCategory category);
        Task<ProductCategoryContent> GetCategoryAsync(int id);
        Task<ProductCategoryContent> UpdateCategoryAsync(ProductCategoryContent category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
