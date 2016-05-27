using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using System.Security.Claims;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using System;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using VitalChoice.Interfaces.Services.Settings;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VC.Admin.Models.Products;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Marketing)]
    public class PromotionController : BaseApiController
    {
        private readonly IPromotionService _promotionService;
        private readonly IProductService _productService;
        private readonly IDynamicMapper<PromotionDynamic, Promotion> _mapper;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly ILogger _logger;
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly ExtendedUserManager _userManager;

        public PromotionController(
            IPromotionService promotionService, 
            IProductService productService, 
            ILoggerProviderExtended loggerProvider, 
            IDynamicMapper<PromotionDynamic, Promotion> mapper,
            IObjectHistoryLogService objectHistoryLogService, ExtendedUserManager userManager)
        {
            _promotionService = promotionService;
            _productService = productService;
            _objectHistoryLogService = objectHistoryLogService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = loggerProvider.CreateLogger<PromotionController>();
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        }

        #region Promotions
        
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
                    Assigned = null, //All
                    IdObjectType = PromotionType.BuyXGetY,
                    PromotionBuyType = PromoBuyType.Any,
                    StartDate = TimeZoneInfo.ConvertTime(now, _pstTimeZoneInfo, TimeZoneInfo.Local),
                    ExpirationDate = TimeZoneInfo.ConvertTime(now.AddDays(30), _pstTimeZoneInfo, TimeZoneInfo.Local),
                    CanUseWithDiscount=true,
                    PromotionsToBuySkus = new List<PromotionToBuySkuModel>(),
                    PromotionsToGetSkus = new List<PromotionToGetSkuModel>(),
                };
            }

            var item = await _promotionService.SelectAsync(id);
            PromotionManageModel toReturn = await _mapper.ToModelAsync<PromotionManageModel>(item);

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<PromotionManageModel>> UpdatePromotion([FromBody]PromotionManageModel model)
        {
            if (!Validate(model))
                return null;
            var promotion = await _mapper.FromModelAsync(model);

            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                promotion.IdEditedBy = userId;
            }
            if (promotion.Id > 0)
            {
                promotion = await _promotionService.UpdateAsync(promotion);
            }
            else
            {
                promotion = await _promotionService.InsertAsync(promotion);
            }

            return await _mapper.ToModelAsync<PromotionManageModel>(promotion);
        }

        [HttpPost]
        public async Task<Result<bool>> DeletePromotion(int id)
        {
            return await _promotionService.DeleteAsync(id);
        }

        #endregion

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (PromotionDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(PromotionDynamic));
                var model = await _mapper.ToModelAsync<PromotionManageModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (PromotionDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(PromotionDynamic));
                var model = await _mapper.ToModelAsync<PromotionManageModel>(dynamic);
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