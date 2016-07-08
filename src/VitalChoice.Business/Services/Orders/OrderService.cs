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
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.CodeAnalysis.CSharp;
using Renci.SshNet;
using VitalChoice.Business.CsvImportMaps;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Business.Mail;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities.Healthwise;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Avatax;
using VitalChoice.Infrastructure.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Extensions;
using VitalChoice.Infrastructure.Domain.Transfer.Reports;
using AddressType = VitalChoice.Ecommerce.Domain.Entities.Addresses.AddressType;

namespace VitalChoice.Business.Services.Orders
{
    public class OrderService : ExtendedEcommerceDynamicService<OrderDynamic, Order, OrderOptionType, OrderOptionValue>,
        IOrderService
    {
        //private readonly IEcommerceRepositoryAsync<VOrder> _vOrderRepository;
        private readonly IEcommerceRepositoryAsync<VOrderWithRegionInfoItem> _vOrderWithRegionInfoItemRepository;
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        //private readonly IEcommerceRepositoryAsync<Sku> _skusRepository;
        private readonly ProductMapper _productMapper;
        private readonly SkuMapper _skuMapper;
        private readonly CustomerMapper _customerMapper;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowFactory _treeFactory;
        private readonly IEcommerceRepositoryAsync<VCustomer> _vCustomerRepositoryAsync;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly IEncryptedOrderExportService _encryptedOrderExportService;
        private readonly SpEcommerceRepository _sPEcommerceRepository;
        private readonly IPaymentMethodService _paymentMethodService;
        //private readonly IObjectMapper<OrderPaymentMethodDynamic> _paymentMapper;
        private readonly IEcommerceRepositoryAsync<OrderToGiftCertificate> _orderToGiftCertificateRepositoryAsync;
        private readonly ICountryService _countryService;
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
        private readonly OrderRepository _orderRepository;

        public OrderService(
            //IEcommerceRepositoryAsync<VOrder> vOrderRepository,
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
            ILoggerProviderExtended loggerProvider, //IEcommerceRepositoryAsync<Sku> skusRepository,
            IEcommerceRepositoryAsync<VCustomer> vCustomerRepositoryAsync,
            DynamicExtensionsRewriter queryVisitor,
            IAppInfrastructureService appInfrastructureService,
            IEncryptedOrderExportService encryptedOrderExportService,
            SpEcommerceRepository sPEcommerceRepository,
            IPaymentMethodService paymentMethodService,
            //IObjectMapper<OrderPaymentMethodDynamic> paymentMapper,
            IEcommerceRepositoryAsync<OrderToGiftCertificate> orderToGiftCertificateRepositoryAsync,
            IExtendedDynamicServiceAsync
                <OrderPaymentMethodDynamic, OrderPaymentMethod, CustomerPaymentMethodOptionType, OrderPaymentMethodOptionValue>
                paymentGenericService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            IProductService productService,
            INotificationService notificationService,
            ICountryService countryService, ITransactionAccessor<EcommerceContext> transactionAccessor, SkuMapper skuMapper,
            IEcommerceRepositoryAsync<OrderToSku> orderToSkusRepository, IDiscountService discountService,
            IEcommerceRepositoryAsync<VAutoShip> vAutoShipRepository, IEcommerceRepositoryAsync<VAutoShipOrder> vAutoShipOrderRepository,
            AffiliateOrderPaymentRepository affiliateOrderPaymentRepository, ICountryNameCodeResolver codeResolver, IDynamicEntityOrderingExtension<Order> orderingExtension)
            : base(
                mapper, orderRepository, orderValueRepositoryAsync,
                bigStringValueRepository, objectLogItemExternalService, loggerProvider, queryVisitor, transactionAccessor, orderingExtension)
        {
            //_vOrderRepository = vOrderRepository;
            _vOrderWithRegionInfoItemRepository = vOrderWithRegionInfoItemRepository;
            _adminProfileRepository = adminProfileRepository;
            _productMapper = productMapper;
            _customerService = customerService;
            _treeFactory = treeFactory;
            //_skusRepository = skusRepository;
            _vCustomerRepositoryAsync = vCustomerRepositoryAsync;
            _appInfrastructureService = appInfrastructureService;
            _encryptedOrderExportService = encryptedOrderExportService;
            _sPEcommerceRepository = sPEcommerceRepository;
            _paymentMethodService = paymentMethodService;
            //_paymentMapper = paymentMapper;
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
            _orderRepository = orderRepository;
            _addressMapper = addressMapper;
            _productService = productService;
            _notificationService = notificationService;
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
                    await EnsurePaymentMethod(model);
                    var authTask = _paymentMethodService.AuthorizeCreditCard(model.PaymentMethod);
                    var paymentCopy = new OrderCardData
                    {
                        CardNumber = model.PaymentMethod.SafeData.CardNumber,
                        IdCustomerPaymentMethod = model.PaymentMethod.IdCustomerPaymentMethod
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
                            IdCustomerPaymentMethod = p.PaymentMethod.IdCustomerPaymentMethod
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
                    await EnsurePaymentMethod(model);
                    var authTask = _paymentMethodService.AuthorizeCreditCard(model.PaymentMethod);
                    var paymentCopy = new OrderCardData
                    {
                        CardNumber = model.PaymentMethod.SafeData.CardNumber,
                        IdOrder = model.Id,
                        IdCustomerPaymentMethod = model.PaymentMethod.IdCustomerPaymentMethod
                    };
                    (await authTask).Raise();
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

            var autoShip = await _orderRepository.Query(orderQuery).SelectFirstOrDefaultAsync();
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
                //var invalidSkuOrdered =
                //    entities.SelectMany(o => o.Skus)
                //        .Where(s => s.Sku?.Product == null || s.Sku.OptionTypes == null)
                //        .ToArray();
                //var skuIds = new HashSet<int>(invalidSkuOrdered.Select(s => s.IdSku));
                //var invalidSkus = (await _skusRepository.Query(p => skuIds.Contains(p.Id))
                //    .Include(s => s.OptionValues)
                //    .Include(s => s.Product)
                //    .ThenInclude(p => p.OptionValues)
                //    .Include(s => s.Product)
                //    .ThenInclude(p => p.ProductsToCategories)
                //    .SelectAsync(false)).ToDictionary(s => s.Id);
                //foreach (var orderToSku in invalidSkuOrdered)
                //{
                //    Sku sku;
                //    if (invalidSkus.TryGetValue(orderToSku.IdSku, out sku))
                //    {
                //        var optionTypes = _productMapper.FilterByType(sku.Product.IdObjectType);
                //        orderToSku.Sku = sku;
                //        orderToSku.Sku.Product = sku.Product;
                //        orderToSku.Sku.OptionTypes = optionTypes;
                //        orderToSku.Sku.Product.OptionTypes = optionTypes;
                //    }
                //}
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
                //var invalidSkuOrdered =
                //    entities.SelectMany(o => o.Skus)
                //        .Where(s => s.Sku?.Product == null || s.Sku.OptionTypes == null)
                //        .ToArray();
                //var skuIds = new HashSet<int>(invalidSkuOrdered.Select(s => s.IdSku));
                //var invalidSkus = (await _skusRepository.Query(p => skuIds.Contains(p.Id))
                //    .Include(s => s.OptionValues)
                //    .Include(s => s.Product)
                //    .ThenInclude(p => p.OptionValues)
                //    .Include(s => s.Product)
                //    .ThenInclude(p => p.ProductsToCategories)
                //    .SelectAsync(false)).ToDictionary(s => s.Id);
                //foreach (var orderToSku in invalidSkuOrdered)
                //{
                //    Sku sku;
                //    if (invalidSkus.TryGetValue(orderToSku.IdSku, out sku))
                //    {
                //        var optionTypes = _productMapper.FilterByType(sku.Product.IdObjectType);
                //        orderToSku.Sku = sku;
                //        orderToSku.Sku.Product = sku.Product;
                //        orderToSku.Sku.OptionTypes = optionTypes;
                //        orderToSku.Sku.Product.OptionTypes = optionTypes;
                //    }
                //}
            }
            return TaskCache.CompletedTask;
        }

        protected override async Task AfterEntityChangesAsync(OrderDynamic model, Order updated, IUnitOfWorkAsync uow)
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
                Order = order
            };
            var tree = await _treeFactory.CreateTreeAsync<OrderDataContext, decimal>("Order");
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

