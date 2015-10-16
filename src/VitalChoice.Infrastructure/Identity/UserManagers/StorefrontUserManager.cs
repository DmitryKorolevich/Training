using System;
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
    public class StorefrontUserManager : UserManager<ApplicationUser>
    {
        public StorefrontUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services,
            ILoggerProviderExtended logger, IHttpContextAccessor contextAccessor)
            : base(
                store, optionsAccessor, passwordHasher, userValidators.Where(x => x is DummyUserValidator),
                passwordValidators, keyNormalizer, errors, services, new Logger<UserManager<ApplicationUser>>(logger.Factory), contextAccessor)
        {
        }
    }
}