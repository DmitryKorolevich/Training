using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VitalChoice.Core.Infrastructure
{
    public class CustomerAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly bool _notConfirmedAllowed;

        public CustomerAuthorizeAttribute(bool notConfirmedAllowed = false)
        {
            _notConfirmedAllowed = notConfirmedAllowed;
        }

        protected void Fail(AuthorizationFilterContext context)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("returnUrl", context.HttpContext.Request.Path);
            context.Result = new RedirectToActionResult("Login", "Account", parameters);
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

            var claimUser = context.HttpContext.User;
            var result =
                authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile).GetAwaiter().GetResult();
            if (result)
            {
                if (!_notConfirmedAllowed && claimUser.HasClaim(x => x.Type == IdentityConstants.NotConfirmedClaimType))
                {
                    Fail(context);
                }

                if (claimUser.HasClaim(x => x.Type == IdentityConstants.CustomerRoleType))
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
                    if (claimUser.HasClaim(x => x.Type == IdentityConstants.AffiliateRole))
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