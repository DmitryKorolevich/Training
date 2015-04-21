using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Users;

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