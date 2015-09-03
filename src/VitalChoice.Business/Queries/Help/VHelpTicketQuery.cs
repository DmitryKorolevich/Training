using System;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Logs;

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
                Add(x => x.StatusCode <= status.Value);
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