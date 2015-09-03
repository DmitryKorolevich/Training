using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Entities.eCommerce.Help
{
    public class HelpTicketComment : Entity
    {
        public int IdHelpTicket { get; set; }

        public HelpTicket HelpTicket { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int IdEditedBy { get; set; }

        public string EditedBy { get; set; }

        public int Order { get; set; }

        public string Comment { get; set; }
    }
}