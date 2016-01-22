using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Cart;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
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
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

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

        public async Task<IActionResult> ViewCart()
        {
            ViewCartModel cartModel;
            var existingUid = GetCartUid();
            if (await CustomerLoggenIn())
            {
                var id = GetInternalCustomerId();
                cartModel = await GetFromCustomerCart(existingUid, id);
            }
            else
            {
                cartModel = await GetFromAnonymCart(existingUid);
            }
            return View(cartModel);
        }

        [HttpPost]
        public async Task<bool> AddToCart(string skuCode)
        {
            var existingUid = GetCartUid();
            var sku = await _productService.GetSkuOrderedAsync(skuCode);
            if (sku == null)
                return false;
            if (await CustomerLoggenIn())
            {
                var id = GetInternalCustomerId();
                var cart = await _checkoutService.GetOrCreateCart(existingUid, id);
                cart.Order.Skus.MergeUpdateWithDeleteKeyed(Enumerable.Repeat(sku, 1).ToArray(),
                    ordered => ordered.Sku.Code, skuModel => skuModel.Sku.Code, skuModel =>
                    {
                        var result = _productService.GetSkuOrderedAsync(skuModel.Sku.Code).Result;
                        result.Quantity = 1;
                        result.Amount = HasRole(RoleType.Wholesale) ? skuModel.Sku.WholesalePrice : skuModel.Sku.Price;
                        return result;
                    }, (ordered, skuModel) => ordered.Quantity += 1);
                return await _checkoutService.UpdateCart(cart);
            }
            else
            {
                var cart = await _checkoutService.GetOrCreateAnonymCart(existingUid);
                cart.Skus.MergeUpdateWithDeleteKeyed(Enumerable.Repeat(sku, 1).ToArray(), ordered => ordered.Sku.Code,
                    skuModel => skuModel.Sku.Code, skuModel =>
                    {
                        var result = _productService.GetSkuOrderedAsync(skuModel.Sku.Code).Result;
                        result.Quantity = 1;
                        result.Amount = skuModel.Sku.Price;
                        return result;
                    }, (ordered, skuModel) => ordered.Quantity += 1);
                return await _checkoutService.UpdateCart(cart);
            }
        }

        [HttpPost]
        public async Task<ViewCartModel> UpdateCart(ViewCartModel model)
        {
            var existingUid = GetCartUid();
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
                cart.Order.GiftCertificates.MergeKeyed(model.GiftCertificateCodes, gc => gc.GiftCertificate.Code, code => code,
                    code => new GiftCertificateInOrder
                    {
                        GiftCertificate = _gcService.GetGiftCertificateAsync(code).Result
                    });
                await _checkoutService.UpdateCart(cart);
                await FillModel(model, cart);
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
                cart.GiftCertificates.MergeKeyed(model.GiftCertificateCodes, gc => gc.GiftCertificate.Code, code => code,
                    code => new GiftCertificateInOrder
                    {
                        GiftCertificate = _gcService.GetGiftCertificateAsync(code).Result
                    });
                await _checkoutService.UpdateCart(cart);
                await FillModel(model, cart);
                return model;
            }
        }

        private Guid? GetCartUid()
        {
            var cartUidString = Request.Cookies[CheckoutConstants.CartUidCookieName];
            Guid? existingUid = null;
            Guid cartUid;
            if (cartUidString.Count > 0 && Guid.TryParse(cartUidString.First(), out cartUid))
            {
                existingUid = cartUid;
            }
            return existingUid;
        }

        private async Task<ViewCartModel> GetFromCustomerCart(Guid? existingUid, int idCustomer)
        {
            var cartModel = new ViewCartModel();
            var cart = await _checkoutService.GetOrCreateCart(existingUid, idCustomer);
            await FillModel(cartModel, cart);
            return cartModel;
        }

        private async Task<ViewCartModel> GetFromAnonymCart(Guid? existingUid)
        {
            var cartModel = new ViewCartModel();
            var cart = await _checkoutService.GetOrCreateAnonymCart(existingUid);
            await FillModel(cartModel, cart);
            return cartModel;
        }

        private async Task FillModel(ViewCartModel cartModel, CustomerCartOrder cart)
        {
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
            cartModel.GiftCertificateCodes.AddRange(cart.Order.GiftCertificates.Select(g => g.GiftCertificate.Code));
            await _orderService.CalculateOrder(cart.Order);
            FillFromOrder(cartModel, cart.Order);
        }

        private async Task FillModel(ViewCartModel cartModel, CustomerCart cart)
        {
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
            cartModel.GiftCertificateCodes.AddRange(cart.GiftCertificates?.Select(g => g.GiftCertificate.Code) ?? Enumerable.Empty<string>());
            var order = await _orderService.CreatePrototypeAsync((int) OrderType.Normal);
            order.Skus = cart.Skus;
            order.GiftCertificates = cart.GiftCertificates;
            await _orderService.CalculateOrder(order);
            FillFromOrder(cartModel, order);
        }

        private static void FillFromOrder(ViewCartModel cartModel, OrderDynamic order)
        {
            cartModel.OrderTotal = order.Total;
            cartModel.PromoCode = order.Discount?.Code;
            cartModel.ShippingCost = order.ShippingTotal;
            cartModel.SubTotal = order.ProductsSubtotal;
            //cartModel.ShipAsap = cart.Order.SafeData.ShipDelayType != null && ShipDelayType
            //cartModel.ShippingDate = cart.Order.
            //cartModel.UpgradeOption = (ShippingUpgrade)(int?)order.SafeData.ShippingUpgradeP |
            //                           (ShippingUpgrade)(int?)order.SafeData.ShippingUpgradeNP;
        }
    }
}