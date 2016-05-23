using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using VC.Public.Helpers;
using VC.Public.Models.Cart;
using VitalChoice.Core.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Core.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Settings;

namespace VC.Public.Controllers
{
    public abstract class CheckoutControllerBase : PublicControllerBase
    {
        protected readonly ISettingService SettingService;
        protected readonly IOrderService OrderService;
        protected readonly IDynamicMapper<SkuDynamic, Sku> SkuMapper;
        protected readonly IDynamicMapper<ProductDynamic, Product> ProductMapper;
        protected readonly ReferenceData AppInfrastructure;

        protected CheckoutControllerBase(ICustomerService customerService,
            IAppInfrastructureService infrastructureService, IAuthorizationService authorizationService, ICheckoutService checkoutService,
            IOrderService orderService, IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper,
            IPageResultService pageResultService, ISettingService settingService) : base(customerService,
                infrastructureService, authorizationService, checkoutService, pageResultService)
        {
            OrderService = orderService;
            SkuMapper = skuMapper;
            ProductMapper = productMapper;
            SettingService = settingService;
            AppInfrastructure = infrastructureService.Data();
        }

        protected async Task<bool> IsCartEmpty()
        {
            var uid = Request.GetCartUid();
            return uid == null || await CheckoutService.GetCartItemsCount(uid.Value) == 0;
        }

        protected async Task<ViewCartModel> InitCartModelInternal(ViewCartModel cartModel)
        {
            await FillCartModel(cartModel);

            if (!cartModel.GiftCertificateCodes.Any())
            {
                cartModel.GiftCertificateCodes.Add(new CartGcModel() {Value = string.Empty}); //needed to to force first input to appear
            }
            return cartModel;
        }

        protected async Task FillCartModel(ViewCartModel cartModel)
        {
            var cart = await GetCurrentCart();
            var context = await OrderService.CalculateStorefrontOrder(cart.Order, OrderStatus.Incomplete);
            await FillModel(cartModel, cart.Order, context);
            SetCartUid(cart.CartUid);
        }

        protected void SetCartUid(Guid uid)
        {
            Response.Cookies.Append(CheckoutConstants.CartUidCookieName, uid.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1)
            });
        }

        protected async Task FillModel(ViewCartModel cartModel, OrderDynamic order, OrderDataContext context)
        {
            if (!context.ProductsPerishableThresholdIssue)
            {
                cartModel.TopGlobalMessage = null;
            }
            else
            {
                var globalThreshold = (await SettingService.GetAppSettingsAsync()).GlobalPerishableThreshold;
                cartModel.TopGlobalMessage = globalThreshold.HasValue && globalThreshold.Value > 0
                    ? $"Frozen products must total {globalThreshold.Value.ToString("c")}, to prevent thawing during shipping. Please add frozen items until they total {globalThreshold.Value.ToString("c")}."
                    : null;
            }
            if (cartModel.TopGlobalMessage != null)
            {
                ModelState.AddModelError(string.Empty, cartModel.TopGlobalMessage);
            }
            var gcMessages = context.GcMessageInfos.ToDictionary(m => m.Field);
            if (!string.IsNullOrWhiteSpace(cartModel.DiscountCode) && order.Discount == null && !((bool?)order.SafeData.IsHealthwise ?? false))
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
                context.Messages.Select(x => new KeyValuePair<string, string>(x.Field, x.Message)).ToList();
            cartModel.Skus.Clear();
            cartModel.Skus.AddRange(await Task.WhenAll(
                order.Skus?.Select(async sku =>
                {
                    var result = await SkuMapper.ToModelAsync<CartSkuModel>(sku.Sku);
                    await ProductMapper.UpdateModelAsync(result, sku.Sku.Product);
                    result.Price = sku.Amount;
                    result.Quantity = sku.Quantity;
                    result.SubTotal = sku.Quantity*sku.Amount;

                    result.GeneratedGCCodes = sku.GcsGenerated?.Select(g => g.Code).ToList();

                    return result;
                })) ?? Enumerable.Empty<CartSkuModel>());
            var gcsInCart = cartModel.GiftCertificateCodes.ToArray();
            var hasEmpty = gcsInCart.Any(g => string.IsNullOrWhiteSpace(g.Value));
            cartModel.GiftCertificateCodes.Clear();
            foreach (var code in gcsInCart)
            {
                if (!string.IsNullOrWhiteSpace(code.Value) && order.GiftCertificates.All(g => g.GiftCertificate.Code != code.Value))
                {
                    cartModel.GiftCertificateCodes.Add(code);
                    code.ErrorMessage = "Gift Certificate not found";
                }
            }
            cartModel.GiftCertificateCodes.AddRange(
                order.GiftCertificates?.Select(g => g.GiftCertificate.Code).Select(x =>
                {
                    var message = gcMessages.ContainsKey(x) ? gcMessages[x] : new MessageInfo();

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
            cartModel.PromoSkus.AddRange(await Task.WhenAll(context.PromoSkus.Where(p => p.Enabled).Select(async sku =>
            {
                var result = await SkuMapper.ToModelAsync<CartSkuModel>(sku.Sku);
                await ProductMapper.UpdateModelAsync(result, sku.Sku.Product);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity*sku.Amount;

                result.GeneratedGCCodes = sku.GcsGenerated?.Select(g => g.Code).ToList();

                return result;
            })));
            cartModel.Tax = order.TaxTotal;
            cartModel.OrderTotal = order.Total;
            cartModel.DiscountCode = order.Discount?.Code;
            cartModel.ShippingCost = order.ShippingTotal;
            cartModel.SubTotal = order.ProductsSubtotal;
            if (((ShipDelayType?) order.SafeData.ShipDelayType ?? ShipDelayType.None) != ShipDelayType.None)
            {
                cartModel.ShipAsap = false;
                cartModel.ShippingDate = order.SafeData.ShipDelayDate;
            }
            else
            {
                cartModel.ShipAsap = true;
                cartModel.ShippingDate = null;
            }
            cartModel.ShippingUpgradeP = (ShippingUpgradeOption?) order.SafeData.ShippingUpgradeP;
            cartModel.ShippingUpgradeNP = (ShippingUpgradeOption?) order.SafeData.ShippingUpgradeNP;
            cartModel.AutoShip = order.IdObjectType == (int) OrderType.AutoShip;

            if (!cartModel.GiftCertificateCodes.Any())
            {
                cartModel.GiftCertificateCodes.Add(new CartGcModel() {Value = string.Empty});
            }

            foreach (var message in context.Messages?.Where(m => m.MessageLevel == MessageLevel.Error) ?? Enumerable.Empty<MessageInfo>())
            {
                ModelState.AddModelError(message.Field, message.Message);
            }

            int index = 0;
            foreach (var sku in context.SkuOrdereds)
            {
                foreach (var error in sku.Messages)
                {
                    ModelState.AddModelError("Code".FormatCollectionError("Skus", index), error.Message);
                    ModelState.AddModelError(string.Empty, $"{sku.Sku.Code}: {error.Message}");
                }
                index++;
            }
            index = 0;
            foreach (var promo in context.PromoSkus.Where(p => p.Enabled))
            {
                foreach (var error in promo.Messages)
                {
                    ModelState.AddModelError("Code".FormatCollectionError("Promos", index), error.Message);
                    ModelState.AddModelError(string.Empty, $"{promo.Sku.Code}: {error.Message}");
                }
                index++;
            }
        }
    }
}