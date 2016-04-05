using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Autofac.Core;
using Autofac.Extensions.DependencyInjection;
using Avalara.Avatax.Rest.Services;
using Microsoft.AspNet.Identity;
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
using VitalChoice.Business.Services.Bronto;
using VitalChoice.Business.Services.Checkout;
using VitalChoice.Business.Services.Content;
using VitalChoice.Business.Services.Content.ContentProcessors;
using VitalChoice.Business.Services.Customers;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Business.Services.FedEx;
using VitalChoice.Business.Services.HelpService;
using VitalChoice.Business.Services.InventorySkus;
using VitalChoice.Business.Services.Orders;
using VitalChoice.Business.Services.Payment;
using VitalChoice.Business.Services.Products;
using VitalChoice.Business.Services.Settings;
using VitalChoice.Business.Services.Users;
using VitalChoice.Business.Services.Workflow;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.ContentProcessing.Helpers;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Core.DependencyInjection;
using VitalChoice.Core.Infrastructure.Helpers.ReCaptcha;
using VitalChoice.Core.Services;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Context;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Infrastructure.Identity.UserStores;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Avatax;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Help;
using VitalChoice.Interfaces.Services.InventorySkus;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Jobs.Jobs;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Jobs
{
    public class Configuration
    {
	    private static void RegisterJobModules(ContainerBuilder builder)
	    {
			builder.RegisterModule(new QuartzAutofacFactoryModule());
			builder.RegisterModule(new QuartzAutofacJobsModule(typeof(Configuration).Assembly));
		}

	    private static void RegisterServices(ContainerBuilder builder)
		{
			//builder.RegisterType<VitalChoiceContext>()
			//	.As<IDataContextAsync>()
			//	.AsSelf()
			//	.InstancePerLifetimeScope();
			//builder.RegisterType<EcommerceContext>()
			//	.InstancePerLifetimeScope();
			//builder.RegisterType<LogsContext>();
			//builder.RegisterGeneric(typeof(RepositoryAsync<>))
			//	.As(typeof(IRepositoryAsync<>));
			//builder.RegisterGeneric(typeof(ReadRepositoryAsync<>))
			//	.As(typeof(IReadRepositoryAsync<>));
			//builder.RegisterGeneric(typeof(EcommerceRepositoryAsync<>))
			//	.As(typeof(IEcommerceRepositoryAsync<>))
			//	.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
			//builder.RegisterGeneric(typeof(LogsRepositoryAsync<>))
			//	.As(typeof(ILogsRepositoryAsync<>))
			//	.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<LogsContext>());
			//builder.RegisterGeneric(typeof(GenericService<>))
			//	.As(typeof(IGenericService<>));
			//builder.RegisterType<ContentEditService>().As<IContentEditService>();
			//builder.RegisterType<LogViewService>().As<ILogViewService>();
			//builder.RegisterType<MasterContentService>().As<IMasterContentService>();
			//builder.RegisterType<GeneralContentService>().As<IGeneralContentService>();
			//builder.RegisterType<CategoryService>().As<ICategoryService>();
			//builder.RegisterType<RecipeService>().As<IRecipeService>();
			//builder.RegisterType<FAQService>().As<IFAQService>();
			//builder.RegisterType<ArticleService>().As<IArticleService>();
			//builder.RegisterType<ContentPageService>().As<IContentPageService>();
			//builder.RegisterType<TtlGlobalCache>().As<ITtlGlobalCache>().SingleInstance();
			//builder.RegisterType<MemoryCache>().As<IMemoryCache>();
			//builder.RegisterType<CacheProvider>().As<ICacheProvider>().SingleInstance();
			//builder.RegisterType<AppInfrastructureService>().As<IAppInfrastructureService>();
			//builder.RegisterType<AdminUserService>().As<IAdminUserService>();

			//builder.RegisterType<CategoryViewService>().As<ICategoryViewService>();
			//builder.RegisterType<ProductViewService>().As<IProductViewService>();
			//builder.RegisterType<ContentPageViewService>().As<IContentPageViewService>();
			//builder.RegisterType<ProductCategoryService>().As<IProductCategoryService>();
			//builder.RegisterType<InventoryCategoryService>().As<IInventoryCategoryService>();
			//builder.RegisterType<ProductReviewService>().As<IProductReviewService>();
			//builder.RegisterType<CountryService>().As<ICountryService>();
			//builder.RegisterType<SettingService>().As<ISettingService>();
			//builder.RegisterType<FileService>().As<IFileService>();

			//builder.RegisterType<EmailSender>()
			//	.As<IEmailSender>()
			//	.WithParameter((pi, cc) => pi.Name == "options", (pi, cc) => cc.Resolve<IOptions<AppOptions>>())
			//	.InstancePerLifetimeScope();
			//builder.RegisterType<NotificationService>().As<INotificationService>();
			//builder.RegisterType<GCService>().As<IGcService>();
			//builder.RegisterType<ProductService>().As<IProductService>();
			//builder.RegisterType<DiscountService>().As<IDiscountService>();
			//builder.RegisterType<CountryService>().As<ICountryService>();
			//builder.RegisterType<ActionItemProvider>().As<IActionItemProvider>();
			//builder.RegisterType<WorkflowFactory>().As<IWorkflowFactory>();
			//builder.RegisterType<VProductSkuRepository>()
			//	.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
			//builder.RegisterType<OrderSkusRepository>()
			//	.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
			//builder.RegisterType<AffiliateOrderPaymentRepository>()
			//	.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
			//builder.RegisterType<AddressOptionValueRepository>()
			//	.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
			//builder.RegisterType<CustomerRepository>()
			//	.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
			//builder.RegisterType<OrderRepository>()
			//	.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
			//builder.RegisterType<PaymentMethodService>().As<IPaymentMethodService>();
			//builder.RegisterType<OrderNoteService>().As<IOrderNoteService>();
			//builder.RegisterType<CustomerService>().As<ICustomerService>();
			//builder.RegisterType<OrderService>().As<IOrderService>();
			//builder.RegisterType<AffiliateService>().As<IAffiliateService>();
			//builder.RegisterType<HelpService>().As<IHelpService>();
			//builder.RegisterType<BlobStorageClient>().As<IBlobStorageClient>();
			//builder.RegisterType<PromotionService>().As<IPromotionService>();
			//builder.RegisterType<ContentAreaService>().As<IContentAreaService>();
			//builder.RegisterType<FedExService>().As<IFedExService>();
			//builder.RegisterType<VitalGreenService>().As<IVitalGreenService>();
			//builder.RegisterType<StylesService>().As<IStylesService>();
			//builder.RegisterType<CatalogRequestAddressService>().As<ICatalogRequestAddressService>();

			//builder.RegisterMappers(typeof(ProductService).GetTypeInfo().Assembly, (type, registration) =>
			//{
			//	if (type == typeof(SkuMapper))
			//	{
			//		return registration.OnActivated(a => ((SkuMapper)a.Instance).ProductMapper = a.Context.Resolve<ProductMapper>());
			//	}
			//	return registration;
			//});

			//builder.RegisterGeneric(typeof(ExtendedEcommerceDynamicService<,,,>))
			//	.As(typeof(IExtendedDynamicServiceAsync<,,,>));

			//builder.RegisterGenericServiceDecorator(typeof(EcommerceDynamicServiceDecorator<,>), "extendedService")
			//	.As(typeof(IDynamicServiceAsync<,>));

			//builder.RegisterGenericServiceDecorator(typeof(EcommerceDynamicReadServiceDecorator<,>), "extendedService")
			//	.As(typeof(IDynamicReadServiceAsync<,>));

			//builder.RegisterGenericServiceDecorator(typeof(ExtendedEcommerceDynamicReadServiceDecorator<,>), "extendedService")
			//	.As(typeof(IExtendedDynamicReadServiceAsync<,>));

			//builder.RegisterGeneric(typeof(TreeSetup<,>)).As(typeof(ITreeSetup<,>));
			//builder.RegisterContentBase();
			//builder.RegisterDynamicsBase();
			//builder.RegisterType<DynamicExtensionsRewriter>()
			//	.AsSelf()
			//	.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>());
			//builder.RegisterProcessors(typeof(IContentProcessor).GetTypeInfo().Assembly);
			//builder.RegisterType<TaxService>().As<ITaxService>();
			//builder.RegisterType<AddressService>().As<IAddressService>();
			//builder.RegisterType<AvalaraTax>().As<IAvalaraTax>();
			//builder.RegisterType<BackendSettingsService>().As<IBackendSettingsService>();
			//builder.RegisterType<ObjectHistoryLogService>().As<IObjectHistoryLogService>();
			//builder.RegisterType<ObjectLogItemExternalService>().As<IObjectLogItemExternalService>();
			//builder.RegisterType<ObjectEncryptionHost>().As<IObjectEncryptionHost>().SingleInstance();


			

			builder.RegisterType<VitalChoiceContext>()
				.As<IDataContextAsync>()
				.AsSelf()
				.InstancePerLifetimeScope();
			builder.RegisterType<EcommerceContext>()
				.InstancePerLifetimeScope();
			builder.RegisterType<LogsContext>().InstancePerLifetimeScope();
			builder.RegisterGeneric(typeof(RepositoryAsync<>))
				.As(typeof(IRepositoryAsync<>)).InstancePerLifetimeScope();
			builder.RegisterGeneric(typeof(ReadRepositoryAsync<>))
				.As(typeof(IReadRepositoryAsync<>)).InstancePerLifetimeScope();
			builder.RegisterGeneric(typeof(EcommerceRepositoryAsync<>))
				.As(typeof(IEcommerceRepositoryAsync<>))
				.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
			builder.RegisterGeneric(typeof(LogsRepositoryAsync<>))
				.As(typeof(ILogsRepositoryAsync<>))
				.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<LogsContext>()).InstancePerLifetimeScope();
			builder.RegisterGeneric(typeof(GenericService<>))
				.As(typeof(IGenericService<>)).InstancePerLifetimeScope();
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
			//builder.RegisterType<CustomUrlHelper>().As<IUrlHelper>().InstancePerLifetimeScope();
			builder.RegisterType<MemoryCache>().As<IMemoryCache>().SingleInstance();
			builder.RegisterType<CacheProvider>().As<ICacheProvider>().SingleInstance();
			builder.RegisterType<AppInfrastructureService>().As<IAppInfrastructureService>().InstancePerLifetimeScope();
			builder.RegisterType<AdminUserService>().As<IAdminUserService>().InstancePerLifetimeScope();

			builder.RegisterType<StorefrontUserStore>().Named<IUserStore<ApplicationUser>>("storefronUserStore").InstancePerLifetimeScope();
			//builder.RegisterType<StorefrontUserValidator>().Named<IUserValidator<ApplicationUser>>("storefrontUserValidator").InstancePerLifetimeScope();
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
			//builder.RegisterType<HealthwiseService>().As<IHealthwiseService>().InstancePerLifetimeScope();
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
			//builder.RegisterModelConverters(projectAssembly);
			builder.RegisterModelConverters(typeof(OrderService).GetTypeInfo().Assembly);

			builder.RegisterGeneric(typeof(ExtendedEcommerceDynamicService<,,,>))
				.As(typeof(IExtendedDynamicServiceAsync<,,,>)).InstancePerLifetimeScope();

			builder.RegisterGenericServiceDecorator(typeof(EcommerceDynamicServiceDecorator<,>), "extendedService")
				.As(typeof(IDynamicServiceAsync<,>)).InstancePerLifetimeScope();

			builder.RegisterGenericServiceDecorator(typeof(EcommerceDynamicReadServiceDecorator<,>), "extendedService")
				.As(typeof(IDynamicReadServiceAsync<,>)).InstancePerLifetimeScope();

			builder.RegisterGenericServiceDecorator(typeof(ExtendedEcommerceDynamicReadServiceDecorator<,>), "extendedService")
				.As(typeof(IExtendedDynamicReadServiceAsync<,>)).InstancePerLifetimeScope();

			builder.RegisterGeneric(typeof(TreeSetup<,>)).As(typeof(ITreeSetup<,>)).InstancePerLifetimeScope();
			builder.RegisterContentBase();
			builder.RegisterDynamicsBase();
			builder.RegisterType<DynamicExtensionsRewriter>()
				.AsSelf()
				.WithParameter((pi, cc) => pi.Name == "context", (pi, cc) => cc.Resolve<EcommerceContext>()).InstancePerLifetimeScope();
			builder.RegisterProcessors(typeof(ProductCategoryProcessor).GetTypeInfo().Assembly);
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
				.WithParameter((pi, cc) => pi.ParameterType == typeof(ILogger),
					(pi, cc) => cc.Resolve<ILoggerProviderExtended>().CreateLogger(typeof(ObjectEncryptionHost)))
				.SingleInstance();
			builder.RegisterType<EncryptedOrderExportService>().As<IEncryptedOrderExportService>().InstancePerLifetimeScope();
			builder.RegisterGeneric(typeof(TransactionAccessor<>)).As(typeof(ITransactionAccessor<>)).InstancePerLifetimeScope();
			builder.RegisterType<ExtendedClaimsPrincipalFactory>().As<IUserClaimsPrincipalFactory<ApplicationUser>>().InstancePerLifetimeScope();
			builder.RegisterType<PageResultService>().As<IPageResultService>().SingleInstance();
		}

		public static IContainer BuildContainer()
        {
   //        var configurationBuilder = new ConfigurationBuilder()
   //             .AddJsonFile("config.json")
   //             .AddJsonFile("config.local.json", true);

   //         var configuration = configurationBuilder.Build();

   //         var services = new ServiceCollection();

   //         services.AddEntityFramework()
   //             .AddSqlServer();

   //         services.Configure<AppOptionsBase>(options =>
   //         {
   //             options.LogPath = configuration.GetSection("App:LogPath").Value;
   //             options.Connection = new Connection
   //             {
   //                 UserName = configuration.GetSection("App:Connection:UserName").Value,
   //                 Password = configuration.GetSection("App:Connection:Password").Value,
   //                 Server = configuration.GetSection("App:Connection:Server").Value,
   //             };
   //         });

   //         services.Configure<JobSettings>(options =>
   //         {
			//	options.LogPath = configuration.GetSection("App:LogPath").Value;
   //             options.DefaultCultureId = configuration.GetSection("App:DefaultCultureId").Value;
   //             options.Connection = new Connection
   //             {
   //                 UserName = configuration.GetSection("App:Connection:UserName").Value,
   //                 Password = configuration.GetSection("App:Connection:Password").Value,
   //                 Server = configuration.GetSection("App:Connection:Server").Value,
   //             };
   //         });


   //        var builder = new ContainerBuilder();
   //         builder.Populate(services);
			//builder.RegisterInstance(configuration).As<IConfiguration>();

			//	builder.RegisterType<LoggerProviderExtended>()
			//   .As<ILoggerProviderExtended>()
			//   .As<ILoggerProvider>()
			//   .SingleInstance();
			//builder.Register(cc => cc.Resolve<ILoggerProviderExtended>().Factory).As<ILoggerFactory>();

			////TODO: omit ILogger override in config parameter
			//builder.Register((cc, pp) => cc.Resolve<ILoggerProviderExtended>().CreateLogger("Root")).As<ILogger>();
			//builder.RegisterGeneric(typeof(Logger<>))
			//	.WithParameter((pi, cc) => pi.ParameterType == typeof(ILoggerFactory),
			//		(pi, cc) => cc.Resolve<ILoggerProviderExtended>().Factory)
			//	.As(typeof(ILogger<>));

			//builder.RegisterType<LocalizationService>()
			//	.As<ILocalizationService>()
			//	.WithParameters(new List<Parameter>
			//	{
			//		new NamedParameter("defaultCultureId", configuration.GetSection("App:DefaultCultureId").Value)
			//	})
			//	.SingleInstance();

			//RegisterServices(builder);

			//RegisterJobModules(builder);

			//var container = builder.Build();

			//AutofacExecutionContext.Configure(container);

			//UnitOfWorkBase.SetOptions(container.Resolve<IOptions<AppOptionsBase>>());
			//LoggerService.Build(container.Resolve<IOptions<AppOptions>>(), container.Resolve<IApplicationEnvironment>());
			//EcommerceContextBase.ServiceProvider = container.Resolve<IServiceProvider>();

			//RegisterServices(builder);

			//DefaultDependencyConfig.

			//RegisterJobModules(builder);

			//container = builder.Build();

			return null;
        }
    }
}