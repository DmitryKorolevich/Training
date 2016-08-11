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
using Newtonsoft.Json;

namespace VC.Public.Components.Tracking
{
    [ViewComponent(Name = "HeadTrackScripts")]
    public class HeadTrackScriptsComponent : ViewComponent
    {
        private readonly Lazy<IAuthorizationService> _authorizationService;
        private readonly Lazy<ICheckoutService> _checkoutService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<ICustomerService> _customerService;
        private readonly Lazy<IAffiliateService> _affiliateService;
        private readonly Lazy<ReferenceData> _referenceData;
        private readonly BaseTrackScriptsComponentHelper _helper;

        public HeadTrackScriptsComponent()
        {
            _helper = new BaseTrackScriptsComponentHelper();
            _authorizationService = new Lazy<IAuthorizationService>(() => HttpContext.RequestServices.GetService<IAuthorizationService>());
            _checkoutService = new Lazy<ICheckoutService>(() => HttpContext.RequestServices.GetService<ICheckoutService>());
            _orderService = new Lazy<IOrderService>(() => HttpContext.RequestServices.GetService<IOrderService>());
            _customerService = new Lazy<ICustomerService>(() => HttpContext.RequestServices.GetService<ICustomerService>());
            _affiliateService = new Lazy<IAffiliateService>(() => HttpContext.RequestServices.GetService<IAffiliateService>());
            _referenceData = new Lazy<ReferenceData>(() => HttpContext.RequestServices.GetService<ReferenceData>());
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var toReturn = new HeadTrackScriptsModel();

            await _helper.SetBaseOptions(toReturn, HttpContext, _orderService, _authorizationService, _checkoutService, _customerService,
                _referenceData);

            if (toReturn.OrderCompleteStep && toReturn.Order != null)
            {
                MasterTmsUdoInfo info=new MasterTmsUdoInfo();
                info.CID = "1531092";
                info.DISCOUNT = toReturn.Order.DiscountTotal.ToString("F");
                info.OID = toReturn.Order.Id.ToString();
                info.CURRENCY = "USD";
                info.COUPON = toReturn.Order.Discount?.Code ?? "";
                info.FIRECJ = Request.Cookies.ContainsKey("source") && Request.Cookies["source"]== "CJ" ?
                    "TRUE" : "FALSE";

                var count = await _customerService.Value.GetActiveOrderCount(toReturn.Order.Customer.Id);
                info.TYPE = count > 1 ? "373119" : "373118";

                var skus = toReturn.Order.Skus;
                skus.AddRange(toReturn.Order.PromoSkus.Where(p => p.Enabled));
                foreach (var skuOrdered in skus)
                {
                    MasterTmsUdoItemInfo item=new MasterTmsUdoItemInfo();
                    item.ITEM = skuOrdered.Sku.Code;
                    item.AMT = skuOrdered.Amount.ToString("F");
                    item.QTY = skuOrdered.Quantity.ToString();

                    info.PRODUCTLIST.Add(item);
                }

                var data = JsonConvert.SerializeObject(info);
                toReturn.MasterTmsUdo = $"var MasterTmsUdo = {{ 'CJ' : {data} }};";
            }

            return View("~/Views/Shared/Components/Tracking/HeadTrackScripts.cshtml", toReturn);
        }
    }
}