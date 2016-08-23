using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using VitalChoice.Ecommerce.Domain.Mail;

namespace VitalChoice.Infrastructure.Domain.Mail
{
    public class GLOrdersImportEmail : EmailTemplateDataModel
    {
        public DateTime Date { get; set; }

        public string Agent { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public int IdCustomer { get; set; }

        public int ImportedOrdersCount { get; set; }

        public decimal ImportedOrdersAmount { get; set; }

        public string CardNumber { get; set; }

        public ICollection<int> OrderIds { get; set; }
    }
}