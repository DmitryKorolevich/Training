using System;
using System.Collections.Generic;
using System.Linq;
#if DNX451
using System.Net.Mime;
#endif
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using Microsoft.Net.Http.Headers;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Setting;
using VitalChoice.Business.Queries.User;
using VitalChoice.Domain.Transfer.Azure;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Payment;
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
using VitalChoice.Workflow.Core;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Customers)]
    public class CustomerController : BaseApiController
    {
        private readonly ICountryService _countryService;
        private readonly IGenericService<AdminProfile> _adminProfileService;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IDynamicToModelMapper<CustomerDynamic> _customerMapper;
        private readonly IDynamicToModelMapper<CustomerAddressDynamic> _addressMapper;
        private readonly IDynamicToModelMapper<CustomerNoteDynamic> _noteMapper;
        private readonly ICustomerService _customerService;

        private readonly IEcommerceDynamicObjectService<CustomerAddressDynamic, Address, AddressOptionType, AddressOptionValue>
            _addressService;
        private readonly IEcommerceDynamicObjectService<CustomerNoteDynamic, CustomerNote, CustomerNoteOptionType, CustomerNoteOptionValue>
            _notesService;

		private readonly ILogger logger;

        public CustomerController(ICustomerService customerService,
            IDynamicToModelMapper<CustomerDynamic> customerMapper,
            IDynamicToModelMapper<CustomerAddressDynamic> addressMapper, ICountryService countryService,
            IGenericService<AdminProfile> adminProfileService, IHttpContextAccessor contextAccessor,
            IEcommerceDynamicObjectService<CustomerAddressDynamic, Address, AddressOptionType, AddressOptionValue>
                addressService,
            IEcommerceDynamicObjectService
                <CustomerNoteDynamic, CustomerNote, CustomerNoteOptionType, CustomerNoteOptionValue> notesService,
            IDynamicToModelMapper<CustomerNoteDynamic> noteMapper, ILoggerProviderExtended loggerProvider)
        {
            _customerService = customerService;
            _countryService = countryService;
            _adminProfileService = adminProfileService;
            _contextAccessor = contextAccessor;
            _customerMapper = customerMapper;
            _addressMapper = addressMapper;
            _addressService = addressService;
            _notesService = notesService;
            _noteMapper = noteMapper;
            this.logger = loggerProvider.CreateLoggerDefault();
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
				PublicId = Guid.NewGuid(),
                CustomerType = CustomerType.Retail,
                TaxExempt = TaxExempt.YesCurrentCertificate,
                Tier = Tier.Tier1,
                InceptionDate = DateTime.Now,
                TradeClass = 1,
                CustomerNotes = new List<CustomerNoteModel>()
                {
                },
                Shipping = new List<AddressModel>() {new AddressModel() {AddressType = AddressType.Shipping}}
            };
        }

        [HttpPost]
        public Result<CreditCardModel> CreateCreditCardPrototype()
        {
            return new CreditCardModel
            {
                Address = new AddressModel {AddressType = AddressType.Billing},
                CardType = CreditCardType.MasterCard
            };
        }

        [HttpPost]
        public Result<OacPaymentModel> CreateOacPrototype()
        {
            return new OacPaymentModel
            {
                Address = new AddressModel { AddressType = AddressType.Billing },
                Fob = 1,
                Terms = 1
            };
        }

        [HttpPost]
        public Result<CheckPaymentModel> CreateCheckPrototype()
        {
            return new CheckPaymentModel
            {
                Address = new AddressModel {AddressType = AddressType.Billing}
            };
        }
            
        [HttpPost]
        public Result<AddressModel> CreateAddressPrototype()
        {
            return new AddressModel() {AddressType = AddressType.Shipping};
        }

        [HttpPost]
        public async Task<Result<CustomerNoteModel>> CreateCustomerNotePrototype()
        {
            var toReturn = new CustomerNoteModel()
            {
                Priority = CustomerNotePriority.NormalPriority,
                DateEdited = DateTime.Now,
            };
            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                var adminProfileCondition = new AdminProfileQuery().WithId(userId);
                var adminProfile = (await _adminProfileService.QueryAsync(adminProfileCondition)).FirstOrDefault();
                if (adminProfile != null)
                {
                    toReturn.EditedBy = adminProfile.AgentId;
                }
            }

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<CustomerNoteModel>> AddNote([FromBody] CustomerNoteModel model, int idCustomer)
        {
            if (!Validate(model))
                return null;
            var note = _noteMapper.FromModel(model);
            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (int.TryParse(sUserId, out userId))
            {
                note.IdEditedBy = userId;
                note.IdCustomer = idCustomer;
            }
            note = await _notesService.InsertAsync(note);
            var toReturn = _noteMapper.ToModel<CustomerNoteModel>(note);
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteNote(int idNote)
        {
            if (idNote > 0)
                return await _notesService.DeleteAsync(idNote, true);
            throw new AppValidationException("Note cannot be deleted.");
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
            if (idAddress > 0)
                return await _addressService.DeleteAsync(idAddress, true);
            throw new AppValidationException("Please select address to delete.");
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
            item.IdEditedBy = userId;
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

		[HttpPost]
	    public async Task<Result<CustomerFileModel>> UploadCustomerFile()
	    {
		    var form = await Request.ReadFormAsync();

			var publicId = form["data"];
			
			var parsedContentDisposition = ContentDispositionHeaderValue.Parse(form.Files[0].ContentDisposition);

			var contentType = form.Files[0].ContentType;
            using (var stream = form.Files[0].OpenReadStream())
		    {
			    var fileContent = stream.ReadFully();
			    try
			    {
				    var fileName = await _customerService.UploadFileAsync(fileContent, parsedContentDisposition.FileName.Replace("\"", ""), publicId, contentType);

					return new CustomerFileModel() { FileName = fileName, UploadDate = DateTime.Now };
				}
				catch (Exception ex)
			    {
				    this.logger.LogError(ex.ToString());
				    throw;
			    }
		    }
	    }

#if DNX451
		[HttpGet]
		public async Task<FileResult> GetFile([FromQuery]string publicId, [FromQuery]string fileName, [FromQuery]bool viewMode)
	    {
			var blob = await _customerService.DownloadFileAsync(fileName, publicId);

			var contentDisposition = new ContentDisposition()
		    {
				FileName = fileName,
				Inline = viewMode
			};

			Response.Headers.Append("Content-Disposition", contentDisposition.ToString());
			return File(blob.File, blob.ContentType);
		}
#endif
    }
}