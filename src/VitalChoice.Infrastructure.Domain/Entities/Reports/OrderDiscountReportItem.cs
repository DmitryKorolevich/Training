using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class OrderDiscountReportItem : Entity
    { 
        public OrderType IdObjectType { get; set; }

        public int? OrderStatus { get; set; }

        public int? POrderStatus { get; set; }

        public int? NPOrderStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public int IdCustomer { get; set; }

        public CustomerType CustomerIdObjectType { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public decimal Total { get; set; }

        public decimal DiscountTotal { get; set; }

        public int? IdDiscount { get; set; }

        public int? OrderIdDiscountTier { get; set; }

        public int? TotalCount { get; set; }

        public string DiscountCode { get; set; }

        public string DiscountInfo { get; set; }

        public string DiscountMessage { get; set; }
    }
}
