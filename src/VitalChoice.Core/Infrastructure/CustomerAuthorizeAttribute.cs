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
using VitalChoice.Ecommerce.Domain.Entities.Customers;

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
				if (claimUser.HasClaim(x=>x.Type == IdentityConstants.CustomerRoleType))
				{
					var customerRoles =
						context.HttpContext.RequestServices.GetService<IAppInfrastructureService>()
							.Get()
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