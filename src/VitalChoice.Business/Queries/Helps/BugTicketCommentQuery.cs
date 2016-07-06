using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Help;

namespace VitalChoice.Business.Queries.Helps
{
    public class BugTicketCommentQuery : QueryObject<BugTicketComment>
    {
        public BugTicketCommentQuery WithId(int id)
        {
            Add(x => x.Id == id);
            return this;
        }

        public BugTicketCommentQuery WithTicketId(int id)
        {
            Add(x => x.IdBugTicket == id);
            return this;
        }

        public BugTicketCommentQuery NotDeleted()
        {
            Add(x => x.StatusCode !=RecordStatusCode.Deleted);
            return this;
        }
    }
}