using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VitalChoice.Business.Queries.Helps
{
    public class HelpTicketQuery : QueryObject<HelpTicket>
    {
        public HelpTicketQuery WithId(int id)
        {
            Add(x => x.Id == id);
            return this;
        }

        public HelpTicketQuery NotDeleted()
        {
            Add(x => x.StatusCode !=RecordStatusCode.Deleted);
            return this;
        }
    }
}