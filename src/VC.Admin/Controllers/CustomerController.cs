using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using VC.Admin.Models.Customer;
using VC.Admin.Models.Setting;
using VitalChoice.Business.Queries.User;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VC.Admin.Models.Customers;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Business.CsvExportMaps.Customers;
using VitalChoice.Business.Queries.Users;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Payments;

namespace VC.Admin.Controllers
{
    public class CustomerController : BaseApiController
    {
        private readonly ICountryService _countryService;
        private readonly IGenericService<AdminProfile> _adminProfileService;
	    private readonly IDynamicMapper<CustomerDynamic, Customer> _customerMapper;
        private readonly IDynamicMapper<AddressDynamic, Address> _addressMapper;
        private readonly IDynamicMapper<CustomerNoteDynamic, CustomerNote> _noteMapper;
        private readonly ICustomerService _customerService;
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly Country _defaultCountry;
        private readonly ICsvExportService<ExtendedVCustomer, CustomersForAffiliatesCsvMap> _csvExportCustomersForAffiliatesService;
        private readonly ICsvExportService<WholesaleListitem, WholesaleListitemCsvMap> _csvExportWholesaleListitemService;

        private readonly IDynamicServiceAsync<AddressDynamic, Address> _addressService;
        private readonly IDynamicServiceAsync<CustomerNoteDynamic, CustomerNote> _notesService;

		private readonly ILogger logger;
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly ExtendedUserManager _userManager;

