using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VC.Public.Models.Tracking;
using VitalChoice.Infrastructure.Domain.Constants;
using Microsoft.AspNetCore.Http;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Identity;

namespace VC.Public.Components.Tracking
{
    public static class BaseTrackScriptsComponentHelper
    {
        private const string TrackScriptsBaseModel = "TrackScriptsBaseModel";

        public static async Task SetBaseOptions(this BaseTrackScriptsModel toReturn, HttpContext context,
            Lazy<IOrderService> orderService, Lazy<IAuthorizationService> authorizationService, Lazy<ICheckoutService> checkoutService,
            Lazy<ICustomerService> customerService, Lazy<ReferenceData> referenceData)
        {
            toReturn.PageName = GetCheckoutStep(context);

            if (context.Items.ContainsKey(TrackScriptsBaseModel))
            {
                var baseModel = (BaseTrackScriptsModel) context.Items[TrackScriptsBaseModel];
                toReturn.Order = baseModel.Order;
                toReturn.OrderCompleteStep = baseModel.OrderCompleteStep;
                return;
            }

            OrderDynamic order;
            if (toReturn.PageName == PageName.Receipt)
            {
                var ids = (context.Session.GetString(CheckoutConstants.ReceiptSessionOrderIds) ?? String.Empty).Split(',');
                var orderIds = new List<int>();
                foreach (var id in ids)
                {
                    int result;
                    if (Int32.TryParse(id, out result))
                    {
                        orderIds.Add(result);
                    }
                }

                if (orderIds.Count == 0)
                {
                    return;
                }
                var tOrderService = orderService.Value;
                var orders = (await tOrderService.SelectAsync(orderIds, true)).OrderBy(p => p.Id).ToList();
                for (int i = 0; i < orders.Count; i++)
                {
                    if (orders[i].IdObjectType == (int)OrderType.AutoShip)
                    {
                        var id = (await tOrderService.SelectAutoShipOrdersAsync(orders[i].Id)).First();
                        if (id != 0)
                        {
                            orders[i] = await tOrderService.SelectAsync(id, true);
                        }
                        else
                        {
                            orders[i] = null;
                        }
                    }
                }

                if (orders.Count>0 && orders[0]!=null)
                {
                    var mainOrder = orders[0];
                    mainOrder.Customer = await customerService.Value.SelectAsync(mainOrder.Customer.Id, true);

                    //merge other shipments to the main order
                    orders.Remove(mainOrder);
                    orders.RemoveAll(p => p == null);
                    foreach (var additionalOrder in orders)
                    {
                        if (additionalOrder.Skus != null)
                        {
                            mainOrder.Skus.AddUpdateKeyed(additionalOrder.Skus, p=>p.Sku.Id, (d, s) =>
                            {
                                d.Quantity += s.Quantity;
                            });
                        }
                        if (additionalOrder.PromoSkus != null)
                        {
                            mainOrder.PromoSkus.AddUpdateKeyed(additionalOrder.PromoSkus, p => p.Sku.Id, (d, s) =>
                            {
                                d.Quantity += s.Quantity;
                            });
                        }

                        mainOrder.Total += additionalOrder.Total;
                        mainOrder.ProductsSubtotal += additionalOrder.ProductsSubtotal;
                        mainOrder.DiscountTotal += additionalOrder.DiscountTotal;
                        mainOrder.TaxTotal += additionalOrder.TaxTotal;
                        mainOrder.ShippingTotal += additionalOrder.ShippingTotal;
                    }

                    toReturn.OrderCompleteStep = true;
                    toReturn.Order = mainOrder;
                }
            }
            else
            {
                var existingUid = context.GetCartUid();
                if (existingUid.HasValue)
                {
                    var loggedIn =
                        await authorizationService.Value.AuthorizeAsync(context.User, null, IdentityConstants.IdentityBasicProfile);
                    if (loggedIn)
                    {
                        if (!referenceData.Value.IsValidCustomer(context.User))
                        {
                            loggedIn = false;
                        }
                    }

                    var cart = await checkoutService.Value.GetOrCreateCart(existingUid, loggedIn);
                    if (cart?.Order?.Skus?.Count > 0)
                    {
                        order = cart.Order;
                        toReturn.Order = order;
                    }
                }
            }

            context.Items.Add(TrackScriptsBaseModel, new BaseTrackScriptsModel()
            {
                Order = toReturn.Order,
                OrderCompleteStep = toReturn.OrderCompleteStep
            });
        }

        public static PageName GetCheckoutStep(this HttpContext context)
        {
            if (context.Request.Path.HasValue)
            {
                switch (context.Request.Path.Value)
                {
                    case "/checkout/welcome":
                        return PageName.Welcome;
                    case "/checkout/addupdatebillingaddress":
                        return PageName.Billing;
                    case "/checkout/addupdateshippingmethod":
                        return PageName.Shipping;
                    case "/checkout/revieworder":
                        return PageName.Preview;
                    case "/cart/viewcart":
                        return PageName.ViewCart;
                    case "/checkout/receipt":
                        return PageName.Receipt;
                    case "/profile/changebillinginfo":
                        return PageName.ProfileBilling;
                    default:
                        return PageName.Unknown;
                }
            }
            return PageName.Unknown;
        }

        public static string GetProductFullName(this SkuOrdered skuOrdered)
        {
            var toReturn = skuOrdered.Sku.Product.Name ?? String.Empty;
            if (!string.IsNullOrEmpty(skuOrdered.Sku.Product.SafeData.SubTitle))
            {
                toReturn += " " + skuOrdered.Sku.Product.Data.SubTitle;
            }
            toReturn += $" ({skuOrdered.Sku.SafeData.QTY ?? 0})";
            return toReturn;
        }
    }
}