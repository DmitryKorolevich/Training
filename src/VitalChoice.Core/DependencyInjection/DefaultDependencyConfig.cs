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
using Autofac;
using Microsoft.Framework.DependencyInjection.Autofac;

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

			// Add MVC services to the services container.
			services.AddMvc();
			// Uncomment the following line to add Web API servcies which makes it easier to port Web API 2 controllers.
			// You need to add Microsoft.AspNet.Mvc.WebApiCompatShim package to project.json
			// services.AddWebApiConventions();

			var builder = new ContainerBuilder();

			builder.Populate(services);

			builder.Register<IDataContextAsync>(x=>x.Resolve<VitalChoiceContext>());
			builder.RegisterGeneric(typeof(RepositoryAsync<>)).As(typeof(IRepositoryAsync<>));
			builder.RegisterType<CommentService>().As<ICommentService>();

			IContainer container = builder.Build();

			return container.Resolve<IServiceProvider>();
		}
	}
}