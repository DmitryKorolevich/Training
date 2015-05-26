using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models.Product;
using VitalChoice.Business.Services;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Transfer.Product;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Interfaces.Services.Product;
using VitalChoice.Validation.Base;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Products)]
    public class ProductController : BaseApiController
    {
        private readonly IProductCategoryService productCategoryService;
        private readonly ILogger logger;

        public ProductController(IProductCategoryService productCategoryService)
        {
            this.productCategoryService = productCategoryService;
            this.logger = LoggerService.GetDefault();
        }

        #region Products

        [HttpPost]
        public async Task<Result<ProductManageModel>> UpdateProduct([FromBody]ProductManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            //item = await productCategoryService.UpdateCategoryAsync(item);

            return new ProductManageModel();
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