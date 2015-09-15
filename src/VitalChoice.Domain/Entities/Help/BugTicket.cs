using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Entities.Help
{
    public class BugTicket : Entity
    {
        public int IdAddedBy { get; set; }

        public string AddedBy { get; set; }

        public string AddedByAgent { get; set; }

        public string AddedByEmail { get; set; }

        public int IdEditedBy { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public TicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public Guid PublicId { get; set; }

        public ICollection<BugTicketComment> Comments { get; set; }

        public ICollection<BugFile> Files { get; set; }
    }
}