using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class CustomerOrderAbuseReportRawItem : Entity
    { 
        public OrderType IdObjectType { get; set; }

        public int IdCustomer { get; set; }

        public DateTime DateCreated { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public int? IdServiceCode { get; set; }
    }
}