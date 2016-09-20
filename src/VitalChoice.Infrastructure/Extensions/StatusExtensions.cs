using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Extensions
{
    public static class StatusExtensions
    {
        public static bool IsAnyIncomplete(this Order order)
        {
            return order.OrderStatus == OrderStatus.Incomplete || order.POrderStatus == OrderStatus.Incomplete ||
                   order.NPOrderStatus == OrderStatus.Incomplete;
        }

        public static bool IsAnyNotIncomplete(this Order order)
        {
            return !order.IsAnyIncomplete();
        }

        public static bool IsAnyIncomplete(this OrderDynamic order)
        {
            return order.OrderStatus == OrderStatus.Incomplete || order.POrderStatus == OrderStatus.Incomplete ||
                   order.NPOrderStatus == OrderStatus.Incomplete;
        }

        public static bool IsAnyNotIncomplete(this OrderDynamic order)
        {
            return !order.IsAnyIncomplete();
        }

        public static bool IsAnyShipDelayed(this OrderDynamic order)
        {
            return order.OrderStatus == OrderStatus.ShipDelayed || order.POrderStatus == OrderStatus.ShipDelayed ||
                   order.NPOrderStatus == OrderStatus.ShipDelayed;
        }

        public static bool IsAnyNotShipDelayed(this OrderDynamic order)
        {
            return !order.IsAnyShipDelayed();
        }
    }
}
