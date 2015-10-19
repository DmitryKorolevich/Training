using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
#if DNX451
using System.Transactions;
#endif
using Microsoft.Framework.OptionsModel;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Order;
using VitalChoice.Business.Queries.Payment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Dynamic;
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

namespace VitalChoice.Business.Services.Customers
{
    public class CustomerService: EcommerceDynamicObjectService<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>, ICustomerService
    {
	    private readonly IEcommerceRepositoryAsync<OrderNote> _orderNoteRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<User> _userRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<PaymentMethod> _paymentMethodRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<Customer> _customerRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<VCustomer> _vCustomerRepositoryAsync;
	    private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
	    private readonly IBlobStorageClient _storageClient;
	    private readonly IStorefrontUserService _storefrontUserService;
	    private readonly IOptions<AppOptions> _appOptions;

		private static string _customerContainerName;

	    public CustomerService(IEcommerceRepositoryAsync<OrderNote> orderNoteRepositoryAsync,
            IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepositoryAsync,
            IEcommerceRepositoryAsync<Customer> customerRepositoryAsync,
            IEcommerceRepositoryAsync<CustomerOptionType> customerOptionTypeRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, CustomerMapper customerMapper,
            IEcommerceRepositoryAsync<ObjectHistoryLogItem> objectHistoryLogItemRepository,
            IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<CustomerOptionValue> customerOptionValueRepositoryAsync,
			IBlobStorageClient storageClient,
			IOptions<AppOptions> appOptions,
			IStorefrontUserService storefrontUserService,
			IEcommerceRepositoryAsync<User> userRepositoryAsync,
            ILoggerProviderExtended loggerProvider)
            : base(
                customerMapper, customerRepositoryAsync, customerOptionTypeRepositoryAsync,
                customerOptionValueRepositoryAsync, bigStringRepositoryAsync, objectHistoryLogItemRepository, loggerProvider)
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
			}

			appUser.Email = model.Email;
            appUser.UserName = model.Email;

            var profileAddress = model.Addresses.Single(x => x.IdObjectType == (int)AddressType.Profile);
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

			var profileAddress = model.Addresses.Single(x => x.IdObjectType == (int)AddressType.Profile);
            var appUser = new ApplicationUser()
            {
                FirstName = profileAddress.Data.FirstName,
                LastName = profileAddress.Data.LastName,
                Email = model.Email,
				UserName = model.Email,
                TokenExpirationDate = DateTime.Now.AddDays(_appOptions.Value.ActivationTokenExpirationTermDays),
                IsConfirmed = false,
                ConfirmationToken = Guid.NewGuid(),
                IsAdminUser = false,
                Profile = null,
				Status = UserStatus.NotActive
            };

			var suspendedCustomer = (int) CustomerStatus.Suspended;

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
                catch (Exception ex)
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

		protected override async Task<List<MessageInfo>> Validate(CustomerDynamic model)
		{
			var errors = new List<MessageInfo>();

			var customerSameEmail =
				await
					_customerRepositoryAsync.Query(
						new CustomerQuery().NotDeleted().Excluding(model.Id).WithEmail(model.Email))
						.SelectAsync(false);

			if (customerSameEmail.Any())
			{
				errors.AddRange(
					model.CreateError()
						.Property(p => p.Email)
						.Error("Customer email should be unique in the database")
						.Build());
            }

			if (
				model.Addresses.Where(
					x => x.IdObjectType == (int)AddressType.Shipping && x.StatusCode != (int)RecordStatusCode.Deleted)
					.All(x => !x.Data.Default))
			{
				throw new AppValidationException(
					ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AtLeastOneDefaultShipping]);
			}

