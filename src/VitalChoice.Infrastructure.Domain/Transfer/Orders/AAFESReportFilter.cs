using System;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class AAFESReportFilter : FilterBase
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public DateTime ShipFrom { get; set; }
        public DateTime ShipTo { get; set; }
    }
}