        public CustomerController(ICustomerService customerService,
            IDynamicMapper<CustomerDynamic, Customer> customerMapper,
            IDynamicMapper<AddressDynamic, Address> addressMapper, ICountryService countryService,
            IGenericService<AdminProfile> adminProfileService, IDynamicServiceAsync<AddressDynamic, Address> addressService,
            IDynamicServiceAsync<CustomerNoteDynamic, CustomerNote> notesService,
            IDynamicMapper<CustomerNoteDynamic, CustomerNote> noteMapper, ILoggerProviderExtended loggerProvider,
            IStorefrontUserService storefrontUserService,
            IAppInfrastructureService appInfrastructureService,
            IObjectHistoryLogService objectHistoryLogService,
            ICsvExportService<ExtendedVCustomer, CustomersForAffiliatesCsvMap> csvExportCustomersForAffiliatesService,
            ICsvExportService<WholesaleListitem, WholesaleListitemCsvMap> csvExportWholesaleListitemService,
            IPaymentMethodService paymentMethodService, ExtendedUserManager userManager)
        {
            _customerService = customerService;
            _countryService = countryService;
            _adminProfileService = adminProfileService;
	        _customerMapper = customerMapper;
            _addressMapper = addressMapper;
            _addressService = addressService;
            _notesService = notesService;
            _noteMapper = noteMapper;
            this.logger = loggerProvider.CreateLogger<CustomerController>();
	        _storefrontUserService = storefrontUserService;
            _objectHistoryLogService = objectHistoryLogService;
            _defaultCountry = appInfrastructureService.Data().DefaultCountry;
            _csvExportCustomersForAffiliatesService = csvExportCustomersForAffiliatesService;
            _csvExportWholesaleListitemService = csvExportWholesaleListitemService;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
	        _paymentMethodService = paymentMethodService;
            _userManager = userManager;
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
        [AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<AddUpdateCustomerModel>> CreateCustomerPrototype()
        {
            var model = await _customerService.Mapper.CreatePrototypeForAsync<AddUpdateCustomerModel>((int)CustomerType.Retail);
            model.PublicId = Guid.NewGuid();
            model.TaxExempt = TaxExempt.YesCurrentCertificate;
            model.Tier = Tier.Tier1;
            model.InceptionDate = DateTime.Now;
            model.TradeClass = 1;
            model.CustomerNotes = new List<CustomerNoteModel>();
            model.ProfileAddress = new AddressModel() { };
            model.ProfileAddress.Country = new CountryListItemModel(_defaultCountry);
            model.Shipping = new List<AddressModel>()
            { new AddressModel()
                {
                    Country = new CountryListItemModel(_defaultCountry),
                    PreferredShipMethod = PreferredShipMethod.Best,
                    ShippingAddressType = ShippingAddressType.Residential,
                }
            };
            model.StatusCode = (int)CustomerStatus.PhoneOnly;

	        var defaultPaymentMethodId = (await _paymentMethodService.GetStorefrontDefaultPaymentMethod()).Id;

			model.ApprovedPaymentMethods = new List<int>() { defaultPaymentMethodId };
            model.DefaultPaymentMethod = defaultPaymentMethodId; 
            return model;
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public Result<CreditCardModel> CreateCreditCardPrototype()
        {
            return new CreditCardModel
            {
                Address = new AddressModel { Country = new CountryListItemModel(_defaultCountry) },
                CardType = CreditCardType.MasterCard
            };
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public Result<OacPaymentModel> CreateOacPrototype()
        {
            return new OacPaymentModel
            {
                Address = new AddressModel { Country = new CountryListItemModel(_defaultCountry) },
                Fob = 1,
                Terms = 1
            };
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public Result<CheckPaymentModel> CreateCheckPrototype()
        {
            return new CheckPaymentModel
            {
                Address = new AddressModel {Country = new CountryListItemModel(_defaultCountry) }
            };
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public Result<WireTransferPaymentModel> CreateWireTransferPrototype()
        {
            return new WireTransferPaymentModel
            {
                Address = new AddressModel { Country = new CountryListItemModel(_defaultCountry) }
            };
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public Result<MarketingPaymentModel> CreateMarketingPrototype()
        {
            return new MarketingPaymentModel
            {
                Address = new AddressModel { Country = new CountryListItemModel(_defaultCountry) }
            };
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public Result<VCWellnessEmployeeProgramPaymentModel> CreateVCWellnessPrototype()
        {
            return new VCWellnessEmployeeProgramPaymentModel
            {
                Address = new AddressModel { Country = new CountryListItemModel(_defaultCountry) }
            };
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public Result<AddressModel> CreateAddressPrototype()
        {
            return new AddressModel()
            {
                Country = new CountryListItemModel(_defaultCountry),
                PreferredShipMethod = PreferredShipMethod.Best,
                ShippingAddressType = ShippingAddressType.Residential,
            };
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<CustomerNoteModel>> CreateCustomerNotePrototype()
        {
            var toReturn = new CustomerNoteModel()
            {
                Priority = CustomerNotePriority.NormalPriority,
                DateCreated = DateTime.Now,
                DateEdited = DateTime.Now,
            };
            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                var adminProfileCondition = new AdminProfileQuery().WithId(userId);
                var adminProfile = (await _adminProfileService.QueryAsync(adminProfileCondition)).FirstOrDefault();
                if (adminProfile != null)
                {
                    toReturn.IdEditedBy = userId;
                    toReturn.EditedBy = adminProfile.AgentId;
                    toReturn.IdAddedBy = userId;
                    toReturn.AddedBy = adminProfile.AgentId;
                }
            }

            return toReturn;
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<CustomerNoteModel>> AddNote([FromBody] CustomerNoteModel model, int idCustomer)
        {
            if (!Validate(model))
                return null;
            var note = await _noteMapper.FromModelAsync(model);
            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (int.TryParse(sUserId, out userId))
            {
                note.IdEditedBy = userId;
                note.IdCustomer = idCustomer;
            }
            note = await _notesService.InsertAsync(note);
            var toReturn = await _noteMapper.ToModelAsync<CustomerNoteModel>(note);
            return toReturn;
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<bool>> DeleteNote(int idNote)
        {
            if (idNote > 0)
                return await _notesService.DeleteAsync(idNote, true);
            throw new AppValidationException("Note cannot be deleted.");
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<AddressModel>> AddAddress([FromBody] AddressModel model, int idCustomer)
        {
            if (!Validate(model))
                return null;
            var address = await _addressMapper.FromModelAsync(model);
            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (int.TryParse(sUserId, out userId))
            {
                address.IdEditedBy = userId;
            }
            address = await _addressService.InsertAsync(address);
            var toReturn = await _addressMapper.ToModelAsync<AddressModel>(address);
            return toReturn;
        }

        [HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<bool>> DeleteAddress(int idAddress)
        {
            if (idAddress > 0)
                return await _addressService.DeleteAsync(idAddress, true);
            throw new AppValidationException("Please select address to delete.");
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<AddUpdateCustomerModel>> AddUpdateCustomer(
            [FromBody] AddUpdateCustomerModel addUpdateCustomerModel)
        {
            if (!Validate(addUpdateCustomerModel))
                return null;
            var customer = await _customerMapper.FromModelAsync(addUpdateCustomerModel, (int) CustomerType.Retail);
            var sUserId = _userManager.GetUserId(User);
            int userId;
            if (int.TryParse(sUserId, out userId))
            {
                customer.IdEditedBy = userId;
                foreach (var address in customer.ShippingAddresses)
                {
                    address.IdEditedBy = userId;
                }
                customer.ProfileAddress.IdEditedBy = userId;
            }
            customer.IdEditedBy = userId;
            if (customer.Id > 0)
            {
                customer = await _customerService.UpdateAsync(customer);
            }
            else
            {
                customer = await _customerService.InsertAsync(customer);

                if (customer.StatusCode != (int)CustomerStatus.Suspended && !String.IsNullOrEmpty(customer.Email))
                {
                    await _storefrontUserService.SendActivationAsync(customer.Email);
                }
            }
            var toReturn = await _customerMapper.ToModelAsync<AddUpdateCustomerModel>(customer);

			toReturn.IsConfirmed = addUpdateCustomerModel.IsConfirmed;
			toReturn.PublicUserId = addUpdateCustomerModel.PublicUserId;

            await PrepareCustomerNotes(customer, toReturn);

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
                    IdObjectType = p.IdObjectType,
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

            var statistic = await _customerService.GetCustomerOrderStatistics(toReturn.Items.Select(p => p.Id).ToList());
            foreach(var statisticItem in statistic)
            {
                var customer = toReturn.Items.FirstOrDefault(p => p.Id == statisticItem.IdCustomer);
                if(customer!=null)
                {
                    customer.TotalOrders = statisticItem.TotalOrders;
                    customer.FirstOrderPlaced = statisticItem.FirstOrderPlaced;
                    customer.LastOrderPlaced = statisticItem.LastOrderPlaced;
                }
            }

            return toReturn;
        }

        [HttpGet]
        public async Task<FileResult> GetCustomersForAffiliates([FromQuery]int idaffiliate)
        {
            IList<ExtendedVCustomer> items = new List<ExtendedVCustomer>();
            if (idaffiliate != 0)
            {
                CustomerFilter filter = new CustomerFilter()
                {
                    IdAffiliate = idaffiliate
                };
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

            var data = _csvExportCustomersForAffiliatesService.ExportToCsv(items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.AFFILIATE_CUSTOMERS_REPORT, DateTime.Now.ToString("MM_dd_yyyy_hh_mm_ss"))
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }

        [HttpGet]
        //[AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<AddUpdateCustomerModel>> GetExistingCustomer(int id)
        {
            var result = await _customerService.SelectAsync(id, true);
            if (result == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }

            if (result.IdObjectType == (int) CustomerType.Wholesale && result.SafeData.InceptionDate == null)
            {
                result.Data.InceptionDate = DateTime.Now;
            }

            var customerModel = await _customerMapper.ToModelAsync<AddUpdateCustomerModel>(result);

	        var login = await _storefrontUserService.GetAsync(customerModel.Id);
	        if (login == null)
	        {
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
			}

			customerModel.IsConfirmed = login.IsConfirmed;
			customerModel.PublicUserId = login.PublicId;

            await PrepareCustomerNotes(result,customerModel);

            return customerModel;
        }

        [NonAction]
        public async Task PrepareCustomerNotes(CustomerDynamic dynamic, AddUpdateCustomerModel model)
        {
            var noteIds = dynamic.CustomerNotes.Where(x => x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList();
            noteIds.AddRange(dynamic.CustomerNotes.Where(x => x.IdAddedBy.HasValue).Select(x => x.IdAddedBy.Value).ToList());
            var adminProfileCondition = new AdminProfileQuery().IdInRange(noteIds);

            var adminProfiles = await _adminProfileService.QueryAsync(adminProfileCondition);
            foreach (var customerNote in model.CustomerNotes)
            {
                customerNote.EditedBy =
                    adminProfiles.SingleOrDefault(
                        y => y.Id == dynamic.CustomerNotes.Single(z => z.Id == customerNote.Id).IdEditedBy)?
                        .AgentId;
                customerNote.AddedBy =
                    adminProfiles.SingleOrDefault(
                        y => y.Id == dynamic.CustomerNotes.Single(z => z.Id == customerNote.Id).IdAddedBy)?
                        .AgentId;
            }

            model.CustomerNotes = model.CustomerNotes.OrderByDescending(x => x.DateEdited).ToList();
        }

		[HttpPost]
        //[AdminAuthorize(PermissionType.Customers)]
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
        [AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<bool>> ResendActivation(Guid id)
		{
			await _storefrontUserService.ResendActivationAsync(id);

			return true;
		}

		[HttpPost]
        [AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<bool>> ResetPassword(Guid id)
		{
			await _storefrontUserService.SendResetPasswordAsync(id);

			return true;
		}

		[HttpPost]
        [AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<string>> LoginAsCustomer(Guid id)
		{
			var token = await _storefrontUserService.GenerateLoginTokenAsync(id);

			return token;
		}

		[HttpGet]
        [AdminAuthorize(PermissionType.Customers)]
        public async Task<FileResult> GetFile([FromQuery]string publicId, [FromQuery]string fileName, [FromQuery]bool viewMode)
	    {
			var blob = await _customerService.DownloadFileAsync(fileName, publicId);

			var contentDisposition = new ContentDispositionHeaderValue(viewMode ? "inline" : "attachment")
            {
                FileName = fileName
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
			return File(blob.File, blob.ContentType);
		}

        [HttpPost]
        [AdminAuthorize(PermissionType.Customers)]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (CustomerDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(CustomerDynamic));
                var model = await _customerMapper.ToModelAsync<AddUpdateCustomerModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (CustomerDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(CustomerDynamic));
                var model = await _customerMapper.ToModelAsync<AddUpdateCustomerModel>(dynamic);
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

        #region Reports

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<Result<WholesaleSummaryReport>> GetWholesaleSummaryReport()
        {
            return await _customerService.GetWholesaleSummaryReportAsync();
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<Result<ICollection<WholesaleSummaryReportMonthStatistic>>> GetWholesaleSummaryReportMonthStatistic(int count = 3, bool include = true)
        {
            if (count > 12)
                count = 12;
            if (count < 3)
                count = 3;
            if (include)
                count++;
            DateTime lastMonthStartDay = DateTime.Now;
            lastMonthStartDay = new DateTime(lastMonthStartDay.Year, lastMonthStartDay.Month, 1);
            if (!include)
            {
                lastMonthStartDay = lastMonthStartDay.AddMonths(-1);
            }
            lastMonthStartDay = TimeZoneInfo.ConvertTime(lastMonthStartDay, _pstTimeZoneInfo, TimeZoneInfo.Local);
            return (await _customerService.GetWholesaleSummaryReportMonthStatisticAsync(lastMonthStartDay, count)).ToList();
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<PagedList<WholesaleListitem>>> GetWholesales([FromBody]WholesaleFilter filter)
        {
            var toReturn = await _customerService.GetWholesalesAsync(filter);
            return toReturn;
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<FileResult> GetWholesalesReportFile([FromQuery]int? idtradeclass = null, [FromQuery]int? idtier = null, [FromQuery]bool? onlyactive = true)
        {
            WholesaleFilter filter = new WholesaleFilter()
            {
                IdTradeClass = idtradeclass,
                IdTier = idtier,
                OnlyActive = onlyactive ?? true
            };
            filter.Paging = null;

            var data = await _customerService.GetWholesalesAsync(filter);

            var result = _csvExportWholesaleListitemService.ExportToCsv(data.Items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.WHOLESALE_LIST_REPORT, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(result, "text/csv");
        }

        #endregion
    }
}