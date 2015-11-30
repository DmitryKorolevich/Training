using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
    public class OrderExportItem
    {
        public int Id { get; set; }

        public POrderType OrderType { get; set; }
    }

    public class OrderExportData
    {
        public ICollection<OrderExportItem> ExportInfo { get; set; }
    }

    public class OrderExportItemResult
    {
        public int Id { get; set; }

        public ICollection<string> Errors { get; set; }

        public bool Success { get; set; }
    }
}
