using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
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
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using VitalChoice.Business.CsvExportMaps;
using VitalChoice.Infrastructure.Domain.Entities.Orders;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using FluentValidation.Validators;
using System.Text.RegularExpressions;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Business.Mail;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;

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
        private readonly SkuMapper _skuMapper;
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
        private readonly ICountryService _countryService;
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly IExtendedDynamicServiceAsync<OrderPaymentMethodDynamic, OrderPaymentMethod, CustomerPaymentMethodOptionType, OrderPaymentMethodOptionValue> _paymentGenericService;
        private readonly IEcommerceRepositoryAsync<GiftCertificate> _giftCertificatesRepository;
        private readonly IEcommerceRepositoryAsync<OrderToSku> _orderToSkusRepository;

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
            DynamicExtensionsRewriter queryVisitor,
            IEcommerceRepositoryAsync<HealthwiseOrder> healthwiseOrderRepositoryAsync,
            IEcommerceRepositoryAsync<HealthwisePeriod> healthwisePeriodRepositoryAsync,
            IAppInfrastructureService appInfrastructureService,
            IEncryptedOrderExportService encryptedOrderExportService,
            SPEcommerceRepository sPEcommerceRepository,
            IPaymentMethodService paymentMethodService,
            IObjectMapper<OrderPaymentMethodDynamic> paymentMapper,
            IEcommerceRepositoryAsync<OrderToGiftCertificate> orderToGiftCertificateRepositoryAsync,
            IExtendedDynamicServiceAsync<OrderPaymentMethodDynamic, OrderPaymentMethod, CustomerPaymentMethodOptionType, OrderPaymentMethodOptionValue> paymentGenericService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            IProductService productService,
            INotificationService notificationService,
            ICountryService countryService, ITransactionAccessor<EcommerceContext> transactionAccessor, IEcommerceRepositoryAsync<GiftCertificate> giftCertificatesRepository, SkuMapper skuMapper, IEcommerceRepositoryAsync<OrderToSku> orderToSkusRepository)
            : base(
                mapper, orderRepository, orderValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, directMapper, queryVisitor, transactionAccessor)
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
            _paymentGenericService = paymentGenericService;
            _countryService = countryService;
            _giftCertificatesRepository = giftCertificatesRepository;
            _skuMapper = skuMapper;
            _orderToSkusRepository = orderToSkusRepository;
            _addressMapper = addressMapper;
            _productService = productService;
            _notificationService = notificationService;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
        }

        protected override IQueryLite<Order> BuildQuery(IQueryLite<Order> query)
        {
            return
                query.Include(o => o.Discount)
                    .ThenInclude(d => d.OptionValues)
                    .Include(o => o.Discount)
                    .ThenInclude(d => d.DiscountTiers)
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
                    .ThenInclude(s => s.InventorySkus)
                    .Include(o => o.Skus)
                    .ThenInclude(s => s.GeneratedGiftCertificates)
                    .Include(p => p.OptionValues)
                    .Include(o => o.HealthwiseOrder)
                    .Include(o => o.ReshipProblemSkus)
                    .ThenInclude(g => g.Sku);
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
            if (entities.All(e => e.PromoSkus != null))
            {
                foreach (
                    var orderToSku in
                        entities.SelectMany(o => o.PromoSkus).Where(s => s.Sku?.Product != null && s.Sku.OptionTypes == null))
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

        protected override async Task AfterEntityChangesAsync(OrderDynamic model, Order updated, Order initial, IUnitOfWorkAsync uow)
        {
            //We need to manually remove generated but unlinked gift certificates
            var gcRep = uow.RepositoryAsync<GiftCertificate>();
            if ((model.OrderStatus == null || model.OrderStatus.Value != OrderStatus.Incomplete) &&
                (model.POrderStatus != OrderStatus.Incomplete || model.NPOrderStatus != OrderStatus.Incomplete))
            {
                var toLoadUp =
                    new HashSet<int>(updated.GiftCertificates.Where(g => g.GiftCertificate == null).Select(g => g.IdGiftCertificate));
                var gcs = await gcRep.Query(g => toLoadUp.Contains(g.Id)).SelectAsync();
                updated.GiftCertificates.ForEach(g =>
                {
                    if (g.GiftCertificate == null)
                    {
                        g.GiftCertificate = gcs.FirstOrDefault(db => db.Id == g.IdGiftCertificate);
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

        public async Task<OrderDynamic> SelectWithCustomerAsync(int id, bool withDefaults = false)
        {
            var order = await SelectAsync(id, withDefaults);
            order.Customer = await _customerService.SelectAsync(order.Customer.Id, withDefaults);
            return order;
        }

        public async Task<OrderDataContext> CalculateOrder(OrderDynamic order, OrderStatus combinedStatus, IWorkflowTree<OrderDataContext, decimal> tree)
        {
            var context = new OrderDataContext(combinedStatus)
            {
                Order = order
            };
            await tree.ExecuteAsync(context);
            UpdateOrderFromCalculationContext(order, context);
            return context;
        }

        public async Task<OrderDataContext> CalculateOrder(OrderDynamic order, OrderStatus combinedStatus)
        {
            var context = new OrderDataContext(combinedStatus)
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
            var orderQuery = new OrderQuery().NotDeleted().WithActualStatusOnly().WithCustomerId(customerId);
            var order = (await _orderRepository.Query(orderQuery).OrderBy(p => p.OrderByDescending(x => x.Id)).SelectAsync(false)).FirstOrDefault();
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
            order.PromoSkus = dataContext.PromoSkus;
            SetOrderSplitStatuses(dataContext, order);
        }

        public async Task OrderTypeSetup(OrderDynamic order)
        {
            var orderType = order.Data.MailOrder ? (int?)SourceOrderType.MailOrder : null;
            var idOrder = order.Id;
            if (idOrder != 0)
            {
                var optionType = DynamicMapper.OptionsForType(order.IdObjectType).FirstOrDefault(o => o.Name == "OrderType")?.Id;
                if (optionType != null)
                {
                    var dbItem =
                        await
                            OptionValuesRepository.Query()
                                .Where(v => v.IdOrder == idOrder && v.IdOptionType == optionType.Value)
                                .SelectFirstOrDefaultAsync(false);
                    if (dbItem != null)
                    {
                        if (dbItem.Value == ((int)SourceOrderType.MailOrder).ToString())
                        {
                            if (!orderType.HasValue)
                            {
                                orderType = (int)SourceOrderType.Phone;
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
                        order.Data.OrderType = orderType ?? (int)SourceOrderType.Phone;
                    }
                }
            }
            else
            {
                if (!orderType.HasValue)
                {
                    orderType = (int)SourceOrderType.Phone;
                }
                order.Data.OrderType = orderType.Value;
                order.ShippingAddress.Id = 0;
                if (order.PaymentMethod.Address != null)
                {
                    order.PaymentMethod.Address.Id = 0;
                }
                order.PaymentMethod.Id = 0;
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
                else if ((int?)dynamic.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder)
                {
                    dynamic.POrderStatus = combinedStatus;
                    dynamic.NPOrderStatus = combinedStatus;
                    if (dynamic.SafeData.ShipDelayDate != null)
                    {
                        dynamic.POrderStatus = OrderStatus.ShipDelayed;
                        dynamic.NPOrderStatus = OrderStatus.ShipDelayed;
                    }
                }
                else if ((int?)dynamic.SafeData.ShipDelayType == (int)ShipDelayType.PerishableAndNonPerishable)
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
                    if ((int?)dynamic.SafeData.ShipDelayType == (int)ShipDelayType.EntireOrder && dynamic.SafeData.ShipDelayDate != null)
                    {
                        dynamic.OrderStatus = OrderStatus.ShipDelayed;
                    }
                }
            }
        }

        protected override Task<List<MessageInfo>> ValidateAsync(OrderDynamic dynamic)
        {
            if (dynamic.Customer.StatusCode == (int)CustomerStatus.Suspended)
            {
                throw new CustomerSuspendException();
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

        public async Task<bool> CancelOrderAsync(int id)
        {
            var toReturn = false;
            var order = await SelectAsync(id, false);
            if (order != null)
            {
                if (order.OrderStatus == OrderStatus.Shipped || order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus == OrderStatus.Exported ||
                    order.POrderStatus == OrderStatus.Shipped || order.POrderStatus == OrderStatus.Cancelled || order.POrderStatus == OrderStatus.Exported ||
                    order.NPOrderStatus == OrderStatus.Shipped || order.NPOrderStatus == OrderStatus.Cancelled || order.NPOrderStatus == OrderStatus.Exported)
                {
                    throw new AppValidationException("This operation isn't allowed for the order in the given status");
                }

                using (var uow = CreateUnitOfWork())
                {
                    using (var transaction = uow.BeginTransaction())
                    {
                        try
                        {
                            var giftCertificateRepository = uow.RepositoryAsync<GiftCertificate>();
                            List<GiftCertificate> generatedGcs = new List<GiftCertificate>();
                            if (order.Skus.Any(s => s.GcsGenerated?.Any() ?? false))
                            {
                                generatedGcs =
                                    await
                                        giftCertificateRepository.Query(
                                            p => p.IdOrder == order.Id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync();
                                generatedGcs.ForEach(p =>
                                {
                                    p.StatusCode = RecordStatusCode.Deleted;
                                });
                            }

                            List<GiftCertificate> usedGcs = new List<GiftCertificate>();
                            if (order.GiftCertificates?.Count > 0)
                            {
                                var ids = order.GiftCertificates.Select(p => p.GiftCertificate?.Id).ToList();
                                usedGcs = await giftCertificateRepository.Query(p => ids.Contains(p.Id)).SelectAsync();
                                foreach (var giftCertificateInOrder in order.GiftCertificates)
                                {
                                    var gc = usedGcs.FirstOrDefault(p => p.Id == giftCertificateInOrder.GiftCertificate?.Id);
                                    if (gc != null)
                                    {
                                        gc.Balance += giftCertificateInOrder.Amount;
                                        if (giftCertificateInOrder.GiftCertificate != null)
                                        {
                                            giftCertificateInOrder.GiftCertificate.Balance = gc.Balance;
                                        }
                                    }
                                }
                                order.GiftCertificates.Clear();
                            }

                            //TODO: add removing one time per a customer discount info

                            await uow.SaveChangesAsync();
                            giftCertificateRepository.DetachAll(generatedGcs);
                            giftCertificateRepository.DetachAll(usedGcs);                            

                            if (order.OrderStatus.HasValue)
                            {
                                order.OrderStatus=OrderStatus.Cancelled;
                            }
                            if (order.POrderStatus.HasValue)
                            {
                                order.POrderStatus = OrderStatus.Cancelled;
                            }
                            if (order.NPOrderStatus.HasValue)
                            {
                                order.NPOrderStatus = OrderStatus.Cancelled;
                            }

                            await base.UpdateAsync(order, uow);

                            transaction.Commit();

                            var dbEntity = await SelectEntityFirstAsync(o => o.Id == id);
                            await LogItemChanges(new[] { await DynamicMapper.FromEntityAsync(dbEntity) });

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

        protected override async Task<Order> InsertAsync(OrderDynamic model, IUnitOfWorkAsync uow)
        {
            Order entity;
            Task<bool> remoteUpdateTask;
            using (var transaction = uow.BeginTransaction())
            {
                try
                {
                    SetPOrderType(new List<OrderDynamic>() { model });
                    await SetSkusBornDate(new[] { model }, uow);
                    await EnsurePaymentMethod(model);
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

        private async Task EnsurePaymentMethod(OrderDynamic model)
        {
            if (model.PaymentMethod == null)
            {
                switch ((OrderType)model.IdObjectType)
                {
                    case OrderType.Normal:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int)PaymentMethodType.CreditCard);
                        break;
                    case OrderType.AutoShip:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int)PaymentMethodType.CreditCard);
                        break;
                    case OrderType.DropShip:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int)PaymentMethodType.NoCharge);
                        break;
                    case OrderType.GiftList:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int)PaymentMethodType.NoCharge);
                        break;
                    case OrderType.Reship:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int)PaymentMethodType.CreditCard);
                        break;
                    case OrderType.Refund:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int)PaymentMethodType.CreditCard);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
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
                    SetPOrderType(models);
                    await SetSkusBornDate(models, uow);
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
                    SetPOrderType(new List<OrderDynamic>() { model });
                    await SetSkusBornDate(new[] { model }, uow);
                    await EnsurePaymentMethod(model);
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
                    SetPOrderType(models);
                    await SetSkusBornDate(models, uow);
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

        private async Task SetSkusBornDate(ICollection<OrderDynamic> orders, IUnitOfWorkAsync uow)
        {
            var option = _productService.GetProductOptionTypes(new HashSet<string>() { ProductConstants.FIELD_NAME_SKU_INVENTORY_BORN_DATE }).FirstOrDefault();
            if (option != null)
            {
                var skuIds = orders.SelectMany(p => p?.Skus).Select(p => p.Sku.Id).ToList();
                skuIds.AddRange(orders.SelectMany(p => p?.PromoSkus).Select(p => p.Sku.Id).ToList());
                skuIds = skuIds.Distinct().ToList();
                var dbOptionValues = await _productService.GetSkuOptionValues(skuIds, new[] { option.Id });
                var skuIdsForInsert = skuIds.Except(dbOptionValues.Select(p => p.IdSku)).ToList();
                if (skuIdsForInsert.Any())
                {
                    var now = DateTime.Now.ToString(CultureInfo.InvariantCulture);
                    var skuOptionValueRepository = uow.RepositoryAsync<SkuOptionValue>();
                    skuOptionValueRepository.InsertRange(skuIdsForInsert.Select(p => new SkuOptionValue()
                    {
                        IdOptionType = option.Id,
                        IdSku = p,
                        Value = now
                    }));
                }
            }
        }

        private async Task UpdateAffiliateOrderPayment(OrderDynamic dynamic, IUnitOfWorkAsync uow)
        {
            if (!dynamic.IdAddedBy.HasValue && dynamic.Customer.IdAffiliate.HasValue &&
                dynamic.AffiliatePaymentAmount.HasValue && dynamic.AffiliateNewCustomerOrder.HasValue)
            {
                AffiliateOrderPayment payment = new AffiliateOrderPayment();
                payment.Id = dynamic.Id;
                payment.Status = AffiliateOrderPaymentStatus.NotPaid;
                payment.IdAffiliate = dynamic.Customer.IdAffiliate.Value;
                //TODO - calculate commission and set is a first order or no the given customer
                payment.Amount = dynamic.AffiliatePaymentAmount.Value;
                payment.NewCustomerOrder = dynamic.AffiliateNewCustomerOrder.Value;

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
                        var periods = (await healthwisePeriodRepository.Query(p => p.IdCustomer == model.Customer.Id && dateNow >= p.StartDate && dateNow < p.EndDate && !p.PaidDate.HasValue).Include(p => p.HealthwiseOrders).SelectAsync(false)).ToList();

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
                                new HealthwiseOrder() {Id = model.Id}
                            };
                            _healthwisePeriodRepositoryAsync.InsertGraph(period);
                        }

                        await uow.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<PagedList<OrderInfoItem>> GetOrdersAsync2(VOrderFilter filter)
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
            conditions = conditions
                .WithId(filter.Id)//TODO - should be redone after adding - https://github.com/aspnet/EntityFramework/issues/2850
                .WithOrderType(filter.IdObjectType)
                .WithOrderStatus(filter.OrderStatus)
                .WithCustomerType(filter.IdCustomerType)
                .WithoutIncomplete(filter.OrderStatus, filter.IgnoreNotShowingIncomplete)
                .WithShipState(filter.IdShipState)
                .WithOrderDynamicValues(filter.IdOrderSource, filter.POrderType, filter.IdShippingMethod)
                .WithCustomerDynamicValues(filter.CustomerFirstName, filter.CustomerLastName, filter.CustomerCompany);

            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VOrderSortPath.IdPaymentMethod:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.PaymentMethod.IdObjectType) : x.OrderByDescending(y => y.PaymentMethod.IdObjectType);
                    break;
                case VOrderSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated).ThenBy(y => y.Id)
                                : x.OrderByDescending(y => y.DateCreated).ThenByDescending(y => y.Id);
                    break;
                case VOrderSortPath.IdCustomerType:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.Customer.IdObjectType) : x.OrderByDescending(y => y.Customer.IdObjectType);
                    break;
                case VOrderSortPath.IdObjectType:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.IdObjectType) : x.OrderByDescending(y => y.IdObjectType);
                    break;
                case VOrderSortPath.Id:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.Id) : x.OrderByDescending(y => y.Id);
                    break;
                case VOrderSortPath.Total:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.Total) : x.OrderByDescending(y => y.Total);
                    break;
                case VOrderSortPath.DateEdited:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.DateEdited) : x.OrderByDescending(y => y.DateEdited);
                    break;
            }

            var orders = await SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, conditions,
                    includes => includes.Include(c => c.OptionValues).Include(c => c.PaymentMethod).
                    Include(c => c.ShippingAddress).ThenInclude(c => c.OptionValues).
                    Include(c => c.Customer).ThenInclude(p => p.ProfileAddress).ThenInclude(c => c.OptionValues).
                    Include(c => c.HealthwiseOrder),
                    orderBy: sortable, withDefaults: true);

            var countries = await _countryService.GetCountriesAsync(new CountryFilter());

            PagedList<OrderInfoItem> toReturn = new PagedList<OrderInfoItem>()
            {
                Items = orders.Items.Select(p => new OrderInfoItem()
                {
                    Id = p.Id,
                    IdObjectType = (OrderType)p.IdObjectType,
                    OrderStatus = p.OrderStatus,
                    POrderStatus = p.POrderStatus,
                    NPOrderStatus = p.NPOrderStatus,
                    IdOrderSource = p.SafeData.OrderType,
                    OrderNotes = p.SafeData.OrderNotes,
                    IdPaymentMethod = p.PaymentMethod?.IdObjectType,
                    DateCreated = p.DateCreated,
                    DateShipped = p.SafeData.ShipDelayDate,
                    PDateShipped = p.SafeData.ShipDelayDateP,
                    NPDateShipped = p.SafeData.ShipDelayDateNP,
                    Total = p.Total,
                    IdEditedBy = p.IdEditedBy,
                    DateEdited = p.DateEdited,
                    POrderType = p.SafeData.POrderType,
                    IdCustomerType = p.Customer.IdObjectType,
                    IdCustomer = p.Customer.Id,
                    Company = p.Customer?.ProfileAddress.SafeData.Company,
                    Customer = p.Customer?.ProfileAddress.SafeData.FirstName + " " + p.Customer?.ProfileAddress.SafeData.LastName,
                    StateCode = countries.SelectMany(pp => pp.States).FirstOrDefault(pp => pp.Id == p.ShippingAddress?.IdState)?.StateCode,
                    ShipTo = p?.ShippingAddress.SafeData.FirstName + " " + p?.ShippingAddress.SafeData.LastName,
                    PreferredShipMethod = p.SafeData.PreferredShipMethod,
                    Healthwise = p.IsHealthwise,
                }).ToList(),
                Count = orders.Count
            };

            if (toReturn.Items.Any())
            {
                var ids = toReturn.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy.Value).Distinct().ToList();
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
            conditions = conditions.WithOrderStatus(filter.OrderStatus).WithoutIncomplete(filter.OrderStatus, filter.IgnoreNotShowingIncomplete).WithId(filter.IdString) //TODO - should be redone after adding - https://github.com/aspnet/EntityFramework/issues/2850
                .WithOrderType(filter.IdObjectType).WithOrderSource(filter.IdOrderSource).WithPOrderType(filter.POrderType).WithCustomerType(filter.IdCustomerType).WithShippingMethod(filter.IdShippingMethod);

            var query = _vOrderRepository.Query(conditions);

            Func<IQueryable<VOrder>, IOrderedQueryable<VOrder>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VOrderSortPath.OrderStatus:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.OrderStatus) : x.OrderByDescending(y => y.OrderStatus);
                    break;
                case VOrderSortPath.IdOrderSource:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.IdOrderSource) : x.OrderByDescending(y => y.IdOrderSource);
                    break;
                case VOrderSortPath.IdPaymentMethod:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.IdPaymentMethod) : x.OrderByDescending(y => y.IdPaymentMethod);
                    break;
                case VOrderSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated).ThenBy(y => y.Id)
                                : x.OrderByDescending(y => y.DateCreated).ThenByDescending(y => y.Id);
                    break;
                case VOrderSortPath.DateShipped:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.DateShipped) : x.OrderByDescending(y => y.DateShipped);
                    break;
                case VOrderSortPath.Company:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.Company) : x.OrderByDescending(y => y.Company);
                    break;
                case VOrderSortPath.StateCode:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.StateCode) : x.OrderByDescending(y => y.StateCode);
                    break;
                case VOrderSortPath.IdCustomerType:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.IdCustomerType) : x.OrderByDescending(y => y.IdCustomerType);
                    break;
                case VOrderSortPath.IdObjectType:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.IdObjectType) : x.OrderByDescending(y => y.IdObjectType);
                    break;
                case VOrderSortPath.Customer:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.Customer) : x.OrderByDescending(y => y.Customer);
                    break;
                case VOrderSortPath.ShipTo:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.ShipTo) : x.OrderByDescending(y => y.ShipTo);
                    break;
                case VOrderSortPath.Id:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.Id) : x.OrderByDescending(y => y.Id);
                    break;
                case VOrderSortPath.Total:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.Total) : x.OrderByDescending(y => y.Total);
                    break;
                case VOrderSortPath.DateEdited:
                    sortable = (x) => sortOrder == SortOrder.Asc ? x.OrderBy(y => y.DateEdited) : x.OrderByDescending(y => y.DateEdited);
                    break;
            }

            var toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            if (toReturn.Items.Any())
            {
                var ids = toReturn.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy.Value).Distinct().ToList();
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

        public async Task<OrderDynamic> CreateNewNormalOrder(OrderStatus status)
        {
            var toReturn = await Mapper.CreatePrototypeAsync((int)OrderType.Normal);

            toReturn.StatusCode = (int)RecordStatusCode.Active;
            if (status != OrderStatus.Processed && status != OrderStatus.Incomplete)
            {
                throw new Exception("New normal order invalid status");
            }
            toReturn.OrderStatus = status;
            toReturn.DateCreated = DateTime.Now;
            toReturn.Data.ShipDelayType = (int)ShipDelayType.None;

            return toReturn;
        }

        private void SetPOrderType(ICollection<OrderDynamic> items)
        {
            if (items != null)
            {
                foreach (var item in items)
                {
                    POrderType? toReturn = null;
                    var pOrder = false;
                    var npOrder = false;
                    if (item.Skus != null)
                    {
                        foreach (var skuOrdered in item.Skus)
                        {
                            pOrder = pOrder || skuOrdered?.Sku.Product.IdObjectType == (int)ProductType.Perishable;
                            npOrder = npOrder || skuOrdered?.Sku.Product.IdObjectType == (int)ProductType.NonPerishable;
                        }
                    }
                    if (item.PromoSkus != null)
                    {
                        foreach (var skuOrdered in item.PromoSkus.Where(p => p.Enabled))
                        {
                            pOrder = pOrder || skuOrdered?.Sku.Product.IdObjectType == (int)ProductType.Perishable;
                            npOrder = npOrder || skuOrdered?.Sku.Product.IdObjectType == (int)ProductType.NonPerishable;
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
                    item.Data.POrderType = (int?)toReturn;
                }
            }
        }

        #region OrdersImport

        public async Task<bool> ImportOrders(byte[] file, string fileName, OrderType orderType, int idCustomer, int idPaymentMethod, int idAddedBy)
        {
            var customer = await _customerService.SelectAsync(idCustomer);
            if (customer == null)
            {
                throw new AppValidationException("Invalid file format");
            }
            if (!customer.ApprovedPaymentMethods.Contains((int)PaymentMethodType.NoCharge))
            {
                throw new AppValidationException("Payment method \"No Charge\" should be allowed");
            }
            if (orderType != OrderType.GiftList && orderType != OrderType.DropShip)
            {
                throw new ApiException("Orders import with the given orderType isn't implemented");
            }

            List<OrderBaseImportItem> records = null;
            Dictionary<string, OrderValidationGenericProperty> validationSettings = null;
            var countries = await _countryService.GetCountriesAsync(new CountryFilter());
            using (var memoryStream = new MemoryStream(file))
            {
                using (var streamReader = new StreamReader(memoryStream))
                {
                    CsvConfiguration configuration = new CsvConfiguration();
                    configuration.TrimFields = true;
                    configuration.TrimHeaders = true;
                    configuration.RegisterClassMap<OrderGiftListImportItemCsvMap>();
                    configuration.RegisterClassMap<OrderDropShipImportItemCsvMap>();
                    using (var csv = new CsvReader(streamReader, configuration))
                    {
                        records = ProcessImportOrderItems(csv, orderType, countries, out validationSettings);
                    }
                }
            }

            if (records != null && validationSettings != null)
            {
                ValidateOrderImportItems(records, validationSettings);
            }

            //throw parsing and validation errors
            var messages = FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var map = OrdersForImportBaseConvert(records, orderType, customer, idAddedBy);

            await LoadSkusDynamic(map, customer);
            //not found SKU errors
            messages = FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var tree = await _treeFactory.CreateTreeAsync<OrderDataContext, decimal>("Order");

            foreach (var item in map)
            {
                var orderCombinedStatus = item.Order.OrderStatus ?? OrderStatus.Processed;
                item.Order.Data.ShipDelayType = item.Order.SafeData.ShipDelayDate != null ? ShipDelayType.EntireOrder : ShipDelayType.None;

                var context = await CalculateOrder(item.Order, orderCombinedStatus, tree);

                item.OrderImportItem.ErrorMessages.AddRange(context.Messages);
                if (context.SkuOrdereds != null)
                {
                    item.OrderImportItem.ErrorMessages.AddRange(
                        context.SkuOrdereds.Where(p => p.Messages != null).SelectMany(p => p.Messages).Select(p =>
                            new MessageInfo()
                            {
                                Message = p
                            }));
                }
                if (context.PromoSkus != null)
                {
                    item.OrderImportItem.ErrorMessages.AddRange(
                        context.PromoSkus.Where(p => p.Enabled && p.Messages != null).SelectMany(p => p.Messages).Select(p =>
                            new MessageInfo()
                            {
                                Message = p
                            }));
                }
            }

            //throw calculating errors
            messages = FormatRowsRecordErrorMessages(map.Select(p => p.OrderImportItem));
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var orders = map.Select(p => p.Order).ToList();
            orders = await InsertRangeAsync(orders);

            if (orderType == OrderType.GiftList)
            {
                await SendGLOrdersImportEmailAsync(orders, customer, idPaymentMethod, idAddedBy);
            }

            return true;
        }

        private async Task SendGLOrdersImportEmailAsync(ICollection<OrderDynamic> orders, CustomerDynamic customer, int idPaymentMethod, int idAddedBy)
        {
            GLOrdersImportEmail model = new GLOrdersImportEmail();
            model.Date = DateTime.Now;
            model.IdCustomer = customer.Id;
            model.CustomerFirstName = customer.ProfileAddress.SafeData.FirstName;
            model.CustomerLastName = customer.ProfileAddress.SafeData.LastName;
            var creditCard = customer.CustomerPaymentMethods.FirstOrDefault(p => p.Id == idPaymentMethod);
            if (creditCard != null)
            {
                model.CardNumber = creditCard.SafeData.CardNumber;
            }
            var profile = (await _adminProfileRepository.Query(p => p.Id == idAddedBy).SelectAsync(false)).FirstOrDefault();
            model.Agent = profile?.AgentId;
            model.ImportedOrdersCount = orders.Count;
            model.ImportedOrdersAmount = orders.Sum(p => p.Total);
            model.OrderIds = orders.Select(p => p.Id).ToList();

            await _notificationService.SendGLOrdersImportEmailAsync(model);
        }

        private async Task LoadSkusDynamic(List<OrderImportItemOrderDynamic> map, CustomerDynamic customer)
        {
            List<string> requestCodes = map.Select(p => p.Order).Where(p => p.Skus != null).SelectMany(p => p.Skus).Where(p => p.Sku != null).Select(p => p.Sku.Code).ToList();

            var dbSkus = await _productService.GetSkusOrderedAsync(requestCodes);
            foreach (var item in map)
            {
                if (item.Order.Skus != null)
                {
                    int index = 1;
                    foreach (var sku in item.Order.Skus)
                    {
                        var dbSku = dbSkus.FirstOrDefault(p => p.Sku.Code == sku.Sku.Code);
                        if (dbSku == null)
                        {
                            item.OrderImportItem.ErrorMessages.Add(AddErrorMessage("SKU " + index, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SkuNotFoundOrderImport],
                                "SKU " + index, sku.Sku.Code)));
                            continue;
                        }
                        else
                        {
                            sku.Sku.Product = dbSku.Sku.Product;
                            sku.Sku = dbSku.Sku;
                            if (sku.Sku != null)
                            {
                                sku.Amount = customer.IdObjectType == (int)CustomerType.Retail ? sku.Sku.Price :
                                    customer.IdObjectType == (int)CustomerType.Wholesale ? sku.Sku.WholesalePrice : 0;
                            }
                        }

                        index++;
                    }
                }
            }
        }

        private List<OrderImportItemOrderDynamic> OrdersForImportBaseConvert(List<OrderBaseImportItem> records, OrderType orderType, CustomerDynamic customer,
            int idAddedBy)
        {
            List<OrderImportItemOrderDynamic> toReturn = new List<OrderImportItemOrderDynamic>();
            foreach (var record in records)
            {
                var order = Mapper.CreatePrototype((int)orderType);
                order.IdEditedBy = idAddedBy;
                order.Customer = customer;
                order.ShippingAddress = _addressMapper.FromModel(record, (int)AddressType.Shipping);
                record.SetFields(order);
                toReturn.Add(new OrderImportItemOrderDynamic
                {
                    OrderImportItem = record,
                    Order = order,
                });
            }
            return toReturn;
        }

        private List<OrderBaseImportItem> ProcessImportOrderItems(CsvReader reader, OrderType orderType, ICollection<Country> countries, out Dictionary<string, OrderValidationGenericProperty> validationSettings)
        {
            List<OrderBaseImportItem> toReturn = new List<OrderBaseImportItem>();

            Type orderImportItemType;
            if (orderType == OrderType.GiftList)
            {
                orderImportItemType = typeof(OrderGiftListImportItem);
            }
            else if (orderType == OrderType.DropShip)
            {
                orderImportItemType = typeof(OrderDropShipImportItem);
            }
            else
            {
                throw new ApiException("Orders import with the given orderType isn't implemented");
            }
            PropertyInfo[] modelProperties = orderImportItemType.GetProperties();
            validationSettings = GetOrderImportValidationSettings(modelProperties);

            var shipDateProperty = modelProperties.FirstOrDefault(p => p.Name == nameof(OrderGiftListImportItem.ShipDelayDate));
            var shipDateHeader = shipDateProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name;

            var skuProperties = typeof(OrderSkuImportItem).GetProperties();
            var skuProperty = skuProperties.FirstOrDefault(p => p.Name == nameof(OrderSkuImportItem.SKU));
            var skuBaseHeader = skuProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name;
            var qtyProperty = skuProperties.FirstOrDefault(p => p.Name == nameof(OrderSkuImportItem.QTY));
            var qtyBaseHeader = qtyProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name;// ReSharper disable all

            int rowNumber = 1;
            try
            {
                while (reader.Read())
                {
                    OrderBaseImportItem item = (OrderBaseImportItem)reader.GetRecord(orderImportItemType);
                    item.RowNumber = rowNumber;
                    var messages = new List<MessageInfo>();
                    rowNumber++;

                    if (orderImportItemType == typeof(OrderGiftListImportItem))
                    {
                        ((OrderGiftListImportItem)item).ShipDelayDate = ParseOrderShipDate(reader, shipDateHeader, ref messages);
                    }
                    if (orderImportItemType == typeof(OrderDropShipImportItem))
                    {
                        ((OrderDropShipImportItem)item).ShipDelayDate = ParseOrderShipDate(reader, shipDateHeader, ref messages);
                    }
                    item.Skus = ParseOrderSkus(reader, skuBaseHeader, qtyBaseHeader, ref messages);

                    int? idState = null;
                    int? idCountry = null;
                    GetContryAndState(item.State, item.Country, countries, ref messages, out idState, out idCountry);
                    item.IdState = idState;
                    item.IdCountry = idCountry;

                    if (!String.IsNullOrEmpty(item.Phone))
                    {
                        item.Phone = item.Phone.Replace(" ", "").Replace("+", "").Replace("-", "");
                    }

                    item.ErrorMessages = messages;
                    toReturn.Add(item);
                }
            }
            catch (Exception e)
            {
                throw new AppValidationException("Invalid file format");
            }
            return toReturn;
        }

        private void ValidateOrderImportItems(ICollection<OrderBaseImportItem> models, Dictionary<string, OrderValidationGenericProperty> settings)
        {
            EmailValidator emailValidator = new EmailValidator();
            var emailRegex = new Regex(emailValidator.Expression, RegexOptions.IgnoreCase);
            foreach (var model in models)
            {
                foreach (var pair in settings)
                {
                    var setting = pair.Value;

                    bool valid = true;
                    if (typeof(string) == setting.PropertyType)
                    {
                        string value = (string)setting.Get(model);
                        if (setting.IsRequired && String.IsNullOrEmpty(value))
                        {
                            model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsRequired],
                                setting.DisplayName)));
                            valid = false;
                        }

                        if (valid && setting.MaxLength.HasValue && !String.IsNullOrEmpty(value) && value.Length > setting.MaxLength.Value)
                        {
                            model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldMaxLength],
                                setting.DisplayName, setting.MaxLength.Value)));
                            valid = false;
                        }

                        if (valid && setting.IsEmail && !String.IsNullOrEmpty(value))
                        {
                            if (!emailRegex.IsMatch(value))
                            {
                                model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsInvalidEmail],
                                    setting.DisplayName)));
                            }
                        }
                    }
                }
            }
        }

        private Dictionary<string, OrderValidationGenericProperty> GetOrderImportValidationSettings(ICollection<PropertyInfo> modelProperties)
        {
            Dictionary<string, OrderValidationGenericProperty> toReturn = new Dictionary<string, OrderValidationGenericProperty>();
            foreach (var modelProperty in modelProperties)
            {
                var displayAttribute = modelProperty.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault();
                if (displayAttribute != null)
                {
                    OrderValidationGenericProperty item = new OrderValidationGenericProperty();
                    item.DisplayName = displayAttribute.Name;
                    item.PropertyInfo = modelProperty;
                    item.PropertyType = modelProperty.PropertyType;
                    item.Get = modelProperty.GetMethod?.CompileAccessor<object, object>();
                    var requiredAttribute = modelProperty.GetCustomAttributes<RequiredAttribute>(true).FirstOrDefault();
                    if (requiredAttribute != null)
                    {
                        item.IsRequired = true;
                    }
                    var emailAddressAttribute = modelProperty.GetCustomAttributes<EmailAddressAttribute>(true).FirstOrDefault();
                    if (emailAddressAttribute != null)
                    {
                        item.IsEmail = true;
                    }
                    var maxLengthAttribute = modelProperty.GetCustomAttributes<MaxLengthAttribute>(true).FirstOrDefault();
                    if (maxLengthAttribute != null)
                    {
                        item.MaxLength = maxLengthAttribute.Length;
                    }
                    toReturn.Add(modelProperty.Name, item);
                }
            }

            return toReturn;
        }

        private void GetContryAndState(string stateCode, string countryCode, ICollection<Country> countries,
            ref List<MessageInfo> messages, out int? idState, out int? idCountry)
        {
            idState = null;
            idCountry = null;
            if (!String.IsNullOrEmpty(countryCode))
            {
                var country = countries.FirstOrDefault(p => p.CountryCode == countryCode);
                if (country != null)
                {
                    idCountry = country.Id;
                    if (!String.IsNullOrEmpty(stateCode))
                    {
                        var state = country.States.FirstOrDefault(p => p.StateCode == stateCode);
                        if (state == null)
                        {
                            messages.Add(AddErrorMessage(nameof(OrderBaseImportItem.State), String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue],
                                nameof(OrderBaseImportItem.State))));
                        }
                        else
                        {
                            idState = state.Id;
                        }
                    }
                }
                else
                {
                    messages.Add(AddErrorMessage(nameof(OrderBaseImportItem.Country), String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue],
                        nameof(OrderBaseImportItem.Country))));
                }
            }
        }

        private DateTime? ParseOrderShipDate(CsvReader reader, string columnName, ref List<MessageInfo> messages)
        {
            DateTime? toReturn = null;
            DateTime shipDate;
            var sShipDate = reader.GetField<string>(columnName);
            if (!String.IsNullOrEmpty(sShipDate))
            {
                if (DateTime.TryParse(sShipDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out shipDate))
                {
                    toReturn = TimeZoneInfo.ConvertTime(shipDate, _pstTimeZoneInfo, TimeZoneInfo.Local);
                    if (toReturn < DateTime.Now)
                    {
                        messages.Add(AddErrorMessage(columnName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.MustBeFutureDateError], columnName)));
                    }
                }
                else
                {
                    messages.Add(AddErrorMessage(columnName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ParseDateError], columnName)));
                }
            }

            return toReturn;
        }

        private ICollection<OrderSkuImportItem> ParseOrderSkus(CsvReader reader, string skuColumnName, string qtyColumnName, ref List<MessageInfo> messages)
        {
            List<OrderSkuImportItem> toReturn = new List<OrderSkuImportItem>();
            int number = 1;
            bool existInHeaders = true;
            while (existInHeaders)
            {
                existInHeaders = reader.FieldHeaders.Contains($"{skuColumnName} {number}");
                existInHeaders = existInHeaders && reader.FieldHeaders.Contains($"{qtyColumnName} {number}");

                if (existInHeaders)
                {
                    var sku = reader.GetField<string>($"{skuColumnName} {number}");
                    var sqty = reader.GetField<string>($"{qtyColumnName} {number}");

                    if (!String.IsNullOrEmpty(sku) || !String.IsNullOrEmpty(sqty))
                    {
                        int qty = 0;
                        Int32.TryParse(sqty, out qty);
                        if (qty == 0)
                        {
                            messages.Add(AddErrorMessage($"{qtyColumnName} {number}",
                                String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ParseIntError], $"{qtyColumnName} {number}")));
                        }
                        else
                        {
                            OrderSkuImportItem item = new OrderSkuImportItem();
                            item.SKU = sku;
                            item.QTY = qty;
                            toReturn.Add(item);
                        }
                    }
                }

                number++;
            }
            if (toReturn.Count == 0)
            {
                messages.Add(AddErrorMessage(null,
                            String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ZeroSkusForOrderInImport])));
            }
            return toReturn;
        }

        private ICollection<MessageInfo> FormatRowsRecordErrorMessages(IEnumerable<OrderBaseImportItem> items)
        {
            List<MessageInfo> toReturn = new List<MessageInfo>();
            foreach (var item in items)
            {
                toReturn.AddRange(item.ErrorMessages.Select(p => new MessageInfo()
                {
                    Field = p.Field,
                    Message = String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.OrderImportRowError], item.RowNumber, p.Message),
                }));
            }
            return toReturn;
        }

        private MessageInfo AddErrorMessage(string field, string message)
        {
            return new MessageInfo()
            {
                Field = field ?? "Base",
                Message = message,
            };
        }

        #endregion

        #region AffiliatesOrders

        public async Task<PagedList<AffiliateOrderListItemModel>> GetAffiliateOrderPaymentsWithCustomerInfo(AffiliateOrderPaymentFilter filter)
        {
            PagedList<AffiliateOrderListItemModel> toReturn = new PagedList<AffiliateOrderListItemModel>();

            OrderQuery conditions = new OrderQuery().WithIdAffiliate(filter.IdAffiliate).WithPaymentStatus(filter.Status).WithAffiliateOrderStatus().WithFromDate(filter.From).WithToDate(filter.To);
            Func<IQueryLite<Order>, IQueryLite<Order>> includes = (p => p.Include(o => o.PaymentMethod).ThenInclude(o => o.BillingAddress).ThenInclude(o => o.OptionValues).Include(o => o.PaymentMethod).ThenInclude(o => o.OptionValues).Include(o => o.PaymentMethod).ThenInclude(o => o.PaymentMethod).Include(o => o.AffiliateOrderPayment));

            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var result = await this.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, queryObject: conditions, orderBy: sortable, includesOverride: includes);

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
                if (order.PaymentMethod != null && order.PaymentMethod.Address != null && order.PaymentMethod.Address.DictionaryData.ContainsKey("FirstName") && order.PaymentMethod.Address.DictionaryData.ContainsKey("LastName"))
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

        public ICollection<MessageInfo> ValidateUpdateHealthwiseOrder(OrderDynamic order)
        {
            List<MessageInfo> toReturn = new List<MessageInfo>();
            if (order == null)
            {
                toReturn.Add(new MessageInfo() { Message = "Invalid order #" });
            }
            if (order == null || (order.OrderStatus == OrderStatus.Incomplete || order.OrderStatus == OrderStatus.Cancelled ||
                order.OrderStatus == OrderStatus.ShipDelayed || order.OrderStatus == OrderStatus.OnHold ||
                order.POrderStatus == OrderStatus.Incomplete || order.POrderStatus == OrderStatus.Cancelled ||
                order.POrderStatus == OrderStatus.ShipDelayed || order.POrderStatus == OrderStatus.OnHold ||
                order.NPOrderStatus == OrderStatus.Incomplete || order.NPOrderStatus == OrderStatus.Cancelled ||
                order.NPOrderStatus == OrderStatus.ShipDelayed || order.NPOrderStatus == OrderStatus.OnHold))
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
            if (messages.Count > 0)
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
                    var periods = (await _healthwisePeriodRepositoryAsync.Query(p => p.IdCustomer == order.Customer.Id && orderCreatedDate >= p.StartDate && orderCreatedDate < p.EndDate && !p.PaidDate.HasValue).Include(p => p.HealthwiseOrders).SelectAsync(false)).ToList();
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
                            new HealthwiseOrder() {Id = order.Id}
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

        public async Task<ICollection<SkuOrdered>> GetGeneratedGcs(int id)
        {
            var items =
                await
                    _orderToSkusRepository.Query(
                        s => s.IdOrder == id && (s.Sku.IdObjectType == (int) ProductType.EGс || s.Sku.IdObjectType == (int) ProductType.Gc))
                        .Include(g => g.Sku)
                        .ThenInclude(s => s.OptionValues)
                        .Include(g => g.Sku)
                        .ThenInclude(s => s.Product)
                        .ThenInclude(p => p.OptionValues)
                        .Include(s => s.GeneratedGiftCertificates)
                        .SelectAsync(false);

            return items.Select(s => new SkuOrdered
            {
                Sku = _skuMapper.FromEntity(s.Sku, true),
                GcsGenerated = s.GeneratedGiftCertificates,
                Quantity = s.Quantity,
                Amount = s.Amount
            }).ToList();
        }

        #endregion
    }
}