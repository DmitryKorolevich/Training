using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IProductService
    {
        Task<List<ProductOptionType>> GetProductOptionTypesAsync(ICollection<string> names);

        Task<Dictionary<int, Dictionary<string, string>>> GetProductEditDefaultSettingsAsync();

        Task<List<ProductOptionType>> GetProductLookupsAsync();

        Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter);

	    Task<ProductDynamic> GetProductAsync(int id, bool withDefaults = false);

	    Task<ProductDynamic> UpdateProductAsync(ProductDynamic model);

        Task<bool> DeleteProductAsync(int id);
    }
}
