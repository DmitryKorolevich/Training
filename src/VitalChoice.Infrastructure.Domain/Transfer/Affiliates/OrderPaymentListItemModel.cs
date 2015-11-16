using System;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;

namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class OrderPaymentListItemModel
    {
        public DateTime DateCreated { get; set; }

        public int IdOrder { get; set; }

        public decimal ProductTotal { get; set; }

        public decimal OrderTotal { get; set; }

        public decimal Shipping { get; set; }

        public decimal Tax { get; set; }

        public decimal Commission { get; set; }

        public bool NewCustomerOrder { get; set; }

	    public OrderPaymentListItemModel(AffiliateOrderPayment orderPayment)
        {
            if (orderPayment != null)
            {
                DateCreated = orderPayment.Order.DateCreated;
                IdOrder = orderPayment.Id;
                NewCustomerOrder = orderPayment.NewCustomerOrder;
                ProductTotal = orderPayment.Order.ProductsSubtotal;
                OrderTotal = orderPayment.Order.Total;
                Shipping = orderPayment.Order.ShippingTotal;
                Tax = orderPayment.Order.TaxTotal;
                Commission = orderPayment.Amount;
            }
        }
    }
}
