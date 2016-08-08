using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Interfaces.Services;
using VitalChoice.Validation.Models;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services.Healthwise;
using VitalChoice.Interfaces.Services.Orders;
using VC.Admin.Models.Healthwise;
using VitalChoice.Business.Mailings;
using VitalChoice.Infrastructure.Domain.Transfer.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Admin.Controllers
{
    public class HealthwiseController : BaseApiController
    {
        private readonly IHealthwiseService _healthwiseService;
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly ExtendedUserManager _userManager;
        private readonly ReferenceData _referenceData;
        private readonly AppSettings _appSettings;

        public HealthwiseController(
            IHealthwiseService healthwiseService,
            IOrderService orderService,
            INotificationService notificationService,
            ILoggerProviderExtended loggerProvider,
            ExtendedUserManager userManager, ReferenceData referenceData, AppSettings appSettings)
        {
            _healthwiseService = healthwiseService;
            _orderService = orderService;
            _notificationService = notificationService;
            _userManager = userManager;
            _referenceData = referenceData;
            _appSettings = appSettings;
            _logger = loggerProvider.CreateLogger<HealthwiseController>();
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<ICollection<HealthwiseCustomerListItemModel>>> GetHealthwiseCustomersWithPeriods(
            [FromBody] VHealthwisePeriodFilter filter)
        {
            var toReturn = new List<HealthwiseCustomerListItemModel>();
            var items = await _healthwiseService.GetVHealthwisePeriodsAsync(filter);
            var periodGroups = items.GroupBy(p => p.IdCustomer).Select(p => new
            {
                IdCustomer = p.Key,
                Periods = p.ToList()
            }).ToList();
            var hwMaxPeriod = _appSettings.HealthwisePeriodMaxItemsCount;
            foreach (var periodGroup in periodGroups)
            {
                var item = new HealthwiseCustomerListItemModel(periodGroup.Periods);
                SetAllowPaymentPeriodSetting(item, hwMaxPeriod);
                toReturn.Add(item);
            }
            return toReturn;
        }

        [NonAction]
        private void SetAllowPaymentPeriodSetting(HealthwiseCustomerListItemModel model, int hwMaxPeriod)
        {
            foreach (var period in model.Periods)
            {
                period.AllowPayment = !period.PaidDate.HasValue && period.OrdersCount >= hwMaxPeriod;
            }
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<Result<HealthwiseCustomerListItemModel>> GetHealthwisePeriod(int id)
        {
            var item = await _healthwiseService.GetVHealthwisePeriodAsync(id);
            HealthwiseCustomerListItemModel toReturn;
            if(item!=null)
            {
                var hwMaxPeriod = _appSettings.HealthwisePeriodMaxItemsCount;
                toReturn = new HealthwiseCustomerListItemModel(new List<VHealthwisePeriod>() { item});
                SetAllowPaymentPeriodSetting(toReturn, hwMaxPeriod);
            }
            else
            {
                toReturn = new HealthwiseCustomerListItemModel(null);
            }
            return toReturn;
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<bool>> DeleteHealthwisePeriod(int id)
        {
            var toReturn = await _healthwiseService.DeleteHealthwisePeriod(id);

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<Result<ICollection<HealthwiseOrderListItemModel>>> GetHealthwiseOrders(int id)
        {
            var items = await _healthwiseService.GetHealthwiseOrdersAsync(id);
            return items.Select(p=>new HealthwiseOrderListItemModel(p)).ToList();
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<bool>> MakeHealthwisePeriodPayment([FromBody]HealthwisePeriodMakePaymentModel model)
        {
            if (!Validate(model))
                return false;

            var sUserId = _userManager.GetUserId(User);
            int tempId;
            int? userId = null;
            if (Int32.TryParse(sUserId, out tempId))
            {
                userId = tempId;
            }
            return await _healthwiseService.MakeHealthwisePeriodPaymentAsync(model.Id, model.Amount, model.Date, model.PayAsGC, userId);
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<bool>> MarkOrder(int id)
        {
            return await _orderService.UpdateHealthwiseOrderWithValidationAsync(id, true);
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<bool>> MarkCustomerOrders(int id)
        {
            return await _healthwiseService.MarkOrdersAsHealthwiseForCustomerIdAsync(id);
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<ICollection<HealthwisePeriodListItemModel>>> GetHealthwisePeriodsForMovement([FromBody] HealthwiseOrdersMoveModel model)
        {
            var items = await _healthwiseService.GetVHealthwisePeriodsForMovementAsync(model.IdPeriod, model.Ids!=null ? model.Ids.Count : 0);
            return items.Select(p => new HealthwisePeriodListItemModel(p)).ToList();
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<bool>> MoveHealthwiseOrders([FromBody]HealthwiseOrdersMoveModel model)
        {
            return await _healthwiseService.MoveHealthwiseOrdersAsync(model.IdPeriod,model.Ids);
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<bool>> AddPeriod(int id,[FromBody]object model)
        {
            var period =await _healthwiseService.AddPeriodAsync(id);
            return period != null;
        }
    }
}