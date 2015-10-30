using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Interfaces.Services.Orders
{
	public interface IOrderService: IEcommerceDynamicService<OrderDynamic, Order, OrderOptionType, OrderOptionValue>
	{
        Task<PagedList<Order>> GetShortOrdersAsync(ShortOrderFilter filter);
        Task<PagedList<VOrder>> GetOrdersAsync(VOrderFilter filter);
	    Task<OrderDynamic> SelectWithCustomerAsync(int id, bool withDefaults = false);
	    Task<OrderDataContext> CalculateOrder(OrderDynamic order);
    }
}
