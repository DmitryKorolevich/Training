using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;

namespace VitalChoice.Interfaces.Services.Checkout
{
    public interface ICheckoutService
    {
        Task<CustomerCart> GetOrCreateAnonymCart(Guid? uid);
        Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, int idCustomer);
        Task<bool> UpdateCart(CustomerCart anonymCart);
        Task<bool> UpdateCart(CustomerCartOrder cartOrder);
        Task<OrderDataContext> CalculateCart(CustomerCart cart);
        Task<OrderDataContext> CalculateCart(CustomerCartOrder cart);
        Task<bool> SaveOrder(CustomerCartOrder cart);
    }
}