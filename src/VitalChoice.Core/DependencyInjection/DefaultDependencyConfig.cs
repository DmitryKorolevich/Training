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
using VitalChoice.Infrastructure.UnitOfWork;
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
using VitalChoice.ContentProcessing.Base;
using Microsoft.Extensions.PlatformAbstractions;
using VitalChoice.Workflow.Base;
using VitalChoice.ContentProcessing.Helpers;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.DynamicData.Extensions;
using Autofac.Extensions.DependencyInjection;
using VitalChoice.Business.Repositories;
using VitalChoice.Core.Infrastructure.Helpers.ReCaptcha;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Context;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Ecommerce.UnitOfWork;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Business.Services.Healthwise;
using VitalChoice.Interfaces.Services.Healthwise;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Infrastructure.ServiceBus;

namespace VitalChoice.Core.DependencyInjection
{
    public abstract class DefaultDependencyConfig : IDependencyConfig
    {
        //private static bool _called = false;

        public IServiceProvider RegisterInfrastructure(IConfiguration configuration, IServiceCollection services, Assembly projectAssembly)
        {
            //if (!_called)
            //{
            //    _called = true;

            // Add EF services to the services container.
            services.AddEntityFramework() //.AddMigrations()
                .AddSqlServer();
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
            StartCustomServicesRegistration(services);
            AddMvc(services);

            services.AddAuthorization(x => x.AddPolicy(IdentityConstants.IdentityBasicProfile, y => y.RequireAuthenticatedUser()));

            services.Configure<AppOptionsBase>(options =>
            {
                options.LogPath = configuration.GetSection("App:LogPath").Value;
                options.LogLevel = configuration.GetSection("App:LogLevel").Value;
                options.Connection = new Connection
                {
                    UserName = configuration.GetSection("App:Connection:UserName").Value,
                    Password = configuration.GetSection("App:Connection:Password").Value,
                    Server = configuration.GetSection("App:Connection:Server").Value,
                };
            });

            services.Configure<AppOptions>(options =>
            {
                options.GenerateLowercaseUrls = Convert.ToBoolean(configuration.GetSection("App:GenerateLowercaseUrls").Value);
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
                options.LogPath = configuration.GetSection("App:LogPath").Value;
                options.LogLevel = configuration.GetSection("App:LogLevel").Value;
                options.DefaultCacheExpirationTermMinutes =
                    Convert.ToInt32(configuration.GetSection("App:DefaultCacheExpirationTermMinutes").Value);
                options.ActivationTokenExpirationTermDays =
                    Convert.ToInt32(configuration.GetSection("App:ActivationTokenExpirationTermDays").Value);
                options.DefaultCultureId = configuration.GetSection("App:DefaultCultureId").Value;
                options.Connection = new Connection
                {
                    UserName = configuration.GetSection("App:Connection:UserName").Value,
                    Password = configuration.GetSection("App:Connection:Password").Value,
                    Server = configuration.GetSection("App:Connection:Server").Value,
                };
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
		            Password = configuration.GetSection("App:Email:Password").Value
	            };
                options.ExportService = new ExportService
                {
                    ConnectionString = configuration.GetSection("App:ExportService:ConnectionString").Value,
                    EncryptedQueueName = configuration.GetSection("App:ExportService:EncryptedQueueName").Value,
                    PlainQueueName = configuration.GetSection("App:ExportService:PlainQueueName").Value,
                    CertThumbprint = configuration.GetSection("App:ExportService:CertThumbprint").Value,
                    RootThumbprint = configuration.GetSection("App:ExportService:RootThumbprint").Value,
                    EncryptionHostSessionExpire = Convert.ToBoolean(configuration.GetSection("App:ExportService:EncryptionHostSessionExpire").Value),
                    ServerHostName = configuration.GetSection("App:ExportService:ServerHostName").Value
                };
                options.AzureStorage = new AzureStorage()
				{
					StorageConnectionString = configuration.GetSection("App:AzureStorage:StorageConnectionString").Value,
					CustomerContainerName = configuration.GetSection("App:AzureStorage:CustomerContainerName").Value,
                    BugTicketFilesContainerName = configuration.GetSection("App:AzureStorage:BugTicketFilesContainerName").Value,
                    BugTicketCommentFilesContainerName = configuration.GetSection("App:AzureStorage:BugTicketCommentFilesContainerName").Value,
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
            });

            services.Configure<MvcOptions>(o =>
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
            });


