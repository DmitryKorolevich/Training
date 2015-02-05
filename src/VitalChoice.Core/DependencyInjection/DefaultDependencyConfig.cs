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

            //Temp work arround for using custom pre-configuration action logic(BaseControllerActionInvoker).
            var describe = new ServiceDescriber(configuration);
            services.TryAdd(describe.Transient<INestedProvider<ActionInvokerProviderContext>, BaseControllerActionInvokerProvider>());

            // Add MVC services to the services container.
            services.AddMvc().Configure<MvcOptions>(options =>
			{
               
			});
            // Uncomment the following line to add Web API servcies which makes it easier to port Web API 2 controllers.
            // You need to add Microsoft.AspNet.Mvc.WebApiCompatShim package to project.json
            // services.AddWebApiConventions();

#if ASPNET50
            var builder = new ContainerBuilder();

			builder.Populate(services);

			builder.Register<IDataContextAsync>(x=>x.Resolve<VitalChoiceContext>());
			builder.RegisterGeneric(typeof(RepositoryAsync<>)).As(typeof(IRepositoryAsync<>));
			builder.RegisterType<CommentService>().As<ICommentService>();
            builder.RegisterType<SettingService>().As<ISettingService>().SingleInstance();
            builder.RegisterInstance(configuration).As<IConfiguration>();

            IContainer container = builder.Build();

            LocalizationService.Init(container.Resolve<IRepositoryAsync<LocalizationItemData>>(), container.Resolve<ISettingService>());

            return container.Resolve<IServiceProvider>();
#else

		    return null;
#endif


        }
    }
}