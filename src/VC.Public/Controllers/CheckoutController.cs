using System;
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

	    public CheckoutController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
		    ICustomerService customerService,
		    IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
		    IOrderService orderService, IProductService productService, IAppInfrastructureService infrastructureService,
		    IAuthorizationService authorizationService, ICheckoutService checkoutService,
		    IDynamicMapper<AddressDynamic, Address> addressConverter, IAppInfrastructureService appInfrastructureService,
		    IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> orderPaymentMethodConverter,
		    IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper)
		    : base(
			    contextAccessor, customerService, infrastructureService, authorizationService, checkoutService, orderService,
			    skuMapper, productMapper)
	    {
		    _storefrontUserService = storefrontUserService;
		    _paymentMethodConverter = paymentMethodConverter;
	        _productService = productService;
	        _addressConverter = addressConverter;
		    _orderPaymentMethodConverter = orderPaymentMethodConverter;
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
                        if ((cart.Order.Customer?.Id ?? 0) == 0 ||
                            cart.Order.Customer?.StatusCode != (int) CustomerStatus.NotActive && model.GuestCheckout)
                        {
                            var newCustomer = await CreateAccount(model);
                            cart.Order.Customer = newCustomer;
                            if (!model.GuestCheckout)
                            {
                                var user = await _storefrontUserService.SignInAsync(cart.Order.Customer.Email, model.Password);
                                if (user == null)
                                {
                                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
                                }
                            }
                        }
                        if (model.GuestCheckout && ((bool?) cart.Order.Customer?.SafeData.Guest ?? false))
                        {
                            var appUser = await CustomerService.GetUser(cart.Order.Customer.Id);

                            var user = await _storefrontUserService.SignInNoStatusCheckingAsync(appUser);
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
                    }
                    else
                    {
                        _orderPaymentMethodConverter.UpdateObject((BillingInfoModel) model, cart.Order.PaymentMethod);
                        if (cart.Order.PaymentMethod.Address == null)
                        {
                            cart.Order.PaymentMethod.Address = new AddressDynamic {IdObjectType = (int) AddressType.Billing};
                        }
                        _addressConverter.UpdateObject(model, cart.Order.PaymentMethod.Address);
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
                    throw;
                }
            }

            return View(model);
        }

        private async Task<CustomerDynamic> CreateAccount(AddUpdateBillingAddressModel model)
        {
            var newCustomer = await CustomerService.CreatePrototypeAsync((int)CustomerType.Retail);
            newCustomer.IdDefaultPaymentMethod = (int)PaymentMethodType.CreditCard;
            newCustomer.StatusCode = (int)CustomerStatus.Active;
            if (model.GuestCheckout)
            {
                newCustomer.Data.Guest = true;
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
	    public async Task<IActionResult> ReviewOrder()
		{
			var reviewOrderModel = new ReviewOrderModel()
			{
				Skus = new List<CartSkuModel>()
				{
					new CartSkuModel()
					{
						ProductPageUrl = "url",
						Code = "CWR712P",
						DisplayName = "Wild Traditional Canned Sockeye Salmon 7.5 oz. - Easy Open (12)",
						IconUrl = "/Assets/images/cart/NRT501_alabcore_pouched_30z_218.jpg",
						Price = 72,
						Quantity = 2,
						InStock = true,
						SubTotal = 144
					},
					new CartSkuModel()
					{
						ProductPageUrl = "url1",
						Code = "PNC",
						DisplayName = "Vital Choice Catalog",
						IconUrl = "/Assets/images/cart/seaweedsalad_218.jpg",
						Quantity = 1,
						InStock = true
					}
				},
				GiftCertificateCodes = new List<CartGcModel>() { new CartGcModel() { Value = string.Empty} },
				OrderTotal = 169,
				ShipAsap = true,
				ShippingCost = 0,
				SubTotal = 144,
				BillToAddress = new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(string.Empty, "first name last name"),
					new KeyValuePair<string, string>(string.Empty, "company name"),
					new KeyValuePair<string, string>(string.Empty, "address1"),
					new KeyValuePair<string, string>(string.Empty, "address2"),
					new KeyValuePair<string, string>(string.Empty, "city, AA 123456"),
					new KeyValuePair<string, string>("Phone", "923112312323"),
					new KeyValuePair<string, string>("Email", "mailforspam200@mailforspam.com"),
				},
				ShipToAddress = new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(string.Empty, "first name last name"),
					new KeyValuePair<string, string>(string.Empty, "company name"),
					new KeyValuePair<string, string>(string.Empty, "address1"),
					new KeyValuePair<string, string>(string.Empty, "address2"),
					new KeyValuePair<string, string>(string.Empty, "city, AA 123456"),
					new KeyValuePair<string, string>("Phone", "923112312323"),
				},
				PaymentDetails = new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>("Credit Card", "Visa"),
					new KeyValuePair<string, string>("Number", "XXXX-XXXX-XXXX-7890"),
					new KeyValuePair<string, string>("Expiration", "01/16")
				}
			};

			return View(reviewOrderModel);
		}

		[HttpGet]
		public async Task<IActionResult> Receipt()
		{
			var reviewOrderModel = new ReceiptModel()
			{
				Skus = new List<CartSkuModel>()
				{
					new CartSkuModel()
					{
						ProductPageUrl = "url",
						Code = "CWR712P",
						DisplayName = "Wild Traditional Canned Sockeye Salmon 7.5 oz. - Easy Open (12)",
						IconUrl = "/Assets/images/cart/NRT501_alabcore_pouched_30z_218.jpg",
						Price = 72,
						Quantity = 2,
						InStock = true,
						SubTotal = 144
					},
					new CartSkuModel()
					{
						ProductPageUrl = "url1",
						Code = "PNC",
						DisplayName = "Vital Choice Catalog",
						IconUrl = "/Assets/images/cart/seaweedsalad_218.jpg",
						Quantity = 1,
						InStock = true
					}
				},
				BillToAddress = new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(string.Empty, "first name last name"),
					new KeyValuePair<string, string>(string.Empty, "company name"),
					new KeyValuePair<string, string>(string.Empty, "address1"),
					new KeyValuePair<string, string>(string.Empty, "address2"),
					new KeyValuePair<string, string>(string.Empty, "city, AA 123456"),
					new KeyValuePair<string, string>("Phone", "923112312323"),
					new KeyValuePair<string, string>("Email", "mailforspam200@mailforspam.com"),
				},
				ShipToAddress = new List<KeyValuePair<string, string>>()
				{
					new KeyValuePair<string, string>(string.Empty, "first name last name"),
					new KeyValuePair<string, string>(string.Empty, "company name"),
					new KeyValuePair<string, string>(string.Empty, "address1"),
					new KeyValuePair<string, string>(string.Empty, "address2"),
					new KeyValuePair<string, string>(string.Empty, "city, AA 123456"),
					new KeyValuePair<string, string>("Phone", "923112312323"),
				},
				ShippingCost = 100,
				SubTotal = 50,
				Tax = 10,
				OrderTotal = 160,
				OrderNumber = "1051135",
				OrderDate = DateTime.Now,
				ShippingMethod = "Standard Shipping"
			};

			return View(reviewOrderModel);
		}
	}
}