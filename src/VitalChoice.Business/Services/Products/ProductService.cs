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

namespace VitalChoice.Business.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly IEcommerceRepositoryAsync<VProductSku> _vProductSkuRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> _productOptionTypeRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionValue> _productOptionValueRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> _lookupRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly EcommerceContext _context;
        private readonly ILogger _logger;

        public ProductService(IEcommerceRepositoryAsync<VProductSku> vProductSkuRepository,
            IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Product> productRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository, EcommerceContext context,
            IEcommerceRepositoryAsync<ProductOptionValue> productOptionValueRepository)
        {
            this._vProductSkuRepository = vProductSkuRepository;
            this._productOptionTypeRepository = productOptionTypeRepository;
            this._lookupRepository = lookupRepository;
            this._productRepository = productRepository;
            this._skuRepository = skuRepository;
            _context = context;
            _productOptionValueRepository = productOptionValueRepository;
            _logger = LoggerService.GetDefault();
        }

        #region ProductOptions

        public async Task<ICollection<ProductOptionType>> GetProductOptionTypesAsync(ICollection<string> names)
        {
            ICollection<ProductOptionType> toReturn = (await _productOptionTypeRepository.Query(p => names.Contains(p.Name)).SelectAsync()).ToList();
            return toReturn;
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
                if (item.IdProductType != null && toReturn.ContainsKey((int)item.IdProductType))
                {
                    productTypeDefaultValues = toReturn[(int)item.IdProductType.Value];
                }
                else
                {
                    productTypeDefaultValues = new Dictionary<string, string>();
                    if (item.IdProductType != null)
                        toReturn.Add((int)item.IdProductType, productTypeDefaultValues);
                }
                if (!productTypeDefaultValues.ContainsKey(item.Name))
                {
                    productTypeDefaultValues.Add(item.Name, item.DefaultValue);
                }
            }

            return toReturn;
        }

        public async Task<ICollection<ProductOptionType>> GetProductLookupsAsync()
        {
            ICollection<ProductOptionType> toReturn = (await _productOptionTypeRepository.Query(p => p.IdLookup.HasValue).SelectAsync()).ToList();
            var lookups = (await _lookupRepository.Query(p => toReturn.Select(pp => pp.IdLookup).Contains(p.Id)).Include(p => p.LookupVariants).SelectAsync()).ToList();
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
            var conditions = new VProductSkuQuery().NotDeleted().WithText(filter.SearchText);
            var query = _vProductSkuRepository.Query(conditions);

            Func<IQueryable<VProductSku>, IOrderedQueryable<VProductSku>> sortable = x => x.OrderByDescending(y => y.Name);
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

            PagedList<VProductSku> toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

            return toReturn;
        }

        public async Task<ProductDynamic> GetProductAsync(int id, bool withDefaults = false)
        {
            IQueryFluent<Product> res = _productRepository.Query(
                p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .Include(p => p.ProductsToCategories)
                .Include(p => p.OptionTypes);
            var entity = (await res.SelectAsync(false)).FirstOrDefault();
           
            if (entity != null)
            {
                Dictionary<int, ProductOptionType> optionTypes = entity.OptionTypes.ToDictionary(o => o.Id, o => o);
                IncludeProductOptionTypes(entity, optionTypes);
                entity.Skus =
                    (await
                        _skuRepository.Query(p => p.IdProduct == entity.Id)
                            .Include(p => p.OptionValues)
                            .SelectAsync(false)).ToArray();
                IncludeSkuOptionTypes(entity, optionTypes);
                return new ProductDynamic(entity, withDefaults);
            }

            return null;
        }

        public async Task<ProductDynamic> UpdateProductAsync(ProductDynamic model)
        {
            using (var transaction = new TransactionManager(_context).BeginTransaction())
            {
                try
                {
                    if (model.Id == 0)
                    {
                        return new ProductDynamic(await InsertProduct(model));
                    }
                    return new ProductDynamic(await UpdateProduct(model));
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    _logger.LogError(e.Message, e);
                }
                transaction.Commit();
            }

            return null;
        }

        private async Task<Product> InsertProduct(ProductDynamic model)
        {
            var entity = model.ToEntity();
            if (entity != null)
            {
                entity.OptionTypes =
                    (await _productOptionTypeRepository.Query(o => o.IdProductType == model.Type).SelectAsync(false))
                        .ToArray();
                Dictionary<string, ProductOptionType> optionTypes = entity.OptionTypes.ToDictionary(o => o.Name, o => o);
                IncludeProductOptionTypesByName(entity, optionTypes);
                IncludeSkuOptionTypesByName(entity, optionTypes);
                return await _productRepository.InsertGraphAsync(entity);
            }
            return null;
        }

        private async Task<Product> UpdateProduct(ProductDynamic model)
        {
            var entity =
                            (await
                                _productRepository.Query(
                                    p => p.Id == model.Id && p.StatusCode != RecordStatusCode.Deleted)
                                    .Include(p => p.OptionValues)
                                    .Include(p => p.ProductsToCategories)
                                    .Include(p => p.OptionTypes)
                                    .SelectAsync()).FirstOrDefault();
            if (entity != null)
            {
                Dictionary<int, ProductOptionType> optionTypes = entity.OptionTypes.ToDictionary(o => o.Id, o => o);
                IncludeProductOptionTypes(entity, optionTypes);
                entity.Skus =
                    (await
                        _skuRepository.Query(p => p.IdProduct == entity.Id)
                            .Include(p => p.OptionValues)
                            .SelectAsync()).ToArray();
                IncludeSkuOptionTypes(entity, optionTypes);
                model.UpdateEntity(entity);
                await _productOptionValueRepository.DeleteAllAsync(entity.OptionValues);
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
                var skusCount = await _skuRepository.Query(p => p.IdProduct == id && p.StatusCode!=RecordStatusCode.Deleted).SelectCountAsync();
                if(skusCount>0)
                {
                    throw new AppValidationException("The given product contains sub products. Delete sub products first.");
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

        private static void IncludeProductOptionTypesByName(Product item, Dictionary<string, ProductOptionType> optionTypes)
        {
            foreach (var value in item.OptionValues)
            {
                ProductOptionType optionType;
                value.OptionType = optionTypes.TryGetValue(value.OptionType.Name, out optionType) ? optionType : null;
            }
        }

        private static void IncludeSkuOptionTypesByName(Product item, Dictionary<string, ProductOptionType> optionTypes)
        {
            foreach (var sku in item.Skus)
            {
                foreach (var value in sku.OptionValues)
                {
                    ProductOptionType optionType;
                    value.OptionType = optionTypes.TryGetValue(value.OptionType.Name, out optionType) ? optionType : null;
                }
            }
        }
    }
}