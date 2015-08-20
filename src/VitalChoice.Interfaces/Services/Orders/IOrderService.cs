using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;

namespace VitalChoice.Interfaces.Services.Orders
{
	public interface IOrderService: IEcommerceDynamicObjectService<OrderDynamic, Order, OrderOptionType, OrderOptionValue>
	{
        Task<PagedList<VOrder>> GetOrdersAsync(VOrderFilter filter);
    }
}
