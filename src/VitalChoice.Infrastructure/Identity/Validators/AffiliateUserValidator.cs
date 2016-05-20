using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Constants;
using System;
using System.Linq;
using Microsoft.Extensions.OptionsModel;
#if NET451
using System.Net.Mail;
#endif

namespace VitalChoice.Infrastructure.Identity.Validators
{
    public class AffiliateUserValidator : UserValidator<ApplicationUser>
    {
        private IdentityOptions _options;

        public AffiliateUserValidator(IOptions<IdentityOptions> optionsAccessor, IdentityErrorDescriber errors = null)
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
#if NET451
            try
            {
                var m = new MailAddress(email);
            }
            catch (FormatException)
            {
                errors.Add(Describer.InvalidEmail(email));
                return;
            }
#endif
            //var owner = await manager.FindByEmailAsync(email);
            //if (owner != null &&
            //    !string.Equals(await manager.GetUserIdAsync(owner), await manager.GetUserIdAsync(user)))
            //{
            //    errors.Add(Describer.DuplicateEmail(email));
            //}
        }
    }
}
