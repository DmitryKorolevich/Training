﻿using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.Infrastructure;
using VitalChoice.Core.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
    public class InfrastructureController : BaseApiController
    {
        private readonly IAppInfrastructureService appInfrastructureService;
        private readonly IHttpContextAccessor contextAccessor;

        public InfrastructureController(IAppInfrastructureService appInfrastructureService, IHttpContextAccessor contextAccessor,
            ILoggerProviderExtended loggerProvider)
        {
            this.appInfrastructureService = appInfrastructureService;
            this.contextAccessor = contextAccessor;
        }

        [HttpGet]
        public Result<RestrictedReferenceData> GetReferenceData()
        {
            var referenceData = this.appInfrastructureService.Get();

	        RestrictedReferenceData referenceDataModel;

			if (contextAccessor.HttpContext.User.Identity.IsAuthenticated)
	        {
                referenceDataModel = new FullReferenceDataModel()
                {
                    AppSettings=referenceData.AppSettings,
                    Labels = referenceData.Labels,
                    AdminRoles = referenceData.AdminRoles,
                    CustomerRoles = referenceData.CustomerRoles,
                    UserStatuses = referenceData.UserStatuses,
                    ContentTypes = referenceData.ContentTypes,
                    ContentProcessors = referenceData.ContentProcessors,
                    Months = referenceData.Months,
                    PublicHost = referenceData.PublicHost,
                    VisibleOptions = referenceData.VisibleOptions,
                    ContentItemStatusNames = referenceData.ContentItemStatusNames,
                    ProductCategoryStatusNames = referenceData.ProductCategoryStatusNames,
                    GCTypes = referenceData.GCTypes,
                    GCShortTypes = referenceData.GCShortTypes,
                    RecordStatuses = referenceData.RecordStatuses,
                    PublicRecordStatuses=referenceData.PublicRecordStatuses,
                    CustomerStatuses = referenceData.CustomerStatuses,
                    AffiliateStatuses = referenceData.AffiliateStatuses,
                    ProductTypes = referenceData.ProductTypes,
                    ShortProductTypes = referenceData.ShortProductTypes,
                    DiscountTypes = referenceData.DiscountTypes,
                    AssignedCustomerTypes = referenceData.AssignedCustomerTypes,
                    ActiveFilterOptions = referenceData.ActiveFilterOptions,
                    CustomerTypes = referenceData.CustomerTypes,
                    ShortCustomerTypes=referenceData.ShortCustomerTypes,
                    OrderStatuses = referenceData.OrderStatuses,
                    PaymentMethods = referenceData.PaymentMethods,
                    ShortPaymentMethods = referenceData.ShortPaymentMethods,
                    OrderTypes = referenceData.OrderTypes,
                    ShortOrderTypes = referenceData.ShortOrderTypes,
                    TaxExempts = referenceData.TaxExempts,
					Tiers = referenceData.Tiers,
					TradeClasses = referenceData.TradeClasses,
					CustomerNotePriorities = referenceData.CustomerNotePriorities,
                    CreditCardTypes = referenceData.CreditCardTypes,
                    OacFob = referenceData.OacFob,
                    OacTerms = referenceData.OacTerms,
                    MarketingPromotionTypes = referenceData.MarketingPromotionTypes,
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
                    Priorities = referenceData.Priorities,
                    PromotionTypes = referenceData.PromotionTypes,
                    ExpiredTypes = referenceData.ExpiredTypes,
                    DateStatuses = referenceData.DateStatuses,
                    PromotionBuyTypes = referenceData.PromotionBuyTypes,
                    ShippingUpgrades = referenceData.ShippingUpgrades,
                    PersonTitles=referenceData.PersonTitles,
					CartShippingOptions = referenceData.CartShippingOptions,
                    RefundRedeemOptions=referenceData.RefundRedeemOptions,
                    ProductSellers = referenceData.ProductSellers,
                    GoogleCategories = referenceData.GoogleCategories,
                };
	        }
			else
			{
				referenceDataModel = new RestrictedReferenceData()
				{
					Labels = referenceData.Labels,
					CartShippingOptions = referenceData.CartShippingOptions
				};
			}

	        return referenceDataModel;
        }
    }
}
