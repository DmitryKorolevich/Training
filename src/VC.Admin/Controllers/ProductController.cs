using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VC.Admin.Models;
using VitalChoice.Validation.Models;
using System;
using System.IO;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Interfaces.Services.Products;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using Newtonsoft.Json;
using VC.Admin.Models.Products;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using VitalChoice.Business.CsvExportMaps;
using Microsoft.Net.Http.Headers;
using VitalChoice.Business.CsvExportMaps.Products;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.InventorySkus;
using VitalChoice.Business.Helpers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Services;
using VitalChoice.Infrastructure.Domain;

namespace VC.Admin.Controllers
{
    public class ProductController : BaseApiController
    {
        private readonly IProductCategoryService productCategoryService;
        private readonly IProductService productService;
        private readonly IDynamicServiceAsync<ProductDynamic, Product> productUniversalService;
        private readonly IInventoryCategoryService inventoryCategoryService;
        private readonly IProductReviewService productReviewService;
        private readonly ISettingService settingService;
        private readonly IDynamicMapper<ProductDynamic, Product> _mapper;
        private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
        private readonly AppSettings _appSettings;
        private readonly ICsvExportService<ProductCategoryStatisticTreeItemModel, ProductCategoryStatisticTreeItemCsvMap> productCategoryStatisticTreeItemCSVExportService;
        private readonly ICsvExportService<SkuBreakDownReportItem, SkuBreakDownReportItemCsvMap> _skuBreakDownReportItemCSVExportService;
        private readonly IObjectHistoryLogService objectHistoryLogService;
        private readonly ILogger logger;
        private readonly ExtendedUserManager _userManager;
        private readonly IAgentService _agentService;

        public ProductController(IProductCategoryService productCategoryService,
            IProductService productService,
            IDynamicServiceAsync<ProductDynamic, Product> productUniversalService,
            IInventoryCategoryService inventoryCategoryService,
            IProductReviewService productReviewService,
            ILoggerFactory loggerProvider,
            ISettingService settingService,
            IDynamicMapper<ProductDynamic, Product> mapper,
            ICsvExportService<ProductCategoryStatisticTreeItemModel, ProductCategoryStatisticTreeItemCsvMap> productCategoryStatisticTreeItemCSVExportService,
            ICsvExportService<SkuBreakDownReportItem, SkuBreakDownReportItemCsvMap> skuBreakDownReportItemCSVExportService,
            IObjectHistoryLogService objectHistoryLogService, ExtendedUserManager userManager, IAgentService agentService, IDynamicMapper<SkuDynamic, Sku> skuMapper, AppSettings appSettings)
        {
            this.productCategoryService = productCategoryService;
            this.inventoryCategoryService = inventoryCategoryService;
            this.productService = productService;
            this.productUniversalService = productUniversalService;
            this.productReviewService = productReviewService;
            this.settingService = settingService;
            this.productCategoryStatisticTreeItemCSVExportService = productCategoryStatisticTreeItemCSVExportService;
            _skuBreakDownReportItemCSVExportService = skuBreakDownReportItemCSVExportService;
            this.objectHistoryLogService = objectHistoryLogService;
            _userManager = userManager;
            _agentService = agentService;
            _skuMapper = skuMapper;
            _appSettings = appSettings;
            _mapper = mapper;
            this.logger = loggerProvider.CreateLogger<ProductController>();
        }

        #region Products

