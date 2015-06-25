using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using AuthorizationContext = Microsoft.AspNet.Mvc.AuthorizationContext;

namespace VitalChoice.Core.Infrastructure
{
	public class SuperAdminAuthorizeAttribute : AuthorizationFilterAttribute
	{
		public SuperAdminAuthorizeAttribute()
		{
		}

		protected override void Fail(AuthorizationContext context)
		{
			if (context.HttpContext.User.Identity.IsAuthenticated)
			{
				context.Result = new HttpForbiddenResult();
			}
			else
			{
				context.Result = new HttpUnauthorizedResult();
			}
		}

		public override async Task OnAuthorizationAsync(AuthorizationContext context)
		{
			if (HasAllowAnonymous(context))
			{
				return;
			}

			var authorizationService = context.HttpContext.ApplicationServices.GetService<IAuthorizationService>();

			var claimUser = context.HttpContext.User;
			var result = await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
			if (result)
			{
				var superAdmin =
				context.HttpContext.ApplicationServices.GetService<IAppInfrastructureService>()
					.Get()
					.Roles.Single(x => x.Key == (int)RoleType.SuperAdminUser)
					.Text;

				if (claimUser.IsInRole(superAdmin.Normalize()))
				{
					return;
				}
			}

			Fail(context);
		}
	}
}