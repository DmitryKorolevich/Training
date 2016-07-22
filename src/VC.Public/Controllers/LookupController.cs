using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VC.Public.Models.Lookup;
using VitalChoice.Core.Base;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Validation.Models;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Business.Services.Customers;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;

namespace VC.Public.Controllers
{
	[AllowAnonymous]
    public class LookupController : BaseMvcController
	{
		private readonly ICountryService _countryService;
	    private readonly ReferenceData _referenceData;
        private readonly ExtendedUserManager _userManager;
        private readonly ICustomerService _customerService;

        public LookupController(ICountryService countryService,
            IPageResultService pageResultService,
            ReferenceData referenceData,
            ExtendedUserManager userManager,
            ICustomerService customerService) : base(pageResultService)
	    {
	        _countryService = countryService;
	        _referenceData = referenceData;
            _userManager = userManager;
            _customerService = customerService;

	    }

	    [HttpGet]
		public async Task<Result<IList<CountryListItemModel>>> GetCountries(int? id)
		{
			var result = await _countryService.GetCountriesAsync(new CountryFilter() {ActiveOnly = true});
	        ICollection<CountryListItemModel> toReturn = null;
            var countries = result.OrderBy(x => x.Order).Select(p => new CountryListItemModel()
			{
				CountryName = p.CountryName,
				Id = p.Id,
                IdVisibility = p.IdVisibility,
                States = p.States.Select(x => new StateListItemModel()
				{
					StateName = x.StateName,
                    StateCode = x.StateCode,
					Id = x.Id
				}).OrderBy(x => x.StateName).ToList()
			}).ToList();
            
	        if (id.HasValue)
	        {
	            if (id == (int) CustomerTypeCode.All)
	            {
	                toReturn = countries;
	            }
	            if (id == (int) CustomerTypeCode.Wholesale)
	            {
	                toReturn = countries.Where(p => p.IdVisibility == CustomerTypeCode.All ||
	                                               p.IdVisibility == CustomerTypeCode.Wholesale).ToList();
	            }
                if (id == (int)CustomerTypeCode.Retail)
                {
                    toReturn = countries.Where(p => p.IdVisibility == CustomerTypeCode.All ||
                                                   p.IdVisibility == CustomerTypeCode.Retail).ToList();
                }
            }
	        else
	        {
                //inner areas
	            var sInternalId = _userManager.GetUserId(User);
	            if (!string.IsNullOrEmpty(sInternalId))
                {
                    var internalId = Convert.ToInt32(sInternalId);
                    var customer = await _customerService.SelectAsync(internalId);
                    if (customer != null)
                    {
                        if (customer.IdObjectType == (int)CustomerType.Wholesale)
                        {
                            toReturn = countries.Where(p => p.IdVisibility == CustomerTypeCode.All ||
                                                           p.IdVisibility == CustomerTypeCode.Wholesale).ToList();
                        }
                        if (customer.IdObjectType == (int)CustomerType.Retail)
                        {
                            toReturn = countries.Where(p => p.IdVisibility == CustomerTypeCode.All ||
                                                           p.IdVisibility == CustomerTypeCode.Retail).ToList();
                        }
                    }
                }
            }

            //default
	        if (toReturn == null)
	        {
	            toReturn = countries.Where(p => p.IdVisibility == CustomerTypeCode.All ||
                                                   p.IdVisibility == CustomerTypeCode.Retail).ToList();
            }

	        return toReturn.ToList();
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