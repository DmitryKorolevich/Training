using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VitalChoice.Infrastructure.Domain.Transfer.Help
{
    public class VHelpTicket : Entity
    {
        public int IdOrder { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public TicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public int IdCustomer { get; set; }

        public string Customer { get; set; }

        public string CustomerEmail { get; set; }
    }
}