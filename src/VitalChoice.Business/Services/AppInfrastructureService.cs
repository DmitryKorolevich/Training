﻿using System;
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
using EnumHelper = VitalChoice.Business.Helpers.EnumHelper;

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
	    private readonly IEcommerceRepositoryAsync<LookupVariant> lookupVariantRepository;
	    private readonly IEcommerceRepositoryAsync<Lookup> lookupRepository;

	    public AppInfrastructureService(ICacheProvider cache, IOptions<AppOptions> appOptions, RoleManager<IdentityRole<int>> roleManager,
            IRepositoryAsync<ContentProcessor> contentProcessorRepository, IRepositoryAsync<ContentTypeEntity> contentTypeRepository, 
            IOptions<AppOptions> appOptionsAccessor, IEcommerceRepositoryAsync<CustomerTypeEntity> customerTypeRepository, IEcommerceRepositoryAsync<LookupVariant> lookupVariantRepository, IEcommerceRepositoryAsync<Lookup> lookupRepository)
        {
		    this.cache = cache;
		    this.expirationTerm = appOptions.Options.DefaultCacheExpirationTermMinutes;
		    this.roleManager = roleManager;
            this.contentProcessorRepository = contentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.appOptionsAccessor = appOptionsAccessor;
		    this.customerTypeRepository = customerTypeRepository;
		    this.lookupVariantRepository = lookupVariantRepository;
		    this.lookupRepository = lookupRepository;
        }

	    private ReferenceData Populate()
	    {
		    var tradeLookup = lookupRepository.Query(x=>x.Name == LookupNames.CustomerTradeClass).Select(false).Single().Id;
		    var taxExemptLookup = lookupRepository.Query(x=>x.Name == LookupNames.CustomerTaxExempt).Select(false).Single().Id;
		    var priorityLookup = lookupRepository.Query(x=>x.Name == LookupNames.CustomerNotePriorities).Select(false).Single().Id;
		    var tierLookup = lookupRepository.Query(x=>x.Name == LookupNames.CustomerTier).Select(false).Single().Id;

			var referenceData = new ReferenceData
		    {
			    Roles = roleManager.Roles.Select(x => new LookupItem<int>
			    {
				    Key = x.Id,
				    Text = x.Name
			    }).ToList(),

			    UserStatuses = Infrastructure.Utils.EnumHelper.GetItemsWithDescription<byte>(typeof (UserStatus)).Select(x => new LookupItem<byte>()
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

			    ContentTypes = contentTypeRepository.Query().Select(false).ToList().Select(x => new LookupItem<int>
			    {
				    Key = x.Id,
				    Text = x.Name
			    }).ToList(),

			    ContentProcessors = contentProcessorRepository.Query().Select(false).ToList(),

			    Labels = LocalizationService.GetStrings(),

			    PublicHost =
				    !String.IsNullOrEmpty(appOptionsAccessor.Options.PublicHost)
					    ? appOptionsAccessor.Options.PublicHost
					    : "http://notdefined/",

			    ContentItemStatusNames = EnumHelper.GetContentItemStatusNames().Select(x => new LookupItem<string>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

			    ProductCategoryStatusNames = EnumHelper.GetProductCategoryStatusNames().Select(x => new LookupItem<string>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

			    GCTypes = EnumHelper.GetGCTypeNames().Select(x => new LookupItem<int>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

			    RecordStatuses = EnumHelper.GetRecordStatuses().Select(x => new LookupItem<int>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

			    ProductTypes = EnumHelper.GetProductTypes().Select(x => new LookupItem<int>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

                DiscountTypes = EnumHelper.GetDiscountTypes().Select(x => new LookupItem<int>
                {
                    Key = x.Key,
                    Text = x.Value
                }).ToList(),

                AssignedCustomerTypes = EnumHelper.GetAssignedCustomerTypes().Select(x => new LookupItem<int>
                {
                    Key = x.Key,
                    Text = x.Value
                }).ToList(),

                ActiveFilterOptions = EnumHelper.GetActiveFilterOptions().Select(x => new LookupItem<int?>
                {
                    Key = x.Key==-1 ? null : (int?)x.Key,
                    Text = x.Value
                }).ToList(),

                CustomerTypes =
				    customerTypeRepository.Query(new CustomerTypeQuery().NotDeleted())
					    .Select(x => new LookupItem<int>() {Key = x.Id, Text = x.Name})
					    .ToList(),

				TaxExempts = lookupVariantRepository.Query().Where(x=>x.IdLookup == taxExemptLookup).Select(false).Select(x=> new LookupItem<int>()
				{
					Key = x.Id,
					Text = x.ValueVariant
				}).ToList(),

				Tiers = lookupVariantRepository.Query().Where(x => x.IdLookup == tierLookup).Select(false).Select(x => new LookupItem<int>()
				{
					Key = x.Id,
					Text = x.ValueVariant
				}).ToList(),

				TradeClasses = lookupVariantRepository.Query().Where(x => x.IdLookup == tradeLookup).Select(false).Select(x => new LookupItem<int>()
				{
					Key = x.Id,
					Text = x.ValueVariant
				}).ToList(),

				CustomerNotePriorities = lookupVariantRepository.Query().Where(x => x.IdLookup == priorityLookup).Select(false).Select(x => new LookupItem<int>()
				{
					Key = x.Id,
					Text = x.ValueVariant
				}).ToList(),

                CreditCardTypes = EnumHelper.GetCreditCardTypes().Select(x => new LookupItem<int>
                {
                    Key = x.Key,
                    Text = x.Value
                }).ToList()
			};

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