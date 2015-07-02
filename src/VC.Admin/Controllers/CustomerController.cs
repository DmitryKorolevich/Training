using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.Customer;
using VC.Admin.Models.OrderNote;
using VC.Admin.Models.Product;
using VC.Admin.Models.Setting;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Customers;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Interfaces.Services.Customer;
using VitalChoice.Interfaces.Services.Order;
using VitalChoice.Interfaces.Services.Payment;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
	[AdminAuthorize(PermissionType.Customers)]
    public class CustomerController : BaseApiController
    {
		private readonly ICountryService _countryService;
	    private readonly CustomerMapper _customerMapper;
	    private readonly ICustomerService _customerService;


		public CustomerController(ICustomerService customerService, ICountryService countryService, CustomerMapper customerMapper)
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
		public Result<AddCustomerModel> CreateCustomerPrototype()
		{
			return new AddCustomerModel()
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
		public async Task<Result<AddCustomerModel>> AddCustomer([FromBody] AddCustomerModel addCustomerModel)
		{
			if (!Validate(addCustomerModel))
				return null;
            var item = _customerMapper.FromModel(addCustomerModel);
            var sUserId = Request.HttpContext.User.GetUserId();
			int userId;
			if (int.TryParse(sUserId, out userId))
			{
				item.IdEditedBy = userId;
			}

			item = (await _customerService.AddUpdateCustomerAsync(item));

			var toReturn = _customerMapper.ToModel<AddCustomerModel>(item);
			return toReturn;
		}

		[HttpPost]
		public async Task<Result<PagedList<CustomerListItemModel>>> GetDiscounts([FromBody]CustomerFilter filter)
		{
			var result = await _customerService.GetCustomersAsync(filter);

			var toReturn = new PagedList<CustomerListItemModel>
			{
				Items = result.Items.Select(p => new CustomerListItemModel(p)).ToList(),
				Count = result.Count,
			};

			return toReturn;
		}
	}
}