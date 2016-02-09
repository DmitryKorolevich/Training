using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Helpers;
using VC.Public.Models.Cart;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Validation.Models;

namespace VC.Public.Controllers
{
    public class CartController : CheckoutControllerBase
    {
	    private readonly IProductService _productService;
        private readonly ICheckoutService _checkoutService;
        private readonly IDiscountService _discountService;
        private readonly IGcService _gcService;
       

        public CartController(IHttpContextAccessor contextAccessor,
            ICustomerService customerService,
            IOrderService orderService, IProductService productService, ICheckoutService checkoutService,
            IAuthorizationService authorizationService, IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper,
            IDiscountService discountService, IGcService gcService)
            : base(contextAccessor, customerService, appInfrastructureService, authorizationService, checkoutService, orderService, skuMapper,productMapper)
        {
	        _productService = productService;
            _checkoutService = checkoutService;
            _discountService = discountService;
            _gcService = gcService;
        }

        [HttpGet]
        public async Task<Result<ViewCartModel>> InitCartModel()
        {
            return await InitCartModelInternal();
        }

        public async Task<IActionResult> ViewCart()
        {
	        if (await IsCartEmpty())
	        {
		        return View("EmptyCart");
	        }

	        var cartModel = await InitCartModelInternal();

            return View(cartModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCartView(string skuCode)
        {
            var cart = await AddToCart(skuCode);
            if (cart == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SkuNotFound]);
            }
            return PartialView("_CartLite", cart);
        }

        [HttpPost]
        public async Task<ViewCartModel> AddToCart(string skuCode)
        {
            var existingUid = Request.GetCartUid();
            var sku = await _productService.GetSkuOrderedAsync(skuCode);
            if (sku == null)
                return null;
            CustomerCartOrder cart;
            if (await CustomerLoggedIn())
            {
                var id = GetInternalCustomerId();
                cart = await _checkoutService.GetOrCreateCart(existingUid, id);
            }
            else
            {
                cart = await _checkoutService.GetOrCreateCart(existingUid);
            }
            cart.Order.Skus.AddUpdateKeyed(Enumerable.Repeat(sku, 1).ToArray(),
                ordered => ordered.Sku.Code, skuModel => skuModel.Sku.Code, skuModel =>
                {
                    var skuOrdered = _productService.GetSkuOrderedAsync(skuModel.Sku.Code).Result;
                    skuOrdered.Quantity = 1;
                    skuOrdered.Amount = HasRole(RoleType.Wholesale) ? skuModel.Sku.WholesalePrice : skuModel.Sku.Price;
                    return skuOrdered;
                },
                (ordered, skuModel) => ordered.Quantity += 1);
            SetCartUid(cart.CartUid);
            if (!await _checkoutService.UpdateCart(cart))
                return null;
            ViewCartModel result = new ViewCartModel();
            await FillModel(result, cart);
            SetCartUid(cart.CartUid);
            return result;
        }

        [HttpPost]
        public async Task<Result<ViewCartModel>> UpdateCart([FromBody] ViewCartModel model)
        {
	        if (model.ShipAsap && model.ShippingDate.HasValue)
	        {
		        model.ShippingDate = null;
		        ModelState.Clear();
	        }

            model.ShippingDateError = !ModelState.IsValid
                ? ModelState["ShippingDate"].Errors.Select(x => x.ErrorMessage).FirstOrDefault()
                : string.Empty;

            var existingUid = Request.GetCartUid();
            CustomerCartOrder cart;
            if (await CustomerLoggedIn())
            {
                var id = GetInternalCustomerId();
                cart = await _checkoutService.GetOrCreateCart(existingUid, id);
            }
            else
            {
                cart = await _checkoutService.GetOrCreateCart(existingUid);
            }
            cart.Order.Skus?.MergeKeyed(model.Skus.Where(s => s.Quantity > 0).ToArray(), ordered => ordered.Sku.Code,
                skuModel => skuModel.Code, skuModel =>
                {
                    var result = _productService.GetSkuOrderedAsync(skuModel.Code).Result;
                    result.Quantity = skuModel.Quantity;
                    return result;
                }, (ordered, skuModel) => ordered.Quantity = skuModel.Quantity);
            cart.Order.Discount = await _discountService.GetByCode(model.PromoCode);
            var gcCodes = model.GiftCertificateCodes.Select(x => x.Value).ToList();
            cart.Order.GiftCertificates?.MergeKeyed(
                gcCodes.Select(code => _gcService.GetGiftCertificateAsync(code).Result).Where(g => g != null).ToArray(),
                gc => gc.GiftCertificate?.Code, code => code.Code,
                code => new GiftCertificateInOrder
                {
                    GiftCertificate = code
                });
            if (!model.ShipAsap)
            {
                cart.Order.Data.ShipDelayType = ShipDelayType.EntireOrder;
                cart.Order.Data.ShipDelayDate = model.ShippingDate;
            }
            else
            {
                cart.Order.Data.ShipDelayType = ShipDelayType.None;
                cart.Order.Data.ShipDelayDate = null;
            }
            cart.Order.Data.ShippingUpgradeP = model.ShippingUpgradeP;
            cart.Order.Data.ShippingUpgradeNP = model.ShippingUpgradeNP;
            if (ModelState.IsValid)
            {
                if (!await _checkoutService.UpdateCart(cart))
                {
                    return new Result<ViewCartModel>(false, model);
                }
            }
            await FillModel(model, cart);
            SetCartUid(cart.CartUid); 
            return model;
        }

        private async Task<ViewCartModel> InitCartModelInternal()
        {
            var cartModel = await GetCart();

            if (!cartModel.GiftCertificateCodes.Any())
            {
                cartModel.GiftCertificateCodes.Add(new CartGcModel() {Value = string.Empty}); //needed to to force first input to appear
            }
            return cartModel;
        }

        private async Task<ViewCartModel> GetCart()
        {
            var existingUid = Request.GetCartUid();
            var cartModel = new ViewCartModel();
            if (await CustomerLoggedIn())
            {
                var id = GetInternalCustomerId();
                var cart = await _checkoutService.GetOrCreateCart(existingUid, id);
                await FillModel(cartModel, cart);
                SetCartUid(cart.CartUid);
                return cartModel;
            }
            else
            {
                var cart = await _checkoutService.GetOrCreateCart(existingUid);
                await FillModel(cartModel, cart);
                SetCartUid(cart.CartUid);
                return cartModel;
            }
        }

        private void SetCartUid(Guid uid)
        {
            Response.Cookies.Append(CheckoutConstants.CartUidCookieName, uid.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1)
            });
        }
    }
}