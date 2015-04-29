using System;
using System.Collections.Generic;
using System.Linq;
#if DNX451
using System.Net.Mail;
#endif
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Identity
{
    public class ExtendedUserValidator :UserValidator<ApplicationUser>
    {
	    private readonly IRepositoryAsync<AdminProfile> profileRepository;

	    public ExtendedUserValidator(IRepositoryAsync<AdminProfile> profileRepository)
	    {
		    this.profileRepository = profileRepository;
	    }

	    protected async Task ValidateEmail(UserManager<ApplicationUser> manager, ApplicationUser user, List<IdentityError> errors)
		{
			var email = await manager.GetEmailAsync(user);
			if (string.IsNullOrWhiteSpace(email))
			{
				errors.Add(this.Describer.InvalidEmail(email));
			}
			else
			{
				try
				{
#if DNX451
					new MailAddress(email);
#endif
				}
				catch (FormatException)
				{
					errors.Add(this.Describer.InvalidEmail(email));
					return;
				}
				var byEmailAsync = await manager.FindByEmailAsync(email);
				var flag = byEmailAsync != null;
				if (flag)
				{
					var a = await manager.GetUserIdAsync(byEmailAsync);
					var userIdAsync = await manager.GetUserIdAsync(user);
					flag = !string.Equals(a, userIdAsync);
				}
				if (flag)
					errors.Add(this.Describer.DuplicateEmail(email));
			}
		}

		protected async Task ValidateUserName(UserManager<ApplicationUser> manager, ApplicationUser user, ICollection<IdentityError> errors)
		{
			var userName = await manager.GetUserNameAsync(user);
			if (string.IsNullOrWhiteSpace(userName))
				errors.Add(this.Describer.InvalidUserName(userName));
			//else if (manager.Options.User.UserNameValidationRegex != null && !Regex.IsMatch(userName, manager.Options.User.UserNameValidationRegex))
			//{
			//	errors.Add(this.Describer.InvalidUserName(userName));
			//}
			else
			{
				var byNameAsync = await manager.FindByNameAsync(userName);
				var flag = byNameAsync != null;
				if (flag)
				{
					var a = await manager.GetUserIdAsync(byNameAsync);
					var userIdAsync = await manager.GetUserIdAsync(user);
					flag = !string.Equals(a, userIdAsync);
				}
				if (flag)
					errors.Add(this.Describer.DuplicateUserName(userName));
			}
		}

		protected async Task ValidateAgentId(UserManager<ApplicationUser> manager, ApplicationUser user, ICollection<IdentityError> errors)
		{
			var userName = await manager.GetUserNameAsync(user);
			if (string.IsNullOrWhiteSpace(userName))
				errors.Add(this.Describer.InvalidUserName(userName));
			//else if (manager.Options.User.UserNameValidationRegex != null && !Regex.IsMatch(userName, manager.Options.User.UserNameValidationRegex))
			//{
			//	errors.Add(this.Describer.InvalidUserName(userName));
			//}
			else
			{
				var findByAgentId = await profileRepository.Query(x => x.AgentId == user.Profile.AgentId).Include(x => x.User).SelectAsync(false); //todo: refactor this
				findByAgentId = findByAgentId.Where(x => !x.User.DeletedDate.HasValue).ToList();
                var flag = !findByAgentId.Any();
				if (flag)
				{
					flag = findByAgentId.Any(x => x.User.Id != user.Id);
				}
				if (flag)
					errors.Add(new IdentityError() { Description = "Provided Agent Id is already taken" });
			}
		}

		public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
	    {
			if (manager == null)
				throw new ArgumentNullException(nameof(manager));
			if (user == null)
				throw new ArgumentNullException(nameof(user));
			var errors = new List<IdentityError>();

			await this.ValidateUserName(manager, user, errors);
			//if (manager.Options.User.RequireUniqueEmail)
				await this.ValidateEmail(manager, user, errors);

			await ValidateAgentId(manager, user, errors);

			return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
		}
    }
}