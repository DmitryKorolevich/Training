using System;
using System.Linq;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using LookupHelper = VitalChoice.Business.Helpers.LookupHelper;
using System.Collections.Generic;
using VitalChoice.Interfaces.Services.Settings;
using System.Globalization;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Promotions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Ecommerce.Utils;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Settings;

namespace VitalChoice.Business.Services
{
    public class AppInfrastructureService : IAppInfrastructureService
    {
        private readonly ICacheProvider cache;
        private readonly int expirationTerm;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IRepositoryAsync<ContentProcessorEntity> contentProcessorRepository;
        private readonly IOptions<AppOptions> appOptionsAccessor;
        private readonly IEcommerceRepositoryAsync<CustomerTypeEntity> customerTypeRepository;
        private readonly IEcommerceRepositoryAsync<OrderStatusEntity> orderStatusRepository;
        private readonly IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepository;
        private readonly IEcommerceRepositoryAsync<PromotionTypeEntity> promotionTypeEntityRepository;
        private readonly IEcommerceRepositoryAsync<LookupVariant> _lookupVariantRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> _lookupRepository;
        private readonly IEcommerceRepositoryAsync<AppOption> appOptionRepository;
        private readonly IEcommerceRepositoryAsync<OrderTypeEntity> orderTypeEntityRepository;
        private readonly ISettingService settingService;
        private readonly IBackendSettingsService _backendSettingsService;
        private readonly ILocalizationService _localizationService;

        public AppInfrastructureService(
            ICacheProvider cache, 
            IOptions<AppOptions> appOptions, 
            RoleManager<ApplicationRole> roleManager,
            IRepositoryAsync<ContentProcessorEntity> contentProcessorRepository, 
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            IOptions<AppOptions> appOptionsAccessor,
            IEcommerceRepositoryAsync<CustomerTypeEntity> customerTypeRepository,
            IEcommerceRepositoryAsync<OrderStatusEntity> orderStatusRepository,
            IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepository,
            IEcommerceRepositoryAsync<PromotionTypeEntity> promotionTypeEntityRepository,
            IEcommerceRepositoryAsync<LookupVariant> lookupVariantRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository,
            IEcommerceRepositoryAsync<AppOption> appOptionRepository,
            IEcommerceRepositoryAsync<OrderTypeEntity> orderTypeEntityRepository,
            ISettingService settingService,
            IBackendSettingsService backendSettingsService,
            ILocalizationService localizationService)
        {
            this.cache = cache;
            this.expirationTerm = appOptions.Value.DefaultCacheExpirationTermMinutes;
            this.roleManager = roleManager;
            this.contentProcessorRepository = contentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.appOptionsAccessor = appOptionsAccessor;
            this.customerTypeRepository = customerTypeRepository;
            this.orderStatusRepository = orderStatusRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.promotionTypeEntityRepository = promotionTypeEntityRepository;
            this._lookupVariantRepository = lookupVariantRepository;
            this._lookupRepository = lookupRepository;
            this.appOptionRepository = appOptionRepository;
            this.orderTypeEntityRepository = orderTypeEntityRepository;
            this.settingService = settingService;
            _localizationService = localizationService;
            _backendSettingsService = backendSettingsService;
        }

