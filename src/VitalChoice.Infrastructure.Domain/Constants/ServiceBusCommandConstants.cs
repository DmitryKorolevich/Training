using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Constants
{
    public static class OrderExportServiceCommandConstants
    {
        public const string ExportOrder = "application/order-export";
        public const string CardExist = "application/check-customer-card";
        public const string AuthorizeCard = "application/authorize-card";
        public const string AuthorizeCardInOrder = "application/authorize-card-in-order";
        public const string UpdateOrderPayment = "application/update-order-payment";
        public const string UpdateCustomerPayment = "application/update-customer-payment";
    }

    public static class ServiceBusCommandConstants
    {
        public const string GetPublicKey = "application/get-public-key";
        public const string SetSessionKey = "application/set-session-key";
        public const string CheckSessionKey = "application/check-session-key";
        public const string SessionExpired = "application/session-key-expired";
    }
}