using System;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Mail;
using VitalChoice.Business.Services;
using VitalChoice.Business.Services.Content;
using VitalChoice.Business.Services.Content.ContentProcessors;
using VitalChoice.Business.Services.Settings;
using VitalChoice.Business.Services.Workflow;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Workflow.Core;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using VitalChoice.Business.Services.Customers;
using VitalChoice.Business.Services.Orders;
using VitalChoice.Business.Services.Payment;
using VitalChoice.Business.Services.Products;
using VitalChoice.Core.Base;
using VitalChoice.Core.Services;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Interfaces.Services.Products;
using Autofac;
using Autofac.Core;
using VitalChoice.Data.Repositories.Specifics;
using Avalara.Avatax.Rest.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Business.Services.Affiliates;
using VitalChoice.Business.Services.Avatax;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Help;
using VitalChoice.Business.Services.HelpService;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Business.Services.FedEx;
using VitalChoice.Business.Services.Users;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Infrastructure.Identity.UserStores;
using VitalChoice.Infrastructure.Identity.Validators;
using VitalChoice.Interfaces.Services.Avatax;
using VitalChoice.Interfaces.Services.Users;
using Microsoft.AspNet.Mvc.Abstractions;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Extensions.PlatformAbstractions;
using VitalChoice.Workflow.Base;
using VitalChoice.ContentProcessing.Helpers;
using VitalChoice.DynamicData.Extensions;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Hosting;
using VitalChoice.Business.Repositories;
using VitalChoice.Core.Infrastructure.Helpers.ReCaptcha;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Context;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Business.Services.Healthwise;
using VitalChoice.Interfaces.Services.Healthwise;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.Business.Services.Checkout;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Business.Services.InventorySkus;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Services;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Data.Transaction;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.InventorySkus;
#if !DOTNET5_4
using VitalChoice.Caching.Interfaces;
using VitalChoice.Business.Services.Cache;
#endif