        private ReferenceData Populate()
        {
            var lookups = _lookupRepository.Query().Select(false);
            var tradeLookup = lookups.Single(x => x.Name == LookupNames.CustomerTradeClass).Id;
            var taxExemptLookup = lookups.Single(x => x.Name == LookupNames.CustomerTaxExempt).Id;
            var priorityLookup = lookups.Single(x => x.Name == LookupNames.CustomerNotePriorities).Id;
            var tierLookup = lookups.Single(x => x.Name == LookupNames.CustomerTier).Id;
            var termsLookup = lookups.Single(x => x.Name == LookupNames.Terms).Id;
            var fobLookup = lookups.Single(x => x.Name == LookupNames.Fob).Id;
            var marketingPromotionTypesLookup = lookups.Single(x => x.Name == LookupNames.MarketingPromotionType).Id;
            var orderSourcesLookup = lookups.Single(x => x.Name == LookupNames.OrderSources).Id;
            var orderSourcesCelebrityHealthAdvocateLookup = lookups.Single(x => x.Name == LookupNames.OrderSourcesCelebrityHealthAdvocate).Id;
            var orderPreferredShipMethod = lookups.Single(x => x.Name == LookupNames.OrderPreferredShipMethod).Id;
            var orderSourceTypes = lookups.Single(x => x.Name == LookupNames.OrderSourceTypes).Id;
            var pOrderTypes = lookups.Single(x => x.Name == LookupNames.POrderTypes).Id;
            var serviceCodes = lookups.Single(x => x.Name == LookupNames.ServiceCodes).Id;
            var affiliateProfessionalPractices = lookups.Single(x => x.Name == LookupNames.AffiliateProfessionalPractices).Id;
            var affiliateMonthlyEmailsSentOptions = lookups.Single(x => x.Name == LookupNames.AffiliateMonthlyEmailsSentOptions).Id;
            var affiliateTiers = lookups.Single(x => x.Name == LookupNames.AffiliateTiers).Id;
            var promotionBuyTypes = lookups.Single(x => x.Name == LookupNames.PromotionBuyTypes).Id;
            var shippingUpgrades = lookups.Single(x => x.Name == LookupNames.ShippingUpgrades).Id;
            var autoshipOptions = lookups.Single(x => x.Name == LookupNames.AutoShipSchedule).Id;
            var refundRedeemOptions = lookups.Single(x => x.Name == LookupNames.RefundRedeemOptions).Id;
            var productSellersOptions = lookups.Single(x => x.Name == LookupNames.ProductSellers).Id;
            var googleCategoriesOptions = lookups.Single(x => x.Name == LookupNames.GoogleCategories).Id;

            var lookupVariants = _lookupVariantRepository.Query()
                .Select(false)
                .GroupBy(l => l.IdLookup)
                .ToDictionary(l => l.Key, l => l.ToArray());

            // ReSharper disable once UseObjectOrCollectionInitializer
            var referenceData = new ReferenceData();
            referenceData.DefaultCountry = _backendSettingsService.GetDefaultCountry();
            referenceData.AdminRoles = roleManager.Roles.Where(x => x.IdUserType == UserType.Admin).OrderBy(p=>p.Order).Select(x => new LookupItem<int>
            {
                Key = x.Id,
                Text = x.Name
            }).ToList();
            referenceData.CustomerRoles = roleManager.Roles.Where(x => x.IdUserType == UserType.Customer).Select(x => new LookupItem<int>
            {
                Key = x.Id,
                Text = x.Name
            }).ToList();
            referenceData.AffiliateRoles = roleManager.Roles.Where(x => x.IdUserType == UserType.Affiliate).Select(x => new LookupItem<int>
            {
                Key = x.Id,
                Text = x.Name
            }).ToList();
            referenceData.UserStatuses = EnumHelper.GetItemsWithDescription<byte>(typeof(UserStatus)).Select(x => new LookupItem<byte>()
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.ContentTypes = contentTypeRepository.Query().Select(false).ToList().Select(x => new LookupItem<int>
            {
                Key = x.Id,
                Text = x.Name
            }).ToList();
            referenceData.ContentProcessors = contentProcessorRepository.Query().Select(false).ToList();
            referenceData.Labels = _localizationService.GetStrings();
            var months = CultureInfo.InvariantCulture.DateTimeFormat.MonthNames.Where(p => !String.IsNullOrEmpty(p)).ToList();
            referenceData.Months = new List<LookupItem<int>>();
            for (int i = 0; i < months.Count; i++)
            {
                referenceData.Months.Add(new LookupItem<int>()
                {
                    Key = i + 1,
                    Text = months[i],
                });
            }
            referenceData.PublicHost = !String.IsNullOrEmpty(appOptionsAccessor.Value.PublicHost)
                ? appOptionsAccessor.Value.PublicHost
                : "http://notdefined/";
            referenceData.VisibleOptions = LookupHelper.GetVisibleOptions();
            referenceData.ContentItemStatusNames = LookupHelper.GetContentItemStatusNames().Select(x => new LookupItem<string>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.ProductCategoryStatusNames = LookupHelper.GetProductCategoryStatusNames().Select(x => new LookupItem<string>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.GCTypes = LookupHelper.GetGCTypeNames().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.GCShortTypes = LookupHelper.GetShortGCTypeNames().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.RecordStatuses = LookupHelper.GetRecordStatuses().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.PublicRecordStatuses = LookupHelper.GetPublicRecordStatuses();
            referenceData.CustomerStatuses = LookupHelper.GetCustomerStatuses().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.AffiliateStatuses = LookupHelper.GetAffiliateStatuses().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.ProductTypes = LookupHelper.GetProductTypes().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.ShortProductTypes = LookupHelper.GetShortProductTypes().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.DiscountTypes = LookupHelper.GetDiscountTypes().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.AssignedCustomerTypes = LookupHelper.GetAssignedCustomerTypes().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.ActiveFilterOptions = LookupHelper.GetActiveFilterOptions().Select(x => new LookupItem<int?>
            {
                Key = x.Key == -1 ? null : (int?)x.Key,
                Text = x.Value
            }).ToList();
            referenceData.CustomerTypes = customerTypeRepository.Query(new CustomerTypeQuery().NotDeleted())
                .Select(x => new LookupItem<int>() { Key = x.Id, Text = x.Name })
                .ToList();
            referenceData.ShortCustomerTypes = referenceData.CustomerTypes.Select(x => new LookupItem<int>() { Key = x.Key, Text = x.Text.Substring(0,1) }).ToList();
            referenceData.OrderStatuses = orderStatusRepository.Query().Select(x => new LookupItem<int>() { Key = x.Id, Text = x.Name })
                .ToList();
            referenceData.PaymentMethods = paymentMethodRepository.Query().Select(x => new LookupItem<int>() { Key = x.Id, Text = x.Name })
                .ToList();
            var shortPaymentMethods = (new List<LookupItem<int>>(referenceData.PaymentMethods));
            referenceData.ShortPaymentMethods = LookupHelper.GetShortPaymentMethods(shortPaymentMethods);
            referenceData.OrderTypes = orderTypeEntityRepository.Query().Select(x => new LookupItem<int>() { Key = x.Id, Text = x.Name })
                .ToList();
            referenceData.PublicOrderTypes = LookupHelper.GetPublicOrderTypes(referenceData.OrderTypes);
            var shortOrderTypes = (new List<LookupItem<int>>(referenceData.OrderTypes));
            referenceData.ShortOrderTypes = LookupHelper.GetShortOrderTypes(shortOrderTypes);
            referenceData.TaxExempts = lookupVariants[taxExemptLookup].Select(x => new LookupItem<int>()
            {
                Key = x.Id,
                Text = x.ValueVariant
            }).ToList();
            referenceData.Tiers = lookupVariants[tierLookup].Select(x => new LookupItem<int>()
            {
                Key = x.Id,
                Text = x.ValueVariant
            }).ToList();
            referenceData.TradeClasses = lookupVariants[tradeLookup].Select(x => new LookupItem<int>()
            {
                Key = x.Id,
                Text = x.ValueVariant
            }).ToList();
            referenceData.CustomerNotePriorities = lookupVariants[priorityLookup].Select(x => new LookupItem<int>()
            {
                Key = x.Id,
                Text = x.ValueVariant
            }).ToList();
            referenceData.CreditCardTypes = LookupHelper.GetCreditCardTypes().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.OacTerms =
                lookupVariants[termsLookup]
                    .Select(x => new LookupItem<int>()
                    {
                        Key = x.Id,
                        Text = x.ValueVariant
                    }).ToList();
            referenceData.OacFob = lookupVariants[fobLookup]
                    .Select(x => new LookupItem<int>()
                    {
                        Key = x.Id,
                        Text = x.ValueVariant
                    }).ToList();
            referenceData.MarketingPromotionTypes =
                lookupVariants[marketingPromotionTypesLookup]
                    .Select(x => new LookupItem<int>()
                    {
                        Key = x.Id,
                        Text = x.ValueVariant
                    }).ToList();
            referenceData.OrderSources = lookupVariants[orderSourcesLookup]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.OrderSourcesCelebrityHealthAdvocate = lookupVariants[orderSourcesCelebrityHealthAdvocateLookup]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.OrderPreferredShipMethod = lookupVariants[orderPreferredShipMethod]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.OrderSourceTypes = lookupVariants[orderSourceTypes]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.POrderTypes = lookupVariants[pOrderTypes]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.FilterPNPOrderTypes = LookupHelper.GetFilterPNPOrderTypes();
            referenceData.ServiceCodes = lookupVariants[serviceCodes]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.AffiliateProfessionalPractices = lookupVariants[affiliateProfessionalPractices]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.AffiliateMonthlyEmailsSentOptions = lookupVariants[affiliateMonthlyEmailsSentOptions]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.AffiliateTiers = lookupVariants[affiliateTiers]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.TicketStatuses = LookupHelper.GetTicketStatuses().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.BugTicketStatuses = LookupHelper.GetBugTicketStatuses().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.Priorities = LookupHelper.GetPriorities().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.PromotionTypes = promotionTypeEntityRepository.Query()
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.Name
                }).ToList();
            referenceData.ExpiredTypes = LookupHelper.GetExpiredTypes();
            referenceData.DateStatuses = LookupHelper.GetDateStatuses();
            referenceData.CartShippingOptions = LookupHelper.GetCartShippingOptions().Select(x => new LookupItem<int>
			{
				Key = x.Key,
				Text = x.Value
			}).ToList();
            referenceData.PromotionBuyTypes = lookupVariants[promotionBuyTypes]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.ShippingUpgrades = lookupVariants[shippingUpgrades]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
			referenceData.AutoShipOptions = lookupVariants[autoshipOptions]
				.Select(x => new LookupItem<int>()
				{
					Key = x.Id,
					Text = x.ValueVariant
				}).ToList();
            referenceData.RefundRedeemOptions = lookupVariants[refundRedeemOptions]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.ProductSellers = lookupVariants[productSellersOptions]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.GoogleCategories = lookupVariants[googleCategoriesOptions]
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.PersonTitles = LookupHelper.GetPersonTitles();
            referenceData.ShipMethodTypes = LookupHelper.GetShipMethodTypes();
            referenceData.Carriers = LookupHelper.GetCarriers();
            referenceData.Warehouses = LookupHelper.GetWarehouses();
            referenceData.ProductCategoryViewTypes = LookupHelper.GetProductCategoryViewTypes();

            //BUG: shoule be moved to the specific worker
            SetupAppSettings();

            return referenceData;
            
        }

        private void SetupAppSettings()
        {
            var affiliateReportDateOption = appOptionRepository.Query(p => p.OptionName == AffiliateConstants.AffiliateOrderPaymentsCountToDateOptionName).Select().FirstOrDefault();
            if (affiliateReportDateOption != null)
            {
                var firstDayOfCurrentMonth = DateTime.Now;
                firstDayOfCurrentMonth = new DateTime(firstDayOfCurrentMonth.Year, firstDayOfCurrentMonth.Month, 1);
                var date = TimeZoneInfo.ConvertTime(firstDayOfCurrentMonth, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local);
                var dbDate = DateTime.MinValue;
                DateTime.TryParse(affiliateReportDateOption.OptionValue, out dbDate);
                if (date != dbDate)
                {
                    affiliateReportDateOption.OptionValue = date.ToString("yyyy-MM-dd hh:mm:ss.fff");
                    appOptionRepository.Update(affiliateReportDateOption);
                }
            }
        }

        private ReferenceData SetAppSettings(ReferenceData referenceData)
        {
            var settings = settingService.GetSettingsAsync().GetAwaiter().GetResult();;
            referenceData.AppSettings = new AppSettings()
            {
                CreditCardAuthorizations = settings.SafeData.CreditCardAuthorizations,
                GlobalPerishableThreshold = settings.SafeData.GlobalPerishableThreshold,
                HealthwisePeriodMaxItemsCount = settings.SafeData.HealthwisePeriodMaxItemsCount
            };
            return referenceData;
        }

        public ReferenceData Data()
        {
            var referenceData = cache.GetItem<ReferenceData>(CacheKeys.AppInfrastructure);

            if (referenceData == null)
            {
                referenceData = Populate();
                cache.SetItem(CacheKeys.AppInfrastructure, referenceData, expirationTerm);
            }
            referenceData = SetAppSettings(referenceData);

            return referenceData;
        }
    }
}