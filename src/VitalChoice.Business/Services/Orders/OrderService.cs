﻿using Microsoft.AspNet.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.History;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
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
        private readonly AffiliateOrderPaymentRepository _affiliateOrderPaymentRepository;
        private readonly IEcommerceRepositoryAsync<VCustomer> _vCustomerRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<HealthwiseOrder> _healthwiseOrderRepositoryAsync;
        private readonly IAppInfrastructureService _appInfrastructureService;

        public OrderService(IEcommerceRepositoryAsync<VOrder> vOrderRepository,
            IEcommerceRepositoryAsync<Order> orderRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<OrderOptionValue> orderValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IEcommerceRepositoryAsync<ProductOptionType> productOptionTypesRepository, ProductMapper productMapper,
            ICustomerService customerService, IWorkflowFactory treeFactory,
            ILoggerProviderExtended loggerProvider, IEcommerceRepositoryAsync<Sku> skusRepository,
            IAffiliateService affiliateService,
            AffiliateOrderPaymentRepository affiliateOrderPaymentRepository,
            IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync, DirectMapper<Order> directMapper, DynamicExpressionVisitor queryVisitor,
            IAppInfrastructureService appInfrastructureService)
            : base(
                mapper, orderRepository, orderValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, directMapper, queryVisitor)
        {
            _vOrderRepository = vOrderRepository;
            _adminProfileRepository = adminProfileRepository;
            _productOptionTypesRepository = productOptionTypesRepository;
            _productMapper = productMapper;
            _customerService = customerService;
            _treeFactory = treeFactory;
            _skusRepository = skusRepository;
            _affiliateService = affiliateService;
            _affiliateOrderPaymentRepository = affiliateOrderPaymentRepository;
            _vCustomerRepositoryAsync = vCustomerRepositoryAsync;
            _appInfrastructureService = appInfrastructureService;
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

        protected override async Task AfterSelect(ICollection<Order> entities)
        {
            if (entities.All(e => e.Skus != null))
            {
                foreach (
                    var orderToSku in
                        entities.SelectMany(o => o.Skus).Where(s => s.Sku?.Product != null && s.Sku.OptionTypes == null))
                {
                    var optionTypes = _productMapper.FilterByType(orderToSku.Sku.Product.IdObjectType);
                    orderToSku.Sku.OptionTypes = optionTypes;
                    orderToSku.Sku.Product.OptionTypes = optionTypes;
                }
                var invalidSkuOrdered =
                    entities.SelectMany(o => o.Skus)
                        .Where(s => s.Sku?.Product == null || s.Sku.OptionTypes == null)
                        .ToArray();
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
                        var optionTypes = _productMapper.FilterByType(sku.Product.IdObjectType);
                        orderToSku.Sku = sku;
                        orderToSku.Sku.Product = sku.Product;
                        orderToSku.Sku.OptionTypes = optionTypes;
                        orderToSku.Sku.Product.OptionTypes = optionTypes;
                    }
                }
            }
        }

        protected override void LogItemInfoSetAfter(ObjectHistoryLogItem log, OrderDynamic model)
        {
            log.IdObjectStatus = (int)model.OrderStatus;
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

        protected override Task<List<MessageInfo>> ValidateAsync(OrderDynamic dynamic)
        {
            if (dynamic.Customer.StatusCode == (int)CustomerStatus.Suspended)
            {
                throw new AppValidationException(
                    ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SuspendedCustomer]);
            }
            return base.ValidateAsync(dynamic);
        }

        public async Task<int?> GetOrderIdCustomer(int id)
        {
            var order = (await this.ObjectRepository.Query(p => p.StatusCode != (int)RecordStatusCode.Deleted && p.Id == id).SelectAsync(false)).FirstOrDefault();
            return order?.IdCustomer;
        }

        public async Task<PagedList<Order>> GetShortOrdersAsync(ShortOrderFilter filter)
        {
            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable;

            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case OrderSortPath.OrderDate:
                    if (sortOrder == SortOrder.Desc)
                    {
                        sortable = x => x.OrderByDescending(y => y.DateCreated);
                    }
                    else
                    {
                        sortable = x => x.OrderBy(y => y.DateCreated);
                    }
                    break;
                default:
                    if (sortOrder == SortOrder.Desc)
                    {
                        sortable = x => x.OrderByDescending(y => y.Id);
                    }
                    else
                    {
                        sortable = x => x.OrderBy(y => y.Id);
                    }
                    break;
            }

            var orderQuery = new OrderQuery().WithCustomerId(filter.IdCustomer).FilterById(filter.Id).NotDeleted();

            var query =
                this.ObjectRepository.Query(orderQuery)
                    .OrderBy(sortable);
            return await query.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
        }

        protected override async Task<Order> InsertAsync(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    var entity = await base.InsertAsync(model, uow);
                    model.IdAddedBy = model.IdEditedBy;
                    UpdateAffiliateOrderPayment(model, uow);
                    await UpdateHealthwiseOrder(model, uow);

                    transaction.Commit();
                    return entity;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected override async Task<List<Order>> InsertRangeAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    var entities = await base.InsertRangeAsync(models, uow);
                    foreach (var model in models)
                    {
                        model.IdAddedBy = model.IdEditedBy;
                        UpdateAffiliateOrderPayment(model, uow);
                        await UpdateHealthwiseOrder(model, uow);
                    }

                    transaction.Commit();
                    return entities;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected override async Task<Order> UpdateAsync(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    var entity = await base.UpdateAsync(model, uow);
                    model.IdAddedBy = entity.IdAddedBy;
                    UpdateAffiliateOrderPayment(model, uow);
                    await UpdateHealthwiseOrder(model, uow);

                    transaction.Commit();
                    return entity;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        protected override async Task<List<Order>> UpdateRangeAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    var entities = await base.UpdateRangeAsync(models, uow);
                    foreach (var model in models)
                    {
                        var entity = entities.FirstOrDefault(p => p.Id == model.Id);
                        if (entity != null)
                        {
                            model.IdAddedBy = entity.IdAddedBy;
                        }
                        UpdateAffiliateOrderPayment(model, uow);
                        await UpdateHealthwiseOrder(model, uow);
                    }

                    transaction.Commit();
                    return entities;
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }

        private void UpdateAffiliateOrderPayment(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            if (!model.IdAddedBy.HasValue && model.Customer.IdAffiliate.HasValue)
            {
                AffiliateOrderPayment payment = new AffiliateOrderPayment();
                payment.Id = model.Id;
                payment.Status = AffiliateOrderPaymentStatus.NotPaid;
                payment.IdAffiliate = model.Customer.IdAffiliate.Value;
                //TODO - calculate commission and set is a first order or no the given customer
                //payment.Amount =
                //payment.NewCustomerOrder =
            }
        }

        private async Task UpdateHealthwiseOrder(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            model.IsHealthwise = true;
            var healthwisePeriodRepository = uow.RepositoryAsync<HealthwisePeriod>();
            var healthwiseOrderRepository = uow.RepositoryAsync<HealthwiseOrder>();
            if (!model.IsHealthwise)
            {
                healthwiseOrderRepository.Delete(model.Id);
            }
            else
            {
                HealthwiseOrder healthwiseOrder = (await healthwiseOrderRepository.Query(p=>p.Id==model.Id).SelectAsync(false)).FirstOrDefault();
                if(healthwiseOrder==null)
                {
                    var maxCount = _appInfrastructureService.Get().AppSettings.HealthwisePeriodMaxItemsCount;
                    var dateNow = DateTime.Now;
                    var period = (await healthwisePeriodRepository.Query(p => p.IdCustomer == model.Customer.Id && dateNow >= p.StartDate && dateNow < p.EndDate).Include(p => p.HealthwiseOrders).SelectAsync(false)).FirstOrDefault();
                    if (period == null || period.HealthwiseOrders.Count >= maxCount)
                    {
                        period = new HealthwisePeriod();
                        period.IdCustomer = model.Customer.Id;
                        period.StartDate = dateNow;
                        period.EndDate = period.StartDate.AddYears(1);
                        period.HealthwiseOrders = new List<HealthwiseOrder>()
                        {
                            new HealthwiseOrder() { Id=model.Id }
                        };
                        healthwisePeriodRepository.InsertGraph(period);
                    }
                    else
                    {
                        healthwiseOrder = new HealthwiseOrder() {
                            Id = model.Id,
                            IdHealthwisePeriod=period.Id
                        };
                        healthwiseOrderRepository.Insert(healthwiseOrder);
                    }
                    await uow.SaveChangesAsync();
                }
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

        #region AffiliatesOrders

        public async Task<PagedList<AffiliateOrderListItemModel>> GetAffiliateOrderPaymentsWithCustomerInfo(AffiliateOrderPaymentFilter filter)
        {
            PagedList<AffiliateOrderListItemModel> toReturn = new PagedList<AffiliateOrderListItemModel>();

            OrderQuery conditions = new OrderQuery().WithIdAffiliate(filter.IdAffiliate).WithPaymentStatus(filter.Status).
                WithAffiliateOrderStatus().WithFromDate(filter.From).WithToDate(filter.To);
            Func<IQueryLite<Order>, IQueryLite<Order>> includes = (p => p.Include(o => o.PaymentMethod)
                                                                         .ThenInclude(o => o.BillingAddress)
                                                                         .ThenInclude(o => o.OptionValues)
                                                                         .Include(o => o.PaymentMethod)
                                                                         .ThenInclude(o => o.OptionValues)
                                                                         .Include(o => o.PaymentMethod)
                                                                         .ThenInclude(o => o.PaymentMethod)
                                                                         .Include(o => o.AffiliateOrderPayment));

            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var result = await this.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount,
                queryObject: conditions, orderBy: sortable, includesOverride: includes);

            var customerOrders = await _affiliateOrderPaymentRepository.GetAffiliateOrdersInCustomers(filter.IdAffiliate, result.Items.Select(p => p.Customer.Id).Distinct().ToList());

            List<int> customerIds = new List<int>();
            toReturn.Count = result.Count;
            toReturn.Items = new List<AffiliateOrderListItemModel>();
            string customerFirstName = null;
            string customerLastName = null;
            int customerOrdersCount = 0;
            foreach (var order in result.Items)
            {
                customerFirstName = null;
                customerLastName = null;
                customerOrdersCount = 0;
                if (order.PaymentMethod != null && order.PaymentMethod.Address != null &&
                    order.PaymentMethod.Address.DictionaryData.ContainsKey("FirstName") &&
                    order.PaymentMethod.Address.DictionaryData.ContainsKey("LastName"))
                {
                    customerFirstName = order.PaymentMethod.Address.Data.FirstName;
                    customerLastName = order.PaymentMethod.Address.Data.LastName;
                }
                else
                {
                    customerIds.Add(order.Customer.Id);
                }
                if (customerOrders.ContainsKey(order.Customer.Id))
                {
                    customerOrdersCount = customerOrders[order.Customer.Id];
                }

                AffiliateOrderListItemModel item = new AffiliateOrderListItemModel(order.AffiliateOrderPayment, customerFirstName, customerLastName,
                    customerOrdersCount);
                toReturn.Items.Add(item);
            }

            if (customerIds.Count > 0)
            {
                var customerConditions = new VCustomerQuery().NotDeleted().WithIds(customerIds.Distinct().ToList());
                var customers = (await _vCustomerRepositoryAsync.Query(customerConditions).SelectAsync(false));

                foreach (var item in toReturn.Items)
                {
                    if (String.IsNullOrEmpty(item.CustomerName))
                    {
                        var customer = customers.FirstOrDefault(p => p.Id == item.IdCustomer);
                        if (customer != null)
                        {
                            item.CustomerName = customer.FirstName + " " + customer.LastName;
                        }
                    }
                }
            }

            return toReturn;
        }

        #endregion
    }
}