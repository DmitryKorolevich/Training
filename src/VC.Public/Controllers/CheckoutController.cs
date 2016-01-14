using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Auth;
using VC.Public.Models.Cart;
using VC.Public.Models.Checkout;
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
    public class CheckoutController : PublicControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly ICustomerService _customerService;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;

        public CheckoutController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
            ICustomerService customerService, IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
			IOrderService orderService, IProductService productService):base(contextAccessor, customerService)
        {
            _storefrontUserService = storefrontUserService;
            _paymentMethodConverter = paymentMethodConverter;
            _orderService = orderService;
            _productService = productService;
        }

	    public async Task<IActionResult> Welcome(bool forgot = false)
	    {
			ViewBag.ForgotPassSuccess = forgot;

			return View(new LoginModel());
	    }

		[HttpGet]
		public async Task<IActionResult> AddUpdateBillingAddress()
		{
			var billingInfoModel = new AddUpdateBillingAddress();
			if (ContextAccessor.HttpContext.User.Identity.IsAuthenticated)
			{
				var currentCustomer = await GetCurrentCustomerDynamic();

				//todo: populate model
			}

			return View(billingInfoModel);
		}
	}
}