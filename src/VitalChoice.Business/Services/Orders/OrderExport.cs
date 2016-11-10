using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Utils;
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
                var totalCount = 0;
                lock (_exportResults)
                {
                    foreach (var result in _exportResults)
                    {
                        lock (result)
                        {
                            totalCount += result.TotalCount;
                        }
                    }
                }
                return totalCount;
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

        public int ExportErrors
        {
            get
            {
                var totalCount = 0;
                lock (_exportResults)
                {
                    foreach (var result in _exportResults.Where(r => r.TotalCount == r.ExportedOrders.Count))
                    {
                        lock (result)
                        {
                            totalCount += result.ExportedOrders.Count(o => !o.Success);
                        }
                    }
                }
                return totalCount;
            }
        }

        private readonly List<ExportResult> _exportResults = new List<ExportResult>();
        private readonly ConcurrentDictionary<int, ExportSide> _exportedOrders = new ConcurrentDictionary<int, ExportSide>();
        private readonly ILifetimeScope _rootScope;
        private readonly ExportPool _exportPool;

        public OrderExport(ILifetimeScope rootScope, ILoggerFactory loggerFactory)
        {
            _exportPool = new ExportPool(loggerFactory.CreateLogger<OrderExport>(), rootScope, _exportedOrders);
            _rootScope = rootScope;
        }

        public async Task ExportOrders(OrderExportData exportData)
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
                    _exportedOrders.TryAdd(data.Id, data.OrderType);
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
                return _exportResults.Where(r =>
                {
                    lock (r)
                    {
                        return r.TotalCount == r.ExportedOrders.Count && r.ExportedOrders.Any(o => !o.Success);
                    }
                }).OrderByDescending(r => r.DateStarted).ToList();
            }
        }

        public void ClearDone(DateTime loadTimestamp)
        {
            loadTimestamp = loadTimestamp.AddSeconds(1);
            lock (_exportResults)
            {
                foreach (var result in _exportResults.ToArray())
                {
                    lock (result)
                    {
                        if (result.DateStarted <= loadTimestamp && result.TotalCount == result.ExportedOrders.Count)
                        {
                            _exportResults.Remove(result);
                        }
                    }
                }
            }
        }

        public bool GetIsOrderExporting(int id) => _exportedOrders.ContainsKey(id);

        private class ExportPool : RoundRobinAbstractPool<OrderExportData>
        {
            private readonly ILifetimeScope _rootScope;

            public ExportPool(ILogger logger, ILifetimeScope rootScope, IDictionary<int, ExportSide> exportLockList)
                : base(1, logger, () => exportLockList)
            {
                _rootScope = rootScope;
            }

            protected override void ProcessingAction(OrderExportData data, object localData, object processParameter)
            {
                var lockList = (ConcurrentDictionary<int, ExportSide>) localData;
                var result = (ExportResult) processParameter;
                result.DateStarted = DateTime.Now;
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

                                if (r.Success)
                                    return;

                                var order = data.ExportInfo.FirstOrDefault(o => o.Id == r.Id);
                                if (order == null)
                                    return;
                                ProcessFailingOrder(lockList, order);
                            }).WaitResult();
                        }
                        catch (Exception e)
                        {
                            var errorText = e.ToString();
                            Logger.LogError(errorText);
                            lock (result)
                            {
                                foreach (var order in data.ExportInfo.Where(o => result.ExportedOrders.All(eo => eo.Id != o.Id)))
                                {
                                    ProcessFailingOrder(lockList, order);
                                    result.ExportedOrders.Add(new OrderExportItemResult
                                    {
                                        Error = errorText,
                                        Success = false,
                                        Id = order.Id
                                    });
                                }
                            }
                        }
                    }
                }
            }

            private static void ProcessFailingOrder(ConcurrentDictionary<int, ExportSide> lockList, OrderExportItem order)
            {
                ExportSide side;
                switch (order.OrderType)
                {
                    case ExportSide.All:
                        lockList.TryRemove(order.Id, out side);
                        break;
                    case ExportSide.Perishable:
                        if (lockList.TryGetValue(order.Id, out side))
                        {
                            switch (side)
                            {
                                case ExportSide.Perishable:
                                    lockList.TryRemove(order.Id, out side);
                                    break;
                                case ExportSide.All:
                                    lockList[order.Id] = ExportSide.NonPerishable;
                                    break;
                            }
                        }
                        break;
                    case ExportSide.NonPerishable:
                        if (lockList.TryGetValue(order.Id, out side))
                        {
                            switch (side)
                            {
                                case ExportSide.NonPerishable:
                                    lockList.TryRemove(order.Id, out side);
                                    break;
                                case ExportSide.All:
                                    lockList[order.Id] = ExportSide.Perishable;
                                    break;
                            }
                        }
                        break;
                }
            }
        }
    }
}