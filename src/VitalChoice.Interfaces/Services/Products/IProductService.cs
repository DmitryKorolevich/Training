using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IProductService : IDynamicObjectServiceAsync<ProductDynamic, Product>
	{
        #region Products

        Task<List<ProductOptionType>> GetProductOptionTypesAsync(ICollection<string> names);

        Task<Dictionary<int, Dictionary<string, string>>> GetProductEditDefaultSettingsAsync();

        Task<List<ProductOptionType>> GetProductLookupsAsync();

	    Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter);

        #endregion

        #region Skus

        Task<Sku> GetSku(string code);

        Task<Sku> GetSku(int id);

        Task<ICollection<VSku>> GetSkusAsync(VProductSkuFilter filter);

        #endregion
    }
}