            var builder = new ContainerBuilder();

            builder.Populate(services);
            builder.RegisterInstance(configuration).As<IConfiguration>();
            builder.RegisterType<LoggerProviderExtended>()
                .As<ILoggerProviderExtended>()
                .As<ILoggerProvider>()
                .SingleInstance();
            builder.Register(cc => cc.Resolve<ILoggerProviderExtended>().Factory).As<ILoggerFactory>();

            //TODO: omit ILogger override in config parameter
            //builder.Register((cc, pp) => cc.Resolve<ILoggerProviderExtended>().CreateLogger("Root")).As<ILogger>();
            //builder.RegisterGeneric(typeof(Logger<>))
            //    .WithParameter((pi, cc) => pi.ParameterType == typeof(ILoggerFactory),
            //        (pi, cc) => cc.Resolve<ILoggerProviderExtended>().Factory)
            //    .As(typeof(ILogger<>));

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
            return container.Resolve<IServiceProvider>();
            //}
            //return null;
        }

        public IContainer BuildContainer(Assembly projectAssembly, ContainerBuilder builder)
        {
            builder.RegisterType<VitalChoiceContext>()
                .As<IDataContextAsync>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<EcommerceContext>()
                .InstancePerLifetimeScope();
            builder.RegisterType<LogsContext>();
            builder.RegisterGeneric(typeof (RepositoryAsync<>))
                .As(typeof (IRepositoryAsync<>));
            builder.RegisterGeneric(typeof (ReadRepositoryAsync<>))
                .As(typeof (IReadRepositoryAsync<>));
            builder.RegisterGeneric(typeof (EcommerceRepositoryAsync<>))
                .As(typeof (IEcommerceRepositoryAsync<>))
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterGeneric(typeof (LogsRepositoryAsync<>))
                .As(typeof (ILogsRepositoryAsync<>))
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<LogsContext>());
            builder.RegisterGeneric(typeof (GenericService<>))
                .As(typeof (IGenericService<>));
            builder.RegisterGeneric(typeof(CsvExportService<,>))
                .As(typeof(ICsvExportService<,>));
            builder.RegisterType<ContentEditService>().As<IContentEditService>();
            builder.RegisterType<LogViewService>().As<ILogViewService>();
            builder.RegisterType<MasterContentService>().As<IMasterContentService>();
            builder.RegisterType<GeneralContentService>().As<IGeneralContentService>();
            builder.RegisterType<CategoryService>().As<ICategoryService>();
            builder.RegisterType<RecipeService>().As<IRecipeService>();
            builder.RegisterType<FAQService>().As<IFAQService>();
            builder.RegisterType<ArticleService>().As<IArticleService>();
            builder.RegisterType<ContentPageService>().As<IContentPageService>();
            builder.RegisterType<TtlGlobalCache>().As<ITtlGlobalCache>().SingleInstance();
            builder.RegisterType<CustomUrlHelper>().As<IUrlHelper>();
            builder.RegisterType<MemoryCache>().As<IMemoryCache>();
            builder.RegisterType<CacheProvider>().As<ICacheProvider>().SingleInstance();
            builder.RegisterType<AppInfrastructureService>().As<IAppInfrastructureService>();
	        builder.RegisterType<AdminUserService>().As<IAdminUserService>();