        [HttpGet]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductEditSettingsModel>> GetProductEditSettings()
        {
            var lookups = productService.GetExpandedOptionTypesWithSkuTypes().Select(
                        p => new LookupViewModel(p.Name, p.IdObjectType, p.DefaultValue, p.Lookup)).ToList();
            var inventoryChannelLookup = (await settingService.GetLookupsAsync(new[] { SettingConstants.INVENTORY_SKU_LOOKUP_CHANNEL_NAME })).FirstOrDefault();
            if (inventoryChannelLookup != null)
            {
                lookups.Add(new LookupViewModel(inventoryChannelLookup.Name, null, null, inventoryChannelLookup));
            }
            var defaultValues = productService.GetProductEditDefaultSettingsAsync();
            ProductEditSettingsModel toReturn = new ProductEditSettingsModel()
            {
                Lookups = lookups,
                DefaultValues = defaultValues
            };
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<SkuListItemModel>> GetSku([FromBody] VProductSkuFilter filter)
        {
            var result = await productService.GetSkusAsync(filter);

            return
                await
                    (result.Select(async p => await _skuMapper.ToModelAsync<SkuListItemModel>(p)).FirstOrDefault() ??
                     TaskCache<SkuListItemModel>.DefaultCompletedTask);
        }

        [HttpPost]
        public async Task<Result<ICollection<SkuListItemModel>>> GetSkus([FromBody] VProductSkuFilter filter)
        {
            var result = await productService.GetSkusAsync(filter);
            return await result.Select(p => _skuMapper.ToModelAsync<SkuListItemModel>(p)).ToListAsync();
        }

        [HttpGet]
        public async Task<Result<ICollection<SkuWithStatisticListItemModel>>> GetTopPurchasedSkus(int id)
        {
            ICollection<SkuWithStatisticListItemModel> toReturn = new List<SkuWithStatisticListItemModel>();

            FilterBase idsFilter = new FilterBase {Paging = null};
            Dictionary<int, int> items = await productService.GetTopPurchasedSkuIdsAsync(idsFilter, id);

            VProductSkuFilter filter = new VProductSkuFilter
            {
                Ids = items.Select(p => p.Key).ToList(),
                Paging = null
            };

            var result = await productService.GetSkusAsync(filter);
            //only in stock
            var inStockItems = result.Where(dynamic => (ProductType) dynamic.Product.IdObjectType == ProductType.EGс ||
                                                       (ProductType) dynamic.Product.IdObjectType == ProductType.Gc ||
                                                       ((bool?) dynamic.SafeData.DisregardStock ?? false) ||
                                                       ((int?) dynamic.SafeData.Stock ?? 0) > 0)
                .Select(p => _skuMapper.ToModelAsync<SkuWithStatisticListItemModel>(p));
            foreach (var skuTask in inStockItems)
            {
                var sku = await skuTask;
                int ordered;
                // ReSharper disable once PossibleInvalidOperationException
                // ReSharper disable once AssignNullToNotNullAttribute
                if (items.TryGetValue(sku.Id.Value, out ordered))
                {
                    sku.Ordered = ordered;
                    toReturn.Add(sku);
                }
            }
            return toReturn.OrderByDescending(p => p.Ordered).Take(20).ToList();
        }

        [HttpGet]
        public async Task<Result<ICollection<ProductCategoryOrderModel>>> GetProductsOnCategoryOrder(int id)
        {
            return (await productService.GetProductsOnCategoryOrderAsync(id)).ToList();
        }

        [HttpPost]
        public async Task<Result<bool>> UpdateProductsOnCategoryOrder(int id, [FromBody]ICollection<ProductCategoryOrderModel> products)
        {
            return await productService.UpdateProductsOnCategoryOrderAsync(id, products);
        }

        [HttpPost]
        public async Task<Result<PagedList<ProductListItemModel>>> GetProducts([FromBody] VProductSkuFilter filter)
        {
            var result = await productService.GetProductsAsync2(filter);

            var agents =
                await
                    _agentService.GetAgents(
                        result.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy.Value).Distinct().ToList());

            var toReturn = new PagedList<ProductListItemModel>
            {
                Items = await result.Items.Select(async p =>
                {
                    var productItem = await _mapper.ToModelAsync<ProductListItemModel>(p);
                    if (p.IdEditedBy != null)
                    {
                        productItem.EditedByAgentId = agents.GetValueOrDefault(p.IdEditedBy.Value);
                    }
                    return productItem;
                }).ToListAsync(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductManageModel>> GetProduct(string id)
        {
            int idProduct = 0;
            if (id != null && !Int32.TryParse(id, out idProduct))
                throw new NotFoundException();

            if (idProduct == 0)
            {
                return new ProductManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                    IdVisibility = CustomerTypeCode.All,
                    CategoryIds = new List<int>(),
                    SKUs = new List<SKUManageModel>(),
                    CrossSellProducts = new List<CrossSellProductModel>()
                    {
                        new CrossSellProductModel() { IsDefault=true},
                        new CrossSellProductModel() { IsDefault=true},
                        new CrossSellProductModel() { IsDefault=true},
                        new CrossSellProductModel() { IsDefault=true},
                    },
                    Videos = new List<VideoModel>()
                    {
                        new VideoModel() { IsDefault=true},
                        new VideoModel() { IsDefault=true},
                        new VideoModel() { IsDefault=true},
                        new VideoModel() { IsDefault=true}
                    },
                };
            }

            var item = await productService.SelectTransferAsync(idProduct);
            if(item==null || item.ProductDynamic==null)
                throw new NotFoundException();

            ProductManageModel toReturn = await _mapper.ToModelAsync<ProductManageModel>(item?.ProductDynamic);
            if (item.ProductContent != null)
            {
                toReturn.Url = item.ProductContent.Url;
                toReturn.MasterContentItemId = item.ProductContent.MasterContentItemId;
                toReturn.Template = item.ProductContent.ContentItem.Template;
                toReturn.MetaTitle = item.ProductContent.ContentItem.Title;
                toReturn.MetaDescription = item.ProductContent.ContentItem.MetaDescription;
            }
            if (toReturn.CrossSellProducts != null)
            {
                foreach (var product in toReturn.CrossSellProducts)
                {
                    product.IsDefault = String.IsNullOrEmpty(product.Image) && String.IsNullOrEmpty(product.Url);
                }
            }
            if (toReturn.Videos != null)
            {
                foreach (var video in toReturn.Videos)
                {
                    video.IsDefault = String.IsNullOrEmpty(video.Image) && String.IsNullOrEmpty(video.Video) && String.IsNullOrEmpty(video.Text);
                }
            }
            return toReturn;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductManageModel>> UpdateProduct([FromBody]ProductManageModel model)
        {
            if (!Validate(model))
                return null;

            var product = await _mapper.FromModelAsync(model);
            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                product.IdEditedBy = userId;
            }

            ProductContentTransferEntity transferEntity = new ProductContentTransferEntity();
            ProductContent content = new ProductContent();
            content.Id = model.Id;
            content.Url = model.Url;
            content.ContentItem = new ContentItem();
            content.ContentItem.Template = model.Template ?? string.Empty;
            content.ContentItem.Description = model.Description ?? string.Empty;
            content.ContentItem.Title = model.MetaTitle;
            content.ContentItem.MetaDescription = model.MetaDescription;
            content.MasterContentItemId = model.MasterContentItemId;
            transferEntity.ProductContent = content;
            transferEntity.ProductDynamic = product;

            if (model.Id > 0)
                product = (await productService.UpdateAsync(transferEntity));
            else
            {
                transferEntity.ProductDynamic.PublicId = Guid.NewGuid();
                product = (await productService.InsertAsync(transferEntity));
            }

            ProductManageModel toReturn = await _mapper.ToModelAsync<ProductManageModel>(product);
            toReturn.MasterContentItemId = transferEntity.ProductContent.MasterContentItemId;

            return toReturn;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Settings)]
        public async Task<Result<bool>> UpdateProductTaxCodes([FromBody]ICollection<ProductListItemModel> items)
        {
            var products = await productUniversalService.SelectAsync(items.Select(p => p.ProductId).ToArray(), false);
            foreach (var product in products)
            {
                foreach (var item in items)
                {
                    if (product.Id == item.ProductId)
                    {
                        product.Data.TaxCode = item.TaxCode;
                    }
                }
            }
            var toReturn = await productUniversalService.UpdateRangeAsync(products);

            return true;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> DeleteProduct(int id)
        {
            return await productService.DeleteAsync(id);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<PagedList<SkuPricesManageItemModel>>> GetSkusPrices([FromBody] FilterBase filter)
        {
            return await productService.GetSkusPricesAsync(filter);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<bool> UpdateSkusPrices([FromBody] ICollection<SkuPricesManageItemModel> items)
        {
            return await productService.UpdateSkusPricesAsync(items);
        }

        #endregion

        #region ProductCategories

        [HttpPost]
        public async Task<Result<ProductCategoryTreeItemModel>> GetCategoriesTree(
            [FromBody] ProductCategoryTreeFilter filter)
        {
            var productCategory = await productCategoryService.GetCategoriesTreeAsync(filter);
            var contentCategories =
                await productCategoryService.GetLiteCategoriesTreeAsync(productCategory, new ProductCategoryLiteFilter
                {
                    ShowAll = true,
                    Statuses = filter.Statuses,
                    Paging = filter.Paging,
                    SearchText = filter.SearchText,
                    Sorting = filter.Sorting
                });

            return new ProductCategoryTreeItemModel(contentCategories);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> UpdateCategoriesTree([FromBody]ProductCategoryTreeItemModel model)
        {
            if (!Validate(model))
                return false;
            var category = model.Convert();

            return await productCategoryService.UpdateCategoriesTreeAsync(category);
        } 

        [HttpGet]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductCategoryManageModel>> GetCategory(string id)
        {
            int idCategory = 0;
            if (id != null && !Int32.TryParse(id, out idCategory))
                throw new NotFoundException();

            if (idCategory == 0)
            {
                return new ProductCategoryManageModel()
                {
                    Template = String.Empty,
                    StatusCode = RecordStatusCode.Active,
                    Assigned = CustomerTypeCode.All,
                    NavIdVisible = CustomerTypeCode.All,
                    ViewType = ProductCategoryViewType.Retail,
                };
            }
            var item = await productCategoryService.GetCategoryAsync(idCategory);
            if (item == null)
                throw new NotFoundException();

            return new ProductCategoryManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductCategoryManageModel>> UpdateCategory([FromBody]ProductCategoryManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.UserId = userId;
            }
            item = await productCategoryService.UpdateCategoryAsync(item);

            return new ProductCategoryManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> DeleteCategory(int id)
        {
            return await productCategoryService.DeleteCategoryAsync(id);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Reports)]
        public async Task<Result<ProductCategoryStatisticTreeItemModel>> GetProductCategoriesStatistic([FromBody]ProductCategoryStatisticFilter filter)
        {
            return await productCategoryService.GetProductCategoriesStatisticAsync(filter);
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Reports)]
        public async Task<FileResult> GetProductCategoriesStatisticReportFile([FromQuery]DateTime from, [FromQuery]DateTime to)
        {
            var rootItem = await productCategoryService.GetProductCategoriesStatisticAsync(
                new ProductCategoryStatisticFilter()
                {
                    From = from,
                    To = to
                });

            List<ProductCategoryStatisticTreeItemModel> items = new List<ProductCategoryStatisticTreeItemModel>();
            TransformProductCategoryTreeToList(rootItem, items, 1);
            items.Remove(rootItem);

            var data = productCategoryStatisticTreeItemCSVExportService.ExportToCsv(items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.PRODUCT_CATEGORY_STATISTIC, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }

        private void TransformProductCategoryTreeToList(ProductCategoryStatisticTreeItemModel item, ICollection<ProductCategoryStatisticTreeItemModel> list, int level)
        {
            var prefix = String.Empty;
            int current = 0;
            while (current < level)
            {
                prefix += "----";
                current++;
            }
            item.Name = prefix + item.Name;
            item.Percent = item.Percent / 100;
            list.Add(item);
            if (item.SubItems != null)
            {
                foreach (var subItem in item.SubItems)
                {
                    TransformProductCategoryTreeToList(subItem, list, ++level);
                }
            }
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ICollection<SkusInProductCategoryStatisticItem>>> GetSkusInProductCategoryStatistic([FromBody]ProductCategoryStatisticFilter filter)
        {
            return (await productCategoryService.GetSkusInProductCategoryStatisticAsync(filter)).ToList();
        }

        #endregion

        #region InventoryCategories

        [HttpPost]
        public async Task<Result<IList<InventoryCategoryTreeItemModel>>> GetInventoryCategoriesTree([FromBody]InventoryCategoryTreeFilter filter)
        {
            var result = await inventoryCategoryService.GetCategoriesTreeAsync(filter);

            return result.Select(p => new InventoryCategoryTreeItemModel(p)).ToList();
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> UpdateInventoryCategoriesTree([FromBody]IList<InventoryCategoryTreeItemModel> model)
        {
            IList<InventoryCategory> categories = new List<InventoryCategory>();
            if (model != null)
            {
                foreach (var modelCategory in model)
                {
                    categories.Add(modelCategory.Convert());
                }
            }

            return await inventoryCategoryService.UpdateCategoriesTreeAsync(categories);
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<InventoryCategoryManageModel>> GetInventoryCategory(string id)
        {
            int idCategory = 0;
            if (id != null && !Int32.TryParse(id, out idCategory))
                throw new NotFoundException();

            if (idCategory == 0)
            {
                return new InventoryCategoryManageModel()
                {
                };
            }
            var item = await inventoryCategoryService.GetCategoryAsync(idCategory);
            if (item == null)
                throw new NotFoundException();

            return new InventoryCategoryManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<InventoryCategoryManageModel>> UpdateInventoryCategory([FromBody]InventoryCategoryManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await inventoryCategoryService.UpdateCategoryAsync(item);

            return new InventoryCategoryManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> DeleteInventoryCategory(int id)
        {
            return await inventoryCategoryService.DeleteCategoryAsync(id);
        }

        #endregion

        #region ProductReviews

        [HttpPost]
        public async Task<Result<PagedList<VProductsWithReviewListItemModel>>> GetProductsWithReviews([FromBody]VProductsWithReviewFilter filter)
        {
            var result = await productReviewService.GetVProductsWithReviewsAsync(filter);

            var toReturn = new PagedList<VProductsWithReviewListItemModel>
            {
                Items = result.Items.Select(p => new VProductsWithReviewListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<PagedList<ProductReviewListItemModel>>> GetProductReviews([FromBody]ProductReviewFilter filter)
        {
            var result = await productReviewService.GetProductReviewsAsync(filter);

            var toReturn = new PagedList<ProductReviewListItemModel>
            {
                Items = result.Items.Select(p => new ProductReviewListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductReviewManageModel>> GetProductReview(string id)
        {
            int idReview = 0;
            if (id != null && !Int32.TryParse(id, out idReview))
                throw new NotFoundException();

            if (idReview == 0)
            {
                return new ProductReviewManageModel()
                {
                    StatusCode = RecordStatusCode.NotActive,
                    Rating = 5,
                };
            }
            var item = await productReviewService.GetProductReviewAsync(idReview);
            if (item == null)
                throw new NotFoundException();

            return new ProductReviewManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductReviewManageModel>> UpdateProductReview([FromBody]ProductReviewManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await productReviewService.UpdateProductReviewAsync(item);

            return new ProductReviewManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> DeleteProductReview(int id)
        {
            return await productReviewService.DeleteProductReviewAsync(id);
        }

        #endregion

        #region ProductOutOfStockRequests

        [HttpPost]
        public async Task<Result<ICollection<ProductOutOfStockContainerListItemModel>>> GetProductOutOfStockContainers()
        {
            var toReturn = await productService.GetProductOutOfStockContainersAsync();
            return toReturn.Select(p => new ProductOutOfStockContainerListItemModel(p)).ToList();
        }

        [HttpGet]
        public Result<string> GetProductOutOfStockRequestsMessageFormat()
        {
            if (_appSettings.ProductOutOfStockEmailTemplate == null)
            {
                throw new NotSupportedException($"{SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE} not configurated.");
            }
            return _appSettings.ProductOutOfStockEmailTemplate;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> SendProductOutOfStockRequests([FromBody]SendOutOfStockProductRequestsModel model)
        {
            return await productService.SendProductOutOfStockRequestsAsync(model.Ids, model.MessageFormat);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> DeleteProductOutOfStockRequests([FromBody]ICollection<int> ids)
        {
            return await productService.DeleteProductOutOfStockRequestsAsync(ids);
        }

        #endregion

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (ProductDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(ProductDynamic));
                var model = await _mapper.ToModelAsync<ProductManageModel>(dynamic);
                if (string.IsNullOrEmpty(model.Url))
                {
                    model.Url = dynamic.SafeData.Url;
                }
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (ProductDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(ProductDynamic));
                var model = await _mapper.ToModelAsync<ProductManageModel>(dynamic);
                if (string.IsNullOrEmpty(model.Url))
                {
                    model.Url = dynamic.SafeData.Url;
                }
                toReturn.Before.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }

            return toReturn;
        }

        #region Reports

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<ICollection<SkuBreakDownReportItem>>> GetSkuBreakDownReportItems([FromBody]SkuBreakDownReportFilter filter)
        {
            var toReturn = await productService.GetSkuBreakDownReportItemsAsync(filter);
            return toReturn.ToList();
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<FileResult> GetSkuBreakDownReportItemsReportFile([FromQuery]string from, [FromQuery]string to)
        {
            var dFrom = from.GetDateFromQueryStringInPst(TimeZoneHelper.PstTimeZoneInfo);
            var dTo = to.GetDateFromQueryStringInPst(TimeZoneHelper.PstTimeZoneInfo);
            if (!dFrom.HasValue || !dTo.HasValue)
            {
                return null;
            }

            SkuBreakDownReportFilter filter = new SkuBreakDownReportFilter()
            {
                From = dFrom.Value,
                To = dTo.Value.AddDays(1),
            };
            filter.Paging = null;

            var data = await productService.GetSkuBreakDownReportItemsAsync(filter);

            var result = _skuBreakDownReportItemCSVExportService.ExportToCsv(data);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.SKU_BREAKDOWN_REPORT, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(result, "text/csv");
        }

        [HttpPost]
        public async Task<Result<SkuPOrderTypeBreakDownReport>> GetSkuPOrderTypeBreakDownReport([FromBody]SkuPOrderTypeBreakDownReportFilter filter)
        {
            filter.To = filter.To.AddDays(1);
            var toReturn = await productService.GetSkuPOrderTypeBreakDownReportAsync(filter);
            //correct dates for UI
            if (toReturn.FrequencyType == FrequencyType.Weekly || toReturn.FrequencyType == FrequencyType.Monthly)
            {
                for (int i = 0; i < toReturn.POrderTypePeriods.Count; i++)
                {
                    toReturn.POrderTypePeriods[i].To = toReturn.POrderTypePeriods[i].To.AddDays(-1);
                }

                foreach (var sku in toReturn.Skus)
                {
                    for (int i = 0; i < sku.Periods.Count; i++)
                    {
                        sku.Periods[i].To = sku.Periods[i].To.AddDays(-1);
                    }
                }
            }
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<SkuPOrderTypeBreakDownReport>> GetSkuPOrderTypeFutureBreakDownReport([FromBody]SkuPOrderTypeBreakDownReportFilter filter)
        {
            filter.To = filter.To.AddDays(1);
            var toReturn = await productService.GetSkuPOrderTypeFutureBreakDownReportAsync(filter);
            //correct dates for UI
            if (toReturn.FrequencyType == FrequencyType.Weekly || toReturn.FrequencyType == FrequencyType.Monthly)
            {
                for (int i = 0; i < toReturn.POrderTypePeriods.Count; i++)
                {
                    toReturn.POrderTypePeriods[i].To = toReturn.POrderTypePeriods[i].To.AddDays(-1);
                }

                foreach (var sku in toReturn.Skus)
                {
                    for (int i = 0; i < sku.Periods.Count; i++)
                    {
                        sku.Periods[i].To = sku.Periods[i].To.AddDays(-1);
                    }
                }
            }
            return toReturn;
        }

        #endregion
    }
}