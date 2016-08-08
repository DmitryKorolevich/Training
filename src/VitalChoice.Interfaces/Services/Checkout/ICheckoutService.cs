using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;

namespace VitalChoice.Interfaces.Services.Checkout
{
    public interface ICheckoutService
    {
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, bool loggedIn);
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, int idCustomer);
        Task<bool> UpdateCart(CustomerCartOrder anonymCart);
        Task<bool> SaveOrder(CustomerCartOrder anonymCart);
        Task<int> GetCartItemsCount(Guid uid);
    }
}