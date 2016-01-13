using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Entities.Cart
{
    public class IncompleteOrderCart : Cart
    {
        public IncompleteOrderCart(Cart source)
        {
            Id = source.Id;
            CartUid = source.CartUid;
            IdCustomer = source.IdCustomer;
            IdOrder = source.IdOrder;
            Order = source.Order;
        }

        public sealed override Discount Discount
        {
            get { return Order.Discount; }
            set { Order.Discount = value; }
        }

        public sealed override int? IdDiscount
        {
            get { return Order.IdDiscount; }
            set { Order.IdDiscount = value; }
        }

        public sealed override ICollection<OrderToGiftCertificate> GiftCertificates
        {
            get { return Order.GiftCertificates; }
            set { Order.GiftCertificates = value; }
        }

        public sealed override ICollection<OrderToSku> Skus
        {
            get { return Order.Skus; }
            set { Order.Skus = value; }
        }
    }
}