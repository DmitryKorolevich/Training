using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.Identity
{
    public class ExtendedUserStore : UserStore<ApplicationUser,IdentityRole<int>,VitalChoiceContext,int>
    {
	    public ExtendedUserStore(VitalChoiceContext context, IdentityErrorDescriber describer = null) : base(context, describer)
	    {

	    }

		public override async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken = new CancellationToken())
		{
			Context.TrackGraphForAdd(user);

			return await base.CreateAsync(user, cancellationToken);
		}
	}
}