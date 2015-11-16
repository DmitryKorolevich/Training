using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VC.Admin.Models;
using VC.Admin.Models.Product;
using VitalChoice.Validation.Models;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Interfaces.Services.Products;
using System.Security.Claims;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using Newtonsoft.Json;
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

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Products)]
    public class ProductController : BaseApiController
    {
        private readonly IProductCategoryService productCategoryService;
        private readonly IProductService productService;
        private readonly IEcommerceDynamicService<ProductDynamic, Product, ProductOptionType, ProductOptionValue> productUniversalService;
        private readonly IInventoryCategoryService inventoryCategoryService;
        private readonly IProductReviewService productReviewService;
        private readonly IDynamicMapper<ProductDynamic, Product> _mapper;
        private readonly IObjectHistoryLogService objectHistoryLogService;
        private readonly ILogger logger;

        public ProductController(IProductCategoryService productCategoryService, IProductService productService,
            IEcommerceDynamicService<ProductDynamic, Product, ProductOptionType, ProductOptionValue> productUniversalService,
            IInventoryCategoryService inventoryCategoryService, IProductReviewService productReviewService,
            ILoggerProviderExtended loggerProvider, IDynamicMapper<ProductDynamic, Product> mapper,
            IObjectHistoryLogService objectHistoryLogService)
        {
            this.productCategoryService = productCategoryService;
            this.inventoryCategoryService = inventoryCategoryService;
            this.productService = productService;
            this.productUniversalService = productUniversalService;
            this.productReviewService = productReviewService;
            this.objectHistoryLogService = objectHistoryLogService;
            _mapper = mapper;
            this.logger = loggerProvider.CreateLoggerDefault();
        }

        #region Products

        [HttpGet]
        public Task<Result<ProductEditSettingsModel>> GetProductEditSettings()
        {
            var lookups = productService.GetProductLookupsAsync().Select(
                        p => new LookupViewModel(p.Name, p.IdObjectType, p.DefaultValue, p.Lookup)).ToList();
            var defaultValues = productService.GetProductEditDefaultSettingsAsync();
            ProductEditSettingsModel toReturn = new ProductEditSettingsModel()
            {
                Lookups = lookups,
                DefaultValues = defaultValues
            };
            return Task.FromResult<Result<ProductEditSettingsModel>>(toReturn);
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
        public async Task<Result<ICollection<SkuWithStatisticListItemModel>>> GetTopPurchasedSkus()
        {
            ICollection<SkuWithStatisticListItemModel> toReturn = new List<SkuWithStatisticListItemModel>();

            FilterBase idsFilter = new FilterBase();
            idsFilter.Paging.PageIndex = 1;
            idsFilter.Paging.PageItemCount = 20;
            Dictionary<int, int> items =await productService.GetTopPurchasedSkuIdsAsync(idsFilter);
            //items.Add(54, 20);
            //items.Add(55, 5);
            //items.Add(25, 10);
            //items.Add(1, 1);

            VProductSkuFilter filter = new VProductSkuFilter();
            filter.Ids = items.Select(p => p.Key).ToList();
            filter.Paging.PageIndex = 1;
            filter.Paging.PageItemCount = 20;

            var result = await productService.GetSkusAsync(filter);
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

            return toReturn.ToArray();
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
            var skus = await productService.GetSkusAsync(skuFilter);
            foreach (var item in toReturn.Items)
            {
                item.SKUs = new List<SkuListItemModel>();
                foreach (var sku in skus)
                {
                    if(item.ProductId==sku.IdProduct)
                    {
                        item.SKUs.Add(new SkuListItemModel(sku));
                    }
                }
            }

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<ProductManageModel>> GetProduct(int id)
        {
            if (id == 0)
            {
                return new ProductManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                    Hidden = false,
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
                    },
                };
            }

            var item = await productService.SelectTransferAsync(id);
            
            ProductManageModel toReturn = _mapper.ToModel<ProductManageModel>(item!=null ? item.ProductDynamic : null);
            if (item.ProductContent != null)
            {
                toReturn.Url = item.ProductContent.Url;
                toReturn.MasterContentItemId = item.ProductContent.MasterContentItemId;
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
        public async Task<Result<ProductManageModel>> UpdateProduct([FromBody]ProductManageModel model)
        {
            if (!Validate(model))
                return null;
            
            var item = _mapper.FromModel(model);
            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }

            ProductContentTransferEntity transferEntity = new ProductContentTransferEntity();
            ProductContent content = new ProductContent();
            content.Id = model.Id;
            content.Url = model.Url;
            content.ContentItem = new ContentItem();
            content.ContentItem.Template = String.Empty;
            content.ContentItem.Description = String.Empty;
            transferEntity.ProductContent = content;
            transferEntity.ProductDynamic = item;

            if (model.Id > 0)
                item = (await productService.UpdateAsync(transferEntity));
            else
                item = (await productService.InsertAsync(transferEntity));

            ProductManageModel toReturn = _mapper.ToModel<ProductManageModel>(item);
            return toReturn;
        }

        [HttpPost]
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
                    Statuses = filter.Statuses,
                    Paging = filter.Paging,
                    SearchText = filter.SearchText,
                    Sorting = filter.Sorting
                });

            return new ProductCategoryTreeItemModel(contentCategories);
        }

        [HttpPost]
        public async Task<Result<bool>> UpdateCategoriesTree([FromBody]ProductCategoryTreeItemModel model)
        {            
            if (!Validate(model))
                return false;
            var category = model.Convert();

            return await productCategoryService.UpdateCategoriesTreeAsync(category);
        }

        [HttpGet]
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
        public async Task<Result<ProductCategoryManageModel>> UpdateCategory([FromBody]ProductCategoryManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await productCategoryService.UpdateCategoryAsync(item);

            return new ProductCategoryManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteCategory(int id, [FromBody] object model)
        {
            return await productCategoryService.DeleteCategoryAsync(id);
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
        public async Task<Result<bool>> UpdateInventoryCategoriesTree([FromBody]IList<InventoryCategoryTreeItemModel> model)
        {
            IList<InventoryCategory> categories = new List<InventoryCategory>();
            if(categories!=null)
            {
                foreach(var modelCategory in model)
                {
                    categories.Add(modelCategory.Convert());
                }
            }

            return await inventoryCategoryService.UpdateCategoriesTreeAsync(categories);
        }

        [HttpGet]
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
        public async Task<Result<InventoryCategoryManageModel>> UpdateInventoryCategory([FromBody]InventoryCategoryManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await inventoryCategoryService.UpdateCategoryAsync(item);

            return new InventoryCategoryManageModel(item);
        }

        [HttpPost]
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
        public async Task<Result<ProductReviewManageModel>> UpdateProductReview([FromBody]ProductReviewManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            item = await productReviewService.UpdateProductReviewAsync(item);

            return new ProductReviewManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteProductReview(int id, [FromBody] object model)
        {
            return await productReviewService.DeleteProductReviewAsync(id);
        }

        #endregion

        #region ProductOutOfStockRequests

        [HttpPost]
        public async Task<Result<ICollection<ProductOutOfStockContainerListItemModel>>> GetProductOutOfStockContainers([FromBody] object model)
        {
            var toReturn = await productService.GetProductOutOfStockContainers();
            return toReturn.Select(p => new ProductOutOfStockContainerListItemModel(p)).ToList();
        }

        [HttpPost]
        public async Task<Result<bool>> SendProductOutOfStockRequests([FromBody]ICollection<int> ids)
        {
            return await productService.SendProductOutOfStockRequests(ids);
        }

        #endregion

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (ProductDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(ProductDynamic));
                var model = _mapper.ToModel<ProductManageModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (ProductDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(ProductDynamic));
                var model = _mapper.ToModel<ProductManageModel>(dynamic);
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