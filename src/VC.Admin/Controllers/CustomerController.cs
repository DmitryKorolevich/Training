using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Setting;
using VitalChoice.Business.Services;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Customers;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
	[AdminAuthorize(PermissionType.Customers)]
    public class CustomerController : BaseApiController
    {
		private readonly ICountryService _countryService;
	    private readonly IDynamicToModelMapper<CustomerDynamic> _customerMapper;
		private readonly ICustomerService _customerService;


		public CustomerController(ICustomerService customerService, IDynamicToModelMapper<CustomerDynamic> customerMapper, ICountryService countryService)
		{
			_customerService = customerService;
			_countryService = countryService;
		    _customerMapper = customerMapper;
		}

		[HttpGet]
	    public async Task<Result<IList<OrderNoteModel>>> GetOrderNotes(CustomerType customerType)
		{
			var result =  await _customerService.GetAvailableOrderNotesAsync(customerType);
			return result.Select(x => new OrderNoteModel()
			{
				Id = x.Id,
				Name = x.Title
			}).ToList();
		}

		[HttpGet]
		public async Task<Result<IList<PaymentMethodModel>>> GetPaymentMethods(CustomerType customerType)
		{
			var result = await _customerService.GetAvailablePaymentMethodsAsync(customerType);
			return result.Select(x => new PaymentMethodModel()
			{
				Id = x.Id,
				Name = x.Name
			}).ToList();
		}

		[HttpGet]
		public async Task<Result<IList<CountryListItemModel>>> GetCountries()
		{
			var result = await _countryService.GetCountriesAsync();
			return result.Select(p => new CountryListItemModel(p)).ToList();
		}

		[HttpPost]
		public Result<AddUpdateCustomerModel> CreateCustomerPrototype()
		{
			return new AddUpdateCustomerModel()
			{
				CustomerType = CustomerType.Retail,
				TaxExempt = TaxExempt.YesCurrentCertificate,
				Tier = Tier.Tier1,
				InceptionDate = DateTime.Now,
				TradeClass = 1,
				CustomerNote = new CustomerNoteModel()
				{
					Priority = CustomerNotePriority.NormalPriority
				}
			};
		}

		[HttpPost]
		public async Task<Result<AddUpdateCustomerModel>> AddUpdateCustomer([FromBody] AddUpdateCustomerModel addUpdateCustomerModel)
		{
			if (!Validate(addUpdateCustomerModel))
				return null;
            var item = _customerMapper.FromModel(addUpdateCustomerModel);
            var sUserId = Request.HttpContext.User.GetUserId();
			int userId;
			if (int.TryParse(sUserId, out userId))
			{
				item.IdEditedBy = userId;
				foreach (var address in item.Addresses)
				{
					address.IdEditedBy = userId;
				}
				foreach (var customerNote in item.CustomerNotes)
				{
					customerNote.IdEditedBy = userId;
				}
			}
		    if (item.Id > 0)
		    {
		        item = await _customerService.UpdateAsync(item);
		    }
		    else
		    {
		        item = await _customerService.InsertAsync(item);
		    }
			var toReturn = _customerMapper.ToModel<AddUpdateCustomerModel>(item);
			return toReturn;
		}

		[HttpPost]
		public async Task<Result<PagedList<CustomerListItemModel>>> GetCustomers([FromBody]CustomerFilter filter)
		{
			var result = await _customerService.GetCustomersAsync(filter);
			
			var toReturn = new PagedList<CustomerListItemModel>
			{
				Items = result.Items.Select(p => new CustomerListItemModel()
					{
						Id = p.Id,
						Name = $"{p.LastName}, {p.FirstName}",
						City = p.City,
						Country = p.CountryCode,
						State = p.StateOrCounty,
						DateEdited = p.DateEdited,
						EditedBy = p.AdminProfile?.AgentId,
						StatusCode = p.StatusCode
					}).ToList(),
				Count = result.Count,
			};

			return toReturn;
		}
	}
}