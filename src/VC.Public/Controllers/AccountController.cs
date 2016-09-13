using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Public.Models.Auth;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Interfaces.Services.Affiliates;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Business.Mailings;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Core.Services;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Validation.Models;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;
using VitalChoice.Interfaces.Services;

namespace VC.Public.Controllers
{
	[AllowAnonymous]
    public class AccountController : PublicControllerBase
	{
		private readonly IStorefrontUserService _userService;
		private readonly ICustomerService _customerService;
        private readonly IAffiliateService _affiliateService;
        private readonly IPaymentMethodService _paymentMethodService;
		private readonly IDynamicMapper<CustomerDynamic, Customer> _customerMapper;
        private readonly INotificationService _notificationService;
        private readonly IOrderSchedulerService _orderSchedulerService;
	    private readonly ExtendedUserManager _userManager;
	    private readonly ITokenService _tokenService;
        private readonly IObjectEncryptionHost _encryptionHost;


        public AccountController(
	        IStorefrontUserService userService,
	        IDynamicMapper<CustomerDynamic, Customer> customerMapper,
	        ICustomerService customerService,
	        IAffiliateService affiliateService,
	        IPaymentMethodService paymentMethodService,
	        INotificationService notificationService,
	        IOrderSchedulerService orderSchedulerService,
	        ExtendedUserManager userManager, IAuthorizationService authorizationService, ICheckoutService checkoutService,
	        ReferenceData referenceData, ITokenService tokenService, IObjectEncryptionHost encryptionHost) : base(customerService, authorizationService, checkoutService, userManager, referenceData)
	    {
	        _userService = userService;
	        _customerMapper = customerMapper;
	        _customerService = customerService;
	        _affiliateService = affiliateService;
	        _paymentMethodService = paymentMethodService;
	        _notificationService = notificationService;
	        _orderSchedulerService = orderSchedulerService;
	        _userManager = userManager;
	        _tokenService = tokenService;
	        _encryptionHost = encryptionHost;
	    }

	    [HttpGet]
        public async Task<Result<bool>> TestReviewEmail(int id)
        {
            await _orderSchedulerService.SendOrderProductReviewEmailTest(id);
            return true;
        }

        [HttpGet]
        public IActionResult Login(string alreadyTakenEmail = null, bool forgot = false, int? type=null, string returnUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(alreadyTakenEmail))
            {
                ViewBag.AlreadyTakenEmail = alreadyTakenEmail;
            }
            ViewBag.ForgotPassSuccess = forgot;
            ViewBag.Type = type;
	        ViewBag.ReturnUrl = returnUrl;

			return View(new LoginModel());
        }

	    [HttpPost]
		[CustomValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model, [FromQuery]string returnUrl)
	    {
			ViewBag.ReturnUrl = returnUrl;
			if (!ModelState.IsValid)
		    {
				return View("Login", model);
			}

            ApplicationUser user;
	        try
	        {
	            user = await CustomerPasswordLogin(model);
	            var cartUid = HttpContext.GetCartUid();
	            if (cartUid == null || await CheckoutService.GetCartItemsCount(cartUid.Value) == 0)
	            {
	                var cart = await CheckoutService.GetOrCreateCart(null, user.Id);
	                HttpContext.SetCartUid(cart.CartUid);
	            }
	        }
	        catch (WholesalePendingException)
	        {
	            return Redirect("/content/wholesale-review");
	        }
	        catch (AppValidationException e)
	        {
	            ModelState.AddModelError(string.Empty, e.Messages.First().Message);
	            return View("Login", model);
	        }
	        if (user == null)
			{
				ModelState.AddModelError(string.Empty,ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
				return View("Login", model);
			}

		    if (!string.IsNullOrWhiteSpace(returnUrl))
		    {
			    return Redirect(returnUrl);
		    }

			return RedirectToAction("Index", "Profile");
	    }

	    private async Task<ApplicationUser> CustomerPasswordLogin(LoginModel model)
	    {
	        var id = await _customerService.TryGetActiveIdByEmailAsync(model.Email);
	        if (!id.HasValue)
	        {
	            id = await _customerService.TryGetNotActiveIdByEmailAsync(model.Email);
	        }
	        if (!id.HasValue)
	        {
	            throw new AppValidationException(
	                ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.IncorrectUserPassword]);
	        }
	        var user = await _userService.SignInAsync(id.Value, model.Password);
	        await HttpContext.SpinAuthorizationToken(_tokenService, null, user, _encryptionHost);
	        return user;
	    }

	    [HttpPost]
	    public async Task<Result<string>> CheckEmail(object model, string email)
	    {
	        var id = await _customerService.TryGetActiveIdByEmailAsync(email);
	        if (id.HasValue)
	        {
	            var user = await _userService.FindAsync(id.Value);
	            return user != null ? $"/account/login?alreadytakenemail={email}&type=2" : null; //2 - wholesale
	        }
	        return null;
	    }

