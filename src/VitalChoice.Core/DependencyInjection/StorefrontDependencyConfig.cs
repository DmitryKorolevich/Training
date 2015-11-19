using Autofac;
using cloudscribe.Web.Pagination;
using Microsoft.Extensions.DependencyInjection;

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
		}
    }
}
