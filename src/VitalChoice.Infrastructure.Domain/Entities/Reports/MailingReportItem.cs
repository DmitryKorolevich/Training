using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class MailingReportItem : Entity
    { 
        public string Email { get; set; }

        public CustomerType CustomerIdObjectType { get; set; }

        public DateTime? FirstOrderDateCreated { get; set; }

        public string FirstKeyCode { get; set; }

        public string FirstDiscountCode { get; set; }

        public DateTime? LastOrderDateCreated { get; set; }

        public decimal? LastOrderTotal { get; set; }

        public int? LastOrderIdPaymentMethod { get; set; }

        public int OrdersCount { get; set; }

        public decimal OrdersTotal { get; set; }

        public bool DoNotMail { get; set; }

        public bool DoNotRent { get; set; }

        public int? IdCustomerOrderSource { get; set; }

        public string CustomerOrderSource { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public string Phone { get; set; }

        public string Zip { get; set; }

        public int? IdCountry { get; set; }

        public string CountryCode { get; set; }

        public int? IdState { get; set; }

        public string StateCode { get; set; }

        public int Count { get; set; }
    }
}
