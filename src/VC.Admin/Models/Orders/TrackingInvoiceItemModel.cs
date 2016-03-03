using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VC.Admin.Models.Orders
{
    public class TrackingInvoiceItemModel
    {
        public string TrackingNumber { get; set; }

        public string Sku { get; set; }

        public string ServiceUrl { get; set; }
    }
}