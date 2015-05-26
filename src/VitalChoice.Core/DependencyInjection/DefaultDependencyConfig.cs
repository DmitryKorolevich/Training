using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Core;
using Microsoft.Framework.Caching.Memory;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Templates;
using VitalChoice.Business.Mail;
using VitalChoice.Business.Services;
using VitalChoice.Business.Services.Content;
using VitalChoice.Business.Services.Content.ContentProcessors;
using VitalChoice.Business.Services.Product;
using VitalChoice.Business.Services.Settings;
using VitalChoice.Business.Services.Workflow;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Content.ContentProcessors;
using VitalChoice.Interfaces.Services.Product;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Validation.Base;
using VitalChoice.Workflow.Core;
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

				services.ConfigureIdentityApplicationCookie(x =>
				{
					x.AuthenticationScheme = IdentityOptions.ApplicationCookieAuthenticationScheme;
					x.LogoutPath = PathString.Empty;
					x.AccessDeniedPath = PathString.Empty;
					x.LoginPath = PathString.Empty;
					x.ReturnUrlParameter = null;
				});

				// Add EF services to the services container.
				services.AddEntityFramework() //.AddMigrations()
                    .AddSqlServer()
                    .AddDbContext<VitalChoiceContext>();

                // Add Identity services to the services container.
	            services.AddIdentity<ApplicationUser, IdentityRole<int>>()
		            .AddEntityFrameworkStores<VitalChoiceContext, int>()
		            .AddUserStore<ExtendedUserStore>()
					.AddUserValidator<ExtendedUserValidator>()
					.AddUserManager<ExtendedUserManager>()
					.AddTokenProvider<UserTokenProvider>();
				
				//Temp work arround for using custom pre-configuration action logic(BaseControllerActionInvoker).
                services.TryAdd(
                    ServiceDescriptor
                        .Transient<IActionInvokerProvider, ValidationActionInvokerProvider>());

                // Add MVC services to the services container.
                services.AddMvc();

				services.AddOptions();

				services.AddAuthorization();

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
                    options.AdminHost = configuration.Get("App:AdminHost");
                    options.FilesRelativePath = configuration.Get("App:FilesRelativePath");
                    options.EmailConfiguration = new Email
	                {
						 From = configuration.Get("App:Email:From"),
						 Host = configuration.Get("App:Email:Host"),
						 Port = Convert.ToInt32(configuration.Get("App:Email:Port")),
						 Secured = Convert.ToBoolean(configuration.Get("App:Email:Secured")),
						 Username = configuration.Get("App:Email:Username"),
						 Password = configuration.Get("App:Email:Password")
	                };
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

				services.ConfigureAuthorization(x => x.AddPolicy(IdentityConstants.IdentityBasicProfile, y => y.RequireAuthenticatedUser()));

				

#if DNX451
				var builder = new ContainerBuilder();

                builder.Populate(services);

                builder.Register<IDataContextAsync>(x => x.Resolve<VitalChoiceContext>());
                builder.RegisterType<EcommerceContext>();
                builder.RegisterType<LogsContext>();
                builder.RegisterGeneric(typeof(RepositoryAsync<>))
                    .As(typeof(IRepositoryAsync<>));
                builder.RegisterGeneric(typeof(ReadRepositoryAsync<>))
                    .As(typeof(IReadRepositoryAsync<>));
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
                builder.RegisterType<EmailSender>().As<IEmailSender>().WithParameter((pi, cc) => pi.Name == "options", (pi, cc) => cc.Resolve<IOptions<AppOptions>>()).SingleInstance();
				builder.RegisterType<NotificationService>().As<INotificationService>();
                builder.RegisterType<GCService>().As<IGCService>();
                builder.RegisterType<CountryService>().As<ICountryService>();
                builder.RegisterType(typeof(ExtendedUserValidator)).As(typeof(IUserValidator<ApplicationUser>));
                builder.RegisterType<ActionItemProvider>().As<IActionItemProvider>().SingleInstance();
                builder.RegisterType<WorkflowFactory>().As<IWorkflowFactory>().SingleInstance();
                var container = builder.Build();

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