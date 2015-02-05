using Microsoft.AspNet.Identity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Business.Services.Impl;
using System;
using Microsoft.AspNet.Mvc;
using VitalChoice.Core.Infrastructure;

#if ASPNET50
using Autofac;
using Microsoft.Framework.DependencyInjection.Autofac;
#endif

namespace VitalChoice.Core.DependencyInjection
{
	public class DefaultDependencyConfig : IDependencyConfig
    {
		public IServiceProvider RegisterInfrastructure(IConfiguration configuration, IServiceCollection services)
		{
			// Add EF services to the services container.
			services.AddEntityFramework(configuration)//.AddMigrations()
				.AddSqlServer()
				.AddDbContext<VitalChoiceContext>();

			// Add Identity services to the services container.
			//services.AddDefaultIdentity<VitalChoiceContext, ApplicationUser, IdentityRole>(Configuration);
			services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<VitalChoiceContext>();

			services.AddOptions(configuration);

			// Add MVC services to the services container.
			services.AddMvc();

			services.Configure<AppOptions>(options =>
			{
				options.ServeCdnContent = Convert.ToBoolean(configuration.Get("App:ServeCdnContent"));
				options.CdnServerBaseUrl = configuration.Get("App:CdnServerBaseUrl");
				options.GenerateLowercaseUrls = Convert.ToBoolean(configuration.Get("App:GenerateLowercaseUrls"));
				options.EnableBundlingAndMinification = Convert.ToBoolean(configuration.Get("App:EnableBundlingAndMinification"));
				options.RandomPathPart = new DateTime().ToString("dd-mm-yyyy");
			});

#if ASPNET50
			var builder = new ContainerBuilder();

			builder.Populate(services);

			builder.Register<IDataContextAsync>(x=>x.Resolve<VitalChoiceContext>());
			builder.RegisterGeneric(typeof(RepositoryAsync<>)).As(typeof(IRepositoryAsync<>));
			builder.RegisterType<CommentService>().As<ICommentService>();
            builder.RegisterType<LocalizationService>().As<ILocalizationService>().SingleInstance();

			builder.RegisterType<CustomUrlHelper>().As<IUrlHelper>();
			builder.RegisterType<FrontEndAssetManager>().As<FrontEndAssetManager>().SingleInstance();

            IContainer container = builder.Build();

			return container.Resolve<IServiceProvider>();
#else
		    return null;
#endif
		}
	}
}