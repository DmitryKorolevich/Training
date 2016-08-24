using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Core.Infrastructure
{
    [AttributeUsage(AttributeTargets.All)]
    public class AdminAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IList<int> _permissions;

        public AdminAuthorizeAttribute()
        {
            _permissions = null;
        }

        public AdminAuthorizeAttribute(params PermissionType[] permissions) : this()
        {
            _permissions = permissions.Select(x => Convert.ToInt32(x)).ToList();
        }

        protected void Fail(AuthorizationFilterContext context)
        {
            context.Result = context.HttpContext.User.Identity.IsAuthenticated ? new StatusCodeResult(403) : new UnauthorizedResult();
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var authorizationService = context.HttpContext.RequestServices.GetService<IAuthorizationService>();

            var claimUser = context.HttpContext.User;
            var result =
                await authorizationService.AuthorizeAsync(claimUser, null, IdentityConstants.IdentityBasicProfile);
            if (result)
            {
                if (_permissions == null || !(_permissions.Count > 0))
                {
                    return;
                }

                var superAdmin =
                    context.HttpContext.RequestServices.GetService<ReferenceData>()
                        .AdminRoles.Single(x => x.Key == (int) RoleType.SuperAdminUser)
                        .Text;

                if (claimUser.IsInRole(superAdmin.Normalize()))
                {
                    return;
                }

                if (
                    _permissions.Any(
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