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
using VitalChoice.Domain.Entities.eCommerce.Discounts;
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
    public class DiscountController : BaseApiController
    {
        private readonly IDiscountService _discountService;
        private readonly IProductService _productService;
        private readonly IDynamicToModelMapper<DiscountDynamic> _mapper;
        private readonly ILogger _logger;
        private readonly TimeZoneInfo _pstTimeZoneInfo;

        public DiscountController(IDiscountService discountService, IProductService productService, ILoggerProviderExtended loggerProvider, IDynamicToModelMapper<DiscountDynamic> mapper)
        {
            this._discountService = discountService;
            this._productService = productService;
            _mapper = mapper;
            this._logger = loggerProvider.CreateLoggerDefault();
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
        public async Task<Result<bool>> DeleteDiscount(int id)
        {
            return await _discountService.DeleteAsync(id);
        }

        #endregion
    }
}