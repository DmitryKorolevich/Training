using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Identity.UserStores
{
    public class ExtendedUserStore : UserStore<ApplicationUser, ApplicationRole, VitalChoiceContext, int>
    {
	    public ExtendedUserStore(VitalChoiceContext context, IdentityErrorDescriber describer = null) : base(context, describer)
	    {

	    }

	    public override IQueryable<ApplicationUser> Users => Context.Users.Include(x => x.Roles).Where(x=> !x.DeletedDate.HasValue);

	    public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = new CancellationToken())
		{
			Context.TrackGraphForAdd(user);

			return await base.CreateAsync(user, cancellationToken);
		}

	 //   public override async Task<ApplicationUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = new CancellationToken())
	 //   {
		//    return await Users.Include(x=>x.Roles).FirstOrDefaultAsync(x=>x.NormalizedEmail.Equals(normalizedEmail) && !x.DeletedDate.HasValue,cancellationToken);
	 //   }

  //      public override async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken = new CancellationToken())
  //      {
  //          return await Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(userId) && !x.DeletedDate.HasValue, cancellationToken);
  //      }

  //      public override async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken = new CancellationToken())
	 //   {
		//	return await Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.NormalizedUserName.Equals(normalizedUserName) && !x.DeletedDate.HasValue, cancellationToken);
		//}
    }
}