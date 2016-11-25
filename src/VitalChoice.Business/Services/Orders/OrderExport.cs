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
using VitalChoice.Infrastructure.Services;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderExport : IOrderExport
    {
        private readonly List<ExportResult> _exportResults = new List<ExportResult>();
        private readonly ConcurrentDictionary<int, ExportSide> _exportedOrders = new ConcurrentDictionary<int, ExportSide>();
        private readonly ILifetimeScope _rootScope;
        private readonly ExportPool _exportPool;
        private volatile int _totalExporting;
        private volatile int _totalExported;
        private volatile int _exportErrors;
        private readonly BasicTimer _timer;

        public OrderExport(ILifetimeScope rootScope, ILoggerFactory loggerFactory, IAdminEditLockService lockService)
        {
            var logger = loggerFactory.CreateLogger<OrderExport>();
            _exportPool = new ExportPool(logger, rootScope, _exportedOrders, lockService);
            _rootScope = rootScope;
            _timer = new BasicTimer(RefreshStatistics, TimeSpan.FromSeconds(5), e => logger.LogError(e.ToString()));
        }

        public int TotalExporting => _totalExporting;

        public int TotalExported => _totalExported;

        public int ExportErrors => _exportErrors;

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

        private void RefreshStatistics()
        {
            var exportErrors = 0;
            var totalExported = 0;
            var totalExporting = 0;
            lock (_exportResults)
            {
                ClearExpiredList();
                foreach (var result in _exportResults)
                {
                    lock (result)
                    {
                        if (result.TotalCount == result.ExportedOrders.Count)
                        {
                            exportErrors += result.ExportedOrders.Count(e => !e.Success);
                        }
                        totalExported += result.ExportedOrders.Count;
                        totalExporting += result.TotalCount;
                    }
                }
                _totalExporting = totalExporting;
                _totalExported = totalExported;
                _exportErrors = exportErrors;
            }
        }

        private void ClearExpiredList()
        {
            DateTime hourAgo = DateTime.Now.AddHours(-1);
            if (_exportResults.All(r => r.DateStarted < hourAgo))
            {
                foreach (var result in _exportResults)
                {
                    foreach (var order in result.ExportedOrders)
                    {
                        ExportSide side;
                        _exportedOrders.TryRemove(order.Id, out side);
                    }
                }
                _exportResults.Clear();
            }
        }

        private class ExportPool : RoundRobinAbstractPool<OrderExportData>
        {
            private readonly ILifetimeScope _rootScope;
            private readonly IDictionary<int, ExportSide> _exportLockList;
            private readonly IAdminEditLockService _lockService;

            public ExportPool(ILogger logger, ILifetimeScope rootScope, IDictionary<int, ExportSide> exportLockList,
                IAdminEditLockService lockService)
                : base(1, logger)
            {
                _rootScope = rootScope;
                _exportLockList = exportLockList;
                _lockService = lockService;
            }

            protected override void ProcessingAction(OrderExportData data, object localData, object processParameter)
            {
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
                                var order = data.ExportInfo.FirstOrDefault(o => o.Id == r.Id);
                                if (order != null)
                                {
                                    ProcessOrderUnlockAfterExporting(r.Id, order.OrderType);
                                }
                                lock (result)
                                {
                                    result.ExportedOrders.Add(r);
                                }

                                if (r.Success)
                                    return;

                                if (order == null)
                                    return;
                                ProcessFailingOrder(order);
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
                                    ProcessOrderUnlockAfterExporting(order.Id, order.OrderType);
                                    ProcessFailingOrder(order);
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

            private void ProcessOrderUnlockAfterExporting(int idOrder, ExportSide type)
            {
                ExportSide side;
                switch (type)
                {
                    case ExportSide.All:
                        _lockService.ExportOrderEditLockRelease(idOrder);
                        break;
                    case ExportSide.Perishable:
                        if (_exportLockList.TryGetValue(idOrder, out side))
                        {
                            switch (side)
                            {
                                case ExportSide.Perishable:
                                    _lockService.ExportOrderEditLockRelease(idOrder);
                                    break;
                            }
                        }
                        break;
                    case ExportSide.NonPerishable:
                        if (_exportLockList.TryGetValue(idOrder, out side))
                        {
                            switch (side)
                            {
                                case ExportSide.NonPerishable:
                                    _lockService.ExportOrderEditLockRelease(idOrder);
                                    break;
                            }
                        }
                        break;
                }
            }

            private void ProcessFailingOrder(OrderExportItem order)
            {
                ExportSide side;
                switch (order.OrderType)
                {
                    case ExportSide.All:
                        _exportLockList.TryRemove(order.Id, out side);
                        break;
                    case ExportSide.Perishable:
                        if (_exportLockList.TryGetValue(order.Id, out side))
                        {
                            switch (side)
                            {
                                case ExportSide.Perishable:
                                    _exportLockList.TryRemove(order.Id, out side);
                                    break;
                                case ExportSide.All:
                                    _exportLockList[order.Id] = ExportSide.NonPerishable;
                                    break;
                            }
                        }
                        break;
                    case ExportSide.NonPerishable:
                        if (_exportLockList.TryGetValue(order.Id, out side))
                        {
                            switch (side)
                            {
                                case ExportSide.NonPerishable:
                                    _exportLockList.TryRemove(order.Id, out side);
                                    break;
                                case ExportSide.All:
                                    _exportLockList[order.Id] = ExportSide.Perishable;
                                    break;
                            }
                        }
                        break;
                }
            }
        }

        public void Dispose()
        {
            _timer.Dispose();
            _exportPool.Dispose();
        }
    }
}