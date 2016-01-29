using System;
using Microsoft.AspNet.Http;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Helpers
{
    public static class CartHelperExtension
    {
        public static Guid? GetCartUid(this HttpRequest request)
        {
            var cartUidString = request.Cookies[CheckoutConstants.CartUidCookieName];
            Guid? existingUid = null;
            Guid cartUid;
            if (cartUidString.Count > 0 && Guid.TryParse(cartUidString, out cartUid))
            {
                existingUid = cartUid;
            }
            return existingUid;
        }
    }
}