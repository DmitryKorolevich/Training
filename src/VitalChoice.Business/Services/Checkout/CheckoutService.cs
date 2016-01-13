using System;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Checkout;

namespace VitalChoice.Business.Services.Checkout
{
    public class CheckoutService : ICheckoutService
    {
        public CustomerCart GetOrCreateCart(Guid uid, int? idCustomer)
        {
            throw new NotImplementedException();
        }

        public bool UpdateCart(CustomerCart cart)
        {
            throw new NotImplementedException();
        }

        public OrderDataContext CalculateCart(CustomerCart cart)
        {
            throw new NotImplementedException();
        }

        public int SaveOrder(CustomerCart cart)
        {
            throw new NotImplementedException();
        }
    }
}
