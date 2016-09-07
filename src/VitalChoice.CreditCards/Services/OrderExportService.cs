using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Lifetime;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.CreditCards.Services
{
    public class OrderExportService : IOrderExportService
    {
        private readonly IVeraCoreExportService _veraCoreExportService;
        private readonly IOrderService _orderService;
        private readonly IOrderRefundService _refundService;
        private readonly ICustomerService _customerService;
        private readonly ILifetimeScope _scope;

        public OrderExportService(IVeraCoreExportService veraCoreExportService, IOrderService orderService,
            IOrderRefundService refundService, ICustomerService customerService, ILifetimeScope scope)
        {
            _veraCoreExportService = veraCoreExportService;
            _orderService = orderService;
            _refundService = refundService;
            _customerService = customerService;
            _scope = scope;
        }

        public async Task ExportOrders(ICollection<OrderExportItem> exportItems,
            Action<OrderExportItemResult> exportCallBack)
        {
            await DoExportOrders(exportItems, exportCallBack);
        }

        private async Task DoExportOrders(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack)
        {
            var orders = exportItems.Where(i => !i.IsRefund).ToDictionary(o => o.Id);

            var orderList =
                new HashSet<OrderDynamic>(
                    await _orderService.SelectAsync(exportItems.Where(i => !i.IsRefund).Select(o => o.Id).ToList(), true));
            var customerList =
                (await
                        _customerService.SelectAsync(
                            orderList.Select(o => o.Customer.Id).Distinct().ToList(), true))
                    .ToDictionary(c => c.Id);

            foreach (var order in orderList)
            {
                order.Customer = customerList.GetValueOrDefault(order.Customer.Id) ?? order.Customer;
            }

            var rootScope = ((LifetimeScope) _scope).RootLifetimeScope;
            Parallel.ForEach(orderList, new ParallelOptions
            {
                MaxDegreeOfParallelism = 16,
                CancellationToken = CancellationToken.None,
                TaskScheduler = TaskScheduler.Default
            }, order =>
            {
                using (var scope = rootScope.BeginLifetimeScope())
                {
                    var veracoreExportService = scope.Resolve<IVeraCoreExportService>();
                    try
                    {
                        veracoreExportService.ExportOrder(order, orders[order.Id].OrderType).GetAwaiter().GetResult();
                        exportCallBack(new OrderExportItemResult
                        {
                            Id = order.Id,
                            Success = true
                        });
                    }
                    catch (Exception e)
                    {
                        exportCallBack(new OrderExportItemResult
                        {
                            Error = e.ToString(),
                            Id = order.Id,
                            Success = false
                        });
                    }
                }
            });
            await _orderService.UpdateRangeAsync(orderList);
            exportCallBack(new OrderExportItemResult
            {
                Id = -1,
                Success = true
            });
        }

        private async Task DoExportRefunds(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack)
        {
            var refundList =
                new HashSet<OrderRefundDynamic>(
                    await _refundService.SelectAsync(exportItems.Where(i => i.IsRefund).Select(o => o.Id).ToList(), true));
            var refundCustomerList =
                (await
                        _customerService.SelectAsync(
                            refundList.Select(o => o.Customer.Id).Distinct().ToList(), true))
                    .ToDictionary(c => c.Id);

            foreach (var refund in refundList)
            {
                refund.Customer = refundCustomerList.GetValueOrDefault(refund.Customer.Id) ?? refund.Customer;
            }

            await refundList.ToArray().ForEachAsync(async refund =>
            {
                try
                {
                    await _veraCoreExportService.ExportRefund(refund);
                    exportCallBack(new OrderExportItemResult
                    {
                        Id = refund.Id,
                        Success = true
                    });
                }
                catch (Exception e)
                {
                    refundList.Remove(refund);
                    exportCallBack(new OrderExportItemResult
                    {
                        Error = e.ToString(),
                        Id = refund.Id,
                        Success = false
                    });
                }
            });

            await _refundService.UpdateRangeAsync(refundList);
        }
    }
}