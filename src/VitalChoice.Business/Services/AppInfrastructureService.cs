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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Entities.Settings;

namespace VitalChoice.Business.Services
{
    public class AppInfrastructureService : IAppInfrastructureService
    {
        private readonly ICacheProvider _cache;
        private readonly int _expirationTerm;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IRepositoryAsync<ContentTypeEntity> _contentTypeRepository;
        private readonly IRepositoryAsync<ContentProcessorEntity> _contentProcessorRepository;
        private readonly IOptions<AppOptions> _appOptionsAccessor;
        private readonly IEcommerceRepositoryAsync<CustomerTypeEntity> _customerTypeRepository;
        private readonly IEcommerceRepositoryAsync<OrderStatusEntity> _orderStatusRepository;
        private readonly IEcommerceRepositoryAsync<PaymentMethod> _paymentMethodRepository;
        private readonly IEcommerceRepositoryAsync<PromotionTypeEntity> _promotionTypeEntityRepository;
        private readonly IEcommerceRepositoryAsync<LookupVariant> _lookupVariantRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> _lookupRepository;
        private readonly IEcommerceRepositoryAsync<AppOption> _appOptionRepository;
        private readonly IEcommerceRepositoryAsync<OrderTypeEntity> _orderTypeEntityRepository;
        private readonly ISettingService _settingService;
        private readonly IBackendSettingsService _backendSettingsService;
        private readonly ILocalizationService _localizationService;
        private static ReferenceData _cachedData;

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
            _cache = cache;
            _expirationTerm = appOptions.Value.DefaultCacheExpirationTermMinutes;
            _roleManager = roleManager;
            _contentProcessorRepository = contentProcessorRepository;
            _contentTypeRepository = contentTypeRepository;
            _appOptionsAccessor = appOptionsAccessor;
            _customerTypeRepository = customerTypeRepository;
            _orderStatusRepository = orderStatusRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _promotionTypeEntityRepository = promotionTypeEntityRepository;
            _lookupVariantRepository = lookupVariantRepository;
            _lookupRepository = lookupRepository;
            _appOptionRepository = appOptionRepository;
            _orderTypeEntityRepository = orderTypeEntityRepository;
            _settingService = settingService;
            _localizationService = localizationService;
            _backendSettingsService = backendSettingsService;
        }

