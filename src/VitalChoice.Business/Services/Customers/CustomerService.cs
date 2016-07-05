using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Business.FedEx.Ship;
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
using VitalChoice.DynamicData.Extensions;
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
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;
using Address = VitalChoice.Ecommerce.Domain.Entities.Addresses.Address;

namespace VitalChoice.Business.Services.Customers
{
    public class CustomerService : ExtendedEcommerceDynamicService<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>,
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
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IEcommerceRepositoryAsync<VWholesaleSummaryInfo> _vWholesaleSummaryInfoRepositoryAsync;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly SpEcommerceRepository _sPEcommerceRepository;

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
            ILoggerProviderExtended loggerProvider, DynamicExtensionsRewriter queryVisitor,
            AddressOptionValueRepository addressOptionValueRepositoryAsync, CustomerAddressMapper customerAddressMapper,
            ICountryNameCodeResolver countryNameCode, IEncryptedOrderExportService encryptedOrderExportService,
            IPaymentMethodService paymentMethodService,
            IEcommerceRepositoryAsync<VWholesaleSummaryInfo> vWholesaleSummaryInfoRepositoryAsync,
            IAppInfrastructureService appInfrastructureService,
            SpEcommerceRepository sPEcommerceRepository,
            ITransactionAccessor<EcommerceContext> transactionAccessor, IDynamicEntityOrderingExtension<Customer> orderingExtension)
            : base(
                customerMapper, customerRepositoryAsync,
                customerOptionValueRepositoryAsync, bigStringRepositoryAsync, objectLogItemExternalService, loggerProvider, 
                queryVisitor, transactionAccessor, orderingExtension)
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
            _paymentMethodService = paymentMethodService;
            _vWholesaleSummaryInfoRepositoryAsync = vWholesaleSummaryInfoRepositoryAsync;
            _appInfrastructureService = appInfrastructureService;
            _sPEcommerceRepository = sPEcommerceRepository;
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

            if (!string.IsNullOrEmpty(model.Email) && model.StatusCode == (int) CustomerStatus.Active)
            {
                var customerSameEmail =
                    await
                        _customerRepositoryAsync.Query(
                            new CustomerQuery().Active().WithEmail(model.Email))
                            .Include(c => c.OptionValues)
                            .SelectAsync(false);

                if (customerSameEmail.Count > 0 && customerSameEmail.All(c => c.Id != model.Id))
                {
                    throw new AppValidationException(
                        string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.EmailIsTakenAlready], model.Email));
                }
            }

            if (model.ShippingAddresses.Where(x => x.StatusCode != (int)RecordStatusCode.Deleted).All(x => !x.Data.Default))
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
                if (!exist)
                {
                    throw new AppValidationException("IdAffiliate", ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidIdAffiliate]);
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

            if (model.Files != null && model.Files.Count > 0 && entity.Files != null)
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

        protected override async Task AfterEntityChangesAsync(CustomerDynamic model, Customer updated, 
            IUnitOfWorkAsync uow)
        {
            var customerToShippingRepository = uow.RepositoryAsync<CustomerToShippingAddress>();

            await SyncDbCollections<Address, AddressOptionValue>(uow, updated.ShippingAddresses.Select(s => s.ShippingAddress), true);

            await
                customerToShippingRepository.DeleteAllAsync(
                    updated.ShippingAddresses.Where(s => s.ShippingAddress.StatusCode == (int) RecordStatusCode.Deleted));

            await
                customerToShippingRepository.InsertGraphRangeAsync(
                    updated.ShippingAddresses.Where(s => s.ShippingAddress.StatusCode != (int) RecordStatusCode.Deleted && s.IdAddress == 0));

            await SyncDbCollections<CustomerNote, CustomerNoteOptionValue>(uow, updated.CustomerNotes, true);

            await SyncDbCollections<CustomerPaymentMethod, CustomerPaymentMethodOptionValue>(uow, updated.CustomerPaymentMethods, true);

            await SyncDbCollections<Address, AddressOptionValue>(uow, updated.CustomerPaymentMethods.Select(p => p.BillingAddress), true);

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
                await Task.WhenAll(fileNamesForDelete.Select(fileName => DeleteFileAsync(fileName, publicId.ToString())));
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

        public async Task<bool> GetCustomerHasAffiliateOrders(int idCustomer, int? excludeIdOrder = null)
        {
            if (excludeIdOrder.HasValue && excludeIdOrder.Value > 0)
            {
                return await _orderRepository.Query(
                    o =>
                        o.IdCustomer == idCustomer && o.OrderStatus != OrderStatus.Incomplete &&
                        o.OrderStatus != OrderStatus.Cancelled && o.IdObjectType == (int)OrderType.Normal &&
                        o.AffiliateOrderPayment.Id != excludeIdOrder.Value && o.AffiliateOrderPayment.Id > 0)
                    .Include(o => o.AffiliateOrderPayment).SelectAnyAsync();
            }
            return await _orderRepository.Query(
                o =>
                    o.IdCustomer == idCustomer && o.OrderStatus != OrderStatus.Incomplete &&
                    o.OrderStatus != OrderStatus.Cancelled && o.IdObjectType == (int)OrderType.Normal && o.AffiliateOrderPayment.Id > 0)
                .Include(o => o.AffiliateOrderPayment).SelectAnyAsync();
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

            return idStatus != 0 ? (CustomerStatus?)idStatus : null;
        }

        public async Task<PagedList<ExtendedVCustomer>> GetCustomersAsync(CustomerFilter filter)
        {
            Func<IQueryable<Customer>, IOrderedQueryable<Customer>> sortable = x => x.OrderByDescending(y => y.DateEdited);
            //Func<IEnumerable<CustomerDynamic>, IOrderedEnumerable<CustomerDynamic>> sortDynamic = null;
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VCustomerSortPath.Id:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case VCustomerSortPath.IdObjectType:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.IdObjectType)
                                : x.OrderByDescending(y => y.IdObjectType);
                    break;
                case VCustomerSortPath.Email:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Email)
                                : x.OrderByDescending(y => y.Email);
                    break;
                case VCustomerSortPath.Updated:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
                case VCustomerSortPath.Status:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.StatusCode)
                                : x.OrderByDescending(y => y.StatusCode);
                    break;
            }
            var customers =
                await
                    SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount,
                        new CustomerQuery().WithIdContains(filter.IdContains)
                            .NotDeleted()
                            .WithId(filter.SearchText)
                            .WithEmailContains(filter.Email)
                            .WithIdAffiliate(filter.IdAffiliate, filter.IdAffiliateRequired)
                            .FilterProfileAddress(filter.Address)
                            .FilterDefaultShippingAddress(filter.DefaultShippingAddress),
                        includes =>
                            includes.Include(c => c.ProfileAddress)
                                .ThenInclude(c => c.OptionValues)
                                .Include(p=>p.ShippingAddresses)
                                .ThenInclude(p=>p.ShippingAddress)
                                .ThenInclude(p=>p.OptionValues), orderBy: sortable, withDefaults: true);

            var adminProfileCondition =
                new AdminProfileQuery().IdInRange(
                    customers.Items.Where(x => x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList());

            var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).SelectAsync(false);

            IEnumerable<CustomerDynamic> orderedCustomers= customers.Items;

            var resultList = new List<ExtendedVCustomer>(customers.Items.Count);

            foreach (var item in orderedCustomers)
            {
                var newItem = new ExtendedVCustomer
                {
                    AdminProfile = adminProfiles.SingleOrDefault(y => y.Id == item.IdEditedBy),
                    IdEditedBy = item.IdEditedBy,
                    DateEdited = item.DateEdited,
                    IdObjectType = (CustomerType) item.IdObjectType,
                    Id = item.Id,
                    Email = item.Email,
                    County = item.ProfileAddress.County,
                    StatusCode = item.StatusCode,
                };
                var defaultShippingAddress = item.ShippingAddresses.FirstOrDefault(p => p.SafeData.Default == true);
                if (defaultShippingAddress != null)
                {
                    newItem.CountryCode = _countryNameCode.GetCountryCode(defaultShippingAddress);
                    newItem.StateCode = _countryNameCode.GetStateCode(defaultShippingAddress);
                    newItem.StateName = _countryNameCode.GetStateName(defaultShippingAddress);
                    newItem.CountryName = _countryNameCode.GetCountryName(defaultShippingAddress);
                    newItem.StateOrCounty = _countryNameCode.GetRegionOrStateCode(defaultShippingAddress);
                }
                await _customerAddressMapper.UpdateModelAsync(newItem, item.ProfileAddress);
                if (defaultShippingAddress != null)
                {
                    newItem.Address1 = defaultShippingAddress.SafeData.Address1;
                    newItem.City= defaultShippingAddress.SafeData.City;
                    newItem.Zip= defaultShippingAddress.SafeData.Zip;
                }
                resultList.Add(newItem);
            }

            var result = new PagedList<ExtendedVCustomer>
            {
                Items = resultList,
                Count = customers.Count
            };

            return result;
        }

        public async Task<ICollection<CustomerOrderStatistic>> GetCustomerOrderStatistics(ICollection<int> ids)
        {
            return await _orderRepository.GetCustomerOrderStatistics(ids);
        }

        public async Task<string> UploadFileAsync(byte[] file, string fileName, Guid customerPublicId, string contentType = null)
        {
            var i = 0;
            var customerId = customerPublicId.ToString("D");
            string generatedFileName;
            var items = await _storageClient.GetBlobItems(_customerContainerName, customerId);
            do
            {
                generatedFileName = (i != 0 ? (i + "_") : string.Empty) + fileName;
                i++;
            } while (items.Contains(generatedFileName));

            await _storageClient.UploadBlobAsync(_customerContainerName, $"{customerId}/{generatedFileName}", file, contentType);

            return generatedFileName;
        }

        public async Task<Blob> DownloadFileAsync(string fileName, string customerPublicId)
        {
            return await _storageClient.DownloadBlobBlockAsync(_customerContainerName, $"{customerPublicId}/{fileName}");
        }

        public async Task<ICollection<string>> GetAddressFieldValuesByValueAsync(ValuesByFieldValueFilter filter)
        {
            ICollection<string> toReturn = new List<string>();
            var fields = _customerAddressMapper.OptionTypes.Where(p => p.Name == filter.FieldName).ToList();
            if (fields.Count > 0)
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
                case (int) CustomerStatus.Active:
                    appUser.Status = UserStatus.Active;
                    break;
                case (int) CustomerStatus.PhoneOnly:
                    appUser.Status = UserStatus.NotActive;
                    break;
                case (int) CustomerStatus.Suspended:
                    appUser.Status = UserStatus.Disabled;
                    break;
                case (int) CustomerStatus.Deleted:
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

            Customer entity;
            var paymentCopies =
                model.CustomerPaymentMethods.Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard)
                    .Select(method => new
                    {
                        method.SafeData.CardNumber,
                        Model = method
                    }).ToArray();
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
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

                    transaction.Commit();

                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            Task.Run(async () =>
            {
                if (!await _encryptedOrderExportService.UpdateCustomerPaymentMethodsAsync(paymentCopies.Select(p => new CustomerCardData
                {
                    CardNumber = p.CardNumber,
                    IdPaymentMethod = p.Model.Id,
                    IdCustomer = p.Model.IdCustomer
                }).ToArray()))
                {
                    Logger.LogError("Cannot update customer payment info on remote.");
                }
            }).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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

                    var paymentCopies =
                        model.CustomerPaymentMethods.Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard).Select(method => new
                        {
                            method.SafeData.CardNumber,
                            Model = method
                        }).ToArray();
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

                    model.CustomerPaymentMethods.ForEach(p => p.IdCustomer = entity.Id);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    Task.Run(async () =>
                    {
                        updatePaymentsTask =
                            _encryptedOrderExportService.UpdateCustomerPaymentMethodsAsync(paymentCopies.Select(p => new CustomerCardData
                            {
                                CardNumber = p.CardNumber,
                                IdPaymentMethod = p.Model.Id,
                                IdCustomer = p.Model.IdCustomer
                            }).ToArray());

                        if (!await updatePaymentsTask)
                        {
                            Logger.LogError("Cannot update customer payment info on remote.");
                        }
                    }).ConfigureAwait(false);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

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
            return SelectFirstAsync(c => c.Email == email && c.StatusCode != (int)RecordStatusCode.Deleted, withDefaults: true);
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

                    customer.StatusCode = (int)CustomerStatus.Active;
                    await UpdateAsync(customer);

                    customerUpdated = true;

                    applicationUser = await _storefrontUserService.ResetPasswordAsync(email, token, newPassword);

                    tran.Commit();

                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    if (customerUpdated) //this needs to be done since distributed transactions not supported yet
                                         //todo: refactor this once distributed transactions arrive
                    {
                        applicationUser = await _storefrontUserService.FindAsync(email);

                        applicationUser.Status = UserStatus.NotActive;

                        await _storefrontUserService.UpdateAsync(applicationUser);
                    }
                    tran.Rollback();
                    throw;
                }
            }

            await _storefrontUserService.SendSuccessfulRegistration(customer.Email, applicationUser.FirstName, applicationUser.LastName);
        }

        public async Task UpdateEcommerceOnlyAsync(CustomerDynamic model)
        {
            var uow = CreateUnitOfWork();
            await base.UpdateAsync(model, uow);
        }

        #region Reports 

        public async Task<WholesaleSummaryReport> GetWholesaleSummaryReportAsync()
        {
            WholesaleSummaryReport toReturn = new WholesaleSummaryReport();
            toReturn.TradeClasses = _appInfrastructureService.Data().TradeClasses.Select(p => new WholesaleSummaryReportTradeClassItem()
            {
                Id = p.Key,
                Name = p.Text
            }).ToList();

            var items = await _vWholesaleSummaryInfoRepositoryAsync.Query().SelectAsync(false);
            toReturn.AllAccounts = items.Count();
            toReturn.ActiveAccounts = items.Count(p => p.OrdersExist);
            toReturn.NewActiveAccounts = items.Count(p => p.OrdersExist && p.NewCustomer);
            foreach (var wholesaleSummaryReportTradeClassItem in toReturn.TradeClasses)
            {
                wholesaleSummaryReportTradeClassItem.Count =
                    items.Count(p => p.TradeClass == wholesaleSummaryReportTradeClassItem.Id);
            }

            return toReturn;
        }

        public async Task<ICollection<WholesaleSummaryReportMonthStatistic>> GetWholesaleSummaryReportMonthStatisticAsync(DateTime lastMonthStartDay, int monthCount)
        {
            List<WholesaleSummaryReportMonthStatistic> toReturn = new List<WholesaleSummaryReportMonthStatistic>();

            var items = await _vWholesaleSummaryInfoRepositoryAsync.Query().SelectAsync(false);
            var newIds = items.Where(p => p.NewCustomer && p.OrdersExist).Select(p => p.Id).ToList();
            var establishedIds = items.Where(p => !p.NewCustomer && p.OrdersExist).Select(p => p.Id).ToList();

            OrderQuery query = new OrderQuery().NotDeleted().WithActualStatusOnly().WithCreatedDate(lastMonthStartDay.AddMonths(-monthCount), null).
                WithOrderTypes(new[] { OrderType.AutoShipOrder, OrderType.DropShip, OrderType.GiftList, OrderType.Normal });

            OrderQuery newIdsQuery = query.WithCustomerIds(newIds);
            var newOrders = await _orderRepository.Query(newIdsQuery).SelectAsync(p => new { p.DateCreated, p.Total }, false);

            OrderQuery stablishedIdsQuery = query.WithCustomerIds(establishedIds);
            var establishedOrders = await _orderRepository.Query(stablishedIdsQuery).SelectAsync(p => new { p.DateCreated, p.Total }, false);

            while (monthCount > 0)
            {
                WholesaleSummaryReportMonthStatistic item = new WholesaleSummaryReportMonthStatistic();
                item.Month = lastMonthStartDay.ToString("MMMM", CultureInfo.InvariantCulture);

                item.NewSales = newOrders.Where(p => lastMonthStartDay <= p.DateCreated && lastMonthStartDay.AddMonths(1) >= p.DateCreated).Sum(p => p.Total);
                item.EstablishedSales = establishedOrders.Where(p => lastMonthStartDay <= p.DateCreated && lastMonthStartDay.AddMonths(1) >= p.DateCreated).Sum(p => p.Total);
                item.Total = item.NewSales + item.EstablishedSales;

                toReturn.Add(item);
                lastMonthStartDay = lastMonthStartDay.AddMonths(-1);
                monthCount--;
            }

            return toReturn;
        }

        public async Task<PagedList<WholesaleListitem>> GetWholesalesAsync(WholesaleFilter filter)
        {
            PagedList<WholesaleListitem> toReturn = new PagedList<WholesaleListitem>();

            CustomerQuery conditions =new CustomerQuery().NotDeleted().NotPending().WithType(CustomerType.Wholesale)
                .FilterWholesaleOptions(filter.IdTradeClass,filter.IdTier);

            if (filter.OnlyActive)
            {
                var summaryItems = await _vWholesaleSummaryInfoRepositoryAsync.Query().SelectAsync(false);
                var activeIds = summaryItems.Where(p => p.OrdersExist).Select(p => p.Id).ToList();
                conditions = conditions.WithIds(activeIds);
            }

            ICollection<CustomerDynamic> dbItems = null;
            int count;
            if (filter.Paging != null)
            {
                var result = await this.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount,
                    conditions, p => p.Include(pp => pp.ProfileAddress).ThenInclude(pp => pp.OptionValues));
                dbItems = result.Items;
                count = result.Count;
            }
            else
            {
                var result = await this.SelectAsync(conditions,
                    p => p.Include(pp => pp.ProfileAddress).ThenInclude(pp => pp.OptionValues));
                dbItems = result;
                count = result.Count;
            }
            dbItems.ForEach(p =>
            {
                var item = new WholesaleListitem()
                {
                    Id = p.Id,
                    DateCreated = p.DateCreated,
                    Email = p.Email,
                    Company = p.ProfileAddress.SafeData.Company,
                    FirstName = p.ProfileAddress.SafeData.FirstName,
                    LastName = p.ProfileAddress.SafeData.LastName,
                    Phone = p.ProfileAddress.SafeData.Phone,
                    IdTradeClass = p.SafeData.TradeClass,
                    IdTier = p.SafeData.Tier,
                };
                item.TradeClass = item.IdTradeClass.HasValue ? _appInfrastructureService.Data().TradeClasses.FirstOrDefault(pp => item.IdTradeClass.Value == pp.Key)?.Text : null;
                item.Tier = item.IdTier.HasValue ? _appInfrastructureService.Data().Tiers.FirstOrDefault(pp => item.IdTier.Value == pp.Key)?.Text : null;


                toReturn.Items.Add(item);
            });
            toReturn.Count = count;

            var ids = toReturn.Items.Select(p => p.Id).ToList();

            var now = DateTime.Now;
            var threeMonthSales = await _sPEcommerceRepository.GetCustomersStandardOrderTotalsAsync(ids, now.AddDays(-90), now);
            var yearSales = await _sPEcommerceRepository.GetCustomersStandardOrderTotalsAsync(ids, now.AddYears(-1), now);
            var lastOrders = await _sPEcommerceRepository.GetCustomersStandardOrdersLastAsync(ids);
            toReturn.Items.ForEach(p =>
            {
                var sales = threeMonthSales.FirstOrDefault(pp => pp.Id == p.Id);
                p.SalesLastThreeMonths = sales?.Total ?? 0;

                sales = yearSales.FirstOrDefault(pp => pp.Id == p.Id);
                p.SalesLastYear = sales?.Total ?? 0;

                var lastOrderDate = lastOrders.FirstOrDefault(pp => pp.Id == p.Id);
                p.LastOrderDate = lastOrderDate?.LastOrderDate;
            });

            return toReturn;
        }

        #endregion
    }
}
