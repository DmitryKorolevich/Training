using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VC.Public.Models.Tracking;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VitalChoice.Infrastructure.Domain.Options;

namespace VC.Public.Components.Tracking
{
    [ViewComponent(Name = "HeadTrackScripts")]
    public class HeadTrackScriptsComponent : ViewComponent
    {
        private readonly Lazy<IAuthorizationService> _authorizationService;
        private readonly Lazy<ICheckoutService> _checkoutService;
        private readonly Lazy<IOrderService> _orderService;
        private readonly Lazy<ICustomerService> _customerService;
        private readonly Lazy<ReferenceData> _referenceData;
        private readonly Lazy<IOptions<AppOptions>> _appOptions;

        public HeadTrackScriptsComponent(Lazy<IOptions<AppOptions>> appOptions, Lazy<ReferenceData> referenceData,
            Lazy<ICustomerService> customerService, Lazy<IOrderService> orderService, Lazy<ICheckoutService> checkoutService,
            Lazy<IAuthorizationService> authorizationService)
        {
            _appOptions = appOptions;
            _referenceData = referenceData;
            _customerService = customerService;
            _orderService = orderService;
            _checkoutService = checkoutService;
            _authorizationService = authorizationService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var toReturn = new HeadTrackScriptsModel();

            await toReturn.SetBaseOptions(HttpContext, _orderService, _authorizationService, _checkoutService, _customerService,
                _referenceData);

            if (toReturn.OrderCompleteStep && toReturn.Order != null && _appOptions.Value.Value.EnableOrderTrackScripts)
            {
                toReturn.EnableOrderCompleteTrack = true;
                MasterTmsUdoInfo info = new MasterTmsUdoInfo();
                info.CID = "1531092";
                info.DISCOUNT = toReturn.Order.DiscountTotal.ToString("F");
                info.OID = toReturn.Order.Id.ToString();
                info.CURRENCY = "USD";
                info.COUPON = toReturn.Order.Discount?.Code ?? "";
                info.FIRECJ = Request.Cookies.ContainsKey("source") && Request.Cookies["source"] == "CJ" ?
                    "TRUE" : "FALSE";

                var count = await _customerService.Value.GetActiveOrderCount(toReturn.Order.Customer.Id);
                info.TYPE = count > 1 ? "373119" : "373118";

                var skus = toReturn.Order.Skus;
                skus.AddRange(toReturn.Order.PromoSkus.Where(p => p.Enabled));
                foreach (var skuOrdered in skus)
                {
                    MasterTmsUdoItemInfo item = new MasterTmsUdoItemInfo();
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