using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Internal;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime.Infrastructure;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Identity;
using AuthorizationContext = Microsoft.AspNet.Mvc.AuthorizationContext;

namespace VitalChoice.Core.Infrastructure
{
	public class AdminAuthorizeAttribute : AuthorizationFilterAttribute
	{
		private readonly IList<int> permissions;

		public AdminAuthorizeAttribute(params PermissionType[] permissions)
		{
			this.permissions = permissions.Select(x => Convert.ToInt32(x)).ToList();
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
			var superAdmin =
				context.HttpContext.ApplicationServices.GetService<IAppInfrastructureService>()
					.Get()
					.Roles.Single(x => x.Key == (int) RoleType.SuperAdminUser)
					.Text;

			var claimUser = context.HttpContext.User;
			var result = await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
			if (result)
			{
				if (claimUser.IsInRole(superAdmin.Normalize()))
				{
					return;
				}

				if (
					permissions.Any(
						permission =>
							context.HttpContext.User.HasClaim(
								x => x.Type == IdentityConstants.PermissionRoleClaimType && x.Value == permission.ToString())))
				{
					return;
				}
			}

			Fail(context);
		}
	}
}