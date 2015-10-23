using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Newtonsoft.Json;
using VC.Public.ModelConverters;
using VC.Public.Models.Auth;
using VC.Public.Models.Profile;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;
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
		private readonly IDynamicToModelMapper<CustomerPaymentMethodDynamic> _paymentMethodConverter;

		public ProfileController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
			ICustomerService customerService, IDynamicToModelMapper<CustomerAddressDynamic> addressConverter,
			IDynamicToModelMapper<CustomerPaymentMethodDynamic> paymentMethodConverter)
		{
			_contextAccessor = contextAccessor;
			_storefrontUserService = storefrontUserService;
			_customerService = customerService;
			_addressConverter = addressConverter;
			_paymentMethodConverter = paymentMethodConverter;
		}

		private IActionResult RenderCreditCardView(CustomerDynamic currentCustomer, BillingInfoModel model)
		{
			var creditCards = new List<BillingInfoModel>();
			foreach (
				var creditCard in
					currentCustomer.CustomerPaymentMethods.Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard))
			{
				var billingInfoModel = _addressConverter.ToModel<BillingInfoModel>(creditCard.Address);
				_paymentMethodConverter.ToModel(creditCard, billingInfoModel);

				creditCards.Add(billingInfoModel);
			}

			ViewBag.CreditCards = null;
			if (creditCards.Any())
			{
				model = creditCards.First();
				ViewBag.CreditCards = JsonConvert.SerializeObject(creditCards, Formatting.None);
			}

			return View(model);
		}

		private IActionResult RenderShippingAddressView(CustomerDynamic currentCustomer, ShippingInfoModel model)
		{
			var shippingAddresses = new List<ShippingInfoModel>();
			foreach (
				var shipping in
					currentCustomer.Addresses.Where(p => p.IdObjectType == (int)AddressType.Shipping))
			{
				var shippingModel = _addressConverter.ToModel<ShippingInfoModel>(shipping);

				shippingAddresses.Add(shippingModel);
			}

			ViewBag.ShippingAddresses = null;
			if (shippingAddresses.Any())
			{
				model = shippingAddresses.First();
				ViewBag.ShippingAddresses = JsonConvert.SerializeObject(shippingAddresses, Formatting.None);
			}

			return View(model);
		}

		private int GetInternalCustomerId()
		{
			var context = _contextAccessor.HttpContext;
			var internalId = Convert.ToInt32(context.User.GetUserId());

			return internalId;
		}

		private async Task<CustomerDynamic> GetCurrentCustomerDynamic()
		{
			var internalId = GetInternalCustomerId();
			var customer = await _customerService.SelectAsync(internalId);
			if (customer == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
			}

			if (customer.StatusCode == (int) CustomerStatus.Suspended || customer.StatusCode == (int) CustomerStatus.Deleted)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
			}

			return customer;
		}

		private void CleanProfileEmailFields(ChangeProfileModel model)
		{
			model.ConfirmEmail = model.NewEmail = string.Empty;
			ModelState["NewEmail"] = new ModelState();
			ModelState["ConfirmEmail"] = new ModelState();
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
			var currentCustomer = await GetCurrentCustomerDynamic();

			var model =
				_addressConverter.ToModel<ChangeProfileModel>(
					currentCustomer.Addresses.Single(x => x.IdObjectType == (int) AddressType.Profile));

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeProfile(ChangeProfileModel model)
		{
			if (!ModelState.IsValid)
			{
				CleanProfileEmailFields(model);
				return View(model);
			}

			var customer = await GetCurrentCustomerDynamic();

			var profileAddress = customer.Addresses.Single(x => x.IdObjectType == (int) AddressType.Profile);

			var oldEmail = customer.Email;

			customer.Addresses.Remove(profileAddress);
			var newProfileAddress = _addressConverter.FromModel(model);
			newProfileAddress.IdObjectType = (int) AddressType.Profile;
			customer.Addresses.Add(newProfileAddress);
			customer.Email =
				newProfileAddress.Data.Email =
					!string.IsNullOrWhiteSpace(model.NewEmail) && !string.IsNullOrWhiteSpace(model.ConfirmEmail)
						? model.NewEmail
						: oldEmail;

			CleanProfileEmailFields(model);
			customer = await _customerService.UpdateAsync(customer);

			if (oldEmail != customer.Email)
			{
				var user = await _storefrontUserService.GetAsync(GetInternalCustomerId());
				if (user == null)
				{
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
				}
				await _storefrontUserService.RefreshSignInAsync(user);
			}

			return RedirectToAction("Index");
		}

		[HttpGet]
		public async Task<IActionResult> ChangeBillingInfo()
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			return RenderCreditCardView(currentCustomer, new BillingInfoModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeBillingInfo(BillingInfoModel model)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			if (!ModelState.IsValid)
			{
				return RenderCreditCardView(currentCustomer, model);
			}

			if (model.Id > 0)
			{
				var creditCardToUpdate = currentCustomer.CustomerPaymentMethods.Single(x => x.IdObjectType == (int)PaymentMethodType.CreditCard && x.Id == model.Id);
				currentCustomer.CustomerPaymentMethods.Remove(creditCardToUpdate);
			}

			var customerPaymentMethod = _paymentMethodConverter.FromModel(model);
			customerPaymentMethod.IdObjectType = (int) PaymentMethodType.CreditCard;

			customerPaymentMethod.Address = _addressConverter.FromModel(model);
			customerPaymentMethod.Address.IdObjectType = (int) AddressType.Billing;

			currentCustomer.CustomerPaymentMethods.Add(customerPaymentMethod);

			await _customerService.UpdateAsync(currentCustomer);

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<Result<bool>> DeleteBillingInfo(int id)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			var creditCardToDelete = currentCustomer.CustomerPaymentMethods.FirstOrDefault(x => x.IdObjectType == (int)PaymentMethodType.CreditCard && x.Id == id);
			if (creditCardToDelete == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

			creditCardToDelete.StatusCode = (int)RecordStatusCode.Deleted;

			await _customerService.UpdateAsync(currentCustomer);

			return true;
		}

		[HttpGet]
		public async Task<IActionResult> ChangeShippingInfo()
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			return RenderShippingAddressView(currentCustomer, new ShippingInfoModel());
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeShippingInfo(ShippingInfoModel model)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			if (!ModelState.IsValid)
			{
				return RenderShippingAddressView(currentCustomer, model);
			}

			if (model.Id > 0)
			{
				var shippingAddressToUpdate = currentCustomer.Addresses.Single(x => x.IdObjectType == (int)AddressType.Shipping && x.Id == model.Id);
				currentCustomer.Addresses.Remove(shippingAddressToUpdate);
			}

			var newAddress = _addressConverter.FromModel(model);
			newAddress.IdObjectType = (int)AddressType.Shipping;

			currentCustomer.Addresses.Add(newAddress);

			await _customerService.UpdateAsync(currentCustomer);

			return RedirectToAction("Index");
		}

		[HttpPost]
		public async Task<Result<bool>> DeleteShippingInfo(int id)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			var shippingAddressToDelete = currentCustomer.Addresses.FirstOrDefault(x => x.IdObjectType == (int)AddressType.Shipping && x.Id == id);
			if (shippingAddressToDelete == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

			shippingAddressToDelete.StatusCode = (int)RecordStatusCode.Deleted;

			await _customerService.UpdateAsync(currentCustomer);

			return true;
		}
	}
}