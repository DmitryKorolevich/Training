using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Interfaces.Services.Orders
{
    public interface IOrderExport
    {
        int TotalExporting { get; }
        int TotalExported { get; }
        Task ExportOrders(OrderExportData exportData);
        IReadOnlyList<ExportResult> GetExportResults();
    }
}