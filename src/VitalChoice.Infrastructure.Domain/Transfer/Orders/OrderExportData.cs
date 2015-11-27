using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
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
