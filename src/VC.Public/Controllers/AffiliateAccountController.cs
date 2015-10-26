using System;
using System.Collections.Generic;
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
using VitalChoice.Interfaces.Services.Affiliates;

namespace VC.Public.Controllers
{
	[AllowAnonymous]
    public class AffiliateAccountController : BaseMvcController
	{
		private readonly IAffiliateUserService _userService;
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IAffiliateService _affiliateService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IDynamicToModelMapper<AffiliateDynamic> _affiliateMapper;

		public AffiliateAccountController(
            IAffiliateUserService userService,
            IHttpContextAccessor contextAccessor,
            IDynamicToModelMapper<AffiliateDynamic> affiliateMapper, 
            IAffiliateService affiliateService)
		{
			_userService = userService;
			_contextAccessor = contextAccessor;
            _affiliateMapper = affiliateMapper;
            _affiliateService = affiliateService;
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

			return RedirectToAction("Index", "AffiliateProfile");
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

			var affiliate = await _affiliateService.SelectAsync(user.Id);
			if (affiliate == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			if (affiliate.StatusCode == (int)CustomerStatus.Suspended || affiliate.StatusCode == (int)CustomerStatus.Deleted)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
			}

            affiliate.Email = model.Email;
            affiliate.StatusCode = (int) CustomerStatus.Active;

			await _affiliateService.UpdateAsync(affiliate, model.Password);

			await _userService.SendSuccessfulRegistration(model.Email, user.FirstName, user.LastName);

			return await Login(new LoginModel() {Email = model.Email, Password = model.Password}, string.Empty);
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

            ViewBag.SuccessSend = true;
            return View(new ForgotPasswordEmailModel());
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
    }
}