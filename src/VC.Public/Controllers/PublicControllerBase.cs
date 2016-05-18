﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using VC.Public.Helpers;
using VitalChoice.Core.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Core.Services;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Checkout;
using System.Linq;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Identity.UserManagers;

namespace VC.Public.Controllers
{
    public abstract class PublicControllerBase : BaseMvcController
    {
        protected readonly IAppInfrastructureService InfrastructureService;
        protected readonly IAuthorizationService AuthorizationService;
        protected readonly ICustomerService CustomerService;
        protected readonly ICheckoutService CheckoutService;
        private readonly ExtendedUserManager _userManager;

        protected PublicControllerBase(ICustomerService customerService,
            IAppInfrastructureService infrastructureService, IAuthorizationService authorizationService, ICheckoutService checkoutService,
            IPageResultService pageResultService, ExtendedUserManager userManager) : base(pageResultService)
        {
            CheckoutService = checkoutService;
            _userManager = userManager;
            InfrastructureService = infrastructureService;
            AuthorizationService = authorizationService;
			CustomerService = customerService;
        }

        protected async Task<bool> CustomerLoggedIn()
        {
            var context = HttpContext;
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

        protected bool HasRole(RoleType role)
        {
            if (role != RoleType.Retail && role != RoleType.Wholesale)
                return false;
            var context = HttpContext;
            return InfrastructureService.HasRole(context.User, role);
        }

        protected int GetInternalCustomerId()
        {
            var context = HttpContext;
            var internalId = Convert.ToInt32(_userManager.GetUserId(User));

            return internalId;
        }

        protected async Task<CustomerCartOrder> GetCurrentCart(bool? loggedIn = null)
        {
            if (loggedIn == null)
            {
                loggedIn = await CustomerLoggedIn();
            }
            if (loggedIn.Value)
            {
                var existingUid = Request.GetCartUid();
                return await CheckoutService.GetOrCreateCart(existingUid, GetInternalCustomerId());
            }
            else
            {
                var existingUid = Request.GetCartUid();
                return await CheckoutService.GetOrCreateCart(existingUid);
            }
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

		protected async Task PopulateCreditCardsLookup()
		{
			var currentCustomer = await GetCurrentCustomerDynamic();
		    var creditCards = currentCustomer.CustomerPaymentMethods
		        .Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard);

		    ViewBag.CreditCards = creditCards.ToDictionary(x => x.Id,
		        y =>
		            InfrastructureService.Data().CreditCardTypes.Single(z => z.Key == (int) y.Data.CardType).Text + ", ending in " +
		            ((string) y.Data.CardNumber).Substring(((string) y.Data.CardNumber).Length - 4) +
		            (y.SafeData.Default == true ? " (Default)" : ""));
		}
	}
}