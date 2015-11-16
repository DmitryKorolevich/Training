using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;

#if DNX451

#endif

namespace VitalChoice.Infrastructure.Identity.Validators
{
    public class AdminUserValidator : UserValidator<ApplicationUser>
    { 
	    private readonly IRepositoryAsync<AdminProfile> profileRepository;

	    public AdminUserValidator(IRepositoryAsync<AdminProfile> profileRepository, IdentityErrorDescriber describer):base(describer)
	    {
		    this.profileRepository = profileRepository;
	    }

		protected virtual async Task ValidateAgentId(UserManager<ApplicationUser> manager, ApplicationUser user, ICollection<IdentityError> errors)
		{
			var userName = await manager.GetUserNameAsync(user);
			if (string.IsNullOrWhiteSpace(userName))
				errors.Add(Describer.InvalidUserName(userName));
			else
			{
				var findByAgentId = await profileRepository.Query(x => x.AgentId == user.Profile.AgentId).Include(x => x.User).SelectAsync(false); //todo: refactor this
				findByAgentId = findByAgentId.Where(x => !x.User.DeletedDate.HasValue).ToList();
				var flag = findByAgentId.Any(x => x.User.Id != user.Id);
				if (flag)
					errors.Add(new IdentityError() { Description = ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AgentIdIsTakenAlready] });
			}
		}

		public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
		{
			var result = await base.ValidateAsync(manager, user);

			var errors = new List<IdentityError>();
			if (!result.Succeeded)
			{
				errors.AddRange(result.Errors);
			}

			await ValidateAgentId(manager, user, errors);

			return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
		}
	}
}