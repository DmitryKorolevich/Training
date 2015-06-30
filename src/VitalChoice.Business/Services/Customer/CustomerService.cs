using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Order;
using VitalChoice.Business.Queries.Payment;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Customers;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Customer;

namespace VitalChoice.Business.Services.Customer
{
    public class CustomerService: ICustomerService
    {
	    private readonly IEcommerceRepositoryAsync<OrderNote> _orderNoteRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<PaymentMethod> _paymentMethodRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<Domain.Entities.eCommerce.Customers.Customer> _customerRepositoryAsync;
	    private readonly IEcommerceRepositoryAsync<CustomerOptionType> _customerOptionTypeRepositoryAsync;

	    public CustomerService(IEcommerceRepositoryAsync<OrderNote> orderNoteRepositoryAsync, IEcommerceRepositoryAsync<PaymentMethod> paymentMethodRepositoryAsync, IEcommerceRepositoryAsync<Domain.Entities.eCommerce.Customers.Customer> customerRepositoryAsync, IEcommerceRepositoryAsync<CustomerOptionType> customerOptionTypeRepositoryAsync)
	    {
		    _orderNoteRepositoryAsync = orderNoteRepositoryAsync;
		    _paymentMethodRepositoryAsync = paymentMethodRepositoryAsync;
		    _customerRepositoryAsync = customerRepositoryAsync;
		    _customerOptionTypeRepositoryAsync = customerOptionTypeRepositoryAsync;
	    }

	    private async Task<List<MessageInfo>> ValidateCustomerAsync(CustomerDynamic model, int? existingCustomerId = null)
	    {
			var errors = new List<MessageInfo>();

			var customerSameEmail =
				await
					_customerRepositoryAsync.Query(
						new CustomerQuery().NotDeleted().Excluding(existingCustomerId).WithEmail(model.Email))
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

		private async Task<Domain.Entities.eCommerce.Customers.Customer> UpdateCustomerAsync(CustomerDynamic model, EcommerceUnitOfWork uow)
		{
			var customerRepository = uow.RepositoryAsync<Domain.Entities.eCommerce.Customers.Customer>();
			var customerOptionValueRepository = uow.RepositoryAsync<CustomerOptionValue>();
			//var bigValueRepository = uow.RepositoryAsync<BigStringValue>();

			var entity = (await customerRepository.Query(
				p => p.Id == model.Id && p.StatusCode != RecordStatusCode.Deleted)
				.Include(p => p.OptionValues)
				.SelectAsync()).FirstOrDefault();
			if (entity != null)
			{
				//await SetBigValuesAsync(entity, bigValueRepository, true);
				(await ValidateCustomerAsync(model, model.Id)).Raise();

				//await
				//	bigValueRepository.DeleteAllAsync(
				//		entity.OptionValues.Where(o => o.BigValue != null).Select(o => o.BigValue).ToList());
				await customerOptionValueRepository.DeleteAllAsync(entity.OptionValues);

				entity.OptionTypes =
					await _customerOptionTypeRepositoryAsync.Query(o => o.IdCustomerType == model.CustomerType).SelectAsync(false);

				model.UpdateEntity(entity);

				//await
				//	bigValueRepository.InsertRangeAsync(
				//		entity.OptionValues.Where(b => b.BigValue != null).Select(o => o.BigValue).ToList());
				var toReturn = await customerRepository.UpdateAsync(entity);

				await uow.SaveChangesAsync(CancellationToken.None);

				return toReturn;
			}
			return null;
		}

		public async Task<Domain.Entities.eCommerce.Customers.Customer> InsertCustomerAsync(CustomerDynamic model,
		    EcommerceUnitOfWork uow)
	    {
		    (await ValidateCustomerAsync(model)).Raise();

		    var optionTypes =
			    await _customerOptionTypeRepositoryAsync.Query(o => o.IdCustomerType == model.CustomerType).SelectAsync(false);
		    var entity = model.ToEntity(optionTypes);
		    if (entity != null)
		    {
			    entity.OptionTypes = new List<CustomerOptionType>();
			    var customerRepository = uow.RepositoryAsync<Domain.Entities.eCommerce.Customers.Customer>();

			    var result = await customerRepository.InsertGraphAsync(entity);
			    await uow.SaveChangesAsync(CancellationToken.None);

			    result.OptionTypes = optionTypes;
			    return result;
		    }
		    return null;
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

		public async Task<CustomerDynamic> AddUpdateCustomerAsync(CustomerDynamic model)
		{
			if (model == null)
				throw new ArgumentNullException(nameof(model));
			using (var uow = new EcommerceUnitOfWork())
			{
				var idCustomer = 0;
				if (model.Id == 0)
				{
					idCustomer = (await InsertCustomerAsync(model, uow)).Id;
				}
				var customer = await UpdateCustomerAsync(model, uow);
				if (idCustomer != 0)
					return await GetCustomerAsync(idCustomer);
				return new CustomerDynamic(customer);
			}
		}

		public async Task<CustomerDynamic> GetCustomerAsync(int id, bool withDefaults = false)
		{
			IQueryFluent<Domain.Entities.eCommerce.Customers.Customer> res = _customerRepositoryAsync.Query(
				p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted)
				.Include(p => p.OptionValues)
				.Include(p => p.Addresses)
				.Include(p => p.CustomerNotes)
				.Include(p => p.CustomerPaymentMethods)
				.Include(p => p.DefaultPaymentMethod)
				.Include(p => p.OrderNotes)
				.Include(p => p.PaymentMethods)
				.Include(p => p.User);

			var entity = (await res.SelectAsync(false)).FirstOrDefault();

			if (entity != null)
			{
				//await SetBigValuesAsync(entity, _bigStringValueRepository);
				entity.OptionTypes =
					await
						_customerOptionTypeRepositoryAsync.Query(o => o.IdCustomerType == (CustomerType)entity.IdCustomerType)
							.SelectAsync(false);

				return new CustomerDynamic(entity, withDefaults);
			}

			return null;
		}

		public async Task<bool> DeleteCustomerAsync(int id)
		{
			var dbItem =
				(await
					_customerRepositoryAsync.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted)
						.SelectAsync(false)).FirstOrDefault();
			if (dbItem != null)
			{
				dbItem.StatusCode = RecordStatusCode.Deleted;
				await _customerRepositoryAsync.UpdateAsync(dbItem);

				return true;
			}
			return false;
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

			return null;
		}
	}
}
