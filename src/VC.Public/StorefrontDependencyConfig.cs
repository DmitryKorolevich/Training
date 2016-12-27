using Autofac;
using cloudscribe.Web.Pagination;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VC.Public.DataAnnotations;
using VitalChoice.Business.Services;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Interfaces.Services;

namespace VC.Public
{
    public class StorefrontDependencyConfig : DefaultDependencyConfig
    {
        protected override void AddMvc(IServiceCollection services)
        {
            services.TryAddSingleton<CustomValidateAntiforgeryTokenAuthorizationFilter>();
            services.AddMvc().AddMvcOptions(options => options.ModelBinderProviders.Insert(0, new AntiXssModelBinderProvider()));
        }

        protected override void StartCustomServicesRegistration(IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSession();
            services.Replace(new ServiceDescriptor(typeof(IHtmlGenerator), typeof(StoreFrontHtmlGenerator), ServiceLifetime.Scoped));
            services.Replace(new ServiceDescriptor(typeof(IValidationAttributeAdapterProvider),
                typeof(CustomValidationAttributeAdapterProvider), ServiceLifetime.Singleton));
        }

        protected override void FinishCustomRegistrations(ContainerBuilder builder)
        {
            builder.RegisterType<PaginationLinkBuilder>().As<IBuildPaginationLinks>().SingleInstance();
            builder.RegisterType<RedirectViewService>().As<IRedirectViewService>().SingleInstance();
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
