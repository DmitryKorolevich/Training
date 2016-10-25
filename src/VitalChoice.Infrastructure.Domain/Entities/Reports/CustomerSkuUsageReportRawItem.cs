using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class CustomerSkuUsageReportRawItem : Entity
    { 
        public int IdSku { get; set; }

        public int IdCustomer { get; set; }

        public int CustomerIdObjectType { get; set; }

        public CustomerType CustomerType { get; set; }

        public int Quantity { get; set; }

        public int LastIdOrder { get; set; }

        public DateTime LastOrderDateCreated { get; set; }

        public string Email { get; set; }

        public string Code { get; set; }

        public string CategoryIds { get; set; }

        public string CategoryNames { get; set; }

        public bool? DoNotMail { get; set; }

        public string ShippingFirstName { get; set; }

        public string ShippingLastName { get; set; }

        public string ShippingAddress1 { get; set; }

        public string ShippingAddress2 { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingZip { get; set; }

        public int? ShippingIdCountry { get; set; }

        public string ShippingCountryCode { get; set; }

        public int? ShippingIdState { get; set; }

        public string ShippingStateCode { get; set; }

        public string ShippingCounty { get; set; }
        
        public int TotalCount { get; set; }
    }
}