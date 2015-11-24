using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using LookupHelper = VitalChoice.Business.Helpers.LookupHelper;
using System.Collections.Generic;
using VitalChoice.Interfaces.Services.Settings;
using System.Globalization;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Ecommerce.Utils;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;

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
        private readonly IEcommerceRepositoryAsync<LookupVariant> lookupVariantRepository;
        private readonly IEcommerceRepositoryAsync<Lookup> lookupRepository;
        private readonly IEcommerceRepositoryAsync<AppOption> appOptionRepository;
        private readonly ISettingService settingService;
        private readonly IBackendSettingsService _backendSettingsService;
        private readonly ILocalizationService _localizationService;
        private readonly TimeZoneInfo _pstTimeZoneInfo;

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
            this.lookupVariantRepository = lookupVariantRepository;
            this.lookupRepository = lookupRepository;
            this.appOptionRepository = appOptionRepository;
            this.settingService = settingService;
            _localizationService = localizationService;
            _backendSettingsService = backendSettingsService;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        }

        private ReferenceData Populate()
        {
            var tradeLookup = lookupRepository.Query(x => x.Name == LookupNames.CustomerTradeClass).Select(false).Single().Id;
            var taxExemptLookup = lookupRepository.Query(x => x.Name == LookupNames.CustomerTaxExempt).Select(false).Single().Id;
            var priorityLookup = lookupRepository.Query(x => x.Name == LookupNames.CustomerNotePriorities).Select(false).Single().Id;
            var tierLookup = lookupRepository.Query(x => x.Name == LookupNames.CustomerTier).Select(false).Single().Id;
            var termsLookup = lookupRepository.Query(x => x.Name == LookupNames.Terms).Select(false).Single().Id;
            var fobLookup = lookupRepository.Query(x => x.Name == LookupNames.Fob).Select(false).Single().Id;
            var marketingPromotionTypesLookup = lookupRepository.Query(x => x.Name == LookupNames.MarketingPromotionType).Select(false).Single().Id;
            var orderSourcesLookup = lookupRepository.Query(x => x.Name == LookupNames.OrderSources).Select(false).Single().Id;
            var orderSourcesCelebrityHealthAdvocateLookup = lookupRepository.Query(x => x.Name == LookupNames.OrderSourcesCelebrityHealthAdvocate).Select(false).Single().Id;
            var orderPreferredShipMethod = lookupRepository.Query(x => x.Name == LookupNames.OrderPreferredShipMethod).Select(false).Single().Id;
            var orderTypes = lookupRepository.Query(x => x.Name == LookupNames.OrderTypes).Select(false).Single().Id;
            var pOrderTypes = lookupRepository.Query(x => x.Name == LookupNames.POrderTypes).Select(false).Single().Id;
            var affiliateProfessionalPractices = lookupRepository.Query(x => x.Name == LookupNames.AffiliateProfessionalPractices).Select(false).Single().Id;
            var affiliateMonthlyEmailsSentOptions = lookupRepository.Query(x => x.Name == LookupNames.AffiliateMonthlyEmailsSentOptions).Select(false).Single().Id;
            var affiliateTiers = lookupRepository.Query(x => x.Name == LookupNames.AffiliateTiers).Select(false).Single().Id;
            var promotionBuyTypes = lookupRepository.Query(x => x.Name == LookupNames.PromotionBuyTypes).Select(false).Single().Id;

            var referenceData = new ReferenceData();
            referenceData.DefaultCountry = _backendSettingsService.GetDefaultCountry();
            referenceData.AdminRoles = roleManager.Roles.Where(x => x.IdUserType == UserType.Admin).Select(x => new LookupItem<int>
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
            referenceData.RecordStatuses = LookupHelper.GetRecordStatuses().Select(x => new LookupItem<int>
            {
                Key = x.Key,
                Text = x.Value
            }).ToList();
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
            referenceData.OrderStatuses = orderStatusRepository.Query().Select(x => new LookupItem<int>() { Key = x.Id, Text = x.Name })
                .ToList();
            referenceData.PaymentMethods = paymentMethodRepository.Query().Select(x => new LookupItem<int>() { Key = x.Id, Text = x.Name })
                .ToList();
            var shortPaymentMethods = (new List<LookupItem<int>>(referenceData.PaymentMethods));
            referenceData.ShortPaymentMethods = LookupHelper.GetShortPaymentMethods(shortPaymentMethods);
            referenceData.TaxExempts = lookupVariantRepository.Query().Where(x => x.IdLookup == taxExemptLookup).Select(false).Select(x => new LookupItem<int>()
            {
                Key = x.Id,
                Text = x.ValueVariant
            }).ToList();
            referenceData.Tiers = lookupVariantRepository.Query().Where(x => x.IdLookup == tierLookup).Select(false).Select(x => new LookupItem<int>()
            {
                Key = x.Id,
                Text = x.ValueVariant
            }).ToList();
            referenceData.TradeClasses = lookupVariantRepository.Query().Where(x => x.IdLookup == tradeLookup).Select(false).Select(x => new LookupItem<int>()
            {
                Key = x.Id,
                Text = x.ValueVariant
            }).ToList();
            referenceData.CustomerNotePriorities = lookupVariantRepository.Query().Where(x => x.IdLookup == priorityLookup).Select(false).Select(x => new LookupItem<int>()
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
                lookupVariantRepository.Query()
                    .Where(x => x.IdLookup == termsLookup)
                    .Select(false)
                    .Select(x => new LookupItem<int>()
                    {
                        Key = x.Id,
                        Text = x.ValueVariant
                    }).ToList();
            referenceData.OacFob = lookupVariantRepository.Query()
                    .Where(x => x.IdLookup == fobLookup)
                    .Select(false)
                    .Select(x => new LookupItem<int>()
                    {
                        Key = x.Id,
                        Text = x.ValueVariant
                    }).ToList();
            referenceData.MarketingPromotionTypes =
                lookupVariantRepository.Query()
                    .Where(x => x.IdLookup == marketingPromotionTypesLookup)
                    .Select(false)
                    .Select(x => new LookupItem<int>()
                    {
                        Key = x.Id,
                        Text = x.ValueVariant
                    }).ToList();
            referenceData.OrderSources = lookupVariantRepository.Query()
                .Where(x => x.IdLookup == orderSourcesLookup)
                .Select(false)
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.OrderSourcesCelebrityHealthAdvocate = lookupVariantRepository.Query()
                .Where(x => x.IdLookup == orderSourcesCelebrityHealthAdvocateLookup)
                .Select(false)
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.OrderPreferredShipMethod = lookupVariantRepository.Query()
                .Where(x => x.IdLookup == orderPreferredShipMethod)
                .Select(false)
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.OrderTypes = lookupVariantRepository.Query()
                .Where(x => x.IdLookup == orderTypes)
                .Select(false)
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.POrderTypes = lookupVariantRepository.Query()
                .Where(x => x.IdLookup == pOrderTypes)
                .Select(false)
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.AffiliateProfessionalPractices = lookupVariantRepository.Query()
                .Where(x => x.IdLookup == affiliateProfessionalPractices)
                .Select(false)
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.AffiliateMonthlyEmailsSentOptions = lookupVariantRepository.Query()
                .Where(x => x.IdLookup == affiliateMonthlyEmailsSentOptions)
                .Select(false)
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.AffiliateTiers = lookupVariantRepository.Query()
                .Where(x => x.IdLookup == affiliateTiers)
                .Select(false)
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
            referenceData.PromotionBuyTypes = lookupVariantRepository.Query()
                .Where(x => x.IdLookup == promotionBuyTypes)
                .Select(false)
                .Select(x => new LookupItem<int>()
                {
                    Key = x.Id,
                    Text = x.ValueVariant
                }).ToList();
            referenceData.PersonTitles = LookupHelper.GetPersonTitles();

            //BUG: shoule be moved to the specific worker
            SetupAppSettings();

            return referenceData;
            
        }

        private void SetupAppSettings()
        {
            var affiliateReportDateOption = appOptionRepository.Query(p => p.OptionName == AffiliateConstants.AffiliateOrderPaymentsCountToDateOptionName).Select().FirstOrDefault();
            var firstDayOfCurrentMonth = DateTime.Now;
            firstDayOfCurrentMonth = new DateTime(firstDayOfCurrentMonth.Year, firstDayOfCurrentMonth.Month, 1);
            var date = TimeZoneInfo.ConvertTime(firstDayOfCurrentMonth, _pstTimeZoneInfo, TimeZoneInfo.Local);
            var dbDate = DateTime.MinValue;
            DateTime.TryParse(affiliateReportDateOption.OptionValue, out dbDate);
            if(date!= dbDate)
            {
                affiliateReportDateOption.OptionValue = date.ToString("yyyy-MM-dd hh:mm:ss.fff");
                appOptionRepository.Update(affiliateReportDateOption);
            }
        }

        private ReferenceData SetAppSettings(ReferenceData referenceData)
        {
            referenceData.AppSettings = settingService.GetAppSettingsAsync().Result;
            return referenceData;
        }

        public ReferenceData Get()
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