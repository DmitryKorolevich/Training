﻿using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Identity
{
    public class DummyUserValidator : UserValidator<ApplicationUser>
	{
	    public override async Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager, ApplicationUser user)
		{
			return await Task.FromResult(IdentityResult.Success);
		}
	}
}