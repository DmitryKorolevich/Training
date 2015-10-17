using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.ModelConverters;
using VC.Public.Models.Auth;
using VC.Public.Models.Profile;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
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
	[CustomerAuthorize]
    public class ProfileController : BaseMvcController
	{
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly IStorefrontUserService _storefrontUserService;
		private readonly ICustomerService _customerService;
		private readonly IDynamicToModelMapper<CustomerAddressDynamic> _addressConverter;

		public ProfileController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService, ICustomerService customerService, IDynamicToModelMapper<CustomerAddressDynamic> addressConverter)
		{
			_contextAccessor = contextAccessor;
			_storefrontUserService = storefrontUserService;
			_customerService = customerService;
			_addressConverter = addressConverter;
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

		[HttpGet]
		public async Task<IActionResult> ChangeProfile()
		{
			var context = _contextAccessor.HttpContext;

			var currentCustomer = await _customerService.SelectAsync(Convert.ToInt32(context.User.GetUserId()));
			if (currentCustomer == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			var model = _addressConverter.ToModel<ChangeProfileModel>(currentCustomer.Addresses.Single(x => x.IdObjectType == (int)AddressType.Profile));

			return View(model);

			//return View(new ChangeProfileModel()
			//{
			//	Email = currentCustomer.Email,
			//	Address1 = profileAddress.Data.Address1,
			//	Address2 = profileAddress.Data.Address2,
			//	FirstName = profileAddress.Data.FirstName,
			//	LastName = profileAddress.Data.LastName,
			//	City = profileAddress.Data.City,
			//	Company = profileAddress.Data.Company,
			//	Fax = profileAddress.Data.Fax,
			//	Phone = profileAddress.Data.Phone,
			//	County = profileAddress.Data.County,
			//	IdState = profileAddress.Data.IdState,
			//	IdCountry = profileAddress.Data.IdCountry,
			//	PostalCode = profileAddress.Data.Zip
			//});
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeProfile(ChangeProfileModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			var context = _contextAccessor.HttpContext;

			var internalId = Convert.ToInt32(context.User.GetUserId());
            var customer = await _customerService.SelectAsync(internalId);
			if (customer == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			if (customer.StatusCode == (int)CustomerStatus.Suspended || customer.StatusCode == (int)CustomerStatus.Deleted)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
			}

			var profileAddress = customer.Addresses.Single(x => x.IdObjectType == (int)AddressType.Profile);

			var oldEmail = customer.Email;
			
			customer.Addresses.Remove(profileAddress);
			customer.Addresses.Add(_addressConverter.FromModel(model));

			customer =  await _customerService.UpdateAsync(customer);

			if (oldEmail != customer.Email)
			{
				var user = await _storefrontUserService.GetAsync(internalId);
                if (user == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
				}
				await _storefrontUserService.RefreshSignInAsync(user);
			}

			return RedirectToAction("Index");
		}
	}
}