using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class MatchbackReportItem
    {
        public int IdOrder { get; set; }

        public int IdObjectType { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public OrderStatus? POrderStatus { get; set; }

        public OrderStatus? NPOrderStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public int IdCustomer { get; set; }
        
        public string OrderSource { get; set; }
        
        public string DiscountCode { get; set; }

        public string KeyCode { get; set; }

        public string Source { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal ShippingTotal { get; set; }

        public decimal Total { get; set; }
        
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