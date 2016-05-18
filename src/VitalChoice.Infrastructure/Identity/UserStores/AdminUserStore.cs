﻿using System.Linq;
using Microsoft.AspNetCore.Identity;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace VitalChoice.Infrastructure.Identity.UserStores
{
    public class AdminUserStore : ExtendedUserStore
    {
	    public AdminUserStore(VitalChoiceContext context, IdentityErrorDescriber describer = null) : base(context, describer)
	    {

	    }

        public override IQueryable<ApplicationUser> Users
            =>
                Context.Users.Include(x => x.Profile)
                    .Include(x => x.Roles)
                    .Where(x => x.IdUserType == UserType.Admin && !x.DeletedDate.HasValue);
    }
}