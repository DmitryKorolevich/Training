using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace VitalChoice.Core.DependencyInjection
{
    public class AdminDependencyConfig : DefaultDependencyConfig
    {
		protected override void PopulateCookieIdentityOptions(CookieAuthenticationOptions options)
		{
			base.PopulateCookieIdentityOptions(options);

			options.LogoutPath = PathString.Empty;
			options.AccessDeniedPath = PathString.Empty;
			options.LoginPath = PathString.Empty;
			options.ReturnUrlParameter = null;
			options.CookieName = "VitalChoice.Admin";
		}
	}
}
