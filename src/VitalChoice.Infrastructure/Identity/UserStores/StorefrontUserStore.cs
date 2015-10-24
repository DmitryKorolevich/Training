using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Infrastructure.Identity.UserStores
{
    public class StorefrontUserStore : ExtendedUserStore
	{
	    public StorefrontUserStore(VitalChoiceContext context, IdentityErrorDescriber describer = null) : base(context, describer)
	    {

	    }

		public override IQueryable<ApplicationUser> Users => Context.Users.Include(x => x.Roles).Where(x => x.IdUserType == UserType.Customer && !x.DeletedDate.HasValue);
	}
}