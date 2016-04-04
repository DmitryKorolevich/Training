using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Queries.Payment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Interfaces.Services;
using VitalChoice.Data.Services;
using VitalChoice.Business.Queries.Affiliate;
using VitalChoice.Business.Queries.Customers;
using VitalChoice.Business.Queries.Users;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Transaction;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Azure;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Customers
{
    public class CustomerService: ExtendedEcommerceDynamicService<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>,
        ICustomerService
    {
	    private readonly IEcommerceRepositoryAsync<OrderNote> _orderNoteRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<PaymentMethod> _paymentMethodRepositoryAsync;
        private readonly OrderRepository _orderRepository;
        private readonly CustomerRepository _customerRepositoryAsync;
	    private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
	    private readonly IBlobStorageClient _storageClient;
	    private readonly IStorefrontUserService _storefrontUserService;
        private readonly IEcommerceRepositoryAsync<Affiliate> _affiliateRepositoryAsync;
        private readonly IOptions<AppOptions> _appOptions;
        private readonly AddressOptionValueRepository _addressOptionValueRepositoryAsync;
        private readonly CustomerAddressMapper _customerAddressMapper;
        private readonly ICountryNameCodeResolver _countryNameCode;
        private readonly IEncryptedOrderExportService _encryptedOrderExportService;
        private readonly IObjectMapper<CustomerPaymentMethodDynamic> _paymentMapper;
        private readonly IPaymentMethodService _paymentMethodService;

        private static string _customerContainerName;

        public CustomerService(IEcommerceRepositoryAsync<OrderNote> orderNoteRepositoryAsync,
            IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepositoryAsync,
            OrderRepository orderRepository,
            CustomerRepository customerRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, CustomerMapper customerMapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<CustomerOptionValue> customerOptionValueRepositoryAsync,
            IBlobStorageClient storageClient,
            IOptions<AppOptions> appOptions,
            IStorefrontUserService storefrontUserService,
            IEcommerceRepositoryAsync<Affiliate> affiliateRepositoryAsync,
            ILoggerProviderExtended loggerProvider, DirectMapper<Customer> directMapper, DynamicExtensionsRewriter queryVisitor,
            AddressOptionValueRepository addressOptionValueRepositoryAsync, CustomerAddressMapper customerAddressMapper,
            ICountryNameCodeResolver countryNameCode, IEncryptedOrderExportService encryptedOrderExportService,
            IObjectMapper<CustomerPaymentMethodDynamic> paymentMapper, IPaymentMethodService paymentMethodService,
            ITransactionAccessor<EcommerceContext> transactionAccessor)
            : base(
                customerMapper, customerRepositoryAsync,
                customerOptionValueRepositoryAsync, bigStringRepositoryAsync, objectLogItemExternalService, loggerProvider, directMapper,
                queryVisitor, transactionAccessor)
        {
            _orderNoteRepositoryAsync = orderNoteRepositoryAsync;
            _paymentMethodRepositoryAsync = paymentMethodRepositoryAsync;
            _orderRepository = orderRepository;
            _customerRepositoryAsync = customerRepositoryAsync;
            _adminProfileRepository = adminProfileRepository;
            _storageClient = storageClient;
            _storefrontUserService = storefrontUserService;
            _customerContainerName = appOptions.Value.AzureStorage.CustomerContainerName;
            _appOptions = appOptions;
            _affiliateRepositoryAsync = affiliateRepositoryAsync;
            _addressOptionValueRepositoryAsync = addressOptionValueRepositoryAsync;
            _customerAddressMapper = customerAddressMapper;
            _countryNameCode = countryNameCode;
            _encryptedOrderExportService = encryptedOrderExportService;
            _paymentMapper = paymentMapper;
            _paymentMethodService = paymentMethodService;
        }

        protected override IQueryLite<Customer> BuildIncludes(IQueryLite<Customer> query)
        {
            return query
                .Include(p => p.Files)
                .Include(p => p.DefaultPaymentMethod)
                .Include(p => p.User)
                .Include(p => p.ProfileAddress)
                .ThenInclude(p => p.OptionValues)
                .Include(a => a.ShippingAddresses)
                .ThenInclude(a => a.ShippingAddress)
                .ThenInclude(a => a.OptionValues)
                .Include(p => p.CustomerNotes)
                .ThenInclude(n => n.OptionValues)
                .Include(p => p.OrderNotes)
                .Include(p => p.PaymentMethods)
                .Include(p => p.CustomerPaymentMethods)
                .ThenInclude(p => p.OptionValues)
                .Include(p => p.CustomerPaymentMethods)
                .ThenInclude(p => p.BillingAddress)
                .ThenInclude(p => p.OptionValues);
        }

        protected override async Task<List<MessageInfo>> ValidateAsync(CustomerDynamic model)
		{
			var errors = new List<MessageInfo>();

            if (!String.IsNullOrEmpty(model.Email))
            {
                var customerSameEmail =
                    await
                        _customerRepositoryAsync.Query(
                            new CustomerQuery().NotDeleted().Excluding(model.Id).WithEmail(model.Email))
                            .Include(c => c.OptionValues)
                            .SelectAsync(false);

                if (customerSameEmail.Any())
                {
                    throw new AppValidationException(
                        string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.EmailIsTakenAlready], model.Email));
                }
            }

            if (model.ShippingAddresses.Where(x => x.StatusCode != (int) RecordStatusCode.Deleted).All(x => !x.Data.Default))
            {
                throw new AppValidationException(
                    ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AtLeastOneDefaultShipping]);
            }

			if (model.CustomerPaymentMethods.Any(x => x.IdObjectType == (int)PaymentMethodType.CreditCard && x.StatusCode != (int)RecordStatusCode.Deleted) &&
                model.CustomerPaymentMethods.Where(x => x.IdObjectType == (int)PaymentMethodType.CreditCard && x.StatusCode != (int)RecordStatusCode.Deleted).All(x => !x.Data.Default))
			{
				throw new AppValidationException(
					ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AtLeastOneDefaultCreditCard]);
			}

			if (model.IdAffiliate.HasValue)
            {
                AffiliateQuery conditions = new AffiliateQuery().NotDeleted().WithDirectId(model.IdAffiliate.Value);
                var exist = await _affiliateRepositoryAsync.Query(conditions).SelectAnyAsync();
                if(!exist)
                {
                    throw new AppValidationException("IdAffiliate",ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidIdAffiliate]);
                }
            }
            //foreach (var paymentMethod in model.CustomerPaymentMethods.Where(p => p.IdObjectType == (int)PaymentMethodType.CreditCard))
            //{
                
            //}

            //Don't allow to return a customer registered from the admin part(Not Active) to Not Active status from 
            //Active(by a store front activation)
	        if (model.StatusCode == (int)CustomerStatus.PhoneOnly && model.Id > 0)
	        {
				var exists =
				await
					_customerRepositoryAsync.Query(
						new CustomerQuery().NotDeleted().WithId(model.Id).WithStatus(CustomerStatus.Active)).SelectAnyAsync();
		        if (exists)
		        {
					throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CustomerWasModified]);
				}
			}

			return errors;
		}

		protected override async Task BeforeEntityChangesAsync(CustomerDynamic model, Customer entity, IUnitOfWorkAsync uow)
		{
			var customerFileRepositoryAsync = uow.RepositoryAsync<CustomerFile>();

			if (model.Files != null && model.Files.Any() && entity.Files != null)
			{
				//Update
				var toUpdate = entity.Files.Where(e => model.Files.Select(x => x.Id).Contains(e.Id));
				await customerFileRepositoryAsync.UpdateRangeAsync(toUpdate);

				//Delete
				var toDelete = entity.Files.Where(e => model.Files.All(s => s.Id != e.Id));
				await customerFileRepositoryAsync.DeleteAllAsync(toDelete);

				//Insert
				var list = model.Files.Where(s => s.Id == 0).ToList();
				await customerFileRepositoryAsync.InsertRangeAsync(list);
			}
			else if (entity.Files != null)
			{
				foreach (var file in entity.Files)
				{
					await customerFileRepositoryAsync.DeleteAsync(file.Id);
				}
			}
		}

		protected override async Task AfterEntityChangesAsync(CustomerDynamic model, Customer updated, Customer initial, IUnitOfWorkAsync uow)
		{
            var customerToShippingRepository = uow.RepositoryAsync<CustomerToShippingAddress>();

		    await
		        SyncDbCollections<Address, AddressOptionValue>(uow, initial.ShippingAddresses.Select(s => s.ShippingAddress),
		            updated.ShippingAddresses.Select(s => s.ShippingAddress), true);

            await customerToShippingRepository.DeleteAllAsync(
                updated.ShippingAddresses.Where(s => s.ShippingAddress.StatusCode == (int)RecordStatusCode.Deleted));

		    await customerToShippingRepository.InsertGraphRangeAsync(
		        updated.ShippingAddresses.Where(s => s.ShippingAddress.StatusCode != (int) RecordStatusCode.Deleted && s.IdAddress == 0));

            await
		        SyncDbCollections<CustomerNote, CustomerNoteOptionValue>(uow, initial.CustomerNotes, updated.CustomerNotes, true);

		    await
		        SyncDbCollections<CustomerPaymentMethod, CustomerPaymentMethodOptionValue>(uow,
		            initial.CustomerPaymentMethods, updated.CustomerPaymentMethods, true);

            await
                SyncDbCollections<Address, AddressOptionValue>(uow, initial.CustomerPaymentMethods.Select(p => p.BillingAddress),
                    updated.CustomerPaymentMethods.Select(p => p.BillingAddress), true);

            List<string> fileNamesForDelete = new List<string>();
			foreach (var dbFile in updated.Files)
			{
				bool delete = true;
			    if (model.Files != null)
			    {
			        foreach (var file in model.Files)
			        {
			            if (dbFile.Id == file.Id)
			            {
			                delete = false;
			            }
			        }
			    }
			    if (delete)
				{
					fileNamesForDelete.Add(dbFile.FileName);
				}
			}

			if (fileNamesForDelete.Count != 0)
			{
				var publicId = updated.PublicId;
				Task deleteFilesTask = new Task(() =>
				{
					foreach (var fileName in fileNamesForDelete)
					{
						var result = DeleteFileAsync(fileName, publicId.ToString()).Result;
					}
				});
				deleteFilesTask.Start();
			}
		}

        protected override async Task<Customer> InsertAsync(CustomerDynamic model, IUnitOfWorkAsync uow)
        {
            return await InsertAsync(model, uow, null);
	    }

		protected override async Task<Customer> UpdateAsync(CustomerDynamic model, IUnitOfWorkAsync uow)
		{
			return await UpdateAsync(model, uow, null);
		}

        protected override bool LogObjectFullData => true;

        public async Task<CustomerDynamic> InsertAsync(CustomerDynamic model, string password)
		{
            //TODO: lock writing DB until we read result
            using (var uow = CreateUnitOfWork())
			{
				var entity = await InsertAsync(model, uow, password);
                int id = entity.Id;
                entity = await SelectEntityFirstAsync(o => o.Id == id);
                await LogItemChanges(new[] { await DynamicMapper.FromEntityAsync(entity) });
                return await DynamicMapper.FromEntityAsync(entity);
            }
		}

		public async Task<CustomerDynamic> UpdateAsync(CustomerDynamic model, string password)
        {
            //TODO: lock writing DB until we read result
            using (var uow = CreateUnitOfWork())
			{
				var entity = await UpdateAsync(model, uow, password);

                int id = entity.Id;
                entity = await SelectEntityFirstAsync(o => o.Id == id);
                await LogItemChanges(new[] { await DynamicMapper.FromEntityAsync(entity) });
                return await DynamicMapper.FromEntityAsync(entity);
			}
		}

        public async Task<IList<OrderNote>> GetAvailableOrderNotesAsync(CustomerType customerType)
	    {
			var condition = new OrderNoteQuery().NotDeleted().MatchByCustomerType(customerType);

		    return await _orderNoteRepositoryAsync.Query(condition).Include(x => x.CustomerTypes).SelectAsync(false);
	    }

	    public async Task<IList<PaymentMethod>> GetAvailablePaymentMethodsAsync(CustomerType? customerType)
	    {
	        var condition = new PaymentMethodQuery().NotDeleted().MatchByCustomerType(customerType);

		    return await _paymentMethodRepositoryAsync.Query(condition).Include(x => x.CustomerTypes).SelectAsync(false);
		}

        public async Task<CustomerStatus?> GetCustomerStatusAsync(int idCustomer)
        {
            var query = new CustomerQuery().NotDeleted().WithId(idCustomer);
            var idStatus = (await this.ObjectRepository.Query(query).SelectAsync(p => p.StatusCode, false)).FirstOrDefault();

            return idStatus!=0 ? (CustomerStatus?) idStatus : null;
        }

        public async Task<PagedList<ExtendedVCustomer>> GetCustomersAsync(CustomerFilter filter)
        {
            Func<IQueryable<Customer>, IOrderedQueryable<Customer>> sortable = x => x.OrderByDescending(y => y.DateEdited);
            Func<IEnumerable<CustomerDynamic>, IOrderedEnumerable<CustomerDynamic>> sortDynamic = null;
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VCustomerSortPath.Id:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case VCustomerSortPath.IdObjectType:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.IdObjectType)
                                : x.OrderByDescending(y => y.IdObjectType);
                    break;
                case VCustomerSortPath.Name:
                    sortDynamic =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.ProfileAddress.Data.LastName).ThenBy(y => y.ProfileAddress.Data.FirstName)
                                : x.OrderByDescending(y => y.ProfileAddress.Data.LastName)
                                    .ThenByDescending(y => y.ProfileAddress.Data.FirstName);
                    break;
                case VCustomerSortPath.Email:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Email)
                                : x.OrderByDescending(y => y.Email);
                    break;
                case VCustomerSortPath.Updated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
                case VCustomerSortPath.Country:
                    sortDynamic =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => _countryNameCode.GetCountryCode(y.ProfileAddress))
                                : x.OrderByDescending(y => _countryNameCode.GetCountryCode(y.ProfileAddress));
                    break;
                case VCustomerSortPath.City:
                    sortDynamic =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.ProfileAddress.Data.City)
                                : x.OrderByDescending(y => y.ProfileAddress.Data.City);
                    break;
                case VCustomerSortPath.State:
                    sortDynamic =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => _countryNameCode.GetRegionOrStateCode(y.ProfileAddress)).ThenBy(y => y.ProfileAddress.County)
                                : x.OrderByDescending(y => _countryNameCode.GetRegionOrStateCode(y.ProfileAddress)).ThenByDescending(y => y.ProfileAddress.County);
                    break;
                case VCustomerSortPath.Status:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.StatusCode)
                                : x.OrderByDescending(y => y.StatusCode);
                    break;
            }

            var customers =
                await
                    SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount,
                        new CustomerQuery().WithIdContains(filter.IdContains)
                            .WithId(filter.SearchText)
                            .WithEmailContains(filter.Email)
                            .WithIdAffiliate(filter.IdAffiliate, filter.IdAffiliateRequired)
                            .FilterAddress(filter.Address),
                        includes =>
                            includes.Include(c => c.ProfileAddress)
                                .ThenInclude(c => c.OptionValues), orderBy: sortable, withDefaults: true);

            var adminProfileCondition =
                new AdminProfileQuery().IdInRange(
                    customers.Items.Where(x => x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList());

            var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).SelectAsync(false);

            IEnumerable<CustomerDynamic> orderedCustomers;
            if (sortDynamic != null)
                orderedCustomers = sortDynamic(customers.Items);
            else
                orderedCustomers = customers.Items;

            var result = new PagedList<ExtendedVCustomer>
            {
                Items = orderedCustomers.Select(x => new ExtendedVCustomer
                {
                    AdminProfile = adminProfiles.SingleOrDefault(y => y.Id == x.IdEditedBy),
                    IdEditedBy = x.IdEditedBy,
                    FirstName = x.ProfileAddress.Data.FirstName,
                    LastName = x.ProfileAddress.Data.LastName,
                    DateEdited = x.DateEdited,
                    IdObjectType = (CustomerType) x.IdObjectType,
                    CountryCode = _countryNameCode.GetCountryCode(x.ProfileAddress),
                    StateCode = _countryNameCode.GetStateCode(x.ProfileAddress),
                    StateName = _countryNameCode.GetStateName(x.ProfileAddress),
                    CountryName = _countryNameCode.GetCountryName(x.ProfileAddress),
                    City = x.ProfileAddress.Data.City,
                    Company = x.ProfileAddress.Data.Company,
                    Id = x.Id,
                    Address1 = x.ProfileAddress.Data.Address1,
                    Address2 = x.ProfileAddress.Data.Address2,
                    Email = x.Email,
                    Phone = x.ProfileAddress.Data.Phone,
                    Zip = x.ProfileAddress.Data.Zip,
                    County = x.ProfileAddress.County,
                    StateOrCounty = _countryNameCode.GetRegionOrStateCode(x.ProfileAddress),
                    StatusCode = x.StatusCode,
                }).ToList(),
                Count = customers.Count
            };

            return result;
        }
        
        public async Task<ICollection<CustomerOrderStatistic>> GetCustomerOrderStatistics(ICollection<int> ids)
        {
            return await _orderRepository.GetCustomerOrderStatistics(ids);
        }

        public async Task<string> UploadFileAsync(byte[] file, string fileName, string customerPublicId, string contentType = null)
	    {
		    var i = 0;
            Guid publicId = Guid.Parse(customerPublicId);

            string blobname;
            string generatedFileName;

            do
            {
                generatedFileName = (i != 0 ? (i + "_") : string.Empty) + fileName;
                blobname = $"{customerPublicId}/{generatedFileName}";
                i++;
            } while (await _storageClient.BlobExistsAsync(_customerContainerName, blobname));

            await _storageClient.UploadBlobAsync(_customerContainerName, blobname, file, contentType);

		    return generatedFileName;
	    }

	    public async Task<Blob> DownloadFileAsync(string fileName, string customerPublicId)
	    {
		    return await _storageClient.DownloadBlobAsync(_customerContainerName, $"{customerPublicId}/{fileName}");
	    }
		public async Task<ICollection<string>> GetAddressFieldValuesByValueAsync(ValuesByFieldValueFilter filter)
        {
            ICollection<string> toReturn = new List<string>();
            var fields = _customerAddressMapper.OptionTypes.Where(p => p.Name == filter.FieldName).ToList();
            if (fields != null && fields.Count>0)
            {
                filter.FieldIds = fields.Select(p => p.Id).ToList();
                toReturn = await _addressOptionValueRepositoryAsync.GetAddressFieldValuesByValue(filter);
            }
            return toReturn;
        }

        public async Task<ICollection<string>> GetCustomerStaticFieldValuesByValue(ValuesByFieldValueFilter filter)
        {
            return await _customerRepositoryAsync.GetCustomerStaticFieldValuesByValue(filter);
        }

        private async Task<bool> DeleteFileAsync(string fileName, string customerPublicId)
        {
            return await _storageClient.DeleteBlobAsync(_customerContainerName, $"{customerPublicId}/{fileName}");
        }

        private IList<RoleType> MapCustomerTypeToRole(CustomerDynamic model)
        {
            var roles = new List<RoleType>();
            switch (model.IdObjectType)
            {
                case (int)CustomerType.Retail:
                    roles.Add(RoleType.Retail);
                    break;
                case (int)CustomerType.Wholesale:
                    roles.Add(RoleType.Wholesale);
                    break;
                default:
                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.IncorrectCustomerRole]);
            }

            return roles;
        }

        private async Task<Customer> UpdateAsync(CustomerDynamic model, IUnitOfWorkAsync uow, string password)
        {
            var appUser = await _storefrontUserService.GetAsync(model.Id);
            if (appUser == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
            }

            switch (model.StatusCode)
            {
                case (int)CustomerStatus.Active:
                    appUser.Status = UserStatus.Active;
                    break;
                case (int)CustomerStatus.PhoneOnly:
                    appUser.Status = UserStatus.NotActive;
                    break;
                case (int)CustomerStatus.Suspended:
                    appUser.Status = UserStatus.Disabled;
                    break;
                case (int)CustomerStatus.Deleted:
                    appUser.Status = UserStatus.NotActive;
                    appUser.DeletedDate = DateTime.Now;
                    break;
            }

            if (!string.IsNullOrWhiteSpace(password))
            {
                appUser.IsConfirmed = true;
                appUser.ConfirmationToken = Guid.Empty;
            }

            var email = string.IsNullOrEmpty(password) && string.IsNullOrEmpty(model.Email) &&
                        appUser.Email == BaseAppConstants.FAKE_CUSTOMER_EMAIL
                ? BaseAppConstants.FAKE_CUSTOMER_EMAIL
                : model.Email;
            appUser.Email = email;
            appUser.UserName = email;

            var profileAddress = model.ProfileAddress;
            appUser.FirstName = profileAddress.Data.FirstName;
            appUser.LastName = profileAddress.Data.LastName;

            Task<bool> updatePaymentsTask;
            Customer entity;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {

                    var paymentCopies = model.CustomerPaymentMethods.Select(method => _paymentMapper.Clone<ExpandoObject>(method, o =>
                    {
                        var result = new ExpandoObject();
                        result.AddRange(o);
                        return result;
                    })).ToArray();
                    int[] index = {0};
                    await model.CustomerPaymentMethods.ForEachAsync(async method =>
                    {
                        var errors = await _paymentMethodService.AuthorizeCreditCard(method);
                        errors.Select(error => new MessageInfo
                        {
                            Field = error.Field.FormatCollectionError("CreditCards", index[0]).FormatErrorWithForm("card"),
                            Message = error.Message,
                            MessageLevel = error.MessageLevel
                        }).ToList().Raise();
                        index[0]++;
                    });

                    entity = await base.UpdateAsync(model, uow);

                    var roles = MapCustomerTypeToRole(model);

                    await _storefrontUserService.UpdateAsync(appUser, roles, password);

                    updatePaymentsTask = _encryptedOrderExportService.UpdateCustomerPaymentMethodsAsync(paymentCopies);

                    transaction.Commit();

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            if (!await updatePaymentsTask)
            {
                Logger.LogError("Cannot update order payment info on remote.");
            }
            return entity;
        }

        private async Task<Customer> InsertAsync(CustomerDynamic model, IUnitOfWorkAsync uow, string password)
        {
            var roles = MapCustomerTypeToRole(model);

            var profileAddress = model.ProfileAddress;
            var appUser = new ApplicationUser()
            {
                FirstName = profileAddress.Data.FirstName,
                LastName = profileAddress.Data.LastName,
                Email = string.IsNullOrEmpty(password) && string.IsNullOrEmpty(model.Email) ? BaseAppConstants.FAKE_CUSTOMER_EMAIL : model.Email,
                UserName = string.IsNullOrEmpty(password) && string.IsNullOrEmpty(model.Email) ? BaseAppConstants.FAKE_CUSTOMER_EMAIL : model.Email,
                TokenExpirationDate = DateTime.Now.AddDays(_appOptions.Value.ActivationTokenExpirationTermDays),
                IsConfirmed = false,
                ConfirmationToken = Guid.NewGuid(),
                IdUserType = UserType.Customer,
                Profile = null,
                Status = UserStatus.NotActive
            };

            var suspendedCustomer = (int)CustomerStatus.Suspended;

            Task<bool> updatePaymentsTask;
            Customer entity;

            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(password))
                    {
                        appUser.IsConfirmed = true;
                        appUser.Status = model.StatusCode == suspendedCustomer
                            ? UserStatus.Disabled
                            : UserStatus.Active;
                    }
                    else
                    {
                        appUser.Status = model.StatusCode == suspendedCustomer ? UserStatus.Disabled : UserStatus.NotActive;
                    }

                    appUser = await _storefrontUserService.CreateAsync(appUser, roles, false, false, password);

                    model.Id = appUser.Id;
                    //model.User.Id = appUser.Id;

                    var paymentCopies = model.CustomerPaymentMethods.Select(method => _paymentMapper.Clone<ExpandoObject>(method, o =>
                    {
                        var result = new ExpandoObject();
                        result.AddRange(o);
                        return result;
                    })).ToArray();
                    int index = 0;
                    await model.CustomerPaymentMethods.ForEachAsync(async method =>
                    {
                        index++;
                        var errors = await _paymentMethodService.AuthorizeCreditCard(method);
                        errors.Select(error => new MessageInfo
                        {
                            Field = error.Field.FormatCollectionError("CreditCards", index).FormatErrorWithForm("card"),
                            Message = error.Message,
                            MessageLevel = error.MessageLevel
                        }).ToList().Raise();
                    });

                    entity = await base.InsertAsync(model, uow);

                    updatePaymentsTask = _encryptedOrderExportService.UpdateCustomerPaymentMethodsAsync(paymentCopies);

                    transaction.Commit();
                }
                catch
                {
                    if (appUser.Id > 0)
                    {
                        await _storefrontUserService.DeleteAsync(appUser);
                    }

                    transaction.Rollback();
                    throw;
                }
            }
            if (!await updatePaymentsTask)
            {
                Logger.LogError("Cannot update order payment info on remote.");
            }
            return entity;
        }

        public async Task<string> GetNewOrderNotesBasedOnCustomer(int idCustomer)
        {
            string toReturn = null;
            var customer = await SelectAsync(idCustomer);
            if (customer != null)
            {
                var avaliableOrderNotes = await GetAvailableOrderNotesAsync((CustomerType)customer.IdObjectType);
                toReturn = String.Empty;
                foreach (var IdCustomerOrderNote in customer.OrderNotes)
                {
                    var orderNote = avaliableOrderNotes.FirstOrDefault(p => p.Id == IdCustomerOrderNote);
                    if (orderNote != null)
                    {
                        toReturn += orderNote.Description + Environment.NewLine;
                    }
                }
            }

            return toReturn;
        }

        public Task<CustomerDynamic> GetByEmailAsync(string email)
        {
            return SelectFirstAsync(c => c.Email == email && c.StatusCode != (int) RecordStatusCode.Deleted, withDefaults: true);
        }

	    public async Task ActivateGuestAsync(string email, string token, string newPassword)
	    {
		    CustomerDynamic customer = null;
		    ApplicationUser applicationUser;
			using (var tran = TransactionAccessor.BeginTransaction())
			{
				var customerUpdated = false;
			    try
			    {
				    customer = await SelectFirstAsync(x => x.Email.Equals(email));

				    if (customer == null)
				    {
					    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
				    }

				    customer.StatusCode = (int) CustomerStatus.Active;
					await UpdateAsync(customer);

				    customerUpdated = true;

					applicationUser = await _storefrontUserService.ResetPasswordAsync(email, token, newPassword);

					tran.Commit();

				}
			    catch (Exception)
			    {
					tran.Rollback();
				    if (customerUpdated) //this needs to be done since distributed transactions not supported yet
						//todo: refactor this once distributed transactions arrive
				    {
					    applicationUser = await _storefrontUserService.FindAsync(email);

						applicationUser.Status = UserStatus.NotActive;

						await _storefrontUserService.UpdateAsync(applicationUser);
					}
				    throw;
			    }
		    }

			await _storefrontUserService.SendSuccessfulRegistration(customer.Email, applicationUser.FirstName, applicationUser.LastName);
		}
    }
}