        private async Task<ReferenceData> Populate()
        {
            var lookups = (await _lookupRepository.Query().SelectAsync(false)).GroupBy(l => l.Name)
                .ToDictionary(g => g.Key, g => g.ToList());
            var tradeLookup = lookups[LookupNames.CustomerTradeClass].Single().Id;
            var taxExemptLookup = lookups[LookupNames.CustomerTaxExempt].Single().Id;
            var priorityLookup = lookups[LookupNames.CustomerNotePriorities].Single().Id;
            var tierLookup = lookups[LookupNames.CustomerTier].Single().Id;
            var termsLookup = lookups[LookupNames.Terms].Single().Id;
            var fobLookup = lookups[LookupNames.Fob].Single().Id;
            var marketingPromotionTypesLookup = lookups[LookupNames.MarketingPromotionType].Single().Id;
            var orderSourcesLookup = lookups[LookupNames.OrderSources].Single().Id;
            var orderSourcesCelebrityHealthAdvocateLookup = lookups[LookupNames.OrderSourcesCelebrityHealthAdvocate].Single().Id;
            var orderPreferredShipMethod = lookups[LookupNames.OrderPreferredShipMethod].Single().Id;
            var orderSourceTypes = lookups[LookupNames.OrderSourceTypes].Single().Id;
            var pOrderTypes = lookups[LookupNames.POrderTypes].Single().Id;
            var serviceCodes = lookups[LookupNames.ServiceCodes].Single().Id;
            var affiliateProfessionalPractices = lookups[LookupNames.AffiliateProfessionalPractices].Single().Id;
            var affiliateMonthlyEmailsSentOptions = lookups[LookupNames.AffiliateMonthlyEmailsSentOptions].Single().Id;
            var affiliateTiers = lookups[LookupNames.AffiliateTiers].Single().Id;
            var promotionBuyTypes = lookups[LookupNames.PromotionBuyTypes].Single().Id;
            var shippingUpgrades = lookups[LookupNames.ShippingUpgrades].Single().Id;
            var autoshipOptions = lookups[LookupNames.AutoShipSchedule].Single().Id;
            var refundRedeemOptions = lookups[LookupNames.RefundRedeemOptions].Single().Id;
            var productSellersOptions = lookups[LookupNames.ProductSellers].Single().Id;
            var googleCategoriesOptions = lookups[LookupNames.GoogleCategories].Single().Id;

            var lookupVariants = (await _lookupVariantRepository.Query()
                .SelectAsync(false))
                .GroupBy(l => l.IdLookup)
                .ToDictionary(l => l.Key, l => l.ToArray());

            // ReSharper disable once UseObjectOrCollectionInitializer
            var referenceData = new ReferenceData();
            referenceData.DefaultCountry = _backendSettingsService.GetDefaultCountry();
            referenceData.AdminRoles = await _roleManager.Roles.Where(x => x.IdUserType == UserType.Admin).OrderBy(p=>p.Order).Select(x => new LookupItem<int>
            {
                Key = x.Id,
                Text = x.Name
            }).ToListAsync();
            referenceData.CustomerRoles = await _roleManager.Roles.Where(x => x.IdUserType == UserType.Customer).Select(x => new LookupItem<int>
            {
                Key = x.Id,
                Text = x.Name
            }).ToListAsync();
            referenceData.AffiliateRoles = await _roleManager.Roles.Where(x => x.IdUserType == UserType.Affiliate).Select(x => new LookupItem<int>
            {
                Key = x.Id,
                Text = x.Name
            }).ToListAsync();
            referenceData.UserStatuses = EnumHelper.GetItemsWithDescription<byte>(typeof(UserStatus)).Select(x => new LookupItem<byte>()
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
            referenceData.ContentTypes = (await _contentTypeRepository.Query().SelectAsync(false)).Select(x => new LookupItem<int>
            {
                Key = x.Id,
                Text = x.Name
            }).ToList();
            referenceData.ContentProcessors = await _contentProcessorRepository.Query().SelectAsync(false);
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
            referenceData.PublicHost = !String.IsNullOrEmpty(_appOptionsAccessor.Value.PublicHost)
                ? _appOptionsAccessor.Value.PublicHost
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
            referenceData.CustomerTypes = (await _customerTypeRepository.Query(new CustomerTypeQuery().NotDeleted())
                .SelectAsync(false)).Select(x => new LookupItem<int> {Key = x.Id, Text = x.Name}).ToList();
            referenceData.ShortCustomerTypes = referenceData.CustomerTypes.Select(x => new LookupItem<int>() { Key = x.Key, Text = x.Text.Substring(0,1) }).ToList();
            referenceData.OrderStatuses =
                (await _orderStatusRepository.Query().SelectAsync(false)).Select(x => new LookupItem<int>() {Key = x.Id, Text = x.Name})
                    .ToList();
            referenceData.PaymentMethods =
                (await _paymentMethodRepository.Query().SelectAsync(false)).Select(x => new LookupItem<int>() {Key = x.Id, Text = x.Name})
                    .ToList();
            var shortPaymentMethods = new List<LookupItem<int>>(referenceData.PaymentMethods);
            referenceData.ShortPaymentMethods = LookupHelper.GetShortPaymentMethods(shortPaymentMethods);
            referenceData.OrderTypes =
                (await _orderTypeEntityRepository.Query().SelectAsync(false)).Select(x => new LookupItem<int>() {Key = x.Id, Text = x.Name})
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
            referenceData.PromotionTypes = _promotionTypeEntityRepository.Query()
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.Name
                }, false);
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
            await SetupAppSettings();

            return referenceData;
            
        }

        private async Task SetupAppSettings()
        {
            var affiliateReportDateOption =
                await _appOptionRepository.Query(p => p.OptionName == AffiliateConstants.AffiliateOrderPaymentsCountToDateOptionName)
                    .SelectFirstOrDefaultAsync(false);
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
                    _appOptionRepository.Update(affiliateReportDateOption);
                }
            }
        }

        private async Task<ReferenceData> SetAppSettings(ReferenceData referenceData)
        {
            var settings = await _settingService.GetSettingsAsync();
            referenceData.AppSettings = new AppSettings()
            {
                CreditCardAuthorizations = settings.SafeData.CreditCardAuthorizations,
                GlobalPerishableThreshold = settings.SafeData.GlobalPerishableThreshold,
                HealthwisePeriodMaxItemsCount = settings.SafeData.HealthwisePeriodMaxItemsCount
            };
            return referenceData;
        }

        public ReferenceData CachedData => _cachedData;

        public async Task<ReferenceData> GetDataAsync()
        {
            var referenceData = _cache.GetItem<ReferenceData>(CacheKeys.AppInfrastructure);

            if (referenceData == null)
            {
                referenceData = await Populate();
                _cache.SetItem(CacheKeys.AppInfrastructure, referenceData, _expirationTerm);
            }
            referenceData = await SetAppSettings(referenceData);
            _cachedData = referenceData;
            return referenceData;
        }
    }
}