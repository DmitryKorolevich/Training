using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.ModelConverters;
using VC.Public.Models.Auth;
using VC.Public.Models.Profile;
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
	[Authorize]
    public class ProfileController : BaseMvcController
	{
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IStorefrontUserService _storefrontUserService;

		public ProfileController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService)
		{
			_contextAccessor = contextAccessor;
			_storefrontUserService = storefrontUserService;
		}

		public IActionResult Index()
        {
            return RedirectToAction("TopFavoriteItems");
        }
		
		[HttpGet]
		public IActionResult ChangePassword()
		{
			return View(new ChangePasswordModel());
		}

		public IActionResult TopFavoriteItems()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var context = _contextAccessor.HttpContext;

			var user = await _storefrontUserService.FindAsync(context.User.GetUserName());
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			await _storefrontUserService.UpdateWithPasswordChangeAsync(user, model.OldPassword, model.Password);

			return RedirectToAction("Index");
		}
	}
}