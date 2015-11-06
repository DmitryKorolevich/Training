using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Helpers.Export;

namespace VitalChoice.Domain.Transfer.Affiliates
{
    public class AffiliateOrderListItemModel : IExportable
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
                IdOrder = affiliatePayment.IdOrder;
                OrderStatus = affiliatePayment.Order.OrderStatus;
                PaymentStatus = affiliatePayment.Status;
                IdCustomer = affiliatePayment.Order.IdCustomer;
                NewCustomerOrder = affiliatePayment.NewCustomerOrder;
                ProductsSubtotal = affiliatePayment.Order.ProductsSubtotal;
                Total = affiliatePayment.Order.Total;
                ShippingTotal = affiliatePayment.Order.ShippingTotal;
                TaxTotal = affiliatePayment.Order.TaxTotal;
                Commission = affiliatePayment.Amount;
                CustomerName = customerFirstName+" "+ customerLastName;
                RepeatInCustomer = repeatInCustomer;
            }
        }
    }
}