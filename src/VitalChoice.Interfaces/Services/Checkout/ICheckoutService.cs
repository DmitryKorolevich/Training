using System;
using VitalChoice.Ecommerce.Domain.Entities.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;

namespace VitalChoice.Interfaces.Services.Checkout
{
    public interface ICheckoutService
    {
        CustomerCart GetOrCreateCart(Guid uid, int? idCustomer);
        bool UpdateCart(CustomerCart cart);
        OrderDataContext CalculateCart(CustomerCart cart);
        int SaveOrder(CustomerCart cart);
    }
}