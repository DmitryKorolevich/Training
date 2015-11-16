using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IProductService : IDynamicServiceAsync<ProductDynamic, Product>
	{
        #region Products

        List<ProductOptionType> GetProductOptionTypes(HashSet<string> names);

        Dictionary<int, Dictionary<string, string>> GetProductEditDefaultSettingsAsync();

        List<ProductOptionType> GetProductLookupsAsync();

	    Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter);

        #endregion

        #region Skus

        Task<Dictionary<int, int>> GetTopPurchasedSkuIdsAsync(FilterBase filter);

        Task<SkuOrdered> GetSkuOrderedAsync(string code);

        Task<SkuOrdered> GetSkuOrderedAsync(int id);

        Task<List<SkuOrdered>> GetSkusOrderedAsync(ICollection<string> codes);

        Task<List<SkuOrdered>> GetSkusOrderedAsync(ICollection<int> ids);

        Task<SkuDynamic> GetSkuAsync(string code, bool withDefaults = false);

        Task<SkuDynamic> GetSkuAsync(int id, bool withDefaults = false);

        Task<ICollection<VSku>> GetSkusAsync(VProductSkuFilter filter);

	    Task<List<SkuDynamic>> GetSkusAsync(ICollection<SkuInfo> skuInfos, bool withDefaults = false);

        Task<List<SkuDynamic>> GetSkusAsync(ICollection<string> codes, bool withDefaults = false);

        #endregion

        #region ProductOutOfStockRequests

        Task<ICollection<ProductOutOfStockContainer>> GetProductOutOfStockContainers();

        Task<ProductOutOfStockRequest> AddProductOutOfStockRequest(ProductOutOfStockRequest model);

        Task<bool> SendProductOutOfStockRequests(ICollection<int> ids);

		Task<PagedList<VCustomerFavorite>> GetCustomerFavoritesAsync(VCustomerFavoritesFilter filter);

        #endregion

        #region ProductContent

        Task<ProductDynamic> InsertAsync(ProductContentTransferEntity model);

        Task<ProductDynamic> UpdateAsync(ProductContentTransferEntity model);

        Task<ProductContentTransferEntity> SelectTransferAsync(int id, bool withDefaults = false);

        #endregion
    }
}