        protected override Task<List<MessageInfo>> ValidateAsync(OrderDynamic dynamic)
        {
            if (dynamic.Customer.StatusCode == (int) CustomerStatus.Suspended)
            {
                throw new CustomerSuspendException();
            }
            return base.ValidateAsync(dynamic);
        }

        public async Task<int?> GetOrderIdCustomer(int id)
        {
            var order = (await this._orderRepository.Query(p => p.StatusCode != (int) RecordStatusCode.Deleted && p.Id == id).SelectAsync(false)).FirstOrDefault();
            return order?.IdCustomer;
        }

        public async Task<PagedList<Order>> GetShortOrdersAsync(OrderFilter filter)
        {
            Func<IQueryable<Order>, IOrderedQueryable<Order>> sortable = PerformOrderSorting(filter);

            var orderQuery = new OrderQuery().WithCustomerId(filter.IdCustomer).FilterById(filter.Id).NotDeleted().WithOrderType(filter.OrderType).NotAutoShip();

            var query = this._orderRepository.Query(orderQuery).OrderBy(sortable);
            return await query.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
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

            var frequencyAvailable = _appInfrastructureService.Data().AutoShipOptions.Select(x => x.Key).ToList();

            var toProcess = new List<int>();
            foreach (var frequency in frequencyAvailable)
            {
                var tempDate = currentDate.AddDays(-frequency); //AddMonths(-frequency);

                var vAutoShips = await _vAutoShipRepository.Query(x => x.AutoShipFrequency == frequency && x.LastAutoShipDate.HasValue && x.LastAutoShipDate.Value.Day <= tempDate.Day && x.LastAutoShipDate.Value.Year <= tempDate.Year && x.LastAutoShipDate.Value.Month <= tempDate.Month).SelectAsync(x => x.Id);

                if (vAutoShips.Count > 0)
                {
                    toProcess.AddRange(vAutoShips);
                }
            }

            //skipped by some reason
            var skippedAcidently = await _vAutoShipRepository.Query(x => !x.LastAutoShipDate.HasValue).SelectAsync(x => x.Id);

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
                            standardOrder.IdObjectType = (int) OrderType.AutoShipOrder;
                            standardOrder.Data.AutoShipFrequency = null;
                            standardOrder.Data.LastAutoShipDate = null;
                            standardOrder.Id = 0;
                            standardOrder.PaymentMethod.Id = 0;
                            standardOrder.ShippingAddress.Id = 0;
                            standardOrder.Data.AutoShipId = autoShip.Id;

                            var order = await InsertAsyncInternal(standardOrder, uow);

                            transaction.Commit();

                            Logger.LogInfo(i => $"AutoShip {i} handled successfully", autoShip.Id);

                            success = true;

                            var entity = await SelectEntityFirstAsync(o => o.Id == order.Id);
                            await LogItemChanges(new[] {await DynamicMapper.FromEntityAsync(entity)});
                        }
                        catch (Exception e)
                        {
                            Logger.LogError($"AutoShip {autoShip.Id} skipped due to error ocurred. Error: {e.Message}", e);
                            transaction.Rollback();

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
            return await _vAutoShipOrderRepository.Query(x => x.AutoShipId == idAutoShip).SelectAsync(x => x.Id);
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
                                generatedGcs = await giftCertificateRepository.Query(p => p.IdOrder == order.Id && p.StatusCode != RecordStatusCode.NotActive).SelectAsync();
                                //cancel gc=3 with np part
                                if (pOrderType == POrderType.NP)
                                {
                                    generatedGcs.Where(p => p.GCType == GCType.GC).ForEach(p => { p.StatusCode = RecordStatusCode.NotActive; });
                                }
                                //cancel all gcs with all
                                if (IsAllCancel(pOrderType, order))
                                {
                                    generatedGcs.ForEach(p => { p.StatusCode = RecordStatusCode.NotActive; });
                                }
                            }

