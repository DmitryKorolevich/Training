using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using AuthorizationContext = Microsoft.AspNet.Mvc.Filters.AuthorizationContext;
using Microsoft.AspNet.Mvc.Filters;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using System.Linq;
using System;

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

			var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

			var claimUser = context.HttpContext.User;
			var result = await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
			if (result)
			{
				var superAdmin =
				context.HttpContext.RequestServices.GetService<IAppInfrastructureService>()
					.Data()
					.AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser)
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