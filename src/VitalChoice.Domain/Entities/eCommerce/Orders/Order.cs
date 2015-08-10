using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.eCommerce.Orders
{
    public class Order : DynamicDataEntity<OrderOptionValue, OrderOptionType>
    {
        public OrderStatus OrderStatus { get; set; }

        public int IdCustomer { get; set; }

        public Customer Customer { get; set; }

        public ICollection<OrderToSku> Skus { get; set; }

        public int? IdDiscount { get; set; }

        public Discount Discount { get; set; }

        public ICollection<OrderToGiftCertificate> GiftCertificates { get; set; }

        public decimal Total { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal TaxTotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal DiscountTotal { get; set; }
    }
}