namespace VitalChoice.Core.DependencyInjection
{
    public abstract class DefaultDependencyConfig : IDependencyConfig
    {
        public IContainer RegisterInfrastructure(IConfiguration configuration, IServiceCollection services,
            Assembly projectAssembly, IApplicationEnvironment appEnv = null)
        {
            // Add EF services to the services container.
#if !DOTNET5_4
            services.AddEntityFramework()
                .AddEntityFrameworkCache<ServiceBusCacheSyncProvider>(new[] {typeof (VitalChoiceContext), typeof (EcommerceContext)})
                .AddSqlServer();
#else
            services.AddEntityFramework().AddEntityFrameworkCache<CacheSyncProvider>(new [] {typeof(VitalChoiceContext), typeof(EcommerceContext) }).AddSqlServer();
#endif

            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, ApplicationRole>(x =>
            {
                x.User.RequireUniqueEmail = true;
                x.Lockout.MaxFailedAccessAttempts = 10;
                x.Lockout.AllowedForNewUsers = true;
                x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(1);
                x.Password.RequiredLength = 8;
                x.Password.RequireDigit = true;
                x.Password.RequireNonLetterOrDigit = true;

	            PopulateCookieIdentityOptions(x.Cookies.ApplicationCookie);
            })
                .AddEntityFrameworkStores<VitalChoiceContext, int>()
                .AddErrorDescriber<ExtendedIdentityErrorDescriber>()
                .AddUserStore<AdminUserStore>()
                .AddUserValidator<AdminUserValidator>()
                .AddUserManager<ExtendedUserManager>()
                .AddTokenProvider<UserTokenProvider>(IdentityConstants.TokenProviderName);

            //Temp work arround for using custom pre-configuration action logic(BaseControllerActionInvoker).
            services.TryAdd(
                ServiceDescriptor
                    .Transient<IActionInvokerProvider, ValidationActionInvokerProvider>());

            // Add MVC services to the services container.
            AddMvc(services);

            services.AddAuthorization(
                x => x.AddPolicy(IdentityConstants.IdentityBasicProfile, y => y.RequireAuthenticatedUser()));

            services.Configure<AppOptionsBase>(options =>
            {
                ConfigureBaseOptions(configuration, options);
            });

            services.Configure<AppOptions>(options =>
            {
                ConfigureAppOptions(configuration, options);
                options.PDFMyUrl = new PDFMyUrl
                {
                    LicenseKey = configuration.GetSection("App:PDFMyUrl:LicenseKey").Value,
                    ServiceUrl = configuration.GetSection("App:PDFMyUrl:ServiceUrl").Value,
                };
            });

            services.Configure<MvcOptions>(ConfigureMvcOptions);

            StartCustomServicesRegistration(services);

            var builder = new ContainerBuilder();

			if (appEnv != null)
	        {
				builder.RegisterInstance(appEnv).As<IApplicationEnvironment>();
			}

			builder.Populate(services);
            builder.RegisterInstance(configuration).As<IConfiguration>();
            builder.RegisterType<LoggerProviderExtended>()
                .As<ILoggerProviderExtended>()
                .As<ILoggerProvider>()
                .SingleInstance();
            builder.Register(cc => cc.Resolve<ILoggerProviderExtended>().Factory).As<ILoggerFactory>();

            //TODO: omit ILogger override in config parameter
            builder.Register((cc, pp) => cc.Resolve<ILoggerProviderExtended>().CreateLogger("Root")).As<ILogger>();
            builder.RegisterGeneric(typeof (Logger<>))
                .WithParameter((pi, cc) => pi.ParameterType == typeof (ILoggerFactory),
                    (pi, cc) => cc.Resolve<ILoggerProviderExtended>().Factory)
                .As(typeof (ILogger<>));

            builder.RegisterType<LocalizationService>()
                .As<ILocalizationService>()
                .WithParameters(new List<Parameter>
                {
                    new NamedParameter("defaultCultureId", configuration.GetSection("App:DefaultCultureId").Value)
                })
                .SingleInstance();
            var container = BuildContainer(projectAssembly, builder);
            AutofacExecutionContext.Configure(container);

            UnitOfWorkBase.SetOptions(container.Resolve<IOptions<AppOptionsBase>>());
            LoggerService.Build(container.Resolve<IOptions<AppOptions>>(), container.Resolve<IApplicationEnvironment>());
            EcommerceContextBase.ServiceProvider = container.Resolve<IServiceProvider>();
            return container;
        }

        private void ConfigureMvcOptions(MvcOptions o)
        {
            var inputFormatter =
                (JsonInputFormatter)
                    o.InputFormatters.SingleOrDefault(f => f.GetType() == typeof (JsonInputFormatter));
            var outputFormatter =
                (JsonOutputFormatter)
                    o.OutputFormatters.SingleOrDefault(f => f.GetType() == typeof (JsonOutputFormatter));

            if (inputFormatter != null)
            {
                inputFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                inputFormatter.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                inputFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                inputFormatter.SerializerSettings.Converters.Add(new PstLocalIsoDateTimeConverter());
            }
            else
            {
                var newFormatter = new JsonInputFormatter
                {
                    SerializerSettings =
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DateParseHandling = DateParseHandling.DateTime,
                        DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
                    }
                };
                newFormatter.SerializerSettings.Converters.Add(new PstLocalIsoDateTimeConverter());
                o.InputFormatters.Add(newFormatter);
            }

            if (outputFormatter != null)
            {
                outputFormatter.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                outputFormatter.SerializerSettings.DateParseHandling = DateParseHandling.DateTime;
                outputFormatter.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Unspecified;
                outputFormatter.SerializerSettings.Converters.Add(new PstLocalIsoDateTimeConverter());
            }
            else
            {
                var newFormatter = new JsonOutputFormatter
                {
                    SerializerSettings =
                    {
                        DateFormatHandling = DateFormatHandling.IsoDateFormat,
                        DateParseHandling = DateParseHandling.DateTime,
                        DateTimeZoneHandling = DateTimeZoneHandling.Unspecified
                    }
                };
                newFormatter.SerializerSettings.Converters.Add(new PstLocalIsoDateTimeConverter());
                o.OutputFormatters.Add(newFormatter);
            }
        }

