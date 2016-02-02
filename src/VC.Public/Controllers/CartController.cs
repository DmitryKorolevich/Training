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
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Validation.Models;

namespace VC.Public.Controllers
{
    public class CartController : PublicControllerBase
    {
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly ICustomerService _customerService;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
        private readonly ICheckoutService _checkoutService;
        private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
        private readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
        private readonly IDiscountService _discountService;
        private readonly IGcService _gcService;
        private readonly IOrderService _orderService;

        public CartController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
            ICustomerService customerService, IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
            IOrderService orderService, IProductService productService, ICheckoutService checkoutService,
            IAuthorizationService authorizationService, IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper,
            IDiscountService discountService, IGcService gcService)
            : base(contextAccessor, customerService, appInfrastructureService, authorizationService, checkoutService)
        {
            _storefrontUserService = storefrontUserService;
            _customerService = customerService;
            _paymentMethodConverter = paymentMethodConverter;
            _orderService = orderService;
            _productService = productService;
            _checkoutService = checkoutService;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
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

            if (!ModelState.IsValid)
            {
                model.ShippingDateError = ModelState["ShippingDate"].Errors.Select(x => x.ErrorMessage).FirstOrDefault();
                //return model;
            }
            else
            {
                model.ShippingDateError = string.Empty;
            }
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
                cart.Order.Data.ShipDelayDateP = model.ShippingDate;
                cart.Order.Data.ShipDelayDateNP = model.ShippingDate;
            }
            else
            {
                cart.Order.Data.ShipDelayType = ShipDelayType.None;
                cart.Order.Data.ShipDelayDateP = null;
                cart.Order.Data.ShipDelayDateNP = null;
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

        private async Task FillModel(ViewCartModel cartModel, CustomerCartOrder cart)
        {
            await Calculate(cartModel, cart.Order);
        }

        private async Task Calculate(ViewCartModel cartModel, OrderDynamic order)
        {
            var context = await _orderService.CalculateOrder(order);
            var gcMessages = context.GcMessageInfos.ToDictionary(m => m.Field);
            if (!string.IsNullOrWhiteSpace(cartModel.PromoCode) && order.Discount == null)
            {
                context.Messages.Add(new MessageInfo
                {
                    Field = "DiscountCode",
                    Message = "Discount not found"
                });
            }
            cartModel.FreeShipDifference = context.FreeShippingThresholdDifference;
            cartModel.DiscountDescription = context.Order?.Discount?.Description;
	        cartModel.DiscountMessage = context.DiscountMessage;
	        cartModel.Messages =
		        context.Messages?.Select(x => new KeyValuePair<string, string>(x.Field, x.Message)).ToList();
			cartModel.Skus.Clear();
            cartModel.Skus.AddRange(
                order.Skus?.Select(sku =>
                {
                    var result = _skuMapper.ToModel<CartSkuModel>(sku.Sku);
                    _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                    result.Price = sku.Amount;
                    result.Quantity = sku.Quantity;
                    result.SubTotal = sku.Quantity*sku.Amount;
					return result;
                }) ?? Enumerable.Empty<CartSkuModel>());
            var gcsInCart = cartModel.GiftCertificateCodes.ToArray();
            var hasEmpty = gcsInCart.Any(g => string.IsNullOrWhiteSpace(g));
            cartModel.GiftCertificateCodes.Clear();
            foreach (var code in gcsInCart)
            {
                if (!string.IsNullOrWhiteSpace(code.Value) && order.GiftCertificates.All(g => g.GiftCertificate.Code != code.Value))
                {
                    cartModel.GiftCertificateCodes.Add(code);
                    code.ErrorMessage = "Gift Certificate not Found";
                }
            }
            cartModel.GiftCertificateCodes.AddRange(
                order.GiftCertificates?.Select(g => g.GiftCertificate.Code).Select(x =>
                {
                    var message = gcMessages.ContainsKey(x) ?  gcMessages[x] : new MessageInfo();

                    return new CartGcModel
                    {
                        Value = x,
                        SuccessMessage = message.MessageLevel == MessageLevel.Info ? message.Message : string.Empty,
                        ErrorMessage = message.MessageLevel == MessageLevel.Error ? message.Message : string.Empty
                    };
                }) ??
                Enumerable.Empty<CartGcModel>());
            if (hasEmpty)
            {
                cartModel.GiftCertificateCodes.Add(new CartGcModel() {Value = string.Empty});
            }
            cartModel.ShippingUpgradeNPOptions = context.ShippingUpgradeNpOptions;
            cartModel.ShippingUpgradePOptions = context.ShippingUpgradePOptions;
            cartModel.DiscountTotal = -context.DiscountTotal;
            cartModel.GiftCertificatesTotal = context.GiftCertificatesSubtotal;
            cartModel.PromoSkus.Clear();
            cartModel.PromoSkus.AddRange(context.PromoSkus?.Select(sku =>
            {
                var result = _skuMapper.ToModel<CartSkuModel>(sku.Sku);
                _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity*sku.Amount;
				return result;
            }) ?? Enumerable.Empty<CartSkuModel>());
            cartModel.OrderTotal = order.Total;
            cartModel.PromoCode = order.Discount?.Code;
            cartModel.ShippingCost = order.ShippingTotal;
            cartModel.SubTotal = order.ProductsSubtotal;
            if (((ShipDelayType?) order.SafeData.ShipDelayType ?? ShipDelayType.None) != ShipDelayType.None)
            {
                cartModel.ShipAsap = false;
                cartModel.ShippingDate = order.SafeData.ShipDelayDateP;
            }
            else
            {
                cartModel.ShipAsap = true;
            }
            cartModel.ShippingUpgradeP = order.SafeData.ShippingUpgradeP;
            cartModel.ShippingUpgradeNP = order.SafeData.ShippingUpgradeNP;

	        if (!cartModel.GiftCertificateCodes.Any())
	        {
		        cartModel.GiftCertificateCodes.Add(new CartGcModel() { Value = string.Empty});
	        }
        }
    }
}