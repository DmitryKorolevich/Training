using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using System.Linq;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VitalChoice.Core.Infrastructure
{
    public class AffiliateAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        protected void Fail(AuthorizationFilterContext context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object> {{"returnUrl", context.HttpContext.Request.Path}};
            context.Result = new RedirectToActionResult("Login", "AffiliateAccount", parameters);
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

            var claimUser = context.HttpContext.User;
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                var result =
                    authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile).GetAwaiter().GetResult();
                if (result)
                {
                    if (claimUser.HasClaim(x => x.Type == IdentityConstants.AffiliateRole))
                    {
                        var affiliateRoles =
                            context.HttpContext.RequestServices.GetService<IAppInfrastructureService>()
                                .Data()
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

        public static async Task<bool> IsAuthenticated(HttpContext context)
        {
            bool toReturn = false;
            if (context.User.Identity.IsAuthenticated)
            {
                var claimUser = context.User;
                var authorizationService = context.RequestServices.GetService<IAuthorizationService>();
                var result = await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
                if (result)
                {
                    if (claimUser.HasClaim(x => x.Type == IdentityConstants.AffiliateRole))
                    {
                        toReturn = true;
                    }
                }
            }
            return toReturn;
        }
    }
}