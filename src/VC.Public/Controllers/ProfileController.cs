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

		private BillingInfoModel PopulateCreditCard(CustomerDynamic currentCustomer, int selectedId = 0)
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

			BillingInfoModel model = null;
			if (creditCards.Any())
			{
				model = selectedId > 0 ? creditCards.Single(x=>x.Id == selectedId) : creditCards.First();
				ViewBag.CreditCards = JsonConvert.SerializeObject(creditCards, Formatting.None);
			}
			else
			{
				model = new BillingInfoModel();
			}

			return model;
		}

		private ShippingInfoModel PopulateShippingAddress(CustomerDynamic currentCustomer, int selectedId = 0)
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
			ShippingInfoModel model;
            if (shippingAddresses.Any())
			{
				model = selectedId > 0 ? shippingAddresses.Single(x => x.Id == selectedId) : shippingAddresses.First(x => x.Default);
				ViewBag.ShippingAddresses = JsonConvert.SerializeObject(shippingAddresses, Formatting.None);
			}
			else
            {
	            model = new ShippingInfoModel();
            }

			return model;
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

			customer.IdEditedBy = null;

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

			ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated];

			return View(new ChangePasswordModel());
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

			var oldEmail = customer.Email;

			customer.Addresses = customer.Addresses.Where(x => x.IdObjectType != (int)AddressType.Profile).ToList();
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

			ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated];

			model =
				_addressConverter.ToModel<ChangeProfileModel>(
					customer.Addresses.Single(x => x.IdObjectType == (int)AddressType.Profile && x.StatusCode != (int)RecordStatusCode.Deleted));

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> ChangeBillingInfo()
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			return View(PopulateCreditCard(currentCustomer));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeBillingInfo(BillingInfoModel model)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			if (!ModelState.IsValid)
			{
				PopulateCreditCard(currentCustomer, model.Id);
				return View(model);
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

			currentCustomer = await _customerService.UpdateAsync(currentCustomer);

			ViewBag.SuccessMessage = model.Id > 0
				? InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated]
				: InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];

			if (model.Id == 0)
			{
				ModelState["Id"].RawValue = model.Id = currentCustomer.CustomerPaymentMethods.Last(x => x.IdObjectType == (int) PaymentMethodType.CreditCard).Id;
			}

			return View(PopulateCreditCard(currentCustomer, model.Id));
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

			currentCustomer.Addresses = currentCustomer.Addresses.Where(x=>x.Id != creditCardToDelete.Address.Id).ToList();
			currentCustomer.CustomerPaymentMethods.Remove(creditCardToDelete);

			await _customerService.UpdateAsync(currentCustomer);

			return true;
		}

		[HttpGet]
		public async Task<IActionResult> ChangeShippingInfo()
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			return View(PopulateShippingAddress(currentCustomer));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeShippingInfo([FromForm]ShippingInfoModel model)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			if (!ModelState.IsValid)
			{
				PopulateShippingAddress(currentCustomer, model.Id);
				return View(model);
			}

			if (model.Id > 0)
			{
				var shippingAddressToUpdate = currentCustomer.Addresses.Single(x => x.IdObjectType == (int)AddressType.Shipping && x.Id == model.Id);
				currentCustomer.Addresses.Remove(shippingAddressToUpdate);
			}

			if (model.Default)
			{
				var otherAddresses = currentCustomer.Addresses.Where(x => x.IdObjectType == (int)AddressType.Shipping);
				foreach (var otherAddress in otherAddresses)
				{
					otherAddress.Data.Default = false;
				}
			}

			var newAddress = _addressConverter.FromModel(model);
			newAddress.IdObjectType = (int)AddressType.Shipping;

			currentCustomer.Addresses.Add(newAddress);

			currentCustomer = await _customerService.UpdateAsync(currentCustomer);

			ViewBag.SuccessMessage = model.Id > 0
				? InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated]
				: InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];

			if (model.Id == 0 )
			{
				ModelState["Id"].RawValue = model.Id = currentCustomer.Addresses.Last(x => x.IdObjectType == (int) AddressType.Shipping).Id;
			}

			return View(PopulateShippingAddress(currentCustomer, model.Id));
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

			currentCustomer.Addresses.Remove(shippingAddressToDelete);

			await _customerService.UpdateAsync(currentCustomer);

			return true;
		}

		[HttpPost]
		public async Task<Result<bool>> SetDefaultShippingInfo(int id)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			var found = false;
			var addresses = currentCustomer.Addresses.Where(x => x.IdObjectType == (int)AddressType.Shipping);
			foreach (var address in addresses)
			{
				if (address.Id == id)
				{
					address.Data.Default = true;
					found = true;
				}
				else
				{
					address.Data.Default = false;
				}
			}

			if (!found)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

			await _customerService.UpdateAsync(currentCustomer);

			return true;
		}
	}
}