using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using VC.Public.Helpers;
using VC.Public.Models.Cart;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
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
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Ecommerce.Utils;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
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
            IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper, IDiscountService discountService, IGcService gcService)
            : base(contextAccessor, customerService, appInfrastructureService, authorizationService)
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
            if (await CustomerLoggenIn())
            {
                var id = GetInternalCustomerId();
                var cart = await _checkoutService.GetOrCreateCart(existingUid, id);
                cart.Order.Skus.MergeUpdateKeyed(Enumerable.Repeat(sku, 1).ToArray(),
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
            else
            {
                var cart = await _checkoutService.GetOrCreateAnonymCart(existingUid);
                cart.Skus.MergeUpdateKeyed(Enumerable.Repeat(sku, 1).ToArray(), ordered => ordered.Sku.Code,
                    skuModel => skuModel.Sku.Code, skuModel =>
                    {
                        var skuOrdered = _productService.GetSkuOrderedAsync(skuModel.Sku.Code).Result;
                        skuOrdered.Quantity = 1;
                        skuOrdered.Amount = skuModel.Sku.Price;
                        return skuOrdered;
                    }, (ordered, skuModel) => ordered.Quantity += 1);
                SetCartUid(cart.CartUid);
                if (!await _checkoutService.UpdateCart(cart))
                    return null;
                ViewCartModel result = new ViewCartModel();
                await FillModel(result, cart);
                SetCartUid(cart.CartUid);
                return result;
            }
        }

        [HttpPost]
        public async Task<Result<ViewCartModel>> UpdateCart([FromBody]ViewCartModel model)
        {
            var existingUid = Request.GetCartUid();
            if (await CustomerLoggenIn())
            {
                var id = GetInternalCustomerId();
                var cart = await _checkoutService.GetOrCreateCart(existingUid, id);
                cart.Order.Skus.MergeUpdateWithDeleteKeyed(model.Skus, ordered => ordered.Sku.Code, skuModel => skuModel.Code, skuModel =>
                {
                    var result = _productService.GetSkuOrderedAsync(skuModel.Code).Result;
                    result.Quantity = skuModel.Quantity;
                    return result;
                }, (ordered, skuModel) => ordered.Quantity = skuModel.Quantity);
                cart.Order.Discount = await _discountService.GetByCode(model.PromoCode);
				var gcCodes = model.GiftCertificateCodes.Select(x => x.Value).ToList();
				cart.Order.GiftCertificates.MergeKeyed(gcCodes, gc => gc.GiftCertificate.Code, code => code,
                    code => new GiftCertificateInOrder
                    {
                        GiftCertificate = _gcService.GetGiftCertificateAsync(code).Result
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
                await _checkoutService.UpdateCart(cart);
                await FillModel(model, cart);
                SetCartUid(cart.CartUid);
                return model;
            }
            else
            {
                var cart = await _checkoutService.GetOrCreateAnonymCart(existingUid);
                cart.Skus.MergeUpdateWithDeleteKeyed(model.Skus, ordered => ordered.Sku.Code, skuModel => skuModel.Code, skuModel =>
                {
                    var result = _productService.GetSkuOrderedAsync(skuModel.Code).Result;
                    result.Quantity = skuModel.Quantity;
                    return result;
                }, (ordered, skuModel) => ordered.Quantity = skuModel.Quantity);
                cart.Discount = await _discountService.GetByCode(model.PromoCode);
				var gcCodes = model.GiftCertificateCodes.Select(x => x.Value).ToList();
				cart.GiftCertificates.MergeKeyed(gcCodes, gc => gc.GiftCertificate.Code, code => code,
                    code => new GiftCertificateInOrder
                    {
                        GiftCertificate = _gcService.GetGiftCertificateAsync(code).Result
                    });
                await _checkoutService.UpdateCart(cart);
                await FillModel(model, cart);
                SetCartUid(cart.CartUid);
                return model;
            }
        }

		private async Task<ViewCartModel> InitCartModelInternal()
		{
			var cartModel = await GetCart();

			if (!cartModel.GiftCertificateCodes.Any())
			{
				cartModel.GiftCertificateCodes.Add(new CartGcModel() { Value = string.Empty }); //needed to to force first input to appear
			}
			return cartModel;
		}

		private async Task<ViewCartModel> GetCart()
	    {
			ViewCartModel cartModel;
			var existingUid = Request.GetCartUid();
			if (await CustomerLoggenIn())
			{
				var id = GetInternalCustomerId();
				cartModel = await GetFromCustomerCart(existingUid, id);
			}
			else
			{
				cartModel = await GetFromAnonymCart(existingUid);
			}

			return cartModel;
	    }

        private void SetCartUid(Guid uid)
        {
            Response.Cookies.Append(CheckoutConstants.CartUidCookieName, uid.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1)
            });
        }

        private async Task<ViewCartModel> GetFromCustomerCart(Guid? existingUid, int idCustomer)
        {
            var cartModel = new ViewCartModel();
            var cart = await _checkoutService.GetOrCreateCart(existingUid, idCustomer);
            await FillModel(cartModel, cart);
            SetCartUid(cart.CartUid);
            return cartModel;
        }

        private async Task<ViewCartModel> GetFromAnonymCart(Guid? existingUid)
        {
            var cartModel = new ViewCartModel();
            var cart = await _checkoutService.GetOrCreateAnonymCart(existingUid);
            await FillModel(cartModel, cart);
            SetCartUid(cart.CartUid);
            return cartModel;
        }

        private async Task FillModel(ViewCartModel cartModel, CustomerCartOrder cart)
        {
            cartModel.Skus.Clear();
            cartModel.Skus.AddRange(
                cart.Order.Skus.Select(sku =>
                {
                    var result = _skuMapper.ToModel<CartSkuModel>(sku.Sku);
                    _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                    result.Price = sku.Amount;
                    result.Quantity = sku.Quantity;
                    result.SubTotal = sku.Quantity * sku.Amount;
                    return result;
                }));
            cartModel.GiftCertificateCodes.Clear();
            cartModel.GiftCertificateCodes.AddRange(cart.Order.GiftCertificates.Select(g => g.GiftCertificate.Code).Select(x=> new CartGcModel() { Value = x}));
            await Calculate(cartModel, cart.Order);
        }

        private async Task FillModel(ViewCartModel cartModel, CustomerCart cart)
        {
	        cartModel.Skus.Clear();
            cartModel.Skus.AddRange(
                cart.Skus?.Select(sku =>
                {
                    var result = _skuMapper.ToModel<CartSkuModel>(sku.Sku);
                    _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                    result.Price = sku.Amount;
                    result.Quantity = sku.Quantity;
                    result.SubTotal = sku.Quantity*sku.Amount;
                    return result;
                }) ?? Enumerable.Empty<CartSkuModel>());
            cartModel.GiftCertificateCodes.AddRange(cart.GiftCertificates?.Select(g => g.GiftCertificate.Code).Select(x => new CartGcModel() { Value = x }) ?? Enumerable.Empty<CartGcModel>());
            var order = await _orderService.CreatePrototypeAsync((int) OrderType.Normal);
            order.Skus = cart.Skus;
            order.GiftCertificates = cart.GiftCertificates;
            order.Discount = cart.Discount;
            await Calculate(cartModel, order);
        }

        private async Task Calculate(ViewCartModel cartModel, OrderDynamic order)
        {
            var context = await _orderService.CalculateOrder(order);
            cartModel.ShippingUpgradeNPOptions = context.ShippingUpgradeNpOptions;
            cartModel.ShippingUpgradePOptions = context.ShippingUpgradePOptions;
            cartModel.DiscountTotal = context.DiscountTotal;
            cartModel.GiftCertificatesTotal = context.GiftCertificatesSubtotal;
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
            if (((ShipDelayType?)order.SafeData.ShipDelayType ?? ShipDelayType.None) != ShipDelayType.None)
            {
                cartModel.ShipAsap = false;
                cartModel.ShippingDate = order.SafeData.ShipDelayDateP;
            }
            else
            {
                cartModel.ShipAsap = true;
            }
            cartModel.ShippingUpgradeNP = order.SafeData.ShippingUpgradeP;
            cartModel.ShippingUpgradeNP = order.SafeData.ShippingUpgradeNP;
        }
    }
}