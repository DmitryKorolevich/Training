using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VC.Public.Models.Tracking;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services.Orders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services.Affiliates;

namespace VC.Public.Components.Tracking
{
    [ViewComponent(Name = "BodyEndTrackScripts")]
    public class BodyEndTrackScriptsComponent : ViewComponent
    {
        private readonly Lazy<IAuthorizationService> _authorizationService;
        private const string PepperjamProgramId = "8392";

        private readonly Lazy<ICheckoutService> _checkoutService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<ICustomerService> _customerService;
        private readonly Lazy<IAffiliateService> _affiliateService;
        private readonly Lazy<ReferenceData> _referenceData;
        private readonly Lazy<IOptions<AppOptions>> _appOptions;
        private readonly Lazy<ExtendedUserManager> _userManager;

        public BodyEndTrackScriptsComponent(Lazy<IAuthorizationService> authorizationService, Lazy<ICheckoutService> checkoutService,
            Lazy<IOrderService> orderService, Lazy<ICustomerService> customerService, Lazy<IAffiliateService> affiliateService,
            Lazy<ReferenceData> referenceData, Lazy<IOptions<AppOptions>> appOptions, Lazy<ExtendedUserManager> userManager)
        {
            _authorizationService = authorizationService;
            _checkoutService = checkoutService;
            _orderService = orderService;
            _customerService = customerService;
            _affiliateService = affiliateService;
            _referenceData = referenceData;
            _appOptions = appOptions;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var toReturn = new BodyEndTrackScriptsModel();

            var path = HttpContext.Request.Path.Value;
            await BaseTrackScriptsComponentHelper.SetBaseOptions(toReturn, HttpContext, _orderService, _authorizationService, _checkoutService, _customerService,
                _referenceData);
            OrderDynamic order = toReturn.Order;
            var referenceData = _referenceData.Value;


            if (_appOptions.Value.Value.EnableOrderTrackScripts)
            {
                if (path == "/") //home
                {
                    toReturn.CriteoHomeRender = true;
                    if (HttpContext.User != null)
                    {
                        var customer = await _customerService.Value.SelectAsync(Convert.ToInt32(_userManager.Value.GetUserId(HttpContext.User)));
                        toReturn.CustomerEmail = customer?.Email ?? string.Empty;
                    }
                }

                if (order != null)
                {
                    toReturn.MyBuysEnabled = true;
                    //bronto recovery
                    BrontoOrderInfo info = new BrontoOrderInfo
                    {
                        url = $"https://{referenceData.PublicHost}/cart/viewcart",
                        customerEmail = order?.Customer?.Email,
                        currency = "USD",
                        subtotal = Math.Round(order.ProductsSubtotal, 2)
                    };

                    if (toReturn.OrderCompleteStep)
                    {
                        info.orderID = order.Id;
                        info.discountAmount = order.DiscountTotal;
                        info.grandTotal = order.Total;
                        info.taxAmount = order.TaxTotal;
                    }

                    var skus = order.Skus.Union(order.PromoSkus.Where(p => p.Enabled)).ToArray();
                    foreach (var skuOrdered in skus)
                    {
                        BrontoOrderItemInfo item = new BrontoOrderItemInfo
                        {
                            sku = skuOrdered.Sku.Code,
                            imageUrl = skuOrdered.Sku.Product.SafeData.Thumbnail != null
                                ? $"https://{referenceData.PublicHost}{skuOrdered.Sku.Product.Data.Thumbnail}"
                                : null,
                            productUrl = $"https://{referenceData.PublicHost}/product/{skuOrdered.Sku.Product.Url}",
                            quantity = skuOrdered.Quantity,
                            unitPrice = Math.Round(skuOrdered.Amount, 2),
                            totalPrice = Math.Round(skuOrdered.Quantity*skuOrdered.Amount, 2),
                            name = BaseTrackScriptsComponentHelper.GetProductFullName(skuOrdered)
                        };

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
                    switch (toReturn.Step)
                    {
                        case CheckoutStep.ViewCart:
                        case CheckoutStep.Welcome:
                            step = 1;
                            break;
                        case CheckoutStep.Billing:
                            step = 2;
                            break;
                        case CheckoutStep.Shipping:
                            step = 3;
                            break;
                        case CheckoutStep.Preview:
                            step = 4;
                            break;
                        case CheckoutStep.Receipt:
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
                            item.name = BaseTrackScriptsComponentHelper.GetProductFullName(skuOrdered).Replace("|", "-");
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
                    if (toReturn.Step == CheckoutStep.ViewCart)
                    {
                        toReturn.CriteoViewCart = String.Empty;
                        for (int i = 0; i < skus.Length; i++)
                        {
                            var item = skus[i];
                            toReturn.CriteoViewCart +=
                                $"{{ id: \"{item.Sku.Product.Id}\", price: {item.Amount:F}, quantity: {item.Quantity} }}";
                            if (i != skus.Length - 1)
                            {
                                toReturn.CriteoViewCart += ",";
                            }
                        }
                    }

                    //criteo trackTransaction
                    if (toReturn.OrderCompleteStep)
                    {
                        toReturn.CriteoOrderComplete = String.Empty;
                        for (int i = 0; i < skus.Length; i++)
                        {
                            var item = skus[i];
                            toReturn.CriteoOrderComplete +=
                                $"{{ id: \"{item.Sku.Product.Id}\", price: {item.Amount:F}, quantity: {item.Quantity} }}";
                            if (i != skus.Length - 1)
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
                        int isNewCustomer =
                            !(statisticData != null && statisticData.FirstOrderPlaced < toReturn.Order.DateCreated)
                                ? 1
                                : 0;
                        toReturn.PepperjamQuery =
                            $"INT=DYNAMIC&PROGRAM_ID={PepperjamProgramId}&ORDER_ID={order.Id}&COUPON={order.Discount?.Code}&NEW_TO_FILE={isNewCustomer}";


                        var pricePart = toReturn.Order.ProductsSubtotal != 0
                            ? 1 - toReturn.Order.DiscountTotal/toReturn.Order.ProductsSubtotal
                            : 1;

                        for (int i = 0; i < skus.Length; i++)
                        {
                            var skuOrdered = skus[i];
                            var index = i + 1;
                            var price = Math.Round(skuOrdered.Amount*pricePart, 2);
                            toReturn.PepperjamQuery +=
                                $"&ITEM_ID{index}={skuOrdered.Sku.Code}&ITEM_PRICE{index}={price:F}&QUANTITY{index}={skuOrdered.Quantity}";
                        }
                    }
                }
            }

            return View("~/Views/Shared/Components/Tracking/BodyEndTrackScripts.cshtml", toReturn);
        }
    }
}