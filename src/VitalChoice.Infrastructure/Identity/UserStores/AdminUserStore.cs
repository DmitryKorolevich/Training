﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Infrastructure.Identity.UserStores
{
    public class AdminUserStore : ExtendedUserStore
    {
	    public AdminUserStore(VitalChoiceContext context, IdentityErrorDescriber describer = null) : base(context, describer)
	    {

	    }

		public override IQueryable<ApplicationUser> Users => Context.Users.Include(x => x.Profile).Include(x => x.Roles).Where(x => x.IsAdminUser && !x.DeletedDate.HasValue);
	}
}