                            List<GiftCertificate> usedGcs = new List<GiftCertificate>();
                            if (order.GiftCertificates?.Count > 0)
                            {
                                var ids = order.GiftCertificates.Select(p => p.GiftCertificate?.Id).ToList();
                                usedGcs = await giftCertificateRepository.Query(p => ids.Contains(p.Id)).SelectAsync(true);

                                if (IsAllCancel(pOrderType, order))
                                {
                                    usedGcs.UpdateKeyed(order.GiftCertificates, g => g.Id, g => g.GiftCertificate.Id, (db, gc) => db.Balance += gc.Amount);

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
                        if (anyNotIncomplete)
                        {
                            model.Data.LastAutoShipDate = DateTime.Now;
                        }

                        res = await InsertAsyncInternal(model, uow);

                        if (anyNotIncomplete)
                        {
                            model.IdObjectType = (int) OrderType.AutoShipOrder;
                            model.Data.AutoShipFrequency = null;
                            model.Data.LastAutoShipDate = null;
                            model.Data.AutoShipId = res.Id;
                            model.Id = 0;
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
            if (model.IsAnyNotIncomplete())
            {
                var gcsRep = uow.RepositoryAsync<GiftCertificate>();
                var gcs = model.GiftCertificates.Select(g => g.GiftCertificate.Id).Distinct().ToList();
                var gcsInDb = await gcsRep.Query(g => gcs.Contains(g.Id)).SelectAsync(true);
                gcsInDb.UpdateKeyed(model.GiftCertificates.Select(g => g.GiftCertificate), g => g.Id, (gcDb, gc) => gcDb.Balance = gc.Balance);
            }
        }

        private async Task EnsurePaymentMethod(OrderDynamic model)
        {
            if (model.PaymentMethod == null)
            {
                switch ((OrderType) model.IdObjectType)
                {
                    case OrderType.Normal:
                    case OrderType.AutoShipOrder:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int) PaymentMethodType.CreditCard);
                        break;
                    case OrderType.AutoShip:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int) PaymentMethodType.CreditCard);
                        break;
                    case OrderType.DropShip:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int) PaymentMethodType.NoCharge);
                        break;
                    case OrderType.GiftList:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int) PaymentMethodType.NoCharge);
                        break;
                    case OrderType.Reship:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int) PaymentMethodType.CreditCard);
                        break;
                    case OrderType.Refund:
                        model.PaymentMethod = await _paymentGenericService.Mapper.CreatePrototypeAsync((int) PaymentMethodType.CreditCard);
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
                                model.IdObjectType = (int) OrderType.AutoShipOrder;
                                model.Data.AutoShipFrequency = null;
                                model.Data.LastAutoShipDate = null;
                                model.Data.AutoShipId = model.Id;
                                model.ShippingAddress.Id = 0;
                                model.Id = 0;
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

                            model.IdObjectType = (int) OrderType.AutoShipOrder;
                            model.Data.AutoShipFrequency = null;
                            model.Data.LastAutoShipDate = null;
                            model.Data.AutoShipId = res.Id;
                            model.Id = 0;
                            model.PaymentMethod.Id = 0;
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

                                    model.IdObjectType = (int) OrderType.AutoShipOrder;
                                    model.Data.AutoShipFrequency = null;
                                    model.Data.LastAutoShipDate = null;
                                    model.Data.AutoShipId = model.Id;
                                    model.Id = 0;
                                    model.PaymentMethod.Id = 0;
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
                .WithCustomerType(filter.IdCustomerType)
                .WithoutIncomplete(filter.OrderStatus, filter.IgnoreNotShowingIncomplete)
                .WithIdSku(filter.IdSku)
                .WithShipState(filter.IdShipState)
                .WithOrderDynamicValues(filter.IdOrderSource, filter.POrderType, filter.IdShippingMethod)
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
            }

            var orders = await SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount, conditions, includes => includes.Include(c => c.OptionValues).Include(c => c.PaymentMethod).Include(c => c.ShippingAddress).ThenInclude(c => c.OptionValues).Include(c => c.Customer).ThenInclude(p => p.ProfileAddress).ThenInclude(c => c.OptionValues).Include(c => c.OrderShippingPackages), orderBy: sortable, withDefaults: true);

            var resultList = new List<OrderInfoItem>(orders.Items.Count);
            var shippingMethods = _appInfrastructureService.Data().OrderPreferredShipMethod;
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
                    DateEdited = item.DateEdited,
                    IdCustomerType = item.Customer.IdObjectType,
                    IdCustomer = item.Customer.Id,
                    Company = item.Customer?.ProfileAddress.SafeData.Company,
                    Customer = item.Customer?.ProfileAddress.SafeData.FirstName + " " + item.Customer?.ProfileAddress.SafeData.LastName,
                    StateCode = _codeResolver.GetStateCode(item.ShippingAddress?.IdCountry ?? 0, item.ShippingAddress?.IdState ?? 0),
                    ShipTo = item.ShippingAddress?.SafeData.FirstName + " " + item.ShippingAddress?.SafeData.LastName,
                    PreferredShipMethod = item.ShippingAddress?.SafeData.PreferredShipMethod
                };
                await DynamicMapper.UpdateModelAsync(newItem, item);
                resultList.Add(newItem);
            }

            PagedList<OrderInfoItem> toReturn = new PagedList<OrderInfoItem>
            {
                Items = resultList, Count = orders.Count
            };

            if (toReturn.Items.Count > 0)
            {
                var ids = new HashSet<int>(toReturn.Items.Where(p => p.IdEditedBy.HasValue).Select(p => p.IdEditedBy.Value));
                var profiles = await _adminProfileRepository.Query(p => ids.Contains(p.Id)).SelectAsync(false);
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
            var toReturn = await Mapper.CreatePrototypeAsync((int) OrderType.Normal);

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

        #region OrdersImport

        public async Task<bool> ImportOrders(byte[] file, string fileName, OrderType orderType, int idCustomer, int? idPaymentMethod, int idAddedBy)
        {
            var customer = await _customerService.SelectAsync(idCustomer);
            if (customer == null)
            {
                throw new AppValidationException("Invalid file format");
            }
            if (!customer.ApprovedPaymentMethods.Contains((int) PaymentMethodType.NoCharge))
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

            var map = await OrdersForImportBaseConvert(records, orderType, customer, idAddedBy);

            await LoadSkusDynamic(map, customer);
            //not found SKU errors
            messages = FormatRowsRecordErrorMessages(records);
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            foreach (var item in map)
            {
                var orderCombinedStatus = item.Order.OrderStatus ?? OrderStatus.Processed;
                item.Order.Data.ShipDelayType = item.Order.SafeData.ShipDelayDate != null ? ShipDelayType.EntireOrder : ShipDelayType.None;

                var context = await CalculateOrder(item.Order, orderCombinedStatus);

                item.OrderImportItem.ErrorMessages.AddRange(context.Messages);
                item.OrderImportItem.ErrorMessages.AddRange(context.SkuOrdereds.Where(p => p.Messages != null).SelectMany(p => p.Messages));
                item.OrderImportItem.ErrorMessages.AddRange(context.PromoSkus.Where(p => p.Enabled && p.Messages != null).SelectMany(p => p.Messages));
            }

            //throw calculating errors
            messages = FormatRowsRecordErrorMessages(map.Select(p => p.OrderImportItem));
            if (messages.Count > 0)
            {
                throw new AppValidationException(messages);
            }

            var orders = map.Select(p => p.Order).ToList();
            orders = await InsertRangeAsync(orders);

            if (orderType == OrderType.GiftList && idPaymentMethod.HasValue)
            {
                await SendGLOrdersImportEmailAsync(orders, customer, idPaymentMethod.Value, idAddedBy);
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
                            item.OrderImportItem.ErrorMessages.Add(AddErrorMessage("SKU " + index, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.SkuNotFoundOrderImport], "SKU " + index, sku.Sku.Code)));
                            continue;
                        }
                        else
                        {
                            sku.Sku.Product = dbSku.Sku.Product;
                            sku.Sku = dbSku.Sku;
                            if (sku.Sku != null)
                            {
                                sku.Amount = customer.IdObjectType == (int) CustomerType.Retail ? sku.Sku.Price : customer.IdObjectType == (int) CustomerType.Wholesale ? sku.Sku.WholesalePrice : 0;
                            }
                        }

                        index++;
                    }
                }
            }
        }

        private async Task<List<OrderImportItemOrderDynamic>> OrdersForImportBaseConvert(List<OrderBaseImportItem> records, OrderType orderType, CustomerDynamic customer, int idAddedBy)
        {
            List<OrderImportItemOrderDynamic> toReturn = new List<OrderImportItemOrderDynamic>();
            foreach (var record in records)
            {
                var order = Mapper.CreatePrototype((int) orderType);
                order.IdEditedBy = idAddedBy;
                order.Customer = customer;
                order.ShippingAddress = await _addressMapper.FromModelAsync(record, (int) AddressType.Shipping);
                record.SetFields(order);
                toReturn.Add(new OrderImportItemOrderDynamic
                {
                    OrderImportItem = record, Order = order,
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
            var qtyBaseHeader = qtyProperty?.GetCustomAttributes<DisplayAttribute>(true).FirstOrDefault()?.Name; // ReSharper disable all

            int rowNumber = 1;
            try
            {
                while (reader.Read())
                {
                    OrderBaseImportItem item = (OrderBaseImportItem) reader.GetRecord(orderImportItemType);
                    item.RowNumber = rowNumber;
                    var messages = new List<MessageInfo>();
                    rowNumber++;

                    if (orderImportItemType == typeof(OrderGiftListImportItem))
                    {
                        ((OrderGiftListImportItem) item).ShipDelayDate = ParseOrderShipDate(reader, shipDateHeader, ref messages);
                    }
                    if (orderImportItemType == typeof(OrderDropShipImportItem))
                    {
                        ((OrderDropShipImportItem) item).ShipDelayDate = ParseOrderShipDate(reader, shipDateHeader, ref messages);
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
            catch (Exception)
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
                        string value = (string) setting.Get(model);
                        if (setting.IsRequired && String.IsNullOrEmpty(value))
                        {
                            model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsRequired], setting.DisplayName)));
                            valid = false;
                        }

                        if (valid && setting.MaxLength.HasValue && !String.IsNullOrEmpty(value) && value.Length > setting.MaxLength.Value)
                        {
                            model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldMaxLength], setting.DisplayName, setting.MaxLength.Value)));
                            valid = false;
                        }

                        if (valid && setting.IsEmail && !String.IsNullOrEmpty(value))
                        {
                            if (!emailRegex.IsMatch(value))
                            {
                                model.ErrorMessages.Add(AddErrorMessage(setting.DisplayName, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.FieldIsInvalidEmail], setting.DisplayName)));
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

        private void GetContryAndState(string stateCode, string countryCode, ICollection<Country> countries, ref List<MessageInfo> messages, out int? idState, out int? idCountry)
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
                            messages.Add(AddErrorMessage(nameof(OrderBaseImportItem.State), String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], nameof(OrderBaseImportItem.State))));
                        }
                        else
                        {
                            idState = state.Id;
                        }
                    }
                }
                else
                {
                    messages.Add(AddErrorMessage(nameof(OrderBaseImportItem.Country), String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.InvalidFieldValue], nameof(OrderBaseImportItem.Country))));
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
                    toReturn = TimeZoneInfo.ConvertTime(shipDate, TimeZoneHelper.PstTimeZoneInfo, TimeZoneInfo.Local);
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
                            messages.Add(AddErrorMessage($"{qtyColumnName} {number}", String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ParseIntError], $"{qtyColumnName} {number}")));
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
                messages.Add(AddErrorMessage(null, String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.ZeroSkusForOrderInImport])));
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
                    Field = p.Field, Message = String.Format(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.OrderImportRowError], item.RowNumber, p.Message),
                }));
            }
            return toReturn;
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
                await UpdateHealthwiseOrderInnerAsync(uow, order.Id, order.Customer.Id, order.DateCreated, isHealthwise, order.IsFirstHealthwise);
                if (isHealthwise)
                {
                    await MarkHealthwiseCustomerAsync(uow, order.Customer.Id);
                }
                await uow.SaveChangesAsync();
                return true;
            }
        }

        public async Task MarkHealthwiseCustomerAsync(IUnitOfWorkAsync uow, int idCustomer)
        {
            var option = _customerMapper.OptionTypes.FirstOrDefault(p => p.Name == "HasHealthwiseOrders");
            if (option != null)
            {
                var customerOptionValueRepositoryAsync = uow.RepositoryAsync<CustomerOptionValue>();
                var optionValue = (await customerOptionValueRepositoryAsync.Query(p => p.IdCustomer == idCustomer && p.IdOptionType == option.Id).SelectAsync(true)).FirstOrDefault();
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
            //model.IsHealthwise = true;
            await UpdateHealthwiseOrderInnerAsync(uow, model.Id, model.Customer.Id, DateTime.Now, (bool?) model.SafeData.IsHealthwise ?? false, model.IsFirstHealthwise);
        }

        private async Task UpdateHealthwiseOrderInnerAsync(IUnitOfWorkAsync uow, int idOrder, int idCustomer, DateTime orderDateCreated, bool isHealthwise, bool isFirstHealthwise)
        {
            var healthwiseOrderRepositoryAsync = uow.RepositoryAsync<HealthwiseOrder>();
            var healthwisePeriodRepositoryAsync = uow.RepositoryAsync<HealthwisePeriod>();
            if (!isHealthwise)
            {
                healthwiseOrderRepositoryAsync.Delete(idOrder);
            }
            else
            {
                HealthwiseOrder healthwiseOrder = (await healthwiseOrderRepositoryAsync.Query(p => p.Id == idOrder).SelectAsync(false)).FirstOrDefault();
                if (healthwiseOrder == null)
                {
                    //BUG: doesn't make any sense to delete not exists HW order
                    //using (var transaction = uow.BeginTransaction())
                    //{
                    //    try
                    //    {
                    //        healthwiseOrderRepositoryAsync.Delete(idOrder);
                    //        if (isFirstHealthwise)
                    //        {
                    //            var customer = await _customerService.SelectAsync(idCustomer, true);
                    //            customer.Data.HasHealthwiseOrders = false;
                    //            await _customerService.UpdateAsync(customer);
                    //        }
                    //        transaction.Commit();
                    //    }
                    //    catch
                    //    {
                    //        transaction.Rollback();
                    //        throw;
                    //    }
                    //}
                    var maxCount = _appInfrastructureService.Data().AppSettings.HealthwisePeriodMaxItemsCount;
                    var orderCreatedDate = orderDateCreated;
                    var periods = (await healthwisePeriodRepositoryAsync.Query(p => p.IdCustomer == idCustomer && orderCreatedDate >= p.StartDate && orderCreatedDate < p.EndDate && !p.PaidDate.HasValue).Include(p => p.HealthwiseOrders).ThenInclude(p => p.Order).SelectAsync(false)).ToList();
                    foreach (var healthwisePeriod in periods)
                    {
                        healthwisePeriod.HealthwiseOrders = healthwisePeriod.HealthwiseOrders.Where(p => p.Order.OrderStatus == OrderStatus.Processed || p.Order.OrderStatus == OrderStatus.Exported || p.Order.OrderStatus == OrderStatus.Shipped || p.Order.POrderStatus == OrderStatus.Processed || p.Order.POrderStatus == OrderStatus.Exported || p.Order.POrderStatus == OrderStatus.Shipped || p.Order.NPOrderStatus == OrderStatus.Processed || p.Order.NPOrderStatus == OrderStatus.Exported || p.Order.NPOrderStatus == OrderStatus.Shipped).ToList();
                    }
                    bool addedToPeriod = false;
                    foreach (var period in periods.OrderBy(p => p.StartDate))
                    {
                        if (period.HealthwiseOrders.Count < maxCount)
                        {
                            healthwiseOrder = new HealthwiseOrder()
                            {
                                Id = idOrder, IdHealthwisePeriod = period.Id
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
                        using (var transaction = uow.BeginTransaction())
                        {
                            try
                            {
                                healthwisePeriodRepositoryAsync.InsertGraph(period);
                                if (isFirstHealthwise)
                                {
                                    var customer = await _customerService.SelectAsync(idCustomer, true);
                                    customer.Data.HasHealthwiseOrders = true;
                                    await _customerService.UpdateAsync(customer);
                                }
                                transaction.Commit();
                            }
                            catch
                            {
                                transaction.Rollback();
                                throw;
                            }
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
            var items = await _orderToSkusRepository.Query(s => s.IdOrder == id && (s.Sku.IdObjectType == (int) ProductType.EGс || s.Sku.IdObjectType == (int) ProductType.Gc)).Include(g => g.Sku).ThenInclude(s => s.OptionValues).Include(g => g.Sku).ThenInclude(s => s.Product).ThenInclude(p => p.OptionValues).Include(s => s.GeneratedGiftCertificates).SelectAsync(false);

            return items.Select(s => new SkuOrdered
            {
                Sku = _skuMapper.FromEntity(s.Sku, true), GcsGenerated = s.GeneratedGiftCertificates, Quantity = s.Quantity, Amount = s.Amount
            }).ToList();
        }

        #endregion

        private struct OrderPaymentReference
        {
            public OrderDynamic OriginalReference { get; set; }
            public OrderCardData PaymentMethod { get; set; }
        }
    }
}