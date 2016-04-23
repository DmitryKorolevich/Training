using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using AuthorizationContext = Microsoft.AspNet.Mvc.Filters.AuthorizationContext;
using Microsoft.AspNet.Mvc.Filters;

namespace VitalChoice.Core.Infrastructure
{
	public class CustomerAuthorizeAttribute : AuthorizationFilterAttribute
	{
		private readonly bool _notConfirmedAllowed;

		public CustomerAuthorizeAttribute(bool notConfirmedAllowed = false)
		{
			_notConfirmedAllowed = notConfirmedAllowed;
		}

		protected override void Fail(AuthorizationContext context)
		{
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("returnUrl", context.HttpContext.Request.Path);
            context.Result = new RedirectToActionResult("Login", "Account", parameters);
		}

		public override async Task OnAuthorizationAsync(AuthorizationContext context)
		{
			var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

			var claimUser = context.HttpContext.User;
			var result = await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
			if (result)
			{
				if (!_notConfirmedAllowed && claimUser.HasClaim(x => x.Type == IdentityConstants.NotConfirmedClaimType))
				{
					Fail(context);
				}

				if (claimUser.HasClaim(x=>x.Type == IdentityConstants.CustomerRoleType))
				{
					var customerRoles =
						context.HttpContext.RequestServices.GetService<IAppInfrastructureService>()
							.Data()
							.CustomerRoles;

					if (customerRoles.Any(customerRole => claimUser.IsInRole(customerRole.Text.Normalize())))
					{
						return;
					}
				}
                else
                {
                    if(claimUser.HasClaim(x => x.Type == IdentityConstants.AffiliateRole))
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>();
                        parameters.Add("returnUrl", context.HttpContext.Request.Path);
                        context.Result = new RedirectToActionResult("Login", "Account", parameters);
                        return;
                    }
                }
			}

			Fail(context);
		}
	}
}