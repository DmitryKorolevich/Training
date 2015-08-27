using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Azure;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Customers;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Interfaces.Services.Customers;

namespace VitalChoice.Business.Services.Customers
{
    public class CustomerService: EcommerceDynamicObjectService<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>, ICustomerService
    {
	    private readonly IEcommerceRepositoryAsync<OrderNote> _orderNoteRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<PaymentMethod> _paymentMethodRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<Customer> _customerRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<Address> _addressesRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<CustomerNote> _customerNotesRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<CustomerToOrderNote> _customerToOrderNoteRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<CustomerToPaymentMethod> _customerToPaymentMethodRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<VCustomer> _vCustomerRepositoryAsync;
		private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IEcommerceRepositoryAsync<CustomerPaymentMethod> _customerPaymentMethodRepositoryAsync;
	    private readonly IBlobStorageClient _storageClient;
	    private static string _customerContainerName;

	    public CustomerService(IEcommerceRepositoryAsync<OrderNote> orderNoteRepositoryAsync,
            IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepositoryAsync,
            IEcommerceRepositoryAsync<Customer> customerRepositoryAsync,
            IEcommerceRepositoryAsync<CustomerOptionType> customerOptionTypeRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, CustomerMapper customerMapper,
            IEcommerceRepositoryAsync<Address> addressesRepositoryAsync,
            IEcommerceRepositoryAsync<CustomerNote> customerNotesRepositoryAsync,
            IEcommerceRepositoryAsync<CustomerToOrderNote> customerToOrderNoteRepositoryAsync,
            IEcommerceRepositoryAsync<CustomerToPaymentMethod> customerToPaymentMethodRepositoryAsync,
            IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<CustomerOptionValue> customerOptionValueRepositoryAsync,
            IEcommerceRepositoryAsync<CustomerPaymentMethod> customerPaymentMethodRepositoryAsync,
			IBlobStorageClient storageClient,
			IOptions<AppOptions> appOptions)
            : base(
                customerMapper, customerRepositoryAsync, customerOptionTypeRepositoryAsync,
                customerOptionValueRepositoryAsync, bigStringRepositoryAsync)
        {
            _orderNoteRepositoryAsync = orderNoteRepositoryAsync;
            _paymentMethodRepositoryAsync = paymentMethodRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
            _addressesRepositoryAsync = addressesRepositoryAsync;
            _customerNotesRepositoryAsync = customerNotesRepositoryAsync;
            _customerToOrderNoteRepositoryAsync = customerToOrderNoteRepositoryAsync;
            _customerToPaymentMethodRepositoryAsync = customerToPaymentMethodRepositoryAsync;
            _vCustomerRepositoryAsync = vCustomerRepositoryAsync;
            _adminProfileRepository = adminProfileRepository;
            _customerPaymentMethodRepositoryAsync = customerPaymentMethodRepositoryAsync;
		    _storageClient = storageClient;
		    _customerContainerName = appOptions.Options.AzureStorage.CustomerContainerName;
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
                    x => x.IdObjectType == (int) AddressType.Shipping && x.StatusCode != RecordStatusCode.Deleted)
                    .All(x => !x.Data.Default))
            {
                throw new AppValidationException(
                    ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AtLeastOneDefaultShipping]);
            }

            return errors;
		}

        protected async override Task BeforeEntityChangesAsync(CustomerDynamic model, Customer entity, IUnitOfWorkAsync uow)
        {
            var customerPaymentMethodRepository = uow.RepositoryAsync<CustomerPaymentMethod>();
            var customerPaymentMethodOptionValuesRepository = uow.RepositoryAsync<CustomerPaymentMethodOptionValue>();
            var customerToPaymentMethodRepository = uow.RepositoryAsync<CustomerToPaymentMethod>();
			var customerToOrderNoteRepository = uow.RepositoryAsync<CustomerToOrderNote>();
            var addressesRepositoryAsync = uow.RepositoryAsync<Address>();
            var customerNotesRepositoryAsync = uow.RepositoryAsync<CustomerNote>();
            var addressOptionValuesRepositoryAsync = uow.RepositoryAsync<AddressOptionValue>();
            var customerNoteOptionValuesRepositoryAsync = uow.RepositoryAsync<CustomerNoteOptionValue>();
            var customerFileRepositoryAsync = uow.RepositoryAsync<CustomerFile>();

   //         entity.PaymentMethods = customerToPaymentMethodRepository.Query(c => c.IdCustomer == model.Id).Select();
			//entity.OrderNotes = customerToOrderNoteRepository.Query(c => c.IdCustomer == model.Id).Select();

			await customerToPaymentMethodRepository.DeleteAllAsync(entity.PaymentMethods);
			await customerToOrderNoteRepository.DeleteAllAsync(entity.OrderNotes);

            await uow.SaveChangesAsync();

            //var addressType = (int?)AddressType.Billing;
            //entity.Addresses =
            //    await
            //        addressesRepositoryAsync.Query(a => a.IdCustomer == entity.Id && a.IdObjectType != addressType)
            //            .Include(a => a.OptionValues)
            //            .SelectAsync();
            //entity.CustomerNotes =
            //    await
            //        customerNotesRepositoryAsync.Query(a => a.IdCustomer == entity.Id)
            //            .Include(n => n.OptionValues)
            //            .SelectAsync();
            //entity.CustomerPaymentMethods =
            //    await
            //        customerPaymentMethodRepository.Query(p => p.IdCustomer == entity.Id)
            //            .Include(p => p.OptionValues)
            //            .Include(p => p.BillingAddress)
            //            .ThenInclude(a => a.OptionValues)
            //            .SelectAsync();
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
            await
                addressesRepositoryAsync.DeleteAllAsync(
                    entity.Addresses.Where(a => a.StatusCode == RecordStatusCode.Deleted));
            await
                customerNotesRepositoryAsync.DeleteAllAsync(
                    entity.CustomerNotes.Where(n => n.StatusCode == RecordStatusCode.Deleted));

			if (model.Files != null && model.Files.Any() && entity.Files != null)
			{
				//Update
				var toUpdate = entity.Files.Where(e => model.Files.Select(x=>x.Id).Contains(e.Id));
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
            var customerToPaymentMethodRepository = uow.RepositoryAsync<CustomerToPaymentMethod>();
            var customerToOrderNoteRepository = uow.RepositoryAsync<CustomerToOrderNote>();
            var addressesRepositoryAsync = uow.RepositoryAsync<Address>();
            var addressOptionValuesRepositoryAsync = uow.RepositoryAsync<AddressOptionValue>();
            var customerNoteOptionValuesRepositoryAsync = uow.RepositoryAsync<CustomerNoteOptionValue>();

            foreach (var address in entity.Addresses.Where(a => a.IdObjectType != (int?)AddressType.Billing && a.Id != 0 && a.StatusCode != RecordStatusCode.Deleted))
            {
                await addressOptionValuesRepositoryAsync.InsertRangeAsync(address.OptionValues);
            }
            foreach (var address in entity.Addresses.Where(a => a.Id == 0))
            {
                await addressesRepositoryAsync.InsertGraphAsync(address);
            }
            foreach (var customerNote in entity.CustomerNotes)
            {
                await customerNoteOptionValuesRepositoryAsync.InsertRangeAsync(customerNote.OptionValues);
            }
            foreach (var customerPaymentMethod in entity.CustomerPaymentMethods.Where(m => m.StatusCode != RecordStatusCode.Deleted))
            {
                await customerPaymentMethodOptionValuesRepository.InsertRangeAsync(customerPaymentMethod.OptionValues);
                if (customerPaymentMethod.BillingAddress.Id == 0)
                    await addressesRepositoryAsync.InsertGraphAsync(customerPaymentMethod.BillingAddress);
            }
            var paymentsToDelete =
                entity.CustomerPaymentMethods.Where(a => a.StatusCode == RecordStatusCode.Deleted).ToList();
            await customerPaymentMethodRepository.DeleteAllAsync(paymentsToDelete);
            await addressesRepositoryAsync.DeleteAllAsync(paymentsToDelete.Where(p => p.BillingAddress != null).Select(p => p.BillingAddress));

            await customerToPaymentMethodRepository.InsertRangeAsync(entity.PaymentMethods);
            await customerToOrderNoteRepository.InsertRangeAsync(entity.OrderNotes);
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

        protected async override Task<Customer> InsertAsync(CustomerDynamic model, IUnitOfWorkAsync uow)
	    {
			var rand = new Random();

			var userRepository = uow.RepositoryAsync<User>();
			var user = new User()
			{
				Id = rand.Next(1, 100000000) //temp solution
			};

			using (var transaction = uow.BeginTransaction())
		    {
			    try
			    {
					await userRepository.InsertAsync(user);
					await uow.SaveChangesAsync();

					model.Id = user.Id;
					model.User.Id = user.Id;
					var customer = await base.InsertAsync(model, uow);
					transaction.Commit();
				    return customer;
			    }
			    catch (Exception)
			    {
					transaction.Rollback();
				    throw;
			    }
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
					StatusCode = x.StatusCode
				}).ToList(),
				Count = customers.Count
			};

			return result;
		}

	    public async Task<string> UploadFileAsync(byte[] file, string fileName, string customerPublicId, string contentType = null)
	    {
		    var i = 0;
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

	    public async Task<bool> DeleteFileAsync(string fileName, string customerPublicId)
	    {
		    return await _storageClient.DeleteBlobAsync(_customerContainerName, $"{customerPublicId}/{fileName}");
	    }
    }
}
