﻿using System;
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
using AuthorizationContext = Microsoft.AspNet.Mvc.Filters.AuthorizationContext;
using Microsoft.AspNet.Mvc.Filters;
using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.Core.Infrastructure
{
	public class CustomerAuthorizeAttribute : AuthorizationFilterAttribute
	{
		private readonly IList<int> _roles;

		public CustomerAuthorizeAttribute()
		{
			_roles = null;
		}

		public CustomerAuthorizeAttribute(params CustomerType[] roles):this()
		{
			_roles = roles.Select(x => Convert.ToInt32(x)).ToList();
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
				if (_roles == null || !_roles.Any())
				{
					return;
				}

				if (claimUser.HasClaim(x=>x.Type == IdentityConstants.CustomerRoleType))
				{
					var customerRoles =
						context.HttpContext.ApplicationServices.GetService<IAppInfrastructureService>()
							.Get()
							.CustomerRoles;

					if (customerRoles.Any(customerRole => claimUser.IsInRole(customerRole.Text.Normalize())))
					{
						return;
					}
				}
			}

			Fail(context);
		}
	}
}