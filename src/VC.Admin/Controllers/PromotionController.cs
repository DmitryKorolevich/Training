using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Entities;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Domain.Entities.eCommerce.Promotions;
using System.Security.Claims;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.DynamicData.Entities;
using System;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Products)]
    public class PromotionController : BaseApiController
    {
        private readonly IPromotionService _promotionService;
        private readonly IProductService _productService;
        private readonly IDynamicToModelMapper<PromotionDynamic> _mapper;
        private readonly ILogger _logger;

        public PromotionController(IPromotionService promotionService, IProductService productService, ILoggerProviderExtended loggerProvider, IDynamicToModelMapper<PromotionDynamic> mapper)
        {
            this._promotionService = promotionService;
            this._productService = productService;
            _mapper = mapper;
            this._logger = loggerProvider.CreateLoggerDefault();
        }

        #region Products
        
        [HttpPost]
        public async Task<Result<PagedList<PromotionListItemModel>>> GetPromotions([FromBody]PromotionFilter filter)
        {
            var result = await _promotionService.GetPromotionsAsync(filter);

            var toReturn = new PagedList<PromotionListItemModel>
            {
                Items = result.Items.Select(p => new PromotionListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<PromotionManageModel>> GetPromotion(int id)
        {
            if (id == 0)
            {
                var now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day);
                return new PromotionManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                    Assigned = null,//All
                    IdObjectType = PromotionType.BuyXGetY,
                    StartDate = now,
                    ExpirationDate= now.AddDays(30),
                    PromotionsToBuySkus = new List<PromotionToBuySkuModel>(),
                    PromotionsToGetSkus = new List<PromotionToGetSkuModel>(),
                };
            }

            var item = await _promotionService.SelectAsync(id);
            PromotionManageModel toReturn = _mapper.ToModel<PromotionManageModel>(item);

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<PromotionManageModel>> UpdatePromotion([FromBody]PromotionManageModel model)
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
            if (item.Id > 0)
            {
                item = await _promotionService.UpdateAsync(item);
            }
            else
            {
                item = await _promotionService.InsertAsync(item);
            }

            return _mapper.ToModel<PromotionManageModel>(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeletePromotion(int id)
        {
            return await _promotionService.DeleteAsync(id);
        }

        #endregion
    }
}