			return errors;
		}

		protected async override Task BeforeEntityChangesAsync(CustomerDynamic model, Customer entity, IUnitOfWorkAsync uow)
		{
			var customerPaymentMethodOptionValuesRepository = uow.RepositoryAsync<CustomerPaymentMethodOptionValue>();
			var customerToPaymentMethodRepository = uow.RepositoryAsync<CustomerToPaymentMethod>();
			var customerToOrderNoteRepository = uow.RepositoryAsync<CustomerToOrderNote>();
			var addressOptionValuesRepositoryAsync = uow.RepositoryAsync<AddressOptionValue>();
			var customerNoteOptionValuesRepositoryAsync = uow.RepositoryAsync<CustomerNoteOptionValue>();
			var customerFileRepositoryAsync = uow.RepositoryAsync<CustomerFile>();

			await
				customerToPaymentMethodRepository.DeleteAllAsync(
					entity.PaymentMethods.WhereAll(model.ApprovedPaymentMethods, (p, dp) => p.IdPaymentMethod != dp));
			await
				customerToOrderNoteRepository.DeleteAllAsync(entity.OrderNotes.WhereAll(model.OrderNotes,
					(n, dn) => n.IdOrderNote != dn));
			foreach (var address in entity.Addresses)
			{
				await addressOptionValuesRepositoryAsync.DeleteAllAsync(address.OptionValues);
			}
			foreach (var note in entity.CustomerNotes)
			{
				await customerNoteOptionValuesRepositoryAsync.DeleteAllAsync(note.OptionValues);
			}
			foreach (var customerPaymentMethod in entity.CustomerPaymentMethods)
			{
				await customerPaymentMethodOptionValuesRepository.DeleteAllAsync(customerPaymentMethod.OptionValues);
			}

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

		protected async override Task AfterEntityChangesAsync(CustomerDynamic model, Customer entity, IUnitOfWorkAsync uow)
		{
			var customerPaymentMethodRepository = uow.RepositoryAsync<CustomerPaymentMethod>();
			var customerPaymentMethodOptionValuesRepository = uow.RepositoryAsync<CustomerPaymentMethodOptionValue>();
			var addressesRepositoryAsync = uow.RepositoryAsync<Address>();
			var customerNoteRepository = uow.RepositoryAsync<CustomerNote>();

			await
				addressesRepositoryAsync.DeleteAllAsync(
					entity.Addresses.Where(a => a.StatusCode == (int)RecordStatusCode.Deleted));
			await
				customerNoteRepository.DeleteAllAsync(
					entity.CustomerNotes.Where(n => n.StatusCode == (int)RecordStatusCode.Deleted));

			await
				addressesRepositoryAsync.InsertGraphRangeAsync(
					entity.Addresses.Where(
						a =>
							a.Id == 0 && a.IdObjectType != (int?)AddressType.Billing &&
							a.StatusCode != (int)RecordStatusCode.Deleted));
			await
				customerNoteRepository.InsertGraphRangeAsync(
					entity.CustomerNotes.Where(n => n.StatusCode != (int)RecordStatusCode.Deleted && n.Id == 0));

			foreach (var customerPaymentMethod in entity.CustomerPaymentMethods.Where(m => m.StatusCode != (int)RecordStatusCode.Deleted))
			{
				await customerPaymentMethodOptionValuesRepository.InsertRangeAsync(customerPaymentMethod.OptionValues);
				if (customerPaymentMethod.BillingAddress.Id == 0)
				{
					customerPaymentMethod.IdAddress = 0;
					await addressesRepositoryAsync.InsertGraphAsync(customerPaymentMethod.BillingAddress);
				}
			}
			var paymentsToDelete =
				entity.CustomerPaymentMethods.Where(a => a.StatusCode == (int)RecordStatusCode.Deleted).ToList();
			await customerPaymentMethodRepository.DeleteAllAsync(paymentsToDelete);
			await addressesRepositoryAsync.DeleteAllAsync(paymentsToDelete.Where(p => p.BillingAddress != null).Select(p => p.BillingAddress));

			List<string> fileNamesForDelete = new List<string>();
			foreach (var dbFile in entity.Files)
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
				var publicId = entity.PublicId;
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

		protected override IQueryFluent<Customer> BuildQuery(IQueryFluent<Customer> query)
		{
			return query
				.Include(p => p.Files)
				.Include(p => p.DefaultPaymentMethod)
				.Include(p => p.User)
				.ThenInclude(p => p.Customer)
				.Include(a => a.Addresses)
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

		protected override async Task<Customer> InsertAsync(CustomerDynamic model, IUnitOfWorkAsync uow)
        {
            return await InsertAsync(model, uow, null);
	    }

		protected override async Task<Customer> UpdateAsync(CustomerDynamic model, IUnitOfWorkAsync uow)
		{
			return await UpdateAsync(model, uow, null);
		}

        protected override bool LogObject { get { return false; } }

        public async Task<CustomerDynamic> InsertAsync(CustomerDynamic model, string password)
		{
			using (var uow = CreateUnitOfWork())
			{
				var entity = await InsertAsync(model, uow, password);
				return await SelectAsync(entity.Id);
			}
		}

		public async Task<CustomerDynamic> UpdateAsync(CustomerDynamic model, string password)
		{
			using (var uow = CreateUnitOfWork())
			{
				var entity = await UpdateAsync(model, uow, password);
				return await SelectAsync(entity.Id);
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
			var condition =
				new VCustomerQuery().NotDeleted()
                    .WithIdContains(filter.IdContains)
                    .WithId(filter.SearchText)
					.WithEmail(filter.Email)
					.WithAddress1(filter.Address1)
					.WithAddress2(filter.Address2)
					.WithCity(filter.City)
					.WithCompany(filter.Company)
					.WithCountry(filter.Country)
					.WithFirstName(filter.FirstName)
					.WithLastName(filter.LastName)
					.WithPhone(filter.Phone)
					.WithState(filter.State)
					.WithZip(filter.Zip);

			Func<IQueryable<VCustomer>, IOrderedQueryable<VCustomer>> sortable = x => x.OrderByDescending(y => y.DateEdited);
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
                case VCustomerSortPath.Name:
					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.LastName).ThenBy(y => y.FirstName)
								: x.OrderByDescending(y => y.LastName).ThenByDescending(y => y.FirstName);
                    break;
				case VCustomerSortPath.Updated:
					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.DateEdited)
								: x.OrderByDescending(y => y.DateEdited);
					break;
				case VCustomerSortPath.Country:
					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.CountryCode)
								: x.OrderByDescending(y => y.CountryCode);
					break;
				case VCustomerSortPath.City:
					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.City)
								: x.OrderByDescending(y => y.City);
					break;
				case VCustomerSortPath.State:
					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.StateOrCounty)
								: x.OrderByDescending(y => y.StateOrCounty);
					break;
				case VCustomerSortPath.Status:
					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.StatusCode)
								: x.OrderByDescending(y => y.StatusCode);
					break;
			}

			var customers = await _vCustomerRepositoryAsync.Query(condition).OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

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
					FirstName = x.FirstName,
					LastName = x.LastName,
					DateEdited = x.DateEdited,
					IdObjectType = x.IdObjectType,
					CountryCode = x.CountryCode,
					StateCode = x.StateCode,
					StateName = x.StateName,
					CountryName = x.CountryName,
					City = x.City,
					Company = x.Company,
					Id = x.Id,
					Address1 = x.Address1,
					Address2 = x.Address2,
					Email = x.Email,
					Phone = x.Phone,
					Zip = x.Zip,
					County = x.County,
					StateOrCounty = x.StateOrCounty,
					StatusCode = x.StatusCode,
                    LastOrderPlaced = x.LastOrderPlaced,
                    TotalOrders = x.TotalOrders,
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
    }
}
