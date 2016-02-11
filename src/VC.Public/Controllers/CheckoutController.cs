﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Helpers;
using VC.Public.Models.Auth;
using VC.Public.Models.Cart;
using VC.Public.Models.Checkout;
using VC.Public.Models.Profile;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Users;
using System.Linq;
using System.Reflection;
using Microsoft.AspNet.Mvc.ModelBinding;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Validation.Models;

namespace VC.Public.Controllers
{
    public class CheckoutController : CheckoutControllerBase
	{
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _orderPaymentMethodConverter;
	    private readonly IDynamicMapper<AddressDynamic, Address> _addressConverter;
		private readonly ReferenceData _appInfrastructure;
		private readonly ICountryService _countryService;

		public CheckoutController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
		    ICustomerService customerService,
		    IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
		    IOrderService orderService, IProductService productService, IAppInfrastructureService infrastructureService,
		    IAuthorizationService authorizationService, ICheckoutService checkoutService,
		    IDynamicMapper<AddressDynamic, Address> addressConverter, IAppInfrastructureService appInfrastructureService,
		    IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> orderPaymentMethodConverter,
		    IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper, ICountryService countryService)
		    : base(
			    contextAccessor, customerService, infrastructureService, authorizationService, checkoutService, orderService,
			    skuMapper, productMapper)
	    {
		    _storefrontUserService = storefrontUserService;
		    _paymentMethodConverter = paymentMethodConverter;
	        _productService = productService;
	        _addressConverter = addressConverter;
		    _orderPaymentMethodConverter = orderPaymentMethodConverter;
			_countryService = countryService;
			_appInfrastructure = appInfrastructureService.Get();
	    }

	    private void PopulateCreditCardsLookup(IEnumerable<CustomerPaymentMethodDynamic> creditCards)
	    {
			ViewBag.CreditCards = creditCards.ToDictionary(x => x.Id,
					y =>
						_appInfrastructure.CreditCardTypes.Single(z => z.Key == (int)y.Data.CardType).Text + ", ending in " +
						((string)y.Data.CardNumber).Substring(((string)y.Data.CardNumber).Length - 4));
		}

		private void PopulateShippingAddressesLookup(IList<AddressDynamic> addresses)
		{
			ViewBag.ShippingAddresses = addresses.OrderBy(x=>(bool)x.Data.Default).ToDictionary(x => x.Id,
					y => $"{y.Data.FirstName} {y.Data.LastName} {y.Data.Address1}" + ((bool)y.Data.Default ? " (Default)" : ""));
		}

	    private string ResolveStateOrCounty(ICollection<Country> countries, AddressDynamic address)
	    {
			var target = countries.Single(x => x.Id == address.IdCountry);

			var stateOrCounty = address.IdState.HasValue ? target.States.Single(x => x.Id == address.IdState.Value).StateCode : address.County;

		    return stateOrCounty;
	    }

