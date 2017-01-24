using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Constants
{
    public static class CheckoutConstants
    {
        public const string CartUidCookieName = "CartUid";
        public const string CustomerAuthToken = "AuthToken";
        public const string ReceiptSessionOrderIds = "ReceiptIds";
    }
}