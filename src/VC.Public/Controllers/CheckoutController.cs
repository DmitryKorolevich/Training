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
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Public.Controllers
{
    public class CheckoutController : PublicControllerBase
    {
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
	    private readonly ICheckoutService checkoutService;
	    private readonly IOrderService _orderService;
		private readonly ICheckoutService _checkoutService;

		public CheckoutController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
            ICustomerService customerService, IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
			IOrderService orderService, IProductService productService, IAppInfrastructureService infrastructureService, IAuthorizationService authorizationService, ICheckoutService checkoutService) :base(contextAccessor, customerService, infrastructureService, authorizationService, checkoutService)
        {
            _storefrontUserService = storefrontUserService;
            _paymentMethodConverter = paymentMethodConverter;
            _orderService = orderService;
            _productService = productService;
			_checkoutService = checkoutService;
        }

	    public async Task<IActionResult> Welcome(bool forgot = false)
	    {
			ViewBag.ForgotPassSuccess = forgot;

			return View(new LoginModel());
	    }

		[HttpGet]
		public async Task<IActionResult> AddUpdateBillingAddress()
		{
			if (await IsCartEmpty())
			{
				return View("EmptyCart");
			}

			if (!await CustomerLoggedIn())
			{
				return RedirectToAction("Welcome");
			}

			var billingInfoModel = new AddUpdateBillingAddressModel()
			{
				SendNews = false,
				SendCatalog = true
			};
			if (ContextAccessor.HttpContext.User.Identity.IsAuthenticated)
			{
				var currentCustomer = await GetCurrentCustomerDynamic();

				//todo: populate model
			}

			return View(billingInfoModel);
		}

		[HttpGet]
		public async Task<IActionResult> AddUpdateShippingMethod()
		{
			var shippingMethodModel = new AddUpdateShippingMethodModel()
			{
				AddressType = CheckoutAddressType.Residental
			};
			if (ContextAccessor.HttpContext.User.Identity.IsAuthenticated)
			{
				var currentCustomer = await GetCurrentCustomerDynamic();

				//todo: populate model
			}

			ViewBag.ShippingAddresses = null;

			return View(shippingMethodModel);
		}

		[HttpGet]
		public async Task<IActionResult> AddPaymentMethod()
		{
			var shippingMethodModel = new AddPaymentMethodModel()
			{

			};
			if (ContextAccessor.HttpContext.User.Identity.IsAuthenticated)
			{
				var currentCustomer = await GetCurrentCustomerDynamic();

				//todo: populate model
			}

			ViewBag.ShippingAddresses = null;

			return View(shippingMethodModel);
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