using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Product;
using VitalChoice.Data.Repositories.Customs;

namespace VitalChoice.Business.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly VProductSkuRepository _vProductSkuRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> _productOptionTypeRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionValue> _productOptionValueRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> _lookupRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IEcommerceRepositoryAsync<ProductToCategory> _productToCategoryRepository;
        private readonly EcommerceContext _context;
        private readonly ILogger _logger;

        public ProductService(VProductSkuRepository vProductSkuRepository,
            IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Product> productRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository, EcommerceContext context,
            IEcommerceRepositoryAsync<ProductOptionValue> productOptionValueRepository,
            IEcommerceRepositoryAsync<ProductToCategory> _productToCategoryRepository)
        {
            this._vProductSkuRepository = vProductSkuRepository;
            this._productOptionTypeRepository = productOptionTypeRepository;
            this._lookupRepository = lookupRepository;
            this._productRepository = productRepository;
            this._skuRepository = skuRepository;
            _context = context;
            _productOptionValueRepository = productOptionValueRepository;
            this._productToCategoryRepository = _productToCategoryRepository;
            _logger = LoggerService.GetDefault();
        }

        #region ProductOptions

        public async Task<List<ProductOptionType>> GetProductOptionTypesAsync(ICollection<string> names)
        {
            return await _productOptionTypeRepository.Query(p => names.Contains(p.Name)).SelectAsync();
        }

        public async Task<Dictionary<int, Dictionary<string, string>>> GetProductEditDefaultSettingsAsync()
        {
            Dictionary<int, Dictionary<string, string>> toReturn = new Dictionary<int, Dictionary<string, string>>();
            List<string> names = new List<string>();
            for (int i = 1; i <= ProductConstants.FIELD_COUNT_CROSS_SELL_PRODUCT; i++)
            {
                names.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE + i);
                names.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_URL + i);
            }
            for (int i = 1; i <= ProductConstants.FIELD_COUNT_YOUTUBE; i++)
            {
                names.Add(ProductConstants.FIELD_NAME_YOUTUBE_IMAGE + i);
                names.Add(ProductConstants.FIELD_NAME_YOUTUBE_TEXT + i);
                names.Add(ProductConstants.FIELD_NAME_YOUTUBE_VIDEO + i);
            }
            var items = await GetProductOptionTypesAsync(names);
            foreach (var item in items.Where(p => p.IdProductType.HasValue))
            {
                Dictionary<string, string> productTypeDefaultValues = null;
                if (item.IdProductType != null && toReturn.ContainsKey((int) item.IdProductType))
                {
                    productTypeDefaultValues = toReturn[(int) item.IdProductType.Value];
                }
                else
                {
                    productTypeDefaultValues = new Dictionary<string, string>();
                    if (item.IdProductType != null)
                        toReturn.Add((int) item.IdProductType, productTypeDefaultValues);
                }
                if (!productTypeDefaultValues.ContainsKey(item.Name))
                {
                    productTypeDefaultValues.Add(item.Name, item.DefaultValue);
                }
            }

            return toReturn;
        }

        public async Task<List<ProductOptionType>> GetProductLookupsAsync()
        {
            var toReturn = await _productOptionTypeRepository.Query(p => p.IdLookup.HasValue).SelectAsync();
            var lookups =
                await
                    _lookupRepository.Query(p => toReturn.Select(pp => pp.IdLookup).Contains(p.Id))
                        .Include(p => p.LookupVariants)
                        .SelectAsync();
            foreach (var lookup in lookups)
            {
                lookup.LookupVariants = lookup.LookupVariants.OrderBy(p => p.Id).ToList();
            }
            foreach (var item in toReturn)
            {
                foreach (var lookup in lookups)
                {
                    if (item.IdLookup == lookup.Id)
                    {
                        item.Lookup = lookup;
                    }
                }
            }

            return toReturn;
        }

        #endregion

        #region Products

        public async Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter)
        {
            await _vProductSkuRepository.GetProductsAsync(filter);

            var conditions = new VProductSkuQuery().NotDeleted().WithText(filter.SearchText);
            var query = _vProductSkuRepository.Query(conditions);

            Func<IQueryable<VProductSku>, IOrderedQueryable<VProductSku>> sortable =
                x => x.OrderByDescending(y => y.Name);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VProductSkuSortPath.Name:
                    sortable =
                        x =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Name)
                                : x.OrderByDescending(y => y.Name);
                    break;
            }

            PagedList<VProductSku> toReturn =
                await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

            return toReturn;
        }

        public async Task<ProductDynamic> GetProductAsync(int id, bool withDefaults = false)
        {
            IQueryFluent<Product> res = _productRepository.Query(
                p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .Include(p => p.ProductsToCategories);
            var entity = (await res.SelectAsync(false)).FirstOrDefault();

            if (entity != null)
            {
                entity.OptionTypes =
                    await
                        _productOptionTypeRepository.Query(o => o.IdProductType == entity.IdProductType)
                            .SelectAsync(false);
                Dictionary<int, ProductOptionType> optionTypes = entity.OptionTypes.ToDictionary(o => o.Id, o => o);
                IncludeProductOptionTypes(entity, optionTypes);
                entity.Skus =
                    await
                        _skuRepository.Query(p => p.IdProduct == entity.Id && p.StatusCode != RecordStatusCode.Deleted)
                            .Include(p => p.OptionValues)
                            .SelectAsync(false);
                IncludeSkuOptionTypes(entity, optionTypes);
                return new ProductDynamic(entity, withDefaults);
            }

            return null;
        }

        public async Task<ProductDynamic> UpdateProductAsync(ProductDynamic model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            using (var transaction = new TransactionManager(_context).BeginTransaction())
            {
                Product product = null;
                try
                {
                    if (model.Id == 0)
                    {
                        product = await InsertProduct(model);
                    }
                    else
                    {
                        product = await UpdateProduct(model);
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    transaction.Rollback();
                }
                return new ProductDynamic(product);
            }
        }

        private async Task<Product> InsertProduct(ProductDynamic model)
        {
            var entity = model.ToEntity();
            if (entity != null)
            {
                var optionTypes =
                    await _productOptionTypeRepository.Query(o => o.IdProductType == model.Type).SelectAsync(false);
                Dictionary<string, ProductOptionType> optionTypesSorted = optionTypes.ToDictionary(o => o.Name, o => o);
                IncludeProductOptionTypesByName(entity, optionTypesSorted);
                IncludeSkuOptionTypesByName(entity, optionTypesSorted);

                entity.OptionTypes = new List<ProductOptionType>();
                var skus = entity.Skus;
                entity.Skus = new List<Sku>();
                var result = await _productRepository.InsertGraphAsync(entity);
                foreach (var sku in skus)
                {
                    sku.IdProduct = result.Id;
                }
                await _skuRepository.InsertGraphRangeAsync(skus);
                result.Skus = skus;
                result.OptionTypes = optionTypes;
                return result;
            }
            return null;
        }

        private async Task<Product> UpdateProduct(ProductDynamic model)
        {
            var entity = (await _productRepository.Query(
                p => p.Id == model.Id && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .Include(p => p.ProductsToCategories)
                .SelectAsync()).FirstOrDefault();
            if (entity != null)
            {
                await _productOptionValueRepository.DeleteAllAsync(entity.OptionValues);
                await _productToCategoryRepository.DeleteAllAsync(entity.ProductsToCategories);

                entity.OptionTypes =
                    await _productOptionTypeRepository.Query(o => o.IdProductType == model.Type).SelectAsync(false);

                entity.Skus = await _skuRepository.Query(p => p.IdProduct == entity.Id && p.StatusCode!=RecordStatusCode.Deleted)
                    .Include(p => p.OptionValues)
                    .SelectAsync();

                foreach (var sku in entity.Skus)
                {
                    sku.OptionTypes = entity.OptionTypes;
                    await _productOptionValueRepository.DeleteAllAsync(sku.OptionValues);
                }

                model.UpdateEntity(entity);
                await _productOptionValueRepository.InsertRangeAsync(entity.OptionValues);
                return await _productRepository.UpdateAsync(entity);
            }
            return null;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await _productRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                var skusCount =
                    await
                        _skuRepository.Query(p => p.IdProduct == id && p.StatusCode != RecordStatusCode.Deleted)
                            .SelectCountAsync();
                if (skusCount > 0)
                {
                    throw new AppValidationException(
                        "The given product contains sub products. Delete sub products first.");
                }
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await _productRepository.UpdateAsync(dbItem);

                toReturn = true;
            }
            return toReturn;
        }

        #endregion

        private static void IncludeProductOptionTypes(Product item, Dictionary<int, ProductOptionType> optionTypes)
        {
            foreach (var value in item.OptionValues)
            {
                ProductOptionType optionType;
                value.OptionType = optionTypes.TryGetValue(value.IdOptionType, out optionType) ? optionType : null;
            }
        }

        private static void IncludeSkuOptionTypes(Product item, Dictionary<int, ProductOptionType> optionTypes)
        {
            foreach (var sku in item.Skus)
            {
                foreach (var value in sku.OptionValues)
                {
                    ProductOptionType optionType;
                    value.OptionType = optionTypes.TryGetValue(value.IdOptionType, out optionType) ? optionType : null;
                }
            }
        }

        private static void IncludeProductOptionTypesByName(Product item,
            Dictionary<string, ProductOptionType> optionTypes)
        {
            var forRemove = new List<ProductOptionValue>();
            foreach (var value in item.OptionValues)
            {
                ProductOptionType optionType;
                optionTypes.TryGetValue(value.OptionType.Name, out optionType);
                if (optionType == null)
                {
                    forRemove.Add(value);
                }
                else
                {
                    value.OptionType = null;
                    value.IdOptionType = optionType.Id;
                }
            }
            foreach (var forRemoveItem in forRemove)
            {
                item.OptionValues.Remove(forRemoveItem);
            }
        }

        private static void IncludeSkuOptionTypesByName(Product item, Dictionary<string, ProductOptionType> optionTypes)
        {
            foreach (var sku in item.Skus)
            {
                var forRemove = new List<ProductOptionValue>();
                foreach (var value in sku.OptionValues)
                {
                    ProductOptionType optionType;
                    if (!optionTypes.TryGetValue(value.OptionType.Name, out optionType))
                    {
                        forRemove.Add(value);
                    }
                    else
                    {
                        value.OptionType = null;
                        value.IdOptionType = optionType.Id;
                    }
                }
                foreach (var forRemoveItem in forRemove)
                {
                    sku.OptionValues.Remove(forRemoveItem);
                }
            }
        }
    }
}