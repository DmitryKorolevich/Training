﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Validation.Helpers;

namespace VitalChoice.Business.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly VProductSkuRepository _vProductSkuRepository;
        private readonly IEcommerceRepositoryAsync<VSku> _vSkuRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> _productOptionTypeRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionValue> _productOptionValueRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> _lookupRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IEcommerceRepositoryAsync<ProductToCategory> _productToCategoryRepository;
        private readonly EcommerceContext _context;
        private readonly ILogger _logger;

        public ProductService(VProductSkuRepository vProductSkuRepository,
            IEcommerceRepositoryAsync<VSku> _vSkuRepository,
            IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Product> productRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository, EcommerceContext context,
            IEcommerceRepositoryAsync<ProductOptionValue> productOptionValueRepository,
            IEcommerceRepositoryAsync<ProductToCategory> productToCategoryRepository, ILoggerProviderExtended loggerProvider)
        {
            this._vProductSkuRepository = vProductSkuRepository;
            this._vSkuRepository = _vSkuRepository;
            this._productOptionTypeRepository = productOptionTypeRepository;
            this._lookupRepository = lookupRepository;
            this._productRepository = productRepository;
            this._skuRepository = skuRepository;
            _context = context;
            _productOptionValueRepository = productOptionValueRepository;
            this._productToCategoryRepository = productToCategoryRepository;
            _logger = loggerProvider.CreateLoggerDefault();
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
            names.Add(ProductConstants.FIELD_NAME_DISREGARD_STOCK);
            names.Add(ProductConstants.FIELD_NAME_NON_DISCOUNTABLE);
            names.Add(ProductConstants.FIELD_NAME_HIDE_FROM_DATA_FEED);
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

        #region Skus

        public async Task<ICollection<VSku>> GetSkusAsync(VProductSkuFilter filter)
        {
            var conditions = new VSkuQuery().NotDeleted().WithText(filter.SearchText);
            var query = _vSkuRepository.Query(conditions);

            Func<IQueryable<VSku>, IOrderedQueryable<VSku>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;

            return await query.OrderBy(sortable).SelectAsync(false);
        }

        public async Task<Sku> GetSku(string code)
        {
            var query = _skuRepository.Query(p => p.Code == code && p.StatusCode != RecordStatusCode.Deleted);

            return (await query.SelectAsync(false)).FirstOrDefault();
        }

        public async Task<Sku> GetSku(int id)
        {
            var query = _skuRepository.Query(p=>p.Id==id && p.StatusCode!=RecordStatusCode.Deleted);

            return (await query.SelectAsync(false)).FirstOrDefault();
        }

        #endregion

        #region Products

        public async Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter)
        {
            return await _vProductSkuRepository.GetProductsAsync(filter);
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
                entity.Skus = entity.Skus.OrderBy(p => p.Order).ToList();
                IncludeSkuOptionTypes(entity, optionTypes);
                return new ProductDynamic(entity, withDefaults);
            }

            return null;
        }

        public async Task<ProductDynamic> UpdateProductAsync(ProductDynamic model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            using (var uow = new EcommerceUnitOfWork())
            {
                Product product = null;
                int idProduct = 0;
                if (model.Id == 0)
                {
                    idProduct = (await InsertProduct(model, uow)).Id;
                }
                product = await UpdateProduct(model, uow);
                if (idProduct != 0)
                    return await GetProductAsync(idProduct);
                return new ProductDynamic(product);
            }
        }

        private async Task<Product> InsertProduct(ProductDynamic model, EcommerceUnitOfWork uow)
        {
            var entity = model.ToEntity();
            if (entity != null)
            {
                var optionTypes =
                    await _productOptionTypeRepository.Query(o => o.IdProductType == model.Type).SelectAsync(false);
                Dictionary<string, ProductOptionType> optionTypesSorted = optionTypes.ToDictionary(o => o.Name, o => o);
                IncludeProductOptionTypesByName(entity, optionTypesSorted);
                IncludeSkuOptionTypesByName(entity, optionTypesSorted);
                if (entity.Skus != null)
                {
                    int order = 0;
                    foreach (var sku in entity.Skus)
                    {
                        sku.Order = order;
                        order++;
                    }
                }

                entity.OptionTypes = new List<ProductOptionType>();
                var productRepository = uow.RepositoryAsync<Product>();

                var result = await productRepository.InsertGraphAsync(entity);
                await uow.SaveChangesAsync(CancellationToken.None);

                result.OptionTypes = optionTypes;
                return result;
            }
            return null;
        }

        private async Task<Product> UpdateProduct(ProductDynamic model, EcommerceUnitOfWork uow)
        {
            var productRepository = uow.RepositoryAsync<Product>();
            var productOptionValueRepository = uow.RepositoryAsync<ProductOptionValue>();
            var skuRepository = uow.RepositoryAsync<Sku>();
            var productToCategoryRepository = uow.RepositoryAsync<ProductToCategory>();

            var entity = (await productRepository.Query(
                p => p.Id == model.Id && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .SelectAsync()).FirstOrDefault();
            if (entity != null)
            {
                entity.Skus =
                    await skuRepository.Query(p => p.IdProduct == entity.Id && p.StatusCode != RecordStatusCode.Deleted)
                        .Include(p => p.OptionValues)
                        .SelectAsync();

                var newSet = model.Skus.Select(s => s.Code).ToList();
                var oldSet = entity.Skus.Select(s => s.Id).ToList();
                SkuQuery checkQuery = new SkuQuery();
                var existItems = await
                    _skuRepository.Query(checkQuery.NotDeleted().Excluding(oldSet).Including(newSet))
                        .SelectAsync();
                if (existItems.Any())
                {
                    throw new ApiValidationException(CollectionFormProperty.GetFullName("SKUs", newSet.IndexOf(existItems.First().Code), "Name"), "This sku already exists in the database");
                }

                await productOptionValueRepository.DeleteAllAsync(entity.OptionValues);

                entity.OptionTypes =
                    await _productOptionTypeRepository.Query(o => o.IdProductType == model.Type).SelectAsync(false);

                var skuOptions = new ICollection<ProductOptionValue>[entity.Skus.Count];
                var optionIndex = 0;

                foreach (var sku in entity.Skus)
                {
                    skuOptions[optionIndex] = sku.OptionValues;
                    optionIndex++;
                    sku.OptionTypes = entity.OptionTypes;
                }

                model.UpdateEntity(entity);
                optionIndex = 0;
                foreach (var sku in entity.Skus)
                {
                    if (optionIndex >= skuOptions.Length)
                        break;
                    if (sku.StatusCode == RecordStatusCode.Deleted)
                    {
                        sku.OptionValues = skuOptions[optionIndex];
                    }
                    else
                    {
                        await productOptionValueRepository.DeleteAllAsync(skuOptions[optionIndex]);
                    }
                    optionIndex++;
                }

                var categories = entity.ProductsToCategories;
                entity.ProductsToCategories = null;

                var toReturn = await productRepository.UpdateAsync(entity);

                var dbCategories = await productToCategoryRepository.Query(c => c.IdProduct == entity.Id).SelectAsync();
                await productToCategoryRepository.DeleteAllAsync(dbCategories);

                await productToCategoryRepository.InsertRangeAsync(categories);
                await uow.SaveChangesAsync(CancellationToken.None);

                toReturn.ProductsToCategories = categories;
                return toReturn;
            }
            return null;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await _productRepository.Query(p => p.Id == id && p.StatusCode!=RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
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