using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;

namespace VitalChoice.Core.DependencyInjection
{
    public class AdminDependencyConfig : DefaultDependencyConfig
    {
	    protected override void BeginCustomRegistrations(IServiceCollection builder)
	    {
			builder.AddCookieAuthentication(x =>
            {
                x.AuthenticationScheme = IdentityCookieOptions.ApplicationCookieAuthenticationType;
                x.LogoutPath = PathString.Empty;
                x.AccessDeniedPath = PathString.Empty;
                x.LoginPath = PathString.Empty;
                x.ReturnUrlParameter = null;
				x.CookieName = "VitalChoice.Admin";
			});
		}
    }
}
