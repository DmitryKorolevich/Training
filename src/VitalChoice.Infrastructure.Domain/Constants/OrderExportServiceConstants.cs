using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Constants
{
    public static class OrderExportServiceConstants
    {
        public const string ExportOrder = "application/order-export";
        public const string UpdateOrderPayment = "application/update-order-payment";
        public const string UpdateCustomerPayment = "application/update-customer-payment";
    }
}