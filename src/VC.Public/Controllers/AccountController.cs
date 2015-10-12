using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.ModelConverters;
using VC.Public.Models.Auth;
using VitalChoice.Core.Base;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;

namespace VC.Public.Controllers
{
	[AllowAnonymous]
    public class AccountController : BaseMvcController
	{
		private readonly IStorefrontUserService _userService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly ICustomerService _customerService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IDynamicToModelMapper<CustomerDynamic> _customerMapper;

		public AccountController(IStorefrontUserService userService, IHttpContextAccessor contextAccessor, IDynamicToModelMapper<CustomerDynamic> customerMapper, ICustomerService customerService, IPaymentMethodService paymentMethodService)
		{
			_userService = userService;
			_contextAccessor = contextAccessor;
			_customerMapper = customerMapper;
			_customerService = customerService;
			_paymentMethodService = paymentMethodService;
		}

		[HttpGet]
        public IActionResult Login(string alreadyTakenEmail = null)
        {
			if (!string.IsNullOrWhiteSpace(alreadyTakenEmail))
			{
				ViewBag.AlreadyTakenEmail = alreadyTakenEmail;
			}

            return View(new LoginModel());
        }

	    [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model, string returnUrl)
	    {
            if (!Validate(model))
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

			customer.Email = model.Email;

			await _customerService.UpdateAsync(customer, model.Password);

			await _userService.SendSuccessfulRegistration(model.Email, user.FirstName, user.LastName);

			return await Login(new LoginModel() {Email = model.Email, Password = model.Password}, string.Empty);
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

			item.IdObjectType = (int)CustomerType.Retail;
			item.PublicId = Guid.NewGuid();
			item.IdDefaultPaymentMethod = (await _paymentMethodService.GetStorefrontDefaultPaymentMenthod()).Id;

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
	}
}