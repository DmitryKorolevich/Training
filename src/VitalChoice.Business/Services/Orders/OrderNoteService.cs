using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Queries.Users;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.Business.Services.Orders
{
	public class OrderNoteService : IOrderNoteService
	{
		private readonly IEcommerceRepositoryAsync<OrderNote> _orderNoteRepository;
		private readonly IEcommerceRepositoryAsync<OrderNoteToCustomerType> _orderNoteToCustomerTypeRepository;
		private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
		private readonly ILogger _logger;
	    private readonly ITransactionAccessor<EcommerceContext> _infrastructureTransactionAccessor;

	    public OrderNoteService(IEcommerceRepositoryAsync<OrderNote> orderNoteRepository,
	        IEcommerceRepositoryAsync<OrderNoteToCustomerType> orderNoteToCustomerTypeRepository,
	        IRepositoryAsync<AdminProfile> adminProfileRepository, ILoggerProviderExtended loggerProvider,
	        ITransactionAccessor<EcommerceContext> infrastructureTransactionAccessor)
	    {
	        _orderNoteRepository = orderNoteRepository;
	        _orderNoteToCustomerTypeRepository = orderNoteToCustomerTypeRepository;
	        _adminProfileRepository = adminProfileRepository;
	        _infrastructureTransactionAccessor = infrastructureTransactionAccessor;
	        _logger = loggerProvider.CreateLogger<OrderNoteService>();
	    }

	    public async Task<PagedList<ExtendedOrderNote>> GetOrderNotesAsync(FilterBase filter)
		{
			Func<IQueryable<OrderNote>, IOrderedQueryable<OrderNote>> sortable = x => x.OrderByDescending(y => y.DateEdited);
			var sortOrder = filter.Sorting.SortOrder;
			switch (filter.Sorting.Path)
			{
				case OrderNoteSortPath.Title:
					sortable =
						(x) =>
							sortOrder == FilterSortOrder.Asc
								? x.OrderBy(y => y.Title)
								: x.OrderByDescending(y => y.Title);
					break;
				case OrderNoteSortPath.Updated:
					sortable =
						(x) =>
							sortOrder == FilterSortOrder.Asc
								? x.OrderBy(y => y.DateEdited)
								: x.OrderByDescending(y => y.DateEdited);
					break;
			}

			var condition = new OrderNoteQuery().NotDeleted().MatchBySearchText(filter.SearchText);
			var orderNotes = await _orderNoteRepository.Query(condition).Include(x => x.CustomerTypes).OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);

			var adminProfileCondition =
				new AdminProfileQuery().IdInRange(
					orderNotes.Items.Where(x => x.IdEditedBy.HasValue).Select(x => x.IdEditedBy.Value).ToList());

			var adminProfiles = await _adminProfileRepository.Query(adminProfileCondition).SelectAsync(false);

			var result = new PagedList<ExtendedOrderNote>
			{
				Items = orderNotes.Items.Select(orderNote => new ExtendedOrderNote
				{
					AdminProfile = adminProfiles.SingleOrDefault(x => x.Id == orderNote.IdEditedBy),
					IdEditedBy = orderNote.IdEditedBy,
					CustomerTypes = orderNote.CustomerTypes,
					DateCreated = orderNote.DateCreated,
					DateEdited = orderNote.DateEdited,
					EditedBy = orderNote.EditedBy,
					Id = orderNote.Id,
					Title = orderNote.Title,
					Description = orderNote.Description,
					StatusCode = orderNote.StatusCode
				}).ToList(),
				Count = orderNotes.Count
			};

			return result;
		}

		public async Task<ExtendedOrderNote> GetExtendedAsync(int id)
		{
			var orderNote = await GetAsync(id);

			if (orderNote != null)
			{
				return new ExtendedOrderNote
				{
					AdminProfile =
						(await _adminProfileRepository.Query(x => x.Id == id).SelectAsync(false)).SingleOrDefault(
							x => x.Id == orderNote.IdEditedBy),
					IdEditedBy = orderNote.IdEditedBy,
					CustomerTypes = orderNote.CustomerTypes,
					DateCreated = orderNote.DateCreated,
					DateEdited = orderNote.DateEdited,
					EditedBy = orderNote.EditedBy,
					Id = orderNote.Id,
					Title = orderNote.Title,
					Description = orderNote.Description,
					StatusCode = orderNote.StatusCode
				};
			}

			return null;
		}

		public async Task<OrderNote> GetAsync(int id)
		{
			var condition = new OrderNoteQuery().NotDeleted().MatchByid(id);

			var orderNote = (await _orderNoteRepository.Query(condition).Include(x => x.CustomerTypes).SelectAsync(false)).SingleOrDefault();

			return orderNote;
		}

		public async Task<OrderNote> AddOrderNoteAsync(OrderNote orderNote, int userId)
		{
			if (await _orderNoteRepository.Query(new OrderNoteQuery().NotDeleted().MatchByName(orderNote.Title, null)).SelectAnyAsync())
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.LabelTakenAlready]);
			}

			orderNote.StatusCode = RecordStatusCode.Active;
			orderNote.DateCreated = orderNote.DateEdited = DateTime.Now;
		    orderNote.IdEditedBy = userId;
            await _orderNoteRepository.InsertGraphAsync(orderNote);

			return orderNote;
		}

		public async Task<OrderNote> UpdateOrderNoteAsync(OrderNote orderNote, int userId)
		{
			if (await _orderNoteRepository.Query(new OrderNoteQuery().NotDeleted().MatchByName(orderNote.Title, orderNote.Id)).SelectAnyAsync())
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.LabelTakenAlready]);
			}

			orderNote.StatusCode = RecordStatusCode.Active;
			orderNote.DateEdited = DateTime.Now;
		    orderNote.IdEditedBy = userId;

            using (var transaction = _infrastructureTransactionAccessor.BeginTransaction())
			{
				try
				{
					var idsToDelete = await _orderNoteToCustomerTypeRepository.Query(x => x.IdOrderNote == orderNote.Id).SelectAsync(false);
					if (idsToDelete.Count > 0)
					{
						await _orderNoteToCustomerTypeRepository.DeleteAllAsync(idsToDelete);
					}

					if (orderNote.CustomerTypes.Count > 0)
					{
						foreach (var customerType in orderNote.CustomerTypes)
						{
							customerType.IdOrderNote = orderNote.Id;
						}

						await _orderNoteToCustomerTypeRepository.InsertRangeAsync(orderNote.CustomerTypes);
					}

					await _orderNoteRepository.UpdateAsync(orderNote);

					transaction.Commit();
				}
                catch
                {
                    transaction.Rollback();
					throw;
				}
			}

			return orderNote;
		}

		public async Task<bool> DeleteOrderNoteAsync(int id, int userId)
		{
			if (await _orderNoteRepository.Query(new OrderNoteQuery().NotDeleted().MatchByid(id).HasCustomerAssignments()).Include(n => n.Customers).SelectAnyAsync())
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.HasAssignments]);
			}

			var orderNote = (await _orderNoteRepository.Query(new OrderNoteQuery().NotDeleted().MatchByid(id)).SelectAsync(false)).SingleOrDefault();

			if (orderNote != null)
			{
				orderNote.StatusCode = RecordStatusCode.Deleted;
				orderNote.DateEdited = DateTime.Now;
				orderNote.IdEditedBy = userId;

				await _orderNoteRepository.UpdateAsync(orderNote);

				return true;
			}

			return false;
		}
	}
}