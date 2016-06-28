using System.Linq;
using Microsoft.AspNetCore.Identity;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace VitalChoice.Infrastructure.Identity.UserStores
{
    public class AffiliateUserStore : ExtendedUserStore
	{
	    public AffiliateUserStore(VitalChoiceContext context, IdentityErrorDescriber describer = null) : base(context, describer)
	    {

	    }

        public override IQueryable<ApplicationUser> Users
            => Context.Users.Include(x => x.Roles).Where(x => x.IdUserType == UserType.Affiliate && !x.DeletedDate.HasValue);
	}
}