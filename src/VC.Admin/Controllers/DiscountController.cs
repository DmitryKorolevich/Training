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
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using VitalChoice.Interfaces.Services.Settings;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VC.Admin.Models.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Services;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Marketing)]
    public class DiscountController : BaseApiController
    {
        private readonly IDiscountService _discountService;
        private readonly IProductService _productService;
        private readonly IDynamicMapper<DiscountDynamic, Discount> _mapper;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly ExtendedUserManager _userManager;
        private readonly ReferenceData _referenceData;
        private readonly ILogger _logger;

        public DiscountController(
            IDiscountService discountService, 
            IProductService productService,
            ILoggerProviderExtended loggerProvider,
            IDynamicMapper<DiscountDynamic, Discount> mapper,
            IObjectHistoryLogService objectHistoryLogService, ExtendedUserManager userManager, ReferenceData referenceData)
        {
            _discountService = discountService;
            _productService = productService;
            _objectHistoryLogService = objectHistoryLogService;
            _userManager = userManager;
            _referenceData = referenceData;
            _mapper = mapper;
            _logger = loggerProvider.CreateLogger<DiscountController>();
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
        public async Task<Result<DiscountManageModel>> GetDiscount(string id)
        {
            int idProduct = 0;
            if (id != null && !Int32.TryParse(id, out idProduct))
                throw new NotFoundException();

            if (idProduct == 0)
            {
                var now = DateTime.Now;
                now = new DateTime(now.Year, now.Month, now.Day);
                return new DiscountManageModel()
                {
                    StatusCode = RecordStatusCode.Active,
                    Assigned = null,//All
                    DiscountType=DiscountType.PercentDiscount,
                    StartDate = TimeZoneInfo.ConvertTime(now, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local),
                    ExpirationDate= TimeZoneInfo.ConvertTime(now.AddDays(30), TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local),
                    DiscountsToSelectedSkus = new List<DiscountToSelectedSku>(),
                    DiscountsToSkus = new List<DiscountToSku>(),
                    DiscountTiers = new List<DiscountTier>(),
                    CategoryIds = new List<int>(),
                    CategoryIdsAppliedOnlyTo = new List<int>(),
                };
            }

            var item = await _discountService.SelectAsync(idProduct);
            if (item == null)
                throw new NotFoundException();

            DiscountManageModel toReturn = await _mapper.ToModelAsync<DiscountManageModel>(item);
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<DiscountManageModel>> UpdateDiscount([FromBody]DiscountManageModel model)
        {
            if (!Validate(model))
                return null;
            var discount = await _mapper.FromModelAsync(model);
            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                discount.IdEditedBy = userId;
            }
            if (discount.Id > 0)
            {
                var superAdminName = _referenceData.AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
                var isSuperAdmin = HttpContext.User.IsInRole(superAdminName.Normalize());
                discount = await _discountService.UpdateWithSuperAdminCheckAsync(discount, isSuperAdmin);
            }
            else
            {
                discount = await _discountService.InsertAsync(discount);
            }

            return await _mapper.ToModelAsync<DiscountManageModel>(discount);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteDiscount(int id)
        {
            var superAdminName = _referenceData.AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser).Text;
            var isSuperAdmin = HttpContext.User.IsInRole(superAdminName.Normalize());
            return await _discountService.DeleteWithSuperAdminCheckAsync(id, isSuperAdmin);
        }

        #endregion

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (DiscountDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(DiscountDynamic));
                var model = await _mapper.ToModelAsync<DiscountManageModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (DiscountDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(DiscountDynamic));
                var model = await _mapper.ToModelAsync<DiscountManageModel>(dynamic);
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