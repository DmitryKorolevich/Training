using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;

namespace VitalChoice.Interfaces.Services.Checkout
{
    public interface ICheckoutService
    {
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid);
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, int idCustomer);
        Task<bool> UpdateCart(CustomerCartOrder anonymCart);
        Task<OrderDataContext> CalculateCart(CustomerCartOrder cart);
        Task<bool> SaveOrder(CustomerCartOrder cart);
        Task<int> GetCartItemsCount(Guid uid);
    }
}