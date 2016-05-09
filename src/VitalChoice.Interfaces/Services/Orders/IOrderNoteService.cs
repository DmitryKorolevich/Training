using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Interfaces.Services.Orders
{
	public interface IOrderNoteService
	{
		Task<PagedList<ExtendedOrderNote>> GetOrderNotesAsync(FilterBase filter);

		Task<OrderNote> GetAsync(int id);

		Task<ExtendedOrderNote> GetExtendedAsync(int id);

		Task<OrderNote> AddOrderNoteAsync(OrderNote orderNote, int userId);

		Task<OrderNote> UpdateOrderNoteAsync(OrderNote orderNote, int userId);

		Task<bool> DeleteOrderNoteAsync(int id, int userId);
	}
}
