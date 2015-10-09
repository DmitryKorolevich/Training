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
		private readonly IDynamicToModelMapper<CustomerDynamic> _customerMapper;

		public AccountController(IStorefrontUserService userService, IHttpContextAccessor contextAccessor, IDynamicToModelMapper<CustomerDynamic> customerMapper, ICustomerService customerService)
		{
			_userService = userService;
			_contextAccessor = contextAccessor;
			_customerMapper = customerMapper;
			_customerService = customerService;
		}

		[HttpGet]
        public IActionResult Login()
        {
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

		    if (string.IsNullOrWhiteSpace(returnUrl))
		    {
			    returnUrl = ""; //todo: refactor
		    }

			return RedirectToAction("Index", "Profile");
	    }

		[HttpPost]
		public async Task<bool> Logout()
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

			return true;
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

			user.Email = model.Email;
			user.IsConfirmed = true;
			user.Status = UserStatus.Active;

			await _userService.UpdateAsync(user, null, model.Password);

			return await Login(new LoginModel() {Email = model.Email, Password = model.Password}, string.Empty);
		}

		[HttpGet]
		public IActionResult RegisterEmail()
		{
			return View(new RegisterEmailModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult RegisterEmail(RegisterEmailModel model)
		{
			if (!ModelState.IsValid)
				return View(model);

			return View("RegisterAccount", new RegisterAccountModel() { Email = model.Email});
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
			item.IdDefaultPaymentMethod = 1; //todo

			item = await _customerService.InsertAsync(item);
			if (item == null || item.Id == 0)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			return await Login(new LoginModel() { Email = model.Email, Password = model.Password }, string.Empty);
		}
	}
}