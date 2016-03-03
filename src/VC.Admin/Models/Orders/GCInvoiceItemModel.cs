using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VC.Admin.Models.Orders
{
    public class GCInvoiceItemModel
    {
        public string Code { get; set; }

        public decimal Amount { get; set; }
    }
}