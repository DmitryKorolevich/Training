using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Business.Queries.Users
{
    public class AdminProfileQuery: QueryObject<AdminProfile>
    {
	    public AdminProfileQuery IdInRange(ICollection<int> ids)
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
