using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.CsvExportMaps.Products;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Mailings;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Queries.Products;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.Data.UOW;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Entities.Reports;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.Business.Services.Products
{
    public class ProductService : ExtendedEcommerceDynamicService<ProductDynamic, Product, ProductOptionType, ProductOptionValue>,
        IProductService
    {
        private readonly VProductSkuRepository _vProductSkuRepository;
        //private readonly IEcommerceRepositoryAsync<VSku> _vSkuRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly IEcommerceRepositoryAsync<ProductToCategory> _productToCategoryRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IEcommerceRepositoryAsync<VCustomerFavorite> _vCustomerFavoriteRepository;
        private readonly SkuMapper _skuMapper;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly OrderSkusRepository _orderSkusRepositoryRepository;
        private readonly IEcommerceRepositoryAsync<ProductOutOfStockRequest> _productOutOfStockRequestRepository;
        private readonly INotificationService _notificationService;
        private readonly IRepositoryAsync<ProductContent> _productContentRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> _contentTypeRepository;
        private readonly IOptions<AppOptions> _options;
        private readonly IEcommerceRepositoryAsync<SkuOptionValue> _skuOptionValueRepositoryAsync;
        private readonly ReferenceData _referenceData;
        private readonly AppSettings _appSettings;
        private readonly IProductCategoryService _productCategoryService;
        private readonly ICsvExportService<SkuGoogleItem, SkuGoogleItemExportCsvMap> _skuGoogleItemCSVExportService;
        private readonly IBlobStorageClient _storageClient;
        private readonly SpEcommerceRepository _sPEcommerceRepository;
        private readonly IExtendedDynamicReadServiceAsync<SkuDynamic, Sku> _skuReadServiceAsync;
        private readonly IEcommerceRepositoryAsync<SkuOOSHistoryItem> _skuOOSHistoryItemRepository;
        private readonly IEcommerceRepositoryAsync<VShortProduct> _vShortProductRepository;

        public ProductService(VProductSkuRepository vProductSkuRepository,
            IEcommerceRepositoryAsync<Product> productRepository,
            IEcommerceRepositoryAsync<ProductToCategory> productToCategoryRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository, ProductMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<ProductOptionValue> productValueRepositoryAsync,
            IEcommerceRepositoryAsync<SkuOptionValue> skuOptionValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            OrderSkusRepository orderSkusRepositoryRepository,
            SkuMapper skuMapper,
            IEcommerceRepositoryAsync<ProductOutOfStockRequest> productOutOfStockRequestRepository,
            INotificationService notificationService,
            IRepositoryAsync<ProductContent> productContentRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            ICsvExportService<SkuGoogleItem, SkuGoogleItemExportCsvMap> skuGoogleItemCSVExportService,
            IBlobStorageClient storageClient,
            IProductCategoryService productCategoryService,
            IOptions<AppOptions> options,
            ILoggerFactory loggerProvider,
            IEcommerceRepositoryAsync<VCustomerFavorite> vCustomerRepositoryAsync,
            SpEcommerceRepository sPEcommerceRepository, DynamicExtensionsRewriter queryVisitor,
            ITransactionAccessor<EcommerceContext> transactionAccessor,
            IExtendedDynamicReadServiceAsync<SkuDynamic, Sku> skuReadServiceAsync,
            IDynamicEntityOrderingExtension<Product> orderingExtension,
            ReferenceData referenceData,
            AppSettings appSettings,
            IEcommerceRepositoryAsync<SkuOOSHistoryItem> skuOOSHistoryItemRepository,
            IEcommerceRepositoryAsync<VShortProduct> vShortProductRepository)
            : base(
                mapper, productRepository, productValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, queryVisitor, transactionAccessor, orderingExtension
                )
        {
            _vProductSkuRepository = vProductSkuRepository;
            _productRepository = productRepository;
            _productToCategoryRepository = productToCategoryRepository;
            _skuRepository = skuRepository;
            _adminProfileRepository = adminProfileRepository;
            _orderSkusRepositoryRepository = orderSkusRepositoryRepository;
            _skuMapper = skuMapper;
            _productOutOfStockRequestRepository = productOutOfStockRequestRepository;
            _notificationService = notificationService;
            _productContentRepository = productContentRepository;
            _contentTypeRepository = contentTypeRepository;
            _skuGoogleItemCSVExportService = skuGoogleItemCSVExportService;
            _productCategoryService = productCategoryService;
            _storageClient = storageClient;
            _vCustomerFavoriteRepository = vCustomerRepositoryAsync;
            _options = options;
            _skuOptionValueRepositoryAsync = skuOptionValueRepositoryAsync;
            _sPEcommerceRepository = sPEcommerceRepository;
            _skuReadServiceAsync = skuReadServiceAsync;
            _referenceData = referenceData;
            _appSettings = appSettings;
            _skuOOSHistoryItemRepository = skuOOSHistoryItemRepository;
            _vShortProductRepository = vShortProductRepository;
        }

        public async Task<ProductContent> SelectContentForTransfer(int id)
        {
            return (await _productContentRepository.Query(p => p.Id == id).Include(p => p.ContentItem).SelectFirstOrDefaultAsync(false));
        }

        public async Task<ICollection<ProductContent>> SelectContentsForTransfer()
        {
            return await _productContentRepository.Query(p => p.StatusCode != RecordStatusCode.Deleted).Include(p => p.ContentItem).SelectAsync(false);
        }

        public async Task<ICollection<ProductContent>> SelectProductContents(ICollection<int> ids)
        {
            return (await _productContentRepository.Query(p => ids.Contains(p.Id)).Include(p => p.ContentItem).SelectAsync(false)).ToList();
        }

        protected override IQueryLite<Product> BuildIncludes(IQueryLite<Product> query)
        {
            return query.Include(p => p.Skus)
                .ThenInclude(s => s.OptionValues)
                .Include(p => p.Skus)
                .ThenInclude(s => s.SkusToInventorySkus)
                .Include(p => p.ProductsToCategories);
        }

        protected override Task AfterSelect(ICollection<Product> entities)
        {
            foreach (var entity in entities)
            {
                entity.Skus = entity.Skus?.Where(s => s.StatusCode != (int)RecordStatusCode.Deleted).OrderBy(s => s.Order).ToArray();
            }
            return TaskCache.CompletedTask;
        }

        protected override async Task AfterEntityChangesAsync(ProductDynamic model, Product updated, IUnitOfWorkAsync uow)
        {
            await SyncDbCollections<Sku, SkuOptionValue>(uow, updated.Skus, false);

            //save oos history
            if (model.Skus?.Count > 0)
            {
                using (var content = CreateUnitOfWork())
                {
                    var skuOOSHistoryRepository = content.RepositoryAsync<SkuOOSHistoryItem>();

                    var skuIds = model.Skus.Select(pp => pp.Id).ToList();
                    var skuOOSActiveItems = await skuOOSHistoryRepository.Query(p => skuIds.Contains(p.IdSku) 
                        && p.IsCurrent).SelectAsync(true);
                    SkuOOSHistoryItem activeItem = null;
                    var now = DateTime.Now;
                    var newHistoryItems = new List<SkuOOSHistoryItem>();
                    foreach (var sku in model.Skus)
                    {
                        var inStock = sku.InStock();

                        if (inStock)
                        {
                            var activeItems = skuOOSActiveItems.Where(p => p.IdSku == sku.Id).ToList();
                            if (activeItems.Count>0)
                            {
                                foreach (var skuOosHistoryItem in activeItems)
                                {
                                    skuOosHistoryItem.IsCurrent = false;
                                    skuOosHistoryItem.EndDate = now;
                                }
                            }
                        }
                        else
                        {
                            activeItem = skuOOSActiveItems.FirstOrDefault(p => p.IdSku == sku.Id);
                            if (activeItem == null)
                            {
                                newHistoryItems.Add(new SkuOOSHistoryItem()
                                {
                                    IdSku = sku.Id,
                                    StartDate = now,
                                    IsCurrent = true
                                });
                            }
                        }
                    }

                    if (newHistoryItems.Count > 0)
                    {
                        await skuOOSHistoryRepository.InsertRangeAsync(newHistoryItems);
                    }

                    await skuOOSHistoryRepository.SaveChangesAsync();
                }
            }
        }

        protected override async Task<List<MessageInfo>> ValidateAsync(ProductDynamic model)
        {
            List<MessageInfo> errors = new List<MessageInfo>();

            //var productSameName =
            //    await
            //        _productRepository.Query(
            //            new ProductQuery().NotDeleted().Excluding(model.Id).WithName(model.Name))
            //            .SelectFirstOrDefaultAsync(false);

            //if (productSameName != null)
            //{
            //    errors.AddRange(
            //        model.CreateError()
            //            .Property(p => p.Name)
            //            .Error("Product name should be unique in the database")
            //            .Build());
            //}

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
            if (newSet.Length > 0)
            {
                var skusSameCode = await
                    _skuRepository.Query(new SkuQuery().NotDeleted().Excluding(existSkus).Including(newSet))
                        .SelectAsync(false);
                if (skusSameCode.Count > 0)
                {
                    errors.AddRange(model.CreateError()
                        .Collection(p => p.Skus)
                        .Property(s => s.Code, skusSameCode, item => item.Code)
                        .Error("This sku already exists in the database").Build());
                }
            }
            return errors;
        }

        protected override async Task<List<MessageInfo>> ValidateDeleteAsync(int id)
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

        #region ProductOptions

        public IEnumerable<OptionType> GetProductOptionTypes(HashSet<string> names)
        {
            return DynamicMapper.OptionTypes.Where(o => names.Contains(o.Name));
        }

        public IEnumerable<OptionType> GetSkuOptionTypes(HashSet<string> names)
        {
            return _skuMapper.OptionTypes.Where(o => names.Contains(o.Name));
        }

        public Dictionary<int, Dictionary<string, string>> GetProductEditDefaultSettingsAsync()
        {
            Dictionary<int, Dictionary<string, string>> toReturn = new Dictionary<int, Dictionary<string, string>>();
            //HashSet<string> names = new HashSet<string>();
            //for (int i = 1; i <= ProductConstants.FIELD_COUNT_CROSS_SELL_PRODUCT; i++)
            //{
            //    names.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_IMAGE + i);
            //    names.Add(ProductConstants.FIELD_NAME_CROSS_SELL_PRODUCT_URL + i);
            //}
            //for (int i = 1; i <= ProductConstants.FIELD_COUNT_YOUTUBE; i++)
            //{
            //    names.Add(ProductConstants.FIELD_NAME_YOUTUBE_IMAGE + i);
            //    names.Add(ProductConstants.FIELD_NAME_YOUTUBE_TEXT + i);
            //    names.Add(ProductConstants.FIELD_NAME_YOUTUBE_VIDEO + i);
            //}
            //names.Add(ProductConstants.FIELD_NAME_DISREGARD_STOCK);
            //names.Add(ProductConstants.FIELD_NAME_STOCK);
            //names.Add(ProductConstants.FIELD_NAME_NON_DISCOUNTABLE);
            //names.Add(ProductConstants.FIELD_NAME_HIDE_FROM_DATA_FEED);
            //names.Add(ProductConstants.FIELD_NAME_QTY_THRESHOLD);
            //var items = GetProductOptionTypes(names);
            foreach (
                var item in
                    ((IEnumerable<OptionType>)DynamicMapper.OptionTypes).Union(_skuMapper.OptionTypes)
                        .Where(t => t.DefaultValue != null))
            {
                if (item.IdObjectType != null)
                {
                    Dictionary<string, string> productTypeDefaultValues;
                    if (toReturn.ContainsKey(item.IdObjectType.Value))
                    {
                        productTypeDefaultValues = toReturn[item.IdObjectType.Value];
                    }
                    else
                    {
                        productTypeDefaultValues = new Dictionary<string, string>();
                        toReturn.Add(item.IdObjectType.Value, productTypeDefaultValues);
                    }
                    if (!productTypeDefaultValues.ContainsKey(item.Name))
                    {
                        productTypeDefaultValues.Add(item.Name, item.DefaultValue);
                    }
                }
                else
                {
                    foreach (int productType in Enum.GetValues(typeof(ProductType)))
                    {
                        Dictionary<string, string> productTypeDefaultValues;
                        if (toReturn.ContainsKey(productType))
                        {
                            productTypeDefaultValues = toReturn[productType];
                        }
                        else
                        {
                            productTypeDefaultValues = new Dictionary<string, string>();
                            toReturn.Add(productType, productTypeDefaultValues);
                        }
                        if (!productTypeDefaultValues.ContainsKey(item.Name))
                        {
                            productTypeDefaultValues.Add(item.Name, item.DefaultValue);
                        }
                    }
                }
            }

            return toReturn;
        }

        public IEnumerable<OptionType> GetExpandedOptionTypesWithSkuTypes()
        {
            foreach (var type in ((IEnumerable<OptionType>)DynamicMapper.OptionTypes).Union(_skuMapper.OptionTypes))
            {
                if (type.IdObjectType == null)
                {
                    foreach (int productType in Enum.GetValues(typeof(ProductType)))
                    {
                        yield return new ProductOptionType
                        {
                            Id = type.Id,
                            Name = type.Name,
                            IdObjectType = productType,
                            DefaultValue = type.DefaultValue,
                            IdFieldType = type.IdFieldType,
                            IdLookup = type.IdLookup,
                            Lookup = type.Lookup
                        };
                    }
                }
                else
                {
                    yield return type;
                }
            }
        }

        public async Task<ICollection<SkuOptionValue>> GetSkuOptionValues(ICollection<int> skuIds,
            ICollection<int> optionIds)
        {
            var toReturn = await _skuOptionValueRepositoryAsync.Query(p => skuIds.Contains(p.IdSku) && optionIds.Contains(p.IdOptionType))
                .SelectAsync(false);
            return toReturn;
        }

        #endregion

        #region Skus

        public async Task<Dictionary<int, int>> GetTopPurchasedSkuIdsAsync(FilterBase filter, int idCustomer)
        {
            return await _orderSkusRepositoryRepository.GetTopPurchasedSkuIdsAsync(filter, idCustomer);
        }

        private async Task<SkuOrdered> PopulateSkuOrderedWithUrlAsync(Sku sku)
        {
            var skuDynamic = await _skuMapper.FromEntityAsync(sku, true);

            var product = await _productContentRepository.Query(p => p.Id == sku.IdProduct).SelectFirstOrDefaultAsync(false);
            skuDynamic.Product.Url = product.Url;
            return new SkuOrdered
            {
                Sku = skuDynamic
            };
        }

        private async Task<IEnumerable<SkuOrdered>> PopulateSkusOrderedWithUrlAsync(ICollection<Sku> skus)
        {
            var productIds = skus.Select(s => s.IdProduct).Distinct().ToArray();
            var skusDynamic = await _skuMapper.FromEntityRangeAsync(skus, true);

            var products = await _productContentRepository.Query(p => productIds.Contains(p.Id)).SelectAsync(false);
            foreach (var product in products)
            {
                skusDynamic.Where(s => s.Product.Id == product.Id).ForEach(s => s.Product.Url = product.Url);
            }
            return skusDynamic.Select(s => new SkuOrdered
            {
                Sku = s
            });
        }

        private async Task<RefundSkuOrdered> PopulateRefundSkuOrderedAsync(Sku sku)
        {
            var skuDynamic = await _skuMapper.FromEntityAsync(sku, true);
            var skuOrdered = new RefundSkuOrdered
            {
                Sku = skuDynamic
            };

            return skuOrdered;
        }


        public async Task<SkuOrdered> GetSkuOrderedAsync(string code)
        {
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
            if (sku == null)
                return null;

            return await PopulateSkuOrderedWithUrlAsync(sku);
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

            return await PopulateSkuOrderedWithUrlAsync(sku);
        }

        public async Task<List<SkuOrdered>> GetSkusOrderedAsync(ICollection<string> codes)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            if (codes.Count == 0)
                return new List<SkuOrdered>();

            var skus =
                await _skuRepository.Query(s => codes.Contains(s.Code) && s.StatusCode != (int)RecordStatusCode.Deleted)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.ProductsToCategories)
                    .SelectAsync(false);

            return (await PopulateSkusOrderedWithUrlAsync(skus)).ToList();
        }

        public async Task<List<SkuOrdered>> GetSkusOrderedAsync(ICollection<int> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            if (ids.Count == 0)
                return new List<SkuOrdered>();

            var skus =
                await _skuRepository.Query(s => ids.Contains(s.Id) && s.StatusCode != (int)RecordStatusCode.Deleted)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.ProductsToCategories)
                    .SelectAsync(false);

            return (await PopulateSkusOrderedWithUrlAsync(skus)).ToList();
        }

        public async Task<List<RefundSkuOrdered>> GetRefundSkusOrderedAsync(ICollection<int> ids)
        {
            if (ids == null)
                throw new ArgumentNullException(nameof(ids));

            if (ids.Count == 0)
                return new List<RefundSkuOrdered>();

            var skus =
                await _skuRepository.Query(s => ids.Contains(s.Id) && s.StatusCode != (int)RecordStatusCode.Deleted)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(s => s.Product)
                    .ThenInclude(p => p.ProductsToCategories)
                    .SelectAsync(false);

            var res = new List<RefundSkuOrdered>();
            foreach (var temp in skus)
            {
                var skuOrdered = await PopulateRefundSkuOrderedAsync(temp);
                res.Add(skuOrdered);
            }

            return res;
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
            return await _skuMapper.FromEntityAsync(sku, withDefaults);
        }

        public async Task<PagedList<SkuPricesManageItemModel>> GetSkusPricesAsync(FilterBase filter)
        {
            SkuQuery conditions = new SkuQuery().NotDeleted().ProductNotDeleted().WithText(filter.SearchText);
            var data = await _skuRepository.Query(conditions).Include(p => p.Product)
                    .SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, false);
            var toReturn = new PagedList<SkuPricesManageItemModel>();
            toReturn.Count = data.Count;
            toReturn.Items = data.Items.Select(p => new SkuPricesManageItemModel()
            {
                Id = p.Id,
                IdProduct = p.IdProduct,
                ProductName = p.Product.Name,
                Code = p.Code,
                StatusCode = (RecordStatusCode)p.StatusCode,
                Hidden = p.Hidden,
                Price = p.Price,
                WholesalePrice = p.WholesalePrice,
            }).ToList();

            return toReturn;
        }

        public async Task<bool> UpdateSkusPricesAsync(ICollection<SkuPricesManageItemModel> items)
        {
            var productIds = items.Select(p => p.IdProduct).ToList();
            var products = await SelectTransfersAsync(productIds);
            foreach (var skuPricesManageItemModel in items)
            {
                var product = products.FirstOrDefault(p => p.ProductDynamic.Id == skuPricesManageItemModel.IdProduct);
                if (product != null)
                {
                    var sku = product.ProductDynamic.Skus.FirstOrDefault(p => p.Id == skuPricesManageItemModel.Id);
                    if (sku != null)
                    {
                        sku.Price = skuPricesManageItemModel.Price;
                        sku.WholesalePrice = skuPricesManageItemModel.WholesalePrice;
                    }
                }
            }

            await UpdateRangeAsync(products);
            return true;
        }

        public async Task<SkuDynamic> GetSkuAsync(int id, bool withDefaults = false)
        {
            var sku =
                await _skuRepository.Query(s => s.Id == id)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .SelectFirstOrDefaultAsync(false);
            return await _skuMapper.FromEntityAsync(sku, withDefaults);
        }


        public async Task<ICollection<SkuDynamic>> GetSkusByProductIdsAsync(ICollection<int> ids)
        {
            SkuQuery conditions = new SkuQuery().NotDeleted().WithProductIds(ids);
            var skus =
                await _skuRepository.Query(conditions)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .SelectAsync(false);
            return await _skuMapper.FromEntityRangeAsync(skus, true);
        }

        public async Task<ICollection<SkuDynamic>> GetSkusAsync(VProductSkuFilter filter)
        {
            var conditions = new SkuQuery().NotDeleted()
                .WithText(filter.SearchText)
                .WithCode(filter.Code)
                .WithDescriptionName(filter.DescriptionName)
                .WithExactCode(filter.ExactCode)
                .WithExactCodes(filter.ExactCodes)
                .WithExactDescriptionName(filter.ExactDescriptionName)
                .WithIds(filter.Ids)
                .WithIdProducts(filter.IdProducts)
                .WithIdProductTypes(filter.IdProductTypes?.Select(p => (int)p).ToArray())
                .ActiveOnly(filter.ActiveOnly).NotHiddenOnly(filter.NotHiddenOnly);

            Func<IQueryable<Sku>, IOrderedQueryable<Sku>> sortable = x => x.OrderBy(y => y.Code);

            ICollection<SkuDynamic> results;

            if (filter.Paging != null)
            {
                results = (await _skuReadServiceAsync.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, conditions,
                    query => query.Include(s => s.OptionValues).Include(s => s.Product).ThenInclude(p => p.OptionValues), sortable, true))
                    .Items;
            }
            else
            {
                results = await _skuReadServiceAsync.SelectAsync(conditions,
                    query => query.Include(s => s.OptionValues).Include(s => s.Product).ThenInclude(p => p.OptionValues), sortable, true);
            }
            if (!string.IsNullOrEmpty(filter.ExactDescriptionName))
            {
                return
                    results.Where(
                        r =>
                            StringComparer.OrdinalIgnoreCase.Equals(FormatProductName(r),
                                filter.ExactDescriptionName)).ToList();
            }
            if (!string.IsNullOrEmpty(filter.DescriptionName))
            {
                return
                    results.Where(r => FormatProductName(r).StartsWith(filter.DescriptionName, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            return results;
        }

        private static string FormatProductName(SkuDynamic r)
        {
            return
                $"{r.Product?.Name ?? string.Empty} {(string)r.Product?.SafeData.SubTitle ?? string.Empty}{(r.Product?.SafeData.SubTitle != null ? " " : string.Empty)}({r.SafeData.QTY ?? 0})";
        }

        public async Task<List<SkuDynamic>> GetSkusAsync(ICollection<SkuInfo> skuInfos, bool withDefaults = false)
        {
            if (skuInfos == null)
                throw new ArgumentNullException(nameof(skuInfos));

            if (skuInfos.Count == 0)
                return new List<SkuDynamic>();

            var skus =
                await _skuRepository.Query(new SkuQuery().ByIds(skuInfos.Select(i => i.Id).ToList()))
                    .Include(s => s.OptionValues)
                    .SelectAsync(false);
            var skusKeyed = skus.ToDictionary(s => s.Id, s => s);
            foreach (var info in skuInfos)
            {
                var sku = skusKeyed[info.Id];
                sku.OptionTypes =
                    _skuMapper.FilterByType((int?)info.IdProductType);
            }
            return await _skuMapper.FromEntityRangeAsync(skus, withDefaults);
        }

        public async Task<List<SkuDynamic>> GetSkusAsync(ICollection<string> codes, bool withDefaults = false)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            if (codes.Count == 0)
                return new List<SkuDynamic>();
            var skus =
                await _skuRepository.Query(new SkuQuery().Including(codes).NotDeleted())
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .SelectAsync(false);
            foreach (var sku in skus)
            {
                sku.OptionTypes =
                     _skuMapper.FilterByType(sku.Product.IdObjectType);
            }
            return await _skuMapper.FromEntityRangeAsync(skus, withDefaults);
        }

        public byte[] GenerateSkuGoogleItemsReportFile(ICollection<ProductContentTransferEntity> products, ProductCategory productCategories)
        {
            byte[] toReturn = null;
            List<SkuGoogleItem> items = new List<SkuGoogleItem>();

            foreach (var productContentTransferEntity in products)
            {
                if (productContentTransferEntity.ProductDynamic.StatusCode == (int)RecordStatusCode.Active
                    && productContentTransferEntity.ProductDynamic.IdVisibility.HasValue
                    && (productContentTransferEntity.ProductDynamic.IdVisibility.Value == CustomerTypeCode.All ||
                    productContentTransferEntity.ProductDynamic.IdVisibility.Value == CustomerTypeCode.Retail)
                    && productContentTransferEntity.ProductDynamic.Skus != null)
                {
                    var activeSkus =
                        productContentTransferEntity.ProductDynamic.Skus.Where(p => p.StatusCode == (int)RecordStatusCode.Active &&
                                                                                    !p.Hidden && p.SafeData.HideFromDataFeed != true).ToArray();

                    if (activeSkus.Length == 0)
                        continue;

                    //find sku code group part in sku codes
                    var skuGroupCode = activeSkus.First().Code.Length >= 4
                        ? activeSkus.First().Code.Substring(0, 4)
                        : activeSkus.First().Code;

                    var find = false;
                    while (!find && skuGroupCode.Length > 0)
                    {
                        find = true;
                        foreach (var skuDynamic in activeSkus)
                        {
                            if (!skuDynamic.Code.StartsWith(skuGroupCode))
                            {
                                find = false;
                                skuGroupCode = skuGroupCode.Substring(0, skuGroupCode.Length - 1);
                                break;
                            }
                        }
                    }

                    foreach (var skuDynamic in activeSkus)
                    {
                        SkuGoogleItem item = new SkuGoogleItem();
                        item.Id = skuDynamic.Id;
                        item.Title = productContentTransferEntity.ProductDynamic.SafeData.GoogleFeedTitle;
                        if (string.IsNullOrEmpty(item.Title))
                        {
                            item.Title = $"{productContentTransferEntity.ProductDynamic.Name} {productContentTransferEntity.ProductDynamic.SafeData.SubTitle}";
                        }
                        item.Url = $"https://{_options.Value.PublicHost}/product/{productContentTransferEntity.ProductContent?.Url}";
                        item.RetailPrice = skuDynamic.Price;
                        item.Description = productContentTransferEntity.ProductDynamic.SafeData.GoogleFeedDescription;
                        item.Condition = "new";
                        item.Brand = "Vital Choice";
                        item.SkuCode = skuDynamic.Code;
                        if (productContentTransferEntity.ProductDynamic.SafeData.Thumbnail != null)
                        {
                            item.Thumbnail = $"https://{_options.Value.PublicHost}{productContentTransferEntity.ProductDynamic.SafeData.Thumbnail}";
                        }
                        item.GoogleCategory =
                            _referenceData.GoogleCategories.FirstOrDefault(
                                p => p.Key == productContentTransferEntity.ProductDynamic.SafeData.GoogleCategory)?.Text;

                        var category = GetProductCategory(productContentTransferEntity.ProductDynamic.CategoryIds.FirstOrDefault(), productCategories);
                        item.ProductRootCategory = GetRootProductCategory(productCategories, category)?.Name;

                        item.Availability = (productContentTransferEntity.ProductDynamic.IdObjectType == (int)ProductType.EGс || productContentTransferEntity.ProductDynamic.IdObjectType == (int)ProductType.Gc ||
                            ((bool?)skuDynamic.SafeData.DisregardStock ?? false) || ((int?)skuDynamic.SafeData.Stock ?? 0) > 0) ?
                            "in stock" : "out of stock";
                        item.SkuCodeGroup = skuGroupCode;
                        if (productContentTransferEntity.ProductDynamic.SafeData.MainProductImage != null)
                        {
                            item.MainProductImage = $"https://{_options.Value.PublicHost}{productContentTransferEntity.ProductDynamic.SafeData.MainProductImage}";
                        }
                        item.Quantity = skuDynamic.SafeData.QTY;
                        item.Manufacturer = "Vital Choice";
                        item.Seller = _referenceData.ProductSellers.FirstOrDefault(
                                p => p.Key == productContentTransferEntity.ProductDynamic.SafeData.Seller)?.Text;

                        items.Add(item);
                    }
                }
            }

            items = items.OrderBy(p => p.Url).ToList();
            toReturn = _skuGoogleItemCSVExportService.ExportToCsv(items);

            return toReturn;
        }

        public byte[] GenerateSkuCriteoItemsReportFile(ICollection<ProductContentTransferEntity> products, ProductCategory productCategories)
        {
            byte[] toReturn = null;
            List<SkuGoogleItem> items = new List<SkuGoogleItem>();

            foreach (var productContentTransferEntity in products)
            {
                if (productContentTransferEntity.ProductDynamic.StatusCode == (int)RecordStatusCode.Active
                    && productContentTransferEntity.ProductDynamic.IdVisibility.HasValue
                    && (productContentTransferEntity.ProductDynamic.IdVisibility.Value == CustomerTypeCode.All ||
                    productContentTransferEntity.ProductDynamic.IdVisibility.Value == CustomerTypeCode.Retail)
                    && productContentTransferEntity.ProductDynamic.Skus != null)
                {
                    var activeSkus =
                        productContentTransferEntity.ProductDynamic.Skus.Where(p => p.StatusCode == (int)RecordStatusCode.Active &&
                                                                                    !p.Hidden && p.SafeData.HideFromDataFeed != true).ToArray();

                    if (activeSkus.Length == 0)
                        continue;

                    //find sku code group part in sku codes
                    var skuGroupCode = activeSkus.First().Code.Length >= 4
                        ? activeSkus.First().Code.Substring(0, 4)
                        : activeSkus.First().Code;

                    var find = false;
                    while (!find && skuGroupCode.Length > 0)
                    {
                        find = true;
                        foreach (var skuDynamic in activeSkus)
                        {
                            if (!skuDynamic.Code.StartsWith(skuGroupCode))
                            {
                                find = false;
                                skuGroupCode = skuGroupCode.Substring(0, skuGroupCode.Length - 1);
                                break;
                            }
                        }
                    }

                    foreach (var skuDynamic in activeSkus)
                    {
                        SkuGoogleItem item = new SkuGoogleItem();
                        item.Id = productContentTransferEntity.ProductDynamic.Id;
                        item.Title = productContentTransferEntity.ProductDynamic.SafeData.GoogleFeedTitle;
                        if (string.IsNullOrEmpty(item.Title))
                        {
                            item.Title = $"{productContentTransferEntity.ProductDynamic.Name} {productContentTransferEntity.ProductDynamic.SafeData.SubTitle}";
                        }
                        item.Url = $"https://{_options.Value.PublicHost}/product/{productContentTransferEntity.ProductContent?.Url}";
                        item.RetailPrice = skuDynamic.Price;
                        item.Description = productContentTransferEntity.ProductDynamic.SafeData.GoogleFeedDescription;
                        item.Condition = "new";
                        item.Brand = "Vital Choice";
                        item.SkuCode = skuDynamic.Code;
                        if (productContentTransferEntity.ProductDynamic.SafeData.Thumbnail != null)
                        {
                            item.Thumbnail = $"https://{_options.Value.PublicHost}{productContentTransferEntity.ProductDynamic.SafeData.Thumbnail}";
                        }
                        item.GoogleCategory =
                            _referenceData.GoogleCategories.FirstOrDefault(
                                p => p.Key == productContentTransferEntity.ProductDynamic.SafeData.GoogleCategory)?.Text;

                        var category = GetProductCategory(productContentTransferEntity.ProductDynamic.CategoryIds.FirstOrDefault(), productCategories);
                        item.ProductRootCategory = GetRootProductCategory(productCategories, category)?.Name;

                        item.Availability = (productContentTransferEntity.ProductDynamic.IdObjectType == (int)ProductType.EGс || productContentTransferEntity.ProductDynamic.IdObjectType == (int)ProductType.Gc ||
                            ((bool?)skuDynamic.SafeData.DisregardStock ?? false) || ((int?)skuDynamic.SafeData.Stock ?? 0) > 0) ?
                            "in stock" : "out of stock";
                        item.SkuCodeGroup = skuGroupCode;
                        if (productContentTransferEntity.ProductDynamic.SafeData.MainProductImage != null)
                        {
                            item.MainProductImage = $"https://{_options.Value.PublicHost}{productContentTransferEntity.ProductDynamic.SafeData.MainProductImage}";
                        }
                        item.Quantity = skuDynamic.SafeData.QTY;
                        item.Manufacturer = "Vital Choice";
                        item.Seller = _referenceData.ProductSellers.FirstOrDefault(
                                p => p.Key == productContentTransferEntity.ProductDynamic.SafeData.Seller)?.Text;

                        items.Add(item);
                    }
                }
            }

            items = items.OrderBy(p => p.Url).ToList();
            toReturn = _skuGoogleItemCSVExportService.ExportToCsv(items);

            return toReturn;
        }

        public async Task UpdateSkuFeedItemsReportFiles()
        {
            try
            {
                var productCategories = await _productCategoryService.GetCategoriesTreeAsync(new ProductCategoryLiteFilter());
                var products = await this.SelectTransferAsync(true);

                var file = GenerateSkuGoogleItemsReportFile(products, productCategories);
                await _storageClient.UploadBlobAsync(_options.Value.AzureStorage.AppFilesContainerName, FileConstants.GOOGLE_PRODUCTS_FEED,
                    file, "text/csv");

                file = GenerateSkuCriteoItemsReportFile(products, productCategories);
                await _storageClient.UploadBlobAsync(_options.Value.AzureStorage.AppFilesContainerName, FileConstants.CRITEO_PRODUCTS_FEED,
                    file, "text/csv");
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
            }
        }

        public async Task<byte[]> GetSkuGoogleItemsReportFile()
        {
            var blob = await _storageClient.DownloadBlobBlockAsync(_options.Value.AzureStorage.AppFilesContainerName, FileConstants.GOOGLE_PRODUCTS_FEED);
            return blob?.File;
        }

        public async Task<byte[]> GetSkuCriteoItemsReportFile()
        {
            var blob = await _storageClient.DownloadBlobBlockAsync(_options.Value.AzureStorage.AppFilesContainerName, FileConstants.CRITEO_PRODUCTS_FEED);
            return blob?.File;
        }

        private ProductCategory GetProductCategory(int? IdCategory, ProductCategory category)
        {
            if (IdCategory.HasValue)
            {
                if (category.Id == IdCategory.Value)
                {
                    return category;
                }
                foreach (var productCategory in category.SubCategories)
                {
                    var findCategory = GetProductCategory(IdCategory, productCategory);
                    if (findCategory != null)
                    {
                        return findCategory;
                    }
                }
            }

            return null;
        }

        private ProductCategory GetRootProductCategory(ProductCategory root, ProductCategory category)
        {
            if (category != null)
            {
                if (category.ParentId.HasValue)
                {
                    var findCategory = GetProductCategory(category.ParentId.Value, root);
                    if (findCategory != null)
                    {
                        if (!findCategory.ParentId.HasValue)
                        {
                            return category;
                        }
                        else
                        {
                            return GetRootProductCategory(root, findCategory);
                        }
                    }
                }
            }

            return null;
        }

        #endregion

        #region Products

        public async Task<PagedList<VShortProduct>> GetShortProductsAsync(FilterBase filter)
        {
            PagedList<VShortProduct> toReturn = null;
            Func<IQueryable<VShortProduct>, IOrderedQueryable<VShortProduct>> sortable = x => x.OrderBy(y => y.Description);

            var query = _vShortProductRepository.Query(p => p.Description.StartsWith(filter.SearchText)).OrderBy(sortable);
            if (filter.Paging != null)
            {
                toReturn = await query.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            }
            else
            {
                var items = await query.SelectAsync(false);
                toReturn=new PagedList<VShortProduct>(items, items.Count);
            }

            return toReturn;
        }

        public async Task<PagedList<ProductDynamic>> GetProductsAsync2(VProductSkuFilter filter)
        {
            Func<IQueryable<Product>, IOrderedQueryable<Product>> sortable = x => x.OrderByDescending(y => y.Name);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VProductSkuSortPath.Name:
                    sortable =
                        x =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Name)
                                : x.OrderByDescending(y => y.Name);
                    break;
                case VProductSkuSortPath.DateEdited:
                    sortable =
                        x =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
                //case VProductSkuSortPath.TaxCode:
                //    sortable =
                //        x =>
                //            sortOrder == FilterSortOrder.Asc
                //                ? x.OrderBy(y => y.TaxCode)
                //                : x.OrderByDescending(y => y.TaxCode);
                //    break;
                case VProductSkuSortPath.IdProductType:
                    sortable =
                        x =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.IdObjectType)
                                : x.OrderByDescending(y => y.IdObjectType);
                    break;
            }

            var products = await SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount,
                new ProductQuery().NotDeleted().WithNameOrSkuCodeLike(filter.SearchText).WithIds(filter.IdProducts),
                query => query.Include(p => p.Skus).ThenInclude(p => p.OptionValues).Include(p => p.OptionValues), sortable, true);

            return products;
        }

        public async Task<ICollection<ProductListItemModel>> GetProductsOnCategoryOrderAsync(int idCategory)
        {
            List<ProductListItemModel> toReturn = new List<ProductListItemModel>();

            var productsOnCategory = await _productToCategoryRepository.Query(p => p.IdCategory == idCategory &&
                                                                                   p.Product.StatusCode != (int)RecordStatusCode.Deleted).SelectAsync(false);
            var conditions =
                new ProductQuery().NotDeleted().WithIds(productsOnCategory.Select(p => p.IdProduct).ToList());
            var products =
                await SelectAsync(conditions, query => query.Include(p => p.Skus).ThenInclude(p => p.OptionValues).Include(p => p.OptionValues));

            foreach (var product in products)
            {
                foreach (var productSku in product.Skus)
                {
                    productSku.Product = product;
                }
            }

            foreach (var productOnCategory in productsOnCategory.OrderBy(p => p.Order))
            {
                var productDynamic = products.FirstOrDefault(p => p.Id == productOnCategory.IdProduct);
                if (productDynamic != null)
                {
                    var item = await Mapper.ToModelAsync<ProductListItemModel>(productDynamic);
                    toReturn.Add(item);
                }
            }

            return toReturn;
        }

        public async Task<bool> UpdateProductsOnCategoryOrderAsync(int idCategory, ICollection<ProductListItemModel> products)
        {
            var dbProductsOnCategory = await _productToCategoryRepository.Query(p => p.IdCategory == idCategory &&
                                                                                   p.Product.StatusCode != (int)RecordStatusCode.Deleted).SelectAsync(false);

            int order = 0;
            var itemsForAdd = new List<ProductToCategory>();
            foreach (var productCategoryOrderModel in products)
            {
                var dbProductOnCategory =
                    dbProductsOnCategory.FirstOrDefault(p => p.IdProduct == productCategoryOrderModel.ProductId);
                if (dbProductOnCategory != null)
                {
                    dbProductOnCategory.Order = order;
                    order++;
                }
                else
                {
                    var item = new ProductToCategory()
                    {
                        IdCategory = idCategory,
                        IdProduct = productCategoryOrderModel.ProductId,
                        Order = order
                    };
                    order++;
                    itemsForAdd.Add(item);
                }
            }
            var itemsForDelete = new List<ProductToCategory>();
            foreach (var dbProduct in dbProductsOnCategory)
            {
                var product =
                    products.FirstOrDefault(p => p.ProductId == dbProduct.IdProduct);
                if (product == null)
                {
                    itemsForDelete.Add(dbProduct);
                }
            }

            await _productToCategoryRepository.UpdateRangeAsync(dbProductsOnCategory);
            if (itemsForDelete.Count > 0)
            {
                await _productToCategoryRepository.DeleteAllAsync(itemsForDelete);
            }
            await _productToCategoryRepository.InsertRangeAsync(itemsForAdd);

            return true;
        }

        public async Task<bool> UpdateProductsOnCategoriesOrderAsync(ICollection<int> categoryIds)
        {
            var dbProductsOnCategories = await _productToCategoryRepository.Query(p => categoryIds.Contains(p.IdCategory) &&
                                                                                   p.Product.StatusCode != (int)RecordStatusCode.Deleted).SelectAsync(false);
            var categories = dbProductsOnCategories.GroupBy(p => p.IdCategory).ToList();
            int order;
            foreach (var category in categories)
            {
                order = 0;
                foreach (var productToCategory in category.OrderBy(p => p.Order))
                {
                    productToCategory.Order = order;
                    order++;
                }
            }

            await _productToCategoryRepository.UpdateRangeAsync(dbProductsOnCategories);

            return true;
        }


        public async Task<IDictionary<int, int>> GetProductIdsBySkuIds(ICollection<int> skuIds)
        {
            var toReturn =
                (await _skuRepository.Query(p => skuIds.Contains(p.Id)).SelectAsync(false)).ToDictionary(p => p.Id,
                    pp => pp.IdProduct);
            return toReturn;
        }

        #endregion

        #region ProductOutOfStockRequests

        public async Task<ICollection<ProductOutOfStockContainer>> GetProductOutOfStockContainersAsync()
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
                         _skuMapper.FilterByType(product.IdObjectType);
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

        public async Task<ProductOutOfStockRequest> AddProductOutOfStockRequestAsync(ProductOutOfStockRequest model)
        {
            model.DateCreated = DateTime.Now;
            await _productOutOfStockRequestRepository.InsertAsync(model);
            return model;
        }

        public async Task<bool> SendProductOutOfStockRequestsAsync(ICollection<int> ids, string messageFormat = null)
        {
            if (ids.Count > 0)
            {
                if (messageFormat == null)
                {
                    if (_appSettings.ProductOutOfStockEmailTemplate == null)
                    {
                        throw new NotSupportedException($"{SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE} not configurated.");
                    }
                    messageFormat = _appSettings.ProductOutOfStockEmailTemplate;
                }

                var items = await _productOutOfStockRequestRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false);
                var productIds = items.Select(p => p.IdProduct).Distinct().ToArray();
                var products = await _productRepository.Query(p => productIds.Contains(p.Id)).SelectAsync(false);
                var contentProducts = await _productContentRepository.Query(p => productIds.Contains(p.Id)).SelectAsync(false);

                foreach (var item in items)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.IdProduct);
                    var contentProduct = contentProducts.FirstOrDefault(p => p.Id == item.IdProduct);
                    if (product != null && contentProduct != null)
                    {
                        var url = $"https://{_options.Value.PublicHost}/product/{contentProduct.Url}";
                        var text = messageFormat.Replace(SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_CUSTOMER_NAME_HOLDER, item.Name).
                            Replace(SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_PRODUCT_NAME_HOLDER, product.Name).
                            Replace(SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE_PRODUCT_URL_HOLDER, url);

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

        public async Task<bool> DeleteProductOutOfStockRequestsAsync(ICollection<int> ids)
        {
            if (ids.Count > 0)
            {
                await _productOutOfStockRequestRepository.DeleteAllAsync(ids);
            }
            return true;
        }

        public async Task<PagedList<VCustomerFavoriteFull>> GetCustomerFavoritesAsync(VCustomerFavoritesFilter filter)
        {
            var temp =
                await
                    _vCustomerFavoriteRepository.Query(new VCustomerFavoriteQuery().WithCustomerId(filter.IdCustomer))
                        .OrderBy(x => x.OrderByDescending(z => z.Quantity))
                        .SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

            var productIds = temp.Items.Select(x => x.Id).ToList();
            var batch = productIds.Take(BaseAppConstants.DEFAULT_MAX_ALLOWED_PARAMS_SQL).ToList();
            var i = 0;
            var pairs = new Dictionary<int, string>();
            while (batch.Count > 0)
            {
                var tempIds = batch;
                pairs.AddRange((await _productContentRepository.Query(p => tempIds.Contains(p.Id)).SelectAsync(false)).Select(
                        x => new KeyValuePair<int, string>(x.Id, x.Url)));

                i++;
                batch =
                    productIds.Skip(i * BaseAppConstants.DEFAULT_MAX_ALLOWED_PARAMS_SQL)
                        .Take(BaseAppConstants.DEFAULT_MAX_ALLOWED_PARAMS_SQL)
                        .ToList();
            }

            var favorites = new PagedList<VCustomerFavoriteFull>();
            foreach (var item in temp.Items)
            {
                if (pairs.ContainsKey(item.Id))
                {
                    favorites.Items.Add(new VCustomerFavoriteFull
                    {
                        Id = item.Id,
                        IdCustomer = item.Id,
                        ProductName = item.ProductName,
                        ProductSubTitle = item.ProductSubTitle,
                        ProductThumbnail = item.ProductThumbnail,
                        Quantity = item.Quantity,
                        Url = pairs.Single(x => x.Key == item.Id).Value
                    });
                }
            }

            favorites.Count = temp.Count;

            return favorites;
        }

        #endregion

        #region ProductContent

        public async Task<ProductDynamic> InsertAsync(ProductContentTransferEntity model)
        {
            //TODO: lock writing DB until we read result
            using (var uow = CreateUnitOfWork())
            {
                var entity = await InsertAsync(model.ProductDynamic, uow, model.ProductContent);
                int id = entity.Id;
                entity = await SelectEntityFirstAsync(o => o.Id == id);
                var newProductDynamic = await DynamicMapper.FromEntityAsync(entity);
                model.ProductDynamic = newProductDynamic;
                await LogItemProductContentTransferEntityChanges(new[] { model });
                return newProductDynamic;
            }
        }

        public async Task<ProductDynamic> UpdateAsync(ProductContentTransferEntity model)
        {
            //TODO: lock writing DB until we read result
            using (var uow = CreateUnitOfWork())
            {
                var entity = await UpdateAsync(model.ProductDynamic, uow, model.ProductContent);
                int id = entity.Id;
                entity = await SelectEntityFirstAsync(o => o.Id == id);
                var newProductDynamic = await DynamicMapper.FromEntityAsync(entity);
                model.ProductDynamic = newProductDynamic;
                await LogItemProductContentTransferEntityChanges(new[] { model });
                return newProductDynamic;
            }
        }

        public async Task<List<ProductDynamic>> UpdateRangeAsync(ICollection<ProductContentTransferEntity> models)
        {
            //TODO: lock writing DB until we read result
            using (var uow = CreateUnitOfWork())
            {
                var entities = await UpdateRangeAsync(models, uow);
                var entityIds = entities.Select(e => e.Id).ToArray();
                entities = await SelectEntitiesAsync(o => entityIds.Contains(o.Id));

                var newProductDynamics = (await DynamicMapper.FromEntityRangeAsync(entities)).ToList();
                foreach (var productContentTransferEntity in models)
                {
                    productContentTransferEntity.ProductDynamic =
                        newProductDynamics.FirstOrDefault(p => p.Id == productContentTransferEntity.ProductDynamic.Id);
                }
                await LogItemProductContentTransferEntityChanges(models);
                return newProductDynamics;
            }
        }

        protected async Task LogItemProductContentTransferEntityChanges(ICollection<ProductContentTransferEntity> models)
        {
            //write product content fields to dynamic for logging purpouse
            foreach (var productContentTransferEntity in models)
            {
                productContentTransferEntity.ProductDynamic.Data.Url = productContentTransferEntity.ProductContent.Url;
                productContentTransferEntity.ProductDynamic.Data.MasterContentItemId = productContentTransferEntity.ProductContent.MasterContentItemId;
                productContentTransferEntity.ProductDynamic.Data.Template = productContentTransferEntity.ProductContent.ContentItem.Template;
                productContentTransferEntity.ProductDynamic.Data.MetaTitle = productContentTransferEntity.ProductContent.ContentItem.Title;
                productContentTransferEntity.ProductDynamic.Data.MetaDescription = productContentTransferEntity.ProductContent.ContentItem.MetaDescription;
            }
            await LogItemChanges(models.Select(p => p.ProductDynamic).ToList());
        }

        protected override async Task<Product> InsertAsync(ProductDynamic model, IUnitOfWorkAsync uow)
        {
            return await InsertAsync(model, uow, null);
        }

        protected override async Task<Product> UpdateAsync(ProductDynamic model, IUnitOfWorkAsync uow)
        {
            return await UpdateAsync(model, uow, null);
        }

        private async Task<Product> InsertAsync(ProductDynamic model, IUnitOfWorkAsync uow, ProductContent productContent)
        {
            if (productContent == null)
            {
                throw new ApiException("Product without content info cannot be created");
            }

            await Validate(productContent);

            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    var product = await base.InsertAsync(model, uow);

                    productContent.Id = product.Id;
                    await UpdateContentForInsert(model, productContent);

                    await _productContentRepository.InsertGraphAsync(productContent);

                    transaction.Commit();

                    return product;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private async Task<Product> UpdateAsync(ProductDynamic model, IUnitOfWorkAsync uow, ProductContent productContent)
        {
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    var product = await base.UpdateAsync(model, uow);
                    if (productContent != null)
                    {
                        var dbProductContent = (await
                            _productContentRepository.Query(p => p.Id == model.Id && p.StatusCode != RecordStatusCode.Deleted)
                                .Include(p => p.ContentItem)
                                .SelectFirstOrDefaultAsync(true));
                        if (dbProductContent != null)
                        {
                            await Validate(productContent);

                            dbProductContent.Url = productContent.Url;
                            dbProductContent.StatusCode = (RecordStatusCode)model.StatusCode;
                            dbProductContent.ContentItem.Updated = DateTime.Now;
                            dbProductContent.ContentItem.Template = productContent.ContentItem.Template;
                            dbProductContent.ContentItem.Title = productContent.ContentItem.Title;
                            dbProductContent.ContentItem.MetaDescription = productContent.ContentItem.MetaDescription;
                            dbProductContent.ContentItem.Description = productContent.ContentItem.Description;

                            await _productContentRepository.UpdateAsync(dbProductContent);
                        }
                        else
                        {
                            await UpdateContentForInsert(model, productContent);

                            await _productContentRepository.InsertGraphAsync(productContent);
                        }
                    }
                    transaction.Commit();
                    return product;

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private async Task<ICollection<Product>> UpdateRangeAsync(ICollection<ProductContentTransferEntity> models, IUnitOfWorkAsync uow)
        {
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    var ids = models.Select(p => p.ProductDynamic.Id).ToList();
                    var products = await base.UpdateRangeAsync(models.Select(p => p.ProductDynamic).ToList(), uow);
                    var dbProductContents = (await
                            _productContentRepository.Query(p => ids.Contains(p.Id) && p.StatusCode != RecordStatusCode.Deleted)
                                .Include(p => p.ContentItem)
                                .SelectAsync(true)).ToList();
                    foreach (var dbProductContent in dbProductContents)
                    {
                        var model = models.FirstOrDefault(p => p.ProductDynamic.Id == dbProductContent.Id);
                        if (model?.ProductContent != null)
                        {
                            await Validate(model.ProductContent);

                            dbProductContent.Url = model.ProductContent.Url;
                            dbProductContent.StatusCode = (RecordStatusCode)model.ProductDynamic.StatusCode;
                            dbProductContent.ContentItem.Updated = DateTime.Now;
                            dbProductContent.ContentItem.Template = model.ProductContent.ContentItem.Template;
                            dbProductContent.ContentItem.Title = model.ProductContent.ContentItem.Title;
                            dbProductContent.ContentItem.MetaDescription = model.ProductContent.ContentItem.MetaDescription;
                            dbProductContent.ContentItem.Description = model.ProductContent.ContentItem.Description;
                        }
                    }

                    await _productContentRepository.UpdateRangeAsync(dbProductContents);

                    transaction.Commit();
                    return products;

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private async Task UpdateContentForInsert(ProductDynamic model, ProductContent productContent)
        {
            productContent.StatusCode = (RecordStatusCode)model.StatusCode;
            productContent.UserId = model.IdEditedBy;
            productContent.ContentItem.Created = DateTime.Now;
            productContent.ContentItem.Updated = productContent.ContentItem.Created;
            if (productContent.MasterContentItemId == 0)
            {
                //set predefined master
                var contentType = (await _contentTypeRepository.Query(p => p.Id == (int)ContentType.Product).SelectFirstOrDefaultAsync(false));
                if (contentType?.DefaultMasterContentItemId != null)
                {
                    productContent.MasterContentItemId = contentType.DefaultMasterContentItemId.Value;
                }
                else
                {
                    throw new AppValidationException("The default master template isn't confugured. Please contact support.");
                }
            }
        }

        protected async Task Validate(ProductContent model)
        {
            List<MessageInfo> errors = new List<MessageInfo>();

            var productSameUrl =
                await
                    _productContentRepository.Query(
                        new ProductContentQuery().NotDeleted().Excluding(model.Id).WithUrl(model.Url))
                        .SelectAsync(false);


            if (productSameUrl.Count > 0)
            {
                errors.AddRange(
                    model.CreateBasicError()
                        .Property(p => p.Url)
                        .Error("Product url should be unique in the database")
                        .Build());
            }

            errors.Raise();
        }

        protected override async Task<bool> DeleteAsync(IUnitOfWorkAsync uow, int id, bool physically)
        {
            var result = await base.DeleteAsync(uow, id, physically);
            if (result)
            {
                var productContent = (await _productContentRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(true));
                if (productContent != null && productContent.StatusCode != RecordStatusCode.Deleted)
                {
                    productContent.StatusCode = RecordStatusCode.Deleted;
                    result = await _productContentRepository.UpdateAsync(productContent);
                }
            }
            return result;
        }

        public async Task<ProductContentTransferEntity> SelectTransferByIdOldAsync(int id)
        {
            ProductContentTransferEntity toReturn = null;

            var content = (await _productContentRepository.Query(p => p.IdOld == id).Include(p => p.ContentItem).SelectFirstOrDefaultAsync(false));
            if (content != null)
            {
                var ecomProduct = await this.SelectAsync(content.Id);
                if (ecomProduct != null)
                {
                    toReturn = new ProductContentTransferEntity()
                    {
                        ProductContent = content,
                        ProductDynamic = ecomProduct,
                    };
                }
            }

            return toReturn;
        }

        public async Task<ProductContentTransferEntity> SelectTransferAsync(int id, bool withDefaults = false)
        {
            var toReturn = new ProductContentTransferEntity { ProductDynamic = await this.SelectAsync(id, withDefaults) };
            if (toReturn.ProductDynamic != null)
            {
                toReturn.ProductContent = await SelectContentForTransfer(id);
            }
            return toReturn;
        }

        public async Task<ICollection<ProductContentTransferEntity>> SelectTransfersAsync(ICollection<int> ids, bool withDefaults = false)
        {
            var toReturn = (await this.SelectAsync(ids, withDefaults)).Select(p => new ProductContentTransferEntity()
            {
                ProductDynamic = p
            }).ToList();
            var contents = await SelectProductContents(toReturn.Select(p => p.ProductDynamic.Id).ToList());
            foreach (var productContentTransferEntity in toReturn)
            {
                productContentTransferEntity.ProductContent = contents.FirstOrDefault(p => p.Id == productContentTransferEntity.ProductDynamic.Id);
            }
            return toReturn;
        }

        public async Task<ICollection<ProductContentTransferEntity>> SelectTransferAsync(bool withDefaults = false)
        {
            var toReturn = (await this.SelectAsync(p => p.StatusCode != (int)RecordStatusCode.Deleted, withDefaults: withDefaults)).Select(p =>
                new ProductContentTransferEntity { ProductDynamic = p }).ToList();
            var contents = await SelectContentsForTransfer();
            foreach (var productContentTransferEntity in toReturn)
            {
                var content = contents.FirstOrDefault(p => p.Id == productContentTransferEntity.ProductDynamic.Id);
                productContentTransferEntity.ProductContent = content;
            }
            return toReturn;
        }

        public async Task<ProductContentTransferEntity> SelectTransferAsync(Guid id, bool withDefaults = false)
        {
            var toReturn = new ProductContentTransferEntity
            {
                ProductDynamic = await DynamicMapper.FromEntityAsync(
                    await
                        SelectEntityFirstAsync(o => o.PublicId == id && o.StatusCode != (int)RecordStatusCode.Deleted), withDefaults)
            };
            if (toReturn.ProductDynamic != null)
            {
                toReturn.ProductContent = await SelectContentForTransfer(toReturn.ProductDynamic.Id);
            }
            return toReturn;
        }

        public async Task<ProductContentTransferEntity> SelectTransferAsync(string url, bool withDefaults = false)
        {
            var productContent = (await _productContentRepository.Query(p => p.Url.Equals(url)).Include(p => p.ContentItem).SelectFirstOrDefaultAsync(false));

            var toReturn = new ProductContentTransferEntity
            {
                ProductDynamic = productContent != null ? await DynamicMapper.FromEntityAsync(
                    await
                        SelectEntityFirstAsync(o => o.Id == productContent.Id && o.StatusCode != (int)RecordStatusCode.Deleted), withDefaults) : null,
                ProductContent = productContent
            };
            return toReturn;
        }

        public async Task<int> GetProductInternalIdAsync(Guid productId)
        {
            var productQuery = new ProductQuery().WithPublicId(productId).NotDeleted();

            return (await _productRepository.Query(productQuery).SelectFirstOrDefaultAsync(false))?.Id ?? 0;
        }

        #endregion

        #region Reports

        public async Task<ICollection<SkuBreakDownReportItem>> GetSkuBreakDownReportItemsAsync(SkuBreakDownReportFilter filter)
        {
            List<SkuBreakDownReportItem> toReturn = new List<SkuBreakDownReportItem>();
            var dbItems = await _sPEcommerceRepository.GetSkuBreakDownReportRawItemsAsync(filter);
            dbItems = dbItems.OrderBy(p => p.Code).ToList();

            foreach (var skuBreakDownReportRawItem in dbItems)
            {
                SkuBreakDownReportItem item = toReturn.FirstOrDefault(p => p.IdSku == skuBreakDownReportRawItem.IdSku);
                if (item == null)
                {
                    item = new SkuBreakDownReportItem();
                    item.IdSku = skuBreakDownReportRawItem.IdSku;
                    item.IdProduct = skuBreakDownReportRawItem.IdProduct;
                    item.Code = skuBreakDownReportRawItem.Code;
                    toReturn.Add(item);
                }

                if (skuBreakDownReportRawItem.CustomerIdObjectType == (int)CustomerType.Retail)
                {
                    item.RetailAmount += skuBreakDownReportRawItem.Amount;
                    item.TotalAmount += skuBreakDownReportRawItem.Amount;
                    item.RetailQuantity += skuBreakDownReportRawItem.Quantity;
                    item.TotalQuantity += skuBreakDownReportRawItem.Quantity;
                }
                if (skuBreakDownReportRawItem.CustomerIdObjectType == (int)CustomerType.Wholesale)
                {
                    item.WholesaleAmount += skuBreakDownReportRawItem.Amount;
                    item.TotalAmount += skuBreakDownReportRawItem.Amount;
                    item.WholesaleQuantity += skuBreakDownReportRawItem.Quantity;
                    item.TotalQuantity += skuBreakDownReportRawItem.Quantity;
                }
            }

            foreach (var item in toReturn)
            {
                item.RetailPercent = item.TotalQuantity != 0 ? Math.Round((decimal)item.RetailQuantity * 100 / item.TotalQuantity, 2) : 0;
                item.WholesalePercent = item.TotalQuantity != 0 ? Math.Round((decimal)item.WholesaleQuantity * 100 / item.TotalQuantity, 2) : 0;
            }

            return toReturn;
        }

        public async Task<SkuPOrderTypeBreakDownReport> GetSkuPOrderTypeBreakDownReportAsync(SkuPOrderTypeBreakDownReportFilter filter)
        {
            SkuPOrderTypeBreakDownReport toReturn = new SkuPOrderTypeBreakDownReport();
            toReturn.FrequencyType = filter.FrequencyType;

            if (filter.From > filter.To)
            {
                return toReturn;
            }

            var dbItems = await _sPEcommerceRepository.GetSkuPOrderTypeBreakDownReportRawItemsAsync(filter);
            //remove out of view skus from an order(which was filtered by at least one filter code inside) which aren't matched to this condition
            if (!string.IsNullOrEmpty(filter.Code))
            {
                dbItems = dbItems.Where(p => p.Code.StartsWith(filter.Code, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            //create periods
            DateTime current = filter.From;
            if (filter.FrequencyType == FrequencyType.Monthly)
            {
                //start of next month
                current = new DateTime(current.Year, current.Month, 1, current.Hour, current.Minute, current.Second);
                current = current.AddMonths(1);
            }
            if (filter.FrequencyType == FrequencyType.Weekly)
            {
                //start of next week
                current = current.AddDays(-(int)current.DayOfWeek);
                current = current.AddDays(7);
            }
            if (filter.FrequencyType == FrequencyType.Daily)
            {
                current = current.AddDays(1);
            }
            if (current > filter.To)
            {
                current = filter.To;
            }
            var period = new SkuPOrderTypeBreakDownReportPOrderTypePeriod()
            {
                From = filter.From,
                To = current,
            };
            toReturn.POrderTypePeriods.Add(period);

            while (current < filter.To)
            {
                var nextCurrent = current;
                if (filter.FrequencyType == FrequencyType.Monthly)
                {
                    nextCurrent = nextCurrent.AddMonths(1);
                }
                if (filter.FrequencyType == FrequencyType.Weekly)
                {
                    nextCurrent = nextCurrent.AddDays(7);
                }
                if (filter.FrequencyType == FrequencyType.Daily)
                {
                    nextCurrent = nextCurrent.AddDays(1);
                }

                if (nextCurrent > filter.To)
                {
                    nextCurrent = filter.To;
                }

                period = new SkuPOrderTypeBreakDownReportPOrderTypePeriod()
                {
                    From = current,
                    To = nextCurrent,
                };
                toReturn.POrderTypePeriods.Add(period);
                current = nextCurrent;
            }

            List<int> usedOrderIds = new List<int>();

            foreach (var skuPOrderTypeBreakDownReportRawItem in dbItems)
            {
                var pOrderTypePeriod = toReturn.POrderTypePeriods.FirstOrDefault(p => p.From <= skuPOrderTypeBreakDownReportRawItem.DateCreated &&
                    p.To > skuPOrderTypeBreakDownReportRawItem.DateCreated);
                if (pOrderTypePeriod != null && usedOrderIds.All(p => p != skuPOrderTypeBreakDownReportRawItem.IdOrder))
                {
                    if (skuPOrderTypeBreakDownReportRawItem.POrderType == (int)POrderType.P)
                    {
                        pOrderTypePeriod.PCount++;
                    }
                    if (skuPOrderTypeBreakDownReportRawItem.POrderType == (int)POrderType.NP)
                    {
                        pOrderTypePeriod.NPCount++;
                    }
                    if (skuPOrderTypeBreakDownReportRawItem.POrderType == (int)POrderType.PNP)
                    {
                        pOrderTypePeriod.PNPCount++;
                    }
                    pOrderTypePeriod.TotalCount++;
                    usedOrderIds.Add(skuPOrderTypeBreakDownReportRawItem.IdOrder);
                }
            }

            foreach (var skuPOrderTypeBreakDownReportPOrderTypePeriod in toReturn.POrderTypePeriods)
            {
                skuPOrderTypeBreakDownReportPOrderTypePeriod.PPercent = skuPOrderTypeBreakDownReportPOrderTypePeriod.TotalCount != 0 ?
                    Math.Round(((decimal)100 * (skuPOrderTypeBreakDownReportPOrderTypePeriod.PCount + skuPOrderTypeBreakDownReportPOrderTypePeriod.PNPCount))
                                / skuPOrderTypeBreakDownReportPOrderTypePeriod.TotalCount, 2) :
                               0;
            }

            foreach (var skuPOrderTypeBreakDownReportRawItem in dbItems)
            {
                var skuItem = toReturn.Skus.FirstOrDefault(p => p.IdSku == skuPOrderTypeBreakDownReportRawItem.IdSku);
                if (skuItem == null)
                {
                    skuItem = new SkuPOrderTypeBreakDownReportSkuItem();
                    skuItem.IdSku = skuPOrderTypeBreakDownReportRawItem.IdSku;
                    skuItem.IdProduct = skuPOrderTypeBreakDownReportRawItem.IdProduct;
                    skuItem.Code = skuPOrderTypeBreakDownReportRawItem.Code;
                    skuItem.Periods = toReturn.POrderTypePeriods.Select(p => new SkuPOrderTypeBreakDownReportSkuPeriod()
                    {
                        From = p.From,
                        To = p.To
                    }).ToList();
                    toReturn.Skus.Add(skuItem);
                }

                var skuPeriod = skuItem.Periods.FirstOrDefault(p => p.From <= skuPOrderTypeBreakDownReportRawItem.DateCreated &&
                    p.To > skuPOrderTypeBreakDownReportRawItem.DateCreated);
                if (skuPeriod != null)
                {
                    skuPeriod.Quantity += skuPOrderTypeBreakDownReportRawItem.Quantity;
                }
            }

            toReturn.Skus = toReturn.Skus.OrderBy(p => p.Code).ToList();

            return toReturn;
        }

        public async Task<SkuPOrderTypeBreakDownReport> GetSkuPOrderTypeFutureBreakDownReportAsync(SkuPOrderTypeBreakDownReportFilter filter)
        {
            SkuPOrderTypeBreakDownReport toReturn = new SkuPOrderTypeBreakDownReport();
            toReturn.FrequencyType = filter.FrequencyType;

            if (filter.From > filter.To)
            {
                return toReturn;
            }

            var dbItems = await _sPEcommerceRepository.GetSkuPOrderTypeFutureBreakDownReportRawItemsAsync(filter);
            //remove out of view skus from an order(which was filtered by at least one filter code inside) which aren't matched to this condition
            if (!string.IsNullOrEmpty(filter.Code))
            {
                dbItems = dbItems.Where(p => p.Code.StartsWith(filter.Code, StringComparison.InvariantCultureIgnoreCase)).ToList();
            }

            //create periods
            DateTime current = filter.From;
            if (filter.FrequencyType == FrequencyType.Monthly)
            {
                //start of next month
                current = new DateTime(current.Year, current.Month, 1, current.Hour, current.Minute, current.Second);
                current = current.AddMonths(1);
            }
            if (filter.FrequencyType == FrequencyType.Weekly)
            {
                //start of next week
                current = current.AddDays(-(int)current.DayOfWeek);
                current = current.AddDays(7);
            }
            if (filter.FrequencyType == FrequencyType.Daily)
            {
                current = current.AddDays(1);
            }
            if (current > filter.To)
            {
                current = filter.To;
            }
            var period = new SkuPOrderTypeBreakDownReportPOrderTypePeriod()
            {
                From = filter.From,
                To = current,
            };
            toReturn.POrderTypePeriods.Add(period);

            while (current < filter.To)
            {
                var nextCurrent = current;
                if (filter.FrequencyType == FrequencyType.Monthly)
                {
                    nextCurrent = nextCurrent.AddMonths(1);
                }
                if (filter.FrequencyType == FrequencyType.Weekly)
                {
                    nextCurrent = nextCurrent.AddDays(7);
                }
                if (filter.FrequencyType == FrequencyType.Daily)
                {
                    nextCurrent = nextCurrent.AddDays(1);
                }

                if (nextCurrent > filter.To)
                {
                    nextCurrent = filter.To;
                }

                period = new SkuPOrderTypeBreakDownReportPOrderTypePeriod()
                {
                    From = current,
                    To = nextCurrent,
                };
                toReturn.POrderTypePeriods.Add(period);
                current = nextCurrent;
            }

            List<int> usedOrderIds = new List<int>();

            foreach (var skuPOrderTypeBreakDownReportRawItem in dbItems)
            {
                CalculatePOrderTypeInfo(skuPOrderTypeBreakDownReportRawItem.ShipDelayDate,
                    skuPOrderTypeBreakDownReportRawItem.IdOrder,
                    skuPOrderTypeBreakDownReportRawItem.POrderType, toReturn, usedOrderIds);
                CalculatePOrderTypeInfo(skuPOrderTypeBreakDownReportRawItem.ShipDelayDate,
                    skuPOrderTypeBreakDownReportRawItem.IdOrder,
                    skuPOrderTypeBreakDownReportRawItem.POrderType, toReturn, usedOrderIds);
                CalculatePOrderTypeInfo(skuPOrderTypeBreakDownReportRawItem.ShipDelayDate,
                    skuPOrderTypeBreakDownReportRawItem.IdOrder,
                    skuPOrderTypeBreakDownReportRawItem.POrderType, toReturn, usedOrderIds);
            }

            foreach (var skuPOrderTypeBreakDownReportPOrderTypePeriod in toReturn.POrderTypePeriods)
            {
                skuPOrderTypeBreakDownReportPOrderTypePeriod.PPercent = skuPOrderTypeBreakDownReportPOrderTypePeriod.TotalCount != 0 ?
                    Math.Round(((decimal)100 * (skuPOrderTypeBreakDownReportPOrderTypePeriod.PCount + skuPOrderTypeBreakDownReportPOrderTypePeriod.PNPCount))
                                / skuPOrderTypeBreakDownReportPOrderTypePeriod.TotalCount, 2) :
                               0;
            }

            foreach (var skuPOrderTypeBreakDownReportRawItem in dbItems)
            {
                var skuItem = toReturn.Skus.FirstOrDefault(p => p.IdSku == skuPOrderTypeBreakDownReportRawItem.IdSku);
                if (skuItem == null)
                {
                    skuItem = new SkuPOrderTypeBreakDownReportSkuItem();
                    skuItem.IdSku = skuPOrderTypeBreakDownReportRawItem.IdSku;
                    skuItem.IdProduct = skuPOrderTypeBreakDownReportRawItem.IdProduct;
                    skuItem.Code = skuPOrderTypeBreakDownReportRawItem.Code;
                    skuItem.Periods = toReturn.POrderTypePeriods.Select(p => new SkuPOrderTypeBreakDownReportSkuPeriod()
                    {
                        From = p.From,
                        To = p.To
                    }).ToList();
                    toReturn.Skus.Add(skuItem);
                }

                var skuPeriod = skuItem.Periods.FirstOrDefault(p => skuPOrderTypeBreakDownReportRawItem.ShipDelayDate.HasValue &&
                    p.From <= skuPOrderTypeBreakDownReportRawItem.ShipDelayDate.Value &&
                    p.To > skuPOrderTypeBreakDownReportRawItem.ShipDelayDate.Value);
                if (skuPeriod != null)
                {
                    skuPeriod.Quantity += skuPOrderTypeBreakDownReportRawItem.Quantity;
                }

                skuPeriod = skuItem.Periods.FirstOrDefault(p => skuPOrderTypeBreakDownReportRawItem.ShipDelayDateP.HasValue &&
                    p.From <= skuPOrderTypeBreakDownReportRawItem.ShipDelayDateP.Value &&
                    p.To > skuPOrderTypeBreakDownReportRawItem.ShipDelayDateP.Value);
                //do not calculate double time parts of the same order in different ship delay dates
                if (skuPeriod != null && skuPOrderTypeBreakDownReportRawItem.ProductIdObjectType != (int)ProductType.NonPerishable)
                {
                    skuPeriod.Quantity += skuPOrderTypeBreakDownReportRawItem.Quantity;
                }

                skuPeriod = skuItem.Periods.FirstOrDefault(p => skuPOrderTypeBreakDownReportRawItem.ShipDelayDateNP.HasValue &&
                    p.From <= skuPOrderTypeBreakDownReportRawItem.ShipDelayDateNP.Value &&
                    p.To > skuPOrderTypeBreakDownReportRawItem.ShipDelayDateNP.Value);
                //do not calculate double time parts of the same order in different ship delay dates
                if (skuPeriod != null && skuPOrderTypeBreakDownReportRawItem.ProductIdObjectType != (int)ProductType.Perishable)
                {
                    skuPeriod.Quantity += skuPOrderTypeBreakDownReportRawItem.Quantity;
                }
            }

            toReturn.Skus = toReturn.Skus.OrderBy(p => p.Code).ToList();

            return toReturn;
        }

        private void CalculatePOrderTypeInfo(DateTime? date, int idOrder, int? pOrderType, SkuPOrderTypeBreakDownReport report, List<int> usedOrderIds)
        {
            if (date.HasValue)
            {
                var pOrderTypePeriod = report.POrderTypePeriods.FirstOrDefault(p => p.From <= date.Value && p.To > date.Value);
                if (pOrderTypePeriod != null && usedOrderIds.All(p => p != idOrder))
                {
                    if (pOrderType == (int)POrderType.P)
                    {
                        pOrderTypePeriod.PCount++;
                    }
                    if (pOrderType == (int)POrderType.NP)
                    {
                        pOrderTypePeriod.NPCount++;
                    }
                    if (pOrderType == (int)POrderType.PNP)
                    {
                        pOrderTypePeriod.PNPCount++;
                    }
                    pOrderTypePeriod.TotalCount++;
                    usedOrderIds.Add(idOrder);
                }
            }
        }
        
        public async Task<PagedList<SkuAverageDailySalesBySkuReportItem>> GetSkuAverageDailySalesBySkuReportItemsAsync(
            SkuAverageDailySalesReportFilter filter)
        {
            await AssignProductSkusForAverageDailySalesFilter(filter);

            var tData = await _sPEcommerceRepository.GetSkuAverageDailySalesReportRawItemsAsync(filter);

            var conditions = new SkuOOSHistoryItemQuery().ByDates(filter.From, filter.To).BySkuIds(filter.SkuIds);
            var oosHistoryItems = await _skuOOSHistoryItemRepository.Query(conditions).SelectAsync(false);

            var data = new List<SkuAverageDailySalesBySkuReportItem>();
            var filterDaysRange = (filter.To - filter.From).Days;
            foreach (var rawItem in tData)
            {
                SkuAverageDailySalesBySkuReportItem item=new SkuAverageDailySalesBySkuReportItem();
                item.InStock = BusinessHelper.InStock(rawItem.ProductIdObjectType, rawItem.DisregardStock, rawItem.Stock);
                item.CurrentOOS = !item.InStock;
                item.StatusCode = rawItem.StatusCode;

                if (filter.OnlyOOS && item.InStock)
                {
                    continue;
                }
                if (filter.OnlyActiveSku && item.StatusCode!=RecordStatusCode.Active)
                {
                    continue;
                }

                item.Id = rawItem.Id;
                item.IdProduct = rawItem.IdProduct;
                item.ProductCategories = rawItem.ProductCategories;
                item.StatusCode = rawItem.StatusCode;
                item.Code = rawItem.Code;

                item.ProductName = $"{rawItem.Name} {rawItem.SubTitle}";
                item.SkuName = $"{rawItem.Name} {rawItem.SubTitle} ({rawItem.QTY})";

                item.TotalAmount = Math.Round(rawItem.TotalAmount, 2);

                var oosItems = oosHistoryItems.Where(p => p.IdSku == item.Id).ToList();
                var duration = TimeSpan.Zero;
                foreach (var skuOosHistoryItem in oosItems)
                {
                    var from = skuOosHistoryItem.StartDate > filter.From ? skuOosHistoryItem.StartDate : filter.From;
                    var to = skuOosHistoryItem.EndDate.HasValue
                        ? skuOosHistoryItem.EndDate.Value < filter.To ? skuOosHistoryItem.EndDate.Value : filter.To
                        : filter.To;
                    duration = duration + (to - from);
                }
                item.DaysOOS = duration.Days + (duration.Hours >= 12 ? 1 : 0);

                item.AverageDailyAmount = filterDaysRange - item.DaysOOS > 0
                                        ? Math.Round(item.TotalAmount/(filterDaysRange - item.DaysOOS),2)
                                        : 0;
                item.TotalOOSImpactAmount = item.AverageDailyAmount*item.DaysOOS;

                data.Add(item);
            }

            var toReturn=new PagedList<SkuAverageDailySalesBySkuReportItem>();
            toReturn.Count = data.Count;

            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case SkuAverageDailySalesSortPath.Id:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.Id).ToList()
                                : data.OrderByDescending(y => y.Id).ToList();
                    break;
                case SkuAverageDailySalesSortPath.IdProduct:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.IdProduct).ToList()
                                : data.OrderByDescending(y => y.IdProduct).ToList();
                    break;
                case SkuAverageDailySalesSortPath.Code:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.Code).ToList()
                                : data.OrderByDescending(y => y.Code).ToList();
                    break;
                case SkuAverageDailySalesSortPath.InStock:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.InStock).ToList()
                                : data.OrderByDescending(y => y.InStock).ToList();
                    break;
                case SkuAverageDailySalesSortPath.DaysOOS:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.DaysOOS).ToList()
                                : data.OrderByDescending(y => y.DaysOOS).ToList();
                    break;
                case SkuAverageDailySalesSortPath.TotalAmount:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.TotalAmount).ToList()
                                : data.OrderByDescending(y => y.TotalAmount).ToList();
                    break;
                case SkuAverageDailySalesSortPath.AverageDailyAmount:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.AverageDailyAmount).ToList()
                                : data.OrderByDescending(y => y.AverageDailyAmount).ToList();
                    break;
                case SkuAverageDailySalesSortPath.TotalOOSImpactAmount:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.TotalOOSImpactAmount).ToList()
                                : data.OrderByDescending(y => y.TotalOOSImpactAmount).ToList();
                    break;
            }

            if (filter.Paging != null)
            {
                var pageNo = filter.Paging.PageIndex - 1;
                toReturn.Items = pageNo <= 0
                    ? data.Take(filter.Paging.PageItemCount).ToList()
                    : data.Skip(pageNo * filter.Paging.PageItemCount).Take(filter.Paging.PageItemCount).ToList();
            }
            else
            {
                toReturn.Items = data;
            }

            return toReturn;
        }

        public async Task<PagedList<SkuAverageDailySalesByProductReportItem>> GetSkuAverageDailySalesByProductReportItemsAsync(
            SkuAverageDailySalesReportFilter filter)
        {
            await AssignProductSkusForAverageDailySalesFilter(filter);

            var tData = await _sPEcommerceRepository.GetSkuAverageDailySalesReportRawItemsAsync(filter);

            var conditions = new SkuOOSHistoryItemQuery().ByDates(filter.From, filter.To).BySkuIds(filter.SkuIds);
            var oosHistoryItems = await _skuOOSHistoryItemRepository.Query(conditions).SelectAsync(false);

            var data = new List<SkuAverageDailySalesByProductReportItem>();
            var filterDaysRange = (filter.To - filter.From).Days;
            foreach (var productSkuGroup in tData.GroupBy(p=>p.IdProduct))
            {
                SkuAverageDailySalesByProductReportItem item = new SkuAverageDailySalesByProductReportItem();

                var rawSku = productSkuGroup.FirstOrDefault();
                item.IdProduct = rawSku.IdProduct;
                item.ProductCategories = rawSku.ProductCategories;
                item.ProductName = $"{rawSku.Name} {rawSku.SubTitle}";

                foreach (var rawItem in productSkuGroup)
                {
                    var inStock = BusinessHelper.InStock(rawItem.ProductIdObjectType, rawItem.DisregardStock, rawItem.Stock);
                    item.InStock = item.InStock || inStock;
                    item.TotalAmount += Math.Round(rawItem.TotalAmount,2);

                    var oosItems = oosHistoryItems.Where(p => p.IdSku == rawItem.Id).ToList();
                    var duration = TimeSpan.Zero;
                    foreach (var skuOosHistoryItem in oosItems)
                    {
                        var from = skuOosHistoryItem.StartDate > filter.From ? skuOosHistoryItem.StartDate : filter.From;
                        var to = skuOosHistoryItem.EndDate.HasValue
                            ? skuOosHistoryItem.EndDate.Value < filter.To ? skuOosHistoryItem.EndDate.Value : filter.To
                            : filter.To;
                        duration = duration + (to - from);
                    }
                    var daysOOS = duration.Days + (duration.Hours >= 12 ? 1 : 0);
                    item.DaysOOS = daysOOS > item.DaysOOS ? daysOOS : item.DaysOOS;

                    item.Skus.Add(new SkuAverageDailySalesByProductSkuItem()
                    {
                        Code = rawItem.Code,
                        InStock = inStock,
                    });
                    item.SkusLine += inStock ? rawItem.Code + ", " : $"({rawItem.Code})" + ", ";
                }

                if (item.SkusLine.EndsWith(", "))
                {
                    item.SkusLine = item.SkusLine.Substring(0, item.SkusLine.Length - 2);
                }

                item.CurrentOOS = !item.InStock;
                //at least one in stock
                if (filter.OnlyOOS && item.InStock)
                {
                    continue;
                }
                //sku active not actual for this report

                item.AverageDailyAmount = filterDaysRange - item.DaysOOS > 0
                                        ? Math.Round(item.TotalAmount / (filterDaysRange - item.DaysOOS),2)
                                        : 0;
                item.TotalOOSImpactAmount = item.AverageDailyAmount * item.DaysOOS;

                data.Add(item);
            }

            var toReturn = new PagedList<SkuAverageDailySalesByProductReportItem>();
            toReturn.Count = data.Count;

            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case SkuAverageDailySalesSortPath.IdProduct:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.IdProduct).ToList()
                                : data.OrderByDescending(y => y.IdProduct).ToList();
                    break;
                case SkuAverageDailySalesSortPath.InStock:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.InStock).ToList()
                                : data.OrderByDescending(y => y.InStock).ToList();
                    break;
                case SkuAverageDailySalesSortPath.DaysOOS:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.DaysOOS).ToList()
                                : data.OrderByDescending(y => y.DaysOOS).ToList();
                    break;
                case SkuAverageDailySalesSortPath.TotalAmount:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.TotalAmount).ToList()
                                : data.OrderByDescending(y => y.TotalAmount).ToList();
                    break;
                case SkuAverageDailySalesSortPath.AverageDailyAmount:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.AverageDailyAmount).ToList()
                                : data.OrderByDescending(y => y.AverageDailyAmount).ToList();
                    break;
                case SkuAverageDailySalesSortPath.TotalOOSImpactAmount:
                    data = sortOrder == FilterSortOrder.Asc
                                ? data.OrderBy(y => y.TotalOOSImpactAmount).ToList()
                                : data.OrderByDescending(y => y.TotalOOSImpactAmount).ToList();
                    break;
            }

            if (filter.Paging != null)
            {
                var pageNo = filter.Paging.PageIndex - 1;
                toReturn.Items = pageNo <= 0
                    ? data.Take(filter.Paging.PageItemCount).ToList()
                    : data.Skip(pageNo * filter.Paging.PageItemCount).Take(filter.Paging.PageItemCount).ToList();
            }
            else
            {
                toReturn.Items = data;
            }

            return toReturn;
        }

        private async Task AssignProductSkusForAverageDailySalesFilter(SkuAverageDailySalesReportFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.ProductName))
            {
                var shortProduct = (await GetShortProductsAsync(new VProductSkuFilter()
                {
                    SearchText = filter.ProductName,
                    Paging = new Paging() {PageItemCount = 1}
                })).Items.FirstOrDefault();
                if (shortProduct != null)
                {
                    var product = await SelectAsync(shortProduct.Id);
                    if (product != null)
                    {
                        filter.SkuIds.AddRange(product.Skus.Select(p => p.Id).ToList());
                    }
                }
            }
        }

        #endregion
    }
}