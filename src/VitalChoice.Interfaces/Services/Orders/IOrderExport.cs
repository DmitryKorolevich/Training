using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Interfaces.Services.Orders
{
    public interface IOrderExport: IDisposable
    {
        int TotalExporting { get; }
        int TotalExported { get; }
        int ExportErrors { get; }
        Task ExportOrders(OrderExportData exportData);
        IReadOnlyList<ExportResult> GetExportResults();
        void ClearDone(DateTime loadTimestamp);
        bool GetIsOrderExporting(int id);
    }
}