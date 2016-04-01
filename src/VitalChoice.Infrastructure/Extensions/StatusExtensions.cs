using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
