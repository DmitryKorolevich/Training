using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
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
using Microsoft.Extensions.OptionsModel;
using Newtonsoft.Json;
using VitalChoice.Business.ExportMaps;
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

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Orders)]
    public class HealthwiseController : BaseApiController
    {
        private readonly IHealthwiseService _healthwiseService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;

        public HealthwiseController(
            IHealthwiseService healthwiseService,
            IOrderService orderService,
            ILoggerProviderExtended loggerProvider)
        {
            _healthwiseService = healthwiseService;
            _orderService = orderService;
            _logger = loggerProvider.CreateLoggerDefault();
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
                toReturn.Add(new HealthwiseCustomerListItemModel(periodGroup.Periods));
            }
            return toReturn;
		}

        [HttpGet]
        public async Task<Result<HealthwiseCustomerListItemModel>> GetHealthwisePeriod(int id)
        {
            var item = await _healthwiseService.GetVHealthwisePeriodAsync(id);
            HealthwiseCustomerListItemModel toReturn;
            if(item!=null)
            {
                toReturn = new HealthwiseCustomerListItemModel(new List<VHealthwisePeriod>() { item});
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
        public async Task<bool> MakeHealthwisePeriodPayment([FromBody]HealthwisePeriodMakePaymentModel model)
        {
            if (!Validate(model))
                return false;

            return await _healthwiseService.MakeHealthwisePeriodPaymentAsync(model.Id, model.Amount, model.Date, model.PayAsGC);
        }
    }
}