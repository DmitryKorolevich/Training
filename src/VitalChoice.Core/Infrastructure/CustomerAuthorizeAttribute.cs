using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Core.Infrastructure
{
    public class CustomerAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly bool _notConfirmedAllowed;

        public CustomerAuthorizeAttribute(bool notConfirmedAllowed = false)
        {
            _notConfirmedAllowed = notConfirmedAllowed;
        }

        protected void Fail(AuthorizationFilterContext context)
        {
            var parameters = new Dictionary<string, object>
            {
                {"returnUrl", context.HttpContext.Request.Path}
            };
            context.Result = new RedirectToActionResult("Login", "Account", parameters);
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

            var claimUser = context.HttpContext.User;
            var result =
                await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
            if (result)
            {
                if (!_notConfirmedAllowed && claimUser.HasClaim(x => x.Type == IdentityConstants.NotConfirmedClaimType))
                {
                    Fail(context);
                }

                if (claimUser.HasClaim(x => x.Type == IdentityConstants.CustomerRoleType))
                {
                    var customerRoles =
                        context.HttpContext.RequestServices.GetService<ReferenceData>()
                            .CustomerRoles;

                    if (customerRoles.Any(customerRole => claimUser.IsInRole(customerRole.Text.Normalize())))
                    {
                        return;
                    }
                }
                else
                {
                    if (claimUser.HasClaim(x => x.Type == IdentityConstants.AffiliateRole))
                    {
                        Dictionary<string, object> parameters = new Dictionary<string, object>
                        {
                            {"returnUrl", context.HttpContext.Request.Path}
                        };
                        context.Result = new RedirectToActionResult("Login", "Account", parameters);
                        return;
                    }
                }
            }

            Fail(context);
        }

        
    }
}