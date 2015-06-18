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
using VitalChoice.Interfaces.Services.Product;
using VitalChoice.DynamicData.Entities;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using System.Security.Claims;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Products)]
    public class DiscountController : BaseApiController
    {
        private readonly IDiscountService _discountService;
        private readonly ILogger _logger;

        public DiscountController(IDiscountService discountService)
        {
            this._discountService = discountService;
            this._logger = LoggerService.GetDefault();
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
                    Assigned = CustomerTypeCode.All,
                    DiscountType=DiscountType.PriceDiscount,
                    StartDate = now,
                    ExpirationDate= now.AddDays(30),
                    DiscountsToSelectedSkus = new List<DiscountToSelectedSku>(),
                    DiscountsToSkus = new List<DiscountToSku>(),
                    DiscountTiers = new List<DiscountTier>(),
                    CategoryIds = new List<int>(),
                };
            }

            var item = await _discountService.GetDiscountAsync(id);
            DiscountManageModel toReturn = item.ToModel<DiscountManageModel, DiscountDynamic>();
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<DiscountManageModel>> UpdateDiscount([FromBody]DiscountManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }

            item = (await _discountService.UpdateDiscountAsync(item));

            DiscountManageModel toReturn = item.ToModel<DiscountManageModel, DiscountDynamic>();
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteDiscount(int id)
        {
            return await _discountService.DeleteDiscountAsync(id);
        }

        #endregion
    }
}