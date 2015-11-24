using System;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class AffiliateOrderListItemModel
    {
        public DateTime DateCreated { get; set; }

        public int IdOrder { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public AffiliateOrderPaymentStatus PaymentStatus { get; set; }

        public int RepeatInCustomer { get; set; }

        public int IdCustomer { get; set; }

        public string CustomerName { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal Total { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal Commission { get; set; }

        public bool NewCustomerOrder { get; set; }

        public AffiliateOrderListItemModel(AffiliateOrderPayment affiliatePayment,string customerFirstName, string customerLastName, int repeatInCustomer)
        {
            if(affiliatePayment!=null)
            {
                DateCreated = affiliatePayment.Order.DateCreated;
                IdOrder = affiliatePayment.Id;
                OrderStatus = affiliatePayment.Order.OrderStatus;
                PaymentStatus = affiliatePayment.Status;
                IdCustomer = affiliatePayment.Order.IdCustomer;
                NewCustomerOrder = affiliatePayment.NewCustomerOrder;
                ProductsSubtotal = affiliatePayment.Order.ProductsSubtotal;
                Total = affiliatePayment.Order.Total;
                ShippingTotal = affiliatePayment.Order.ShippingTotal;
                TaxTotal = affiliatePayment.Order.TaxTotal;
                Commission = affiliatePayment.Amount;
                if (!String.IsNullOrEmpty(customerFirstName) || !String.IsNullOrEmpty(customerLastName))
                {
                    CustomerName = customerFirstName + " " + customerLastName;
                }
                RepeatInCustomer = repeatInCustomer;
            }
        }
    }
}