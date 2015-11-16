using System;
using System.Collections.Generic;
using System.Linq;
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
using VitalChoice.Domain.Entities;
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
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;
using VitalChoice.Workflow.Core;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Settings;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Domain.Transfer.Settings;
using Newtonsoft.Json;
using Microsoft.Framework.DependencyInjection;
using VitalChoice.Business.ExportMaps;
using VitalChoice.Domain.Transfer;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Customers)]
    public class CustomerController : BaseApiController
    {
        private readonly ICountryService _countryService;
        private readonly IGenericService<AdminProfile> _adminProfileService;
	    private readonly IDynamicMapper<CustomerDynamic> _customerMapper;
        private readonly IDynamicMapper<CustomerAddressDynamic> _addressMapper;
        private readonly IDynamicMapper<CustomerNoteDynamic> _noteMapper;
        private readonly ICustomerService _customerService;
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly Country _defaultCountry;
        private readonly IExportService<ExtendedVCustomer, CustomersForAffiliatesCsvMap> _exportCustomersForAffiliatesService;

        private readonly IEcommerceDynamicService<CustomerAddressDynamic, Address, AddressOptionType, AddressOptionValue>
            _addressService;
        private readonly IEcommerceDynamicService<CustomerNoteDynamic, CustomerNote, CustomerNoteOptionType, CustomerNoteOptionValue>
            _notesService;

		private readonly ILogger logger;
        private readonly TimeZoneInfo _pstTimeZoneInfo;

        public CustomerController(ICustomerService customerService,
            IDynamicMapper<CustomerDynamic> customerMapper,
            IDynamicMapper<CustomerAddressDynamic> addressMapper, ICountryService countryService,
            IGenericService<AdminProfile> adminProfileService, IHttpContextAccessor contextAccessor,
            IEcommerceDynamicService<CustomerAddressDynamic, Address, AddressOptionType, AddressOptionValue>
                addressService,
            IEcommerceDynamicService
                <CustomerNoteDynamic, CustomerNote, CustomerNoteOptionType, CustomerNoteOptionValue> notesService,
            IDynamicMapper<CustomerNoteDynamic> noteMapper, ILoggerProviderExtended loggerProvider, IStorefrontUserService storefrontUserService,
            IOptions<AppOptions> appOptions,
            IAppInfrastructureService appInfrastructureService,
            IObjectHistoryLogService objectHistoryLogService,
            IExportService<ExtendedVCustomer, CustomersForAffiliatesCsvMap> exportCustomersForAffiliatesService)
        {
            _customerService = customerService;
            _countryService = countryService;
            _adminProfileService = adminProfileService;
	        _customerMapper = customerMapper;
            _addressMapper = addressMapper;
            _addressService = addressService;
            _notesService = notesService;
            _noteMapper = noteMapper;
            this.logger = loggerProvider.CreateLoggerDefault();
	        _storefrontUserService = storefrontUserService;
            _objectHistoryLogService = objectHistoryLogService;
            _defaultCountry = appInfrastructureService.Get().DefaultCountry;
            _exportCustomersForAffiliatesService = exportCustomersForAffiliatesService;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
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
        public async Task<Result<AddUpdateCustomerModel>> CreateCustomerPrototype([FromBody] object temp)
        {
            var model = await _customerService.CreatePrototypeForAsync<AddUpdateCustomerModel>((int)CustomerType.Retail);
            model.PublicId = Guid.NewGuid();
            model.TaxExempt = TaxExempt.YesCurrentCertificate;
            model.Tier = Tier.Tier1;
            model.InceptionDate = DateTime.Now;
            model.TradeClass = 1;
            model.CustomerNotes = new List<CustomerNoteModel>();
            model.ProfileAddress.Country = new CountryListItemModel(_defaultCountry);
            model.Shipping = new List<AddressModel>() { new AddressModel() { AddressType = AddressType.Shipping, Country = new CountryListItemModel(_defaultCountry) } };
            model.StatusCode = (int)CustomerStatus.NotActive;
            model.ApprovedPaymentMethods = new List<int>() { 1 };//card
            model.DefaultPaymentMethod = 1;//card
            return model;
        }

        [HttpPost]
        public Result<CreditCardModel> CreateCreditCardPrototype([FromBody] object model)
        {
            return new CreditCardModel
            {
                Address = new AddressModel {AddressType = AddressType.Billing, Country = new CountryListItemModel(_defaultCountry) },
                CardType = CreditCardType.MasterCard
            };
        }

        [HttpPost]
        public Result<OacPaymentModel> CreateOacPrototype([FromBody] object model)
        {
            return new OacPaymentModel
            {
                Address = new AddressModel { AddressType = AddressType.Billing, Country = new CountryListItemModel(_defaultCountry) },
                Fob = 1,
                Terms = 1
            };
        }

        [HttpPost]
        public Result<CheckPaymentModel> CreateCheckPrototype([FromBody] object model)
        {
            return new CheckPaymentModel
            {
                Address = new AddressModel {AddressType = AddressType.Billing, Country = new CountryListItemModel(_defaultCountry) }
            };
        }

        [HttpPost]
        public Result<CheckPaymentModel> CreateWireTransferPrototype([FromBody] object model)
        {
            return new CheckPaymentModel
            {
                Address = new AddressModel { AddressType = AddressType.Billing, Country = new CountryListItemModel(_defaultCountry) }
            };
        }

        [HttpPost]
        public Result<CheckPaymentModel> CreateMarketingPrototype([FromBody] object model)
        {
            return new CheckPaymentModel
            {
                Address = new AddressModel { AddressType = AddressType.Billing, Country = new CountryListItemModel(_defaultCountry) }
            };
        }

        [HttpPost]
        public Result<CheckPaymentModel> CreateVCWellnessPrototype([FromBody] object model)
        {
            return new CheckPaymentModel
            {
                Address = new AddressModel { AddressType = AddressType.Billing, Country = new CountryListItemModel(_defaultCountry) }
            };
        }

        [HttpPost]
        public Result<AddressModel> CreateAddressPrototype([FromBody] object model)
        {
            return new AddressModel() {AddressType = AddressType.Shipping, Country = new CountryListItemModel(_defaultCountry)};
        }

        [HttpPost]
        public async Task<Result<CustomerNoteModel>> CreateCustomerNotePrototype([FromBody] object model)
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
        public async Task<Result<bool>> DeleteNote(int idNote, [FromBody] object model)
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
        public async Task<Result<bool>> DeleteAddress(int idAddress, [FromBody] object model)
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

			toReturn.IsConfirmed = addUpdateCustomerModel.IsConfirmed;
			toReturn.PublicUserId = addUpdateCustomerModel.PublicId;

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
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Email = p.Email,
                    City = p.City,
                    Country = p.CountryCode,
                    State = p.StateOrCounty,
                    DateEdited = p.DateEdited,
                    EditedBy = p.AdminProfile?.AgentId,
                    StatusCode = p.StatusCode,
                    LastOrderPlaced = p.LastOrderPlaced,
                    FirstOrderPlaced = p.FirstOrderPlaced,
                    TotalOrders = p.TotalOrders,
                }).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<FileResult> GetCustomersForAffiliates([FromQuery]int idaffiliate)
        {
            IList<ExtendedVCustomer> items = new List<ExtendedVCustomer>();
            if (idaffiliate != 0)
            {
                CustomerFilter filter = new CustomerFilter();
                filter.IdAffiliate = idaffiliate.ToString();
                filter.Paging = new Paging() { PageIndex = 1, PageItemCount = 1000000 };
                var result = await _customerService.GetCustomersAsync(filter);
                items = result.Items;
            }
            foreach(var item in items)
            {
                if (item.FirstOrderPlaced.HasValue)
                {
                    item.FirstOrderPlaced=TimeZoneInfo.ConvertTime(item.FirstOrderPlaced.Value, TimeZoneInfo.Local, _pstTimeZoneInfo);
                }
            }

            var data = _exportCustomersForAffiliatesService.ExportToCSV(items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.AFFILIATE_CUSTOMERS_REPORT, DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss"))
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
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

	        var login = await _storefrontUserService.GetAsync(customerModel.Id);
	        if (login == null)
	        {
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
			}

			customerModel.IsConfirmed = login.IsConfirmed;
			customerModel.PublicUserId = login.PublicId;

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

			var publicId = form["publicId"];
			
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

		[HttpPost]
		public async Task<Result<bool>> ResendActivation(Guid id, [FromBody] object model)
		{
			await _storefrontUserService.ResendActivationAsync(id);

			return true;
		}

		[HttpPost]
		public async Task<Result<bool>> ResetPassword(Guid id, [FromBody] object model)
		{
			await _storefrontUserService.SendResetPasswordAsync(id);

			return true;
		}

		[HttpPost]
		public async Task<Result<string>> LoginAsCustomer(Guid id, [FromBody] object model)
		{
			var token = await _storefrontUserService.GenerateLoginTokenAsync(id);

			return token;
		}

		[HttpGet]
		public async Task<FileResult> GetFile([FromQuery]string publicId, [FromQuery]string fileName, [FromQuery]bool viewMode)
	    {
			var blob = await _customerService.DownloadFileAsync(fileName, publicId);

			var contentDisposition = new ContentDispositionHeaderValue(viewMode ? "inline" : "attachment")
            {
                FileName = fileName
            };

            Response.Headers.Append("Content-Disposition", contentDisposition.ToString());
			return File(blob.File, blob.ContentType);
		}

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (CustomerDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(CustomerDynamic));
                var model = _customerMapper.ToModel<AddUpdateCustomerModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (CustomerDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(CustomerDynamic));
                var model = _customerMapper.ToModel<AddUpdateCustomerModel>(dynamic);
                toReturn.Before.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<ICollection<string>>> GetCustomerStaticFieldValuesByValue([FromBody]ValuesByFieldValueFilter filter)
        {
            return (await _customerService.GetCustomerStaticFieldValuesByValue(filter)).ToList();
        }

        [HttpPost]
        public async Task<Result<ICollection<string>>> GetProfileAddressFieldValuesByValueAsync([FromBody]ValuesByFieldValueFilter filter)
        {
            filter.IdReferencedObjectType = (int)AddressType.Profile;
            return (await _customerService.GetAddressFieldValuesByValueAsync(filter)).ToList();
        }
    }
}