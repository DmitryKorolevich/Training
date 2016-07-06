using VitalChoice.Data.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Business.Queries.Users
{
    public class UserQuery: QueryObject<ApplicationUser> {
        public UserQuery GetUser(string userName)
        {
            Add(x => x.UserName == userName);
            return this;
        }
    }
}