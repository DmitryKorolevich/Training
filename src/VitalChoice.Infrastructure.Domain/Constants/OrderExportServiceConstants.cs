using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Constants
{
    public static class OrderExportServiceConstants
    {
        public const string GetPublicKey = "application/get-public-key";
        public const string SetSessionKey = "application/set-session-key";
        public const string CheckSessionKey = "application/check-session-key";
        public const string RemoveSessionKey = "application/remove-session-key";
        public const string ExportOrder = "application/order-export";
        public const string UpdateOrderPayment = "application/update-order-payment";
        public const string UpdateCustomerPayment = "application/update-customer-payment";
    }
}