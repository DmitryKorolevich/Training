using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IProductService : IDynamicServiceAsync<ProductDynamic, Product>
	{
        #region Products

        List<ProductOptionType> GetProductOptionTypes(HashSet<string> names);

        Dictionary<int, Dictionary<string, string>> GetProductEditDefaultSettingsAsync();

        List<ProductOptionType> GetProductLookupsAsync();

	    Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter);

	    Task<ICollection<ProductCategoryOrderModel>> GetProductsOnCategoryOrderAsync(int idCategory);

	    Task<bool> UpdateProductsOnCategoryOrderAsync(int idCategory, ICollection<ProductCategoryOrderModel> products);

	    Task<IDictionary<int, int>> GetProductIdsBySkuIds(ICollection<int> skuIds);

        #endregion

        #region Skus

        Task<ICollection<SkuOptionValue>> GetSkuOptionValues(ICollection<int> skuIds,ICollection<int> optionIds);

        Task<Dictionary<int, int>> GetTopPurchasedSkuIdsAsync(FilterBase filter, int idCustomer);

        Task<SkuOrdered> GetSkuOrderedAsync(string code);

        Task<SkuOrdered> GetSkuOrderedAsync(int id);

        Task<List<SkuOrdered>> GetSkusOrderedAsync(ICollection<string> codes);

        Task<List<SkuOrdered>> GetSkusOrderedAsync(ICollection<int> ids);

        Task<List<RefundSkuOrdered>> GetRefundSkusOrderedAsync(ICollection<int> ids);

        Task<SkuDynamic> GetSkuAsync(string code, bool withDefaults = false);

        Task<SkuDynamic> GetSkuAsync(int id, bool withDefaults = false);

        Task<ICollection<VSku>> GetSkusAsync(VProductSkuFilter filter);

	    Task<List<SkuDynamic>> GetSkusAsync(ICollection<SkuInfo> skuInfos, bool withDefaults = false);

        Task<List<SkuDynamic>> GetSkusAsync(ICollection<string> codes, bool withDefaults = false);

	    Task<byte[]> GenerateSkuGoogleItemsReportFile();

	    Task UpdateSkuGoogleItemsReportFile();

	    Task<byte[]> GetSkuGoogleItemsReportFile();

        #endregion

        #region ProductOutOfStockRequests

        Task<ICollection<ProductOutOfStockContainer>> GetProductOutOfStockContainersAsync();

        Task<ProductOutOfStockRequest> AddProductOutOfStockRequestAsync(ProductOutOfStockRequest model);

        Task<bool> SendProductOutOfStockRequestsAsync(ICollection<int> ids, string messageFormat = null);

        Task<bool> DeleteProductOutOfStockRequestsAsync(ICollection<int> ids);

        Task<PagedList<VCustomerFavoriteFull>> GetCustomerFavoritesAsync(VCustomerFavoritesFilter filter);

        #endregion

        #region ProductContent

        Task<ProductDynamic> InsertAsync(ProductContentTransferEntity model);

        Task<ProductDynamic> UpdateAsync(ProductContentTransferEntity model);

	    Task<ProductContentTransferEntity> SelectTransferByIdOldAsync(int id);

        Task<ProductContentTransferEntity> SelectTransferAsync(int id, bool withDefaults = false);

	    Task<ICollection<ProductContentTransferEntity>> SelectTransferAsync(bool withDefaults = false);

        Task<ProductContentTransferEntity> SelectTransferAsync(Guid id, bool withDefaults = false);

		Task<ProductContentTransferEntity> SelectTransferAsync(string url, bool withDefaults = false);

        Task<ICollection<ProductContent>> SelectProductContents(ICollection<int> ids);

        #endregion

        Task<int> GetProductInternalIdAsync(Guid productId);
	}
}
