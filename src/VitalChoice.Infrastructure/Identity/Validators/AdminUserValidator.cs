using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain;

namespace VitalChoice.Infrastructure.Identity.Validators
{
    public class AdminUserValidator : UserValidator<ApplicationUser>
    {
        private IdentityOptions _options;
        private readonly IRepositoryAsync<AdminProfile> profileRepository;

	    public AdminUserValidator(IRepositoryAsync<AdminProfile> profileRepository,
            IOptions<IdentityOptions> optionsAccessor,
            IdentityErrorDescriber describer):base(describer)
	    {
		    this.profileRepository = profileRepository;
            _options = optionsAccessor?.Value ?? new IdentityOptions();
        }

		public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
		{
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            var errors = new List<IdentityError>();
            await ValidateUserName(manager, user, errors);
            if (_options.User.RequireUniqueEmail)
            {
                await ValidateEmail(manager, user, errors);
            }

			await ValidateAgentId(manager, user, errors);

			return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
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

        private async Task ValidateUserName(UserManager<ApplicationUser> manager, ApplicationUser user, ICollection<IdentityError> errors)
        {
            var userName = await manager.GetUserNameAsync(user);
            if (string.IsNullOrWhiteSpace(userName))
            {
                errors.Add(Describer.InvalidUserName(userName));
            }
            else if (!string.IsNullOrEmpty(_options.User.AllowedUserNameCharacters) &&
                userName.Any(c => !_options.User.AllowedUserNameCharacters.Contains(c)))
            {
                errors.Add(Describer.InvalidUserName(userName));
            }
            else
            {
                var owner = await manager.FindByNameAsync(userName);
                if (owner != null &&
                    !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
                {
                    errors.Add(Describer.DuplicateUserName(userName));
                }
            }
        }

        // make sure email is not empty, valid, and unique
        private async Task ValidateEmail(UserManager<ApplicationUser> manager, ApplicationUser user, List<IdentityError> errors)
        {
            var email = await manager.GetEmailAsync(user);
            if (string.IsNullOrWhiteSpace(email))
            {
                errors.Add(Describer.InvalidEmail(email));
                return;
            }

            if (!email.IsValidEmail())
            {
                errors.Add(Describer.InvalidEmail(email));
                return;
            }

            //var owner = await manager.FindByEmailAsync(email);
            //if (owner != null &&
            //    !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
            //{
            //    errors.Add(Describer.DuplicateEmail(email));
            //}
        }
    }
}