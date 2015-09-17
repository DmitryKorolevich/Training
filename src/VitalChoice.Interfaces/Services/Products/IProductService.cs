using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
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

        Task<Dictionary<int, int>> GetTopPurchasedSkuIdsAsync(FilterBase filter);

        Task<Sku> GetSkuAsync(string code);

        Task<Sku> GetSkuAsync(int id);

        Task<ICollection<VSku>> GetSkusAsync(VProductSkuFilter filter);

	    List<SkuDynamic> GetSkus(ICollection<SkuInfo> skuInfos, bool withDefaults = false);

        List<SkuDynamic> GetSkus(ICollection<string> codes, bool withDefaults = false);

	    ProductDynamic GetProductWithoutSkus(int id, bool withDefaults = false);

	    #endregion
	}
}
