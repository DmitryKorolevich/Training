﻿using System.Collections.Generic;
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
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Infrastructure.Services;

namespace VC.Admin.Controllers
{
    public class PromotionController : BaseApiController
    {
        private readonly IPromotionService _promotionService;
        private readonly IProductService _productService;
        private readonly IDynamicMapper<PromotionDynamic, Promotion> _mapper;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly ILogger _logger;
        private readonly ExtendedUserManager _userManager;

        public PromotionController(
            IPromotionService promotionService, 
            IProductService productService, 
            ILoggerFactory loggerProvider, 
            IDynamicMapper<PromotionDynamic, Promotion> mapper,
            IObjectHistoryLogService objectHistoryLogService, ExtendedUserManager userManager)
        {
            _promotionService = promotionService;
            _productService = productService;
            _objectHistoryLogService = objectHistoryLogService;
            _userManager = userManager;
            _mapper = mapper;
            _logger = loggerProvider.CreateLogger<PromotionController>();
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
        public async Task<Result<PromotionManageModel>> GetPromotion(string id)
        {
            int idPromotion = 0;
            if (id != null && !Int32.TryParse(id, out idPromotion))
                throw new NotFoundException();

            if (idPromotion == 0)
            {
                var now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day);
                return new PromotionManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                    Assigned = CustomerType.Retail,
                    IdObjectType = PromotionType.BuyXGetY,
                    PromotionBuyType = PromoBuyType.Any,
                    StartDate = TimeZoneInfo.ConvertTime(now, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local),
                    ExpirationDate = TimeZoneInfo.ConvertTime(now.AddDays(30), TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local),
                    CanUseWithDiscount=true,
                    PromotionsToBuySkus = new List<PromotionToBuySkuModel>(),
                    PromotionsToGetSkus = new List<PromotionToGetSkuModel>(),
                };
            }

            var item = await _promotionService.SelectAsync(idPromotion);
            if (item == null)
                throw new NotFoundException();

            PromotionManageModel toReturn = await _mapper.ToModelAsync<PromotionManageModel>(item);

            return toReturn;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Marketing)]
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
        [AdminAuthorize(PermissionType.Marketing)]
        public async Task<Result<bool>> DeletePromotion(int id)
        {
            return await _promotionService.DeleteAsync(id);
        }

        #endregion

        [HttpPost]
        [AdminAuthorize(PermissionType.Marketing)]
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