using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Identity
{
    public class ExtendedClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        public ExtendedClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor) : base(userManager, roleManager, optionsAccessor)
        {
        }

        public override async Task<ClaimsPrincipal> CreateAsync(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            string userId = (string) await this.UserManager.GetUserIdAsync(user);
            string str1 = (string) await this.UserManager.GetUserNameAsync(user);
            ClaimsIdentity id = new ClaimsIdentity(this.Options.Cookies.ApplicationCookieAuthenticationScheme,
                this.Options.ClaimsIdentity.UserNameClaimType, this.Options.ClaimsIdentity.RoleClaimType);
            id.AddClaim(new Claim(this.Options.ClaimsIdentity.UserIdClaimType, userId));
            id.AddClaim(new Claim(this.Options.ClaimsIdentity.UserNameClaimType, str1));
            ClaimsIdentity claimsIdentity;
            if (this.UserManager.SupportsUserSecurityStamp)
            {
                claimsIdentity = id;
                string str = this.Options.ClaimsIdentity.SecurityStampClaimType;
                string securityStampAsync = await this.UserManager.GetSecurityStampAsync(user);
                claimsIdentity.AddClaim(new Claim(str, securityStampAsync));
                str = null;
            }
            if (this.UserManager.SupportsUserRole)
            {
                foreach (string roleName in await this.UserManager.GetRolesAsync(user))
                {
                    id.AddClaim(new Claim(this.Options.ClaimsIdentity.RoleClaimType, roleName));
                    if (this.RoleManager.SupportsRoleClaims)
                    {
                        var role = await this.RoleManager.FindByNameAsync(roleName);
                        if ((object) role != null)
                        {
                            claimsIdentity = id;
                            IList<Claim> claimsAsync = await this.RoleManager.GetClaimsAsync(role);
                            claimsIdentity.AddClaims(claimsAsync);
                        }
                    }
                }
            }
            if (this.UserManager.SupportsUserClaim)
            {
                claimsIdentity = id;
                IList<Claim> claimsAsync = await this.UserManager.GetClaimsAsync(user);
                claimsIdentity.AddClaims(claimsAsync);
                if (!user.IsConfirmed)
                {
                    claimsIdentity.AddClaim(new Claim(IdentityConstants.NotConfirmedClaimType, true.ToString()));
                }
            }
            return new ClaimsPrincipal(id);
        }
    }
}