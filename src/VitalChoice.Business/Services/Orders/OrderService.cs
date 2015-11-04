﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Order;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Helpers;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderService : EcommerceDynamicService<OrderDynamic, Order, OrderOptionType, OrderOptionValue>,
        IOrderService
    {
        private readonly IEcommerceRepositoryAsync<VOrder> _vOrderRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> _productOptionTypesRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skusRepository;
        private readonly ProductMapper _productMapper;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowFactory _treeFactory;
        private readonly IAffiliateService _affiliateService;

        public OrderService(IEcommerceRepositoryAsync<VOrder> vOrderRepository,
            IEcommerceRepositoryAsync<OrderOptionType> orderOptionTypeRepository,
            IEcommerceRepositoryAsync<Order> orderRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<OrderOptionValue> orderValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<ProductOptionType> productOptionTypesRepository, ProductMapper productMapper,
            ICustomerService customerService, IWorkflowFactory treeFactory,
            ILoggerProviderExtended loggerProvider, IEcommerceRepositoryAsync<Sku> skusRepository,
            IAffiliateService affiliateService)
            : base(
                mapper, orderRepository, orderOptionTypeRepository, orderValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider)
        {
            _vOrderRepository = vOrderRepository;
            _adminProfileRepository = adminProfileRepository;
            _productOptionTypesRepository = productOptionTypesRepository;
            _productMapper = productMapper;
            _customerService = customerService;
            _treeFactory = treeFactory;
            _skusRepository = skusRepository;
            _affiliateService = affiliateService;
        }

        protected override IQueryLite<Order> BuildQuery(IQueryLite<Order> query)
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
					.ThenInclude(s => s.OptionValues)
                    .Include(p => p.OptionValues);
        }

        //TODO: lambda caching
        protected override async Task AfterSelect(ICollection<Order> entities)
        {
            var productOptionTypes = await _productOptionTypesRepository.Query().SelectAsync(false);
            foreach (
                var orderToSku in
                    entities.SelectMany(o => o.Skus).Where(s => s.Sku?.Product != null && s.Sku.OptionTypes == null))
            {
                var optionTypes = productOptionTypes.Where(_productMapper.GetOptionTypeQuery()
                    .WithObjectType(orderToSku.Sku.Product.IdObjectType)
                    .Query()
                    .CacheCompile()).ToList();
                orderToSku.Sku.OptionTypes = optionTypes;
                orderToSku.Sku.Product.OptionTypes = optionTypes;
            }
            var invalidSkuOrdered =
                entities.SelectMany(o => o.Skus)
                    .Where(s => s.Sku?.Product == null || s.Sku.OptionTypes == null)
                    .ToList();
            var skuIds = new HashSet<int>(invalidSkuOrdered.Select(s => s.IdSku));
            var invalidSkus = (await _skusRepository.Query(p => skuIds.Contains(p.Id))
                .Include(p => p.Product)
                .ThenInclude(s => s.OptionValues)
                .Include(p => p.OptionValues)
                .SelectAsync(false)).ToDictionary(s => s.Id);
            foreach (var orderToSku in invalidSkuOrdered)
            {
                Sku sku;
                if (invalidSkus.TryGetValue(orderToSku.IdSku, out sku))
                {
                    var optionTypes = productOptionTypes.Where(_productMapper.GetOptionTypeQuery()
                        .WithObjectType(sku.Product.IdObjectType)
                        .Query()
                        .CacheCompile()).ToList();
                    orderToSku.Sku = sku;
                    orderToSku.Sku.Product = sku.Product;
                    orderToSku.Sku.OptionTypes = optionTypes;
                    orderToSku.Sku.Product.OptionTypes = optionTypes;
                }
            }
        }

        protected override void LogItemInfoSetAfter(ObjectHistoryLogItem log, OrderDynamic model)
        {
            log.IdObjectStatus = (int) model.OrderStatus;
        }

        protected override bool LogObjectFullData => true;

        public async Task<OrderDynamic> SelectWithCustomerAsync(int id, bool withDefaults = false)
        {
            var order = await SelectAsync(id, withDefaults);
            order.Customer = await _customerService.SelectAsync(order.Customer.Id, withDefaults);
            return order;
        }

        public async Task<OrderDataContext> CalculateOrder(OrderDynamic order)
        {
            var context = new OrderDataContext
            {
                Order = order
            };
            var tree = await _treeFactory.CreateTreeAsync<OrderDataContext, decimal>("Order");
            await tree.ExecuteAsync(context);
            UpdateOrderFromCalculationContext(order, context);
            return context;
        }

        public async Task<OrderDynamic> SelectLastOrderAsync(int customerId)
        {
            var orderQuery = new OrderQuery().WithActualStatusOnly().WithCustomerId(customerId);

            return await SelectFirstAsync(queryObject: orderQuery, orderBy: o => o.OrderByDescending(x => x.DateCreated));
        }

        private void UpdateOrderFromCalculationContext(OrderDynamic order, OrderDataContext dataContext)
        {
            order.TaxTotal = dataContext.TaxTotal;
            order.Total = dataContext.Total;
            order.DiscountTotal = dataContext.DiscountTotal;
            order.ShippingTotal = dataContext.ShippingTotal;
            order.ProductsSubtotal = dataContext.ProductsSubtotal;
            //TODO: Add promo skus and skus to order
        }

        protected override async Task BeforeEntityChangesAsync(OrderDynamic model, Order entity, IUnitOfWorkAsync uow)
        {
            var orderAddressOptionValuesRepository = uow.RepositoryAsync<OrderAddressOptionValue>();
            var orderPaymentMethodOptionValuesRepository = uow.RepositoryAsync<OrderPaymentMethodOptionValue>();
            await orderAddressOptionValuesRepository.DeleteAllAsync(entity.ShippingAddress.OptionValues);
            await orderPaymentMethodOptionValuesRepository.DeleteAllAsync(entity.PaymentMethod.OptionValues);
            await orderAddressOptionValuesRepository.DeleteAllAsync(entity.PaymentMethod.BillingAddress.OptionValues);
            var orderToGcRepository = uow.RepositoryAsync<OrderToGiftCertificate>();
            var orderToSkuRepository = uow.RepositoryAsync<OrderToSku>();
            await
                orderToGcRepository.DeleteAllAsync(entity.GiftCertificates.WhereAll(model.GiftCertificates,
                    (g, dg) => g.IdGiftCertificate != dg.GiftCertificate?.Id));
            await
                orderToSkuRepository.DeleteAllAsync(entity.Skus.WhereAll(model.Skus, (s, ds) => s.IdSku != ds.Sku?.Id));
        }

        protected override Task<List<MessageInfo>> Validate(OrderDynamic dynamic)
        {
            if (dynamic.Customer.StatusCode == (int) CustomerStatus.Suspended)
            {
                throw new AppValidationException(
                    ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
            }
            return base.Validate(dynamic);
        }

        public async Task<PagedList<Order>> GetShortOrdersAsync(ShortOrderFilter filter)
        {
            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable = x => x.OrderBy(y => y.Id);
            var query =
                this.ObjectRepository.Query(
                    p => p.Id.ToString().Contains(filter.Id) && p.StatusCode != (int) RecordStatusCode.Deleted)
                    .OrderBy(sortable);
            return await query.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
        }

        protected override async Task<Order> InsertAsync(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            var entity = await base.InsertAsync(model, uow);
            await UpdateAffiliateOrderPayment(entity);
            return entity;
        }

        protected override async Task<List<Order>> InsertRangeAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            var entities = await base.InsertRangeAsync(models, uow);
            foreach(var entity in entities)
            {
                await UpdateAffiliateOrderPayment(entity);
            }
            return entities;
        }

        protected override async Task<Order> UpdateAsync(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            var entity = await base.UpdateAsync(model, uow);
            await UpdateAffiliateOrderPayment(entity);
            return entity;
        }

        protected override async Task<List<Order>> UpdateRangeAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            var entities = await base.UpdateRangeAsync(models, uow);
            foreach (var entity in entities)
            {
                await UpdateAffiliateOrderPayment(entity);
            }
            return entities;
        }

        private async Task UpdateAffiliateOrderPayment(Order entity)
        {
            if (!entity.IdAddedBy.HasValue)
            {
                AffiliateOrderPayment payment = new AffiliateOrderPayment();
                payment.IdOrder = entity.Id;
                payment.Status = AffiliateOrderPaymentStatus.NotPaid;
                //TODO - calculate commission and set is a first order or no the given customer and set IdAffiliate
                //payment.Amount =
                //payment.NewCustomerOrder =
                //payment.IdAffiliate=
                await _affiliateService.UpdateAffiliateOrderPayment(payment);
            }
        }

        public async Task<PagedList<VOrder>> GetOrdersAsync(VOrderFilter filter)
        {
            var conditions = new VOrderQuery();
            conditions = conditions.WithCustomerId(filter.IdCustomer);

            if (!filter.ShipDate)
            {
                conditions = conditions.WithCreatedDate(filter.From, filter.To);
            }
            else
            {
                conditions = conditions.WithShippedDate(filter.From, filter.To);
            }
            conditions = conditions.WithOrderStatus(filter.OrderStatus)
                .WithoutIncomplete(filter.OrderStatus)
                .WithOrderSource(filter.IdOrderSource)
                .WithPOrderType(filter.POrderType)
                .
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
                case VOrderSortPath.ShipTo:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.ShipTo)
                                : x.OrderByDescending(y => y.ShipTo);
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

            var toReturn =
                await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
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