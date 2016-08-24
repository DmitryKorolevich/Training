using System;
using Microsoft.AspNetCore.Http;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Helpers
{
    public static class CartHelperExtension
    {
        public static Guid? GetCartUid(this HttpContext context)
        {
            if (context.Items.ContainsKey(CheckoutConstants.CartUidCookieName))
            {
                return (Guid) context.Items[CheckoutConstants.CartUidCookieName];
            }

            var cartUidString = context.Request.Cookies[CheckoutConstants.CartUidCookieName];
            Guid cartUid;
            if (Guid.TryParse(cartUidString, out cartUid))
            {
                context.Items[CheckoutConstants.CartUidCookieName] = cartUid;
                return cartUid;
            }
            return null;
        }
    }
}