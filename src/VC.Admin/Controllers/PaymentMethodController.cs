using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Validation.Models;
using VC.Admin.Models.PaymentMethod;
using VitalChoice.Core.Base;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.PaymentMethod;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Payments;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Settings)]
    public class PaymentMethodController : BaseApiController
    {
		private readonly IPaymentMethodService _paymentMethodService;
	    private readonly ExtendedUserManager _userManager;

	    public PaymentMethodController(IPaymentMethodService paymentMethodService, ExtendedUserManager userManager)
	    {
	        _paymentMethodService = paymentMethodService;
	        _userManager = userManager;
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
		    await _paymentMethodService.SetStateAsync(paymentMethodsAvailability, Convert.ToInt32(_userManager.GetUserId(User)));

			return true;
		}
    }
}