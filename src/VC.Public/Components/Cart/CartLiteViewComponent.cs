using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using VC.Public.Helpers;
using VitalChoice.Interfaces.Services.Checkout;

namespace VC.Public.Components.Cart
{
    [ViewComponent(Name = "CartLite")]
    public class CartLiteViewComponent : ViewComponent
    {
        private readonly ICheckoutService _checkoutService;
        private readonly IActionContextAccessor _actionContextAccessor;

        public CartLiteViewComponent(ICheckoutService checkoutService, IActionContextAccessor actionContextAccessor)
        {
            _checkoutService = checkoutService;
            _actionContextAccessor = actionContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var uid = _actionContextAccessor.ActionContext.HttpContext.GetCartUid();
            return Content(uid == null ? "0" : (await _checkoutService.GetCartItemsCount(uid.Value)).ToString());
        }
    }
}