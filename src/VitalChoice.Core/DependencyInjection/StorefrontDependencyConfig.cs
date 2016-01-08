using Autofac;
using cloudscribe.Web.Pagination;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Business.Services;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Core.DependencyInjection
{
    public class StorefrontDependencyConfig:DefaultDependencyConfig
    {
        protected override void StartCustomServicesRegistration(IServiceCollection services)
        {
            services.AddCaching();
            services.AddSession();
        }

        protected override void FinishCustomRegistrations(ContainerBuilder builder)
	    {
            builder.RegisterType<PaginationLinkBuilder>().As<IBuildPaginationLinks>();
            builder.RegisterType<RedirectViewService>().As<IRedirectViewService>().SingleInstance();
        }
    }
}
