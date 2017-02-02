using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Checkout;
using System.Linq;
using VitalChoice.Core.Services;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Infrastructure.Services;

namespace VC.Public.Controllers
{
    public abstract class PublicControllerBase : BaseMvcController
    {
        protected readonly IAuthorizationService AuthorizationService;
        protected readonly ICustomerService CustomerService;
        protected readonly ICheckoutService CheckoutService;
        private readonly ExtendedUserManager _userManager;
        protected readonly ReferenceData ReferenceData;
        protected static readonly ObjectSemaphore<Guid> CartLocks = new ObjectSemaphore<Guid>();

        protected PublicControllerBase(ICustomerService customerService, IAuthorizationService authorizationService,
            ICheckoutService checkoutService,
            ExtendedUserManager userManager, ReferenceData referenceData)
        {
            CheckoutService = checkoutService;
            _userManager = userManager;
            ReferenceData = referenceData;
            AuthorizationService = authorizationService;
            CustomerService = customerService;
        }

        protected async Task<bool> CustomerLoggedIn()
        {
            var context = HttpContext;
            var signedIn = await AuthorizationService.AuthorizeAsync(context.User, null, IdentityConstants.IdentityBasicProfile);
            if (signedIn)
            {
                if (ReferenceData.IsValidCustomer(context.User))
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
            return ReferenceData.HasRole(context.User, role);
        }

        protected int? GetInternalCustomerId()
        {
            var internalId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(internalId))
                return null;

            return int.Parse(internalId);
        }

        protected Task<IDisposable> LockCurrentCart(out Guid? cartUid)
        {
            var uid = HttpContext.GetCartUid();
            cartUid = uid;
            // ReSharper disable once PossibleInvalidOperationException
            return CartLocks.LockWhenAsync(() => uid.HasValue, () => uid.Value);
        }

        protected async Task<CustomerCartOrder> GetCurrentCart(bool? loggedIn = null)
        {
            if (loggedIn == null)
            {
                loggedIn = await CustomerLoggedIn();
            }
            var internalId = GetInternalCustomerId();
            if (loggedIn.Value && internalId.HasValue)
            {
                var existingUid = HttpContext.GetCartUid();
                var result = await CheckoutService.GetOrCreateCart(existingUid, internalId.Value);
                HttpContext.SetCartUid(result.CartUid);
                return result;
            }
            else
            {
                var existingUid = HttpContext.GetCartUid();
                var result = await CheckoutService.GetOrCreateCart(existingUid, false);
                HttpContext.SetCartUid(result.CartUid);
                return result;
            }
        }

        protected async Task<CustomerDynamic> GetCurrentCustomerDynamic()
        {
            var internalId = GetInternalCustomerId();
            if (!internalId.HasValue)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }
            var customer = await CustomerService.SelectAsync(internalId.Value);
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

        protected List<KeyValuePair<int, string>> PopulateCreditCardsLookup(CustomerDynamic currentCustomer,
            OrderDynamic currentOrder = null)
        {
            List<KeyValuePair<int, string>> creditCardsList = new List<KeyValuePair<int, string>>();

            var creditCards = currentCustomer.CustomerPaymentMethods.Cast<PaymentMethodDynamic>()
                .Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard);

            if (currentOrder?.PaymentMethod?.Id > 0 && currentOrder.PaymentMethod?.IdObjectType == (int) PaymentMethodType.CreditCard &&
                currentOrder.PaymentMethod?.Address?.IdCountry > 0)
            {
                creditCardsList.Add(new KeyValuePair<int, string>(0,
                    FormatCreditCardShortInfo(currentOrder.PaymentMethod, "(Currently On Order)")));
            }
            foreach (var creditCard in creditCards.OrderByDescending(a => (bool?) a.SafeData.Default ?? false))
            {
                if ((bool?) creditCard.SafeData.Default ?? false)
                {
                    creditCardsList.Add(new KeyValuePair<int, string>(creditCard.Id, FormatCreditCardShortInfo(creditCard, "(Default)")));
                }
                else
                {
                    creditCardsList.Add(new KeyValuePair<int, string>(creditCard.Id, FormatCreditCardShortInfo(creditCard)));
                }
            }

            return creditCardsList;
        }

        private string FormatCreditCardShortInfo(PaymentMethodDynamic y, string namePrefix = null)
        {
            var cardType = ReferenceData.CreditCardTypes.FirstOrDefault(z => y.SafeData.CardType != null &&
                                                                             z.Key == (int) y.Data.CardType)?.Text;
            var cardNumberPart = y.SafeData.CardNumber != null ? (string) y.Data.CardNumber : string.Empty;
            if (cardNumberPart.Length > 4)
            {
                cardNumberPart = cardNumberPart.Substring(cardNumberPart.Length - 4);
            }
            var defaultPart = !string.IsNullOrEmpty(namePrefix) ? namePrefix : string.Empty;
            var toReturn = $"{cardType}, ending in {cardNumberPart} {defaultPart}";

            return toReturn;
        }
    }
}