﻿using System;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Templates;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Business.Services.Contracts.Content.ContentProcessors;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Business.Services.Impl.Content;
using VitalChoice.Business.Services.Impl.Content.ContentProcessors;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Business.Services.Impl.Product;
using VitalChoice.Business.Services.Contracts.Product;
using VitalChoice.Business.Services.Impl.Settings;
using VitalChoice.Business.Services.Contracts.Settings;
#if DNX451
using Autofac;
using Microsoft.Framework.DependencyInjection.Autofac;
using VitalChoice.Data.Repositories.Specifics;
#endif

namespace VitalChoice.Core.DependencyInjection
{
    public class DefaultDependencyConfig : IDependencyConfig
    {
        private static bool _called = false;

        public IServiceProvider RegisterInfrastructure(IConfiguration configuration, IServiceCollection services, string appPath)
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
	            services.AddIdentity<ApplicationUser, IdentityRole<int>>().AddEntityFrameworkStores<VitalChoiceContext, int>().AddUserStore<ExtendedUserStore>();

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
					options.DefaultCacheExpirationTermMinutes = Convert.ToInt32(configuration.Get("App:DefaultCacheExpirationTermMinutes"));
					options.ActivationTokenExpirationTermDays = Convert.ToInt32(configuration.Get("App:ActivationTokenExpirationTermDays"));
					options.DefaultCultureId= configuration.Get("App:DefaultCultureId");
					options.Connection = new Connection
                    {
                        UserName = configuration.Get("App:Connection:UserName"),
                        Password = configuration.Get("App:Connection:Password"),
                        Server = configuration.Get("App:Connection:Server"),
                    };
                    options.PublicHost = configuration.Get("App:PublicHost");
                });

				services.ConfigureIdentity(x =>
				{
					x.User.RequireUniqueEmail = true;
					x.Lockout.MaxFailedAccessAttempts = 5;
					x.Lockout.EnabledByDefault = true;
					x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
					x.Password.RequiredLength = 8;
					x.Password.RequireDigit = true;
					x.Password.RequireNonLetterOrDigit = true;
				});

#if DNX451
				var builder = new ContainerBuilder();

                builder.Populate(services);

                builder.Register<IDataContextAsync>(x => x.Resolve<VitalChoiceContext>());
                builder.RegisterType<EcommerceContext>();
                builder.RegisterType<LogsContext>();
                builder.RegisterGeneric(typeof(RepositoryAsync<>))
                    .As(typeof(IRepositoryAsync<>));
                builder.RegisterGeneric(typeof(EcommerceRepositoryAsync<>))
                    .As(typeof(IEcommerceRepositoryAsync<>))
                    .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
                builder.RegisterGeneric(typeof(LogsRepositoryAsync<>))
                    .As(typeof(ILogsRepositoryAsync<>))
                    .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<LogsContext>());
                builder.RegisterType<ContentViewService>().As<IContentViewService>();
                builder.RegisterType<LogViewService>().As<ILogViewService>();
                builder.RegisterType<MasterContentService>().As<IMasterContentService>();
                builder.RegisterType<GeneralContentService>().As<IGeneralContentService>();
                builder.RegisterType<CategoryService>().As<ICategoryService>();
                builder.RegisterType<RecipeService>().As<IRecipeService>();
                builder.RegisterType<FAQService>().As<IFAQService>();
                builder.RegisterType<ArticleService>().As<IArticleService>();
                builder.RegisterType<ContentPageService>().As<IContentPageService>();
                builder.RegisterType<TtlGlobalCache>().As<ITtlGlobalCache>().SingleInstance();
                builder.RegisterType<ContentProcessorsService>().As<IContentProcessorsService>().SingleInstance();
                builder.RegisterInstance(configuration).As<IConfiguration>();
				builder.RegisterType<CustomUrlHelper>().As<IUrlHelper>();
                builder.RegisterType<FrontEndAssetManager>().As<FrontEndAssetManager>().SingleInstance();
				builder.RegisterType<MemoryCache>().As<IMemoryCache>();
	            builder.RegisterType<CacheProvider>().As<ICacheProvider>().SingleInstance();
	            builder.RegisterType<AppInfrastructureService>().As<IAppInfrastructureService>();
	            builder.RegisterType<UserService>().As<IUserService>();
                builder.RegisterType<ProductViewService>().As<IProductViewService>();
                builder.RegisterType<ProductCategoryService>().As<IProductCategoryService>();
                builder.RegisterType<CountryService>().As<ICountryService>();
                builder.RegisterType<SettingService>().As<ISettingService>();
                builder.RegisterType<FileService>().As<IFileService>();
                IContainer container = builder.Build();

                LocalizationService.Init(container.Resolve<IRepositoryAsync<LocalizationItemData>>(), configuration.Get("App:DefaultCultureId"));
                if (!String.IsNullOrEmpty(appPath))
                {
                    FileService.Init(appPath);
                }

                return container.Resolve<IServiceProvider>();
#else

		        return null;
#endif

            }
            return null;
        }
    }
}