	        builder.RegisterType<StorefrontUserStore>().Named<IUserStore<ApplicationUser>>("storefronUserStore");
	        builder.RegisterType<UserValidator<ApplicationUser>>().Named<IUserValidator<ApplicationUser>>("storefrontUserValidator");
	        builder.RegisterType<ExtendedUserManager>()
		        .Named<ExtendedUserManager>("storefrontUserManager")
		        .WithParameter((pi, cc) => pi.Name == "store",
			        (pi, cc) => cc.ResolveNamed<IUserStore<ApplicationUser>>("storefronUserStore"));
	        builder.RegisterType<SignInManager<ApplicationUser>>()
		        .Named<SignInManager<ApplicationUser>>("storefrontSignInManager")
		        .WithParameter((pi, cc) => pi.Name == "userManager",
			        (pi, cc) => cc.ResolveNamed<ExtendedUserManager>("storefrontUserManager"));
	        builder.RegisterType<StorefrontUserService>()
		        .As<IStorefrontUserService>()
		        .WithParameter((pi, cc) => pi.Name == "userManager",
			        (pi, cc) => cc.ResolveNamed<ExtendedUserManager>("storefrontUserManager"))
		        .WithParameter((pi, cc) => pi.Name == "userValidator",
			        (pi, cc) => cc.ResolveNamed<IUserValidator<ApplicationUser>>("storefrontUserValidator"))
		        .WithParameter((pi, cc) => pi.Name == "signInManager",
			        (pi, cc) => cc.ResolveNamed<SignInManager<ApplicationUser>>("storefrontSignInManager"));

            builder.RegisterType<AffiliateUserStore>().Named<IUserStore<ApplicationUser>>("affiliateUserStore");
            builder.RegisterType<UserValidator<ApplicationUser>>().Named<IUserValidator<ApplicationUser>>("affiliateUserValidator");
            builder.RegisterType<ExtendedUserManager>()
                .Named<ExtendedUserManager>("affiliateUserManager")
                .WithParameter((pi, cc) => pi.Name == "store",
                    (pi, cc) => cc.ResolveNamed<IUserStore<ApplicationUser>>("affiliateUserStore"));
            builder.RegisterType<SignInManager<ApplicationUser>>()
                .Named<SignInManager<ApplicationUser>>("affiliateSignInManager")
                .WithParameter((pi, cc) => pi.Name == "userManager",
                    (pi, cc) => cc.ResolveNamed<ExtendedUserManager>("affiliateUserManager"));
            builder.RegisterType<AffiliateUserService>()
                .As<IAffiliateUserService>()
                .WithParameter((pi, cc) => pi.Name == "userManager",
                    (pi, cc) => cc.ResolveNamed<ExtendedUserManager>("affiliateUserManager"))
                .WithParameter((pi, cc) => pi.Name == "userValidator",
                    (pi, cc) => cc.ResolveNamed<IUserValidator<ApplicationUser>>("affiliateUserValidator"))
                .WithParameter((pi, cc) => pi.Name == "signInManager",
                    (pi, cc) => cc.ResolveNamed<SignInManager<ApplicationUser>>("affiliateSignInManager"));
            builder.RegisterType<CategoryViewService>().As<ICategoryViewService>(); 
			builder.RegisterType<ProductViewService>().As<IProductViewService>();
            builder.RegisterType<ContentPageViewService>().As<IContentPageViewService>();
            builder.RegisterType<ProductCategoryService>().As<IProductCategoryService>();
            builder.RegisterType<InventoryCategoryService>().As<IInventoryCategoryService>();
            builder.RegisterType<ProductReviewService>().As<IProductReviewService>();
            builder.RegisterType<CountryService>().As<ICountryService>();
            builder.RegisterType<SettingService>().As<ISettingService>();
            builder.RegisterType<FileService>().As<IFileService>();
            builder.RegisterType<ArticleCategoryViewService>().As<IArticleCategoryViewService>();
            builder.RegisterType<ArticleViewService>().As<IArticleViewService>();
            builder.RegisterType<RecipeCategoryViewService>().As<IRecipeCategoryViewService>();
            builder.RegisterType<RecipeViewService>().As<IRecipeViewService>();
            builder.RegisterType<FAQCategoryViewService>().As<IFAQCategoryViewService>();
            builder.RegisterType<FAQViewService>().As<IFAQViewService>();

