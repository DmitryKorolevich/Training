using System;
using Microsoft.AspNetCore.Http;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VitalChoice.Core.Infrastructure.Helpers
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

        public static void SetCartUid(this HttpContext context, Guid uid)
        {
            context.Items[CheckoutConstants.CartUidCookieName] = uid;
            context.Response.Cookies.Delete(CheckoutConstants.CartUidCookieName);
            context.Response.Cookies.Append(CheckoutConstants.CartUidCookieName, uid.ToString(), new CookieOptions
            {
                Expires = DateTime.Now.AddYears(1)
            });
        }
    }
}