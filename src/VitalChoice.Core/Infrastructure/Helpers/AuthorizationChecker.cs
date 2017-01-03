using System;
using System.Linq;
using System.Security.Claims;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Core.Infrastructure.Helpers
{
    public static class AuthorizationChecker
    {
        public static bool IsValidCustomer(this ReferenceData reference, ClaimsPrincipal user)
        {
            if (user.HasClaim(x => x.Type == IdentityConstants.CustomerRoleType))
            {
                var customerRoles = reference.CustomerRoles;

                if (customerRoles.Any(role => user.IsInRole(role.Text.Normalize())))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsValidAffiliate(this ReferenceData reference, ClaimsPrincipal user)
        {
            if (user.HasClaim(x => x.Type == IdentityConstants.AffiliateRole))
            {
                var affiliateRoles = reference.AffiliateRoles;

                if (affiliateRoles.Any(role => user.IsInRole(role.Text.Normalize())))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasRole(this ReferenceData reference, ClaimsPrincipal user, RoleType role)
        {
            var roleLookup =
                reference.CustomerRoles.Concat(reference.AffiliateRoles)
                    .Concat(reference.AdminRoles)
                    .FirstOrDefault(r => r.Key == (int) role);
            return roleLookup != null && user.IsInRole(roleLookup.Text.Normalize());
        }
    }
}