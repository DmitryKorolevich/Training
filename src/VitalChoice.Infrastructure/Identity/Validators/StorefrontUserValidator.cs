using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Constants;
using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain;
#if !NETSTANDARD1_5
using System.Net.Mail;
#endif

namespace VitalChoice.Infrastructure.Identity.Validators
{
    public class StorefrontUserValidator : UserValidator<ApplicationUser>
    {
        private IdentityOptions _options;

        public StorefrontUserValidator(IOptions<IdentityOptions> optionsAccessor, IdentityErrorDescriber errors = null)
            : base(errors ?? new IdentityErrorDescriber())
        {
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
            if (user.UserName != BaseAppConstants.FAKE_CUSTOMER_EMAIL)
            {
                await ValidateUserName(manager, user, errors);
                if (_options.User.RequireUniqueEmail)
                {
                    await ValidateEmail(manager, user, errors);
                }
            }
            return errors.Count > 0 ? IdentityResult.Failed(errors.ToArray()) : IdentityResult.Success;
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
        }
    }
}
