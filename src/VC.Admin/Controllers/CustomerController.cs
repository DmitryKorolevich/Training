using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Setting;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
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
        private readonly IGenericService<AdminProfile> _adminProfileService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDynamicToModelMapper<CustomerDynamic> _customerMapper;
        private readonly IDynamicToModelMapper<AddressDynamic> _addressMapper;
        private readonly ICustomerService _customerService;

        private readonly IEcommerceDynamicObjectService<AddressDynamic, Address, AddressOptionType, AddressOptionValue>
            _addressService;

        public CustomerController(ICustomerService customerService,
            IDynamicToModelMapper<CustomerDynamic> customerMapper,
            IDynamicToModelMapper<AddressDynamic> addressMapper, ICountryService countryService,
            IGenericService<AdminProfile> adminProfileService, IHttpContextAccessor contextAccessor,
            IEcommerceDynamicObjectService<AddressDynamic, Address, AddressOptionType, AddressOptionValue>
                addressService)
        {
            _customerService = customerService;
            _countryService = countryService;
            _adminProfileService = adminProfileService;
            _contextAccessor = contextAccessor;
            _customerMapper = customerMapper;
            _addressMapper = addressMapper;
            _addressService = addressService;
        }

        [HttpGet]
        public async Task<Result<IList<OrderNoteModel>>> GetOrderNotes(CustomerType customerType)
        {
            var result = await _customerService.GetAvailableOrderNotesAsync(customerType);
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
                CustomerNotes = new List<CustomerNoteModel>()
                {
                    new CustomerNoteModel()
                    {
                        Priority = CustomerNotePriority.NormalPriority
                    }
                },
                Shipping = new List<AddressModel>() {new AddressModel() {AddressType = AddressType.Shipping}}
            };
        }

        [HttpPost]
        public Result<AddressModel> CreateAddressPrototype()
        {
            return new AddressModel() {AddressType = AddressType.Shipping};
        }

        [HttpPost]
        public Result<CustomerNoteModel> CreateCustomerNotePrototype()
        {
            return new CustomerNoteModel()
            {
                Priority = CustomerNotePriority.NormalPriority,
                DateEdited = DateTime.Now,
                EditedBy =
                    _adminProfileService.Query(
                        x => x.Id == Convert.ToInt32(_contextAccessor.HttpContext.User.GetUserId()))
                        .Single()
                        .AgentId
            };
        }

        [HttpPost]
        public async Task<Result<AddressModel>> AddAddress([FromBody] AddressModel model, int idCustomer)
        {
            if (!Validate(model))
                return null;
            var address = _addressMapper.FromModel(model);
            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (int.TryParse(sUserId, out userId))
            {
                address.IdEditedBy = userId;
                address.IdCustomer = idCustomer;
            }
            address = await _addressService.InsertAsync(address);
            var toReturn = _addressMapper.ToModel<AddressModel>(address);
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteAddress(int idAddress)
        {
            return await _addressService.DeleteAsync(idAddress, true);
        }

        [HttpPost]
        public async Task<Result<AddUpdateCustomerModel>> AddUpdateCustomer(
            [FromBody] AddUpdateCustomerModel addUpdateCustomerModel)
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
        public async Task<Result<PagedList<CustomerListItemModel>>> GetCustomers([FromBody] CustomerFilter filter)
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

        [HttpGet]
        public async Task<Result<AddUpdateCustomerModel>> GetExistingCustomer(int id)
        {
            var result = await _customerService.SelectAsync(id);
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }

            var customerModel = _customerMapper.ToModel<AddUpdateCustomerModel>(result);

            var adminProfileCondition =
                new AdminProfileQuery().IdInRange(
                    result.CustomerNotes.Where(x => x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList());

            var adminProfiles = await _adminProfileService.QueryAsync(adminProfileCondition);
            foreach (var customerNote in customerModel.CustomerNotes)
            {
                customerNote.EditedBy =
                    adminProfiles.SingleOrDefault(
                        y => y.Id == result.CustomerNotes.Single(z => z.Id == customerNote.Id).IdEditedBy)?
                        .AgentId;
            }

            customerModel.CustomerNotes = customerModel.CustomerNotes.OrderByDescending(x => x.DateEdited).ToList();

            return customerModel;
        }
    }
}