        private static void ConfigureBaseOptions(IConfiguration configuration, AppOptionsBase options)
        {
            options.LogPath = configuration.GetSection("App:LogPath").Value;
            options.LogLevel = configuration.GetSection("App:LogLevel").Value;
            options.Connection = new Connection
            {
                UserName = configuration.GetSection("App:Connection:UserName").Value,
                Password = configuration.GetSection("App:Connection:Password").Value,
                Server = configuration.GetSection("App:Connection:Server").Value,
            };
            options.CacheSettings = new CacheSettings
            {
                CacheTimeToLeaveSeconds = Convert.ToInt32(configuration.GetSection("App:CacheSettings:CacheTimeToLeaveSeconds").Value),
                CacheScanPeriodSeconds = Convert.ToInt32(configuration.GetSection("App:CacheSettings:CacheScanPeriodSeconds").Value),
                MaxProcessHeapsSizeBytes = Convert.ToInt64(configuration.GetSection("App:CacheSettings:MaxProcessHeapsSizeBytes").Value),
            };
        }

		protected virtual void ConfigureAppOptions(IConfiguration configuration, AppOptions options)
		{
			ConfigureBaseOptions(configuration, options);
			options.GenerateLowercaseUrls =
				Convert.ToBoolean(configuration.GetSection("App:GenerateLowercaseUrls").Value);
			options.EnableBundlingAndMinification =
				Convert.ToBoolean(configuration.GetSection("App:EnableBundlingAndMinification").Value);
			options.Versioning = new Versioning()
			{
				EnableStaticContentVersioning =
					Convert.ToBoolean(configuration.GetSection("App:Versioning:EnableStaticContentVersioning").Value),
				BuildNumber =
					Convert.ToBoolean(configuration.GetSection("App:Versioning:AutoGenerateBuildNumber").Value)
						? Guid.NewGuid().ToString("N")
						: configuration.GetSection("App:Versioning:BuildNumber").Value
			};
			options.DefaultCacheExpirationTermMinutes =
				Convert.ToInt32(configuration.GetSection("App:DefaultCacheExpirationTermMinutes").Value);
			options.ActivationTokenExpirationTermDays =
				Convert.ToInt32(configuration.GetSection("App:ActivationTokenExpirationTermDays").Value);
			options.DefaultCultureId = configuration.GetSection("App:DefaultCultureId").Value;
			options.PublicHost = configuration.GetSection("App:PublicHost").Value;
			options.AdminHost = configuration.GetSection("App:AdminHost").Value;
			options.MainSuperAdminEmail = configuration.GetSection("App:MainSuperAdminEmail").Value;
			options.CustomerServiceToEmail = configuration.GetSection("App:CustomerServiceToEmail").Value;
			options.CustomerFeedbackToEmail = configuration.GetSection("App:CustomerFeedbackToEmail").Value;
			options.FilesRelativePath = configuration.GetSection("App:FilesRelativePath").Value;
			options.FilesPath = configuration.GetSection("App:FilesPath").Value;
			options.EmailConfiguration = new Email
			{
				From = configuration.GetSection("App:Email:From").Value,
				Host = configuration.GetSection("App:Email:Host").Value,
				Port = Convert.ToInt32(configuration.GetSection("App:Email:Port").Value),
				Secured = Convert.ToBoolean(configuration.GetSection("App:Email:Secured").Value),
				Username = configuration.GetSection("App:Email:Username").Value,
				Password = configuration.GetSection("App:Email:Password").Value,
				Disabled = Convert.ToBoolean(configuration.GetSection("App:Email:Disabled").Value)
			};
			options.ExportService = new ExportService
			{
				PlainConnectionString = configuration.GetSection("App:ExportService:PlainConnectionString").Value,
				EncryptedConnectionString = configuration.GetSection("App:ExportService:EncryptedConnectionString").Value,
				EncryptedQueueName = configuration.GetSection("App:ExportService:EncryptedQueueName").Value,
				PlainQueueName = configuration.GetSection("App:ExportService:PlainQueueName").Value,
				CertThumbprint = configuration.GetSection("App:ExportService:CertThumbprint").Value,
				RootThumbprint = configuration.GetSection("App:ExportService:RootThumbprint").Value,
				EncryptionHostSessionExpire =
					Convert.ToBoolean(
						configuration.GetSection("App:ExportService:EncryptionHostSessionExpire").Value),
				ServerHostName = configuration.GetSection("App:ExportService:ServerHostName").Value
			};
			options.AzureStorage = new AzureStorage()
			{
				StorageConnectionString = configuration.GetSection("App:AzureStorage:StorageConnectionString").Value,
				CustomerContainerName = configuration.GetSection("App:AzureStorage:CustomerContainerName").Value,
				BugTicketFilesContainerName =
					configuration.GetSection("App:AzureStorage:BugTicketFilesContainerName").Value,
				BugTicketCommentFilesContainerName =
					configuration.GetSection("App:AzureStorage:BugTicketCommentFilesContainerName").Value,
			};
			options.FedExOptions = new FedExOptions()
			{
				AccountNumber = configuration.GetSection("App:FedExOptions:AccountNumber").Value,
				MeterNumber = configuration.GetSection("App:FedExOptions:MeterNumber").Value,
				MerchantPhoneNumber = configuration.GetSection("App:FedExOptions:MerchantPhoneNumber").Value,
				Key = configuration.GetSection("App:FedExOptions:Key").Value,
				Password = configuration.GetSection("App:FedExOptions:Password").Value,
				PayAccountNumber = configuration.GetSection("App:FedExOptions:PayAccountNumber").Value,
				ShipServiceUrl = configuration.GetSection("App:FedExOptions:ShipServiceUrl").Value,
				LocatorServiceUrl = configuration.GetSection("App:FedExOptions:LocatorServiceUrl").Value,
			};
			options.Avatax = new AvataxOptions
			{
				AccountNumber = configuration.GetSection("App:Avatax:AccountNumber").Value,
				CompanyCode = configuration.GetSection("App:Avatax:CompanyCode").Value,
				AccountName = configuration.GetSection("App:Avatax:AccountName").Value,
				LicenseKey = configuration.GetSection("App:Avatax:LicenseKey").Value,
				ProfileName = configuration.GetSection("App:Avatax:ProfileName").Value,
				ServiceUrl = configuration.GetSection("App:Avatax:ServiceUrl").Value,
				TurnOffCommit = Convert.ToBoolean(configuration.GetSection("App:Avatax:TurnOffCommit").Value)
			};
			options.GoogleCaptcha = new GoogleCaptcha
			{
				PublicKey = configuration.GetSection("App:GoogleCaptcha:PublicKey").Value,
				SecretKey = configuration.GetSection("App:GoogleCaptcha:SecretKey").Value,
				VerifyUrl = configuration.GetSection("App:GoogleCaptcha:VerifyUrl").Value
			};
			options.AuthorizeNet = new AuthorizeNet
			{
				ApiKey = configuration.GetSection("App:AuthorizeNet:ApiKey").Value,
				ApiLogin = configuration.GetSection("App:AuthorizeNet:ApiLogin").Value,
				TestEnv = Convert.ToBoolean(configuration.GetSection("App:AuthorizeNet:TestEnv").Value)
			};
			var section = configuration.GetSection("App:CacheSyncOptions");
			options.CacheSyncOptions = new CacheSyncOptions
			{
				ConnectionString = section["ConnectionString"],
				ServiceBusQueueName = section["ServiceBusQueueName"],
				Enabled = Convert.ToBoolean(section["Enabled"])
			};
			section = configuration.GetSection("App:Bronto");
			options.Bronto = new BrontoSettings
			{
				ApiKey = section["ApiKey"],
				ApiUrl = section["ApiUrl"],
				PublicFormUrl = section["PublicFormUrl"],
				PublicFormSendData = section["PublicFormSendData"],
				PublicFormSubscribeData = section["PublicFormSubscribeData"],
			};
		}

