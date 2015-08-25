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
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Products
{
    public class ProductService : EcommerceDynamicObjectService<ProductDynamic, Product, ProductOptionType, ProductOptionValue>, IProductService
    {
        private readonly VProductSkuRepository _vProductSkuRepository;
        private readonly IEcommerceRepositoryAsync<VSku> _vSkuRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> _productOptionTypeRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> _lookupRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IEcommerceRepositoryAsync<ProductToCategory> _productToCategoriesRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;

        protected override async Task AfterSelect(Product entity)
        {
            entity.Skus =
                await
                    _skuRepository.Query(new SkuQuery().NotDeleted().WithProductId(entity.Id))
                        .Include(p => p.OptionValues)
                        .OrderBy(skus => skus.OrderBy(s => s.Order))
                        .SelectAsync(false);
            entity.ProductsToCategories =
                await _productToCategoriesRepository.Query(c => c.IdProduct == entity.Id).SelectAsync(false);
        }

        protected async override Task BeforeEntityChangesAsync(ProductDynamic model, Product entity, IUnitOfWorkAsync uow)
        {
            var skuRepository = uow.RepositoryAsync<Sku>();
            var categoriesRepository = uow.RepositoryAsync<ProductToCategory>();
            var productOptionValueRepository = uow.RepositoryAsync<ProductOptionValue>();
            entity.Skus =
                    await skuRepository.Query(p => p.IdProduct == entity.Id && p.StatusCode != RecordStatusCode.Deleted)
                        .Include(p => p.OptionValues)
                        .SelectAsync();
            foreach (var sku in entity.Skus)
            {
                sku.OptionTypes = entity.OptionTypes;
                await productOptionValueRepository.DeleteAllAsync(sku.OptionValues);
            }
            entity.ProductsToCategories = categoriesRepository.Query(c => c.IdProduct == model.Id).Select();
            await categoriesRepository.DeleteAllAsync(entity.ProductsToCategories);
        }

        protected override async Task AfterEntityChangesAsync(ProductDynamic model, Product entity, IUnitOfWorkAsync uow)
        {
            var productOptionValueRepository = uow.RepositoryAsync<ProductOptionValue>();
            var categoriesRepository = uow.RepositoryAsync<ProductToCategory>();
            foreach (var sku in entity.Skus)
            {
                if (sku.StatusCode == RecordStatusCode.Deleted)
                {
                    foreach (var value in sku.OptionValues)
                    {
                        value.Id = 0;
                    }
                }
                await productOptionValueRepository.InsertRangeAsync(sku.OptionValues);
            }
            await categoriesRepository.InsertRangeAsync(entity.ProductsToCategories);
        }

        protected override async Task<List<MessageInfo>> Validate(ProductDynamic model)
        {
            List<MessageInfo> errors = new List<MessageInfo>();

            var productSameName =
                await
                    _productRepository.Query(
                        new ProductQuery().NotDeleted().Excluding(model.Id).WithName(model.Name))
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
                        new ProductQuery().NotDeleted().Excluding(model.Id).WithUrl(model.Url))
                        .SelectAsync(false);


            if (productSameUrl.Any())
            {
                errors.AddRange(
                    model.CreateError()
                        .Property(p => p.Url)
                        .Error("Product url should be unique in the database")
                        .Build());
            }

            var newSet = model.Skus.Select(s => s.Code).ToArray();
            List<int> existSkus = null;
            if (model.Id > 0)
            {
                existSkus =
                    (await _skuRepository.Query(new SkuQuery().NotDeleted().WithProductId(model.Id))
                        .SelectAsync(false))
                        .Select(s => s.Id)
                        .ToList();
            }
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

        protected override async Task<List<MessageInfo>> ValidateDelete(int id)
        {
            var skuExists =
                await
                    _skuRepository.Query(new SkuQuery().WithProductId(id).NotDeleted())
                        .SelectAnyAsync();
            if (skuExists)
            {
                return CreateError().Error("The given product contains sub products. Delete sub products first.").Build();
            }
            return new List<MessageInfo>();
        }

        public ProductService(VProductSkuRepository vProductSkuRepository,
            IEcommerceRepositoryAsync<VSku> vSkuRepository,
            IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Product> productRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository, ProductMapper mapper,
            IEcommerceRepositoryAsync<ProductToCategory> productToCategoriesRepository,
            IEcommerceRepositoryAsync<ProductOptionValue> productValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository)
            : base(
                mapper, productRepository, productOptionTypeRepository, productValueRepositoryAsync,
                bigStringValueRepository)
        {
            _vProductSkuRepository = vProductSkuRepository;
            _vSkuRepository = vSkuRepository;
            _productOptionTypeRepository = productOptionTypeRepository;
            _lookupRepository = lookupRepository;
            _productRepository = productRepository;
            _skuRepository = skuRepository;
            _productToCategoriesRepository = productToCategoriesRepository;
            _adminProfileRepository = adminProfileRepository;
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
            foreach (var item in items.Where(p => p.IdObjectType.HasValue))
            {
                Dictionary<string, string> productTypeDefaultValues = null;
                if (item.IdObjectType != null && toReturn.ContainsKey(item.IdObjectType.Value))
                {
                    productTypeDefaultValues = toReturn[item.IdObjectType.Value];
                }
                else
                {
                    productTypeDefaultValues = new Dictionary<string, string>();
                    if (item.IdObjectType != null)
                        toReturn.Add(item.IdObjectType.Value, productTypeDefaultValues);
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
            var conditions = new VSkuQuery().NotDeleted().WithText(filter.SearchText).WithCode(filter.Code).WithDescriptionName(filter.DescriptionName)
                .WithExactCode(filter.ExactCode).WithExactDescriptionName(filter.ExactDescriptionName).WithIds(filter.Ids).WithIdProducts(filter.IdProducts);
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
            var toReturn = await _vProductSkuRepository.GetProductsAsync(filter);

            if (toReturn.Items.Any())
            {
                var ids = toReturn.Items.Select(p => p.IdEditedBy).ToList();
                var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync();
                foreach (var item in toReturn.Items)
                {
                    foreach (var profile in profiles)
                    {
                        if (item.IdEditedBy == profile.Id)
                        {
                            item.EditedByAgentId = profile.AgentId;
                        }
                    }
                }
            }

            return toReturn;
        }

        #endregion
    }
}