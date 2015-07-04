using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models;
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
using System.Security.Claims;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Products)]
    public class ProductController : BaseApiController
    {
        private readonly IProductCategoryService productCategoryService;
        private readonly IProductService productService;
        private readonly IDynamicToModelMapper<ProductDynamic> _mapper;
        private readonly ILogger logger;

        public ProductController(IProductCategoryService productCategoryService, IProductService productService,
            ILoggerProviderExtended loggerProvider, IDynamicToModelMapper<ProductDynamic> mapper)
        {
            this.productCategoryService = productCategoryService;
            this.productService = productService;
            _mapper = mapper;
            this.logger = loggerProvider.CreateLoggerDefault();
        }

        #region Products

        [HttpGet]
        public async Task<Result<ProductEditSettingsModel>> GetProductEditSettings()
        {
            ProductEditSettingsModel toReturn = new ProductEditSettingsModel
            {
                Lookups =
                    (await productService.GetProductLookupsAsync()).Select(
                        p => new LookupViewModel(p.Name, p.IdObjectType, p.DefaultValue, p.Lookup)).ToList(),
                DefaultValues = await productService.GetProductEditDefaultSettingsAsync()
            };
            return toReturn;

        }

        [HttpPost]
        public async Task<Result<ICollection<SkuListItemModel>>> GetSkus([FromBody]VProductSkuFilter filter)
        {
            var result = await productService.GetSkusAsync(filter);

            return result.Select(p => new SkuListItemModel(p)).ToArray();
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

            var item = await productService.SelectAsync(id);
            
            ProductManageModel toReturn = _mapper.ToModel<ProductManageModel>(item);
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

            item = (await productService.UpdateAsync(item));

            ProductManageModel toReturn = _mapper.ToModel<ProductManageModel>(item);
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteProduct(int id)
        {
            return await productService.DeleteAsync(id);
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
        public async Task<Result<bool>> DeleteCategory(int id)
        {
            return await productCategoryService.DeleteCategoryAsync(id);
        }

        #endregion
    }
}