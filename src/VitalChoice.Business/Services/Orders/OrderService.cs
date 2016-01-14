﻿using Microsoft.AspNet.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Authorize.Net.Api.Contracts.V1;
using Authorize.Net.Api.Controllers;
using Authorize.Net.Api.Controllers.Bases;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Customs;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
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
using VitalChoice.Infrastructure.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Workflow.Core;
using Microsoft.Extensions.Logging;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderService : ExtendedEcommerceDynamicService<OrderDynamic, Order, OrderOptionType, OrderOptionValue>,
        IOrderService
    {
        private readonly OrderRepository _orderRepository;
        private readonly IEcommerceRepositoryAsync<VOrder> _vOrderRepository;
        private readonly IEcommerceRepositoryAsync<VOrderWithRegionInfoItem> _vOrderWithRegionInfoItemRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skusRepository;
        private readonly ProductMapper _productMapper;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowFactory _treeFactory;
        private readonly AffiliateOrderPaymentRepository _affiliateOrderPaymentRepository;
        private readonly IEcommerceRepositoryAsync<VCustomer> _vCustomerRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<HealthwiseOrder> _healthwiseOrderRepositoryAsync;
        private readonly IEcommerceRepositoryAsync<HealthwisePeriod> _healthwisePeriodRepositoryAsync;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly IEncryptedOrderExportService _encryptedOrderExportService;
        private readonly SPEcommerceRepository _sPEcommerceRepository;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IObjectMapper<OrderPaymentMethodDynamic> _paymentMapper;
        private readonly IEcommerceRepositoryAsync<OrderToGiftCertificate> _orderToGiftCertificateRepositoryAsync;

        public OrderService(
            IEcommerceRepositoryAsync<VOrder> vOrderRepository,
            IEcommerceRepositoryAsync<VOrderWithRegionInfoItem> vOrderWithRegionInfoItemRepository,
            OrderRepository orderRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<OrderOptionValue> orderValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            ProductMapper productMapper,
            ICustomerService customerService, IWorkflowFactory treeFactory,
            ILoggerProviderExtended loggerProvider, IEcommerceRepositoryAsync<Sku> skusRepository,
            AffiliateOrderPaymentRepository affiliateOrderPaymentRepository,
            IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync,
            DirectMapper<Order> directMapper,
            DynamicExpressionVisitor queryVisitor,
            IEcommerceRepositoryAsync<HealthwiseOrder> healthwiseOrderRepositoryAsync,
            IEcommerceRepositoryAsync<HealthwisePeriod> healthwisePeriodRepositoryAsync,
            IAppInfrastructureService appInfrastructureService,
            IEncryptedOrderExportService encryptedOrderExportService,
            SPEcommerceRepository sPEcommerceRepository,
            IPaymentMethodService paymentMethodService,
            IObjectMapper<OrderPaymentMethodDynamic> paymentMapper,
            IEcommerceRepositoryAsync<OrderToGiftCertificate> orderToGiftCertificateRepositoryAsync)
            : base(
                mapper, orderRepository, orderValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, directMapper, queryVisitor)
        {
            _orderRepository = orderRepository;
            _vOrderRepository = vOrderRepository;
            _vOrderWithRegionInfoItemRepository = vOrderWithRegionInfoItemRepository;
            _adminProfileRepository = adminProfileRepository;
            _productMapper = productMapper;
            _customerService = customerService;
            _treeFactory = treeFactory;
            _skusRepository = skusRepository;
            _affiliateOrderPaymentRepository = affiliateOrderPaymentRepository;
            _vCustomerRepositoryAsync = vCustomerRepositoryAsync;
            _healthwiseOrderRepositoryAsync = healthwiseOrderRepositoryAsync;
            _healthwisePeriodRepositoryAsync = healthwisePeriodRepositoryAsync;
            _appInfrastructureService = appInfrastructureService;
            _encryptedOrderExportService = encryptedOrderExportService;
            _sPEcommerceRepository = sPEcommerceRepository;
            _paymentMethodService = paymentMethodService;
            _paymentMapper = paymentMapper;
            _orderToGiftCertificateRepositoryAsync = orderToGiftCertificateRepositoryAsync;
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
                    .ThenInclude(p => p.BillingAddress)
                    .Include(o => o.PaymentMethod)
                    .ThenInclude(p => p.BillingAddress)
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
            //TODO - should be redone on standart reading with dynamics after fixing missing data(Skus) with sort of operations
            OrderDynamic toReturn = null;
            var orderQuery = new OrderQuery().WithActualStatusOnly().WithCustomerId(customerId);
            var order = (await _orderRepository.Query(orderQuery).OrderBy(p => p.OrderByDescending(x => x.DateCreated)).SelectAsync(false)).FirstOrDefault();
            if (order != null)
            {
                toReturn = await this.SelectAsync(order.Id);
            }
            return toReturn;

            //var orderQuery = new OrderQuery().WithActualStatusOnly().WithCustomerId(customerId);

            //return await SelectFirstAsync(queryObject: orderQuery, orderBy: o => o.OrderByDescending(x => x.DateCreated));
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
            Order entity;
            Task<bool> remoteUpdateTask;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    model.PaymentMethod.IdOrder = model.Id;
                    var authTask = _paymentMethodService.AuthorizeCreditCard(model.PaymentMethod);
                    var paymentCopy = _paymentMapper.Clone<ExpandoObject>(model.PaymentMethod, o =>
                    {
                        var result = new ExpandoObject();
                        result.AddRange(o);
                        return result;
                    });
                    (await authTask).Raise();
                    entity = await base.InsertAsync(model, uow);
                    model.IdAddedBy = entity.IdEditedBy;
                    await UpdateAffiliateOrderPayment(model, uow);
                    await UpdateHealthwiseOrder(model, uow);
                    model.PaymentMethod.IdOrder = model.Id;

                    remoteUpdateTask = _encryptedOrderExportService.UpdateOrderPaymentMethodAsync(paymentCopy);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            if (!await remoteUpdateTask)
            {
                Logger.LogError("Cannot update order payment info on remote.");
            }
            return entity;
        }

        /// <summary>
        /// We don't do card authorize on collection inserts and also don't make a send to export service
        /// </summary>
        /// <param name="models"></param>
        /// <param name="uow"></param>
        /// <returns></returns>
        protected override async Task<List<Order>> InsertRangeAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            List<Order> entities;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    entities = await base.InsertRangeAsync(models, uow);
                    foreach (var model in models)
                    {
                        var entity = entities.FirstOrDefault(e => e.Id == model.Id);
                        if (entity != null)
                        {
                            model.IdAddedBy = entity.IdAddedBy;
                        }
                        await UpdateAffiliateOrderPayment(model, uow);
                        await UpdateHealthwiseOrder(model, uow);
                        model.PaymentMethod.IdOrder = model.Id;
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return entities;
        }

        protected override async Task<Order> UpdateAsync(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            Order entity;
            Task<bool> remoteUpdateTask;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    model.PaymentMethod.IdOrder = model.Id;
                    var authTask = _paymentMethodService.AuthorizeCreditCard(model.PaymentMethod);
                    var paymentCopy = _paymentMapper.Clone<ExpandoObject>(model.PaymentMethod, o =>
                    {
                        var result = new ExpandoObject();
                        result.AddRange(o);
                        return result;
                    });
                    (await authTask).Raise();
                    entity = await base.UpdateAsync(model, uow);
                    model.IdAddedBy = entity.IdAddedBy;
                    await UpdateAffiliateOrderPayment(model, uow);
                    await UpdateHealthwiseOrder(model, uow);

                    remoteUpdateTask = _encryptedOrderExportService.UpdateOrderPaymentMethodAsync(paymentCopy);

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            if (!await remoteUpdateTask)
            {
                Logger.LogError("Cannot update order payment info on remote.");
            }
            return entity;
        }

        /// <summary>
        /// We don't do card authorize on collection updates and also don't make a send to export service
        /// </summary>
        /// <param name="models"></param>
        /// <param name="uow"></param>
        /// <returns></returns>
        protected override async Task<List<Order>> UpdateRangeAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            List<Order> entities;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    entities = await base.UpdateRangeAsync(models, uow);
                    foreach (var model in models)
                    {
                        var entity = entities.FirstOrDefault(p => p.Id == model.Id);
                        if (entity != null)
                        {
                            model.IdAddedBy = entity.IdAddedBy;
                            model.PaymentMethod.IdOrder = model.Id;
                        }
                        await UpdateAffiliateOrderPayment(model, uow);
                        await UpdateHealthwiseOrder(model, uow);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return entities;
        }

        private async Task UpdateAffiliateOrderPayment(OrderDynamic model, IUnitOfWorkAsync uow)
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

                return;

                var affiliateOrderPaymentRepository = uow.RepositoryAsync<AffiliateOrderPayment>();
                var dbItem = (await affiliateOrderPaymentRepository.Query(p => p.Id == payment.Id).SelectAsync(false)).FirstOrDefault();
                if (dbItem == null)
                {
                    dbItem = new AffiliateOrderPayment();
                    dbItem.Status = AffiliateOrderPaymentStatus.NotPaid;
                    dbItem.IdAffiliate = payment.IdAffiliate;
                    dbItem.Id = payment.Id;
                    dbItem.Amount = payment.Amount;
                    dbItem.NewCustomerOrder = payment.NewCustomerOrder;

                    await _affiliateOrderPaymentRepository.InsertAsync(dbItem);
                }
                else
                {
                    if (dbItem.Status == AffiliateOrderPaymentStatus.NotPaid)
                    {
                        dbItem.Amount = payment.Amount;

                        await _affiliateOrderPaymentRepository.UpdateAsync(dbItem);
                    }
                }
            }
        }

        private async Task UpdateHealthwiseOrder(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            //model.IsHealthwise = true;
            var healthwisePeriodRepository = uow.RepositoryAsync<HealthwisePeriod>();
            var healthwiseOrderRepository = uow.RepositoryAsync<HealthwiseOrder>();
            if (!model.IdAddedBy.HasValue)
            {
                if (!model.IsHealthwise)
                {
                    healthwiseOrderRepository.Delete(model.Id);
                    await uow.SaveChangesAsync();
                }
                else
                {
                    HealthwiseOrder healthwiseOrder = (await healthwiseOrderRepository.Query(p => p.Id == model.Id).SelectAsync(false)).FirstOrDefault();
                    if (healthwiseOrder == null)
                    {
                        var maxCount = _appInfrastructureService.Get().AppSettings.HealthwisePeriodMaxItemsCount;
                        var dateNow = DateTime.Now;
                        var periods = (await healthwisePeriodRepository.Query(p => p.IdCustomer == model.Customer.Id &&
                            dateNow >= p.StartDate && dateNow < p.EndDate && !p.PaidDate.HasValue).
                            Include(p => p.HealthwiseOrders).SelectAsync(false)).ToList();

                        bool addedToPeriod = false;
                        foreach (var period in periods)
                        {
                            if (period.HealthwiseOrders.Count < maxCount)
                            {
                                healthwiseOrder = new HealthwiseOrder()
                                {
                                    Id = model.Id,
                                    IdHealthwisePeriod = period.Id
                                };
                                _healthwiseOrderRepositoryAsync.Insert(healthwiseOrder);
                                addedToPeriod = true;
                                break;
                            }
                        }
                        if (!addedToPeriod)
                        {
                            var period = new HealthwisePeriod();
                            period.IdCustomer = model.Customer.Id;
                            period.StartDate = dateNow;
                            period.EndDate = period.StartDate.AddYears(1);
                            period.HealthwiseOrders = new List<HealthwiseOrder>()
                            {
                                new HealthwiseOrder() { Id=model.Id }
                            };
                            _healthwisePeriodRepositoryAsync.InsertGraph(period);
                        }

                        await uow.SaveChangesAsync();
                    }
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
                .WithId(filter.IdString)//TODO - should be redone after adding - https://github.com/aspnet/EntityFramework/issues/2850
                .WithOrderSource(filter.IdOrderSource)
                .WithPOrderType(filter.POrderType)
                .WithCustomerType(filter.IdCustomerType).WithShippingMethod(filter.IdShippingMethod);

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

        #region HealthWiseOrders

        public ICollection<MessageInfo> ValidateUpdateHealthwiseOrder(OrderDynamic order)
        {
            List<MessageInfo> toReturn = new List<MessageInfo>();
            if (order == null)
            {
                toReturn.Add(new MessageInfo() { Message= "Invalid order #" });
            }
            if (order == null || !(order.OrderStatus == OrderStatus.Processed || order.OrderStatus == OrderStatus.Exported ||
                order.OrderStatus == OrderStatus.Shipped))
            {
                toReturn.Add(new MessageInfo() { Message = "The given order can'be flagged" });
                throw new AppValidationException("The given order can'be flagged");
            }
            if (!order.DictionaryData.ContainsKey("OrderType") || order.Data.OrderType != (int?)SourceOrderType.Web)
            {
                toReturn.Add(new MessageInfo() { Message = "The given order can'be flagged" });
            }
            return toReturn;
        }

        public async Task<bool> UpdateHealthwiseOrderWithValidationAsync(int orderId, bool isHealthwise)
        {
            var order = await this.SelectAsync(orderId);
            var messages = ValidateUpdateHealthwiseOrder(order);
            if(messages.Count>0)
            {
                throw new AppValidationException(messages);
            }

            return await UpdateHealthwiseOrderInnerAsync(order, isHealthwise);
        }

        public async Task<bool> UpdateHealthwiseOrderAsync(int orderId, bool isHealthwise)
        {
            var order = await this.SelectAsync(orderId);
            var messages = ValidateUpdateHealthwiseOrder(order);
            if (messages.Count > 0)
            {
                return false;
            }

            return await UpdateHealthwiseOrderInnerAsync(order, isHealthwise);
        }

        private async Task<bool> UpdateHealthwiseOrderInnerAsync(OrderDynamic order, bool isHealthwise)
        {
            if (!isHealthwise)
            {
                _healthwiseOrderRepositoryAsync.Delete(order.Id);
            }
            else
            {
                HealthwiseOrder healthwiseOrder = (await _healthwiseOrderRepositoryAsync.Query(p => p.Id == order.Id).SelectAsync(false)).FirstOrDefault();
                if (healthwiseOrder == null)
                {
                    var maxCount = _appInfrastructureService.Get().AppSettings.HealthwisePeriodMaxItemsCount;
                    var orderCreatedDate = order.DateCreated;
                    var periods = (await _healthwisePeriodRepositoryAsync.Query(p => p.IdCustomer == order.Customer.Id &&
                        orderCreatedDate >= p.StartDate && orderCreatedDate < p.EndDate && !p.PaidDate.HasValue).
                        Include(p => p.HealthwiseOrders).SelectAsync(false)).ToList();
                    bool addedToPeriod = false;
                    foreach (var period in periods)
                    {
                        if (period.HealthwiseOrders.Count < maxCount)
                        {
                            healthwiseOrder = new HealthwiseOrder()
                            {
                                Id = order.Id,
                                IdHealthwisePeriod = period.Id
                            };
                            _healthwiseOrderRepositoryAsync.Insert(healthwiseOrder);
                            addedToPeriod = true;
                            break;
                        }
                    }
                    if (!addedToPeriod)
                    {
                        var period = new HealthwisePeriod();
                        period.IdCustomer = order.Customer.Id;
                        period.StartDate = orderCreatedDate;
                        period.EndDate = period.StartDate.AddYears(1);
                        period.HealthwiseOrders = new List<HealthwiseOrder>()
                        {
                            new HealthwiseOrder() { Id=order.Id }
                        };
                        _healthwisePeriodRepositoryAsync.InsertGraph(period);
                    }
                }
            }

            return true;
        }

        #endregion

        #region OrdersStatistic

        public async Task<ICollection<OrdersRegionStatisticItem>> GetOrdersRegionStatisticAsync(OrderRegionFilter filter)
        {
            return await _sPEcommerceRepository.GetOrdersRegionStatisticAsync(filter);
        }

        public async Task<ICollection<OrdersZipStatisticItem>> GetOrdersZipStatisticAsync(OrderRegionFilter filter)
        {
            return await _sPEcommerceRepository.GetOrdersZipStatisticAsync(filter);
        }

        public async Task<PagedList<VOrderWithRegionInfoItem>> GetOrderWithRegionInfoItemsAsync(OrderRegionFilter filter)
        {
            VOrderWithRegionInfoItemQuery conditions = new VOrderWithRegionInfoItemQuery().WithDates(filter.From, filter.To).
                WithIdCustomerType(filter.IdCustomerType).WithIdOrderType(filter.IdOrderType).WithRegion(filter.Region).WithZip(filter.Zip);
            Func<IQueryable<VOrderWithRegionInfoItem>, IOrderedQueryable<VOrderWithRegionInfoItem>> sortable = p => p.OrderByDescending(x => x.Id);

            var query = _vOrderWithRegionInfoItemRepository.Query(conditions).OrderBy(sortable);
            PagedList<VOrderWithRegionInfoItem> toReturn = null;
            if (filter.Paging != null)
            {
                toReturn = await query.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            }
            else
            {
                var items = await query.SelectAsync(false);
                toReturn = new PagedList<VOrderWithRegionInfoItem>(items, items.Count);
            }

            if (toReturn.Count > 0)
            {
                var customerConditions = new VCustomerQuery().NotDeleted().WithIds(toReturn.Items.Select(p => p.IdCustomer).Distinct().ToList());
                var customers = (await _vCustomerRepositoryAsync.Query(customerConditions).SelectAsync(false));

                foreach (var item in toReturn.Items)
                {
                    var customer = customers.FirstOrDefault(p => p.Id == item.IdCustomer);
                    if (customer != null)
                    {
                        item.CustomerFirstName = customer.FirstName;
                        item.CustomerLastName = customer.LastName;
                        item.CustomerOrdersCount = customer.TotalOrders;
                    }
                }
            }

            return toReturn;
        }

        public async Task<decimal> GetOrderWithRegionInfoAmountAsync(OrderRegionFilter filter)
        {
            return await _orderRepository.GetOrderWithRegionInfoAmountAsync(filter);
        }

        #endregion

        #region GCOrders
        
        public async Task<ICollection<GCOrderItem>> GetGCOrdersAsync(int idGC)
        {
            List<GCOrderItem> toReturn = new List<GCOrderItem>();

            var orderToGCs = await _orderToGiftCertificateRepositoryAsync.Query(p => p.IdGiftCertificate == idGC).SelectAsync(false);
            var orders = (await SelectAsync(orderToGCs.Select(p => p.IdOrder).ToList())).OrderByDescending(p=>p.DateCreated);
            foreach (var orderToGC in orderToGCs)
            {
                var order = orders.FirstOrDefault(p => p.Id == orderToGC.IdOrder);
                if(order!=null)
                {
                    GCOrderItem item = new GCOrderItem();
                    item.Order = order;
                    item.GCAmountUsed = orderToGC.Amount;
                    toReturn.Add(item);
                }
            }
            var ids = toReturn.Select(p => p.Order.IdEditedBy).ToList();
            var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync();
            foreach (var item in toReturn)
            {
                foreach (var profile in profiles)
                {
                    if (item.Order.IdEditedBy == profile.Id)
                    {
                        item.EditedBy = profile.AgentId;
                    }
                }
            }

            return toReturn;
        }

        #endregion
    }
}