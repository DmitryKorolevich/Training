using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Interfaces.Services.Orders
{
	public interface IOrderService: IDynamicServiceAsync<OrderDynamic, Order>
	{
        Task<PagedList<Order>> GetShortOrdersAsync(OrderFilter filter);
        Task<int?> GetOrderIdCustomer(int id);
        Task<PagedList<VOrder>> GetOrdersAsync(VOrderFilter filter);
	    Task<PagedList<OrderInfoItem>> GetOrdersAsync2(VOrderFilter filter);
        Task<OrderDynamic> SelectWithCustomerAsync(int id, bool withDefaults = false);
	    Task<OrderDataContext> CalculateOrder(OrderDynamic order, OrderStatus combinedStatus);
	    Task<OrderDataContext> CalculateStorefrontOrder(OrderDynamic order, OrderStatus combinedStatus);
        Task<OrderDynamic> SelectLastOrderAsync(int customerId);

        Task<OrderDynamic> CreateNewNormalOrder(OrderStatus status);
        
        Task<bool> ImportOrders(byte[] file, string fileName, OrderType orderType, int idCustomer, int idPaymentMethod, int idAddedBy);

	    Task OrderTypeSetup(OrderDynamic order);

	    Task<bool> CancelOrderAsync(int id);

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

		Task ActivatePauseAutoShipAsync(int customerId, int autoShipId);

		Task DeleteAutoShipAsync(int customerId, int autoShipId);

		Task SubmitAutoShipOrders();

		Task<IList<int>> SelectAutoShipOrdersAsync(int idOrder);
	}
}
