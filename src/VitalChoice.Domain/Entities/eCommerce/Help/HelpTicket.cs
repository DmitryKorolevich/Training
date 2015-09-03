using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Entities.eCommerce.Help
{
    public class HelpTicket : Entity
    {
        public int IdOrder { get; set; }

        public Order Order { get; set; }

        public int IdCustomer { get; set; }

        public string Customer { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public HelpTicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public ICollection<HelpTicketComment> Comments { get; set; }
    }
}