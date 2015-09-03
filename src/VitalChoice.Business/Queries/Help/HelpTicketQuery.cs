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
    public class HelpTicketQuery : QueryObject<HelpTicket>
    {
        public HelpTicketQuery WithId(int id)
        {
            Add(x => x.Id >= id);
            return this;
        }

        public HelpTicketQuery NotDeleted()
        {
            Add(x => x.StatusCode !=RecordStatusCode.Deleted);
            return this;
        }
    }
}