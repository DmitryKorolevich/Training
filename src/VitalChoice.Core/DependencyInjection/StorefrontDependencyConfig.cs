using Autofac;
using cloudscribe.Web.Pagination;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VitalChoice.Business.Services;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Core.Services;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Core.DependencyInjection
{
    public class StorefrontDependencyConfig:DefaultDependencyConfig
    {
        protected override void StartCustomServicesRegistration(IServiceCollection services)
        {
            services.AddCaching();
            services.AddSession();
            services.Replace(new ServiceDescriptor(typeof (IHtmlGenerator), typeof (StoreFrontHtmlGenerator), ServiceLifetime.Scoped));
        }

        protected override void FinishCustomRegistrations(ContainerBuilder builder)
	    {
            builder.RegisterType<PaginationLinkBuilder>().As<IBuildPaginationLinks>();
            builder.RegisterType<RedirectViewService>().As<IRedirectViewService>().InstancePerLifetimeScope();
        }

	    protected override void PopulateCookieIdentityOptions(CookieAuthenticationOptions options)
	    {
			base.PopulateCookieIdentityOptions(options);

			options.LogoutPath = "/Account/Logout";
			options.AccessDeniedPath = "/Shared/AccessDenied";
			options.LoginPath = "/Account/Login";
			options.ReturnUrlParameter = "returnUrl";
			options.CookieName = "VitalChoice.Public";
		}
    }
}
