using VitalChoice.Data.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Business.Queries.User
{
    public class UserQuery: QueryObject<ApplicationUser> {
        public UserQuery GetUser(string userName)
        {
            Add(x => x.UserName.Equals(userName));
            return this;
        }
    }
}