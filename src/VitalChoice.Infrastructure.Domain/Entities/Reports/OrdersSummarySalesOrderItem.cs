using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class OrdersSummarySalesOrderItem : Entity
    {
        public int IdObjectType { get; set; }

        public int IdCustomer { get; set; }

        public int CustomerIdObjectType { get; set; }

        public int? IdDiscount { get; set; }

        public DateTime CustomerDateCreated { get; set; }

        public int? OrderStatus { get; set; }

        public int? POrderStatus { get; set; }

        public int? NPOrderStatus { get; set; }

        public DateTime DateCreated { get; set; }

        public decimal ProductsSubtotal { get; set; }

        public decimal Total { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string CustomerCompany { get; set; }

        public string DiscountCode { get; set; }

        public int? IdAffiliate { get; set; }

        public string KeyCode { get; set; }

        public int? Source { get; set; }

        public string SourceName { get; set; }

        public string SourceDetails { get; set; }

        public long OrdersCount { get; set; }

        public DateTime FirstOrderDate { get; set; }

        public int TotalCount { get; set; }
    }
}
