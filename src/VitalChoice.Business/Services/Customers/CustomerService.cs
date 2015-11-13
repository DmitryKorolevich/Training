using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
#if DNX451
using System.Transactions;
#endif
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Order;
using VitalChoice.Business.Queries.Payment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Helpers;
using VitalChoice.Domain.Transfer.Azure;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Customers;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Interfaces.Services;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using DynamicExpressionVisitor = VitalChoice.DynamicData.Helpers.DynamicExpressionVisitor;
using VitalChoice.Business.Queries.Affiliate;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.Business.Services.Customers
{
    public class CustomerService: EcommerceDynamicService<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>, ICustomerService
    {
	    private readonly IEcommerceRepositoryAsync<OrderNote> _orderNoteRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<User> _userRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<PaymentMethod> _paymentMethodRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<Customer> _customerRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<VCustomer> _vCustomerRepositoryAsync;
	    private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
	    private readonly IBlobStorageClient _storageClient;
	    private readonly IStorefrontUserService _storefrontUserService;
        private readonly IEcommerceRepositoryAsync<Affiliate> _affiliateRepositoryAsync;
        private readonly IOptions<AppOptions> _appOptions;

        private static string _customerContainerName;

	    public CustomerService(IEcommerceRepositoryAsync<OrderNote> orderNoteRepositoryAsync,
            IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepositoryAsync,
            IEcommerceRepositoryAsync<Customer> customerRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, CustomerMapper customerMapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<CustomerOptionValue> customerOptionValueRepositoryAsync,
			IBlobStorageClient storageClient,
			IOptions<AppOptions> appOptions,
			IStorefrontUserService storefrontUserService,
			IEcommerceRepositoryAsync<User> userRepositoryAsync,
            IEcommerceRepositoryAsync<Affiliate> affiliateRepositoryAsync,
            ILoggerProviderExtended loggerProvider, DirectMapper<Customer> directMapper, DynamicExpressionVisitor queryVisitor)
            : base(
                customerMapper, customerRepositoryAsync,
                customerOptionValueRepositoryAsync, bigStringRepositoryAsync, objectLogItemExternalService, loggerProvider, directMapper, queryVisitor)
        {
            _orderNoteRepositoryAsync = orderNoteRepositoryAsync;
            _paymentMethodRepositoryAsync = paymentMethodRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _vCustomerRepositoryAsync = vCustomerRepositoryAsync;
		    _adminProfileRepository = adminProfileRepository;
		    _storageClient = storageClient;
		    _storefrontUserService = storefrontUserService;
		    _customerContainerName = appOptions.Value.AzureStorage.CustomerContainerName;
		    _appOptions = appOptions;
		    _userRepositoryAsync = userRepositoryAsync;
            _affiliateRepositoryAsync = affiliateRepositoryAsync;
        }

        protected override IQueryLite<Customer> BuildQuery(IQueryLite<Customer> query)
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

			var customerSameEmail =
				await
					_customerRepositoryAsync.Query(
						new CustomerQuery().NotDeleted().Excluding(model.Id).WithEmail(model.Email))
						.SelectAsync(false);

			if (customerSameEmail.Any())
			{
				throw new AppValidationException(
					string.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.EmailIsTakenAlready], model.Email));
            }

            if (model.ShippingAddresses.Where(x => x.StatusCode != (int) RecordStatusCode.Deleted).All(x => !x.Data.Default))
            {
                throw new AppValidationException(
                    ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AtLeastOneDefaultShipping]);
            }

            if(model.IdAffiliate.HasValue)
            {
                AffiliateQuery conditions = new AffiliateQuery().NotDeleted().WithDirectId(model.IdAffiliate.Value);
                var exist = await _affiliateRepositoryAsync.Query(conditions).SelectAnyAsync();
                if(!exist)
                {
                    throw new AppValidationException("IdAffiliate",ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidIdAffiliate]);
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
				foreach (var file in model.Files)
				{
					if (dbFile.Id == file.Id)
					{
						delete = false;
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
                await LogItemChanges(new[] { await Mapper.FromEntityAsync(entity) });
                return await Mapper.FromEntityAsync(entity);
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
                await LogItemChanges(new[] { await Mapper.FromEntityAsync(entity) });
                return await Mapper.FromEntityAsync(entity);
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

        public async Task<PagedList<ExtendedVCustomer>> GetCustomersAsync(CustomerFilter filter)
        {


    //        var condition =
				//new VCustomerQuery().NotDeleted()
    //                .WithIdContains(filter.IdContains)
    //                .WithId(filter.SearchText)
    //                .WithIdAffiliate(filter.IdAffiliate, filter.IdAffiliateRequired)
    //                .WithEmail(filter.Email)
    //                .WithAddress1(filter.Address1)
    //                .WithAddress2(filter.Address2)
    //                .WithCity(filter.City)
    //                .WithCompany(filter.Company)
    //                .WithCountry(filter.Country)
    //                .WithFirstName(filter.FirstName)
    //                .WithLastName(filter.LastName)
    //                .WithPhone(filter.Phone)
    //                .WithState(filter.State)
    //                .WithZip(filter.Zip);
            Func<IQueryable<Customer>, IOrderedQueryable<Customer>> sortable = x => x.OrderByDescending(y => y.DateEdited);
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
     //           case VCustomerSortPath.Name:
					//sortable =
					//	(x) =>
					//		sortOrder == SortOrder.Asc
					//			? x.OrderBy(y => y.LastName).ThenBy(y => y.FirstName)
					//			: x.OrderByDescending(y => y.LastName).ThenByDescending(y => y.FirstName);
     //               break;
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
				//case VCustomerSortPath.Country:
				//	sortable =
				//		(x) =>
				//			sortOrder == SortOrder.Asc
				//				? x.OrderBy(y => y.CountryCode)
				//				: x.OrderByDescending(y => y.CountryCode);
				//	break;
				//case VCustomerSortPath.City:
				//	sortable =
				//		(x) =>
				//			sortOrder == SortOrder.Asc
				//				? x.OrderBy(y => y.City)
				//				: x.OrderByDescending(y => y.City);
				//	break;
				//case VCustomerSortPath.State:
				//	sortable =
				//		(x) =>
				//			sortOrder == SortOrder.Asc
				//				? x.OrderBy(y => y.StateOrCounty)
				//				: x.OrderByDescending(y => y.StateOrCounty);
				//	break;
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
                            .WithEmailContains(filter.Email)
                            .WithIdAffiliate(filter.IdAffiliate, filter.IdAffiliateRequired)
                            .FilterAddress(filter.Address),
                        includes =>
                            includes.Include(c => c.ProfileAddress)
                                .ThenInclude(c => c.OptionValues)
                                .Include(c => c.ProfileAddress)
                                .ThenInclude(a => a.Country)
                                .Include(c => c.ProfileAddress)
                                .ThenInclude(a => a.State), orderBy: sortable, withDefaults: true);

            var adminProfileCondition =
                new AdminProfileQuery().IdInRange(
                    customers.Items.Where(x => x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList());

			var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).SelectAsync(false);

            var result = new PagedList<ExtendedVCustomer>
            {
                Items = customers.Items.Select(x => new ExtendedVCustomer()
                {
                    AdminProfile = adminProfiles.SingleOrDefault(y => y.Id == x.IdEditedBy),
                    IdEditedBy = x.IdEditedBy,
                    FirstName = x.ProfileAddress.Data.FirstName,
                    LastName = x.ProfileAddress.Data.LastName,
                    DateEdited = x.DateEdited,
                    IdObjectType = (CustomerType)x.IdObjectType,
                    CountryCode = x.ProfileAddress.Country.CountryCode,
                    StateCode = x.ProfileAddress.State?.StateCode,
                    StateName = x.ProfileAddress.State?.StateName,
                    CountryName = x.ProfileAddress.Country.CountryCode,
                    City = x.ProfileAddress.Data.City,
                    Company = x.ProfileAddress.Data.Company,
                    Id = x.Id,
                    Address1 = x.ProfileAddress.Data.Address1,
                    Address2 = x.ProfileAddress.Data.Address2,
                    Email = x.Email,
                    Phone = x.ProfileAddress.Data.Phone,
                    Zip = x.ProfileAddress.Data.Zip,
                    County = x.ProfileAddress.County,
                    StateOrCounty = x.ProfileAddress.County ?? x.ProfileAddress.State?.StateCode,
                    StatusCode = x.StatusCode,
                    //LastOrderPlaced = x.LastOrderPlaced,
                    //FirstOrderPlaced = x.FirstOrderPlaced,
                    //TotalOrders = x.TotalOrders,
                }).ToList(),
                Count = customers.Count
            };

            return result;
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
                case (int)CustomerStatus.NotActive:
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

            appUser.Email = model.Email;
            appUser.UserName = model.Email;

            var profileAddress = model.ProfileAddress;
            appUser.FirstName = profileAddress.Data.FirstName;
            appUser.LastName = profileAddress.Data.LastName;

            //TODO: Investigate transaction read issues (new transaction allocated with any read on the same connection with overwrite/close current
            //using (var transaction = uow.BeginTransaction())
            //{
            //try
            //{
            var customer = await base.UpdateAsync(model, uow);

            //transaction.Commit();

            var roles = MapCustomerTypeToRole(model);

            await _storefrontUserService.UpdateAsync(appUser, roles, password);

            return customer;
            //}
            //catch (Exception ex)
            //{
            //	transaction.Rollback();
            //	throw;
            //}
            //}
        }

        private async Task<Customer> InsertAsync(CustomerDynamic model, IUnitOfWorkAsync uow, string password)
        {
            var roles = MapCustomerTypeToRole(model);

            var profileAddress = model.ProfileAddress;
            var appUser = new ApplicationUser()
            {
                FirstName = profileAddress.Data.FirstName,
                LastName = profileAddress.Data.LastName,
                Email = model.Email,
                UserName = model.Email,
                TokenExpirationDate = DateTime.Now.AddDays(_appOptions.Value.ActivationTokenExpirationTermDays),
                IsConfirmed = false,
                ConfirmationToken = Guid.NewGuid(),
                IdUserType = UserType.Customer,
                Profile = null,
                Status = UserStatus.NotActive
            };

            var suspendedCustomer = (int)CustomerStatus.Suspended;

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

                    var customer = await base.InsertAsync(model, uow);

                    if (string.IsNullOrWhiteSpace(password) && model.StatusCode != suspendedCustomer)
                    {
                        await _storefrontUserService.SendActivationAsync(model.Email);
                    }

                    transaction.Commit();

                    return customer;
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
        }
    }
}
