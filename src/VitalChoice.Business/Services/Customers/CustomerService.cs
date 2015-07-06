using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Order;
using VitalChoice.Business.Queries.Payment;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
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

        protected override IQueryObject<CustomerOptionType> GetOptionTypeQuery(int? idType)
        {
            throw new NotImplementedException();
        }

        protected override IUnitOfWorkAsync CreateUnitOfWork()
        {
            return new EcommerceUnitOfWork();
        }

        public CustomerService(IEcommerceRepositoryAsync<OrderNote> orderNoteRepositoryAsync,
            IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepositoryAsync,
            IEcommerceRepositoryAsync<Customer> customerRepositoryAsync,
            IEcommerceRepositoryAsync<CustomerOptionType> customerOptionTypeRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepositoryAsync, CustomerMapper customerMapper)
            : base(customerMapper, customerRepositoryAsync, customerOptionTypeRepositoryAsync, bigStringRepositoryAsync)
        {
            _orderNoteRepositoryAsync = orderNoteRepositoryAsync;
            _paymentMethodRepositoryAsync = paymentMethodRepositoryAsync;
            _customerRepositoryAsync = customerRepositoryAsync;
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

			return errors;
		}

        protected override Task<List<MessageInfo>> ValidateDelete(int id)
        {
            throw new NotImplementedException();
        }

        protected override Task BeforeUpdateAsync(CustomerDynamic model, Customer entity, IUnitOfWorkAsync uow)
        {
            throw new NotImplementedException();
        }

        protected override Task AfterUpdateAsync(CustomerDynamic model, Customer entity, IUnitOfWorkAsync uow)
        {
            throw new NotImplementedException();
        }

        protected override IQueryFluent<Customer> BuildQuery(IQueryFluent<Customer> query)
        {
            return query.Include(p => p.Addresses)
                .Include(p => p.CustomerNotes)
                .Include(p => p.CustomerPaymentMethods)
                .Include(p => p.DefaultPaymentMethod)
                .Include(p => p.OrderNotes)
                .Include(p => p.PaymentMethods)
                .Include(p => p.User);
        }

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

		public async Task<PagedList<CustomerDynamic>> GetCustomersAsync(CustomerFilter filter)
		{
			//var conditions = new DiscountQuery().NotDeleted().WithText(filter.SearchText).WithStatus(filter.Status);
			//var query = _discountRepository.Query(conditions);

			//Func<IQueryable<Discount>, IOrderedQueryable<Discount>> sortable = x => x.OrderByDescending(y => y.DateCreated);
			//var sortOrder = filter.Sorting.SortOrder;
			//switch (filter.Sorting.Path)
			//{
			//	case DiscountSortPath.Code:
			//		sortable =
			//			(x) =>
			//				sortOrder == SortOrder.Asc
			//					? x.OrderBy(y => y.Code)
			//					: x.OrderByDescending(y => y.Code);
			//		break;
			//	case DiscountSortPath.Description:
			//		sortable =
			//			(x) =>
			//				sortOrder == SortOrder.Asc
			//					? x.OrderBy(y => y.Description)
			//					: x.OrderByDescending(y => y.Description);
			//		break;
			//	case DiscountSortPath.IdDiscountType:
			//		sortable =
			//			(x) =>
			//				sortOrder == SortOrder.Asc
			//					? x.OrderBy(y => y.IdDiscountType)
			//					: x.OrderByDescending(y => y.IdDiscountType);
			//		break;
			//	case DiscountSortPath.Assigned:
			//		sortable =
			//			(x) =>
			//				sortOrder == SortOrder.Asc
			//					? x.OrderBy(y => y.Assigned)
			//					: x.OrderByDescending(y => y.Assigned);
			//		break;
			//	case DiscountSortPath.StartDate:
			//		sortable =
			//			(x) =>
			//				sortOrder == SortOrder.Asc
			//					? x.OrderBy(y => y.StartDate)
			//					: x.OrderByDescending(y => y.StartDate);
			//		break;
			//	case DiscountSortPath.ExpirationDate:
			//		sortable =
			//			(x) =>
			//				sortOrder == SortOrder.Asc
			//					? x.OrderBy(y => y.ExpirationDate)
			//					: x.OrderByDescending(y => y.ExpirationDate);
			//		break;
			//	case DiscountSortPath.DateCreated:
			//		sortable =
			//			(x) =>
			//				sortOrder == SortOrder.Asc
			//					? x.OrderBy(y => y.DateCreated)
			//					: x.OrderByDescending(y => y.DateCreated);
			//		break;
			//}

			//var result = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
			//PagedList<DiscountDynamic> toReturn = new PagedList<DiscountDynamic>(result.Items.Select(p => new DiscountDynamic(p)).ToList(), result.Count);
			//if (toReturn.Items.Any())
			//{
			//	var ids = result.Items.Select(p => p.IdAddedBy).ToList();
			//	var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync();
			//	foreach (var item in toReturn.Items)
			//	{
			//		foreach (var profile in profiles)
			//		{
			//			if (item.IdAddedBy == profile.Id)
			//			{
			//				item.Data.AddedByAgentId = profile.AgentId;
			//			}
			//		}
			//	}
			//}


			//return toReturn;

			return new PagedList<CustomerDynamic>
			{
			    Count = 0,
                Items = new List<CustomerDynamic>()
			};
		}
    }
}
