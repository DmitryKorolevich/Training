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
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Products;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.InventorySkus;

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
        private readonly ICsvExportService<ProductCategoryStatisticTreeItemModel, ProductCategoryStatisticTreeItemCsvMap> productCategoryStatisticTreeItemCSVExportService;
        private readonly IObjectHistoryLogService objectHistoryLogService;
        private readonly ILogger logger;
        private readonly ExtendedUserManager _userManager;

        public ProductController(IProductCategoryService productCategoryService,
            IProductService productService,
            IDynamicServiceAsync<ProductDynamic, Product> productUniversalService,
            IInventoryCategoryService inventoryCategoryService, 
            IProductReviewService productReviewService,
            ILoggerProviderExtended loggerProvider,
            ISettingService settingService,
            IDynamicMapper<ProductDynamic, Product> mapper,
            ICsvExportService<ProductCategoryStatisticTreeItemModel, ProductCategoryStatisticTreeItemCsvMap> productCategoryStatisticTreeItemCSVExportService,
            IObjectHistoryLogService objectHistoryLogService, ExtendedUserManager userManager)
        {
            this.productCategoryService = productCategoryService;
            this.inventoryCategoryService = inventoryCategoryService;
            this.productService = productService;
            this.productUniversalService = productUniversalService;
            this.productReviewService = productReviewService;
            this.settingService = settingService;
            this.productCategoryStatisticTreeItemCSVExportService = productCategoryStatisticTreeItemCSVExportService;
            this.objectHistoryLogService = objectHistoryLogService;
            _userManager = userManager;
            _mapper = mapper;
            this.logger = loggerProvider.CreateLogger<ProductController>();
        }

        #region Products

        [HttpGet]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductEditSettingsModel>> GetProductEditSettings()
        {
            var lookups = productService.GetProductLookupsAsync().Select(
                        p => new LookupViewModel(p.Name, p.IdObjectType, p.DefaultValue, p.Lookup)).ToList();
            var inventoryChannelLookup = (await settingService.GetLookupsAsync(new [] { SettingConstants.INVENTORY_SKU_LOOKUP_CHANNEL_NAME })).FirstOrDefault();
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
        public async Task<Result<SkuListItemModel>> GetSku([FromBody]VProductSkuFilter filter)
        {
            var result = await productService.GetSkusAsync(filter);

            return result.Select(p => new SkuListItemModel(p)).FirstOrDefault();
        }

        [HttpPost]
        public async Task<Result<ICollection<SkuListItemModel>>> GetSkus([FromBody]VProductSkuFilter filter)
        {
            var result = await productService.GetSkusAsync(filter);

            return result.Select(p => new SkuListItemModel(p)).ToArray();
        }

        [HttpGet]
        public async Task<Result<ICollection<SkuWithStatisticListItemModel>>> GetTopPurchasedSkus(int id)
        {
            ICollection<SkuWithStatisticListItemModel> toReturn = new List<SkuWithStatisticListItemModel>();

            FilterBase idsFilter = new FilterBase();
            idsFilter.Paging = null;
            Dictionary<int, int> items = await productService.GetTopPurchasedSkuIdsAsync(idsFilter, id);

            VProductSkuFilter filter = new VProductSkuFilter();
            filter.Ids = items.Select(p => p.Key).ToList();
            filter.Paging = null;

            var result = await productService.GetSkusAsync(filter);
            //only in stock
            result = result.Where(p => p.DisregardStock || p.Stock > 0).ToList();
            foreach (var sku in result)
            {
                foreach (var item in items)
                {
                    if(sku.SkuId==item.Key)
                    {
                        toReturn.Add(new SkuWithStatisticListItemModel(sku,item.Value));
                        break;
                    }
                }
            }
            
            return toReturn.OrderByDescending(p => p.Ordered).Take(20).ToList();
        }

        [HttpPost]
        public async Task<Result<PagedList<ProductListItemModel>>> GetProducts([FromBody]VProductSkuFilter filter)
        {
            var result = await productService.GetProductsAsync(filter);

            var toReturn = new PagedList<ProductListItemModel>
            {
                Items = result.Items.Select(p => new ProductListItemModel(p)).ToList(),
                Count = result.Count,
            };

            VProductSkuFilter skuFilter = new VProductSkuFilter();
            skuFilter.IdProducts = toReturn.Items.Select(p => p.ProductId).ToList();
            skuFilter.Paging = null;
            var skus = await productService.GetSkusAsync(skuFilter);
            foreach (var item in toReturn.Items)
            {
                item.SKUs = skus.Where(p => p.IdProduct == item.ProductId)
                            .OrderBy(p => p.Order)
                            .Select(p => new SkuListItemModel(p))
                            .ToList();  new List<SkuListItemModel>();
            }

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductManageModel>> GetProduct(int id)
        {
            if (id == 0)
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

            var item = await productService.SelectTransferAsync(id);
            
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
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> UpdateProductTaxCodes([FromBody]ICollection<ProductListItemModel> items)
        {
            var products = await productUniversalService.SelectAsync(items.Select(p => p.ProductId).ToArray(), false);
            foreach (var product in products)
            {
                foreach (var item in items)
                {
                    if(product.Id==item.ProductId)
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
        public async Task<Result<bool>> DeleteProduct(int id, [FromBody] object model)
        {
            return await productService.DeleteAsync(id);
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
                    ShowAll=true,
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
        public async Task<Result<ProductCategoryManageModel>> GetCategory(int id)
        {
            if (id == 0)
            {
                return new ProductCategoryManageModel()
                {
                    Template = String.Empty,
                    StatusCode = RecordStatusCode.Active,
                    Assigned = CustomerTypeCode.All,
                    NavIdVisible=CustomerTypeCode.All,
                };
            }
            return new ProductCategoryManageModel((await productCategoryService.GetCategoryAsync(id)));
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
        public async Task<Result<bool>> DeleteCategory(int id, [FromBody] object model)
        {
            return await productCategoryService.DeleteCategoryAsync(id);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<ProductCategoryStatisticTreeItemModel>> GetProductCategoriesStatistic([FromBody]ProductCategoryStatisticFilter filter)
        {
            return await productCategoryService.GetProductCategoriesStatisticAsync(filter);
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Products)]
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
            while (current <level)
            {
                prefix += "----";
                current++;
            }
            item.Name = prefix + item.Name;
            item.Percent = item.Percent / 100;
            list.Add(item);
            if(item.SubItems!=null)
            {
                foreach(var subItem in item.SubItems)
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
            if(model != null)
            {
                foreach(var modelCategory in model)
                {
                    categories.Add(modelCategory.Convert());
                }
            }

            return await inventoryCategoryService.UpdateCategoriesTreeAsync(categories);
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<InventoryCategoryManageModel>> GetInventoryCategory(int id)
        {
            if (id == 0)
            {
                return new InventoryCategoryManageModel()
                {
                };
            }
            return new InventoryCategoryManageModel((await inventoryCategoryService.GetCategoryAsync(id)));
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
        public async Task<Result<bool>> DeleteInventoryCategory(int id, [FromBody] object model)
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
        public async Task<Result<ProductReviewManageModel>> GetProductReview(int id)
        {
            if (id == 0)
            {
                return new ProductReviewManageModel()
                {
                    StatusCode = RecordStatusCode.NotActive,
                    Rating = 5,
                };
            }
            return new ProductReviewManageModel((await productReviewService.GetProductReviewAsync(id)));
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
        public async Task<Result<bool>> DeleteProductReview(int id, [FromBody] object model)
        {
            return await productReviewService.DeleteProductReviewAsync(id);
        }

        #endregion

        #region ProductOutOfStockRequests

        [HttpPost]
        public async Task<Result<ICollection<ProductOutOfStockContainerListItemModel>>> GetProductOutOfStockContainers([FromBody] object model)
        {
            var toReturn = await productService.GetProductOutOfStockContainersAsync();
            return toReturn.Select(p => new ProductOutOfStockContainerListItemModel(p)).ToList();
        }

        [HttpGet]
        public async Task<Result<string>> GetProductOutOfStockRequestsMessageFormat()
        {
            var setting = (await settingService.GetAppSettingItemsAsync(new List<string>() { SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE })).FirstOrDefault();
            if (setting == null)
            {
                throw new NotSupportedException($"{SettingConstants.PRODUCT_OUT_OF_STOCK_EMAIL_TEMPLATE} not configurated.");
            }
            return setting.Value;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Products)]
        public async Task<Result<bool>> SendProductOutOfStockRequests([FromBody]SendOutOfStockProductRequestsModel model)
        {
            return await productService.SendProductOutOfStockRequestsAsync(model.Ids,model.MessageFormat);
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
    }
}