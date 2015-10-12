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
			builder.ConfigureIdentityApplicationCookie(x =>
			{
				x.AuthenticationScheme = IdentityOptions.ApplicationCookieAuthenticationScheme;
				x.LogoutPath = PathString.Empty;
				x.AccessDeniedPath = PathString.Empty;
				x.LoginPath = PathString.Empty;
				x.ReturnUrlParameter = null;
			});
		}
    }
}
