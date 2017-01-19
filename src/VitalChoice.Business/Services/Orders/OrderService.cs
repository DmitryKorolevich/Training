using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Business.Queries.Orders;
using VitalChoice.Business.Repositories;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Ecommerce;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Validation;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Affiliates;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Workflow.Core;
using Microsoft.Extensions.Logging;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Business.Helpers;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Exceptions;
using VitalChoice.Infrastructure.Extensions;
using VitalChoice.Business.Mailings;
using VitalChoice.Data.UOW;
using VitalChoice.Infrastructure.Domain.Entities.Checkout;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderService : ExtendedEcommerceDynamicService<OrderDynamic, Order, OrderOptionType, OrderOptionValue>,
        IOrderService
    {
        private readonly IEcommerceRepositoryAsync<VOrderWithRegionInfoItem> _vOrderWithRegionInfoItemRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly ProductMapper _productMapper;
        private readonly SkuMapper _skuMapper;
        private readonly CustomerMapper _customerMapper;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowFactory _treeFactory;
        private readonly IEcommerceRepositoryAsync<VCustomer> _vCustomerRepositoryAsync;
        private readonly IEncryptedOrderExportService _encryptedOrderExportService;
        private readonly SpEcommerceRepository _sPEcommerceRepository;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IEcommerceRepositoryAsync<OrderToGiftCertificate> _orderToGiftCertificateRepositoryAsync;
        private readonly ICountryNameCodeResolver _countryService;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly
            IExtendedDynamicServiceAsync
                <OrderPaymentMethodDynamic, OrderPaymentMethod, CustomerPaymentMethodOptionType, OrderPaymentMethodOptionValue>
            _paymentGenericService;
        private readonly IEcommerceRepositoryAsync<OrderToSku> _orderToSkusRepository;
        private readonly IDiscountService _discountService;
	    private readonly IEcommerceRepositoryAsync<VAutoShip> _vAutoShipRepository;
	    private readonly IEcommerceRepositoryAsync<VAutoShipOrder> _vAutoShipOrderRepository;
        private readonly AffiliateOrderPaymentRepository _affiliateOrderPaymentRepository;
        private readonly ICountryNameCodeResolver _codeResolver;
        private readonly ReferenceData _referenceData;
        private readonly OrderRepository _orderRepository;
        private readonly AppSettings _appSettings;
        private readonly IAdminEditLockService _lockService;
        private readonly ILoggerFactory _loggerFactory;

        public OrderService(
            IEcommerceRepositoryAsync<VOrderWithRegionInfoItem> vOrderWithRegionInfoItemRepository,
            OrderRepository orderRepository,
            IEcommerceRepositoryAsync<BigStringValue> bigStringValueRepository,
            OrderMapper mapper,
            IObjectLogItemExternalService objectLogItemExternalService,
            IEcommerceRepositoryAsync<OrderOptionValue> orderValueRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            ProductMapper productMapper,
            CustomerMapper customerMapper,
            ICustomerService customerService, IWorkflowFactory treeFactory,
            ILoggerFactory loggerProvider,
            IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync,
            DynamicExtensionsRewriter queryVisitor,
            IEncryptedOrderExportService encryptedOrderExportService,
            SpEcommerceRepository sPEcommerceRepository,
            IPaymentMethodService paymentMethodService,
            IEcommerceRepositoryAsync<OrderToGiftCertificate> orderToGiftCertificateRepositoryAsync,
            IExtendedDynamicServiceAsync
                <OrderPaymentMethodDynamic, OrderPaymentMethod, CustomerPaymentMethodOptionType, OrderPaymentMethodOptionValue>
                paymentGenericService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            IProductService productService,
            INotificationService notificationService,
            ICountryNameCodeResolver countryService, ITransactionAccessor<EcommerceContext> transactionAccessor, SkuMapper skuMapper,
            IEcommerceRepositoryAsync<OrderToSku> orderToSkusRepository, IDiscountService discountService,
            IEcommerceRepositoryAsync<VAutoShip> vAutoShipRepository, IEcommerceRepositoryAsync<VAutoShipOrder> vAutoShipOrderRepository,
            AffiliateOrderPaymentRepository affiliateOrderPaymentRepository, ICountryNameCodeResolver codeResolver,
            IDynamicEntityOrderingExtension<Order> orderingExtension, ReferenceData referenceData, AppSettings appSettings, IAdminEditLockService lockService)
            : base(
                mapper, orderRepository, orderValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, queryVisitor, transactionAccessor, orderingExtension
                )
        {
            _vOrderWithRegionInfoItemRepository = vOrderWithRegionInfoItemRepository;
            _adminProfileRepository = adminProfileRepository;
            _productMapper = productMapper;
            _customerService = customerService;
            _treeFactory = treeFactory;
            _vCustomerRepositoryAsync = vCustomerRepositoryAsync;
            _encryptedOrderExportService = encryptedOrderExportService;
            _sPEcommerceRepository = sPEcommerceRepository;
            _paymentMethodService = paymentMethodService;
            _customerMapper = customerMapper;
            _orderToGiftCertificateRepositoryAsync = orderToGiftCertificateRepositoryAsync;
            _paymentGenericService = paymentGenericService;
            _countryService = countryService;
            _skuMapper = skuMapper;
            _orderToSkusRepository = orderToSkusRepository;
            _discountService = discountService;
            _vAutoShipRepository = vAutoShipRepository;
            _vAutoShipOrderRepository = vAutoShipOrderRepository;
            _affiliateOrderPaymentRepository = affiliateOrderPaymentRepository;
            _codeResolver = codeResolver;
            _referenceData = referenceData;
            _appSettings = appSettings;
            _lockService = lockService;
            _orderRepository = orderRepository;
            _addressMapper = addressMapper;
            _productService = productService;
            _notificationService = notificationService;
            _loggerFactory = loggerProvider;
        }

        private async Task<Order> InsertAsyncInternal(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            Order entity;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    SetExtendedOptions(Enumerable.Repeat(model, 1));
                    await SetSkusBornDate(new[] {model}, uow);
                    EnsurePaymentMethod(model);
                    var authTask = _paymentMethodService.AuthorizeCreditCard(model.PaymentMethod);
                    var paymentCopy = new OrderCardData
                    {
                        CardNumber = model.PaymentMethod.SafeData.CardNumber,
                        IdCustomerPaymentMethod = model.PaymentMethod.IdCustomerPaymentMethod,
                        IdOrderSource = model.PaymentMethod.IdOrderSource
                    };
                    (await authTask).Raise();
                    entity = await base.InsertAsync(model, uow);

                    paymentCopy.IdOrder = entity.Id;
                    if (!await _encryptedOrderExportService.UpdateOrderPaymentMethodAsync(paymentCopy))
                    {
                        throw new ApiException("Cannot update order payment info on remote.");
                    }

                    //storefront update
                    if (model.IsAnyNotIncomplete())
                    {
                        await UpdateAffiliateOrderPayment(model, uow);
                        await UpdateHealthwiseOrderWithOrder(model, uow);
                        await ChargeGiftCertificates(model, uow);
                        await ChargeOnetimeDiscount(model);
                        await uow.SaveChangesAsync();
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            
            return entity;
        }

        /// <summary>
        /// We don't do card authorize on collection inserts and also don't make a send to export service
        /// </summary>
        /// <param name="models"></param>
        /// <param name="uow"></param>
        /// <returns></returns>
        private async Task<List<Order>> InsertRangeInternalAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            List<Order> entities;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    SetExtendedOptions(models);
                    await SetSkusBornDate(models, uow);
                    var paymentCopies = models.Select(p => new OrderPaymentReference
                    {
                        OriginalReference = p,
                        PaymentMethod = new OrderCardData
                        {
                            CardNumber = p.PaymentMethod.SafeData.CardNumber,
                            IdCustomerPaymentMethod = p.PaymentMethod.IdCustomerPaymentMethod,
                            IdOrderSource = p.PaymentMethod.IdOrderSource
                        }
                    }).ToArray();
                    entities = await base.InsertRangeAsync(models, uow);

                    paymentCopies.ForEach(p => p.PaymentMethod.IdOrder = p.OriginalReference.Id);
                    var paymentRemoteUpdates =
                        paymentCopies.Select(p => _encryptedOrderExportService.UpdateOrderPaymentMethodAsync(p.PaymentMethod));

                    if ((await Task.WhenAll(paymentRemoteUpdates)).Any(t => !t))
                    {
                        throw new ApiException("Cannot update order payment info on remote.");
                    }

                    foreach (var model in models)
                    {
                        var entity = entities.FirstOrDefault(e => e.Id == model.Id);
                        //storefront update
                        if (entity != null)
                        {
                            await UpdateAffiliateOrderPayment(model, uow);
                            await UpdateHealthwiseOrderWithOrder(model, uow);
                        }
                        await ChargeGiftCertificates(model, uow);
                    }
                    await uow.SaveChangesAsync();
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

        private async Task<Order> UpdateInternalAsync(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            Order entity;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    SetExtendedOptions(Enumerable.Repeat(model, 1));
                    await SetSkusBornDate(new[] {model}, uow);
                    EnsurePaymentMethod(model);
                    (await _paymentMethodService.AuthorizeCreditCard(model.PaymentMethod)).Raise();
                    var paymentCopy = new OrderCardData
                    {
                        CardNumber = model.PaymentMethod.SafeData.CardNumber,
                        IdOrder = model.Id,
                        IdCustomerPaymentMethod = model.PaymentMethod.IdCustomerPaymentMethod,
                        IdOrderSource = model.PaymentMethod.IdOrderSource
                    };
                    var initial = await SelectEntityFirstAsync(o => o.Id == model.Id, query => query);
                    entity = await base.UpdateAsync(model, uow);

                    paymentCopy.IdOrder = entity.Id;
                    if (!await _encryptedOrderExportService.UpdateOrderPaymentMethodAsync(paymentCopy))
                    {
                        throw new ApiException("Cannot update order payment info on remote.");
                    }

                    //Update date created if order was incomplete and become processed
                    if (initial.IsAnyIncomplete() && model.IsAnyNotIncomplete())
                    {
                        entity.DateCreated = DateTime.Now;
                        var cartRepository = uow.RepositoryAsync<CartExtended>();
                        var carts = await cartRepository.Query(c => c.IdOrder == entity.Id).SelectAsync(true);
                        foreach (var cart in carts)
                        {
                            cart.IdOrder = null;
                        }
                        await uow.SaveChangesAsync();
                        if (model.Discount?.Id > 0)
                        {
                            await _discountService.SetDiscountUsed(model.Discount, model.Customer.Id);
                        }
                    }
                    else if (model.IsAnyNotIncomplete())
                    {
                        await ChargeOnetimeDiscount(model, initial);
                    }
                    if (model.IsAnyNotIncomplete())
                    {
                        //storefront update
                        await UpdateAffiliateOrderPayment(model, uow);
                        await UpdateHealthwiseOrderWithOrder(model, uow);
                        //charge one-time discount, remove old charge if different
                    }
                    await uow.SaveChangesAsync();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return entity;
        }

        private async Task ChargeOnetimeDiscount(OrderDynamic model, Order initial = null)
        {
            if (model.Discount?.Id != initial?.IdDiscount)
            {
                if (initial?.IdDiscount != null)
                {
                    var discount = await _discountService.SelectAsync(initial.IdDiscount.Value, true);
                    await _discountService.SetDiscountUsed(discount, initial.IdCustomer, false);
                }
                else if (model.Discount?.Id > 0)
                {
                    await _discountService.SetDiscountUsed(model.Discount, model.Customer.Id);
                }
            }
        }

        /// <summary>
        /// We don't do card authorize on collection updates and also don't make a send to export service
        /// </summary>
        /// <param name="models"></param>
        /// <param name="uow"></param>
        /// <returns></returns>
        private async Task<List<Order>> UpdateRangeInternalAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            List<Order> entities;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    SetExtendedOptions(models);
                    await SetSkusBornDate(models, uow);
                    var ids = models.Select(o => o.Id).Distinct().ToList();
                    var paymentCopies = models.Select(p => new OrderCardData
                    {
                        CardNumber = p.PaymentMethod.SafeData.CardNumber,
                        IdCustomerPaymentMethod = p.PaymentMethod.IdCustomerPaymentMethod,
                        IdOrderSource = p.PaymentMethod.IdOrderSource,
                        IdOrder = p.Id
                    }).ToArray();
                    var initialList = await SelectEntitiesAsync(o => ids.Contains(o.Id), query => query);
                    entities = await base.UpdateRangeAsync(models, uow);

                    var paymentRemoteUpdates =
                        paymentCopies.Select(p => _encryptedOrderExportService.UpdateOrderPaymentMethodAsync(p));

                    if ((await Task.WhenAll(paymentRemoteUpdates)).Any(t => !t))
                    {
                        throw new ApiException("Cannot update order payment info on remote.");
                    }

                    foreach (var model in models)
                    {
                        var entity = entities.FirstOrDefault(p => p.Id == model.Id);
                        var initial = initialList.FirstOrDefault(o => o.Id == model.Id);

                        if (entity != null && initial != null && initial.IsAnyIncomplete() && model.IsAnyNotIncomplete())
                        {
                            entity.DateCreated = DateTime.Now;
                        }

                        //storefront update
                        await UpdateAffiliateOrderPayment(model, uow);
                        await UpdateHealthwiseOrderWithOrder(model, uow);
                    }
                    await uow.SaveChangesAsync();
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

        private async Task<Order> FindAutoShipToChangeStatusAsync(int customerId, int autoShipId)
        {
            var orderQuery = new OrderQuery().WithCustomerId(customerId).WithId(autoShipId).NotDeleted().WithOrderType(OrderType.AutoShip);

            var autoShip = await _orderRepository.Query(orderQuery).SelectFirstOrDefaultAsync(true);
            if (autoShip == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AutoShipNotAvailable]);
            }

            return autoShip;
        }

        private Func<IQueryable<Order>, IOrderedQueryable<Order>> PerformOrderSorting(FilterBase filter)
        {
            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable;

            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case OrderSortPath.OrderDate:
                    if (sortOrder == FilterSortOrder.Desc)
                    {
                        sortable = x => x.OrderByDescending(y => y.DateCreated);
                    }
                    else
                    {
                        sortable = x => x.OrderBy(y => y.DateCreated);
                    }
                    break;
                case OrderSortPath.DateEdited:
                    if (sortOrder == FilterSortOrder.Desc)
                    {
                        sortable = x => x.OrderByDescending(y => y.DateEdited);
                    }
                    else
                    {
                        sortable = x => x.OrderBy(y => y.DateEdited);
                    }
                    break;
                default:
                    if (sortOrder == FilterSortOrder.Desc)
                    {
                        sortable = x => x.OrderByDescending(y => y.Id);
                    }
                    else
                    {
                        sortable = x => x.OrderBy(y => y.Id);
                    }
                    break;
            }

            return sortable;
        }

        protected override IQueryLite<Order> BuildIncludes(IQueryLite<Order> query)
        {
            return
                query.Include(o => o.Discount)
                    .ThenInclude(d => d.OptionValues)
                    .Include(o => o.Discount)
                    .ThenInclude(d => d.DiscountTiers)
                    .Include(o => o.Discount)
                    .ThenInclude(p => p.DiscountsToSelectedSkus)
                    .Include(o => o.Discount)
                    .ThenInclude(p => p.DiscountsToSkus)
                    .Include(o => o.Discount)
                    .ThenInclude(p => p.DiscountsToCategories)
                    .Include(o => o.Discount)
                    .ThenInclude(p => p.DiscountsToSelectedCategories)
                    .Include(o => o.GiftCertificates)
                    .ThenInclude(g => g.GiftCertificate)
                    .Include(o => o.PromoSkus)
                    .ThenInclude(p => p.Sku)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.PromoSkus)
                    .ThenInclude(p => p.Sku)
                    .ThenInclude(s => s.Product)
                    .ThenInclude(p => p.OptionValues)
                    .Include(o => o.PromoSkus)
                    .ThenInclude(p => p.Promo)
                    .ThenInclude(s => s.OptionValues)
                    .Include(o => o.PromoSkus)
                    .ThenInclude(s => s.InventorySkus)
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
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.Product)
                    .ThenInclude(s => s.ProductsToCategories)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.InventorySkus)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.GeneratedGiftCertificates)
                    .Include(o => o.HealthwiseOrder)
                    .Include(o => o.ReshipProblemSkus)
                    .ThenInclude(g => g.Sku)
                    .Include(o => o.OrderShippingPackages);
        }

        protected override Task AfterSelect(ICollection<Order> entities)
        {
            if (entities.All(e => e.Skus != null))
            {
                foreach (
                    var orderToSku in
                        entities.SelectMany(o => o.Skus).Where(s => s.Sku?.Product != null && s.Sku.OptionTypes == null))
                {
                    var optionTypes = _productMapper.FilterByType(orderToSku.Sku.Product.IdObjectType);
                    orderToSku.Sku.OptionTypes = _skuMapper.FilterByType(orderToSku.Sku.Product.IdObjectType);
                    orderToSku.Sku.Product.OptionTypes = optionTypes;
                }
            }
            if (entities.All(e => e.PromoSkus != null))
            {
                foreach (
                    var orderToSku in
                        entities.SelectMany(o => o.PromoSkus).Where(s => s.Sku?.Product != null && s.Sku.OptionTypes == null))
                {
                    var optionTypes = _productMapper.FilterByType(orderToSku.Sku.Product.IdObjectType);
                    orderToSku.Sku.OptionTypes = _skuMapper.FilterByType(orderToSku.Sku.Product.IdObjectType);
                    orderToSku.Sku.Product.OptionTypes = optionTypes;
                }
            }
            return TaskCache.CompletedTask;
        }

        protected override async Task AfterEntityChangesAsync(OrderDynamic model, Order updated, IUnitOfWorkAsync uow)
        {
            //TODO: We need to manually remove generated but unlinked gift certificates
            var gcRep = uow.RepositoryAsync<GiftCertificate>();
            if ((model.OrderStatus == null || model.OrderStatus.Value != OrderStatus.Incomplete) &&
                (model.POrderStatus != OrderStatus.Incomplete || model.NPOrderStatus != OrderStatus.Incomplete))
            {
                var toLoadUp =
                    new HashSet<int>(updated.GiftCertificates.Where(g => g.GiftCertificate == null).Select(g => g.IdGiftCertificate));
                List<GiftCertificate> gcs = null;
                if (toLoadUp.Count > 0)
                {
                    gcs = await gcRep.Query(g => toLoadUp.Contains(g.Id)).SelectAsync(true);
                }
                updated.GiftCertificates.ForEach(g =>
                {
                    if (g.GiftCertificate == null)
                    {
                        g.GiftCertificate = gcs?.FirstOrDefault(db => db.Id == g.IdGiftCertificate);
                    }
                    if (g.GiftCertificate != null)
                    {
                        g.GiftCertificate.Balance =
                            model.GiftCertificates.Where(dyn => dyn.GiftCertificate.Id == g.IdGiftCertificate)
                                .Select(dyn => dyn.GiftCertificate.Balance).FirstOrDefault();
                    }
                });
            }
        }

        protected override bool LogObjectFullData => true;

        public async Task LogOrderUpdateAsync(int id)
        {
            var entity = await SelectEntityFirstAsync(o => o.Id == id);
            if (entity != null)
            {
                await LogItemChanges(new[] {await DynamicMapper.FromEntityAsync(entity)});
            }
        }

        public async Task<OrderDynamic> SelectWithCustomerAsync(int id, bool withDefaults = false)
        {
            var order = await SelectAsync(id, withDefaults);
            if (order != null)
            {
                order.Customer = await _customerService.SelectAsync(order.Customer.Id, withDefaults);
            }
            return order;
        }

        public async Task<OrderDataContext> CalculateStorefrontOrder(OrderDynamic order, OrderStatus combinedStatus)
        {
            if (combinedStatus == OrderStatus.Incomplete)
            {
                if (order.PromoSkus != null)
                {
                    foreach (var promo in order.PromoSkus)
                    {
                        promo.Enabled = true;
                    }
                }
            }
            var context = new OrderDataContext(combinedStatus)
            {
                Order = order,
                CheckForFraud = combinedStatus != OrderStatus.Incomplete
            };
            var tree = await _treeFactory.CreateTreeAsync<OrderDataContext, decimal>("Order");
            await tree.ExecuteAsync(context);
            UpdateOrderFromCalculationContext(order, context);
            return context;
        }

        public async Task<OrderDataContext> CalculateOrderForExport(OrderDynamic order, OrderStatus combinedStatus)
        {
            var context = new OrderDataContext(combinedStatus)
            {
                Order = order
            };
            IWorkflowTreeExecutor<OrderDataContext, decimal> tree;
            switch ((OrderType) order.IdObjectType)
            {
                case OrderType.Normal:
                case OrderType.AutoShip:
                case OrderType.DropShip:
                case OrderType.GiftList:
                case OrderType.AutoShipOrder:
                    tree = await _treeFactory.CreateTreeAsync<OrderDataContext, decimal>("ExportOrder");
                    break;
                case OrderType.Reship:
                    tree = await _treeFactory.CreateTreeAsync<OrderDataContext, decimal>("Reship");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await tree.ExecuteAsync(context);
            UpdateOrderFromCalculationContext(order, context);
            return context;
        }

        public async Task<OrderDataContext> CalculateOrder(OrderDynamic order, OrderStatus combinedStatus, bool checkForFraud)
        {
            var context = new OrderDataContext(combinedStatus)
            {
                Order = order, 
                CheckForFraud = checkForFraud
            };
            IWorkflowTreeExecutor<OrderDataContext, decimal> tree;
            switch ((OrderType) order.IdObjectType)
            {
                case OrderType.Normal:
                case OrderType.AutoShip:
                case OrderType.DropShip:
                case OrderType.GiftList:
                case OrderType.AutoShipOrder:
                    tree = await _treeFactory.CreateTreeAsync<OrderDataContext, decimal>("Order");
                    break;
                case OrderType.Reship:
                    tree = await _treeFactory.CreateTreeAsync<OrderDataContext, decimal>("Reship");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            await tree.ExecuteAsync(context);
            UpdateOrderFromCalculationContext(order, context);
            return context;
        }

        public async Task<OrderDynamic> SelectLastOrderAsync(int customerId)
        {
            //TODO - should be redone on standart reading with dynamics after fixing missing data(Skus) with sort of operations
            OrderDynamic toReturn = null;
            var orderQuery = new OrderQuery().NotDeleted().WithActualStatusOnly().WithCustomerId(customerId);
            var order = (await _orderRepository.Query(orderQuery).OrderBy(p => p.OrderByDescending(x => x.Id)).SelectFirstOrDefaultAsync(false));
            if (order != null)
            {
                toReturn = await SelectAsync(order.Id);
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
            order.PromoSkus = dataContext.PromoSkus;
            if (dataContext.IsFraud)
            {
                order.Data.Review = ReviewType.ForReview;
                order.Data.ReviewReason = string.Join("; ", dataContext.FraudReason);
            }
            SetOrderSplitStatuses(dataContext, order);
        }

        public async Task OrderTypeSetup(OrderDynamic order)
        {
            if (order == null)
                return;
            var orderType = ((bool?) order.SafeData.MailOrder ?? false) ? (int?) SourceOrderType.MailOrder : null;
            var idOrder = order.Id;
            if (idOrder != 0)
            {
                var optionType = DynamicMapper.OptionsForType(order.IdObjectType).FirstOrDefault(o => o.Name == "OrderType")?.Id;
                if (optionType != null)
                {
                    var dbItem = await OptionValuesRepository.Query().Where(v => v.IdOrder == idOrder && v.IdOptionType == optionType.Value).SelectFirstOrDefaultAsync(false);
                    if (dbItem != null)
                    {
                        if (dbItem.Value == ((int) SourceOrderType.MailOrder).ToString())
                        {
                            if (!orderType.HasValue)
                            {
                                orderType = (int) SourceOrderType.Phone;
                            }
                            order.Data.OrderType = orderType.Value;
                        }
                        else
                        {
                            int value;
                            if (int.TryParse(dbItem.Value, out value))
                            {
                                order.Data.OrderType = orderType ?? value;
                            }
                            else
                            {
                                order.Data.OrderType = orderType;
                            }
                        }
                    }
                    else
                    {
                        order.Data.OrderType = orderType ?? (int) SourceOrderType.Phone;
                    }
                }
            }
            else
            {
                if (!orderType.HasValue)
                {
                    orderType = (int) SourceOrderType.Phone;
                }
                order.Data.OrderType = orderType.Value;
                order.ShippingAddress.Id = 0;
                if (order.PaymentMethod != null)
                {
                    order.PaymentMethod.Id = 0;
                    if (order.PaymentMethod.Address != null)
                    {
                        order.PaymentMethod.Address.Id = 0;
                    }
                }
            }
        }

        private void SetOrderSplitStatuses(OrderDataContext model, OrderDynamic dynamic)
        {
            var combinedStatus = model.CombinedStatus;
            if (combinedStatus == OrderStatus.Incomplete)
            {
                if (model.SplitInfo?.ShouldSplit ?? false)
                {
                    dynamic.OrderStatus = null;
                    dynamic.POrderStatus = combinedStatus;
                    dynamic.NPOrderStatus = combinedStatus;
                }
                else
                {
                    dynamic.OrderStatus = combinedStatus;
                    dynamic.POrderStatus = null;
                    dynamic.NPOrderStatus = null;
                }
                return;
            }
            if (model.SplitInfo?.ShouldSplit ?? false)
            {
                dynamic.OrderStatus = null;
                if (combinedStatus == OrderStatus.OnHold)
                {
                    dynamic.POrderStatus = dynamic.NPOrderStatus = OrderStatus.OnHold;
                }
                else if ((int?) dynamic.SafeData.ShipDelayType == (int) ShipDelayType.EntireOrder)
                {
                    dynamic.POrderStatus = combinedStatus;
                    dynamic.NPOrderStatus = combinedStatus;
                    if (dynamic.SafeData.ShipDelayDate != null)
                    {
                        dynamic.POrderStatus = OrderStatus.ShipDelayed;
                        dynamic.NPOrderStatus = OrderStatus.ShipDelayed;
                    }
                }
                else if ((int?) dynamic.SafeData.ShipDelayType == (int) ShipDelayType.PerishableAndNonPerishable)
                {
                    dynamic.POrderStatus = combinedStatus;
                    dynamic.NPOrderStatus = combinedStatus;
                    if (dynamic.SafeData.ShipDelayDateP != null)
                    {
                        dynamic.POrderStatus = OrderStatus.ShipDelayed;
                    }
                    if (dynamic.SafeData.ShipDelayDateNP != null)
                    {
                        dynamic.NPOrderStatus = OrderStatus.ShipDelayed;
                    }
                }
                else
                {
                    dynamic.POrderStatus = combinedStatus;
                    dynamic.NPOrderStatus = combinedStatus;
                }
            }
            else
            {
                dynamic.OrderStatus = combinedStatus;
                dynamic.POrderStatus = null;
                dynamic.NPOrderStatus = null;
                if (dynamic.OrderStatus == OrderStatus.Incomplete || dynamic.OrderStatus == OrderStatus.Processed ||
                    dynamic.OrderStatus == OrderStatus.ShipDelayed)
                {
                    if ((int?) dynamic.SafeData.ShipDelayType == (int) ShipDelayType.EntireOrder && dynamic.SafeData.ShipDelayDate != null)
                    {
                        dynamic.OrderStatus = OrderStatus.ShipDelayed;
                    }
                }
            }
        }

        protected override Task<List<MessageInfo>> ValidateDeleteAsync(int id)
        {
            if (_lockService.GetIsOrderLocked(id))
            {
                throw new AppValidationException(
                    "This order is currently being exported. You won't be able to save your changes. Wait a few minutes then refresh.");
            }
            return base.ValidateDeleteAsync(id);
        }

        protected override Task<List<MessageInfo>> ValidateAsync(OrderDynamic dynamic)
        {
            if (_lockService.GetIsOrderLocked(dynamic.Id))
            {
                throw new AppValidationException(
                    "This order is currently being exported. You won't be able to save your changes. Wait a few minutes then refresh.");
            }

            if (dynamic.Id == 0 && dynamic.Customer.StatusCode == (int) CustomerStatus.Suspended)
            {
                throw new CustomerSuspendException();
            }
            if (dynamic.IsAnyNotIncomplete())
            {
                if (dynamic.Skus?.Count == 0)
                {
                    throw new AppValidationException("You cannot place order without products");
                }
            }
            return base.ValidateAsync(dynamic);
        }

        public async Task<int?> GetOrderIdCustomer(int id)
        {
            var order = (await this._orderRepository.Query(p => p.StatusCode != (int) RecordStatusCode.Deleted && p.Id == id).SelectFirstOrDefaultAsync(false));
            return order?.IdCustomer;
        }

        public async Task<PagedList<ShortOrderItemModel>> GetShortOrdersAsync(OrderFilter filter)
        {
            filter.SelectOnlyTop = true;
            return await _sPEcommerceRepository.GetShortOrdersAsync(filter);
        }

        public async Task<PagedList<OrderDynamic>> GetFullOrdersAsync(OrderFilter filter)
        {
            var orderQuery = new OrderQuery().WithCustomerId(filter.IdCustomer).FilterById(filter.Id).NotDeleted().WithOrderType(filter.OrderType).WithoutIncomplete().NotAutoShip();

            var orders = await SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, orderQuery, null, PerformOrderSorting(filter), true);

            return orders;
        }

        public async Task<PagedList<OrderDynamic>> GetFullAutoShipsAsync(OrderFilterBase filter)
        {
            var orderQuery = new OrderQuery().WithCustomerId(filter.IdCustomer).FilterById(filter.Id).NotDeleted().WithOrderType(OrderType.AutoShip).WithoutIncomplete();

            var orders = await SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, orderQuery, null, PerformOrderSorting(filter), true);

            return orders;
        }

        public async Task ActivatePauseAutoShipAsync(int customerId, int autoShipId, bool activate)
        {
            var autoShip = await FindAutoShipToChangeStatusAsync(customerId, autoShipId);

            if (activate && autoShip.StatusCode == (int)RecordStatusCode.Active)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AutoShipAlreadyStarted]);
            }

            if (!activate && autoShip.StatusCode == (int)RecordStatusCode.NotActive)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AutoShipAlreadyPaused]);
            }

            autoShip.StatusCode = activate ? (int) RecordStatusCode.Active : (int) RecordStatusCode.NotActive;

            await _orderRepository.UpdateAsync(autoShip);
        }

        public async Task DeleteAutoShipAsync(int customerId, int autoShipId)
        {
            var autoShip = await FindAutoShipToChangeStatusAsync(customerId, autoShipId);

            autoShip.StatusCode = (int) RecordStatusCode.Deleted;

            await _orderRepository.UpdateAsync(autoShip);
        }

        public async Task SubmitAutoShipOrders()
        {
            var currentDate = DateTime.Now;

            var frequencyAvailable = _referenceData.AutoShipOptions.Select(x => x.Key).ToList();

            var toProcess = new List<int>();
            foreach (var frequency in frequencyAvailable)
            {
                var tempDate = currentDate.AddMonths(-frequency);

                var vAutoShips =
                    await
                        _vAutoShipRepository.Query(
                            x => 
                                x.AutoShipFrequency == frequency && x.LastAutoShipDate.HasValue &&
                                x.LastAutoShipDate.Value <= tempDate).SelectAsync(x => x.Id, false);

                if (vAutoShips.Count > 0)
                {
                    toProcess.AddRange(vAutoShips);
                }
            }

            //skipped by some reason
            var skippedAcidently = await _vAutoShipRepository.Query(x => !x.LastAutoShipDate.HasValue).SelectAsync(x => x.Id, false);

            if (skippedAcidently.Count > 0)
            {
                toProcess.AddRange(skippedAcidently);
            }

            foreach (var id in toProcess)
            {
                var autoShip = await SelectAsync(id);

                Logger.LogInfo(i => $"AutoShip {i} suitable for order submit", autoShip.Id);

                OrderDynamic standardOrder = null;

                var success = false;
                using (var uow = CreateUnitOfWork())
                {
                    using (var transaction = uow.BeginTransaction())
                    {
                        try
                        {
                            autoShip.Data.LastAutoShipDate = currentDate;
                            await UpdateInternalAsync(autoShip, uow);

                            standardOrder = autoShip;
                            standardOrder.PaymentMethod.IdOrderSource = autoShip.Id;
                            standardOrder.IdObjectType = (int) OrderType.AutoShipOrder;
                            standardOrder.Data.AutoShipFrequency = null;
                            standardOrder.Data.LastAutoShipDate = null;
                            standardOrder.Id = 0;
                            standardOrder.PaymentMethod.Id = 0;
                            if (standardOrder.PaymentMethod.Address != null)
                            {
                                standardOrder.PaymentMethod.Address.Id = 0;
                            }
                            standardOrder.ShippingAddress.Id = 0;
                            standardOrder.Data.AutoShipId = autoShip.Id;
                            standardOrder.OrderStatus = OrderStatus.Processed;
                            standardOrder.POrderStatus = null;
                            standardOrder.NPOrderStatus = null;

                            var order = await InsertAsyncInternal(standardOrder, uow);

                            transaction.Commit();

                            Logger.LogInfo(i => $"AutoShip {i} handled successfully", autoShip.Id);

                            success = true;

                            var entity = await SelectEntityFirstAsync(o => o.Id == order.Id);
                            await LogItemChanges(new[] {await DynamicMapper.FromEntityAsync(entity)});
                        }
                        catch (Exception e)
                        {
                            transaction.Rollback();
                            Logger.LogError(
                                $"AutoShip {autoShip.Id} skipped due to error ocurred. Customer Id: {autoShip.Customer?.Id}. Error: {e}");

                            success = false;
                        }
                    }
                }

                if (success)
                {
                    try
                    {
                        var emailModel = await Mapper.ToModelAsync<VitalChoice.Ecommerce.Domain.Mail.OrderConfirmationEmail>(standardOrder);
                        if (emailModel != null)
                        {
                            await _notificationService.SendOrderConfirmationEmailAsync(standardOrder.Customer.Email, emailModel);
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError($"Order confirmation {standardOrder.Id} was not sent. Error: {ex.Message}", ex);
                    }
                }
            }
        }

        public async Task<IList<int>> SelectAutoShipOrdersAsync(int idAutoShip)
        {
            return await _vAutoShipOrderRepository.Query(x => x.AutoShipId == idAutoShip).SelectAsync(x => x.Id, false);
        }

        public async Task<bool> CancelOrderAsync(int id, POrderType? pOrderType = null)
        {
            var toReturn = false;
            var order = await SelectAsync(id, false);
            if (order != null)
            {
                using (var uow = CreateUnitOfWork())
                {
                    using (var transaction = uow.BeginTransaction())
                    {
                        try
                        {
                            var giftCertificateRepository = uow.RepositoryAsync<GiftCertificate>();
                            List<GiftCertificate> generatedGcs = new List<GiftCertificate>();
                            if (order.Skus.Any(s => (s.GcsGenerated?.Count ?? 0) > 0))
                            {
                                generatedGcs =
                                    await
                                        giftCertificateRepository.Query(
                                            p => p.IdOrder == order.Id && p.StatusCode != RecordStatusCode.NotActive).SelectAsync(true);
                                //cancel gc=3 with np part
                                if (pOrderType == POrderType.NP)
                                {
                                    generatedGcs.Where(p => p.GCType == GCType.GC).ForEach(p => p.StatusCode = RecordStatusCode.NotActive);
                                }
                                //cancel all gcs with all
                                if (IsAllCancel(pOrderType, order))
                                {
                                    generatedGcs.ForEach(p => p.StatusCode = RecordStatusCode.NotActive);
                                }
                            }

                            List<GiftCertificate> usedGcs = new List<GiftCertificate>();
                            if (order.GiftCertificates?.Count > 0)
                            {
                                var ids = order.GiftCertificates.Select(p => p.GiftCertificate?.Id).Distinct().ToList();
                                usedGcs = await giftCertificateRepository.Query(p => ids.Contains(p.Id)).SelectAsync(true);

                                if (IsAllCancel(pOrderType, order))
                                {
                                    order.GiftCertificates.Clear();
                                }
                                else
                                {
                                    if (pOrderType == POrderType.P)
                                    {
                                        usedGcs.UpdateKeyed(order.GiftCertificates, g => g.Id, g => g.GiftCertificate.Id, (db, gc) => db.Balance += gc.PAmount ?? 0);
                                        order.GiftCertificates.ForEach(p =>
                                        {
                                            p.Amount = p.Amount - (p.PAmount ?? 0);
                                            p.Amount = p.Amount >= 0 ? p.Amount : 0;
                                            p.PAmount = 0;
                                            var usedGc = usedGcs.FirstOrDefault(pp => pp.Id == p.GiftCertificate?.Id);
                                            if (usedGc != null)
                                            {
                                                p.GiftCertificate.Balance = usedGc.Balance;
                                            }
                                        });
                                    }
                                    if (pOrderType == POrderType.NP)
                                    {
                                        usedGcs.UpdateKeyed(order.GiftCertificates, g => g.Id, g => g.GiftCertificate.Id, (db, gc) => db.Balance += gc.NPAmount ?? 0);
                                        order.GiftCertificates.ForEach(p =>
                                        {
                                            p.Amount = p.Amount - (p.NPAmount ?? 0);
                                            p.Amount = p.Amount >= 0 ? p.Amount : 0;
                                            p.NPAmount = 0;
                                            var usedGc = usedGcs.FirstOrDefault(pp => pp.Id == p.GiftCertificate?.Id);
                                            if (usedGc != null)
                                            {
                                                p.GiftCertificate.Balance = usedGc.Balance;
                                            }
                                        });
                                    }
                                }
                            }

                            //cancel one time discount with all
                            if (order.Discount?.Id > 0 && IsAllCancel(pOrderType, order))
                            {
                                await _discountService.SetDiscountUsed(order.Discount, order.Customer.Id, false);
                            }

                            await uow.SaveChangesAsync();
                            giftCertificateRepository.DetachAll(generatedGcs);
                            giftCertificateRepository.DetachAll(usedGcs);

                            if (order.OrderStatus.HasValue && !pOrderType.HasValue)
                            {
                                order.OrderStatus = OrderStatus.Cancelled;
                            }
                            if (order.POrderStatus.HasValue && (pOrderType == POrderType.P || !pOrderType.HasValue))
                            {
                                order.POrderStatus = OrderStatus.Cancelled;
                            }
                            if (order.NPOrderStatus.HasValue && (pOrderType == POrderType.NP || !pOrderType.HasValue))
                            {
                                order.NPOrderStatus = OrderStatus.Cancelled;
                            }

                            await base.UpdateAsync(order, uow);

                            transaction.Commit();

                            var dbEntity = await SelectEntityFirstAsync(o => o.Id == id);
                            await LogItemChanges(new[] {await DynamicMapper.FromEntityAsync(dbEntity)});

                            toReturn = true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            return toReturn;
        }

        private bool IsAllCancel(POrderType? pOrderType, OrderDynamic order)
        {
            return !pOrderType.HasValue || (pOrderType == POrderType.P && order.NPOrderStatus == OrderStatus.Cancelled) || (pOrderType == POrderType.NP && order.POrderStatus == OrderStatus.Cancelled);
        }

        protected override async Task<Order> InsertAsync(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            Order res;

            if (model.IdObjectType == (int) OrderType.AutoShip)
            {
                int? idAutoShipOrder = null;
                using (var transaction = uow.BeginTransaction())
                {
                    try
                    {
                        var anyNotIncomplete = model.IsAnyNotIncomplete();
                        var anyNotShipDelayed = model.IsAnyNotShipDelayed();
                        if (anyNotIncomplete && anyNotShipDelayed)
                        {
                            model.Data.LastAutoShipDate = DateTime.Now;
                        }

                        res = await InsertAsyncInternal(model, uow);

                        if (anyNotIncomplete && anyNotShipDelayed)
                        {
                            model.PaymentMethod.IdOrderSource = res.Id;
                            model.IdObjectType = (int) OrderType.AutoShipOrder;
                            model.Data.AutoShipFrequency = null;
                            model.Data.LastAutoShipDate = null;
                            model.Data.AutoShipId = res.Id;
                            model.Id = 0;
                            model.PaymentMethod.Id = 0;
                            if (model.PaymentMethod.Address != null)
                            {
                                model.PaymentMethod.Address.Id = 0;
                            }
                            model.ShippingAddress.Id = 0;

                            idAutoShipOrder = (await InsertAsyncInternal(model, uow)).Id;
                        }

                        transaction.Commit();

                        if (idAutoShipOrder.HasValue)
                        {
                            var entity = await SelectEntityFirstAsync(o => o.Id == idAutoShipOrder.Value);
                            await LogItemChanges(new[] {await DynamicMapper.FromEntityAsync(entity)});
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            else
            {
                res = await InsertAsyncInternal(model, uow);
            }

            return res;
        }

        private async Task ChargeGiftCertificates(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            if (model.IsAnyNotIncomplete() && model.GiftCertificates.Count > 0)
            {
                var gcsRep = uow.RepositoryAsync<GiftCertificate>();
                var gcs = model.GiftCertificates.Select(g => g.GiftCertificate.Id).Distinct().ToList();
                var gcsInDb = await gcsRep.Query(g => gcs.Contains(g.Id)).SelectAsync(true);
                gcsInDb.UpdateKeyed(model.GiftCertificates.Select(g => g.GiftCertificate), g => g.Id,
                    (gcDb, gc) => gcDb.Balance = gc.Balance);
            }
        }

        private void EnsurePaymentMethod(OrderDynamic model)
        {
            if (model.PaymentMethod == null)
            {
                switch ((OrderType) model.IdObjectType)
                {
                    case OrderType.Normal:
                    case OrderType.AutoShipOrder:
                        model.PaymentMethod = _paymentGenericService.Mapper.CreatePrototype((int) PaymentMethodType.CreditCard);
                        break;
                    case OrderType.AutoShip:
                        model.PaymentMethod = _paymentGenericService.Mapper.CreatePrototype((int) PaymentMethodType.CreditCard);
                        break;
                    case OrderType.DropShip:
                        model.PaymentMethod = _paymentGenericService.Mapper.CreatePrototype((int) PaymentMethodType.NoCharge);
                        break;
                    case OrderType.GiftList:
                        model.PaymentMethod = _paymentGenericService.Mapper.CreatePrototype((int) PaymentMethodType.NoCharge);
                        break;
                    case OrderType.Reship:
                        model.PaymentMethod = _paymentGenericService.Mapper.CreatePrototype((int) PaymentMethodType.CreditCard);
                        break;
                    case OrderType.Refund:
                        model.PaymentMethod = _paymentGenericService.Mapper.CreatePrototype((int) PaymentMethodType.CreditCard);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        protected override async Task<List<Order>> InsertRangeAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            var autoShips = models.Where(x => x.IdObjectType == (int) OrderType.AutoShip).ToList();
            var others = models.Where(x => x.IdObjectType != (int) OrderType.AutoShip).ToList();

            List<Order> res = new List<Order>();
            if (autoShips.Count > 0)
            {
                List<int> autoShipOrderIds = null;
                using (var transaction = uow.BeginTransaction())
                {
                    try
                    {
                        foreach (var autoShip in autoShips)
                        {
                            var anyNotIncomplete = autoShip.IsAnyNotIncomplete();
                            if (anyNotIncomplete)
                            {
                                autoShip.Data.LastAutoShipDate = DateTime.Now;
                            }
                        }

                        res.AddRange(await InsertRangeInternalAsync(autoShips, uow));

                        var completed = autoShips.Where(x => x.IsAnyNotIncomplete()).ToList();
                        if (completed.Count > 0)
                        {
                            foreach (var model in completed)
                            {
                                model.PaymentMethod.IdOrderSource = model.Id;
                                model.IdObjectType = (int)OrderType.AutoShipOrder;
                                model.Data.AutoShipFrequency = null;
                                model.Data.LastAutoShipDate = null;
                                model.Data.AutoShipId = model.Id;
                                model.Id = 0;
                                model.PaymentMethod.Id = 0;
                                if (model.PaymentMethod.Address != null)
                                {
                                    model.PaymentMethod.Address.Id = 0;
                                }
                                model.ShippingAddress.Id = 0;
                            }

                            autoShipOrderIds = (await InsertRangeInternalAsync(completed, uow)).Select(p => p.Id).ToList();
                        }

                        transaction.Commit();

                        if (autoShipOrderIds != null)
                        {
                            var entities = await SelectEntitiesAsync(o => autoShipOrderIds.Contains(o.Id));
                            await LogItemChanges(await DynamicMapper.FromEntityRangeAsync(entities));
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

            if (others.Count > 0)
            {
                res.AddRange(await InsertRangeInternalAsync(models, uow));
            }

            return res;
        }


        protected override async Task<Order> UpdateAsync(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            Order res;

            if (model.IdObjectType == (int) OrderType.AutoShip)
            {
                var previous = await SelectEntityFirstAsync(x => x.Id == model.Id);
                int? idAutoShipOrder = null;
                using (var transaction = uow.BeginTransaction())
                {
                    try
                    {
                        res = await UpdateInternalAsync(model, uow);

                        if (previous.IsAnyIncomplete() && model.IsAnyNotIncomplete())
                        {
                            model.Data.LastAutoShipDate = DateTime.Now;
                            await UpdateInternalAsync(model, uow);

                            model.PaymentMethod.IdOrderSource = res.Id;
                            model.IdObjectType = (int)OrderType.AutoShipOrder;
                            model.Data.AutoShipFrequency = null;
                            model.Data.LastAutoShipDate = null;
                            model.Data.AutoShipId = res.Id;
                            model.Id = 0;
                            model.PaymentMethod.Id = 0;
                            if (model.PaymentMethod.Address != null)
                            {
                                model.PaymentMethod.Address.Id = 0;
                            }
                            model.ShippingAddress.Id = 0;

                            idAutoShipOrder = (await InsertAsyncInternal(model, uow)).Id;
                        }

                        transaction.Commit();

                        if (idAutoShipOrder.HasValue)
                        {
                            var entity = await SelectEntityFirstAsync(o => o.Id == idAutoShipOrder.Value);
                            await LogItemChanges(new[] {await DynamicMapper.FromEntityAsync(entity)});
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            else
            {
                res = await UpdateInternalAsync(model, uow);
            }

            return res;
        }

        protected override async Task<List<Order>> UpdateRangeAsync(ICollection<OrderDynamic> models, IUnitOfWorkAsync uow)
        {
            var autoShips = models.Where(x => x.IdObjectType == (int) OrderType.AutoShip).ToList();
            var others = models.Where(x => x.IdObjectType != (int) OrderType.AutoShip).ToList();

            List<Order> res = new List<Order>();
            if (autoShips.Count > 0)
            {
                List<int> autoShipOrderIds = null;
                using (var transaction = uow.BeginTransaction())
                {
                    try
                    {
                        var list = autoShips.Select(y => y.Id).Distinct().ToList();
                        var previous = await SelectAsync(x => list.Contains(x.Id));

                        res.AddRange(await UpdateRangeInternalAsync(autoShips, uow));

                        var completed = autoShips.Where(x => x.OrderStatus != OrderStatus.Incomplete).ToList();
                        if (completed.Count > 0)
                        {
                            var toInsert = new List<OrderDynamic>();

                            foreach (var model in completed)
                            {
                                if (previous.Single(x => x.Id == model.Id).OrderStatus == OrderStatus.Incomplete && model.OrderStatus != OrderStatus.Incomplete)
                                {
                                    model.Data.LastAutoShipDate = DateTime.Now;
                                    await UpdateInternalAsync(model, uow);

                                    model.PaymentMethod.IdOrderSource = model.Id;
                                    model.IdObjectType = (int)OrderType.AutoShipOrder;
                                    model.Data.AutoShipFrequency = null;
                                    model.Data.LastAutoShipDate = null;
                                    model.Data.AutoShipId = model.Id;
                                    model.Id = 0;
                                    model.PaymentMethod.Id = 0;
                                    if (model.PaymentMethod.Address != null)
                                    {
                                        model.PaymentMethod.Address.Id = 0;
                                    }
                                    model.ShippingAddress.Id = 0;

                                    toInsert.Add(model);
                                }
                            }

                            if (toInsert.Count > 0)
                            {
                                autoShipOrderIds = (await InsertRangeInternalAsync(toInsert, uow)).Select(p => p.Id).ToList();
                            }
                        }

                        transaction.Commit();


                        if (autoShipOrderIds != null)
                        {
                            var entities = await SelectEntitiesAsync(o => autoShipOrderIds.Contains(o.Id));
                            await LogItemChanges(await DynamicMapper.FromEntityRangeAsync(entities));
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            if (others.Count > 0)
            {
                res.AddRange(await UpdateRangeInternalAsync(models, uow));
            }

            return res;
        }

        private async Task SetSkusBornDate(ICollection<OrderDynamic> orders, IUnitOfWorkAsync uow)
        {
            var option = _productService.GetSkuOptionTypes(new HashSet<string>() {ProductConstants.FIELD_NAME_SKU_INVENTORY_BORN_DATE}).FirstOrDefault();
            if (option != null)
            {
                var skuIds = new HashSet<int>(orders.Where(o => o.IdObjectType != (int) OrderType.AutoShip).SelectMany(p => p.Skus).Select(p => p.Sku.Id));
                skuIds.AddRange(orders.Where(o => o.IdObjectType != (int) OrderType.AutoShip).SelectMany(p => p.PromoSkus).Select(p => p.Sku.Id));
                if (skuIds.Count > 0)
                {
                    var dbOptionValues = await _productService.GetSkuOptionValues(skuIds, new[] {option.Id});
                    var skuIdsForInsert = skuIds.Except(dbOptionValues.Select(p => p.IdSku)).ToList();
                    if (skuIdsForInsert.Count > 0)
                    {
                        var now = MapperTypeConverter.ConvertDateToIsoStringAndDropMc(DateTime.Now);
                        var skuOptionValueRepository = uow.RepositoryAsync<SkuOptionValue>();
                        skuOptionValueRepository.InsertRange(skuIdsForInsert.Select(p => new SkuOptionValue()
                        {
                            IdOptionType = option.Id, IdSku = p, Value = now
                        }));
                    }
                }
            }
        }

        private async Task UpdateAffiliateOrderPayment(OrderDynamic dynamic, IUnitOfWorkAsync uow)
        {
            if (dynamic.Customer.IdObjectType == (int) CustomerType.Retail && dynamic.Customer.IdAffiliate.HasValue && dynamic.AffiliatePaymentAmount.HasValue && dynamic.AffiliateNewCustomerOrder.HasValue)
            {
                AffiliateOrderPayment payment = new AffiliateOrderPayment
                {
                    Id = dynamic.Id, Status = AffiliateOrderPaymentStatus.NotPaid, IdAffiliate = dynamic.Customer.IdAffiliate.Value, Amount = dynamic.AffiliatePaymentAmount.Value, NewCustomerOrder = dynamic.AffiliateNewCustomerOrder.Value
                };

                var affiliateOrderPaymentRepository = uow.RepositoryAsync<AffiliateOrderPayment>();
                var dbItem = await affiliateOrderPaymentRepository.Query(p => p.Id == payment.Id).SelectFirstOrDefaultAsync(true);
                if (dbItem == null)
                {
                    dbItem = new AffiliateOrderPayment
                    {
                        Status = AffiliateOrderPaymentStatus.NotPaid, IdAffiliate = payment.IdAffiliate, Id = payment.Id, Amount = payment.Amount, NewCustomerOrder = payment.NewCustomerOrder
                    };

                    await affiliateOrderPaymentRepository.InsertAsync(dbItem);
                }
                else
                {
                    if (dbItem.Status == AffiliateOrderPaymentStatus.NotPaid)
                    {
                        dbItem.Amount = payment.Amount;
                    }
                }
            }
        }

        public async Task<PagedList<OrderInfoItem>> GetOrdersAsync(VOrderFilter filter)
        {
            var conditions = new OrderQuery();
            conditions = conditions.WithCustomerId(filter.IdCustomer).NotDeleted();

            if (!filter.ShipDate)
            {
                conditions = conditions.WithCreatedDate(filter.From, filter.To);
            }
            else
            {
                conditions = conditions.WithShippedDate(filter.From, filter.To);
            }
            conditions = conditions.WithId(filter.Id)
                //TODO - should be redone after adding - https://github.com/aspnet/EntityFramework/issues/2850
                .WithOrderType(filter.IdObjectType)
                .WithOrderStatus(filter.OrderStatus)
                .WithOrderStatuses(filter.IncludeOrderStatuses)
                .WithCustomerType(filter.IdCustomerType)
                .WithoutIncomplete(filter.OrderStatus, filter.IgnoreNotShowingIncomplete)
                .WithIdSku(filter.IdSku)
                .WithIdAddedBy(filter.IdAddedBy)
                .WithShipState(filter.IdShipState)
                .WithOrderDynamicValues(filter.IdOrderSource, filter.POrderType, filter.IdShippingMethod,
                filter.ForReview ? ReviewType.ForReview : (ReviewType?)null)
                .WithCustomerDynamicValues(filter.CustomerFirstName, filter.CustomerLastName, filter.CustomerCompany)
                .NotAutoShip();

            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VOrderSortPath.IdPaymentMethod:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.PaymentMethod.IdObjectType)
                                : x.OrderByDescending(y => y.PaymentMethod.IdObjectType);
                    break;
                case VOrderSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated).ThenBy(y => y.Id)
                                : x.OrderByDescending(y => y.DateCreated).ThenByDescending(y => y.Id);
                    break;
                case VOrderSortPath.IdCustomerType:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Customer.IdObjectType)
                                : x.OrderByDescending(y => y.Customer.IdObjectType);
                    break;
                case VOrderSortPath.IdObjectType:
                    sortable =
                        (x) => sortOrder == FilterSortOrder.Asc ? x.OrderBy(y => y.IdObjectType) : x.OrderByDescending(y => y.IdObjectType);
                    break;
                case VOrderSortPath.Id:
                    sortable = (x) => sortOrder == FilterSortOrder.Asc ? x.OrderBy(y => y.Id) : x.OrderByDescending(y => y.Id);
                    break;
                case VOrderSortPath.Total:
                    sortable = (x) => sortOrder == FilterSortOrder.Asc ? x.OrderBy(y => y.Total) : x.OrderByDescending(y => y.Total);
                    break;
                case VOrderSortPath.DateEdited:
                    sortable =
                        (x) => sortOrder == FilterSortOrder.Asc ? x.OrderBy(y => y.DateEdited) : x.OrderByDescending(y => y.DateEdited);
                    break;
                case VOrderSortPath.IdOrderSource:
                    sortable =
                        x =>
                            sortOrder == FilterSortOrder.Asc
                                ? OrderingExtension.OrderByValue(x, "OrderType")
                                : OrderingExtension.OrderByDescendingValue(x, "OrderType");
                    break;
                case VOrderSortPath.IdAddedBy:
                    sortable =
                        (x) => sortOrder == FilterSortOrder.Asc ? x.OrderBy(y => y.IdAddedBy) : x.OrderByDescending(y => y.IdAddedBy);
                    break;
            }

            var orders = await SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, conditions, 
                includes => includes.Include(c => c.OptionValues).Include(c => c.PaymentMethod).
                Include(c => c.ShippingAddress).ThenInclude(c => c.OptionValues).Include(c => c.Customer).
                ThenInclude(p => p.ProfileAddress).ThenInclude(c => c.OptionValues).
                Include(c => c.OrderShippingPackages).Include(c=>c.HealthwiseOrder), orderBy: sortable, withDefaults: true);

            var resultList = new List<OrderInfoItem>(orders.Items.Count);
            foreach (var item in orders.Items)
            {
                var newItem = new OrderInfoItem
                {
                    Id = item.Id,
                    IdObjectType = (OrderType) item.IdObjectType,
                    OrderStatus = item.OrderStatus,
                    POrderStatus = item.POrderStatus,
                    NPOrderStatus = item.NPOrderStatus,
                    IdPaymentMethod = item.PaymentMethod?.IdObjectType,
                    DateCreated = item.DateCreated,
                    Total = item.Total,
                    IdEditedBy = item.IdEditedBy,
                    IdAddedBy = item.IdAddedBy,
                    DateEdited = item.DateEdited,
                    IdCustomerType = item.Customer.IdObjectType,
                    IdCustomer = item.Customer.Id,
                    Company = item.Customer?.ProfileAddress.SafeData.Company,
                    Customer = item.Customer?.ProfileAddress.SafeData.FirstName + " " + item.Customer?.ProfileAddress.SafeData.LastName,
                    StateCode = _codeResolver.GetStateCode(item.ShippingAddress?.IdCountry ?? 0, item.ShippingAddress?.IdState ?? 0),
                    ShipTo = item.ShippingAddress?.SafeData.FirstName + " " + item.ShippingAddress?.SafeData.LastName,
                    PreferredShipMethod = item.ShippingAddress?.SafeData.PreferredShipMethod,
                    Healthwise = (bool?) item.SafeData.IsHealthwise ?? false,
                    Review = (int?)item.SafeData.Review ?? null,
                };
                await DynamicMapper.UpdateModelAsync(newItem, item);

                newItem.DateShipped =
                    item.OrderShippingPackages.FirstOrDefault(p => !p.POrderType.HasValue)?.ShippedDate ??
                    newItem.DateShipped;
                newItem.PDateShipped =
                    item.OrderShippingPackages.FirstOrDefault(p => p.POrderType == (int)POrderType.P)?.ShippedDate ??
                    newItem.PDateShipped;
                newItem.NPDateShipped =
                    item.OrderShippingPackages.FirstOrDefault(p => p.POrderType == (int)POrderType.NP)?.ShippedDate ??
                    newItem.NPDateShipped;

                resultList.Add(newItem);
            }

            PagedList<OrderInfoItem> toReturn = new PagedList<OrderInfoItem>
            {
                Items = resultList, Count = orders.Count
            };

            if (toReturn.Items.Count > 0)
            {
                var ids = new HashSet<int>(toReturn.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy.Value));
                ids.AddRange(toReturn.Items.Where(p => p.IdAddedBy.HasValue).Select(p => p.IdAddedBy.Value));
                var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false);
                foreach (var item in toReturn.Items)
                {
                    foreach (var profile in profiles)
                    {
                        if (item.IdEditedBy == profile.Id)
                        {
                            item.EditedByAgentId = profile.AgentId;
                        }
                        if (item.IdAddedBy == profile.Id)
                        {
                            item.AddedByAgentId = profile.AgentId;
                        }
                    }
                }
            }

            return toReturn;
        }

        public OrderDynamic CreateNewNormalOrder(OrderStatus status)
        {
            var toReturn = Mapper.CreatePrototype((int) OrderType.Normal);

            toReturn.StatusCode = (int) RecordStatusCode.Active;
            if (status != OrderStatus.Processed && status != OrderStatus.Incomplete)
            {
                throw new Exception("New normal order invalid status");
            }
            toReturn.OrderStatus = status;
            toReturn.DateCreated = DateTime.Now;
            toReturn.Data.ShipDelayType = (int) ShipDelayType.None;

            return toReturn;
        }

        public string GenerateOrderCode(POrderType? pOrderType, int idOrder, out TaxGetType type)
        {
            var orderCode = !pOrderType.HasValue ? idOrder.ToString() : (pOrderType.Value == POrderType.P ? idOrder + "-P" : idOrder + "-NP");
            type = !pOrderType.HasValue ? TaxGetType.UseBoth : (pOrderType.Value == POrderType.P ? TaxGetType.Perishable : TaxGetType.NonPerishable);
            return orderCode;
        }

        private void SetExtendedOptions(IEnumerable<OrderDynamic> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (!(bool?) item.SafeData.GiftOrder ?? false)
                    {
                        item.Data.GiftMessage = null;
                    }

                    if ((bool?) item.Customer?.SafeData.Guest ?? false)
                    {
                        item.Data.Guest = true;
                    }

                    POrderType? toReturn = null;
                    var pOrder = false;
                    var npOrder = false;
                    if (item.Skus != null)
                    {
                        foreach (var skuOrdered in item.Skus)
                        {
                            pOrder = pOrder || skuOrdered.Sku.Product.IdObjectType == (int) ProductType.Perishable;
                            npOrder = npOrder || skuOrdered.Sku.Product.IdObjectType == (int) ProductType.NonPerishable;
                        }
                    }
                    if (item.PromoSkus != null)
                    {
                        foreach (var skuOrdered in item.PromoSkus.Where(p => p.Enabled))
                        {
                            pOrder = pOrder || skuOrdered.Sku.Product.IdObjectType == (int) ProductType.Perishable;
                            npOrder = npOrder || skuOrdered.Sku.Product.IdObjectType == (int) ProductType.NonPerishable;
                        }
                    }
                    if (pOrder && npOrder)
                    {
                        toReturn = POrderType.PNP;
                    }
                    else if (pOrder)
                    {
                        toReturn = POrderType.P;
                    }
                    else if (npOrder)
                    {
                        toReturn = POrderType.NP;
                    }
                    item.Data.POrderType = (int?) toReturn;
                }
            }
        }


        public override async Task<OrderDynamic> InsertAsync(OrderDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                var toReturn = await InsertAsync(uow, model);

                ICollection<GiftCertificate> generatedGCs = toReturn.Skus?.SelectMany(p => p?.GcsGenerated).ToList();
                if (generatedGCs?.Count > 0)
                {
                    await ObjectLogItemExternalService.LogItems(generatedGCs);
                }

                return toReturn;
            }
        }

        public override async Task<OrderDynamic> UpdateAsync(OrderDynamic model)
        {
            using (var uow = CreateUnitOfWork())
            {
                var toReturn = await UpdateAsync(uow, model);

                ICollection<GiftCertificate> generatedGCs = toReturn.Skus?.SelectMany(p => p?.GcsGenerated).ToList();
                if (generatedGCs?.Count > 0)
                {
                    await ObjectLogItemExternalService.LogItems(generatedGCs);
                }

                return toReturn;
            }
        }

        public override async Task<List<OrderDynamic>> InsertRangeAsync(ICollection<OrderDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                var toReturn = await InsertRangeAsync(uow, models);

                ICollection<GiftCertificate> generatedGCs = toReturn?.SelectMany(p=>p.Skus?.SelectMany(pp => pp?.GcsGenerated)).ToList();
                if (generatedGCs?.Count > 0)
                {
                    await ObjectLogItemExternalService.LogItems(generatedGCs);
                }

                return toReturn;
            }
        }

        public override async Task<List<OrderDynamic>> UpdateRangeAsync(ICollection<OrderDynamic> models)
        {
            using (var uow = CreateUnitOfWork())
            {
                var toReturn = await UpdateRangeAsync(uow, models);

                ICollection<GiftCertificate> generatedGCs = toReturn?.SelectMany(p => p.Skus?.SelectMany(pp => pp?.GcsGenerated)).ToList();
                if (generatedGCs?.Count > 0)
                {
                    await ObjectLogItemExternalService.LogItems(generatedGCs);
                }

                return toReturn;
            }
        }

        #region OrdersImport

        public async Task<bool> ImportOrders(byte[] file, string fileName, OrderImportType orderType, int idCustomer, int? idPaymentMethod,
            int idAddedBy)
        {
            var customer = await _customerService.SelectAsync(idCustomer);
            if (customer == null)
            {
                throw new AppValidationException("Invalid file format");
            }

            BaseOrderImportProcessor processor = null;
            CustomerPaymentMethodDynamic paymentMethod = null;

            switch (orderType)
            {
                case OrderImportType.GiftList:
                    if (!customer.ApprovedPaymentMethods.Contains((int) PaymentMethodType.NoCharge))
                    {
                        throw new AppValidationException("Payment method \"No Charge\" should be allowed");
                    }
                    paymentMethod = customer.CustomerPaymentMethods.FirstOrDefault(p => p.Id == idPaymentMethod);
                    if (paymentMethod == null || paymentMethod.IdObjectType != (int) PaymentMethodType.CreditCard ||
                        paymentMethod.Address == null)
                    {
                        throw new AppValidationException("Payment method \"Credit Card\" should be configured");
                    }
                    processor = new GiftListOrderImportProcessor(_countryService, Mapper, _addressMapper, _referenceData, _loggerFactory);

                    break;
                case OrderImportType.DropShip:
                    if (!customer.ApprovedPaymentMethods.Contains((int) PaymentMethodType.Oac))
                    {
                        throw new AppValidationException("Payment method \"On Approved Credit\" should be allowed");
                    }
                    paymentMethod = customer.CustomerPaymentMethods.FirstOrDefault(p => p.Id == idPaymentMethod);
                    if (paymentMethod == null || paymentMethod.IdObjectType != (int) PaymentMethodType.Oac ||
                        paymentMethod.Address == null)
                    {
                        throw new AppValidationException("Payment method \"On Approved Credit\" should be configured");
                    }
                    processor = new DropShipOrderImportProcessor(_countryService, Mapper, _addressMapper, _referenceData, _loggerFactory);

                    break;
                case OrderImportType.DropShipAAFES:
                    if (!customer.ApprovedPaymentMethods.Contains((int) PaymentMethodType.Oac))
                    {
                        throw new AppValidationException("Payment method \"On Approved Credit\" should be allowed");
                    }
                    paymentMethod = customer.CustomerPaymentMethods.FirstOrDefault(p => p.Id == idPaymentMethod);
                    if (paymentMethod == null || paymentMethod.IdObjectType != (int) PaymentMethodType.Oac ||
                        paymentMethod.Address == null)
                    {
                        throw new AppValidationException("Payment method \"On Approved Credit\" should be configured");
                    }
                    processor = new DropShipAAFESSOrderImportProcessor(_countryService, Mapper, _addressMapper, _referenceData,
                        _loggerFactory);

                    break;
                default:
                    throw new ApiException("Orders import with the given orderType isn't implemented");
            }

            var map = await processor.ParseAndValidateAsync(file, orderType, customer, paymentMethod, idAddedBy);

            await LoadSkusDynamic(map, customer);
            //not found SKU errors
            var messages = BusinessHelper.FormatRowsRecordErrorMessages(map.SelectMany(p => p.OrderImportItems));
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var calculationMessages = new Dictionary<IList<int>, IList<MessageInfo>>();
            foreach (var item in map)
            {
                //merge the same skus
                var forRemove = new List<SkuOrdered>();
                foreach (var skuOrdered in item.Order.Skus)
                {
                    if (!forRemove.Contains(skuOrdered))
                    {
                        var dublicates = item.Order.Skus.Where(p => p.Sku.Id == skuOrdered.Sku.Id && p!=skuOrdered).ToList();
                        foreach (var dublicate in dublicates)
                        {
                            skuOrdered.Quantity += dublicate.Quantity;
                            forRemove.Add(dublicate);
                        }
                    }
                }
                item.Order.Skus.RemoveAll(forRemove);

                var orderCombinedStatus = item.Order.OrderStatus ?? OrderStatus.Processed;
                item.Order.Data.ShipDelayType = item.Order.SafeData.ShipDelayDate != null ? ShipDelayType.EntireOrder : ShipDelayType.None;

                var context = await CalculateOrder(item.Order, orderCombinedStatus, false);
                if (orderType == OrderImportType.DropShipAAFES)
                {
                    if (context.ShippingUpgradePOptions == null ||
                        context.ShippingUpgradePOptions.FirstOrDefault(p => p.Key == ShippingUpgradeOption.Overnight) ==
                        null)
                    {
                        item.Order.Data.ShippingUpgradeP = null;
                    }
                    if (context.ShippingUpgradeNpOptions == null ||
                        context.ShippingUpgradeNpOptions.FirstOrDefault(p => p.Key == ShippingUpgradeOption.Overnight) ==
                        null)
                    {
                        item.Order.Data.ShippingUpgradeNP = null;
                    }
                }

                var rows = item.OrderImportItems.Select(p => p.RowNumber).ToList();
                var tempMessages = new List<MessageInfo>(context.Messages.Where(p => p.MessageLevel == MessageLevel.Error));
                tempMessages.AddRange(context.SkuOrdereds.Where(p => p.Messages != null).SelectMany(p => p.Messages)
                    .Where(p => p.MessageLevel == MessageLevel.Error));
                tempMessages.AddRange(context.PromoSkus.Where(p => p.Enabled && p.Messages != null).SelectMany(p => p.Messages)
                    .Where(p => p.MessageLevel == MessageLevel.Error));
                calculationMessages.Add(rows, tempMessages);
            }

            //throw calculating errors
            messages = FormatCalculationRowsRecordErrorMessages(calculationMessages);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            //additional recalculation for setting shiping to 0
            if (orderType == OrderImportType.DropShipAAFES)
            {
                foreach (var item in map)
                {
                    item.Order.Data.ShippingOverride = item.Order.ShippingTotal;

                    var orderCombinedStatus = item.Order.OrderStatus ?? OrderStatus.Processed;
                    item.Order.Data.ShipDelayType = item.Order.SafeData.ShipDelayDate != null
                        ? ShipDelayType.EntireOrder
                        : ShipDelayType.None;

                    var context = await CalculateOrder(item.Order, orderCombinedStatus, false);

                    var rows = item.OrderImportItems.Select(p => p.RowNumber).ToList();
                    var tempMessages = new List<MessageInfo>(context.Messages.Where(p => p.MessageLevel == MessageLevel.Error));
                    tempMessages.AddRange(context.SkuOrdereds.Where(p => p.Messages != null).SelectMany(p => p.Messages)
                        .Where(p => p.MessageLevel == MessageLevel.Error));
                    tempMessages.AddRange(context.PromoSkus.Where(p => p.Enabled && p.Messages != null).SelectMany(p => p.Messages)
                        .Where(p => p.MessageLevel == MessageLevel.Error));
                    calculationMessages.Add(rows, tempMessages);
                }

                //throw calculating errors
                messages = FormatCalculationRowsRecordErrorMessages(calculationMessages);
                if (messages.Count > 0)
                {
                    throw new AppValidationException(messages);
                }
            }

            var orders = map.Select(p => p.Order).ToList();

            using (var transaction = TransactionAccessor.BeginTransaction())
            {
                try
                {
                    orders = await InsertRangeAsync(orders);

                    if (orderType == OrderImportType.GiftList && idPaymentMethod.HasValue)
                    {
                        await ExportGlCardDetails(orders, customer, idPaymentMethod.Value, idAddedBy);
                        await SendGlOrdersImportEmailAsync(orders, customer, idPaymentMethod.Value, idAddedBy);
                    }

                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return true;
        }

        private ICollection<MessageInfo> FormatCalculationRowsRecordErrorMessages(Dictionary<IList<int>, IList<MessageInfo>> items)
        {
            List<MessageInfo> toReturn = new List<MessageInfo>();
            foreach (var item in items)
            {
                var rowNumbers = String.Empty;
                for (int i = 0; i < item.Key.Count; i++)
                {
                    rowNumbers += item.Key[i];
                    if (i != item.Key.Count - 1)
                    {
                        rowNumbers += ",";
                    }
                }
                toReturn.AddRange(item.Value.Select(p => new MessageInfo()
                {
                    Field = p.Field,
                    Message = String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ImportRowError], rowNumbers, p.Message),
                }));
            }
            return toReturn;
        }

        private async Task SendGlOrdersImportEmailAsync(ICollection<OrderDynamic> orders, CustomerDynamic customer, int idPaymentMethod,
            int idAddedBy)
        {
            var model = await GetGlEmailModelAsync(orders, customer, idPaymentMethod, idAddedBy);
            await _notificationService.SendGLOrdersImportEmailAsync(model);
        }

        private async Task ExportGlCardDetails(ICollection<OrderDynamic> orders, CustomerDynamic customer, int idPaymentMethod, int idAddedBy)
        {
            var exportModel = await GetGlExportModelAsync(orders, customer, idPaymentMethod, idAddedBy);
            await _encryptedOrderExportService.ExportGiftListCreditCard(exportModel);
        }

        private async Task<GiftListExportModel> GetGlExportModelAsync(ICollection<OrderDynamic> orders, CustomerDynamic customer,
            int idPaymentMethod, int idAddedBy)
        {
            var profile = await _adminProfileRepository.Query(p => p.Id == idAddedBy).SelectFirstOrDefaultAsync(false);
            var model = new GiftListExportModel
            {
                Date = DateTime.Now,
                IdCustomer = customer.Id,
                CustomerFirstName = customer.ProfileAddress.SafeData.FirstName,
                CustomerLastName = customer.ProfileAddress.SafeData.LastName,
                IdPaymentMethod = idPaymentMethod,
                Agent = profile?.AgentId,
                ImportedOrdersCount = orders.Count,
                ImportedOrdersAmount = orders.Sum(p => p.Total),
                OrderIds = orders.Select(p => p.Id).ToList()
            };
            return model;
        }

        private async Task<GLOrdersImportEmail> GetGlEmailModelAsync(ICollection<OrderDynamic> orders, CustomerDynamic customer,
            int idPaymentMethod, int idAddedBy)
        {
            var profile = await _adminProfileRepository.Query(p => p.Id == idAddedBy).SelectFirstOrDefaultAsync(false);
            GLOrdersImportEmail model = new GLOrdersImportEmail
            {
                Date = DateTime.Now,
                IdCustomer = customer.Id,
                CustomerFirstName = customer.ProfileAddress.SafeData.FirstName,
                CustomerLastName = customer.ProfileAddress.SafeData.LastName,
                Agent = profile?.AgentId,
                ImportedOrdersCount = orders.Count,
                ImportedOrdersAmount = orders.Sum(p => p.Total),
                OrderIds = orders.Select(p => p.Id).ToList()
            };
            var creditCard = customer.CustomerPaymentMethods.FirstOrDefault(p => p.Id == idPaymentMethod);
            if (creditCard != null)
            {
                model.CardNumber = creditCard.SafeData.CardNumber;
            }
            return model;
        }

        private async Task LoadSkusDynamic(IList<OrderImportItemOrderDynamic> map, CustomerDynamic customer)
        {
            List<string> requestCodes =
                map.Where(p => p.Order.Skus != null)
                    .SelectMany(p => p.Order.Skus)
                    .Where(p => p.Sku != null)
                    .Select(p => p.Sku.Code)
                    .Distinct()
                    .ToList();

            var dbSkus = (await _productService.GetSkusOrderedAsync(requestCodes)).ToDictionary(p => p.Sku.Code,
                StringComparer.OrdinalIgnoreCase);
            foreach (var item in map)
            {
                if (item.Order.Skus != null)
                {
                    int index = 1;
                    foreach (var sku in item.Order.Skus)
                    {
                        SkuOrdered dbSku;
                        if (dbSkus.TryGetValue(sku.Sku.Code, out dbSku))
                        {
                            sku.Sku.Product = dbSku.Sku.Product;
                            sku.Sku = dbSku.Sku;
                            if (sku.Sku != null)
                            {
                                sku.Amount = customer.IdObjectType == (int) CustomerType.Retail
                                    ? sku.Sku.Price
                                    : customer.IdObjectType == (int) CustomerType.Wholesale ? sku.Sku.WholesalePrice : 0;
                            }
                        }
                        else
                        {
                            var firstRecord = item.OrderImportItems.First();
                            firstRecord.ErrorMessages.Add(AddErrorMessage("SKU " + index,
                                String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SkuNotFoundOrderImport], "SKU " + index,
                                    sku.Sku.Code)));
                            continue;
                        }

                        index++;
                    }
                }
            }
        }

        private MessageInfo AddErrorMessage(string field, string message)
        {
            return new MessageInfo()
            {
                Field = field ?? "Base", Message = message,
            };
        }

        #endregion

        #region AffiliatesOrders

        public async Task<PagedList<AffiliateOrderListItemModel>> GetAffiliateOrderPaymentsWithCustomerInfo(AffiliateOrderPaymentFilter filter)
        {
            PagedList<AffiliateOrderListItemModel> toReturn = new PagedList<AffiliateOrderListItemModel>();

            OrderQuery conditions = new OrderQuery().WithIdAffiliate(filter.IdAffiliate).WithPaymentStatus(filter.Status).Active().WithFromDate(filter.From).WithToDate(filter.To);
            Func<IQueryLite<Order>, IQueryLite<Order>> includes = (p => p.Include(o => o.PaymentMethod).ThenInclude(o => o.BillingAddress).ThenInclude(o => o.OptionValues).Include(o => o.PaymentMethod).ThenInclude(o => o.OptionValues).Include(o => o.PaymentMethod).ThenInclude(o => o.PaymentMethod).Include(o => o.AffiliateOrderPayment));

            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var result = await this.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, queryObject: conditions, orderBy: sortable, includesOverride: includes);

            var affiliateOrders = await _affiliateOrderPaymentRepository.GetAffiliateOrders(filter.IdAffiliate /*, result.Items.Select(p => p.Customer.Id).Distinct().ToList()*/);

            List<int> customerIds = new List<int>();
            toReturn.Count = result.Count;
            toReturn.Items = new List<AffiliateOrderListItemModel>();
            foreach (var order in result.Items)
            {
                string customerFirstName = null;
                string customerLastName = null;
                var customerOrdersCount = 0;
                if (order.PaymentMethod?.Address != null && order.PaymentMethod.Address.DictionaryData.ContainsKey("FirstName") &&
                    order.PaymentMethod.Address.DictionaryData.ContainsKey("LastName"))
                {
                    customerFirstName = order.PaymentMethod.Address.Data.FirstName;
                    customerLastName = order.PaymentMethod.Address.Data.LastName;
                }
                else
                {
                    customerIds.Add(order.Customer.Id);
                }
                if (affiliateOrders.ContainsKey(order.Customer.Id))
                {
                    customerOrdersCount = affiliateOrders[order.Customer.Id];
                }

                AffiliateOrderListItemModel item = new AffiliateOrderListItemModel(order.AffiliateOrderPayment, customerFirstName, customerLastName, customerOrdersCount);
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

        private ICollection<MessageInfo> ValidateUpdateHealthwiseOrder(OrderDynamic order)
        {
            List<MessageInfo> toReturn = new List<MessageInfo>();
            if (order == null)
            {
                toReturn.Add(new MessageInfo() {Message = "Invalid order #"});
            }
            if (order == null || (order.OrderStatus == OrderStatus.Incomplete || order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus == OrderStatus.ShipDelayed || order.OrderStatus == OrderStatus.OnHold || order.POrderStatus == OrderStatus.Incomplete || order.POrderStatus == OrderStatus.Cancelled || order.POrderStatus == OrderStatus.ShipDelayed || order.POrderStatus == OrderStatus.OnHold || order.NPOrderStatus == OrderStatus.Incomplete || order.NPOrderStatus == OrderStatus.Cancelled || order.NPOrderStatus == OrderStatus.ShipDelayed || order.NPOrderStatus == OrderStatus.OnHold))
            {
                toReturn.Add(new MessageInfo() {Message = "The given order can'be flagged"});
                throw new AppValidationException("The given order can'be flagged");
            }
            //if (!order.DictionaryData.ContainsKey("OrderType") || order.Data.OrderType != (int?)SourceOrderType.Web)
            //{
            //    toReturn.Add(new MessageInfo() { Message = "The given order can'be flagged" });
            //}
            return toReturn;
        }

        public async Task<bool> UpdateHealthwiseOrderWithValidationAsync(int orderId, bool isHealthwise)
        {
            var order = await this.SelectAsync(orderId);
            var messages = ValidateUpdateHealthwiseOrder(order);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            return await UpdateHealthwiseOrderManualAsync(order, isHealthwise);
        }

        public async Task<bool> UpdateHealthwiseOrderAsync(int orderId, bool isHealthwise)
        {
            var order = await this.SelectAsync(orderId);
            var messages = ValidateUpdateHealthwiseOrder(order);
            if (messages.Count > 0)
            {
                return false;
            }

            return await UpdateHealthwiseOrderManualAsync(order, isHealthwise);
        }

        private async Task<bool> UpdateHealthwiseOrderManualAsync(OrderDynamic order, bool isHealthwise)
        {
            using (var uow = CreateUnitOfWork())
            {
                using (var transaction = uow.BeginTransaction())
                {
                    try
                    { 
                        await UpdateHealthwiseOrderInnerAsync(uow, order.Id, order.Customer.Id, order.DateCreated,
                                isHealthwise, order.IsFirstHealthwise);
                        if (isHealthwise)
                        {
                            await MarkHealthwiseCustomerAsync(uow, order.Customer.Id);
                        }
                        await uow.SaveChangesAsync();
                        transaction.Commit();

                        return true;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public async Task MarkHealthwiseCustomerAsync(IUnitOfWorkAsync uow, int idCustomer)
        {
            var option = _customerMapper.OptionTypes.FirstOrDefault(p => p.Name == "HasHealthwiseOrders");
            if (option != null)
            {
                var customerOptionValueRepositoryAsync = uow.RepositoryAsync<CustomerOptionValue>();
                var optionValue = (await customerOptionValueRepositoryAsync.Query(p => p.IdCustomer == idCustomer && p.IdOptionType == option.Id).SelectFirstOrDefaultAsync(true));
                if (optionValue == null)
                {
                    optionValue = new CustomerOptionValue()
                    {
                        Value = "True", IdCustomer = idCustomer, IdOptionType = option.Id
                    };
                    await customerOptionValueRepositoryAsync.InsertAsync(optionValue);
                }
                else if (optionValue.Value != "True")
                {
                    optionValue.Value = "True";
                }
            }
        }

        private async Task UpdateHealthwiseOrderWithOrder(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            if (model.IdObjectType == (int) OrderType.AutoShipOrder || model.IdObjectType == (int) OrderType.Normal)
            {
                await
                    UpdateHealthwiseOrderInnerAsync(uow, model.Id, model.Customer.Id, DateTime.Now,
                        (bool?) model.SafeData.IsHealthwise ?? false, model.IsFirstHealthwise);
            }
        }

        private async Task UpdateHealthwiseOrderInnerAsync(IUnitOfWorkAsync uow, int idOrder, int idCustomer, DateTime orderDateCreated,
            bool isHealthwise, bool isFirstHealthwise)
        {
            var healthwiseOrderRepositoryAsync = uow.RepositoryAsync<HealthwiseOrder>();
            var healthwisePeriodRepositoryAsync = uow.RepositoryAsync<HealthwisePeriod>();
            if (!isHealthwise)
            {
                healthwiseOrderRepositoryAsync.Delete(idOrder);
            }
            else
            {
                HealthwiseOrder healthwiseOrder =
                    await healthwiseOrderRepositoryAsync.Query(p => p.Id == idOrder).SelectFirstOrDefaultAsync(false);
                if (healthwiseOrder == null)
                {
                    var maxCount = _appSettings.HealthwisePeriodMaxItemsCount;
                    var orderCreatedDate = orderDateCreated;
                    var periods =
                        (await
                                healthwisePeriodRepositoryAsync.Query(
                                    p =>
                                        p.IdCustomer == idCustomer && orderCreatedDate >= p.StartDate && orderCreatedDate < p.EndDate &&
                                        !p.PaidDate.HasValue).Include(p => p.HealthwiseOrders).ThenInclude(p => p.Order).SelectAsync(false))
                            .ToList();
                    foreach (var healthwisePeriod in periods)
                    {
                        healthwisePeriod.HealthwiseOrders =
                            healthwisePeriod.HealthwiseOrders.Where(
                                p =>
                                    p.Order.OrderStatus == OrderStatus.Processed || p.Order.OrderStatus == OrderStatus.Exported ||
                                    p.Order.OrderStatus == OrderStatus.Shipped || p.Order.POrderStatus == OrderStatus.Processed ||
                                    p.Order.POrderStatus == OrderStatus.Exported || p.Order.POrderStatus == OrderStatus.Shipped ||
                                    p.Order.NPOrderStatus == OrderStatus.Processed || p.Order.NPOrderStatus == OrderStatus.Exported ||
                                    p.Order.NPOrderStatus == OrderStatus.Shipped).ToList();
                    }
                    bool addedToPeriod = false;
                    foreach (var period in periods.OrderBy(p => p.StartDate))
                    {
                        if (period.HealthwiseOrders.Count < maxCount)
                        {
                            healthwiseOrder = new HealthwiseOrder()
                            {
                                Id = idOrder,
                                IdHealthwisePeriod = period.Id
                            };
                            healthwiseOrderRepositoryAsync.Insert(healthwiseOrder);
                            addedToPeriod = true;
                            break;
                        }
                    }
                    if (!addedToPeriod)
                    {
                        var period = new HealthwisePeriod();
                        period.IdCustomer = idCustomer;
                        period.StartDate = orderCreatedDate;
                        period.EndDate = period.StartDate.AddYears(1);
                        period.HealthwiseOrders = new List<HealthwiseOrder>()
                        {
                            new HealthwiseOrder() {Id = idOrder}
                        };
                        //using (var transaction = uow.BeginTransaction())
                        //{
                        //    try
                        //    {
                                healthwisePeriodRepositoryAsync.InsertGraph(period);
                                if (isFirstHealthwise)
                                {
                                    var customer = await _customerService.SelectAsync(idCustomer, true);
                                    customer.Data.HasHealthwiseOrders = true;
                                    await _customerService.UpdateAsync(customer);
                                }
                        //        transaction.Commit();
                        //    }
                        //            catch
                        //    {
                        //        transaction.Rollback();
                        //        throw;
                        //    }
                        //}
                    }
                    else
                    {
                        var customer = await _customerService.SelectAsync(idCustomer, true);
                        if (customer.SafeData.HasHealthwiseOrders == null || customer.SafeData.HasHealthwiseOrders == false)
                        {
                            customer.Data.HasHealthwiseOrders = true;
                            await _customerService.UpdateAsync(customer);
                        }
                    }
                }
            }
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
            VOrderWithRegionInfoItemQuery conditions = new VOrderWithRegionInfoItemQuery().WithDates(filter.From, filter.To).WithIdCustomerType(filter.IdCustomerType).WithIdOrderType(filter.IdOrderType).WithRegion(filter.Region).WithZip(filter.Zip);
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
            var orders = (await SelectAsync(orderToGCs.Select(p => p.IdOrder).ToList())).OrderByDescending(p => p.DateCreated);
            foreach (var orderToGC in orderToGCs)
            {
                var order = orders.FirstOrDefault(p => p.Id == orderToGC.IdOrder);
                if (order != null)
                {
                    GCOrderItem item = new GCOrderItem();
                    item.Order = order;
                    item.GCAmountUsed = orderToGC.Amount;
                    toReturn.Add(item);
                }
            }
            var ids = toReturn.Where(p => p.Order.IdEditedBy.HasValue).Select(p => p.Order.IdEditedBy.Value).Distinct().ToList();
            var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false);
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

        public async Task<ICollection<SkuOrdered>> GetGeneratedGcs(int id)
        {
            var items =
                await
                    _orderToSkusRepository.Query(
                        s =>
                            s.IdOrder == id &&
                            (s.Sku.Product.IdObjectType == (int) ProductType.EGс || s.Sku.Product.IdObjectType == (int) ProductType.Gc))
                        .Include(g => g.Sku)
                        .ThenInclude(s => s.OptionValues)
                        .Include(g => g.Sku)
                        .ThenInclude(s => s.Product)
                        .ThenInclude(p => p.OptionValues)
                        .Include(s => s.GeneratedGiftCertificates)
                        .SelectAsync(false);

            return items.Select(s => new SkuOrdered
            {
                Sku = _skuMapper.FromEntity(s.Sku, true), GcsGenerated = s.GeneratedGiftCertificates, Quantity = s.Quantity, Amount = s.Amount
            }).ToList();
        }

        public Task<int> GetReshipCount(int pastMonths, int idCustomer)
        {
            var endDate = DateTime.Now;
            var startDate = endDate.AddMonths(-pastMonths);
            return
                SelectCountAsync(
                    o =>
                        o.IdCustomer == idCustomer && o.IdObjectType == (int) OrderType.Reship && o.OrderStatus != OrderStatus.Cancelled &&
                        o.StatusCode != (int) RecordStatusCode.Deleted &&
                        o.DateCreated > startDate && o.DateCreated <= endDate, q => q);
        }

        #endregion

        private struct OrderPaymentReference
        {
            public OrderDynamic OriginalReference { get; set; }
            public OrderCardData PaymentMethod { get; set; }
        }
    }
}