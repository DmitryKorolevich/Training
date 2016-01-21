using System.Linq;
using System.Security.Claims;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Core.Infrastructure.Helpers
{
    public static class AuthorizationChecker
    {
        public static bool IsValidCustomer(this IAppInfrastructureService service, ClaimsPrincipal user)
        {
            if (user.HasClaim(x => x.Type == IdentityConstants.CustomerRoleType))
            {
                var customerRoles = service.Get().CustomerRoles;

                if (customerRoles.Any(role => user.IsInRole(role.Text.Normalize())))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsValidAffiliate(this IAppInfrastructureService service, ClaimsPrincipal user)
        {
            if (user.HasClaim(x => x.Type == IdentityConstants.AffiliateRole))
            {
                var affiliateRoles = service.Get().AffiliateRoles;

                if (affiliateRoles.Any(role => user.IsInRole(role.Text.Normalize())))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasRole(this IAppInfrastructureService service, ClaimsPrincipal user, RoleType role)
        {
            var appData = service.Get();
            var roleLookup =
                appData.CustomerRoles.Union(appData.AffiliateRoles)
                    .Union(appData.AdminRoles)
                    .FirstOrDefault(r => r.Key == (int) role);
            return roleLookup != null && user.IsInRole(roleLookup.Text.Normalize());
        }
    }
}