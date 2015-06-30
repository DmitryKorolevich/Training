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
        #region Products

        Task<List<ProductOptionType>> GetProductOptionTypesAsync(ICollection<string> names);

        Task<Dictionary<int, Dictionary<string, string>>> GetProductEditDefaultSettingsAsync();

        Task<List<ProductOptionType>> GetProductLookupsAsync();

        #endregion

        #region Skus

        Task<Sku> GetSku(string code);

        Task<Sku> GetSku(int id);

        Task<ICollection<VSku>> GetSkusAsync(VProductSkuFilter filter);

        #endregion

        #region Products

        Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter);

	    Task<ProductMapped> GetProductAsync(int id, bool withDefaults = false);

	    Task<ProductMapped> UpdateProductAsync(ProductMapped model);

        Task<bool> DeleteProductAsync(int id);

        #endregion
    }
}
