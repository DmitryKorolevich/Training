using System;
using System.Collections.Generic;
using System.Globalization;
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
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
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
		private readonly IDynamicMapper<AddressDynamic, Address> _addressConverter;
		private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
		private readonly IProductService _productService;
		private readonly IOrderService _orderService;

		public ProfileController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
			ICustomerService customerService, IDynamicMapper<AddressDynamic, Address> addressConverter,
            IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter, IOrderService orderService, IProductService productService)
		{
			_contextAccessor = contextAccessor;
			_storefrontUserService = storefrontUserService;
			_customerService = customerService;
			_addressConverter = addressConverter;
			_paymentMethodConverter = paymentMethodConverter;
			_orderService = orderService;
			_productService = productService;
		}

		private async Task<PagedListEx<OrderHistoryItemModel>> PopulateHistoryModel(ShortOrderFilter filter)
		{
			var internalId = GetInternalCustomerId();

			filter.IdCustomer = internalId;
			filter.Sorting.SortOrder = SortOrder.Desc;
			filter.Sorting.Path = OrderSortPath.OrderDate;


			var orders = await _orderService.GetShortOrdersAsync(filter);

			var ordersModel = new PagedListEx<OrderHistoryItemModel>
			{
				Items = orders.Items.Select(p => new OrderHistoryItemModel()
				{
					DateCreated = p.DateCreated,
					Total = p.Total,
					Id = p.Id,
					OrderStatus = p.OrderStatus
				}).ToList(),
				Count = orders.Count,
				Index = filter.Paging.PageIndex
			};

			return ordersModel;
		}

		private BillingInfoModel PopulateCreditCard(CustomerDynamic currentCustomer, int selectedId = 0)
		{
			var creditCards = new List<BillingInfoModel>();
			foreach (
				var creditCard in
					currentCustomer.CustomerPaymentMethods.Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard))
			{
				var billingInfoModel = _addressConverter.ToModel<BillingInfoModel>(creditCard.Address);
				_paymentMethodConverter.UpdateModel(billingInfoModel, creditCard);

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
					currentCustomer.ShippingAddresses.Where(p => p.IdObjectType == (int)AddressType.Shipping))
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
			ModelState["NewEmail"] = new ModelStateEntry();
			ModelState["ConfirmEmail"] = new ModelStateEntry();
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
					currentCustomer.ShippingAddresses.Single(x => x.IdObjectType == (int) AddressType.Profile));

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

			customer.ShippingAddresses = customer.ShippingAddresses.Where(x => x.IdObjectType != (int)AddressType.Profile).ToList();
			var newProfileAddress = _addressConverter.FromModel(model);
			newProfileAddress.IdObjectType = (int) AddressType.Profile;
			customer.ShippingAddresses.Add(newProfileAddress);
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
					customer.ShippingAddresses.Single(x => x.IdObjectType == (int)AddressType.Profile && x.StatusCode != (int)RecordStatusCode.Deleted));

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

			currentCustomer.ShippingAddresses = currentCustomer.ShippingAddresses.Where(x=>x.Id != creditCardToDelete.Address.Id).ToList();
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
				var shippingAddressToUpdate = currentCustomer.ShippingAddresses.Single(x => x.IdObjectType == (int)AddressType.Shipping && x.Id == model.Id);
				currentCustomer.ShippingAddresses.Remove(shippingAddressToUpdate);
			}

			if (model.Default)
			{
				var otherAddresses = currentCustomer.ShippingAddresses.Where(x => x.IdObjectType == (int)AddressType.Shipping);
				foreach (var otherAddress in otherAddresses)
				{
					otherAddress.Data.Default = false;
				}
			}

			var newAddress = _addressConverter.FromModel(model);
			newAddress.IdObjectType = (int)AddressType.Shipping;

			currentCustomer.ShippingAddresses.Add(newAddress);

			currentCustomer = await _customerService.UpdateAsync(currentCustomer);

			ViewBag.SuccessMessage = model.Id > 0
				? InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated]
				: InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];

			if (model.Id == 0 )
			{
				ModelState["Id"].RawValue = model.Id = currentCustomer.ShippingAddresses.Last(x => x.IdObjectType == (int) AddressType.Shipping).Id;
			}

			return View(PopulateShippingAddress(currentCustomer, model.Id));
		}

		[HttpPost]
		public async Task<Result<bool>> DeleteShippingInfo(int id)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			var shippingAddressToDelete = currentCustomer.ShippingAddresses.FirstOrDefault(x => x.IdObjectType == (int)AddressType.Shipping && x.Id == id);
			if (shippingAddressToDelete == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

			currentCustomer.ShippingAddresses.Remove(shippingAddressToDelete);

			await _customerService.UpdateAsync(currentCustomer);

			return true;
		}

		[HttpPost]
		public async Task<Result<bool>> SetDefaultShippingInfo(int id)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			var found = false;
			var addresses = currentCustomer.ShippingAddresses.Where(x => x.IdObjectType == (int)AddressType.Shipping);
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

		[HttpGet]
		public async Task<IActionResult> LastOrderPlaced()
		{
			var internalId = GetInternalCustomerId();

			var lastOrder = await _orderService.SelectLastOrderAsync(internalId);

			var lines = new List<LastOrderLineModel>();

			if (lastOrder != null)
			{
				foreach (var skuOrdered in lastOrder.Skus)
				{
					var lineModel = new LastOrderLineModel()
					{
						ProductUrl = skuOrdered.ProductWithoutSkus.Url,
						IconLink = skuOrdered.ProductWithoutSkus.Data.Thumbnail,
						ProductName = skuOrdered.ProductWithoutSkus.Name,
						PortionsCount = skuOrdered.Sku.Data.QTY,
						Quantity = skuOrdered.Quantity,
						SelectedPrice = skuOrdered.Amount.ToString("C2"),
						SkuCode = skuOrdered.Sku.Code
					};

					lines.Add(lineModel);
				}
			}

			return View(lines);
		}

		[HttpGet]
		public async Task<IActionResult> TopFavoriteItems(bool all = false)
		{
			var internalId = GetInternalCustomerId();

			var filter = new VCustomerFavoritesFilter()
			{
				IdCustomer = internalId,
				Paging = new Paging()
				{
					PageIndex = 0,
					PageItemCount = all ? ProductConstants.MAX_FAVORITES_COUNT : ProductConstants.DEFAULT_FAVORITES_COUNT
				}
			};

			var favorites = await _productService.GetCustomerFavoritesAsync(filter);

			var model = favorites.Items.Select(favorite => new FavoriteModel()
			{
                //TODO - add reading Url from Infrastructure
                ProductName = favorite.ProductName, ProductThumbnail = favorite.ProductThumbnail, Url = String.Empty
            }).ToList();

			ViewBag.MoreExist = !all && favorites.Count > filter.Paging.PageItemCount;

			return View(model);
		}

		[HttpGet]
		public async Task<IActionResult> OrderHistory()
		{
			var filter = new ShortOrderFilter();

			return View(await PopulateHistoryModel(filter));
		}

		public async Task<IActionResult> RefreshOrderHistory(ShortOrderFilter filter)
		{
			var model = await PopulateHistoryModel(filter);

			return PartialView("_OrderHistoryGrid", model);
		}
	}
}