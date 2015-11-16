using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using AuthorizationContext = Microsoft.AspNet.Mvc.Filters.AuthorizationContext;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Http;
using System.Linq;
using System;

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
                            context.HttpContext.RequestServices.GetService<IAppInfrastructureService>()
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

        public static async Task<bool> IsAuthenticated(HttpContext context)
        {
            bool toReturn = false;
            if (context.User.Identity.IsAuthenticated)
            {
                var claimUser = context.User;
                var authorizationService = context.ApplicationServices.GetService<IAuthorizationService>();
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