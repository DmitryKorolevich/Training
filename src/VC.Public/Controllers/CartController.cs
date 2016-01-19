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
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VC.Public.Controllers
{
    public class CartController : PublicControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly ICustomerService _customerService;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
        private readonly ICheckoutService _checkoutService;
        private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
        private readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
        private readonly IOrderService _orderService;

        public CartController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
            ICustomerService customerService, IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
            IOrderService orderService, IProductService productService, ICheckoutService checkoutService,
            IAuthorizationService authorizationService, IAppInfrastructureService appInfrastructureService, IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper)
            : base(contextAccessor, customerService, appInfrastructureService, authorizationService)
        {
            _contextAccessor = contextAccessor;
            _storefrontUserService = storefrontUserService;
            _customerService = customerService;
            _paymentMethodConverter = paymentMethodConverter;
            _orderService = orderService;
            _productService = productService;
            _checkoutService = checkoutService;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
        }

        public async Task<IActionResult> ViewCart()
        {
            var cartUidString = Request.Cookies[CheckoutConstants.CartUidCookieName];
            ViewCartModel cartModel;
            Guid? existingUid = null;
            Guid cartUid;
            if (cartUidString.Count > 0 && Guid.TryParse(cartUidString.First(), out cartUid))
            {
                existingUid = cartUid;
            }
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

        private async Task<ViewCartModel> GetFromCustomerCart(Guid? existingUid, int idCustomer)
        {
            var cartModel = new ViewCartModel();
            var cart = await _checkoutService.GetOrCreateCart(existingUid, idCustomer);
            cartModel.Skus.AddRange(
                cart.Order.Skus.Select(sku =>
                {
                    var result = _skuMapper.ToModel<CartSkuModel>(sku.Sku);
                    _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                    result.Price = sku.Amount;
                    result.Quantity = sku.Quantity;
                    result.SubTotal = sku.Quantity*sku.Amount;
                    return result;
                }));
            cartModel.GiftCertificateCodes.AddRange(cart.Order.GiftCertificates.Select(g => g.GiftCertificate.Code));
            FillFromOrder(cartModel, cart.Order);
            return cartModel;
        }

        private async Task<ViewCartModel> GetFromAnonymCart(Guid? existingUid)
        {
            var cartModel = new ViewCartModel();
            var cart = await _checkoutService.GetOrCreateAnonymCart(existingUid);
            cartModel.Skus.AddRange(
                cart.Skus.Select(sku =>
                {
                    var result = _skuMapper.ToModel<CartSkuModel>(sku.Sku);
                    _productMapper.UpdateModel(result, sku.ProductWithoutSkus);
                    result.Price = sku.Amount;
                    result.Quantity = sku.Quantity;
                    result.SubTotal = sku.Quantity*sku.Amount;
                    return result;
                }));
            cartModel.GiftCertificateCodes.AddRange(cart.GiftCertificates.Select(g => g.GiftCertificate.Code));
            var order = await _orderService.CreatePrototypeAsync((int) OrderType.Normal);
            order.Skus = cart.Skus;
            order.GiftCertificates = cart.GiftCertificates;
            await _orderService.CalculateOrder(order);
            FillFromOrder(cartModel, order);
            return cartModel;
        }

        private static void FillFromOrder(ViewCartModel cartModel, OrderDynamic order)
        {
            cartModel.OrderTotal = order.Total;
            cartModel.PromoCode = order.Discount?.Code;
            cartModel.ShippingCost = order.ShippingTotal;
            cartModel.SubTotal = order.ProductsSubtotal;
            //cartModel.ShipAsap = cart.Order.SafeData.ShipDelayType != null && ShipDelayType
            //cartModel.ShippingDate = cart.Order.
            cartModel.UpgradeOptions = (ShippingUpgrade) (int?)order.SafeData.ShippingUpgradeP |
                                       (ShippingUpgrade) (int?)order.SafeData.ShippingUpgradeNP;
        }
    }
}