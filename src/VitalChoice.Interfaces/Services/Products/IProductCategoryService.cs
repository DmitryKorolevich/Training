using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Products
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
        Task<bool> UpdateCategoriesTreeAsync(ProductNavCategoryLite category);
        Task<ProductCategoryContent> GetCategoryAsync(int id);
        Task<ProductCategoryContent> GetCategoryByIdOldAsync(int id);
        Task<ProductCategoryContent> UpdateCategoryAsync(ProductCategoryContent category);
        Task<bool> DeleteCategoryAsync(int id);
        Task<ProductNavCategoryLite> GetLiteCategoriesTreeAsync(ProductCategoryLiteFilter liteFilter);

        Task<ProductNavCategoryLite> GetLiteCategoriesTreeAsync(ProductCategory productRootCategory,
            ProductCategoryLiteFilter liteFilter);

        Task<ProductCategoryStatisticTreeItemModel> GetProductCategoriesStatisticAsync(ProductCategoryStatisticFilter filter);

        Task<ICollection<SkusInProductCategoryStatisticItem>> GetSkusInProductCategoryStatisticAsync(ProductCategoryStatisticFilter filter);
    }
}
