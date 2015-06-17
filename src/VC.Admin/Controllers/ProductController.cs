using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models.Product;
using VitalChoice.Business.Services;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Entities;
using VitalChoice.DynamicData.Entities;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Products)]
    public class ProductController : BaseApiController
    {
        private readonly IProductCategoryService productCategoryService;
        private readonly IProductService productService;
        private readonly ILogger logger;

        public ProductController(IProductCategoryService productCategoryService, IProductService productService)
        {
            this.productCategoryService = productCategoryService;
            this.productService = productService;
            this.logger = LoggerService.GetDefault();
        }

        #region Products

        [HttpGet]
        public async Task<Result<ProductEditSettingsModel>> GetProductEditSettings()
        {
            ProductEditSettingsModel toReturn = new ProductEditSettingsModel();
            toReturn.Lookups = (await productService.GetProductLookupsAsync()).Select(p => new LookupViewModel(p.Name,
                p.IdProductType.HasValue ? (int?)p.IdProductType.Value : null
                , p.DefaultValue, p.Lookup)).ToList();
            toReturn.DefaultValues = await productService.GetProductEditDefaultSettingsAsync();
            return toReturn;

        }

        [HttpPost]
        public async Task<Result<PagedList<SkuListItemModel>>> GetSkus([FromBody]VProductSkuFilter filter)
        {
            var result = await productService.GetSkusAsync(filter);

            var toReturn = new PagedList<SkuListItemModel>
            {
                Items = result.Items.Select(p => new SkuListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
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
                        new CrossSellProductModel(),
                        new CrossSellProductModel(),
                        new CrossSellProductModel(),
                        new CrossSellProductModel(),
                    },
                    Videos = new List<VideoModel>()
                    {
                        new VideoModel(),
                        new VideoModel(),
                        new VideoModel(),
                    },
                };
            }

            var item = await productService.GetProductAsync(id);
            ProductManageModel toReturn = item.ToModel<ProductManageModel, ProductDynamic>();
            return toReturn;

            return await Task.FromResult<ProductManageModel>(new ProductManageModel()
            {
                Id = 5,
                Name = "Test",
                Url = "test",
                StatusCode = RecordStatusCode.Active,
                Type = ProductType.Gc,
                Hidden = false,
                CategoryIds = new List<int>() { 6, 7, 5 },
                SKUs = new List<SKUManageModel>()
                {
                    new SKUManageModel()
                    {
                        Id = 43454,
                        Name = "FRP006",
                        Active = true,
                        Hidden = true,
                        RetailPrice = (decimal)55.00,
                        WholesalePrice = (decimal)9.00,
                        Stock = 5,
                        DisregardStock = true,
                        AutoShipFrequency2=true,
                        AutoShipFrequency6=true,
                        DisallowSingle=true,
                        AutoShipProduct=true,
                        HideFromDataFeed=true,
                        OffPercent = 11,
                        NonDiscountable=true,
                    },
                    new SKUManageModel()
                    {
                        Id = 43454,
                        Name = "FRP006",
                        Active = true,
                        Hidden = true,
                        RetailPrice = (decimal)55.00,
                        WholesalePrice = (decimal)9.00,
                        Stock = 5,
                        DisregardStock = true,
                        AutoShipFrequency2=true,
                        AutoShipFrequency6=true,
                        DisallowSingle=true,
                        AutoShipProduct=true,
                        HideFromDataFeed=true,
                        OffPercent = 11,
                        NonDiscountable=true,
                    },
                },
                CrossSellProducts = new List<CrossSellProductModel>()
                {
                    new CrossSellProductModel(),
                    new CrossSellProductModel(),
                    new CrossSellProductModel(),
                    new CrossSellProductModel(),
                },
                Videos = new List<VideoModel>() {
                    new VideoModel(),
                    new VideoModel(),
                    new VideoModel(),
                },
            });
        }

        [HttpPost]
        public async Task<Result<ProductManageModel>> UpdateProduct([FromBody]ProductManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = (await productService.UpdateProductAsync(item));

            ProductManageModel toReturn = item.ToModel<ProductManageModel, ProductDynamic>();
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteProduct(int id)
        {
            return await productService.DeleteProductAsync(id);
        }

        #endregion

        #region Categories

        [HttpPost]
        public async Task<Result<ProductCategoryTreeItemModel>> GetCategoriesTree([FromBody]ProductCategoryTreeFilter filter)
        {
            var result = await productCategoryService.GetCategoriesTreeAsync(filter);

            return new ProductCategoryTreeItemModel(result);
        }

        [HttpPost]
        public async Task<Result<bool>> UpdateCategoriesTree([FromBody]ProductCategoryTreeItemModel model)
        {
            var category = ConvertWithValidate(model);
            if (category == null)
                return false;

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
                };
            }
            return new ProductCategoryManageModel((await productCategoryService.GetCategoryAsync(id)));
        }

        [HttpPost]
        public async Task<Result<ProductCategoryManageModel>> UpdateCategory([FromBody]ProductCategoryManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await productCategoryService.UpdateCategoryAsync(item);

            return new ProductCategoryManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteCategory(int id)
        {
            return await productCategoryService.DeleteCategoryAsync(id);
        }

        #endregion
    }
}