using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Infrastructure.Identity.Validators;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Infrastructure.Identity.UserManagers
{
	//needed to remove default user validator
    public class ExtendedUserManager : UserManager<ApplicationUser>
    {
        public ExtendedUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IEnumerable<IUserTokenProvider<ApplicationUser>> tokenProviders,
            ILoggerProviderExtended logger, IHttpContextAccessor contextAccessor)
            : base(
                store, optionsAccessor, passwordHasher, userValidators.Where(x => x is DummyUserValidator),
                passwordValidators, keyNormalizer, errors, tokenProviders, new Logger<UserManager<ApplicationUser>>(logger.Factory), contextAccessor)
        {
        }
    }
}