using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Mail;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.Business.Services.Products
{
    public class ProductService : ExtendedEcommerceDynamicService<ProductDynamic, Product, ProductOptionType, ProductOptionValue>, IProductService
    {
        private readonly VProductSkuRepository _vProductSkuRepository;
        private readonly IEcommerceRepositoryAsync<VSku> _vSkuRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly ProductMapper _mapper;
        private readonly IEcommerceRepositoryAsync<VCustomerFavorite> _vCustomerFavoriteRepository;
        private readonly SkuMapper _skuMapper;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly OrderSkusRepository _orderSkusRepositoryRepository;
        private readonly IEcommerceRepositoryAsync<ProductOutOfStockRequest> _productOutOfStockRequestRepository;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly IRepositoryAsync<ProductContent> _productContentRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> _contentTypeRepository;
        private readonly IOptions<AppOptions> _options;
        private readonly IEcommerceRepositoryAsync<SkuOptionValue> _skuOptionValueRepositoryAsync;

        public async Task<ProductContent> SelectContentForTransfer(int id)
		{
			return (await _productContentRepository.Query(p => p.Id == id).Include(p => p.ContentItem).SelectAsync(false)).FirstOrDefault();
		}

	    public async Task<ICollection<ProductContent>> SelectProductContents(ICollection<int> ids)
        {
            return (await _productContentRepository.Query(p => ids.Contains(p.Id)).Include(p => p.ContentItem).SelectAsync(false)).ToList();
        }

        protected override IQueryLite<Product> BuildQuery(IQueryLite<Product> query)
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
                entity.Skus = entity.Skus?.Where(s => s.StatusCode != (int) RecordStatusCode.Deleted).OrderBy(s => s.Order).ToArray();
            }
            return Task.Delay(0);
        }

        protected override async Task AfterEntityChangesAsync(ProductDynamic model, Product updated, Product initial, IUnitOfWorkAsync uow)
        {
            await SyncDbCollections<Sku, SkuOptionValue>(uow, initial.Skus, updated.Skus, false);
        }

        protected override async Task<List<MessageInfo>> ValidateAsync(ProductDynamic model)
        {
            List<MessageInfo> errors = new List<MessageInfo>();

            var productSameName =
                await
                    _productRepository.Query(
                        new ProductQuery().NotDeleted().Excluding(model.Id).WithName(model.Name))
                        .SelectFirstOrDefaultAsync(false);

            if (productSameName != null)
            {
                errors.AddRange(
                    model.CreateError()
                        .Property(p => p.Name)
                        .Error("Product name should be unique in the database")
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

        public ProductService(VProductSkuRepository vProductSkuRepository,
            IEcommerceRepositoryAsync<VSku> vSkuRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Product> productRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository, ProductMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<ProductToCategory> productToCategoriesRepository,
            IEcommerceRepositoryAsync<ProductOptionValue> productValueRepositoryAsync,
            IEcommerceRepositoryAsync<SkuOptionValue> skuOptionValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            OrderSkusRepository orderSkusRepositoryRepository,
            SkuMapper skuMapper,
            IEcommerceRepositoryAsync<ProductOutOfStockRequest> productOutOfStockRequestRepository,
            ISettingService settingService,
            INotificationService notificationService,
            IRepositoryAsync<ProductContent> productContentRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            IOptions<AppOptions> options,
            ILoggerProviderExtended loggerProvider, IEcommerceRepositoryAsync<VCustomerFavorite> vCustomerRepositoryAsync,
            DirectMapper<Product> directMapper, DynamicExtensionsRewriter queryVisitor, ITransactionAccessor<EcommerceContext> transactionAccessor)
            : base(
                mapper, productRepository, productValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, directMapper, queryVisitor, transactionAccessor)
        {
            _vProductSkuRepository = vProductSkuRepository;
            _vSkuRepository = vSkuRepository;
            _productRepository = productRepository;
            _skuRepository = skuRepository;
            _mapper = mapper;
            _adminProfileRepository = adminProfileRepository;
            _orderSkusRepositoryRepository = orderSkusRepositoryRepository;
            _skuMapper = skuMapper;
            _productOutOfStockRequestRepository = productOutOfStockRequestRepository;
            _settingService = settingService;
            _notificationService = notificationService;
            _productContentRepository = productContentRepository;
            _contentTypeRepository = contentTypeRepository;
            _vCustomerFavoriteRepository = vCustomerRepositoryAsync;
            _options = options;
            _skuOptionValueRepositoryAsync = skuOptionValueRepositoryAsync;
        }

        #region ProductOptions

        public List<ProductOptionType> GetProductOptionTypes(HashSet<string> names)
        {
            return _mapper.OptionTypes.Where(o => names.Contains(o.Name)).ToList();
        }

        public Dictionary<int, Dictionary<string, string>> GetProductEditDefaultSettingsAsync()
        {
            Dictionary<int, Dictionary<string, string>> toReturn = new Dictionary<int, Dictionary<string, string>>();
            HashSet<string> names = new HashSet<string>();
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
            var items = GetProductOptionTypes(names);
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

        public List<ProductOptionType> GetProductLookupsAsync()
        {
            return _mapper.OptionTypes.ToList();
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
			var skuOrdered = new SkuOrdered
			{
				Sku = skuDynamic
			};

			var productUrl = _productContentRepository.Query(p => p.Id == sku.IdProduct).Select(p => p.Url, false).FirstOrDefault();
			skuOrdered.Sku.Product.Url = productUrl;

			return skuOrdered;
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

			var res = new List<SkuOrdered>();
	        foreach (var temp in skus)
	        {
				var skuOrdered = await PopulateSkuOrderedWithUrlAsync(temp);
				res.Add(skuOrdered);
			}

	        return res;
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

			var res = new List<SkuOrdered>();
			foreach (var temp in skus)
			{
				var skuOrdered = await PopulateSkuOrderedWithUrlAsync(temp);
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

        public async Task<SkuDynamic> GetSkuAsync(int id, bool withDefaults = false)
        {
            var sku =
                await _skuRepository.Query(s => s.Id == id)
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .SelectFirstOrDefaultAsync(false);
            return await _skuMapper.FromEntityAsync(sku, withDefaults);
        }

        public async Task<ICollection<VSku>> GetSkusAsync(VProductSkuFilter filter)
        {
            var conditions = new VSkuQuery().NotDeleted()
                .WithText(filter.SearchText)
                .WithCode(filter.Code)
                .WithDescriptionName(filter.DescriptionName)
                .WithExactCode(filter.ExactCode)
                .WithExactCodes(filter.ExactCodes)
                .WithExactDescriptionName(filter.ExactDescriptionName)
                .WithIds(filter.Ids)
                .WithIdProducts(filter.IdProducts)
                .ActiveOnly(filter.ActiveOnly).NotHiddenOnly(filter.NotHiddenOnly);
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

            var skus =
                await _skuRepository.Query(new SkuQuery().ByIds(skuInfos.Select(i => i.Id).ToList()))
                    .Include(s => s.OptionValues)
                    .SelectAsync(false);
            var skusKeyed = skus.ToDictionary(s => s.Id, s => s);
            foreach (var info in skuInfos)
            {
                var sku = skusKeyed[info.Id];
                sku.OptionTypes =
                    _mapper.FilterByType((int?)info.IdProductType);
            }
            return await _skuMapper.FromEntityRangeAsync(skus, withDefaults);
        }

        public async Task<List<SkuDynamic>> GetSkusAsync(ICollection<string> codes, bool withDefaults = false)
        {
            if (codes == null)
                throw new ArgumentNullException(nameof(codes));

            if (!codes.Any())
                return new List<SkuDynamic>();
            var skus =
                await _skuRepository.Query(new SkuQuery().Including(codes).NotDeleted())
                    .Include(s => s.OptionValues)
                    .Include(s => s.Product)
                    .SelectAsync(false);
            foreach (var sku in skus)
            {
                sku.OptionTypes =
                     _mapper.FilterByType(sku.Product.IdObjectType);
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
                var ids = toReturn.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy.Value).Distinct().ToList();
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
                         _mapper.FilterByType(product.IdObjectType);
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

        public async Task<bool> SendProductOutOfStockRequestsAsync(ICollection<int> ids,string messageFormat=null)
        {
            if (ids.Count > 0)
            {
                if (messageFormat == null)
                {
                    var setting = (await _settingService.GetAppSettingItemsAsync(new List<string>() { SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE })).FirstOrDefault();
                    if (setting == null)
                    {
                        throw new NotSupportedException($"{SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE} not configurated.");
                    }
                    messageFormat = setting.Value;
                }

                var items = await _productOutOfStockRequestRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false);
                var productIds = items.Select(p => p.IdProduct).Distinct().ToArray();
                var products = await _productRepository.Query(p => productIds.Contains(p.Id)).SelectAsync(false);
                var contentProducts= await _productContentRepository.Query(p => productIds.Contains(p.Id)).SelectAsync(false);

                foreach (var item in items)
                {
                    var product = products.FirstOrDefault(p => p.Id == item.IdProduct);
                    var contentProduct= contentProducts.FirstOrDefault(p => p.Id == item.IdProduct);
                    if (product != null && contentProduct!=null)
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
			var pairs = new List<KeyValuePair<int,string>>();
            while (batch.Any()) { 
				pairs.AddRange(_productContentRepository.Query(p => batch.Contains(p.Id)).Select(x=> new KeyValuePair<int, string>(x.Id, x.Url)));

	            i++;
	            batch = productIds.Skip(i* BaseAppConstants.DEFAULT_MAX_ALLOWED_PARAMS_SQL).Take(BaseAppConstants.DEFAULT_MAX_ALLOWED_PARAMS_SQL).ToList();
            }

			var favorites = new PagedList<VCustomerFavoriteFull>();
			foreach (var item in temp.Items)
			{
				favorites.Items.Add(new VCustomerFavoriteFull
				{
					Id = item.Id,
					IdCustomer = item.Id,
					ProductName = item.ProductName,
                    ProductSubTitle = item.ProductSubTitle,
                    ProductThumbnail = item.ProductThumbnail,
					Quantity = item.Quantity,
					Url = pairs.Single(x=>x.Key == item.Id).Value
				});
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
                await LogItemChanges(new[] { await DynamicMapper.FromEntityAsync(entity) });
                return await DynamicMapper.FromEntityAsync(entity);
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
                await LogItemChanges(new[] { await DynamicMapper.FromEntityAsync(entity) });
                return await DynamicMapper.FromEntityAsync(entity);
            }
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
            if(productContent==null)
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
                                .SelectAsync()).FirstOrDefault();
                        if (dbProductContent != null)
                        {
                            await Validate(productContent);

                            dbProductContent.Url = productContent.Url;
                            dbProductContent.StatusCode = (RecordStatusCode) model.StatusCode;
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

        private async Task UpdateContentForInsert(ProductDynamic model, ProductContent productContent)
        {
            productContent.StatusCode = (RecordStatusCode) model.StatusCode;
            productContent.UserId = model.IdEditedBy;
            productContent.ContentItem.Created = DateTime.Now;
            productContent.ContentItem.Updated = productContent.ContentItem.Created;
            if (productContent.MasterContentItemId == 0)
            {
                //set predefined master
                var contentType = (await _contentTypeRepository.Query(p => p.Id == (int) ContentType.Product).SelectAsync(false)).FirstOrDefault();
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


            if (productSameUrl.Any())
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
            if(result)
            {
                var productContent = (await _productContentRepository.Query(p=>p.Id==id).SelectAsync()).FirstOrDefault();
                if(productContent!=null && productContent.StatusCode!=RecordStatusCode.Deleted)
                {
                    productContent.StatusCode = RecordStatusCode.Deleted;
                    result = await _productContentRepository.UpdateAsync(productContent);
                }
            }
            return result;
        }

        public async Task<ProductContentTransferEntity> SelectTransferAsync(int id, bool withDefaults = false)
        {
	        var toReturn = new ProductContentTransferEntity {ProductDynamic = await this.SelectAsync(id, withDefaults)};
	        if(toReturn.ProductDynamic!=null)
            {
                toReturn.ProductContent = await SelectContentForTransfer(id);
            }
            return toReturn;
        }

		public async Task<ProductContentTransferEntity> SelectTransferAsync(Guid id, bool withDefaults = false)
		{
			var toReturn = new ProductContentTransferEntity
			{
				ProductDynamic = await DynamicMapper.FromEntityAsync(
					await
						SelectEntityFirstAsync(o => o.PublicId == id && o.StatusCode != (int) RecordStatusCode.Deleted), withDefaults)
			};
			if (toReturn.ProductDynamic != null)
			{
				toReturn.ProductContent = await SelectContentForTransfer(toReturn.ProductDynamic.Id);
			}
			return toReturn;
		}

		public async Task<ProductContentTransferEntity> SelectTransferAsync(string url, bool withDefaults = false)
		{
			var productContent = (await _productContentRepository.Query(p => p.Url.Equals(url)).Include(p => p.ContentItem).SelectAsync(false)).FirstOrDefault();

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

		    return (await _productRepository.Query(productQuery).SelectAsync(x => x.Id)).FirstOrDefault();
	    }

	    #endregion
	}
}