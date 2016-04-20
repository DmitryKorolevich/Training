using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VitalChoice.Infrastructure.Domain.Transfer.Help
{
    public class BugTicketFilter : FilterBase
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public BugTicketStatus? StatusCode { get; set; }

        public TicketPriority? Priority { get; set; }

        public int? IdOrder { get; set; }

        public int? IdCustomer { get; set; }
    }
}