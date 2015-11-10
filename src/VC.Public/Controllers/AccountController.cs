using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.ModelConverters;
using VC.Public.Models.Auth;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;
using VitalChoice.Interfaces.Services.Affiliates;
using Microsoft.AspNet.Authorization;

namespace VC.Public.Controllers
{
	[AllowAnonymous]
    public class AccountController : BaseMvcController
	{
		private readonly IStorefrontUserService _userService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly ICustomerService _customerService;
        private readonly IAffiliateService _affiliateService;
        private readonly IPaymentMethodService _paymentMethodService;
		private readonly IDynamicMapper<CustomerDynamic, Customer> _customerMapper;

		public AccountController(
            IStorefrontUserService userService,
            IHttpContextAccessor contextAccessor, 
            IDynamicMapper<CustomerDynamic, Customer> customerMapper, 
            ICustomerService customerService,
            IAffiliateService affiliateService,
            IPaymentMethodService paymentMethodService)
		{
			_userService = userService;
			_contextAccessor = contextAccessor;
			_customerMapper = customerMapper;
			_customerService = customerService;
            _affiliateService = affiliateService;
            _paymentMethodService = paymentMethodService;
		}

        [HttpGet]
        public IActionResult Login(string alreadyTakenEmail = null, bool forgot = false)
        {
            if (!string.IsNullOrWhiteSpace(alreadyTakenEmail))
            {
                ViewBag.AlreadyTakenEmail = alreadyTakenEmail;
            }
            ViewBag.ForgotPassSuccess = forgot;

            return View(new LoginModel());
        }

	    [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model, string returnUrl)
	    {
			if (!ModelState.IsValid)
				return View(model);

            var user = await _userService.SignInAsync(model.Email, model.Password);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
			}

		    if (!string.IsNullOrWhiteSpace(returnUrl))
		    {
			    return Redirect(returnUrl);
		    }

			return RedirectToAction("Index", "Profile");
	    }

		public async Task<IActionResult> Logout()
		{
			var context = _contextAccessor.HttpContext;

			if (context.User.Identity.IsAuthenticated)
			{
				var user = await _userService.FindAsync(context.User.GetUserName());
				if (user == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
				}

				await _userService.SignOutAsync(user);
			}

			return RedirectToAction("Index", "Home");
		}

		[HttpGet]
		public async Task<IActionResult> Activate(Guid id)
		{
			var result = await _userService.GetByTokenAsync(id);
			if (result == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken]);
			}
			if (result.IsConfirmed)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserAlreadyConfirmed]);
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
		public IActionResult RegisterEmail()
		{
			return View(new RegisterEmailModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterEmail(RegisterEmailModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var validated = await _userService.ValidateEmailUniquenessAsync(model.Email);
			if (!validated)
			{
				return RedirectToAction("Login", new { alreadyTakenEmail = model.Email });
			}

			return View("RegisterAccount", new RegisterAccountModel() { Email = model.Email });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> RegisterAccount(RegisterAccountModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			var item = _customerMapper.FromModel(model);
            var cookies = Request.Cookies[AffiliateConstants.AffiliatePublicIdParam];
            if (!String.IsNullOrEmpty(cookies))
            {
                int idAffiliate = 0;
                if (Int32.TryParse(cookies, out idAffiliate))
                {
                    var affiliate = await _affiliateService.SelectAsync(idAffiliate);
                    if(affiliate!=null)
                    {
                        item.IdAffiliate = affiliate.Id;
                    }
                }
            }

            item.IdObjectType = (int)CustomerType.Retail;
			item.PublicId = Guid.NewGuid();
			item.StatusCode = (int) CustomerStatus.Active;

			var defaultPaymentMethod = await _paymentMethodService.GetStorefrontDefaultPaymentMenthod();
            item.IdDefaultPaymentMethod = defaultPaymentMethod.Id;
			item.ApprovedPaymentMethods = new List<int> {defaultPaymentMethod.Id};
			item.IdEditedBy = null;

			item = await _customerService.InsertAsync(item, model.Password);
			if (item == null || item.Id == 0)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			await _userService.SendSuccessfulRegistration(model.Email, model.FirstName, model.LastName);

			return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, string.Empty);
		}

		[HttpGet]
		public async Task<IActionResult> ResetPassword(Guid id)
		{
			var result = await _userService.GetByTokenAsync(id);
			if (result == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUserByActivationToken]);
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

			await _userService.ResetPasswordAsync(model.Email, model.Token, model.Password);

			var user = await _userService.FindAsync(model.Email);
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			await _userService.SignInAsync(user);

			return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, string.Empty);
		}

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordEmailModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordEmailModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userService.FindAsync(model.Email);
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

            await _userService.SendForgotPasswordAsync(user.PublicId);

            return RedirectToAction("Login",new { forgot = true });
        }

		[HttpGet]
		public async Task<IActionResult> LoginAsCustomer(Guid id)
		{
			var result = await _userService.GetByTokenAsync(id);
			if (result == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
			}

			result.ConfirmationToken = Guid.Empty;
			await _userService.UpdateAsync(result);

			result = await _userService.SignInAsync(result);
			if (result == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
			}

			return RedirectToAction("ChangeProfile", "Profile");
		}
	}
}