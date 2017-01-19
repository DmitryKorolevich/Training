using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.UOW;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Interfaces.Services.Orders
{
	public interface IOrderService: IDynamicServiceAsync<OrderDynamic, Order>
	{
        Task<PagedList<ShortOrderItemModel>> GetShortOrdersAsync(OrderFilter filter);
        Task<int?> GetOrderIdCustomer(int id);
	    Task<PagedList<OrderInfoItem>> GetOrdersAsync(VOrderFilter filter);
        Task<OrderDynamic> SelectWithCustomerAsync(int id, bool withDefaults = false);
	    Task<OrderDataContext> CalculateOrder(OrderDynamic order, OrderStatus combinedStatus);
	    Task<OrderDataContext> CalculateStorefrontOrder(OrderDynamic order, OrderStatus combinedStatus);
	    Task<OrderDataContext> CalculateOrderForExport(OrderDynamic order, OrderStatus combinedStatus);
	    Task<OrderDynamic> SelectLastOrderAsync(int customerId);

        OrderDynamic CreateNewNormalOrder(OrderStatus status);
        
        Task<bool> ImportOrders(byte[] file, string fileName, OrderImportType orderType, int idCustomer, int? idPaymentMethod, int idAddedBy);

	    Task OrderTypeSetup(OrderDynamic order);

	    Task<bool> CancelOrderAsync(int id, POrderType? pOrderType = null);

	    string GenerateOrderCode(POrderType? pOrderType, int idOrder, out TaxGetType type);

	    Task LogOrderUpdateAsync(int id);

        #region AffiliatesOrders

        Task<PagedList<AffiliateOrderListItemModel>> GetAffiliateOrderPaymentsWithCustomerInfo(AffiliateOrderPaymentFilter filter);

        #endregion

        #region HealthWiseOrders

        Task<bool> UpdateHealthwiseOrderWithValidationAsync(int orderId, bool isHealthwise);

        Task<bool> UpdateHealthwiseOrderAsync(int orderId, bool isHealthwise);

	    Task MarkHealthwiseCustomerAsync(IUnitOfWorkAsync uow, int idCustomer);

        #endregion

        #region OrdersStatistic

        Task<ICollection<OrdersRegionStatisticItem>> GetOrdersRegionStatisticAsync(OrderRegionFilter filter);

        Task<ICollection<OrdersZipStatisticItem>> GetOrdersZipStatisticAsync(OrderRegionFilter filter);

        Task<PagedList<VOrderWithRegionInfoItem>> GetOrderWithRegionInfoItemsAsync(OrderRegionFilter filter);

        Task<decimal> GetOrderWithRegionInfoAmountAsync(OrderRegionFilter filter);

        #endregion

        #region GCOrders

        Task<ICollection<GCOrderItem>> GetGCOrdersAsync(int idGC);
        
	    Task<ICollection<SkuOrdered>> GetGeneratedGcs(int id);

        #endregion

		Task<PagedList<OrderDynamic>> GetFullOrdersAsync(OrderFilter filter);

		Task<PagedList<OrderDynamic>> GetFullAutoShipsAsync(OrderFilterBase filter);

		Task ActivatePauseAutoShipAsync(int customerId, int autoShipId, bool activate);

		Task DeleteAutoShipAsync(int customerId, int autoShipId);

		Task SubmitAutoShipOrders();

		Task<IList<int>> SelectAutoShipOrdersAsync(int idOrder);

	    Task<int> GetReshipCount(int pastMonths, int idCustomer);
	}
}
