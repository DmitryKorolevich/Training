using System;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VC.Admin.Models.Orders
{
    public class OrderExportGeneralStatusModel
    {
        public int Exported { get; set; }

        public int All { get; set; }

        public int Percent => All!=0 ? (int)Math.Floor((double)Exported*100/ All) : 0;
    }
}