using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuAddressReportItem
    {
        public int IdOrder { get; set; }

        public int IdObjectType { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public OrderStatus? POrderStatus { get; set; }

        public OrderStatus? NPOrderStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public string SkuCode { get; set; }

        public int Quantity { get; set; }

        public int IdCustomer { get; set; }
        
        public int IdCustomerObjectType { get; set; }

        public string DiscountCode { get; set; }

        public decimal Price { get; set; }

        public decimal Amount { get; set; }

        public string Source { get; set; }

        public bool DoNotMail { get; set; }

        public string ShippingCompany { get; set; }

        public string ShippingFirstName { get; set; }

        public string ShippingLastName { get; set; }

        public string ShippingAddress1 { get; set; }

        public string ShippingAddress2 { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingCountyCode { get; set; }

        public string ShippingCounty { get; set; }

        public string ShippingStateCode { get; set; }

        public string ShippingZip { get; set; }

        public string ShippingPhone { get; set; }

        public string BillingCompany { get; set; }

        public string BillingFirstName { get; set; }

        public string BillingLastName { get; set; }

        public string BillingAddress1 { get; set; }

        public string BillingAddress2 { get; set; }

        public string BillingCity { get; set; }

        public string BillingCountyCode { get; set; }

        public string BillingCounty { get; set; }

        public string BillingStateCode { get; set; }

        public string BillingZip { get; set; }

        public string BillingPhone { get; set; }
    }
}