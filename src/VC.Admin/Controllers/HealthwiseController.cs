using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Setting;
using VitalChoice.Business.Queries.User;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using VitalChoice.Interfaces.Services.Healthwise;
using VitalChoice.Interfaces.Services.Orders;
using VC.Admin.Models.Healthwise;
using VitalChoice.Infrastructure.Domain.Transfer.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Orders)]
    public class HealthwiseController : BaseApiController
    {
        private readonly IHealthwiseService _healthwiseService;
        private readonly IOrderService _orderService;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly ILogger _logger;
        private readonly ExtendedUserManager _userManager;

        public HealthwiseController(
            IHealthwiseService healthwiseService,
            IOrderService orderService,
            IAppInfrastructureService appInfrastructureService,
            ILoggerProviderExtended loggerProvider, ExtendedUserManager userManager)
        {
            _healthwiseService = healthwiseService;
            _orderService = orderService;
            _appInfrastructureService = appInfrastructureService;
            _userManager = userManager;
            _logger = loggerProvider.CreateLogger<HealthwiseController>();
        }

	    [HttpPost]
        public async Task<Result<ICollection<HealthwiseCustomerListItemModel>>> GetHealthwiseCustomersWithPeriods([FromBody]VHealthwisePeriodFilter filter)
	    {
            var toReturn = new List<HealthwiseCustomerListItemModel>();
			var items = await _healthwiseService.GetVHealthwisePeriodsAsync(filter);
            var periodGroups = items.GroupBy(p => p.IdCustomer).Select(p => new
            {
                IdCustomer = p.Key,
                Periods = p.ToList()
            }).ToList();
            foreach(var periodGroup in periodGroups)
            {
                var item = new HealthwiseCustomerListItemModel(periodGroup.Periods);
                SetAllowPaymentPeriodSetting(item);
                toReturn.Add(item);
            }
            return toReturn;
		}

        [NonAction]
        private void SetAllowPaymentPeriodSetting(HealthwiseCustomerListItemModel model)
        {
            foreach (var period in model.Periods)
            {
                period.AllowPayment = !period.PaidDate.HasValue && period.OrdersCount >= _appInfrastructureService.Data().AppSettings.HealthwisePeriodMaxItemsCount;
            }
        }

        [HttpGet]
        public async Task<Result<HealthwiseCustomerListItemModel>> GetHealthwisePeriod(int id)
        {
            var item = await _healthwiseService.GetVHealthwisePeriodAsync(id);
            HealthwiseCustomerListItemModel toReturn;
            if(item!=null)
            {
                toReturn = new HealthwiseCustomerListItemModel(new List<VHealthwisePeriod>() { item});
                SetAllowPaymentPeriodSetting(toReturn);
            }
            else
            {
                toReturn = new HealthwiseCustomerListItemModel(null);
            }
            return toReturn;
        }

        [HttpGet]
        public async Task<Result<ICollection<HealthwiseOrderListItemModel>>> GetHealthwiseOrders(int id)
        {
            var items = await _healthwiseService.GetHealthwiseOrdersAsync(id);
            return items.Select(p=>new HealthwiseOrderListItemModel(p)).ToList();
        }

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

        [HttpPost]
        public async Task<Result<bool>> MarkOrder(int id)
        {
            return await _orderService.UpdateHealthwiseOrderWithValidationAsync(id, true);
        }

        [HttpPost]
        public async Task<Result<bool>> MarkCustomerOrders(int id)
        {
            return await _healthwiseService.MarkOrdersAsHealthwiseForCustomerIdAsync(id);
        }

        [HttpPost]
        public async Task<Result<ICollection<HealthwisePeriodListItemModel>>> GetHealthwisePeriodsForMovement([FromBody] HealthwiseOrdersMoveModel model)
        {
            var items = await _healthwiseService.GetVHealthwisePeriodsForMovementAsync(model.IdPeriod, model.Ids!=null ? model.Ids.Count : 0);
            return items.Select(p => new HealthwisePeriodListItemModel(p)).ToList();
        }

        [HttpPost]
        public async Task<Result<bool>> MoveHealthwiseOrders([FromBody]HealthwiseOrdersMoveModel model)
        {
            return await _healthwiseService.MoveHealthwiseOrdersAsync(model.IdPeriod,model.Ids);
        }

        [HttpPost]
        public async Task<Result<bool>> AddPeriod(int id,[FromBody]object model)
        {
            var period =await _healthwiseService.AddPeriodAsync(id);
            return period != null;
        }
    }
}