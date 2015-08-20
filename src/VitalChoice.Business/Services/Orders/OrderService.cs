using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Order;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderService : EcommerceDynamicObjectService<OrderDynamic, Order, OrderOptionType, OrderOptionValue>, IOrderService
    {
        private readonly IEcommerceRepositoryAsync<VOrder> _vOrderRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;

        public OrderService(IEcommerceRepositoryAsync<VOrder> vOrderRepository,
            IEcommerceRepositoryAsync<OrderOptionType> orderOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Order> orderRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderMapper mapper,
            IEcommerceRepositoryAsync<OrderOptionValue> orderValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository)
            : base(
                mapper, orderRepository, orderOptionTypeRepository, orderValueRepositoryAsync,
                bigStringValueRepository)
        {
            _vOrderRepository = vOrderRepository;
            _adminProfileRepository = adminProfileRepository;
        }

        public async Task<PagedList<VOrder>> GetOrdersAsync(VOrderFilter filter)
        {
            var conditions = new VOrderQuery();

            if (!filter.ShipDate)
            {
                conditions = conditions.WithCreatedDate(filter.From, filter.To);
            }
            else
            {
                conditions = conditions.WithShippedDate(filter.From, filter.To);
            }
            conditions= conditions.WithOrderStatus(filter.OrderStatus).WithoutIncomplete(filter.OrderStatus).WithOrderSource(filter.IdOrderSource).WithPOrderType(filter.POrderType).
                WithCustomerType(filter.IdCustomerType).WithShippingMethod(filter.IdShippingMethod);

            var query = _vOrderRepository.Query(conditions);

            Func<IQueryable<VOrder>, IOrderedQueryable<VOrder>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VOrderSortPath.OrderStatus:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.OrderStatus)
                                : x.OrderByDescending(y => y.OrderStatus);
                    break;
                case VOrderSortPath.IdOrderSource:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.IdOrderSource)
                                : x.OrderByDescending(y => y.IdOrderSource);
                    break;
                case VOrderSortPath.IdPaymentMethod:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.IdPaymentMethod)
                                : x.OrderByDescending(y => y.IdPaymentMethod);
                    break;
                case VOrderSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated)
                                : x.OrderByDescending(y => y.DateCreated);
                    break;
                case VOrderSortPath.DateShipped:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateShipped)
                                : x.OrderByDescending(y => y.DateShipped);
                    break;
                case VOrderSortPath.Company:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Company)
                                : x.OrderByDescending(y => y.Company);
                    break;
                case VOrderSortPath.StateCode:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.StateCode)
                                : x.OrderByDescending(y => y.StateCode);
                    break;
                case VOrderSortPath.Customer:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Customer)
                                : x.OrderByDescending(y => y.Customer);
                    break;
                case VOrderSortPath.Id:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Id)
                                : x.OrderByDescending(y => y.Id);
                    break;
                case VOrderSortPath.Total:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Total)
                                : x.OrderByDescending(y => y.Total);
                    break;
                case VOrderSortPath.DateEdited:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateEdited)
                                : x.OrderByDescending(y => y.DateEdited);
                    break;
            }

            var toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            if (toReturn.Items.Any())
            {
                var ids = toReturn.Items.Select(p => p.IdEditedBy).ToList();
                var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync();
                foreach (var item in toReturn.Items)
                {
                    foreach (var profile in profiles)
                    {
                        if (item.IdEditedBy == profile.Id)
                        {
                            item.EditedByAgentId = profile.AgentId;
                        }
                    }
                }
            }

            return toReturn;
        }
    }
}