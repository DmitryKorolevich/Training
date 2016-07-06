using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VitalChoice.Business.Queries.Helps
{
    public class HelpTicketCommentQuery : QueryObject<HelpTicketComment>
    {
        public HelpTicketCommentQuery WithId(int id)
        {
            Add(x => x.Id == id);
            return this;
        }

        public HelpTicketCommentQuery WithTicketId(int id)
        {
            Add(x => x.IdHelpTicket == id);
            return this;
        }

        public HelpTicketCommentQuery NotDeleted()
        {
            Add(x => x.StatusCode !=RecordStatusCode.Deleted);
            return this;
        }
    }
}