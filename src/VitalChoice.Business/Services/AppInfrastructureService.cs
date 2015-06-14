using System;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.Content;
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
	    private readonly IEcommerceRepositoryAsync<CustomerType> customerTypeRepository;

	    public AppInfrastructureService(ICacheProvider cache, IOptions<AppOptions> appOptions, RoleManager<IdentityRole<int>> roleManager,
            IRepositoryAsync<ContentProcessor> contentProcessorRepository, IRepositoryAsync<ContentTypeEntity> contentTypeRepository, 
            IOptions<AppOptions> appOptionsAccessor, IEcommerceRepositoryAsync<CustomerType> customerTypeRepository)
        {
		    this.cache = cache;
		    this.expirationTerm = appOptions.Options.DefaultCacheExpirationTermMinutes;
		    this.roleManager = roleManager;
            this.contentProcessorRepository = contentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.appOptionsAccessor = appOptionsAccessor;
		    this.customerTypeRepository = customerTypeRepository;
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

			    CustomerTypes =
				    customerTypeRepository.Query(new CustomerTypeQuery().NotDeleted())
					    .Select(x => new LookupItem<int>() {Key = x.Id, Text = x.Name})
					    .ToList()
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