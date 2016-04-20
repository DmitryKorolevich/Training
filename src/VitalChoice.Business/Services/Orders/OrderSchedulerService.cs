using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using Microsoft.Extensions.Logging;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderSchedulerService : IOrderSchedulerService
    {
        private readonly OrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly ILogger _logger;

        public OrderSchedulerService(
            OrderService orderService,
            ICustomerService customerService,
            ILoggerProviderExtended loggerProvider)
        {
            _orderService = orderService;
            _customerService = customerService;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task UpdateShipDelayedOrders()
        {
            DateTime now = DateTime.Now;

            try
            {
                var shipDelayedOrders = await _orderService.SelectAsync(p => p.StatusCode != (int)RecordStatusCode.Deleted &&
                  p.OrderStatus == OrderStatus.ShipDelayed || p.POrderStatus == OrderStatus.ShipDelayed || p.NPOrderStatus == OrderStatus.ShipDelayed);

                List<OrderDynamic> ordersForUpdate = new List<OrderDynamic>();

                bool pPartNeedUpdate = false;
                bool npPartNeedUpdate = false;
                foreach (var shipDelayedOrder in shipDelayedOrders)
                {
                    pPartNeedUpdate = false;
                    npPartNeedUpdate = false;
                    if (shipDelayedOrder.OrderStatus == OrderStatus.ShipDelayed && shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder
                        && shipDelayedOrder.SafeData.ShipDelayDate != null && shipDelayedOrder.SafeData.ShipDelayDate < now)
                    {
                        shipDelayedOrder.OrderStatus = OrderStatus.Processed;
                        shipDelayedOrder.Data.ShipDelayType = null;
                        shipDelayedOrder.Data.ShipDelayDate = null;
                        ordersForUpdate.Add(shipDelayedOrder);
                    }


                    if (shipDelayedOrder.POrderStatus == OrderStatus.ShipDelayed &&
                        ((shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder && shipDelayedOrder.SafeData.ShipDelayDate != null && shipDelayedOrder.SafeData.ShipDelayDate < now)
                        || (shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.PerishableAndNonPerishable && shipDelayedOrder.SafeData.ShipDelayDateP != null && shipDelayedOrder.SafeData.ShipDelayDateP < now)))
                    {
                        shipDelayedOrder.POrderStatus = OrderStatus.Processed;
                        shipDelayedOrder.Data.ShipDelayDateP = null;
                        pPartNeedUpdate = true;
                    }

                    if (shipDelayedOrder.NPOrderStatus == OrderStatus.ShipDelayed &&
                        ((shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder && shipDelayedOrder.SafeData.ShipDelayDate != null && shipDelayedOrder.SafeData.ShipDelayDate < now)
                        || (shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.PerishableAndNonPerishable && shipDelayedOrder.SafeData.ShipDelayDateNP != null && shipDelayedOrder.SafeData.ShipDelayDateNP < now)))
                    {
                        shipDelayedOrder.NPOrderStatus = OrderStatus.Processed;
                        shipDelayedOrder.Data.ShipDelayDateNP = null;
                        npPartNeedUpdate = true;
                    }

                    if (pPartNeedUpdate || npPartNeedUpdate)
                    {
                        if (ordersForUpdate.FirstOrDefault(p => p.Id == shipDelayedOrder.Id) == null)
                        {
                            ordersForUpdate.Add(shipDelayedOrder);
                        }

                        //update common part if needed
                        if (shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder && shipDelayedOrder.POrderStatus != OrderStatus.ShipDelayed
                            && shipDelayedOrder.NPOrderStatus != OrderStatus.ShipDelayed)
                        {
                            shipDelayedOrder.Data.ShipDelayDate = null;
                            shipDelayedOrder.Data.ShipDelayType = null;
                        }

                        if (shipDelayedOrder.SafeData.ShipDelayType == (int)ShipDelayType.PerishableAndNonPerishable && shipDelayedOrder.POrderStatus != OrderStatus.ShipDelayed
                            && shipDelayedOrder.NPOrderStatus != OrderStatus.ShipDelayed)
                        {
                            shipDelayedOrder.Data.ShipDelayType = null;
                        }
                    }
                }

                List<OrderDynamic> ordersForUpdateAfterCalculating = new List<OrderDynamic>();

                if (ordersForUpdate.Count != 0)
                {
                    foreach (var order in ordersForUpdate)
                    {
                        ordersForUpdateAfterCalculating.Add(order);
                        //try
                        //{
                        //    order.Customer = await _customerService.SelectAsync(order.Customer.Id);
                        //    var context = await _orderService.CalculateOrder(order, OrderStatus.Processed);

                        //    List<MessageInfo> messages =new List<MessageInfo>(context.Messages);
                        //    messages.AddRange(context.SkuOrdereds.Where(p => p.Messages != null).SelectMany(p => p.Messages));
                        //    messages.AddRange(context.PromoSkus.Where(p => p.Enabled && p.Messages != null).SelectMany(p => p.Messages));
                        //    if(messages.Count>0)
                        //    {
                        //        //TODO - add additioanl logic for this case
                        //    }
                        //    else
                        //    {
                        //        ordersForUpdateAfterCalculating.Add(context.Order);
                        //    }
                        //}
                        //catch (Exception e)
                        //{
                        //    _logger.LogCritical($"Error till ShipDelayed orders updating - calculating order({order.Id})", e);
                        //}
                    }
                }

                if (ordersForUpdateAfterCalculating.Count != 0)
                {
                    await _orderService.UpdateRangeAsync(ordersForUpdateAfterCalculating);
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical("Error till ShipDelayed orders updating", e);
            }
        }
    }
}