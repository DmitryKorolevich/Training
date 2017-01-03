using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class OrderAbuseReportRawItem : Entity
    { 
        public OrderType IdObjectType { get; set; }

        public int IdCustomer { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal Total { get; set; }

        public int? IdServiceCode { get; set; }

        public string ServiceCodeName { get; set; }

        public int? IdOrderSource { get; set; }

        public DateTime? OrderSourceDateCreated { get; set; }

        public decimal? OrderSourceProductsSubtotal { get; set; }

        public int? IdReship { get; set; }

        public DateTime? ReshipDateCreated { get; set; }

        public int? IdRefund { get; set; }

        public DateTime? RefundDateCreated { get; set; }

        public decimal? RefundTotal { get; set; }

        public string ServiceCodeNotes { get; set; }

        public int? OrderSourceIdAddedBy { get; set; }

        public string OrderSourceAddedBy { get; set; }
    }
}