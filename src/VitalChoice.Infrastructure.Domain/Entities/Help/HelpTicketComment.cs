using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Entities.Help
{
    public class HelpTicketComment : Entity
    {
        public int IdHelpTicket { get; set; }

        public HelpTicket HelpTicket { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int? IdEditedBy { get; set; }

        public string EditedBy { get; set; }

        public int Order { get; set; }

        public string Comment { get; set; }
    }
}