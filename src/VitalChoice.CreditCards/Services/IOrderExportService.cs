using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;

namespace VitalChoice.CreditCards.Services
{
    public interface IOrderExportService
    {
        Task ExportOrders(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack);
    }
}