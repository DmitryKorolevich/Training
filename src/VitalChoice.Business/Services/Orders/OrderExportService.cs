using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.LoadBalancing;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderExportService
    {
        private int _totalExporting;
        private int _totalExported;
        private readonly List<ExportResult> _exportResults = new List<ExportResult>();
        private readonly Dictionary<int, ExportSide> _exportedOrders = new Dictionary<int, ExportSide>();
        private readonly ILifetimeScope _rootScope;
        private readonly RoundRobinAbstractPool<>
        

        public OrderExportService(ILifetimeScope rootScope)
        {
            _rootScope = rootScope;
        }

        public void ExportOrders(OrderExportData exportData)
        {
            _exportQue.Enqueue(exportData);
        }

        private class ExportResult
        {
            public ExportResult(string agentId, int totalCount)
            {
                AgentId = agentId;
                TotalCount = totalCount;
            }

            public DateTime DateCreated { get; } = DateTime.Now;
            public string AgentId { get; }
            public int TotalCount { get; }
            public List<OrderExportItemResult> ExportedOrders { get; } = new List<OrderExportItemResult>();
        }

        private class ExportPool : RoundRobinAbstractPool<OrderExportData>
        {
            private readonly ILifetimeScope _rootScope;

            public ExportPool(ILogger logger, ILifetimeScope rootScope) : base(1, logger)
            {
                _rootScope = rootScope;
            }

            protected override void ProcessingAction(OrderExportData data, object localData, object processParameter)
            {
                var result = (ExportResult) processParameter;
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    using (var exportService = scope.Resolve<IEncryptedOrderExportService>())
                    {
                        exportService.ExportOrdersAsync(data, r =>
                        {
                            lock (result)
                            {
                                result.ExportedOrders.Add(r);
                            }
                        }).GetAwaiter().GetResult();
                    }
                }
            }
        }
    }
}