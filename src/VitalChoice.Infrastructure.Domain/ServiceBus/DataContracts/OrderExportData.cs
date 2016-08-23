using System;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts
{
#if !NETSTANDARD1_5
    [Serializable]
#endif
    public class OrderExportItem
    {
        public int Id { get; set; }

        public ExportSide OrderType { get; set; }

        public bool IsRefund { get; set; }
    }

#if !NETSTANDARD1_5
    [Serializable]
#endif
    public class OrderExportData
    {
        public ICollection<OrderExportItem> ExportInfo { get; set; }
    }

#if !NETSTANDARD1_5
    [Serializable]
#endif
    public class OrderExportItemResult
    {
        public int Id { get; set; }

        public string Error { get; set; }

        public bool Success { get; set; }
    }
}
