using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Validation.Models;
using Microsoft.AspNet.Hosting;
using System.Security.Claims;
using VC.Admin.Models.PaymentMethod;
using VC.Admin.Models.UserManagement;
using VC.Admin.Validators.UserManagement;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Transfer.PaymentMethod;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Payment;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Base;

namespace VC.Admin.Controllers
{
	[AdminAuthorize(PermissionType.Settings)]
    public class PaymentMethodController : BaseApiController
    {
		private readonly IPaymentMethodService _paymentMethodService;
		
		public PaymentMethodController(IPaymentMethodService paymentMethodService)
		{
			_paymentMethodService = paymentMethodService;
		}

		[HttpGet]
	    public async Task<Result<IList<PaymentMethodListItemModel>>> GetPaymentMethods()
		{
			var result =  await _paymentMethodService.GetApprovedPaymentMethodsAsync();

			return result.Select(x => new PaymentMethodListItemModel()
			{
				Id = x.Id,
				CustomerTypes = x.CustomerTypes?.Select(y=>y.IdCustomerType).ToList(),
				DateEdited = x.DateEdited,
				EditedBy = x.AdminProfile?.AgentId,
				Name = x.Name
			}).ToList();
		}

		[HttpPost]
		public async Task<Result<bool>> SetState([FromBody]IList<PaymentMethodsAvailability> paymentMethodsAvailability)
		{
			await _paymentMethodService.SetStateAsync(paymentMethodsAvailability);

			return true;
		}
    }
}