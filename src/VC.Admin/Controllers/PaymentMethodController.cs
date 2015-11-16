using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Validation.Models;
using VC.Admin.Models.PaymentMethod;
using VitalChoice.Core.Base;
using VitalChoice.Infrastructure.Domain.Transfer.PaymentMethod;
using VitalChoice.Interfaces.Services.Payments;

namespace VC.Admin.Controllers
{
	[SuperAdminAuthorize]
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