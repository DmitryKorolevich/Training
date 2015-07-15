using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Customers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Customers;

namespace VitalChoice.Business.Services.Customers
{
    public class CustomerService: DynamicObjectServiceAsync<CustomerDynamic, Customer, CustomerOptionType, CustomerOptionValue>, ICustomerService
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
	    private readonly IEcommerceRepositoryAsync<User> _userRepositoryAsync;

	    protected override IUnitOfWorkAsync CreateUnitOfWork()
        {
            return new EcommerceUnitOfWork();
        }

        public CustomerService(IEcommerceRepositoryAsync<OrderNote> orderNoteRepositoryAsync,
            IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepositoryAsync,
            IEcommerceRepositoryAsync<Customer> customerRepositoryAsync,
            IEcommerceRepositoryAsync<CustomerOptionType> customerOptionTypeRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, CustomerMapper customerMapper, IEcommerceRepositoryAsync<Address> addressesRepositoryAsync, IEcommerceRepositoryAsync<CustomerNote> customerNotesRepositoryAsync, IEcommerceRepositoryAsync<CustomerToOrderNote> customerToOrderNoteRepositoryAsync, IEcommerceRepositoryAsync<CustomerToPaymentMethod> customerToPaymentMethodRepositoryAsync, IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync, IRepositoryAsync<AdminProfile> adminProfileRepository, IEcommerceRepositoryAsync<User> userRepositoryAsync)
            : base(customerMapper, customerRepositoryAsync, customerOptionTypeRepositoryAsync, bigStringRepositoryAsync)
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
	        _userRepositoryAsync = userRepositoryAsync;
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

	        if (model.Addresses.Where(x=>x.IdObjectType == (int)AddressType.Shipping && x.StatusCode != RecordStatusCode.Deleted).All(x => !x.Data.Default))
	        {
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AtLeastOneDefaultShipping]);
			}

			return errors;
		}

        protected async override Task BeforeEntityChangesAsync(CustomerDynamic model, Customer entity, IUnitOfWorkAsync uow)
        {
			var customerToPaymentMethodRepository = uow.RepositoryAsync<CustomerToPaymentMethod>();
			var customerToOrderNoteRepository = uow.RepositoryAsync<CustomerToOrderNote>();

            await customerToPaymentMethodRepository.DeleteAllAsync(entity.PaymentMethods);
            await customerToOrderNoteRepository.DeleteAllAsync(entity.OrderNotes);
            await uow.SaveChangesAsync();
        }

        protected async override Task AfterEntityChangesAsync(CustomerDynamic model, Customer entity, IUnitOfWorkAsync uow)
        {
			var customerToPaymentMethodRepository = uow.RepositoryAsync<CustomerToPaymentMethod>();
			var customerToOrderNoteRepository = uow.RepositoryAsync<CustomerToOrderNote>();

			await customerToPaymentMethodRepository.InsertRangeAsync(entity.PaymentMethods);
       	    await customerToOrderNoteRepository.InsertRangeAsync(entity.OrderNotes);
		}

        protected override async Task AfterSelect(Customer entity)
        {
            entity.Addresses =
                await
                    _addressesRepositoryAsync.Query(a => a.IdCustomer == entity.Id)
                        .Include(a => a.OptionValues)
                        .SelectAsync(false);
            entity.CustomerNotes =
                await
                    _customerNotesRepositoryAsync.Query(a => a.IdCustomer == entity.Id)
                        .Include(n => n.OptionValues)
                        .SelectAsync(false);
			entity.User = (await _userRepositoryAsync.Query(p => p.Id == entity.Id)
						.Include(p => p.Customer)
						.SelectAsync(false)).Single();
		}

        protected override IQueryFluent<Customer> BuildQuery(IQueryFluent<Customer> query)
        {
            return query
                .Include(p => p.DefaultPaymentMethod)
                .Include(p => p.User)
                .Include(p => p.PaymentMethods)
                .Include(p => p.OrderNotes);
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

	    //protected async override Task<CustomerDynamic> UpdateAsync(CustomerDynamic model, IUnitOfWorkAsync uow)
	    //{
		   // return null;
	    //}

	    public async Task<IList<OrderNote>> GetAvailableOrderNotesAsync(CustomerType customerType)
	    {
			var condition = new OrderNoteQuery().NotDeleted().MatchByCustomerType(customerType);

		    return await _orderNoteRepositoryAsync.Query(condition).Include(x => x.CustomerTypes).SelectAsync(false);
	    }

	    public async Task<IList<PaymentMethod>> GetAvailablePaymentMethodsAsync(CustomerType customerType)
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
	}
}
