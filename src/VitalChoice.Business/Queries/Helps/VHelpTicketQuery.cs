using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Transfer.Help;

namespace VitalChoice.Business.Queries.Help
{
    public class VHelpTicketQuery : QueryObject<VHelpTicket>
    {
        public VHelpTicketQuery WithDateCreatedFrom(DateTime? from)
        {
            if (from.HasValue)
            {
                Add(x => x.DateCreated >= from.Value);
            }
            return this;
        }

        public VHelpTicketQuery WithDateCreatedTo(DateTime? to)
        {
            if (to.HasValue)
            {
                Add(x => x.DateCreated <= to.Value);
            }
            return this;
        }

        public VHelpTicketQuery WithStatus(RecordStatusCode? status)
        {
            if (status.HasValue)
            {
                Add(x => x.StatusCode == status.Value);
            }
            return this;
        }

        public VHelpTicketQuery WithPriority(TicketPriority? priority)
        {
            if (priority.HasValue)
            {
                Add(x => x.Priority == priority.Value);
            }
            return this;
        }

        public VHelpTicketQuery WithId(int id)
        {
            Add(x => x.Id == id);
            return this;
        }
    }
}