	    public async Task<IActionResult> Logout()
	    {
	        var context = HttpContext;

	        if (context.User.Identity.IsAuthenticated)
	        {
                int id;
                ApplicationUser user = null;
                if (int.TryParse(_userManager.GetUserId(context.User), out id))
	            {
	                user = await _userService.FindAsync(id);
	            }
	            if (user == null)
	            {
	                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser])
	                {
	                    ViewName = "Login"
	                };
	            }

	            await _userService.SignOutAsync(user);
	            await HttpContext.RemoveAuthorizationToken();
	        }

	        return RedirectToAction("Index", "Home");
	    }

	    [HttpGet]
		public async Task<IActionResult> Activate(Guid id)
		{
            ApplicationUser result;
            try
            {
                result = await _userService.GetByTokenAsync(id);
            }
            catch (AppValidationException e)
            {
                e.ViewName = "Login";
                throw;
            }
            if (result == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken])
                {
                    ViewName = "Login"
                };
			}
			if (result.IsConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserAlreadyConfirmed])
                {
                    ViewName = "Login"
                };
			}

			return View(new CreateAccountModel()
			{
				Email = result.Email,
				PublicId = result.PublicId
			});
		}

		[HttpPost]
		[CustomValidateAntiForgeryToken]
		public async Task<IActionResult> Activate(CreateAccountModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var user = await _userService.GetAsync(model.PublicId);
			if (user == null)
			{ 
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			var customer = await _customerService.SelectAsync(user.Id);
			if (customer == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			if (customer.StatusCode == (int)CustomerStatus.Suspended || customer.StatusCode == (int)CustomerStatus.Deleted)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
			}

			customer.StatusCode = (int) CustomerStatus.Active;
			customer.IdEditedBy = null;

			await _customerService.UpdateAsync(customer, model.Password);

			await _userService.SendSuccessfulRegistration(customer.Email, user.FirstName, user.LastName);

			return await Login(new LoginModel() {Email = customer.Email, Password = model.Password}, string.Empty);
		}

		[HttpGet]
		public IActionResult RegisterEmail(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View(new RegisterEmailModel());
		}

		[HttpPost]
		[CustomValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterEmail(RegisterEmailModel model, string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;

			if (!ModelState.IsValid)
				return View(model);

			bool validated;
			try
			{
				validated = await _userService.ValidateEmailUniquenessAsync(model.Email);
			}
			catch (AppValidationException e)
			{
				ModelState.AddModelError(string.Empty, e.Messages.First().Message);
				return View(model);
			}
			
			if (!validated)
			{
				return RedirectToAction("Login", new { alreadyTakenEmail = model.Email, returnUrl = returnUrl });
			}

			return View("RegisterAccount", new RegisterAccountModel() { Email = model.Email });
		}

		[HttpPost]
		[CustomValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterAccount(RegisterAccountModel model, string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;

			if (!ModelState.IsValid)
				return View(model);

			var customer = await _customerMapper.FromModelAsync(model, (int)CustomerType.Retail);
            var cookies = Request.Cookies[AffiliateConstants.AffiliatePublicIdParam];
		    if (!String.IsNullOrEmpty(cookies))
		    {
		        int idAffiliate = 0;
		        if (Int32.TryParse(cookies, out idAffiliate))
		        {
		            var affiliate = await _affiliateService.SelectAsync(idAffiliate);
		            if (affiliate != null)
		            {
		                customer.IdAffiliate = affiliate.Id;
		            }
		        }
		    }
			customer.PublicId = Guid.NewGuid();
			customer.StatusCode = (int) CustomerStatus.Active;

			var defaultPaymentMethod = await _paymentMethodService.GetStorefrontDefaultPaymentMethod();
            customer.IdDefaultPaymentMethod = defaultPaymentMethod.Id;
			customer.ApprovedPaymentMethods = new List<int> {defaultPaymentMethod.Id};
			customer.IdEditedBy = null;

			try
			{
				customer = await _customerService.InsertAsync(customer, model.Password);
			}
			catch (AppValidationException e)
			{
				ModelState.AddModelError(string.Empty, e.Messages.First().Message);
				return View(model);
			}

			if (customer == null || customer.Id == 0)
			{
				ModelState.AddModelError(string.Empty, ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
				return View(model);
			}

			await _userService.SendSuccessfulRegistration(customer.Email, model.FirstName, model.LastName);

			return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, returnUrl);
		}

        [HttpPost]
        [CustomValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterWholesaleAccount(RegisterWholesaleAccountModel model)
        {
            if (!model.IsAllowAgreement)
            {
                ModelState.AddModelError(String.Empty, "Please agree to the terms and conditions.");
            }
            if (!Validate(model))
            {
                return PartialView("_RegisterWholesaleAccount", model);
            }

            var customer = await _customerMapper.FromModelAsync(model, (int)CustomerType.Wholesale);
            
            customer.PublicId = Guid.NewGuid();
            customer.StatusCode = (int)CustomerStatus.Pending;
            customer.Data.InceptionDate = DateTime.Now;
            customer.Data.TaxExempt = TaxExempt.SalesTaxWillBePaid;

            var defaultPaymentMethod = await _paymentMethodService.GetStorefrontDefaultPaymentMethod();
            customer.IdDefaultPaymentMethod = defaultPaymentMethod.Id;
            customer.ApprovedPaymentMethods = new List<int> { defaultPaymentMethod.Id };
            customer.IdEditedBy = null;

            try
            {
                customer = await _customerService.InsertAsync(customer, model.Password);
            }
            catch(AppValidationException e)
            {
                foreach (var message in e.Messages)
                {
                    ModelState.AddModelError(message.Field, message.Message);
                }
                return PartialView("_RegisterWholesaleAccount", model);
            }
            if (customer == null || customer.Id == 0)
            {
                ModelState.AddModelError(String.Empty, ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
                return PartialView("_RegisterWholesaleAccount", model);
            }

            await _userService.SendWholesaleSuccessfulRegistration(model.Email, model.FirstName, model.LastName);

            ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];

            return PartialView("_RegisterWholesaleAccount", model);
        }

        [HttpGet]
		public async Task<IActionResult> ResetPassword(Guid id)
		{
            ApplicationUser result;
            try
            {
                result = await _userService.GetByTokenAsync(id);
            }
            catch (AppValidationException e)
            {
                e.ViewName = "Login";
                throw;
            }
            if (result == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken])
                {
                    ViewName = "Login"
                };
			}

			return View(new ResetPasswordModel()
			{
				Token = id.ToString(),
			});
		}

		[HttpPost]
		[CustomValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
		{
			if (!ModelState.IsValid)
				return View(model);
		    var id = await _customerService.TryGetActiveIdByEmailAsync(model.Email);
            ApplicationUser user = null;
		    if (!id.HasValue)
		    {
		        id = await _customerService.TryGetPhoneOnlyIdByEmailAsync(model.Email);
		    }
            if (id.HasValue)
		    {
		        user = await _userService.FindAsync(id.Value);
		    }
		    if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserResetPassword]);
			}

			if (!user.IsConfirmed && user.Status == UserStatus.NotActive)
				// the case when guest checkout user wants to activate himself
			{
				await _customerService.ActivateGuestAsync(id.Value, model.Token, model.Password);
            }
			else
			{
				await _userService.ResetPasswordAsync(id.Value, model.Token, model.Password);
            }

            user = await _userService.SignInAsync(await _userService.FindAsync(id.Value));
            await HttpContext.SpinAuthorizationToken(_tokenService, null, user, _encryptionHost);

            return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, string.Empty);
		}

        [HttpGet]
        public IActionResult ForgotPassword(string returnUrl)
        {
			ViewBag.ReturnUrl = returnUrl;
			return View(new ForgotPasswordEmailModel());
        }

        [HttpPost]
        [CustomValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordEmailModel model, string returnUrl)
        {
			ViewBag.ReturnUrl = returnUrl;

			if (!ModelState.IsValid)
                return View(model);

            var id = await _customerService.TryGetActiveIdByEmailAsync(model.Email);
            ApplicationUser user = null;
            if (id.HasValue)
            {
                user = await _userService.FindAsync(id.Value);
            }
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

	        try
	        {
		        await _userService.SendForgotPasswordAsync(user.PublicId);
                await _userService.SignOutAsync(user);
            }
			catch (AppValidationException e)
			{
				ModelState.AddModelError(string.Empty, e.Messages.First().Message);
				return View(model);
			}

            if (!string.IsNullOrWhiteSpace(returnUrl))
			{
				return Redirect(returnUrl);
			}

			return RedirectToAction("Login",new { forgot = true, returnUrl = returnUrl });
        }

		[HttpGet]
		public async Task<IActionResult> LoginAsCustomer(Guid id)
		{
            ApplicationUser result;
            try
            {
                result = await _userService.GetByTokenAsync(id);
            }
            catch (AppValidationException e)
            {
                e.ViewName = "Login";
                throw;
            }
            if (result == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin])
                {
                    ViewName = "Login"
                };
			}

			result.ConfirmationToken = Guid.Empty;
		    try
		    {
		        await _userService.UpdateAsync(result);

		        result = await _userService.SignInAsync(result);
		    }
		    catch (AppValidationException e)
		    {
                e.ViewName = "Login";
                throw;
            }
		    if (result == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn])
                {
                    ViewName = "Login"
                };
			}

			return RedirectToAction("ChangeProfile", "Profile");
		}

	    [HttpGet]
	    public async Task<IActionResult> Unsubscribe([FromQuery]string email, [FromQuery]int type)
	    {
            CustomerFilter filter =new CustomerFilter();
	        filter.Email = email;
	        var customerExist = (await _customerService.GetCustomersAsync(filter))!=null;
	        var blockedEmailExist = await _notificationService.IsEmailUnsubscribedAsync(type, email);
	        if (!customerExist || blockedEmailExist)
	        {
	            return Redirect("/content/unsubscribe-email-not-found");
	        }
            await _notificationService.UpdateUnsubscribeEmailAsync(type, email, true);
            return Redirect("/content/email-unsubscribed");
        }
	}
}