		public IContainer BuildContainer(Assembly projectAssembly, ContainerBuilder builder)
        {
            builder.RegisterType<VitalChoiceContext>()
                .As<IDataContextAsync>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<EcommerceContext>()
                .InstancePerLifetimeScope();
            builder.RegisterType<LogsContext>().InstancePerLifetimeScope();
	        builder.RegisterGeneric(typeof (RepositoryAsync<>))
                .As(typeof (IRepositoryAsync<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof (ReadRepositoryAsync<>))
                .As(typeof (IReadRepositoryAsync<>)).InstancePerLifetimeScope(); 
            builder.RegisterGeneric(typeof (EcommerceRepositoryAsync<>))
                .As(typeof (IEcommerceRepositoryAsync<>))
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof (LogsRepositoryAsync<>))
                .As(typeof (ILogsRepositoryAsync<>))
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<LogsContext>()).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof (GenericService<>))
                .As(typeof (IGenericService<>)).InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(CsvExportService<,>))
                .As(typeof(ICsvExportService<,>)).SingleInstance();
            builder.RegisterType<ContentEditService>().As<IContentEditService>().InstancePerLifetimeScope();
            builder.RegisterType<LogViewService>().As<ILogViewService>().InstancePerLifetimeScope();
            builder.RegisterType<MasterContentService>().As<IMasterContentService>().InstancePerLifetimeScope();
            builder.RegisterType<GeneralContentService>().As<IGeneralContentService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryService>().As<ICategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<RecipeService>().As<IRecipeService>().InstancePerLifetimeScope();
            builder.RegisterType<FAQService>().As<IFAQService>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleService>().As<IArticleService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentPageService>().As<IContentPageService>().InstancePerLifetimeScope();
            builder.RegisterType<TtlGlobalCache>().As<ITtlGlobalCache>().SingleInstance();
            builder.RegisterType<CustomUrlHelper>().As<IUrlHelper>().InstancePerLifetimeScope();
            builder.RegisterType<MemoryCache>().As<IMemoryCache>().SingleInstance();
            builder.RegisterType<CacheProvider>().As<ICacheProvider>().SingleInstance();
            builder.RegisterType<AppInfrastructureService>().As<IAppInfrastructureService>().InstancePerLifetimeScope();
	        builder.RegisterType<AdminUserService>().As<IAdminUserService>().InstancePerLifetimeScope();

	        builder.RegisterType<StorefrontUserStore>().Named<IUserStore<ApplicationUser>>("storefronUserStore").InstancePerLifetimeScope();
	        builder.RegisterType<StorefrontUserValidator>().Named<IUserValidator<ApplicationUser>>("storefrontUserValidator").InstancePerLifetimeScope();
	        builder.RegisterType<ExtendedUserManager>()
		        .Named<ExtendedUserManager>("storefrontUserManager")
		        .WithParameter((pi, cc) => pi.Name == "store",
			        (pi, cc) => cc.ResolveNamed<IUserStore<ApplicationUser>>("storefronUserStore")).InstancePerLifetimeScope();
	        builder.RegisterType<SignInManager<ApplicationUser>>()
		        .Named<SignInManager<ApplicationUser>>("storefrontSignInManager")
		        .WithParameter((pi, cc) => pi.Name == "userManager",
			        (pi, cc) => cc.ResolveNamed<ExtendedUserManager>("storefrontUserManager")).InstancePerLifetimeScope();
	        builder.RegisterType<StorefrontUserService>()
		        .As<IStorefrontUserService>()
		        .WithParameter((pi, cc) => pi.Name == "userManager",
			        (pi, cc) => cc.ResolveNamed<ExtendedUserManager>("storefrontUserManager"))
		        .WithParameter((pi, cc) => pi.Name == "userValidator",
			        (pi, cc) => cc.ResolveNamed<IUserValidator<ApplicationUser>>("storefrontUserValidator"))
		        .WithParameter((pi, cc) => pi.Name == "signInManager",
			        (pi, cc) => cc.ResolveNamed<SignInManager<ApplicationUser>>("storefrontSignInManager")).InstancePerLifetimeScope();

            builder.RegisterType<AffiliateUserStore>().Named<IUserStore<ApplicationUser>>("affiliateUserStore").InstancePerLifetimeScope();
            builder.RegisterType<UserValidator<ApplicationUser>>().Named<IUserValidator<ApplicationUser>>("affiliateUserValidator").InstancePerLifetimeScope();
            builder.RegisterType<ExtendedUserManager>()
                .Named<ExtendedUserManager>("affiliateUserManager")
                .WithParameter((pi, cc) => pi.Name == "store",
                    (pi, cc) => cc.ResolveNamed<IUserStore<ApplicationUser>>("affiliateUserStore")).InstancePerLifetimeScope();
            builder.RegisterType<SignInManager<ApplicationUser>>()
                .Named<SignInManager<ApplicationUser>>("affiliateSignInManager")
                .WithParameter((pi, cc) => pi.Name == "userManager",
                    (pi, cc) => cc.ResolveNamed<ExtendedUserManager>("affiliateUserManager")).InstancePerLifetimeScope();
            builder.RegisterType<AffiliateUserService>()
                .As<IAffiliateUserService>()
                .WithParameter((pi, cc) => pi.Name == "userManager",
                    (pi, cc) => cc.ResolveNamed<ExtendedUserManager>("affiliateUserManager"))
                .WithParameter((pi, cc) => pi.Name == "userValidator",
                    (pi, cc) => cc.ResolveNamed<IUserValidator<ApplicationUser>>("affiliateUserValidator"))
                .WithParameter((pi, cc) => pi.Name == "signInManager",
                    (pi, cc) => cc.ResolveNamed<SignInManager<ApplicationUser>>("affiliateSignInManager")).InstancePerLifetimeScope();
            builder.RegisterType<CategoryViewService>().As<ICategoryViewService>().InstancePerLifetimeScope(); 
			builder.RegisterType<ProductViewService>().As<IProductViewService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentPageViewService>().As<IContentPageViewService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductCategoryService>().As<IProductCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<InventoryCategoryService>().As<IInventoryCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductReviewService>().As<IProductReviewService>().InstancePerLifetimeScope();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerLifetimeScope();
            builder.RegisterType<SettingService>().As<ISettingService>().InstancePerLifetimeScope();
            builder.RegisterType<FileService>().As<IFileService>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleCategoryViewService>().As<IArticleCategoryViewService>().InstancePerLifetimeScope();
            builder.RegisterType<ArticleViewService>().As<IArticleViewService>().InstancePerLifetimeScope();
            builder.RegisterType<RecipeCategoryViewService>().As<IRecipeCategoryViewService>().InstancePerLifetimeScope();
            builder.RegisterType<RecipeViewService>().As<IRecipeViewService>().InstancePerLifetimeScope();
            builder.RegisterType<FAQCategoryViewService>().As<IFAQCategoryViewService>().InstancePerLifetimeScope();
            builder.RegisterType<FAQViewService>().As<IFAQViewService>().InstancePerLifetimeScope();

            builder.RegisterType<EmailSender>()
                .As<IEmailSender>()
                .WithParameter((pi, cc) => pi.Name == "options", (pi, cc) => cc.Resolve<IOptions<AppOptions>>())
                .InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerLifetimeScope();
            builder.RegisterType<GCService>().As<IGcService>().InstancePerLifetimeScope();
            builder.RegisterType<ProductService>().As<IProductService>().InstancePerLifetimeScope();
            builder.RegisterType<DiscountService>().As<IDiscountService>().InstancePerLifetimeScope();
            builder.RegisterType<CountryService>().As<ICountryService>().InstancePerLifetimeScope();
            builder.RegisterType<ActionItemProvider>().As<IActionItemProvider>().InstancePerLifetimeScope();
            builder.RegisterType<WorkflowFactory>().As<IWorkflowFactory>().InstancePerLifetimeScope();
            builder.RegisterType<VProductSkuRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
            builder.RegisterType<OrderSkusRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
            builder.RegisterType<AffiliateOrderPaymentRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
            builder.RegisterType<AddressOptionValueRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
            builder.RegisterType<CustomerRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
            builder.RegisterType<OrderRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
            builder.RegisterType<SPEcommerceRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
            builder.RegisterType<PaymentMethodService>().As<IPaymentMethodService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderNoteService>().As<IOrderNoteService>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerService>().As<ICustomerService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderService>().As<IOrderService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<OrderRefundService>().As<IOrderRefundService>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<AffiliateService>().As<IAffiliateService>().InstancePerLifetimeScope();
            builder.RegisterType<HelpService>().As<IHelpService>().InstancePerLifetimeScope();
            builder.RegisterType<BlobStorageClient>().As<IBlobStorageClient>().InstancePerLifetimeScope();
            builder.RegisterType<PromotionService>().As<IPromotionService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentAreaService>().As<IContentAreaService>().InstancePerLifetimeScope();
            builder.RegisterType<FedExService>().As<IFedExService>().InstancePerLifetimeScope();
            builder.RegisterType<VitalGreenService>().As<IVitalGreenService>().InstancePerLifetimeScope();
            builder.RegisterType<StylesService>().As<IStylesService>().InstancePerLifetimeScope();
            builder.RegisterType<CatalogRequestAddressService>().As<ICatalogRequestAddressService>().InstancePerLifetimeScope();
            builder.RegisterType<HealthwiseService>().As<IHealthwiseService>().InstancePerLifetimeScope();
            builder.RegisterType<RedirectService>().As<IRedirectService>().InstancePerLifetimeScope();
			builder.RegisterType<EmailTemplateService>().As<IEmailTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<CheckoutService>().As<ICheckoutService>().InstancePerLifetimeScope();
            builder.RegisterType<TrackingService>().As<ITrackingService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderSchedulerService>().As<IOrderSchedulerService>().InstancePerLifetimeScope();
            builder.RegisterType<TokenService>().As<ITokenService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentCrossSellService>().As<IContentCrossSellService>().InstancePerLifetimeScope();
            builder.RegisterType<InventorySkuCategoryService>().As<IInventorySkuCategoryService>().InstancePerLifetimeScope();
            builder.RegisterType<InventorySkuService>().As<IInventorySkuService>().InstancePerLifetimeScope();
            builder.RegisterType<BrontoService>().As<BrontoService>().InstancePerLifetimeScope();
            builder.RegisterType<ServiceCodeService>().As<IServiceCodeService>().InstancePerLifetimeScope();
            builder.RegisterMappers(typeof(ProductService).GetTypeInfo().Assembly, (type, registration) =>
            {
                if (type == typeof(SkuMapper))
                {
                    return registration.OnActivated(a => ((SkuMapper)a.Instance).ProductMapper = a.Context.Resolve<ProductMapper>());
                }
                return registration;
            });
            builder.RegisterModelConverters(projectAssembly);
            builder.RegisterModelConverters(typeof(OrderService).GetTypeInfo().Assembly);

            builder.RegisterGeneric(typeof(ExtendedEcommerceDynamicService<,,,>))
                .As(typeof(IExtendedDynamicServiceAsync<,,,>)).InstancePerLifetimeScope();

            builder.RegisterGenericServiceDecorator(typeof (EcommerceDynamicServiceDecorator<,>), "extendedService")
                .As(typeof (IDynamicServiceAsync<,>)).InstancePerLifetimeScope();

            builder.RegisterGenericServiceDecorator(typeof(EcommerceDynamicReadServiceDecorator<,>), "extendedService")
                .As(typeof(IDynamicReadServiceAsync<,>)).InstancePerLifetimeScope();

            builder.RegisterGenericServiceDecorator(typeof (ExtendedEcommerceDynamicReadServiceDecorator<,>), "extendedService")
                .As(typeof (IExtendedDynamicReadServiceAsync<,>)).InstancePerLifetimeScope();

            builder.RegisterGeneric(typeof (TreeSetup<,>)).As(typeof (ITreeSetup<,>)).InstancePerLifetimeScope();
            builder.RegisterContentBase();
            builder.RegisterDynamicsBase();
            builder.RegisterType<DynamicExtensionsRewriter>()
                .AsSelf()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
            builder.RegisterProcessors(typeof (ProductCategoryProcessor).GetTypeInfo().Assembly);
            builder.RegisterType<TaxService>().As<ITaxService>().InstancePerLifetimeScope();
            builder.RegisterType<AddressService>().As<IAddressService>().InstancePerLifetimeScope();
            builder.RegisterType<AvalaraTax>().As<IAvalaraTax>().InstancePerLifetimeScope();
            builder.RegisterType<BackendSettingsService>().As<IBackendSettingsService>().InstancePerLifetimeScope();
            builder.RegisterType<ObjectHistoryLogService>().As<IObjectHistoryLogService>().InstancePerLifetimeScope();
            builder.RegisterType<ObjectLogItemExternalService>().As<IObjectLogItemExternalService>().InstancePerLifetimeScope();
            builder.RegisterType<ReCaptchaValidator>().AsSelf().SingleInstance();
            builder.RegisterType<CountryNameCodeResolver>().As<ICountryNameCodeResolver>()
                .InstancePerLifetimeScope();
