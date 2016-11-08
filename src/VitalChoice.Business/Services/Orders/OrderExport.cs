using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.LoadBalancing;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderExport : IOrderExport
    {
        public int TotalExporting
        {
            get
            {
                lock (_exportedOrders)
                {
                    return _exportedOrders.Count;
                }
            }
        }

        public int TotalExported
        {
            get
            {
                var totalCount = 0;
                lock (_exportResults)
                {
                    foreach (var result in _exportResults)
                    {
                        lock (result)
                        {
                            totalCount += result.ExportedOrders.Count;
                        }
                    }
                }
                return totalCount;
            }
        }

        private readonly List<ExportResult> _exportResults = new List<ExportResult>();
        private readonly Dictionary<int, ExportSide> _exportedOrders = new Dictionary<int, ExportSide>();
        private readonly ILifetimeScope _rootScope;
        private readonly ExportPool _exportPool;

        public OrderExport(ILifetimeScope rootScope, ILoggerFactory loggerFactory)
        {
            _exportPool = new ExportPool(loggerFactory.CreateLogger<OrderExport>(), rootScope);
            _rootScope = rootScope;
        }

        public async Task ExportOrders(OrderExportData exportData)
        {
            lock (_exportedOrders)
            {
                foreach (var data in exportData.ExportInfo.ToArray())
                {
                    ExportSide side;
                    if (_exportedOrders.TryGetValue(data.Id, out side))
                    {
                        if (side == data.OrderType)
                        {
                            exportData.ExportInfo.Remove(data);
                        }
                        else
                        {
                            switch (side)
                            {
                                case ExportSide.All:
                                    exportData.ExportInfo.Remove(data);
                                    break;
                                case ExportSide.Perishable:
                                    _exportedOrders[data.Id] = ExportSide.All;
                                    break;
                                case ExportSide.NonPerishable:
                                    _exportedOrders[data.Id] = ExportSide.All;
                                    break;
                            }
                        }
                    }
                    else
                    {
                        _exportedOrders.Add(data.Id, data.OrderType);
                    }
                }
            }
            if (exportData.ExportInfo.Count > 0)
            {
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    var agentService = scope.Resolve<IAgentService>();
                    var agents = await agentService.GetAgents(new List<int>
                    {
                        exportData.UserId
                    });

                    var exportResult = new ExportResult(agents.GetValueOrDefault(exportData.UserId), exportData.ExportInfo.Count);
                    lock (_exportResults)
                    {
                        _exportResults.Add(exportResult);
                    }
                    _exportPool.EnqueueData(exportData, exportResult);
                }
            }
        }

        public IReadOnlyList<ExportResult> GetExportResults()
        {
            lock (_exportResults)
            {
                return _exportResults.ToList();
            }
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
                        try
                        {
                            exportService.ExportOrdersAsync(data, r =>
                            {
                                lock (result)
                                {
                                    result.ExportedOrders.Add(r);
                                }
                            }).GetAwaiter().GetResult();
                        }
                        catch (Exception e)
                        {
                            lock (result)
                            {
                                result.ExportedOrders.Add(new OrderExportItemResult
                                {
                                    Error = e.ToString(),
                                    Success = false
                                });
                            }
                        }
                    }
                }
            }
        }
    }
}