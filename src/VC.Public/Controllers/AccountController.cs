using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Auth;
using VitalChoice.Core.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Interfaces.Services.Affiliates;
using Microsoft.AspNet.Authorization;
using VitalChoice.Business.Mail;
using VitalChoice.Core.Services;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Validation.Models;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VC.Public.Controllers
{
	[AllowAnonymous]
    public class AccountController : BaseMvcController
	{
		private readonly IStorefrontUserService _userService;
		private readonly ICustomerService _customerService;
        private readonly IAffiliateService _affiliateService;
        private readonly IPaymentMethodService _paymentMethodService;
		private readonly IDynamicMapper<CustomerDynamic, Customer> _customerMapper;
        private readonly INotificationService _notificationService;

        public AccountController(
            IStorefrontUserService userService,
            IDynamicMapper<CustomerDynamic, Customer> customerMapper, 
            ICustomerService customerService,
            IAffiliateService affiliateService,
            IPaymentMethodService paymentMethodService,
            INotificationService notificationService,
            IPageResultService pageResultService) : base(pageResultService)
		{
			_userService = userService;
			_customerMapper = customerMapper;
			_customerService = customerService;
            _affiliateService = affiliateService;
            _paymentMethodService = paymentMethodService;
            _notificationService = notificationService;
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model, [FromQuery]string returnUrl)
	    {
			ViewBag.ReturnUrl = returnUrl;
			if (!ModelState.IsValid)
		    {
				return View(model);
			}

            ApplicationUser user = null;
		    try
		    {
			    user = await _userService.SignInAsync(model.Email, model.Password);
		    }
		    catch (WholesalePendingException)
		    {
			    return Redirect("/content/wholesale-review");
		    }
		    catch (AppValidationException e)
		    {
				ModelState.AddModelError(string.Empty, e.Messages.First().Message);
				return View(model);
			}
		    if (user == null)
			{
				ModelState.AddModelError(string.Empty,ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
				return View(model);
			}

		    if (!string.IsNullOrWhiteSpace(returnUrl))
		    {
			    return Redirect(returnUrl);
		    }

			return RedirectToAction("Index", "Profile");
	    }

        [HttpPost]
        public async Task<Result<string>> CheckEmail(object model, string email)
        {
            var user = await _userService.FindAsync(email);

            return user != null ? $"/account/login?alreadytakenemail={email}&type=2" : null;//2 - wholesale
        }

        public async Task<IActionResult> Logout()
		{
            var context = HttpContext;

			if (context.User.Identity.IsAuthenticated)
			{
				var user = await _userService.FindAsync(context.User.GetUserName());
				if (user == null)
				{
                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser])
                    {
                        ViewName = "Login"
                    };
				}

				await _userService.SignOutAsync(user);
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
		[ValidateAntiForgeryToken]
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
		[ValidateAntiForgeryToken]
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
		[ValidateAntiForgeryToken]
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

			await _userService.SendSuccessfulRegistration(model.Email, model.FirstName, model.LastName);

			return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, returnUrl);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var user = await _userService.FindAsync(model.Email);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			if (!user.IsConfirmed && user.Status == UserStatus.NotActive)
				// the case when guest checkout user wants to activate himself
			{
				await _customerService.ActivateGuestAsync(model.Email, model.Token, model.Password);
			}
			else
			{
				await _userService.ResetPasswordAsync(model.Email, model.Token, model.Password);
			}

			await _userService.SignInAsync(await _userService.FindAsync(model.Email));

			return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, string.Empty);
		}

        [HttpGet]
        public IActionResult ForgotPassword(string returnUrl)
        {
			ViewBag.ReturnUrl = returnUrl;
			return View(new ForgotPasswordEmailModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordEmailModel model, string returnUrl)
        {
			ViewBag.ReturnUrl = returnUrl;

			if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.FindAsync(model.Email);
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

	        try
	        {
		        await _userService.SendForgotPasswordAsync(user.PublicId);
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
            await _notificationService.UnsubscribeEmailAsync(type, email);
            return Redirect("/content/email-unsubscribed");
        }
	}
}