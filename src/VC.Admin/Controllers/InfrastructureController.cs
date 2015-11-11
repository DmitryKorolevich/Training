using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.Infrastructure;
using VitalChoice.Core.Base;
using VitalChoice.Domain;
using VitalChoice.Domain.Transfer.Base;
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
                    RecordStatuses = referenceData.RecordStatuses,
                    CustomerStatuses = referenceData.CustomerStatuses,
                    AffiliateStatuses = referenceData.AffiliateStatuses,
                    ProductTypes = referenceData.ProductTypes,
                    ShortProductTypes = referenceData.ShortProductTypes,
                    DiscountTypes = referenceData.DiscountTypes,
                    AssignedCustomerTypes = referenceData.AssignedCustomerTypes,
                    ActiveFilterOptions = referenceData.ActiveFilterOptions,
                    CustomerTypes = referenceData.CustomerTypes,
                    OrderStatuses = referenceData.OrderStatuses,
                    PaymentMethods = referenceData.PaymentMethods,
                    ShortPaymentMethods = referenceData.ShortPaymentMethods,
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
                    OrderTypes = referenceData.OrderTypes,
                    POrderTypes = referenceData.POrderTypes,
                    AffiliateProfessionalPractices = referenceData.AffiliateProfessionalPractices,
                    AffiliateMonthlyEmailsSentOptions = referenceData.AffiliateMonthlyEmailsSentOptions,
                    AffiliateTiers = referenceData.AffiliateTiers,
                    TicketStatuses = referenceData.TicketStatuses,
                    Priorities = referenceData.Priorities,
                    PromotionTypes = referenceData.PromotionTypes,
                    ExpiredTypes = referenceData.ExpiredTypes,
                    PromotionBuyTypes = referenceData.PromotionBuyTypes,
                };
	        }
			else
			{
				referenceDataModel = new RestrictedReferenceData()
				{
					Labels = referenceData.Labels
				};
			}

	        return referenceDataModel;
        }
    }
}
