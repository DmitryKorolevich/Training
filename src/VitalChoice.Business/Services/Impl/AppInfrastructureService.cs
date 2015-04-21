using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Infrastructure.Cache;
using VitalChoice.Infrastructure.Utils;

namespace VitalChoice.Business.Services.Impl
{
    public class AppInfrastructureService : IAppInfrastructureService
    {
	    private readonly ICacheProvider cache;
	    private readonly int expirationTerm;
	    private readonly RoleManager<IdentityRole> roleManager;

	    public AppInfrastructureService(ICacheProvider cache, IOptions<AppOptions> appOptions, RoleManager<IdentityRole> roleManager)
	    {
		    this.cache = cache;
		    this.expirationTerm = appOptions.Options.DefaultCacheExpirationTermMinutes;
		    this.roleManager = roleManager;
	    }

	    private ReferenceData Populate()
	    {
		    var referenceData = new ReferenceData
		    {
			    Roles = roleManager.Roles.Select(x=>new LookupItem<string>
			    {
				    Key = x.Id,
					Text = x.Name
			    }).ToList(),

				UserStatuses = EnumHelper.GetItems(typeof(UserStatus)).Select(x=>new LookupItem<int>()
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