using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Business.Queries.User
{
    public class AdminProfileQuery: QueryObject<AdminProfile>
    {
	    public AdminProfileQuery IdInRange(IList<int> ids)
	    {
			Add(x=>ids.Contains(x.Id));

		    return this;
	    }

        public AdminProfileQuery WithId(int id)
        {
            Add(x => id == x.Id);

            return this;
        }
    }
}
