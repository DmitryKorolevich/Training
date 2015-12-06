using System;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.ServiceBus
{
#if DNX451 || NET451
    [Serializable]
#endif
    public class OrderExportItem
    {
        public int Id { get; set; }

        public POrderType OrderType { get; set; }
    }

#if DNX451 || NET451
    [Serializable]
#endif
    public class OrderExportData
    {
        public ICollection<OrderExportItem> ExportInfo { get; set; }
    }

#if DNX451 || NET451
    [Serializable]
#endif
    public class OrderExportItemResult
    {
        public int Id { get; set; }

        public ICollection<string> Errors { get; set; }

        public bool Success { get; set; }
    }
}
