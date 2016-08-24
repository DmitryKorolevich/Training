using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Core.Infrastructure.Models;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Tokens;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;
using VitalChoice.Interfaces.Services.Users;

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
            var parameters = new Dictionary<string, object>
            {
                {"returnUrl", context.HttpContext.Request.Path}
            };
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