using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VitalChoice.Infrastructure.Domain.Transfer.Help
{
    public class VHelpTicketFilter : FilterBase
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public RecordStatusCode? StatusCode { get; set; }

        public TicketPriority? Priority { get; set; }
    }
}