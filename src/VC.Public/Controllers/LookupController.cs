using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using VC.Public.ModelConverters;
using VC.Public.Models.Auth;
using VC.Public.Models.Lookup;
using VitalChoice.Core.Base;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;
using System.Linq;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Country;
using VitalChoice.Interfaces.Services;

namespace VC.Public.Controllers
{
	[AllowAnonymous]
    public class LookupController : BaseMvcController
	{
		private readonly ICountryService _countryService;
		private readonly IAppInfrastructureService _appInfrastructureService;

		public LookupController(ICountryService countryService, IAppInfrastructureService appInfrastructureService)
		{
			_countryService = countryService;
			_appInfrastructureService = appInfrastructureService;
		}

		[HttpGet]
		public async Task<Result<IList<CountryListItemModel>>> GetCountries()
		{
			var result = await _countryService.GetCountriesAsync(new CountryFilter() {ActiveOnly = true});
			return result.OrderBy(x => x.Order).Select(p => new CountryListItemModel()
			{
				CountryName = p.CountryName,
				Id = p.Id,
				States = p.States.Select(x => new StateListItemModel()
				{
					StateName = x.StateName,
					Id = x.Id
				}).OrderBy(x => x.StateName).ToList()
			}).ToList();
		}

		[CustomerAuthorize]
		[HttpGet]
		public Result<IList<LookupItem<int>>> GetCreditCardTypes()
		{
			var referenceData = _appInfrastructureService.Get();

			var creditCardTypes = referenceData.CreditCardTypes;

			return new Result<IList<LookupItem<int>>>(true, creditCardTypes);
		}
	}
}