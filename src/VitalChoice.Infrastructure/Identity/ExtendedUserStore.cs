using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.Identity
{
    public class ExtendedUserStore : UserStore<ApplicationUser, IdentityRole<int>, VitalChoiceContext,int>
    {

	    public ExtendedUserStore(VitalChoiceContext context, IdentityErrorDescriber describer = null) : base(context, describer)
	    {

	    }

		public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = new CancellationToken())
		{
			//Context.TrackGraphForAdd(user);
			return await base.CreateAsync(user, cancellationToken);
		}

	    public override async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
	    {
		    return await Context.Users.AsNoTracking().Include(x=>x.Profile).Include(x=>x.Roles).FirstOrDefaultAsync(x=>x.NormalizedEmail.Equals(normalizedEmail) && !x.DeletedDate.HasValue,cancellationToken);
	    }

        public override async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
        {
            return await Context.Users.AsNoTracking().Include(x => x.Profile).Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(userId) && !x.DeletedDate.HasValue, cancellationToken);
        }

        public override async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new CancellationToken())
	    {
			return await Context.Users.AsNoTracking().Include(x => x.Profile).Include(x => x.Roles).FirstOrDefaultAsync(x => x.NormalizedUserName.Equals(normalizedUserName) && !x.DeletedDate.HasValue, cancellationToken);
		}
    }
}