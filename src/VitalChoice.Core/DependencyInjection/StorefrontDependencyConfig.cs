using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;

namespace VitalChoice.Core.DependencyInjection
{
    public class StorefrontDependencyConfig:DefaultDependencyConfig
    {
	    protected override void BeginCustomRegistrations(IServiceCollection builder)
	    {
			builder.ConfigureIdentityApplicationCookie(x =>
			{
				x.AuthenticationScheme = IdentityOptions.ApplicationCookieAuthenticationScheme;
				x.LogoutPath = "/Account/Logout";
				x.AccessDeniedPath = "/Shared/AccessDenied";
				x.LoginPath = "/Account/Login";
				x.ReturnUrlParameter = "returnUrl";
			});
		}
    }
}
