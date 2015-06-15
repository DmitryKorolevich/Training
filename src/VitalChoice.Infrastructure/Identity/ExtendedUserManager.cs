using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Identity
{
	//needed to remove default user validator
    public class ExtendedUserManager : UserManager<ApplicationUser>
    {
        public ExtendedUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IEnumerable<IUserTokenProvider<ApplicationUser>> tokenProviders,
            ILogger<UserManager<ApplicationUser>> logger, IHttpContextAccessor contextAccessor)
            : base(
                store, optionsAccessor, passwordHasher, userValidators.Where(x => x is ExtendedUserValidator),
                passwordValidators, keyNormalizer, errors, tokenProviders, logger, contextAccessor)
        {
        }
    }
}