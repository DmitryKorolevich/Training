using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Entities.Help
{
    public class BugTicketComment : Entity
    {
        public int IdBugTicket { get; set; }

        public BugTicket BugTicket { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int IdEditedBy { get; set; }

        public string EditedBy { get; set; }

        public string EditedByAgent { get; set; }

        public int Order { get; set; }

        public string Comment { get; set; }

        public Guid PublicId { get; set; }

        public ICollection<BugFile> Files { get; set; }
    }
}