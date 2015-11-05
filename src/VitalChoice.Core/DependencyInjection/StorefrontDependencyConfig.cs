using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using cloudscribe.Web.Pagination;
using Microsoft.AspNet.Identity;
using Microsoft.Framework.DependencyInjection;

namespace VitalChoice.Core.DependencyInjection
{
    public class StorefrontDependencyConfig:DefaultDependencyConfig
    {
	    protected override void BeginCustomRegistrations(IServiceCollection builder)
	    {
			builder.AddCookieAuthentication(x =>
			{
				x.AuthenticationScheme = IdentityCookieOptions.ApplicationCookieAuthenticationType;
				x.LogoutPath = "/Account/Logout";
				x.AccessDeniedPath = "/Shared/AccessDenied";
				x.LoginPath = "/Account/Login";
				x.ReturnUrlParameter = "returnUrl";
			});
		}

	    protected override void FinishCustomRegistrations(ContainerBuilder builder)
	    {
            builder.RegisterType<PaginationLinkBuilder>().As<IBuildPaginationLinks>();
		}
    }
}
