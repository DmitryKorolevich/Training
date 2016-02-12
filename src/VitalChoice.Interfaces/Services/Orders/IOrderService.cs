﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Interfaces.Services.Orders
{
	public interface IOrderService: IDynamicServiceAsync<OrderDynamic, Order>
	{
        Task<PagedList<Order>> GetShortOrdersAsync(ShortOrderFilter filter);
        Task<int?> GetOrderIdCustomer(int id);
        Task<PagedList<VOrder>> GetOrdersAsync(VOrderFilter filter);
	    Task<OrderDynamic> SelectWithCustomerAsync(int id, bool withDefaults = false);
	    Task<OrderDataContext> CalculateOrder(OrderDynamic order, OrderStatus combinedStatus);
        Task<OrderDynamic> SelectLastOrderAsync(int customerId);

        Task<List<OrderDynamic>> SelectAsync(Expression<Func<Order, bool>> query = null,
            Func<IQueryLite<Order>, IQueryLite<Order>> includesOverride = null,
            bool withDefaults = false);

        Task<OrderDynamic> CreateNewNormalOrder(OrderStatus status);
        
        Task<bool> ImportOrders(byte[] file, string fileName, OrderType orderType, int idCustomer, int idPaymentMethod, int idAddedBy);

	    Task OrderTypeSetup(OrderDynamic order);

        #region AffiliatesOrders

        Task<PagedList<AffiliateOrderListItemModel>> GetAffiliateOrderPaymentsWithCustomerInfo(AffiliateOrderPaymentFilter filter);

        #endregion

        #region HealthWiseOrders

        Task<bool> UpdateHealthwiseOrderWithValidationAsync(int orderId, bool isHealthwise);

        Task<bool> UpdateHealthwiseOrderAsync(int orderId, bool isHealthwise);

        #endregion

        #region OrdersStatistic

        Task<ICollection<OrdersRegionStatisticItem>> GetOrdersRegionStatisticAsync(OrderRegionFilter filter);

        Task<ICollection<OrdersZipStatisticItem>> GetOrdersZipStatisticAsync(OrderRegionFilter filter);

        Task<PagedList<VOrderWithRegionInfoItem>> GetOrderWithRegionInfoItemsAsync(OrderRegionFilter filter);

        Task<decimal> GetOrderWithRegionInfoAmountAsync(OrderRegionFilter filter);

        #endregion

        #region GCOrders

        Task<ICollection<GCOrderItem>> GetGCOrdersAsync(int idGC);

        #endregion
    }
}
