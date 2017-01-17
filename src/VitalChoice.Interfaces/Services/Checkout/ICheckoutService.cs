using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;

namespace VitalChoice.Interfaces.Services.Checkout
{
    public interface ICheckoutService
    {
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, bool loggedIn, bool withMultipleShipmentsService = false);
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, int idCustomer, bool withMultipleShipmentsService = false);
        Task<bool> UpdateCart(CustomerCartOrder anonymCart, bool withMultipleShipmentsService = false);
        Task<bool> SaveOrder(CustomerCartOrder anonymCart);
        Task<int> GetCartItemsCount(Guid uid);
    }
}