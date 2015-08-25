using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Infrastructure.Utils;
using VitalChoice.Interfaces.Services;
using LookupHelper = VitalChoice.Business.Helpers.LookupHelper;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using System.Collections.Generic;

namespace VitalChoice.Business.Services
{
    public class AppInfrastructureService : IAppInfrastructureService
    {
	    private readonly ICacheProvider cache;
	    private readonly int expirationTerm;
	    private readonly RoleManager<IdentityRole<int>> roleManager;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IRepositoryAsync<ContentProcessor> contentProcessorRepository;
        private readonly IOptions<AppOptions> appOptionsAccessor;
	    private readonly IEcommerceRepositoryAsync<CustomerTypeEntity> customerTypeRepository;
        private readonly IEcommerceRepositoryAsync<OrderStatusEntity> orderStatusRepository;
        private readonly IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepository;
        private readonly IEcommerceRepositoryAsync<LookupVariant> lookupVariantRepository;
	    private readonly IEcommerceRepositoryAsync<Lookup> lookupRepository;

	    public AppInfrastructureService(ICacheProvider cache, IOptions<AppOptions> appOptions, RoleManager<IdentityRole<int>> roleManager,
            IRepositoryAsync<ContentProcessor> contentProcessorRepository, IRepositoryAsync<ContentTypeEntity> contentTypeRepository, 
            IOptions<AppOptions> appOptionsAccessor, IEcommerceRepositoryAsync<CustomerTypeEntity> customerTypeRepository,
            IEcommerceRepositoryAsync<OrderStatusEntity> orderStatusRepository, IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepository,
            IEcommerceRepositoryAsync<LookupVariant> lookupVariantRepository, IEcommerceRepositoryAsync<Lookup> lookupRepository)
        {
		    this.cache = cache;
		    this.expirationTerm = appOptions.Options.DefaultCacheExpirationTermMinutes;
		    this.roleManager = roleManager;
            this.contentProcessorRepository = contentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.appOptionsAccessor = appOptionsAccessor;
		    this.customerTypeRepository = customerTypeRepository;
            this.orderStatusRepository = orderStatusRepository;
            this.paymentMethodRepository = paymentMethodRepository;
            this.lookupVariantRepository = lookupVariantRepository;
		    this.lookupRepository = lookupRepository;
        }

	    private ReferenceData Populate()
	    {
		    var tradeLookup = lookupRepository.Query(x=>x.Name == LookupNames.CustomerTradeClass).Select(false).Single().Id;
		    var taxExemptLookup = lookupRepository.Query(x=>x.Name == LookupNames.CustomerTaxExempt).Select(false).Single().Id;
		    var priorityLookup = lookupRepository.Query(x=>x.Name == LookupNames.CustomerNotePriorities).Select(false).Single().Id;
		    var tierLookup = lookupRepository.Query(x=>x.Name == LookupNames.CustomerTier).Select(false).Single().Id;
            var termsLookup = lookupRepository.Query(x => x.Name == LookupNames.Terms).Select(false).Single().Id;
            var fobLookup = lookupRepository.Query(x => x.Name == LookupNames.Fob).Select(false).Single().Id;
            var orderSourcesLookup = lookupRepository.Query(x => x.Name == LookupNames.OrderSources).Select(false).Single().Id;
            var orderSourcesCelebrityHealthAdvocateLookup = lookupRepository.Query(x => x.Name == LookupNames.OrderSourcesCelebrityHealthAdvocate).Select(false).Single().Id;
            var orderPreferredShipMethod = lookupRepository.Query(x => x.Name == LookupNames.OrderPreferredShipMethod).Select(false).Single().Id;
            var affiliateProfessionalPractices = lookupRepository.Query(x => x.Name == LookupNames.AffiliateProfessionalPractices).Select(false).Single().Id;
            var affiliateMonthlyEmailsSentOptions = lookupRepository.Query(x => x.Name == LookupNames.AffiliateMonthlyEmailsSentOptions).Select(false).Single().Id;
            var affiliateTiers = lookupRepository.Query(x => x.Name == LookupNames.AffiliateTiers).Select(false).Single().Id;

            var referenceData = new ReferenceData();
	        referenceData.Roles = roleManager.Roles.Select(x => new LookupItem<int>
	        {
	            Key = x.Id,
	            Text = x.Name
	        }).ToList();
	        referenceData.UserStatuses = Infrastructure.Utils.EnumHelper.GetItemsWithDescription<byte>(typeof (UserStatus)).Select(x => new LookupItem<byte>()
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
	        referenceData.Labels = LocalizationService.GetStrings();
	        referenceData.PublicHost = !String.IsNullOrEmpty(appOptionsAccessor.Options.PublicHost)
	            ? appOptionsAccessor.Options.PublicHost
	            : "http://notdefined/";
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
	        referenceData.ProductTypes = LookupHelper.GetProductTypes().Select(x => new LookupItem<int>
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
	            Key = x.Key==-1 ? null : (int?)x.Key,
	            Text = x.Value
	        }).ToList();
	        referenceData.CustomerTypes = customerTypeRepository.Query(new CustomerTypeQuery().NotDeleted())
	            .Select(x => new LookupItem<int>() {Key = x.Id, Text = x.Name})
	            .ToList();
            referenceData.OrderStatuses = orderStatusRepository.Query().Select(x => new LookupItem<int>() { Key = x.Id, Text = x.Name })
                .ToList();
            referenceData.PaymentMethods = paymentMethodRepository.Query().Select(x => new LookupItem<int>() { Key = x.Id, Text = x.Name })
                .ToList();
            var shortPaymentMethods = (new List<LookupItem<int>>(referenceData.PaymentMethods));
            referenceData.ShortPaymentMethods = LookupHelper.GetShortPaymentMethods(shortPaymentMethods);
            referenceData.TaxExempts = lookupVariantRepository.Query().Where(x=>x.IdLookup == taxExemptLookup).Select(false).Select(x=> new LookupItem<int>()
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

			return referenceData;
	    }
    }
}