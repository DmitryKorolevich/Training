using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Order;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Business.Services.Customers;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
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
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Workflow.Contexts;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderService : EcommerceDynamicObjectService<OrderDynamic, Order, OrderOptionType, OrderOptionValue>, IOrderService
    {
        private readonly IEcommerceRepositoryAsync<VOrder> _vOrderRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> _productOptionTypesRepository;
        private readonly ProductMapper _productMapper;
        private readonly ICustomerService _customerService;

        public OrderService(IEcommerceRepositoryAsync<VOrder> vOrderRepository,
            IEcommerceRepositoryAsync<OrderOptionType> orderOptionTypeRepository,
            IEcommerceRepositoryAsync<Lookup> lookupRepository, IEcommerceRepositoryAsync<Order> orderRepository,
            IEcommerceRepositoryAsync<Sku> skuRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderMapper mapper,
            IEcommerceRepositoryAsync<OrderOptionValue> orderValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository, IEcommerceRepositoryAsync<ProductOptionType> productOptionTypesRepository, ProductMapper productMapper, ICustomerService customerService)
            : base(
                mapper, orderRepository, orderOptionTypeRepository, orderValueRepositoryAsync,
                bigStringValueRepository)
        {
            _vOrderRepository = vOrderRepository;
            _adminProfileRepository = adminProfileRepository;
            _productOptionTypesRepository = productOptionTypesRepository;
            _productMapper = productMapper;
            _customerService = customerService;
        }

        protected override IQueryFluent<Order> BuildQuery(IQueryFluent<Order> query)
        {
            return
                query.Include(o => o.Discount)
                    .ThenInclude(d => d.OptionValues)
                    .Include(o => o.GiftCertificates)
                    .ThenInclude(g => g.GiftCertificate)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.BillingAddress)
                    .ThenInclude(a => a.OptionValues)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.OptionValues)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.PaymentMethod)
                    .Include(o => o.ShippingAddress)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.Product)
                    .Include(p => p.OptionValues);
        }

        protected override async Task AfterSelect(Order entity)
        {
            var productOptionTypes = await _productOptionTypesRepository.Query().SelectAsync(false);
            foreach (var orderToSku in entity.Skus)
            {
                var optionTypes = productOptionTypes.Where(_productMapper.GetOptionTypeQuery()
                    .WithObjectType(orderToSku.Sku.Product.IdObjectType)
                    .Query()
                    .Compile()).ToList();
                orderToSku.Sku.OptionTypes = optionTypes;
                orderToSku.Sku.Product.OptionTypes = optionTypes;
            }
        }

        public async Task<OrderDynamic> SelectWithCustomerAsync(int id, bool withDefaults = false)
        {
            var order = await SelectAsync(id, withDefaults);
            order.Customer = await _customerService.SelectAsync(order.Customer.Id, withDefaults);
            return order;
        }

        public Task<OrderContext> CalculateOrder(OrderDynamic order)
        {
            return Task.FromResult(new OrderContext
            {
                Order = order
            });
        }

        protected override async Task BeforeEntityChangesAsync(OrderDynamic model, Order entity, IUnitOfWorkAsync uow)
        {
            var orderToGcRepository = uow.RepositoryAsync<OrderToGiftCertificate>();
            var orderToSkuRepository = uow.RepositoryAsync<OrderToSku>();
            await orderToGcRepository.DeleteAllAsync(entity.GiftCertificates);
            await orderToSkuRepository.DeleteAllAsync(entity.Skus);
        }

        protected override async Task AfterEntityChangesAsync(OrderDynamic model, Order entity, IUnitOfWorkAsync uow)
        {
            var orderToGcRepository = uow.RepositoryAsync<OrderToGiftCertificate>();
            var orderToSkuRepository = uow.RepositoryAsync<OrderToSku>();
            await orderToGcRepository.InsertRangeAsync(entity.GiftCertificates);
            await orderToSkuRepository.InsertRangeAsync(entity.Skus);
        }

        protected override Task<List<MessageInfo>> Validate(OrderDynamic dynamic)
        {
            return base.Validate(dynamic);
        }

        protected override Task<List<MessageInfo>> ValidateDelete(int id)
        {
            return base.ValidateDelete(id);
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