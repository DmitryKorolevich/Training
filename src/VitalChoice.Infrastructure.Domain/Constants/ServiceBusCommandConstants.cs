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
        public const string ExportGiftListCard = "application/export-gift-list-card";
    }

    public static class OrderExportProcessCommandConstants
    {
        public const string OrderExportStarted = "application/export-item-started";
        public const string OrderExportFinished = "application/export-item-finished";
    }

    public static class ServiceBusCommandConstants
    {
        public const string GetPublicKey = "session/get-public-key";
        public const string SetSessionKey = "session/set-session-key";
        public const string CheckSessionKey = "session/check-session-key";
    }
}