using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Helpers;
using VC.Public.Models.Menu;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Products;

namespace VC.Public.Components.Cart
{
    [ViewComponent(Name = "CartLite")]
    public class CartLiteViewComponent : ViewComponent
    {
        private readonly ICheckoutService _checkoutService;

        public CartLiteViewComponent(ICheckoutService checkoutService)
        {
            _checkoutService = checkoutService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var uid = Request.GetCartUid();
            return Content(uid == null ? "0" : (await _checkoutService.GetCartItemsCount(uid.Value)).ToString());
        }
    }
}