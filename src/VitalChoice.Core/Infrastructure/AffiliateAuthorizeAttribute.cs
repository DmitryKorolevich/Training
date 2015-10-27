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
	public class AffiliateAuthorizeAttribute : AuthorizationFilterAttribute
	{
		public AffiliateAuthorizeAttribute()
		{
		}

		protected override void Fail(AuthorizationContext context)
		{
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("returnUrl", context.HttpContext.Request.Path);
            context.Result = new RedirectToActionResult("Login", "AffiliateAccount", parameters);
        }

		public override async Task OnAuthorizationAsync(AuthorizationContext context)
		{
			var authorizationService = context.HttpContext.ApplicationServices.GetService<IAuthorizationService>();

			var claimUser = context.HttpContext.User;
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var result = await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
                if (result)
                {
                    if (claimUser.HasClaim(x => x.Type == IdentityConstants.AffiliateRole))
                    {
                        var affiliateRoles =
                            context.HttpContext.ApplicationServices.GetService<IAppInfrastructureService>()
                                .Get()
                                .AffiliateRoles;

                        if (affiliateRoles.Any(affiliateRole => claimUser.IsInRole(affiliateRole.Text.Normalize())))
                        {
                            return;
                        }
                    }
                }
            }

			Fail(context);
		}
	}
}