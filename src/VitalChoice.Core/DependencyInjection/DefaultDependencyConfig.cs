using System;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Context;
using VitalChoice.Domain.Entities;
using VitalChoice.Infrastructure;
using VItalChoice.Business.Services.Contracts;
using VItalChoice.Business.Services.Impl;

namespace VitalChoice.Core.DependencyInjection
{
    public class DefaultDependencyConfig : IDependencyConfig
    {
	    public void RegisterInfrastructure(IConfiguration configuration, IServiceCollection services)
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
		}

	    public void Register(IServiceCollection services)
	    {
		    services.AddScoped<IUnitOfWorkAsync, UnitOfWork>();
			services.AddTransient(typeof(IGenericService<>), typeof(GenericService<>));
			//services.AddTransient(typeof(IGenericService<User>), typeof(GenericService<User>));
			services.AddTransient<IUserService, UserService>();
		    services.AddTransient(typeof (IRepositoryAsync<>), typeof (RepositoryAsync<>));
	    }
	}
}