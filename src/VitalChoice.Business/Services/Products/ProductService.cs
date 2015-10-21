﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Business.Mail;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Mail;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Settings;

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
        private readonly SkuMapper _skuMapper;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly OrderSkusRepository _orderSkusRepositoryRepository;
        private readonly IEcommerceRepositoryAsync<ProductOutOfStockRequest> _productOutOfStockRequestRepository;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;

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
                    await skuRepository.Query(p => p.IdProduct == entity.Id && p.StatusCode != (int)RecordStatusCode.Deleted)
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
                if (sku.StatusCode == (int)RecordStatusCode.Deleted)
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

        protected override bool LogObjectFullData { get { return true; } }

        public ProductService(VProductSkuRepository vProductSkuRepository,
            IEcommerceRepositoryAsync<VSku> vSkuRepository,
            IEcommerceRepositoryAsync<ProductOptionType> productOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Product> productRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository, ProductMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<ProductToCategory> productToCategoriesRepository,
            IEcommerceRepositoryAsync<ProductOptionValue> productValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            OrderSkusRepository orderSkusRepositoryRepository,
            SkuMapper skuMapper,
            IEcommerceRepositoryAsync<ProductOutOfStockRequest> productOutOfStockRequestRepository,
            ISettingService settingService,
            INotificationService notificationService,
            ILoggerProviderExtended loggerProvider)
            : base(
                mapper, productRepository, productOptionTypeRepository, productValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider)
        {
            _vProductSkuRepository = vProductSkuRepository;
            _vSkuRepository = vSkuRepository;
            _productOptionTypeRepository = productOptionTypeRepository;
            _lookupRepository = lookupRepository;
            _productRepository = productRepository;
            _skuRepository = skuRepository;
            _productToCategoriesRepository = productToCategoriesRepository;
            _adminProfileRepository = adminProfileRepository;
            _orderSkusRepositoryRepository = orderSkusRepositoryRepository;
            _skuMapper = skuMapper;
            _productOutOfStockRequestRepository = productOutOfStockRequestRepository;
            _settingService = settingService;
            _notificationService = notificationService;
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
            names.Add(ProductConstants.FIELD_NAME_STOCK);
            names.Add(ProductConstants.FIELD_NAME_NON_DISCOUNTABLE);
            names.Add(ProductConstants.FIELD_NAME_HIDE_FROM_DATA_FEED);
            names.Add(ProductConstants.FIELD_NAME_QTY_THRESHOLD);
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

        public async Task<Dictionary<int, int>> GetTopPurchasedSkuIdsAsync(FilterBase filter)
        {
            return await _orderSkusRepositoryRepository.GetTopPurchasedSkuIdsAsync(filter);
        }

        public async Task<SkuOrdered> GetSkuOrderedAsync(string code)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            if (string.IsNullOrWhiteSpace(code))
                return null;

            var sku =
                await _skuRepository.Query(s => s.Code == code && s.StatusCode != (int)RecordStatusCode.Deleted)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.ProductsToCategories)
                    .SelectFirstOrDefaultAsync(false);
            sku.OptionTypes =
                await _productOptionTypeRepository.Query(GetOptionTypeQuery(sku.Product.IdObjectType)).SelectAsync(false);
            sku.Product.OptionTypes = sku.OptionTypes;
            return new SkuOrdered
            {
                Sku = await _skuMapper.FromEntityAsync(sku, true),
                ProductWithoutSkus = await Mapper.FromEntityAsync(sku.Product, true)
            };
        }

        public async Task<SkuOrdered> GetSkuOrderedAsync(int id)
        {
            var sku =
                await _skuRepository.Query(s => s.Id == id)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.ProductsToCategories)
                    .SelectFirstOrDefaultAsync(false);
            sku.OptionTypes =
                await _productOptionTypeRepository.Query(GetOptionTypeQuery(sku.Product.IdObjectType)).SelectAsync(false);
            sku.Product.OptionTypes = sku.OptionTypes;
            return new SkuOrdered
            {
                Sku = await _skuMapper.FromEntityAsync(sku, true),
                ProductWithoutSkus = await Mapper.FromEntityAsync(sku.Product, true)
            };
        }

        public async Task<List<SkuOrdered>> GetSkusOrderedAsync(ICollection<string> codes)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            if (!codes.Any())
                return new List<SkuOrdered>();

            var skus =
                await _skuRepository.Query(s => codes.Contains(s.Code) && s.StatusCode != (int)RecordStatusCode.Deleted)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.ProductsToCategories)
                    .SelectAsync(false);

            var optionTypes = await _productOptionTypeRepository.Query().SelectAsync(false);

            foreach (var sku in skus)
            {
                sku.OptionTypes =
                    optionTypes.Where(GetOptionTypeQuery(sku.Product.IdObjectType).Query().Compile()).ToList();
                sku.Product.OptionTypes = sku.OptionTypes;
            }

            return skus.Select(sku => new SkuOrdered
            {
                Sku = _skuMapper.FromEntity(sku, true),
                ProductWithoutSkus = Mapper.FromEntity(sku.Product, true)
            }).ToList();
        }

        public async Task<List<SkuOrdered>> GetSkusOrderedAsync(ICollection<int> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            if (!ids.Any())
                return new List<SkuOrdered>();

            var skus =
                await _skuRepository.Query(s => ids.Contains(s.Id) && s.StatusCode != (int)RecordStatusCode.Deleted)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.ProductsToCategories)
                    .SelectAsync(false);

            var optionTypes = await _productOptionTypeRepository.Query().SelectAsync(false);

            foreach (var sku in skus)
            {
                sku.OptionTypes =
                    optionTypes.Where(GetOptionTypeQuery(sku.Product.IdObjectType).Query().Compile()).ToList();
                sku.Product.OptionTypes = sku.OptionTypes;
            }

            return skus.Select(sku => new SkuOrdered
            {
                Sku = _skuMapper.FromEntity(sku, true),
                ProductWithoutSkus = Mapper.FromEntity(sku.Product, true)
            }).ToList();
        }

        public async Task<SkuDynamic> GetSkuAsync(string code, bool withDefaults = false)
        {
            if (code == null)
                throw new ArgumentNullException(nameof(code));

            if (string.IsNullOrWhiteSpace(code))
                return null;

            var sku =
                await _skuRepository.Query(s => s.Code == code && s.StatusCode != (int)RecordStatusCode.Deleted)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .SelectFirstOrDefaultAsync(false);
            sku.OptionTypes =
                await _productOptionTypeRepository.Query(GetOptionTypeQuery(sku.Product.IdObjectType)).SelectAsync(false);
            return await _skuMapper.FromEntityAsync(sku, withDefaults);
        }

        public async Task<SkuDynamic> GetSkuAsync(int id, bool withDefaults = false)
        {
            var sku =
                await _skuRepository.Query(s => s.Id == id)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .SelectFirstOrDefaultAsync(false);
            sku.OptionTypes =
                await
                    _productOptionTypeRepository.Query(GetOptionTypeQuery(sku.Product.IdObjectType)).SelectAsync(false);
            return await _skuMapper.FromEntityAsync(sku, withDefaults);
        }

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
            var pagedList = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return pagedList.Items;
        }

        public async Task<List<SkuDynamic>> GetSkusAsync(ICollection<SkuInfo> skuInfos, bool withDefaults = false)
        {
            if (skuInfos == null)
                throw new ArgumentNullException(nameof(skuInfos));

            if (!skuInfos.Any())
                return new List<SkuDynamic>();

            var optionTypes = await _productOptionTypeRepository.Query().SelectAsync(false);
            var skus =
                await _skuRepository.Query(new SkuQuery().ByIds(skuInfos.Select(i => i.Id).ToList()))
                    .Include(s => s.OptionValues)
                    .SelectAsync(false);
            var skusKeyed = skus.ToDictionary(s => s.Id, s => s);
            foreach (var info in skuInfos)
            {
                var sku = skusKeyed[info.Id];
                sku.OptionTypes =
                    optionTypes.Where(GetOptionTypeQuery((int?)info.IdProductType).Query().Compile()).ToList();
            }
            return await _skuMapper.FromEntityRangeAsync(skus, withDefaults);
        }

        public async Task<List<SkuDynamic>> GetSkusAsync(ICollection<string> codes, bool withDefaults = false)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            if (!codes.Any())
                return new List<SkuDynamic>();

            var optionTypes = await _productOptionTypeRepository.Query().SelectAsync(false);
            var skus =
                await _skuRepository.Query(new SkuQuery().Including(codes).NotDeleted())
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .SelectAsync(false);
            foreach (var sku in skus)
            {
                sku.OptionTypes =
                    optionTypes.Where(GetOptionTypeQuery(sku.Product.IdObjectType).Query().Compile()).ToList();
            }
            return await _skuMapper.FromEntityRangeAsync(skus, withDefaults);
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

        #region ProductOutOfStockRequests

        public async Task<ICollection<ProductOutOfStockContainer>> GetProductOutOfStockContainers()
        {
            var items = await _productOutOfStockRequestRepository.Query().SelectAsync(false);
            var productIds = items.Select(p => p.IdProduct).Distinct().ToArray();
            var products = await _productRepository.Query(p => productIds.Contains(p.Id)).SelectAsync(false);

            var containers = new List<ProductOutOfStockContainer>();
            foreach (var item in items)
            {
                ProductOutOfStockContainer container = containers.FirstOrDefault(p => p.IdProduct == item.IdProduct);
                if (container == null)
                {
                    container = new ProductOutOfStockContainer();
                    container.Requests = new List<ProductOutOfStockRequest>();
                    container.IdProduct = item.IdProduct;
                    var product = products.FirstOrDefault(p => p.Id == item.IdProduct);
                    if (product != null)
                    {
                        container.Name = product.Name;
                    }
                    containers.Add(container);
                }

                container.Requests.Add(item);
            }

            //Set InStock field
            var optionTypes = await _productOptionTypeRepository.Query().SelectAsync(false);
            var skus =
                await _skuRepository.Query(new SkuQuery().ByProductIds(products.Select(i => i.Id).ToList()).NotDeleted())
                    .Include(s => s.OptionValues)
                    .SelectAsync(false);
            foreach (var sku in skus)
            {
                var product = products.FirstOrDefault(p => p.Id == sku.IdProduct);
                if (product != null)
                {
                    sku.OptionTypes =
                        optionTypes.Where(GetOptionTypeQuery((int?)product.IdObjectType).Query().Compile()).ToList();
                }
            }
            var skuDynamics = await _skuMapper.FromEntityRangeAsync(skus, true);

            foreach (var skuDynamic in skuDynamics)
            {
                var container = containers.FirstOrDefault(p => p.IdProduct == skuDynamic.IdProduct);
                if (container != null)
                {
                    bool disregardStock = true;
                    if (skuDynamic.DictionaryData.ContainsKey("DisregardStock") && skuDynamic.DictionaryData["DisregardStock"] is bool)
                    {
                        disregardStock = (bool)skuDynamic.DictionaryData["DisregardStock"];
                    }
                    int stock = 0;
                    if (skuDynamic.DictionaryData.ContainsKey("Stock") && skuDynamic.DictionaryData["Stock"] is int)
                    {
                        stock = (int)skuDynamic.DictionaryData["Stock"];
                    }
                    container.InStock = container.InStock || disregardStock || stock > 0;
                }
            }

            return containers;
        }

        public async Task<ProductOutOfStockRequest> AddProductOutOfStockRequest(ProductOutOfStockRequest model)
        {
            model.DateCreated = DateTime.Now;
            await _productOutOfStockRequestRepository.InsertAsync(model);
            return model;
        }

        public async Task<bool> SendProductOutOfStockRequests(ICollection<int> ids)
        {
            if (ids.Count > 0)
            {
                var setting = (await _settingService.GetAppSettingItemsAsync(new List<string>() { SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE })).FirstOrDefault();
                if (setting == null)
                {
                    throw new NotSupportedException($"{SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE} not configurated.");
                }

                var items = await _productOutOfStockRequestRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false);
                var productIds = items.Select(p => p.IdProduct).Distinct().ToArray();
                var products = await _productRepository.Query(p => productIds.Contains(p.Id)).SelectAsync(false);

                foreach (var item in items)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.IdProduct);
                    if (product != null)
                    {
                        var text = setting.Value.Replace(SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_CUSTOMER_NAME_HOLDER, item.Name).
                            Replace(SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_PRODUCT_NAME_HOLDER, product.Name);
                        //TODO - add setting for a product url

                        BasicEmail email = new BasicEmail();
                        email.IsHTML = true;
                        email.Subject = "Vital Choice - Out of Stock Notification";
                        email.Body = text;
                        email.ToEmail = item.Email;
                        email.ToName = item.Name;
                        await _notificationService.SendBasicEmailAsync(email);
                    }
                }

                await _productOutOfStockRequestRepository.DeleteAllAsync(ids);
            }
            return true;
        }

        #endregion
    }
}