using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Products
{
    public class ProductService : IProductService
    {
        private readonly VProductSkuRepository _vProductSkuRepository;
        private readonly IEcommerceRepositoryAsync<VSku> _vSkuRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> _productOptionTypeRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> _lookupRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IEcommerceRepositoryAsync<BigStringValue> _bigStringValueRepository;
        private readonly ProductMapper _mapper;

        public ProductService(VProductSkuRepository vProductSkuRepository,
            IEcommerceRepositoryAsync<VSku> vSkuRepository,
            IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Product> productRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository, IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository, ProductMapper mapper)
        {
            this._vProductSkuRepository = vProductSkuRepository;
            this._vSkuRepository = vSkuRepository;
            this._productOptionTypeRepository = productOptionTypeRepository;
            this._lookupRepository = lookupRepository;
            this._productRepository = productRepository;
            this._skuRepository = skuRepository;
            _bigStringValueRepository = bigStringValueRepository;
            _mapper = mapper;
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
            var conditions = new VSkuQuery().NotDeleted().WithText(filter.SearchText).WithCode(filter.Code);
            var query = _vSkuRepository.Query(conditions);

            Func<IQueryable<VSku>, IOrderedQueryable<VSku>> sortable = x => x.OrderByDescending(y => y.DateCreated);

            if (filter.Paging == null)
            {
                return await query.OrderBy(sortable).SelectAsync(false);
            }
            var pagedList = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex,filter.Paging.PageItemCount);
            return pagedList.Items;
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
                await SetBigValuesAsync(entity, _bigStringValueRepository);
                entity.OptionTypes =
                    await
                        _productOptionTypeRepository.Query(o => o.IdProductType == entity.IdProductType)
                            .SelectAsync(false);

                entity.Skus =
                    await
                        _skuRepository.Query(p => p.IdProduct == entity.Id && p.StatusCode != RecordStatusCode.Deleted)
                            .Include(p => p.OptionValues)
                            .SelectAsync(false);

                return _mapper.FromEntity(entity, withDefaults);
            }

            return null;
        }

        private async Task SetBigValuesAsync(Product entity, IRepositoryAsync<BigStringValue> bigStringValueRepository, bool tracked = false)
        {
            var bigIdsList = entity.OptionValues.Where(v => v.IdBigString != null)
                .Select(v => v.IdBigString.Value)
                .ToList();
            var bigValues =
                (await bigStringValueRepository.Query(b => bigIdsList.Contains(b.IdBigString)).SelectAsync(tracked))
                    .ToDictionary
                    (b => b.IdBigString, b => b);
            foreach (var value in entity.OptionValues)
            {
                if (value.IdBigString != null)
                {
                    value.BigValue = bigValues[value.IdBigString.Value];
                }
            }
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
                    idProduct = (await InsertProductAsync(model, uow)).Id;
                }
                else
                {
                    product = await UpdateProductAsync(model, uow);
                }
                if (idProduct != 0)
                    return await GetProductAsync(idProduct);
                return _mapper.FromEntity(product);
            }
        }

        private async Task<List<MessageInfo>> ValidateProductAsync(ProductDynamic model, int? existingProductId = null,
            ICollection<int> existSkus = null)
        {
            List<MessageInfo> errors = new List<MessageInfo>();

            var productSameName =
                await
                    _productRepository.Query(
                        new ProductQuery().NotDeleted().Excluding(existingProductId).WithName(model.Name))
                        .SelectAsync(false);

            if (productSameName.Any())
            {
                errors.AddRange(
                    model.CreateError()
                        .Property(p => p.Name)
                        .Error("Product name should be unique in the database")
                        .Build());
            }

            var productSameUrl =
                await
                    _productRepository.Query(
                        new ProductQuery().NotDeleted().Excluding(existingProductId).WithUrl(model.Url))
                        .SelectAsync(false);


            if(productSameUrl.Any())
            {
                errors.AddRange(
                    model.CreateError()
                        .Property(p => p.Url)
                        .Error("Product url should be unique in the database")
                        .Build());
            }

            var newSet = model.Skus.Select(s => s.Code).ToArray();
            if (newSet.Any())
            {
                var skusSameCode = await
                    _skuRepository.Query(new SkuQuery().NotDeleted().Excluding(existSkus).Including(newSet))
                        .SelectAsync(false);
                if (skusSameCode.Any())
                {
                    errors.AddRange(model.CreateError()
                        .Collection(p => p.Skus)
                        .Property(s => s.Code, skusSameCode, item => item.Code)
                        .Error("This sku already exists in the database").Build());
                }
            }
            return errors;
        }

        private async Task<Product> InsertProductAsync(ProductDynamic model, EcommerceUnitOfWork uow)
        {
            (await ValidateProductAsync(model)).Raise();

            var optionTypes =
                    await _productOptionTypeRepository.Query(o => o.IdProductType == model.Type).SelectAsync(false);
            var entity = _mapper.ToEntity(model, optionTypes);
            if (entity != null)
            {
                entity.OptionTypes = new List<ProductOptionType>();
                var productRepository = uow.RepositoryAsync<Product>();

                await productRepository.InsertGraphAsync(entity);
                await uow.SaveChangesAsync(CancellationToken.None);

                entity.OptionTypes = optionTypes;
                return entity;
            }
            return null;
        }

        private async Task<Product> UpdateProductAsync(ProductDynamic model, EcommerceUnitOfWork uow)
        {
            var productRepository = uow.RepositoryAsync<Product>();
            var productOptionValueRepository = uow.RepositoryAsync<ProductOptionValue>();
            var skuRepository = uow.RepositoryAsync<Sku>();
            var productToCategoryRepository = uow.RepositoryAsync<ProductToCategory>();
            var bigValueRepository = uow.RepositoryAsync<BigStringValue>();

            var entity = (await productRepository.Query(
                p => p.Id == model.Id && p.StatusCode != RecordStatusCode.Deleted)
                .Include(p => p.OptionValues)
                .Include(p => p.ProductsToCategories)
                .SelectAsync()).FirstOrDefault();
            if (entity != null)
            {
                entity.Skus =
                    await skuRepository.Query(p => p.IdProduct == entity.Id && p.StatusCode != RecordStatusCode.Deleted)
                        .Include(p => p.OptionValues)
                        .SelectAsync();
                await SetBigValuesAsync(entity, bigValueRepository, true);
                var oldSet = entity.Skus.Select(s => s.Id).ToArray();
                (await ValidateProductAsync(model, model.Id, oldSet)).Raise();

                await
                    bigValueRepository.DeleteAllAsync(
                        entity.OptionValues.Where(o => o.BigValue != null).Select(o => o.BigValue).ToList());
                await productToCategoryRepository.DeleteAllAsync(entity.ProductsToCategories);
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
                _mapper.UpdateEntity(model, entity);
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
                await
                    bigValueRepository.InsertRangeAsync(
                        entity.OptionValues.Where(b => b.BigValue != null).Select(o => o.BigValue).ToList());
                await productToCategoryRepository.InsertRangeAsync(entity.ProductsToCategories);
                await productRepository.UpdateAsync(entity);
                await uow.SaveChangesAsync(CancellationToken.None);
                return entity;
            }
            return null;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {

            var skuExists =
                await
                    _skuRepository.Query(p => p.IdProduct == id && p.StatusCode != RecordStatusCode.Deleted)
                        .SelectAnyAsync();
            if (skuExists)
            {
                throw new AppValidationException(
                    "The given product contains sub products. Delete sub products first.");
            }
            var dbItem =
                (await
                    _productRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted)
                        .SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await _productRepository.UpdateAsync(dbItem);

                return true;
            }
            return false;
        }

        #endregion

        //private static void IncludeProductOptionTypes(Product item, Dictionary<int, ProductOptionType> optionTypes)
        //{
        //    foreach (var value in item.OptionValues)
        //    {
        //        ProductOptionType optionType;
        //        value.OptionType = optionTypes.TryGetValue(value.IdOptionType, out optionType) ? optionType : null;
        //    }
        //}

        //private static void IncludeSkuOptionTypes(Product item, Dictionary<int, ProductOptionType> optionTypes)
        //{
        //    foreach (var sku in item.Skus)
        //    {
        //        foreach (var value in sku.OptionValues)
        //        {
        //            ProductOptionType optionType;
        //            value.OptionType = optionTypes.TryGetValue(value.IdOptionType, out optionType) ? optionType : null;
        //        }
        //    }
        //}

        //private static void IncludeProductOptionTypesByName(Product item,
        //    Dictionary<string, ProductOptionType> optionTypes)
        //{
        //    var forRemove = new List<ProductOptionValue>();
        //    foreach (var value in item.OptionValues)
        //    {
        //        ProductOptionType optionType;
        //        optionTypes.TryGetValue(value.OptionType.Name, out optionType);
        //        if (optionType == null)
        //        {
        //            forRemove.Add(value);
        //        }
        //        else
        //        {
        //            value.OptionType = null;
        //            value.IdOptionType = optionType.Id;
        //        }
        //    }
        //    foreach (var forRemoveItem in forRemove)
        //    {
        //        item.OptionValues.Remove(forRemoveItem);
        //    }
        //}

        //private static void IncludeSkuOptionTypesByName(Product item, Dictionary<string, ProductOptionType> optionTypes)
        //{
        //    foreach (var sku in item.Skus)
        //    {
        //        var forRemove = new List<ProductOptionValue>();
        //        foreach (var value in sku.OptionValues)
        //        {
        //            ProductOptionType optionType;
        //            if (!optionTypes.TryGetValue(value.OptionType.Name, out optionType))
        //            {
        //                forRemove.Add(value);
        //            }
        //            else
        //            {
        //                value.OptionType = null;
        //                value.IdOptionType = optionType.Id;
        //            }
        //        }
        //        foreach (var forRemoveItem in forRemove)
        //        {
        //            sku.OptionValues.Remove(forRemoveItem);
        //        }
        //    }
        //}
    }
}