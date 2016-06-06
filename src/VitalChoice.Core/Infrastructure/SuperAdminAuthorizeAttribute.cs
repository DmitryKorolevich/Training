using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VitalChoice.Core.Infrastructure
{
	public class SuperAdminAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
		protected void Fail(AuthorizationFilterContext context)
		{
			if (context.HttpContext.User.Identity.IsAuthenticated)
			{
				context.Result = new ForbiddenResult();
			}
			else
			{
				context.Result = new UnauthorizedResult();
			}
		}

		public void OnAuthorization(AuthorizationFilterContext context)
		{
            //TODO: review together with Alex
			//if (HasAllowAnonymous(context))
			//{
			//	return;
			//}

			var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

			var claimUser = context.HttpContext.User;
			var result = authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile).GetAwaiter().GetResult();
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