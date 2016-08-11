using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VC.Public.Models.Profile;
using VC.Public.Models.Tracking;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Users;
using Microsoft.AspNetCore.Http;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using Microsoft.Extensions.DependencyInjection;
using VC.Public.Helpers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Identity;

namespace VC.Public.Components.Tracking
{
    public class BaseTrackScriptsComponentHelper
    {
        private const string TrackScriptsBaseModel = "TrackScriptsBaseModel";

        public async Task SetBaseOptions(BaseTrackScriptsModel toReturn, HttpContext context,
            Lazy<IOrderService> orderService, Lazy<IAuthorizationService> authorizationService, Lazy<ICheckoutService> checkoutService,
            Lazy<ICustomerService> customerService,Lazy<ReferenceData> referenceData)
        {
            if (context.Items.ContainsKey(TrackScriptsBaseModel))
            {
                var baseModel = (BaseTrackScriptsModel)context.Items[TrackScriptsBaseModel];
                toReturn.Order = baseModel.Order;
                toReturn.OrderCompleteStep = baseModel.OrderCompleteStep;
                return;
            }

            OrderDynamic order;
            var path = context.Request.Path.Value;
            if (path == "/checkout/receipt")
            {
                var idOrder = context.Session.GetInt32(CheckoutConstants.ReceiptSessionOrderId);
                if (idOrder.HasValue)
                {
                    var tOrderService = orderService.Value;
                    order = await orderService.Value.SelectAsync(idOrder.Value, true);
                    if (order.IdObjectType == (int)OrderType.AutoShip)
                    {
                        var id = (await tOrderService.SelectAutoShipOrdersAsync(idOrder.Value)).First();

                        order = await tOrderService.SelectAsync(id, true);
                    }

                    order.Customer = await customerService.Value.SelectAsync(order.Customer.Id, true);
                    toReturn.OrderCompleteStep = true;
                    toReturn.Order = order;
                }
            }
            else
            {
                var existingUid = context.Request.GetCartUid();
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

        public string GetProductFullName(SkuOrdered skuOrdered)
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