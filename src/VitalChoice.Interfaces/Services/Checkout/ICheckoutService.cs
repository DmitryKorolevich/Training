using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;

namespace VitalChoice.Interfaces.Services.Checkout
{
    public interface ICheckoutService
    {
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid);
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, int idCustomer);
        Task<bool> UpdateCart(CustomerCartOrder anonymCart, int? customerAddressToUpdate = null);
        Task<bool> SaveOrder(CustomerCartOrder anonymCart);
        Task<int> GetCartItemsCount(Guid uid);
    }
}