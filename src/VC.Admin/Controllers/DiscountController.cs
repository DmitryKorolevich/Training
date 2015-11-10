using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Entities;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using System.Security.Claims;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.DynamicData.Entities;
using System;
using VitalChoice.Domain.Transfer.Settings;
using Newtonsoft.Json;
using VitalChoice.Interfaces.Services.Settings;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Products)]
    public class DiscountController : BaseApiController
    {
        private readonly IDiscountService _discountService;
        private readonly IProductService _productService;
        private readonly IDynamicMapper<DiscountDynamic, Discount> _mapper;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly ILogger _logger;
        private readonly TimeZoneInfo _pstTimeZoneInfo;

        public DiscountController(
            IDiscountService discountService, 
            IProductService productService,
            ILoggerProviderExtended loggerProvider,
            IDynamicMapper<DiscountDynamic, Discount> mapper,
            IObjectHistoryLogService objectHistoryLogService)
        {
            _discountService = discountService;
            _productService = productService;
            _objectHistoryLogService = objectHistoryLogService;
            _mapper = mapper;
            _logger = loggerProvider.CreateLoggerDefault();
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        }

        #region Products
        
        [HttpPost]
        public async Task<Result<PagedList<DiscountListItemModel>>> GetDiscounts([FromBody]DiscountFilter filter)
        {
            var result = await _discountService.GetDiscountsAsync(filter);

            var toReturn = new PagedList<DiscountListItemModel>
            {
                Items = result.Items.Select(p => new DiscountListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<DiscountManageModel>> GetDiscount(int id)
        {
            if (id == 0)
            {
                var now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day);
                return new DiscountManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                    Assigned = null,//All
                    DiscountType=DiscountType.PercentDiscount,
                    StartDate = TimeZoneInfo.ConvertTime(now, _pstTimeZoneInfo, TimeZoneInfo.Local),
                    ExpirationDate= TimeZoneInfo.ConvertTime(now.AddDays(30), _pstTimeZoneInfo, TimeZoneInfo.Local),
                    DiscountsToSelectedSkus = new List<DiscountToSelectedSku>(),
                    DiscountsToSkus = new List<DiscountToSku>(),
                    DiscountTiers = new List<DiscountTier>(),
                    CategoryIds = new List<int>(),
                    CategoryIdsAppliedOnlyTo = new List<int>(),
                };
            }

            var item = await _discountService.SelectAsync(id);
            DiscountManageModel toReturn = _mapper.ToModel<DiscountManageModel>(item);
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<DiscountManageModel>> UpdateDiscount([FromBody]DiscountManageModel model)
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
                item = await _discountService.UpdateAsync(item);
            }
            else
            {
                item = await _discountService.InsertAsync(item);
            }

            return _mapper.ToModel<DiscountManageModel>(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteDiscount(int id, [FromBody] object model)
        {
            return await _discountService.DeleteAsync(id);
        }

        #endregion

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (DiscountDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(DiscountDynamic));
                var model = _mapper.ToModel<DiscountManageModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (DiscountDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(DiscountDynamic));
                var model = _mapper.ToModel<DiscountManageModel>(dynamic);
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