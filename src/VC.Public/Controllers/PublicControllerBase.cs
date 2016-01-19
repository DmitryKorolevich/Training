using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using VitalChoice.Core.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services.Customers;

namespace VC.Public.Controllers
{
	public abstract class PublicControllerBase : BaseMvcController
	{
		protected IHttpContextAccessor ContextAccessor { get; }
		protected ICustomerService CustomerService { get; }

		protected PublicControllerBase(IHttpContextAccessor contextAccessor, ICustomerService customerService)
		{
			ContextAccessor = contextAccessor;
			CustomerService = customerService;
		}

		protected int GetInternalCustomerId()
		{
			var context = ContextAccessor.HttpContext;
			var internalId = Convert.ToInt32(context.User.GetUserId());

			return internalId;
		}

		protected async Task<CustomerDynamic> GetCurrentCustomerDynamic()
		{
			var internalId = GetInternalCustomerId();
			var customer = await CustomerService.SelectAsync(internalId);
			if (customer == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			if (customer.StatusCode == (int)CustomerStatus.Suspended || customer.StatusCode == (int)CustomerStatus.Deleted)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
			}

			customer.IdEditedBy = null;

			return customer;
		}
	}
}