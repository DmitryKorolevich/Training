using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VC.Public.Helpers;
using VC.Public.Models.Cart;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Core.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Infrastructure.Services;
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
        protected readonly AppSettings AppSettings;
        protected static readonly ObjectSemaphore<int> OrderLocks = new ObjectSemaphore<int>();

        protected CheckoutControllerBase(ICustomerService customerService,
            ReferenceData referenceData, IAuthorizationService authorizationService, ICheckoutService checkoutService,
            IOrderService orderService, IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper,
            ISettingService settingService, ExtendedUserManager userManager, AppSettings appSettings) : base(customerService,
            authorizationService, checkoutService, userManager, referenceData)
        {
            OrderService = orderService;
            SkuMapper = skuMapper;
            ProductMapper = productMapper;
            SettingService = settingService;
            AppSettings = appSettings;
        }

        protected async Task<bool> IsCartEmpty()
        {
            var uid = HttpContext.GetCartUid();
            return uid == null || await CheckoutService.GetCartItemsCount(uid.Value) == 0;
        }

        protected async Task<ViewCartModel> InitCartModelInternal(ViewCartModel cartModel)
        {
            await FillCartModel(cartModel);

            if (cartModel.GiftCertificateCodes.Count == 0)
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
            HttpContext.SetCartUid(cart.CartUid);
        }

        protected async Task FillModel(ViewCartModel cartModel, OrderDynamic order, OrderDataContext context)
        {
            if (!context.ProductsPerishableThresholdIssue)
            {
                cartModel.TopGlobalMessage = null;
            }
            else
            {
                decimal? globalThreshold = AppSettings.GlobalPerishableThreshold;
                cartModel.TopGlobalMessage = globalThreshold.HasValue && globalThreshold.Value > 0
                    ? $"Frozen products must total {globalThreshold.Value:c}, to prevent thawing during shipping. Please add frozen items until they total {globalThreshold.Value.ToString("c")}."
                    : null;
            }
            if (cartModel.TopGlobalMessage != null)
            {
                ModelState.AddModelError(string.Empty, cartModel.TopGlobalMessage);
            }
            if (!string.IsNullOrWhiteSpace(cartModel.DiscountCode) && order.Discount == null)
            {
                context.Messages.Add(new MessageInfo
                {
                    Field = "DiscountCode",
                    Message = "Discount not found"
                });
            }
            cartModel.FreeShipDifference = context.FreeShippingThresholdDifference;
            cartModel.FreeShipAmount = context.FreeShippingThresholdAmount;
            cartModel.DiscountDescription = context.Order?.Discount?.Description;
            cartModel.DiscountMessage = context.DiscountMessage;
            cartModel.Messages =
                context.Messages.Select(x => new KeyValuePair<string, string>(x.Field, x.Message)).ToList();
            cartModel.Skus.Clear();
            await cartModel.Skus.AddRangeAsync(
                order.Skus?.Select(async sku =>
                {
                    var result = await SkuMapper.ToModelAsync<CartSkuModel>(sku.Sku);
                    await ProductMapper.UpdateModelAsync(result, sku.Sku.Product);
                    result.Price = sku.Amount;
                    result.Quantity = sku.Quantity;
                    result.SubTotal = sku.Quantity*sku.Amount;

                    result.GeneratedGCCodes = sku.GcsGenerated?.Select(g => g.Code).ToList();

                    result.Warnings =
                        sku.Messages.Where(message => message.MessageLevel == MessageLevel.Warning)
                            .Select(message => message.Message)
                            .ToList();
                    result.Infos =
                        sku.Messages.Where(message => message.MessageLevel == MessageLevel.Info).Select(message => message.Message).ToList();

                    return result;
                }) ?? Enumerable.Empty<Task<CartSkuModel>>());
            var gcsInCart = cartModel.GiftCertificateCodes.ToArray();
            var hasEmpty = gcsInCart.Any(g => string.IsNullOrWhiteSpace(g.Value));
            cartModel.GiftCertificateCodes.Clear();
            int num = 0;
            foreach (var code in gcsInCart)
            {
                if (!string.IsNullOrWhiteSpace(code.Value) &&
                    order.GiftCertificates.All(g => !string.Equals(g.GiftCertificate.Code?.Trim(), code.Value.Trim(), StringComparison.CurrentCultureIgnoreCase)))
                {
                    cartModel.GiftCertificateCodes.Add(code);
                    code.ErrorMessage = "Gift Certificate not found";
                    ModelState.AddModelError("", "Gift Certificate not found");
                }
                if (!string.IsNullOrWhiteSpace(code.Value))
                {
                    num++;
                }
            }
            var gcMessages = context.GcMessageInfos.GroupBy(m => m.Field).ToArray();
            cartModel.GiftCertificateCodes.AddRange(
                order.GiftCertificates?.Select(g => g.GiftCertificate.Code).Select(x =>
                {
                    var messages = gcMessages.Where(m => m.Key == x).SelectMany(m => m).ToArray();
                    var errorText = string.Join("<br />", messages.Where(m => m.MessageLevel == MessageLevel.Error).Select(m => m.Message));
                    var infoText = string.Join("<br />", messages.Where(m => m.MessageLevel == MessageLevel.Info).Select(m => m.Message));

                    if (!string.IsNullOrEmpty(errorText))
                    {
                        ModelState.AddModelError("", errorText);
                    }

                    return new CartGcModel
                    {
                        Value = x,
                        SuccessMessage = string.IsNullOrEmpty(errorText) ? infoText : string.Empty,
                        ErrorMessage = errorText
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
            await cartModel.PromoSkus.AddRangeAsync(context.PromoSkus.Where(p => p.Enabled).Select(async sku =>
            {
                var result = await SkuMapper.ToModelAsync<CartSkuModel>(sku.Sku);
                await ProductMapper.UpdateModelAsync(result, sku.Sku.Product);
                result.Price = sku.Amount;
                result.Quantity = sku.Quantity;
                result.SubTotal = sku.Quantity*sku.Amount;

                result.GeneratedGCCodes = sku.GcsGenerated?.Select(g => g.Code).ToList();

                result.Warnings =
                    sku.Messages.Where(message => message.MessageLevel == MessageLevel.Warning).Select(message => message.Message).ToList();
                result.Infos =
                    sku.Messages.Where(message => message.MessageLevel == MessageLevel.Info).Select(message => message.Message).ToList();

                return result;
            }));
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
            cartModel.ShippingUpgradeP = (ShippingUpgradeOption?) (int?) order.SafeData.ShippingUpgradeP;
            cartModel.ShippingUpgradeNP = (ShippingUpgradeOption?) (int?) order.SafeData.ShippingUpgradeNP;
            cartModel.AutoShip = order.IdObjectType == (int) OrderType.AutoShip;

            if (cartModel.GiftCertificateCodes.Count == 0)
            {
                cartModel.GiftCertificateCodes.Add(new CartGcModel() {Value = string.Empty});
            }

            foreach (
                var message in
                context.Messages.Where(m => m.MessageLevel == MessageLevel.Error))
            {
                ModelState.AddModelError(message.Field, message.Message);
            }

            int index = 0;
            foreach (var sku in context.SkuOrdereds)
            {
                foreach (var error in sku.Messages)
                {
                    if (error.MessageLevel == MessageLevel.Error)
                    {
                        ModelState.AddModelError("Code".FormatCollectionError("Skus", index), error.Message);
                        ModelState.AddModelError(string.Empty, $"{sku.Sku.Code}: {error.Message}");
                    }
                }
                index++;
            }
            index = 0;
            foreach (var promo in context.PromoSkus.Where(p => p.Enabled))
            {
                foreach (var error in promo.Messages)
                {
                    if (error.MessageLevel == MessageLevel.Error)
                    {
                        ModelState.AddModelError("Code".FormatCollectionError("Promos", index), error.Message);
                        ModelState.AddModelError(string.Empty, $"{promo.Sku.Code}: {error.Message}");
                    }
                }
                index++;
            }
        }
    }
}