#if NET451
            builder.RegisterType<EncryptedServiceBusHostClient>().As<IEncryptedServiceBusHostClient>().SingleInstance();
#endif
            builder.RegisterType<ObjectEncryptionHost>()
                .As<IObjectEncryptionHost>()
                .WithParameter((pi, cc) => pi.ParameterType == typeof (ILogger),
                    (pi, cc) => cc.Resolve<ILoggerProviderExtended>().CreateLogger(typeof (ObjectEncryptionHost)))
                .SingleInstance();
            builder.RegisterType<EncryptedOrderExportService>().As<IEncryptedOrderExportService>().InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof (TransactionAccessor<>)).As(typeof (ITransactionAccessor<>)).InstancePerLifetimeScope();
			builder.RegisterType<ExtendedClaimsPrincipalFactory>().As<IUserClaimsPrincipalFactory<ApplicationUser>>().InstancePerLifetimeScope();
            builder.RegisterType<PageResultService>().As<IPageResultService>().SingleInstance();

            FinishCustomRegistrations(builder);

            var container = builder.Build();
            return container;
        }

        protected virtual void StartCustomServicesRegistration(IServiceCollection services)
        {

        }

		protected virtual void FinishCustomRegistrations(ContainerBuilder builder)
	    {
	    }

        protected virtual void AddMvc(IServiceCollection services)
        {
            services.AddMvc();
        }

	    protected virtual void PopulateCookieIdentityOptions(CookieAuthenticationOptions options)
	    {
			options.AuthenticationScheme = IdentityCookieOptions.ApplicationCookieAuthenticationType;
		}
    }
}