            builder.RegisterType<EmailSender>()
                .As<IEmailSender>()
                .WithParameter((pi, cc) => pi.Name == "options", (pi, cc) => cc.Resolve<IOptions<AppOptions>>())
                .InstancePerLifetimeScope();
            builder.RegisterType<NotificationService>().As<INotificationService>();
            builder.RegisterType<GCService>().As<IGcService>();
            builder.RegisterType<ProductService>().As<IProductService>();
            builder.RegisterType<DiscountService>().As<IDiscountService>();
            builder.RegisterType<CountryService>().As<ICountryService>();
            builder.RegisterType<ActionItemProvider>().As<IActionItemProvider>();
            builder.RegisterType<WorkflowFactory>().As<IWorkflowFactory>();
            builder.RegisterType<VProductSkuRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterType<OrderSkusRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterType<AffiliateOrderPaymentRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterType<AddressOptionValueRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterType<CustomerRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterType<OrderRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterType<SPEcommerceRepository>()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterType<PaymentMethodService>().As<IPaymentMethodService>();
            builder.RegisterType<OrderNoteService>().As<IOrderNoteService>();
            builder.RegisterType<CustomerService>().As<ICustomerService>();
            builder.RegisterType<OrderService>().As<IOrderService>();
            builder.RegisterType<AffiliateService>().As<IAffiliateService>();
            builder.RegisterType<HelpService>().As<IHelpService>();
            builder.RegisterType<BlobStorageClient>().As<IBlobStorageClient>();
            builder.RegisterType<PromotionService>().As<IPromotionService>();
            builder.RegisterType<ContentAreaService>().As<IContentAreaService>();
            builder.RegisterType<FedExService>().As<IFedExService>();
            builder.RegisterType<VitalGreenService>().As<IVitalGreenService>();
            builder.RegisterType<StylesService>().As<IStylesService>();
            builder.RegisterType<CatalogRequestAddressService>().As<ICatalogRequestAddressService>();
            builder.RegisterType<HealthwiseService>().As<IHealthwiseService>();
            builder.RegisterType<RedirectService>().As<IRedirectService>();
            builder.RegisterMappers(typeof (ProductService).GetTypeInfo().Assembly);
            builder.RegisterModelConverters(projectAssembly);

            builder.RegisterGeneric(typeof(ExtendedEcommerceDynamicService<,,,>))
                .As(typeof(IExtendedDynamicServiceAsync<,,,>));

            builder.RegisterGenericServiceDecorator(typeof (EcommerceDynamicServiceDecorator<,>), "extendedService")
                .As(typeof (IDynamicServiceAsync<,>));

            builder.RegisterGenericServiceDecorator(typeof(EcommerceDynamicReadServiceDecorator<,>), "extendedService")
                .As(typeof(IDynamicReadServiceAsync<,>));

            builder.RegisterGenericServiceDecorator(typeof (ExtendedEcommerceDynamicReadServiceDecorator<,>), "extendedService")
                .As(typeof (IExtendedDynamicReadServiceAsync<,>));

            builder.RegisterGeneric(typeof (TreeSetup<,>)).As(typeof (ITreeSetup<,>));
            builder.RegisterContentBase();
            builder.RegisterDynamicsBase();
            builder.RegisterType<DynamicExpressionVisitor>()
                .AsSelf()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterProcessors(typeof (ProductCategoryProcessor).GetTypeInfo().Assembly);
            builder.RegisterType<TaxService>().As<ITaxService>();
            builder.RegisterType<AddressService>().As<IAddressService>();
            builder.RegisterType<AvalaraTax>().As<IAvalaraTax>();
            builder.RegisterType<BackendSettingsService>().As<IBackendSettingsService>();
            builder.RegisterType<ObjectHistoryLogService>().As<IObjectHistoryLogService>();
            builder.RegisterType<ObjectLogItemExternalService>().As<IObjectLogItemExternalService>();
            builder.RegisterType<ReCaptchaValidator>().AsSelf();
            builder.RegisterType<CountryNameCodeResolver>().As<ICountryNameCodeResolver>()
                //little optimization on instance count, since it's self hosted data
                .InstancePerLifetimeScope();
            builder.RegisterType<EncryptedServiceBusHostClient>().As<IEncryptedServiceBusHostClient>().SingleInstance();
            builder.RegisterType<ObjectEncryptionHost>()
                .As<IObjectEncryptionHost>()
                .WithParameter((pi, cc) => pi.ParameterType == typeof (ILogger),
                    (pi, cc) => cc.Resolve<ILoggerProviderExtended>().CreateLogger(typeof (ObjectEncryptionHost)))
                .SingleInstance();
            builder.RegisterType<EncryptedOrderExportService>().As<IEncryptedOrderExportService>();
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
    }
}