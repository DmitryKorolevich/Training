using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VC.Admin.Models.Infrastructure;
using VitalChoice.Core.Base;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
    public class InfrastructureController : BaseApiController
    {
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly ISettingService _settingService;

        public InfrastructureController(IAppInfrastructureService appInfrastructureService, ISettingService settingService)
        {
            _appInfrastructureService = appInfrastructureService;
            _settingService = settingService;
        }

        [HttpGet]
        public async Task<Result<RestrictedReferenceData>> GetReferenceData()
        {
            var referenceData = await _appInfrastructureService.GetDataAsync();

            RestrictedReferenceData referenceDataModel;

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                referenceDataModel = new FullReferenceDataModel
                {
                    AppSettings = await _settingService.GetSettingsInstanceAsync(),
                    Labels = referenceData.Labels,
                    AdminRoles = referenceData.AdminRoles,
                    CustomerRoles = referenceData.CustomerRoles,
                    UserStatuses = referenceData.UserStatuses,
                    ContentTypes = referenceData.ContentTypes,
                    ContentProcessors = referenceData.ContentProcessors,
                    Months = referenceData.Months,
                    PublicHost = referenceData.PublicHost,
                    GoogleCaptchaPublicKey = referenceData.GoogleCaptchaPublicKey,
                    VisibleOptions = referenceData.VisibleOptions,
                    ContentItemStatusNames = referenceData.ContentItemStatusNames,
                    ProductCategoryStatusNames = referenceData.ProductCategoryStatusNames,
                    GCTypes = referenceData.GCTypes,
                    GCShortTypes = referenceData.GCShortTypes,
                    RecordStatuses = referenceData.RecordStatuses,
                    PublicRecordStatuses = referenceData.PublicRecordStatuses,
                    CustomerStatuses = referenceData.CustomerStatuses,
                    AffiliateStatuses = referenceData.AffiliateStatuses,
                    ProductTypes = referenceData.ProductTypes,
                    ShortProductTypes = referenceData.ShortProductTypes,
                    DiscountTypes = referenceData.DiscountTypes,
                    AssignedCustomerTypes = referenceData.AssignedCustomerTypes,
                    ActiveFilterOptions = referenceData.ActiveFilterOptions,
                    CustomerTypes = referenceData.CustomerTypes,
                    ShortCustomerTypes = referenceData.ShortCustomerTypes,
                    OrderStatuses = referenceData.OrderStatuses,
                    PaymentMethods = referenceData.PaymentMethods,
                    ShortPaymentMethods = referenceData.ShortPaymentMethods,
                    OrderTypes = referenceData.OrderTypes,
                    PublicOrderTypes = referenceData.PublicOrderTypes,
                    ShortOrderTypes = referenceData.ShortOrderTypes,
                    TaxExempts = referenceData.TaxExempts,
                    Tiers = referenceData.Tiers,
                    TradeClasses = referenceData.TradeClasses,
                    CustomerNotePriorities = referenceData.CustomerNotePriorities,
                    CreditCardTypes = referenceData.CreditCardTypes,
                    OacFob = referenceData.OacFob,
                    OacTerms = referenceData.OacTerms,
                    MarketingPromotionTypes = referenceData.MarketingPromotionTypes,
                    MarketingPromotionTypesNotHidden = referenceData.MarketingPromotionTypesNotHidden,
                    OrderSources = referenceData.OrderSources,
                    OrderSourcesCelebrityHealthAdvocate = referenceData.OrderSourcesCelebrityHealthAdvocate,
                    OrderPreferredShipMethod = referenceData.OrderPreferredShipMethod,
                    OrderSourceTypes = referenceData.OrderSourceTypes,
                    POrderTypes = referenceData.POrderTypes,
                    FilterPNPOrderTypes = referenceData.FilterPNPOrderTypes,
                    ServiceCodes = referenceData.ServiceCodes,
                    AffiliateProfessionalPractices = referenceData.AffiliateProfessionalPractices,
                    AffiliateMonthlyEmailsSentOptions = referenceData.AffiliateMonthlyEmailsSentOptions,
                    AffiliateTiers = referenceData.AffiliateTiers,
                    TicketStatuses = referenceData.TicketStatuses,
                    BugTicketStatuses = referenceData.BugTicketStatuses,
                    Priorities = referenceData.Priorities,
                    PromotionTypes = referenceData.PromotionTypes,
                    ExpiredTypes = referenceData.ExpiredTypes,
                    DateStatuses = referenceData.DateStatuses,
                    PromotionBuyTypes = referenceData.PromotionBuyTypes,
                    ShippingUpgrades = referenceData.ShippingUpgrades,
                    PersonTitles = referenceData.PersonTitles,
                    ShipMethodTypes = referenceData.ShipMethodTypes,
                    Carriers = referenceData.Carriers,
                    Warehouses = referenceData.Warehouses,
                    ProductCategoryViewTypes = referenceData.ProductCategoryViewTypes,
                    CartShippingOptions = referenceData.CartShippingOptions,
                    RefundRedeemOptions = referenceData.RefundRedeemOptions,
                    ProductSellers = referenceData.ProductSellers,
                    GoogleCategories = referenceData.GoogleCategories,
                    EditLockAreas= BaseAppConstants.ADMIN_EDIT_LOCK_AREAS!=null ?
                        BaseAppConstants.ADMIN_EDIT_LOCK_AREAS.Split(',') : new string[0],
                };
            }
            else
            {
                referenceDataModel = new RestrictedReferenceData()
                {
                    Labels = referenceData.Labels,
                    CartShippingOptions = referenceData.CartShippingOptions,
                    GoogleCaptchaPublicKey = referenceData.GoogleCaptchaPublicKey,
                };
            }

            return referenceDataModel;
        }
    }
}
