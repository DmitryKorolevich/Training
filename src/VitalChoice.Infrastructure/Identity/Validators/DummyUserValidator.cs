using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Identity.Validators
{
    public class DummyUserValidator : UserValidator<ApplicationUser>
	{
	    public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
		{
			return await Task.FromResult(IdentityResult.Success);
		}
	}
}
