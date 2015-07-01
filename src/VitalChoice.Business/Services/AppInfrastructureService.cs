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

	    public AppInfrastructureService(ICacheProvider cache, IOptions<AppOptions> appOptions, RoleManager<IdentityRole<int>> roleManager,
            IRepositoryAsync<ContentProcessor> contentProcessorRepository, IRepositoryAsync<ContentTypeEntity> contentTypeRepository, 
            IOptions<AppOptions> appOptionsAccessor, IEcommerceRepositoryAsync<CustomerTypeEntity> customerTypeRepository, IEcommerceRepositoryAsync<LookupVariant> lookupVariantRepository)
        {
		    this.cache = cache;
		    this.expirationTerm = appOptions.Options.DefaultCacheExpirationTermMinutes;
		    this.roleManager = roleManager;
            this.contentProcessorRepository = contentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.appOptionsAccessor = appOptionsAccessor;
		    this.customerTypeRepository = customerTypeRepository;
		    this.lookupVariantRepository = lookupVariantRepository;
        }

	    private ReferenceData Populate()
	    {
		    var referenceData = new ReferenceData
		    {
			    Roles = roleManager.Roles.Select(x => new LookupItem<int>
			    {
				    Key = x.Id,
				    Text = x.Name
			    }).ToList(),

			    UserStatuses = EnumHelper.GetItemsWithDescription<byte>(typeof (UserStatus)).Select(x => new LookupItem<byte>()
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

			    ContentItemStatusNames = StatusEnumHelper.GetContentItemStatusNames().Select(x => new LookupItem<string>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

			    ProductCategoryStatusNames = StatusEnumHelper.GetProductCategoryStatusNames().Select(x => new LookupItem<string>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

			    GCTypes = StatusEnumHelper.GetGCTypeNames().Select(x => new LookupItem<int>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

			    RecordStatuses = StatusEnumHelper.GetRecordStatuses().Select(x => new LookupItem<int>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

			    ProductTypes = StatusEnumHelper.GetProductTypes().Select(x => new LookupItem<int>
			    {
				    Key = x.Key,
				    Text = x.Value
			    }).ToList(),

                DiscountTypes = StatusEnumHelper.GetDiscountTypes().Select(x => new LookupItem<int>
                {
                    Key = x.Key,
                    Text = x.Value
                }).ToList(),

                AssignedCustomerTypes = StatusEnumHelper.GetAssignedCustomerTypes().Select(x => new LookupItem<int>
                {
                    Key = x.Key,
                    Text = x.Value
                }).ToList(),

                ActiveFilterOptions = StatusEnumHelper.GetActiveFilterOptions().Select(x => new LookupItem<int?>
                {
                    Key = x.Key==-1 ? null : (int?)x.Key,
                    Text = x.Value
                }).ToList(),

                CustomerTypes =
				    customerTypeRepository.Query(new CustomerTypeQuery().NotDeleted())
					    .Select(x => new LookupItem<int>() {Key = x.Id, Text = x.Name})
					    .ToList(),

				TaxExempts = lookupVariantRepository.Query().Where(x=>x.IdLookup == (int)LookupEnum.CustomerTaxExempt).Select(false).Select(x=> new LookupItem<int>()
				{
					Key = x.Id,
					Text = x.ValueVariant
				}).ToList(),

				Tiers = lookupVariantRepository.Query().Where(x => x.IdLookup == (int)LookupEnum.CustomerTier).Select(false).Select(x => new LookupItem<int>()
				{
					Key = x.Id,
					Text = x.ValueVariant
				}).ToList(),

				TradeClasses = lookupVariantRepository.Query().Where(x => x.IdLookup == (int)LookupEnum.CustomerTradeClass).Select(false).Select(x => new LookupItem<int>()
				{
					Key = x.Id,
					Text = x.ValueVariant
				}).ToList(),

				CustomerNotePriorities = lookupVariantRepository.Query().Where(x => x.IdLookup == (int)LookupEnum.CustomerNotePriorities).Select(false).Select(x => new LookupItem<int>()
				{
					Key = x.Id,
					Text = x.ValueVariant
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