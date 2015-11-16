using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Entities.Help
{
    public class HelpTicket : Entity
    {
        public int IdOrder { get; set; }

        public Order Order { get; set; }

        public int IdCustomer { get; set; }

        public string Customer { get; set; }

        public string CustomerEmail { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public TicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public ICollection<HelpTicketComment> Comments { get; set; }
    }
}