using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using VitalChoice.Core.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Identity;

namespace VC.Public.Controllers
{
    public abstract class PublicControllerBase : BaseMvcController
    {
        protected readonly IAppInfrastructureService InfrastructureService;
        protected readonly IAuthorizationService AuthorizationService;
        protected readonly IHttpContextAccessor ContextAccessor;
        protected readonly ICustomerService CustomerService;

        protected PublicControllerBase(IHttpContextAccessor contextAccessor, ICustomerService customerService,
            IAppInfrastructureService infrastructureService, IAuthorizationService authorizationService)
        {
            InfrastructureService = infrastructureService;
            AuthorizationService = authorizationService;
            ContextAccessor = contextAccessor;
            CustomerService = customerService;
        }

        protected async Task<bool> CustomerLoggenIn()
        {
            var context = ContextAccessor.HttpContext;
            var signedIn = await AuthorizationService.AuthorizeAsync(context.User, null, IdentityConstants.IdentityBasicProfile);
            if (signedIn)
            {
                if (InfrastructureService.IsValidCustomer(context.User))
                {
                    return true;
                }
            }
            return false;
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

            if (customer.StatusCode == (int) CustomerStatus.Suspended || customer.StatusCode == (int) CustomerStatus.Deleted)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
            }

            customer.IdEditedBy = null;

            return customer;
        }
    }
}