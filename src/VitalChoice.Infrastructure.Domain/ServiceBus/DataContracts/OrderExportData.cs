using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts
{
    [DataContract]
    public class OrderExportItem
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public ExportSide OrderType { get; set; }

        [DataMember]
        public bool IsRefund { get; set; }
    }

    [DataContract]
    public class OrderExportData
    {
        [DataMember]
        public int UserId { get; set; }

        [DataMember]
        public List<OrderExportItem> ExportInfo { get; set; }
    }

    [DataContract]
    public class OrderExportItemResult
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Error { get; set; }

        [DataMember]
        public bool Success { get; set; }
    }
}