	    private List<KeyValuePair<string, string>> PopulateBillingAddressDetails(OrderPaymentMethodDynamic paymentMethod, ICollection<Country> countries, string email)
	    {
			var billingAddress = paymentMethod.Address;

			return new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>(string.Empty, _appInfrastructure.CreditCardTypes.Single(z => z.Key == (int)paymentMethod.Data.CardType).Text),
				new KeyValuePair<string, string>(string.Empty, paymentMethod.Data.CardNumber),
				new KeyValuePair<string, string>(string.Empty, $"{paymentMethod.Data.ExpDate.Month}/{paymentMethod.Data.ExpDate.Year % 2000}"),
				new KeyValuePair<string, string>(string.Empty, string.Empty),
				new KeyValuePair<string, string>(string.Empty, $"{billingAddress.Data.FirstName} {billingAddress.Data.LastName}"),
				new KeyValuePair<string, string>(string.Empty, billingAddress.SafeData.Company),
				new KeyValuePair<string, string>(string.Empty, billingAddress.Data.Address1),
				new KeyValuePair<string, string>(string.Empty, billingAddress.SafeData.Address2),
				new KeyValuePair<string, string>(string.Empty, $"{billingAddress.Data.City}, {ResolveStateOrCounty(countries, billingAddress)} {billingAddress.Data.Zip}"),
				new KeyValuePair<string, string>("Phone", string.Format("{0:(###) ###-#### x#####}", billingAddress.Data.Phone)),
				new KeyValuePair<string, string>("Email", email),
			};
		}

		private List<KeyValuePair<string, string>> PopulateShippingAddressDetails(AddressDynamic shippingAddress, ICollection<Country> countries)
		{
			return new List<KeyValuePair<string, string>>()
			{
				new KeyValuePair<string, string>(string.Empty, $"{shippingAddress.Data.FirstName} {shippingAddress.Data.LastName}"),
				new KeyValuePair<string, string>(string.Empty, shippingAddress.SafeData.Company),
				new KeyValuePair<string, string>(string.Empty, shippingAddress.Data.Address1),
				new KeyValuePair<string, string>(string.Empty, shippingAddress.SafeData.Address2),
				new KeyValuePair<string, string>(string.Empty, $"{shippingAddress.Data.City}, {ResolveStateOrCounty(countries, shippingAddress)} {shippingAddress.Data.Zip}"),
				new KeyValuePair<string, string>("Phone", string.Format("{0:(###) ###-#### x#####}", shippingAddress.Data.Phone)),
			};
		}

	    private async Task<CustomerCartOrder> PopulateReviewModel(ReviewOrderModel reviewOrderModel)
	    {
			await InitCartModelInternal(reviewOrderModel);

			var existingUid = Request.GetCartUid();
			var id = GetInternalCustomerId();
			var cart = await CheckoutService.GetOrCreateCart(existingUid, id);

			var countries = await _countryService.GetCountriesAsync();

			var paymentMethod = cart.Order.PaymentMethod;
			reviewOrderModel.BillToAddress = PopulateBillingAddressDetails(paymentMethod, countries, cart.Order.Customer.Email);

			var shippingAddress = cart.Order.ShippingAddress;
			reviewOrderModel.ShipToAddress = PopulateShippingAddressDetails(shippingAddress, countries);

		    return cart;
	    }

	    public async Task<IActionResult> Welcome( bool forgot = false)
	    {
			if (await CustomerLoggedIn())
			{
				return RedirectToAction("AddUpdateBillingAddress");
			}

			ViewBag.ForgotPassSuccess = forgot;

			return View(new LoginModel());
	    }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Welcome(LoginModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(model);
			}

			ApplicationUser user = null;
			try
			{
				user = await _storefrontUserService.SignInAsync(model.Email, model.Password);
			}
			catch (WholesalePendingException e)
			{
				return Redirect("/content/wholesale-review");
			}
			if (user == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
			}

			return RedirectToAction("AddUpdateBillingAddress");
		}

	    [HttpGet]
		[CustomerAuthorize]
	    public async Task<IActionResult> GetBillingAddress(int id)
	    {
	        if (await CustomerLoggedIn())
	        {
	            var currentCustomer = await GetCurrentCustomerDynamic();

	            var creditCard = currentCustomer.CustomerPaymentMethods
	                .Single(p => p.IdObjectType == (int) PaymentMethodType.CreditCard && p.Id == id);

	            var billingInfoModel = _addressConverter.ToModel<AddUpdateBillingAddressModel>(creditCard.Address);
	            _paymentMethodConverter.UpdateModel<BillingInfoModel>(billingInfoModel, creditCard);

	            billingInfoModel.Email = currentCustomer.Email;

	            return PartialView("_AddUpdateBillingAddress", billingInfoModel);
	        }
	        return PartialView("_AddUpdateBillingAddress", null);
	    }

        [HttpGet]
        public async Task<IActionResult> AddUpdateBillingAddress()
        {
            if (await IsCartEmpty())
            {
                return View("EmptyCart");
            }

            var addUpdateModel = new AddUpdateBillingAddressModel();
            if (await CustomerLoggedIn())
            {
                var existingUid = Request.GetCartUid();
                var cart = await CheckoutService.GetOrCreateCart(existingUid, GetInternalCustomerId());

                var currentCustomer = await GetCurrentCustomerDynamic();

                var creditCards = currentCustomer.CustomerPaymentMethods
                    .Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard).ToList();

                var firstCreditCard = creditCards.FirstOrDefault();
                if (firstCreditCard != null)
                {
                    if (cart.Order.PaymentMethod?.Address == null || cart.Order.PaymentMethod.Id == 0)
                    {
                        _addressConverter.UpdateModel(addUpdateModel, firstCreditCard.Address);
                        _paymentMethodConverter.UpdateModel<BillingInfoModel>(addUpdateModel, firstCreditCard);
                    }
                    else
                    {
                        _addressConverter.UpdateModel(addUpdateModel, cart.Order.PaymentMethod.Address);
                        _orderPaymentMethodConverter.UpdateModel<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                    }

                    PopulateCreditCardsLookup(creditCards);
                }
                else
                {
                    if (cart.Order.PaymentMethod?.Address != null && cart.Order.PaymentMethod.Id != 0)
                    {
                        _addressConverter.UpdateModel(addUpdateModel, cart.Order.PaymentMethod.Address);
                        _orderPaymentMethodConverter.UpdateModel<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                    }
                }

                addUpdateModel.Email = currentCustomer.Email;

            }

            return View(addUpdateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUpdateBillingAddress(AddUpdateBillingAddressModel model)
        {
            if (await IsCartEmpty())
            {
                return View("EmptyCart");
            }
            var existingUid = Request.GetCartUid();
            var loggedIn = await CustomerLoggedIn();

            if (!ModelState.IsValid)
            {
                if (loggedIn)
                {
                    var currentCustomer = await GetCurrentCustomerDynamic();
                    var creditCards = currentCustomer.CustomerPaymentMethods
                        .Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard);

                    PopulateCreditCardsLookup(creditCards);
                }
                return View(model);
            }
            using (var transaction = CustomerService.BeginTransaction())
            {
                ApplicationUser user = null;
                try
                {
                    CustomerCartOrder cart;
                    if (loggedIn)
                    {
                        cart = await CheckoutService.GetOrCreateCart(existingUid, GetInternalCustomerId());
                        var currentCustomer = await GetCurrentCustomerDynamic();
                        var creditCards = currentCustomer.CustomerPaymentMethods
                            .Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard);

                        PopulateCreditCardsLookup(creditCards);
                    }
                    else
                    {
                        cart = await CheckoutService.GetOrCreateCart(existingUid);
                        if ((cart.Order.Customer?.Id ?? 0) == 0 || !model.GuestCheckout ||
                            cart.Order.Customer?.StatusCode != (int) CustomerStatus.PhoneOnly && model.GuestCheckout)
                        {
                            var newCustomer = await CreateAccount(model);
                            cart.Order.Customer = newCustomer;
                            if (!model.GuestCheckout)
                            {
                                user = await _storefrontUserService.SignInAsync(cart.Order.Customer.Email, model.Password);
                                if (user == null)
                                {
                                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
                                }
                            }
                            else if (model.GuestCheckout && ((bool?) cart.Order.Customer?.SafeData.Guest ?? false))
                            {
                                await _storefrontUserService.SendActivationAsync(cart.Order.Customer.Email);
                                var appUser = await CustomerService.GetUser(cart.Order.Customer.Id);

                                user = await _storefrontUserService.SignInNoStatusCheckingAsync(appUser);
                                if (user == null)
                                {
                                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
                                }
                            }
                            else
                            {
                                return RedirectToAction("Welcome");
                            }
                        }
                    }
                    if (model.SendCatalog)
                    {
                        var pnc = await _productService.GetSkuOrderedAsync("pnc");
                        if (pnc != null)
                        {
                            pnc.Quantity = 1;
                            pnc.Amount = pnc.Sku.Price;
                            cart.Order.Skus.AddKeyed(Enumerable.Repeat(pnc, 1), ordered => ordered.Sku.Code);
                        }
                    }
                    else
                    {
                        cart.Order.Skus.RemoveAll(s => s.Sku.Code.ToLower() == "pnc");
                    }
                    if (cart.Order.PaymentMethod?.Address == null || cart.Order.PaymentMethod.Id == 0)
                    {
                        cart.Order.PaymentMethod = _orderPaymentMethodConverter.FromModel((BillingInfoModel) model);
                        cart.Order.PaymentMethod.Address = _addressConverter.FromModel(model);
                        cart.Order.PaymentMethod.Address.IdObjectType = (int)AddressType.Billing;
                        cart.Order.PaymentMethod.IdObjectType = (int)PaymentMethodType.CreditCard;
                    }
                    else
                    {
                        _orderPaymentMethodConverter.UpdateObject((BillingInfoModel) model, cart.Order.PaymentMethod);
                        cart.Order.PaymentMethod.IdObjectType = (int)PaymentMethodType.CreditCard;
                        if (cart.Order.PaymentMethod.Address == null)
                        {
                            cart.Order.PaymentMethod.Address = new AddressDynamic {IdObjectType = (int) AddressType.Billing};
                        }
                        _addressConverter.UpdateObject(model, cart.Order.PaymentMethod.Address);
                        cart.Order.PaymentMethod.Address.IdObjectType = (int)AddressType.Billing;
                    }
                    if (await CheckoutService.UpdateCart(cart))
                    {
                        return RedirectToAction("AddUpdateShippingMethod");
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    if (user != null && user.Id != 0)
                    {
                        await _storefrontUserService.SignOutAsync(user);
                        await _storefrontUserService.DeleteAsync(user);
                    }
                    throw;
                }
            }

            return View(model);
        }

        private async Task<CustomerDynamic> CreateAccount(AddUpdateBillingAddressModel model)
        {
            var newCustomer = await CustomerService.CreatePrototypeAsync((int)CustomerType.Retail);
            newCustomer.IdDefaultPaymentMethod = (int)PaymentMethodType.CreditCard;
            if (model.GuestCheckout)
            {
                newCustomer.Data.Guest = true;
                newCustomer.StatusCode = (int)CustomerStatus.PhoneOnly;
            }
            else
            {
                newCustomer.StatusCode = (int)CustomerStatus.Active;
            }
            newCustomer.Email = model.Email;
            newCustomer.PublicId = Guid.NewGuid();
            newCustomer.ProfileAddress = _addressConverter.FromModel(model);
            newCustomer.ProfileAddress.IdObjectType = (int)AddressType.Profile;
            var shipping = _addressConverter.FromModel(model);
            shipping.Data.Default = true;
            shipping.IdObjectType = (int)AddressType.Shipping;
            newCustomer.ShippingAddresses = new List<AddressDynamic> { shipping };
            newCustomer.ApprovedPaymentMethods = new List<int> { (int)PaymentMethodType.CreditCard };
            newCustomer =
                await CustomerService.InsertAsync(newCustomer, model.GuestCheckout ? null : model.Password);
            return newCustomer;
        }
        
        [HttpGet]
		[CustomerAuthorize]
		public async Task<IActionResult> GetShippingAddress(int id)
		{
			var currentCustomer = await GetCurrentCustomerDynamic();

			var shipping = currentCustomer.ShippingAddresses
				.Single(p => p.Id == id);

			var shippingModel = new AddUpdateShippingMethodModel();

			_addressConverter.UpdateModel<ShippingInfoModel>(shippingModel, shipping);

			return PartialView("_AddUpdateShippingMethod", shippingModel);
		}

		[HttpGet]
		[CustomerAuthorize]
		public async Task<IActionResult> AddUpdateShippingMethod()
		{
			if (await IsCartEmpty())
			{
				return View("EmptyCart");
			}

			var shippingMethodModel = new AddUpdateShippingMethodModel()
			{
				AddressType = CheckoutAddressType.Residental
			};
		    if (await CustomerLoggedIn())
		    {
		        var currentCustomer = await GetCurrentCustomerDynamic();

		        var shippingAddresses = currentCustomer.ShippingAddresses.ToList();

		        var defaultShipping = shippingAddresses.FirstOrDefault(x => x.Data.Default == true);
		        if (defaultShipping != null)
		        {
		            _addressConverter.UpdateModel<ShippingInfoModel>(shippingMethodModel, defaultShipping);

		            PopulateShippingAddressesLookup(shippingAddresses);
		        }
		    }
		    else
		    {
                return RedirectToAction("Welcome");
            }

			return View(shippingMethodModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddUpdateShippingMethod(AddUpdateShippingMethodModel model)
		{
		    if (model.UseBillingAddress)
		    {
		        HashSet<string> propertyNames = new HashSet<string>(typeof (AddressModel).GetRuntimeProperties().Select(p => p.Name));
		        ModelState.RemoveAll(pair => propertyNames.Contains(pair.Key));
		    }
		    if (await CustomerLoggedIn())
		    {
		        if (ModelState.IsValid)
		        {
		            var existingUid = Request.GetCartUid();
		            var cart = await CheckoutService.GetOrCreateCart(existingUid, GetInternalCustomerId());
		            if (cart.Order.ShippingAddress == null || cart.Order.ShippingAddress.Id == 0)
		            {
		                cart.Order.ShippingAddress = _addressConverter.FromModel((AddressModel) model);
		            }
		            else
		            {
		                _addressConverter.UpdateObject(model, cart.Order.ShippingAddress);
		            }
		            cart.Order.Data.GiftOrder = model.IsGiftOrder;
		            cart.Order.Data.GiftMessage = model.GiftMessage;
		            cart.Order.Data.DeliveryInstructions = model.DeliveryInstructions;
                    await OrderService.CalculateOrder(cart.Order);
		            if (await CheckoutService.UpdateCart(cart))
		            {
		                return RedirectToAction("ReviewOrder");
		            }
		        }

		        var currentCustomer = await GetCurrentCustomerDynamic();
		        var shippingAddresses = currentCustomer.ShippingAddresses.ToList();

		        PopulateShippingAddressesLookup(shippingAddresses);
		    }
		    else
		    {
                return RedirectToAction("Welcome");
            }

		    return View(model);
		}

		[HttpGet]
		[CustomerAuthorize]
		public async Task<IActionResult> ReviewOrder()
		{
			if (await IsCartEmpty())
			{
				return View("EmptyCart");
			}

			var reviewOrderModel = new ReviewOrderModel();

			await PopulateReviewModel(reviewOrderModel);

			return View(reviewOrderModel);
		}

	    [HttpPost]
	    [CustomerAuthorize]
	    public async Task<Result<string>> ReviewOrder([FromBody]ViewCartModel model)
	    {
			//todo: alex g please add logic to make order processed here. If error occurs just return new Result(false)

		    return Url.Action("Receipt", "Checkout");
	    }

	    [HttpGet]
		public async Task<IActionResult> Receipt()
		{
			var receiptModel = new ReceiptModel();

			var cart = await PopulateReviewModel(receiptModel);

		    receiptModel.OrderNumber = cart.Order.Id.ToString();
		    receiptModel.OrderDate = cart.Order.DateCreated;
		    receiptModel.Tax = cart.Order.TaxTotal;

			return View(receiptModel);
		}
	}
}