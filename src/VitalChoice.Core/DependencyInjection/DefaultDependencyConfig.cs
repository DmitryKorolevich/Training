using Microsoft.AspNet.Identity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain;
using VitalChoice.Validation.Controllers;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Business.Services.Impl;
using System;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Entities.Localization.Groups;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc.Core;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Core.Infrastructure;

#if DNX451
using Autofac;
using Autofac.Core;
using Microsoft.Framework.DependencyInjection.Autofac;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Business.Services.Contracts.ContentProcessors;
using VitalChoice.Business.Services.Impl.ContentProcessors;
#endif

namespace VitalChoice.Core.DependencyInjection
{
	public class DefaultDependencyConfig : IDependencyConfig
	{
	    private static bool _called = false;

		public IServiceProvider RegisterInfrastructure(IConfiguration configuration, IServiceCollection services)
		{
		    if (!_called)
		    {
		        _called = true;
                // Add EF services to the services container.
                services.AddEntityFramework() //.AddMigrations()
		            .AddSqlServer()
		            .AddDbContext<VitalChoiceContext>();

		        // Add Identity services to the services container.
		        //services.AddDefaultIdentity<VitalChoiceContext, ApplicationUser, IdentityRole>(Configuration);
		        services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<VitalChoiceContext>();

		        //Temp work arround for using custom pre-configuration action logic(BaseControllerActionInvoker).
		        services.TryAdd(
		            ServiceDescriptor
		                .Transient<IActionInvokerProvider, Validation.Controllers.ControllerActionInvokerProvider>());

		        // Add MVC services to the services container.
		        services.AddMvc(); //.Configure<MvcOptions>(options =>
		        //{

		        //	});

		        services.AddOptions();

		        services.Configure<AppOptions>(options =>
		        {
		            options.ServeCdnContent = Convert.ToBoolean(configuration.Get("App:ServeCdnContent"));
		            options.CdnServerBaseUrl = configuration.Get("App:CdnServerBaseUrl");
		            options.GenerateLowercaseUrls = Convert.ToBoolean(configuration.Get("App:GenerateLowercaseUrls"));
		            options.EnableBundlingAndMinification =
		                Convert.ToBoolean(configuration.Get("App:EnableBundlingAndMinification"));
		            options.RandomPathPart = new DateTime().ToString("dd-mm-yyyy");
		            options.LogPath = configuration.Get("App:LogPath");
		        });

#if DNX451
            var builder = new ContainerBuilder();

			builder.Populate(services);

			builder.Register<IDataContextAsync>(x=>x.Resolve<VitalChoiceContext>());
			builder.RegisterType<EcommerceContext>();
            builder.RegisterType<LogsContext>();
            builder.RegisterGeneric(typeof(RepositoryAsync<>))
				.As(typeof(IRepositoryAsync<>));
            builder.RegisterGeneric(typeof (EcommerceRepositoryAsync<>))
				.As(typeof (IEcommerceRepositoryAsync<>))
				.WithParameter((pi,cc)=> pi.Name == "context", (pi,cc)=>cc.Resolve<EcommerceContext>());
            builder.RegisterGeneric(typeof(LogsRepositoryAsync<>))
                .As(typeof(ILogsRepositoryAsync<>))
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<LogsContext>());
            builder.RegisterType<CommentService>().As<ICommentService>();
            builder.RegisterType<ContentService>().As<IContentService>();
            builder.RegisterType<LogViewService>().As<ILogViewService>();
            builder.RegisterType<SettingService>().As<ISettingService>().SingleInstance();
            builder.RegisterType<ContentProcessorsService>().As<IContentProcessorsService>().SingleInstance();
            builder.RegisterInstance(configuration).As<IConfiguration>();

            builder.RegisterType<CustomUrlHelper>().As<IUrlHelper>();
			builder.RegisterType<FrontEndAssetManager>().As<FrontEndAssetManager>().SingleInstance();

            IContainer container = builder.Build();

            LocalizationService.Init(container.Resolve<IRepositoryAsync<LocalizationItemData>>(), container.Resolve<ISettingService>());

            return container.Resolve<IServiceProvider>();
#else

		        return null;
#endif

		    }
		    return null;
		}
    }
}