using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.eCommerce.Help
{
    public class VHelpTicket : Entity
    {
        public int IdOrder { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public HelpTicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public int IdCustomer { get; set; }

        public string Customer { get; set; }

        public string CustomerEmail { get; set; }
    }
}