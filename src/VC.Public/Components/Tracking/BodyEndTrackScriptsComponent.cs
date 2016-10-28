using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VC.Public.Helpers;
using VC.Public.Models.Profile;
using VC.Public.Models.Tracking;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using Microsoft.AspNetCore.Http;
using VitalChoice.Business.Services.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Interfaces.Services.Orders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VitalChoice.Business.Services.Affiliates;
using VitalChoice.Business.Services.Checkout;
using VitalChoice.Business.Services.Orders;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Affiliates;

namespace VC.Public.Components.Tracking
{
    [ViewComponent(Name = "BodyEndTrackScripts")]
    public class BodyEndTrackScriptsComponent : ViewComponent
    {
        private const string PepperjamProgramId = "8392";

        private readonly Lazy<IAuthorizationService> _authorizationService;
        private readonly Lazy<ICheckoutService> _checkoutService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<ICustomerService> _customerService;
        private readonly Lazy<IAffiliateService> _affiliateService;
        private readonly Lazy<ReferenceData> _referenceData;
        private readonly BaseTrackScriptsComponentHelper _helper;
        private readonly Lazy<IOptions<AppOptions>> _appOptions;

        public BodyEndTrackScriptsComponent()
        {
            _helper =new BaseTrackScriptsComponentHelper();
            _authorizationService = new Lazy<IAuthorizationService>(() => HttpContext.RequestServices.GetService<IAuthorizationService>());
            _checkoutService = new Lazy<ICheckoutService>(() => HttpContext.RequestServices.GetService<ICheckoutService>());
            _orderService = new Lazy<IOrderService>(() => HttpContext.RequestServices.GetService<IOrderService>());
            _customerService = new Lazy<ICustomerService>(() => HttpContext.RequestServices.GetService<ICustomerService>());
            _affiliateService = new Lazy<IAffiliateService>(() => HttpContext.RequestServices.GetService<IAffiliateService>());
            _referenceData = new Lazy<ReferenceData>(() => HttpContext.RequestServices.GetService<ReferenceData>());
            _appOptions = new Lazy<IOptions<AppOptions>>(() => HttpContext.RequestServices.GetService<IOptions<AppOptions>>());
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var toReturn = new BodyEndTrackScriptsModel();

            var path = HttpContext.Request.Path.Value;
            await _helper.SetBaseOptions(toReturn, HttpContext, _orderService, _authorizationService, _checkoutService, _customerService,
                _referenceData);
            OrderDynamic order = toReturn.Order;
            var referenceData = _referenceData.Value;
            if (order != null && _appOptions.Value.Value.EnableOrderTrackScripts)
            {
                toReturn.MyBuysEnabled = true;
                //bronto recovery
                BrontoOrderInfo info = new BrontoOrderInfo();
                info.url = $"https://{referenceData.PublicHost}/cart/viewcart";
                info.customerEmail = order?.Customer?.Email;
                info.currency = "USD";
                info.subtotal = Math.Round(order.ProductsSubtotal, 2);

                if (toReturn.OrderCompleteStep)
                {
                    info.orderID = order.Id;
                    info.discountAmount = order.DiscountTotal;
                    info.grandTotal = order.Total;
                    info.taxAmount = order.TaxTotal;
                }

                var skus = order.Skus.ToList();
                skus.AddRange(order.PromoSkus.Where(p => p.Enabled));
                foreach (var skuOrdered in skus)
                {
                    BrontoOrderItemInfo item = new BrontoOrderItemInfo();
                    item.sku = skuOrdered.Sku.Code;
                    item.imageUrl = skuOrdered.Sku.Product.SafeData.Thumbnail != null ?
                        $"https://{referenceData.PublicHost}{skuOrdered.Sku.Product.Data.Thumbnail}"
                        : null;
                    item.productUrl = $"https://{referenceData.PublicHost}/product/{skuOrdered.Sku.Product.Url}";
                    item.quantity = skuOrdered.Quantity;
                    item.unitPrice = Math.Round(skuOrdered.Amount, 2);
                    item.totalPrice = Math.Round(skuOrdered.Quantity * skuOrdered.Amount, 2);
                    item.name = _helper.GetProductFullName(skuOrdered);

                    info.lineItems.Add(item);
                }

                var settings = new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                };
                var data = JsonConvert.SerializeObject(info, settings);
                toReturn.BrontoOrderInfo = $"var brontoCartRecovery = {data};";

                //google action scripts
                int? step = null;
                switch (path)
                {
                    case "/cart/viewcart":
                    case "/checkout/welcome":
                        step = 1;
                        break;
                    case "/checkout/addupdatebillingaddress":
                        step = 2;
                        break;
                    case "/checkout/addupdateshippingmethod":
                        step = 3;
                        break;
                    case "/checkout/revieworder":
                        step = 4;
                        break;
                    case "/checkout/receipt":
                        step = 5;
                        break;
                }
                if (step.HasValue)
                {
                    var checkout = new GoogleActionCheckoutInfo();
                    checkout.actionField = new GoogleActionCheckoutStepInfo()
                    {
                        step = step.Value,
                    };

                    foreach (var skuOrdered in skus)
                    {
                        GoogleActionOrderItemInfo item = new GoogleActionOrderItemInfo();
                        item.id = skuOrdered.Sku.Code;
                        item.name = _helper.GetProductFullName(skuOrdered).Replace("|", "-");
                        item.price = Math.Round(skuOrdered.Amount, 2);
                        item.quantity = skuOrdered.Quantity;
                        item.brand = "Vital Choice";
                        item.category = "";
                        item.variant = "";

                        checkout.products.Add(item);
                    }

                    toReturn.GoogleActionCheckout = JsonConvert.SerializeObject(checkout);

                    if (toReturn.OrderCompleteStep)
                    {
                        var purchase = new GoogleActionPurchaseInfo();
                        purchase.actionField = new GoogleActionPurchaseActionInfo();
                        purchase.actionField.id = order.Id;
                        purchase.actionField.revenue = Math.Round(order.Total, 2);
                        purchase.actionField.shipping = Math.Round(order.ShippingTotal, 2);
                        purchase.actionField.tax = Math.Round(order.TaxTotal, 2);
                        purchase.actionField.coupon = order.Discount?.Code;

                        purchase.actionField.affiliation = "N/A";
                        var affiliateService = _affiliateService.Value;
                        var affiliateOrderPayment = await affiliateService.GetAffiliateOrderPaymentAsync(order.Id);
                        if (affiliateOrderPayment != null)
                        {
                            var affiliate = await affiliateService.SelectAsync(affiliateOrderPayment.IdAffiliate);
                            if (affiliate != null)
                            {
                                purchase.actionField.affiliation = affiliate.Name;
                                if (affiliate.SafeData.Company != null)
                                {
                                    purchase.actionField.affiliation += $"({affiliate.Data.Company})";
                                }
                                purchase.actionField.affiliation = purchase.actionField.affiliation.Replace("|", "-");
                            }
                        }

                        purchase.products = checkout.products;

                        toReturn.GoogleActionPurchase = JsonConvert.SerializeObject(purchase);
                    }
                }

                //criteo viewBasket
                toReturn.CustomerEmail = order.Customer?.Email ?? string.Empty;
                if (step.HasValue)
                {
                    toReturn.CriteoViewCart = String.Empty;
                    for (int i = 0; i < skus.Count; i++)
                    {
                        var item = skus[i];
                        toReturn.CriteoViewCart +=
                            $"{{ id: \"{item.Sku.Product.Id}\", price: {item.Amount:F}, quantity: {item.Quantity} }}";
                        if (i != skus.Count - 1)
                        {
                            toReturn.CriteoViewCart += ",";
                        }
                    }
                }

                //criteo trackTransaction
                if (toReturn.OrderCompleteStep)
                {
                    toReturn.CriteoOrderComplete = String.Empty;
                    for (int i = 0; i < skus.Count; i++)
                    {
                        var item = skus[i];
                        toReturn.CriteoOrderComplete += $"{{ id: \"{item.Sku.Product.Id}\", price: {item.Amount:F}, quantity: {item.Quantity} }}";
                        if (i != skus.Count - 1)
                        {
                            toReturn.CriteoOrderComplete += ",";
                        }
                    }
                }

                //pepperjam
                if (toReturn.OrderCompleteStep)
                {
                    var statisticData =
                        (await _customerService.Value.GetCustomerOrderStatistics(new[] {toReturn.Order.Customer.Id}))
                            .FirstOrDefault();
                    int isNewCustomer = !(statisticData != null && statisticData.FirstOrderPlaced < toReturn.Order.DateCreated) ? 1 : 0;
                    toReturn.PepperjamQuery =
                        $"INT=DYNAMIC&PROGRAM_ID={PepperjamProgramId}&ORDER_ID={order.Id}&COUPON={order.Discount?.Code}&NEW_TO_FILE={isNewCustomer}";


                    var pricePart = toReturn.Order.ProductsSubtotal != 0
                        ? 1 - toReturn.Order.DiscountTotal/toReturn.Order.ProductsSubtotal
                        : 1;
                    
                    for (int i = 0; i < skus.Count; i++)
                    {
                        var skuOrdered = skus[i];
                        var index = i + 1;
                        var price = Math.Round(skuOrdered.Amount * pricePart, 2);
                        toReturn.PepperjamQuery += $"&ITEM_ID{index}={skuOrdered.Sku.Code}&ITEM_PRICE{index}={price:F}&QUANTITY{index}={skuOrdered.Quantity}";
                    }
                }
            }

            return View("~/Views/Shared/Components/Tracking/BodyEndTrackScripts.cshtml", toReturn);
        }
    }
}