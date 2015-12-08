using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Identity.Validators;

namespace VitalChoice.Infrastructure.Identity.UserManagers
{
    //needed to remove default user validator
    public class ExtendedUserManager : UserManager<ApplicationUser>
    {
        public ExtendedUserManager(IUserStore<ApplicationUser> store, IOptions<IdentityOptions> optionsAccessor,
            IPasswordHasher<ApplicationUser> passwordHasher, IEnumerable<IUserValidator<ApplicationUser>> userValidators,
            IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, ILookupNormalizer keyNormalizer,
            IdentityErrorDescriber errors, IServiceProvider services,
            ILoggerFactory loggerFactory, IHttpContextAccessor contextAccessor)
            : base(
                store, optionsAccessor, passwordHasher, userValidators.Where(x => x is DummyUserValidator),
                passwordValidators, keyNormalizer, errors, services, new Logger<UserManager<ApplicationUser>>(loggerFactory),
                contextAccessor)
        {
        }
    }
}