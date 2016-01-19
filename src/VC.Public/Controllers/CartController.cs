using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Cart;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Users;

namespace VC.Public.Controllers
{
    public class CartController : BaseMvcController
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly ICustomerService _customerService;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public CartController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
            ICustomerService customerService, IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
			IOrderService orderService, IProductService productService)
        {
            _contextAccessor = contextAccessor;
            _storefrontUserService = storefrontUserService;
            _customerService = customerService;
            _paymentMethodConverter = paymentMethodConverter;
            _orderService = orderService;
            _productService = productService;
        }

	    public async Task<IActionResult> ViewCart()
	    {
			var cartModel = new ViewCartModel()
			{
				Skus = new List<CartSkuModel>()
				{
					new CartSkuModel()
					{
						ProductPageUrl = "url",
						Code = "CWR712P",
						DisplayName = "Wild Traditional Canned Sockeye Salmon 7.5 oz. - Easy Open (12)",
						IconUrl = "/Assets/images/cart/NRT501_alabcore_pouched_30z_218.jpg",
						Price= 72,
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
				GiftCertificateCodes = new List<string>() { "" },
				OrderTotal = 169,
				ShipAsap = true,
				ShippingCost =	0,
				SubTotal = 144,

			};

		    return View(cartModel);
	    }
    }
}