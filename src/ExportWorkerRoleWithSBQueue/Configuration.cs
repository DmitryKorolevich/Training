using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Extensions.DependencyInjection;
using Avalara.Avatax.Rest.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using VitalChoice.Business.Mail;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services;
using VitalChoice.Business.Services.Affiliates;
using VitalChoice.Business.Services.Avatax;
using VitalChoice.Business.Services.Content;
using VitalChoice.Business.Services.Content.ContentProcessors;
using VitalChoice.Business.Services.Customers;
using VitalChoice.Business.Services.FedEx;
using VitalChoice.Business.Services.HelpService;
using VitalChoice.Business.Services.Orders;
using VitalChoice.Business.Services.Payment;
using VitalChoice.Business.Services.Products;
using VitalChoice.Business.Services.Settings;
using VitalChoice.Business.Services.Users;
using VitalChoice.Business.Services.Workflow;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Context;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Infrastructure.Identity.UserStores;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Avatax;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Help;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Workflow.Core;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.ContentProcessing.Helpers;
using VitalChoice.Infrastructure.Ecommerce;

namespace ExportWorkerRoleWithSBQueue
{
    internal class Configuration
    {
        internal static IContainer BuildContainer()
        {
            var applicationEnvironment =
                (IApplicationEnvironment)
                    CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof(IApplicationEnvironment));
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddJsonFile("config.local.json", true);

            var configuration = configurationBuilder.Build();

            var services = new ServiceCollection();

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<VitalChoiceContext>();

            services.Configure<AppOptionsBase>(options =>
            {
                options.LogPath = configuration.GetSection("App:LogPath").Value;
                options.Connection = new Connection
                {
                    UserName = configuration.GetSection("App:Connection:UserName").Value,
                    Password = configuration.GetSection("App:Connection:Password").Value,
                    Server = configuration.GetSection("App:Connection:Server").Value,
                };
            });

            services.Configure<AppOptions>(options =>
            {
                options.LogPath = configuration.GetSection("App:LogPath").Value;
                options.DefaultCultureId = configuration.GetSection("App:DefaultCultureId").Value;
                options.Connection = new Connection
                {
                    UserName = configuration.GetSection("App:Connection:UserName").Value,
                    Password = configuration.GetSection("App:Connection:Password").Value,
                    Server = configuration.GetSection("App:Connection:Server").Value,
                };
            });

            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterInstance(configuration).As<IConfiguration>();
            //builder.RegisterInstance(
            //    LoggerService.Build(applicationEnvironment.ApplicationBasePath,
            //        configuration.GetSection("App:LogPath").Value))
            //    .As<ILoggerProviderExtended>().SingleInstance();

            //builder.Register((cc, pp) => cc.Resolve<ILoggerProviderExtended>().CreateLogger("Root")).As<ILogger>();
            //builder.RegisterGeneric(typeof (Logger<>))
            //    .WithParameter((pi, cc) => pi.ParameterType == typeof (ILoggerFactory),
            //        (pi, cc) => cc.Resolve<ILoggerProviderExtended>().Factory)
            //    .As(typeof (ILogger<>));

            var container = BuildContainer(typeof(WorkerRole).GetTypeInfo().Assembly, builder);
            return container;
        }

        public static IContainer BuildContainer(Assembly projectAssembly, ContainerBuilder builder)
        {
            builder.RegisterType<VitalChoiceContext>()
                .As<IDataContextAsync>()
                .AsSelf()
                .InstancePerLifetimeScope();
            builder.RegisterType<EcommerceContext>()
                .InstancePerLifetimeScope();
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
            builder.RegisterGeneric(typeof(GenericService<>))
                .As(typeof(IGenericService<>));
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
            builder.RegisterMappers(typeof(ProductService).GetTypeInfo().Assembly);
            builder.RegisterModelConverters(projectAssembly);

            builder.RegisterGeneric(typeof(ExtendedEcommerceDynamicService<,,,>))
                .As(typeof(IExtendedDynamicServiceAsync<,,,>));

            builder.RegisterGenericServiceDecorator(typeof(EcommerceDynamicServiceDecorator<,>), "extendedService")
                .As(typeof(IDynamicServiceAsync<,>));

            builder.RegisterGenericServiceDecorator(typeof(EcommerceDynamicReadServiceDecorator<,>), "extendedService")
                .As(typeof(IDynamicReadServiceAsync<,>));

            builder.RegisterGenericServiceDecorator(typeof(ExtendedEcommerceDynamicReadServiceDecorator<,>), "extendedService")
                .As(typeof(IExtendedDynamicReadServiceAsync<,>));

            builder.RegisterGeneric(typeof(TreeSetup<,>)).As(typeof(ITreeSetup<,>));
            builder.RegisterContentBase();
            builder.RegisterDynamicsBase();
            builder.RegisterType<DynamicExpressionVisitor>()
                .AsSelf()
                .WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
            builder.RegisterProcessors(typeof(IContentProcessor).GetTypeInfo().Assembly);
            builder.RegisterProcessors(typeof(ProductCategoryDefaultProcessor).GetTypeInfo().Assembly);
            builder.RegisterType<TaxService>().As<ITaxService>();
            builder.RegisterType<AddressService>().As<IAddressService>();
            builder.RegisterType<AvalaraTax>().As<IAvalaraTax>();
            builder.RegisterType<BackendSettingsService>().As<IBackendSettingsService>();
            builder.RegisterType<ObjectHistoryLogService>().As<IObjectHistoryLogService>();
            builder.RegisterType<ObjectLogItemExternalService>().As<IObjectLogItemExternalService>();

            var container = builder.Build();
            return container;
        }
    }
}