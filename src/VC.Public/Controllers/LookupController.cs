using System.Collections.Generic;
using System.Threading.Tasks;
using VC.Public.Models.Lookup;
using VitalChoice.Core.Base;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Validation.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services;

namespace VC.Public.Controllers
{
	[AllowAnonymous]
    public class LookupController : BaseMvcController
	{
		private readonly ICountryService _countryService;
	    private readonly ReferenceData _referenceData;

	    public LookupController(ICountryService countryService,
            IPageResultService pageResultService, ReferenceData referenceData) : base(pageResultService)
	    {
	        _countryService = countryService;
	        _referenceData = referenceData;
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
                    StateCode = x.StateCode,
					Id = x.Id
				}).OrderBy(x => x.StateName).ToList()
			}).ToList();
		}

		[HttpGet]
		public Result<IList<LookupItem<int>>> GetCreditCardTypes()
		{
			var creditCardTypes = _referenceData.CreditCardTypes;

			return new Result<IList<LookupItem<int>>>(true, creditCardTypes);
		}

		[HttpGet]
		public Result<IList<LookupItem<int>>> GetCartShippingOptions()
		{
			var options = _referenceData.CartShippingOptions;

			return new Result<IList<LookupItem<int>>>(true, options);
		}

		[HttpGet]
		public Result<IList<LookupItem<int>>> GetShippingPreferredOptions()
		{
			var shipMethods = _referenceData.OrderPreferredShipMethod;

			return new Result<IList<LookupItem<int>>>(true, shipMethods);
		}
	}
}