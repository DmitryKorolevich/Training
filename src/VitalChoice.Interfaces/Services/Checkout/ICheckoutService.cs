using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;

namespace VitalChoice.Interfaces.Services.Checkout
{
    public interface ICheckoutService
    {
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, bool loggedIn, bool withMultipleShipmentsService = false);
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, int idCustomer, bool withMultipleShipmentsService = false);
        Task<bool> UpdateCart(CustomerCartOrder anonymCart, bool withMultipleShipmentsService = false);
        Task<int?> SaveOrder(OrderDynamic order, Guid idCart);
        Task<ICollection<int>> SaveOrdersForTheSameCustomer(IList<OrderDynamic> orders, CustomerDynamic customer, Guid idCart);
        Task<int> GetCartItemsCount(Guid uid);
    }
}