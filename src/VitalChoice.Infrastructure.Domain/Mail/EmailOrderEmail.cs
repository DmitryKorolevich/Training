using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Infrastructure.Domain.Mail
{
    public class EmailOrderSku
    {
        public string Code { get; set; }

        public int QTY { get; set; }

        public decimal Price { get; set; }
    }

    public class EmailOrderEmail : EmailTemplateDataModel
    {
        public DateTime DateCreated { get; set; }

        public string DateCreatedDatePart { get; set; }

        public string DateCreatedTimePart { get; set; }

        public string DetailsOnEvent { get; set; }

        public string Instuction { get; set; }

        public string Requestor { get; set; }

        public string Reason { get; set; }

        public string EmailOrderShippingType { get; set; }

        public AddressBaseModel Shipping { get; set; }

        public ICollection<EmailOrderSku> Skus { get; set; }
    }
}