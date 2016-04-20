using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Entities.Help
{
    public class BugTicket : Entity
    {
        public int IdAddedBy { get; set; }

        public ApplicationUser AddedByUser { get; set; }

        public string AddedBy { get; set; }

        public string AddedByAgent { get; set; }

        public string AddedByEmail { get; set; }

        public int IdEditedBy { get; set; }

        public string EditedByAgent { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public BugTicketStatus StatusCode { get; set; }

        public TicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public Guid PublicId { get; set; }

        public ICollection<BugTicketComment> Comments { get; set; }

        public ICollection<BugFile> Files { get; set; }
    }
}