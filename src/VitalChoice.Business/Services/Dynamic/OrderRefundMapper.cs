using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.InventorySkus;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderRefundMapper : DynamicMapper<OrderRefundDynamic, Order, OrderOptionType, OrderOptionValue>
    {
        private readonly SkuMapper _skuMapper;
        private readonly IEcommerceRepositoryAsync<OrderToGiftCertificate> _orderToGiftCertificateRepository;
        private readonly OrderAddressMapper _orderAddressMapper;
        private readonly OrderPaymentMethodMapper _orderPaymentMethodMapper;
        private readonly IOrderService _orderService;

        public OrderRefundMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<OrderOptionType> orderRepositoryAsync, OrderAddressMapper orderAddressMapper,
            OrderPaymentMethodMapper orderPaymentMethodMapper, 
            SkuMapper skuMapper,
            IEcommerceRepositoryAsync<OrderToGiftCertificate> orderToGiftCertificateRepository, IOrderService orderService)
            : base(converter, converterService, orderRepositoryAsync)
        {
            _skuMapper = skuMapper;
            _orderAddressMapper = orderAddressMapper;
            _orderPaymentMethodMapper = orderPaymentMethodMapper;
            _orderToGiftCertificateRepository = orderToGiftCertificateRepository;
            _orderService = orderService;
        }

        public override Expression<Func<OrderOptionValue, int>> ObjectIdSelector => o => o.IdOrder;

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderRefundDynamic, Order>> items, bool withDefaults = false)
        {
            return items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.IdAddedBy = entity.IdAddedBy;
                dynamic.DiscountTotal = entity.DiscountTotal;
                dynamic.OrderStatus = entity.OrderStatus;
                dynamic.ProductsSubtotal = entity.ProductsSubtotal;
                dynamic.ShippingTotal = entity.ShippingTotal;
                dynamic.TaxTotal = entity.TaxTotal;
                dynamic.Total = entity.Total;
                if (!entity.IdOrderSource.HasValue)
                {
                    throw new ApiException("Cannot get original order id");
                }
                dynamic.IdOrderSource = entity.IdOrderSource.Value;

                dynamic.Customer = new CustomerDynamic() { Id = entity.IdCustomer };

                dynamic.ShippingAddress =
                    await _orderAddressMapper.FromEntityAsync(entity.ShippingAddress, withDefaults);

                dynamic.PaymentMethod =
                    await _orderPaymentMethodMapper.FromEntityAsync(entity.PaymentMethod, withDefaults);

                if (entity.RefundSkus != null)
                {
                    await dynamic.RefundSkus.AddRangeAsync(entity.RefundSkus.Select(async s => new RefundSkuOrdered
                    {
                        Redeem = s.Redeem,
                        Quantity = s.Quantity,
                        RefundValue = s.RefundValue,
                        RefundPrice = s.RefundPrice,
                        RefundPercent = s.RefundPercent,
                        Sku = await _skuMapper.FromEntityAsync(s.Sku, withDefaults),
                    }));
                }

                if (entity.RefundOrderToGiftCertificates != null)
                {
                    dynamic.RefundOrderToGiftCertificates.AddRange(entity.RefundOrderToGiftCertificates.Select(
                        s => new RefundOrderToGiftCertificateUsed
                        {
                            IdOrder = s.IdOrder,
                            IdGiftCertificate = s.IdGiftCertificate,
                            Amount = s.Amount,
                            AmountUsedOnSourceOrder = s.OrderToGiftCertificate?.Amount ?? 0,
                            Code = s.OrderToGiftCertificate?.GiftCertificate?.Code,
                        }));
                }
                dynamic.OriginalOrder = await _orderService.SelectAsync(entity.IdOrderSource ?? -1, true);
            });
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderRefundDynamic, Order>> items)
        {
            return items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdAddedBy = entity.IdEditedBy;
                entity.DiscountTotal = dynamic.DiscountTotal;
                entity.OrderStatus = dynamic.OrderStatus;
                entity.ProductsSubtotal = dynamic.ProductsSubtotal;
                entity.ShippingTotal = dynamic.ShippingTotal;
                entity.TaxTotal = dynamic.TaxTotal;
                entity.Total = dynamic.Total;
                entity.IdOrderSource = dynamic.IdOrderSource;

                entity.ShippingAddress =
                    await _orderAddressMapper.ToEntityAsync(dynamic.ShippingAddress);
                entity.IdCustomer = dynamic.Customer.Id;
                entity.PaymentMethod =
                    await _orderPaymentMethodMapper.ToEntityAsync(dynamic.PaymentMethod);

                entity.RefundSkus = new List<RefundSku>(dynamic.RefundSkus.Select(s => new RefundSku
                {
                    Redeem = s.Redeem,
                    Quantity = s.Quantity,
                    RefundValue = s.RefundValue,
                    RefundPrice = s.RefundPrice,
                    RefundPercent = s.RefundPercent,
                    IdOrder = dynamic.Id,
                    IdSku = s.Sku.Id,
                }));

                entity.RefundOrderToGiftCertificates = new List<RefundOrderToGiftCertificate>();
                await entity.RefundOrderToGiftCertificates.AddRangeAsync(
                    dynamic.RefundOrderToGiftCertificates.
                        Where(p => p.Amount > 0).Select(async s => new RefundOrderToGiftCertificate
                        {
                            IdOrder = s.IdOrder,
                            IdGiftCertificate = s.IdGiftCertificate,
                            Amount = s.Amount,
                            OrderToGiftCertificate = await _orderToGiftCertificateRepository.Query(p => p.IdOrder == s.IdOrder &&
                                                                                                        p.IdGiftCertificate ==
                                                                                                        s.IdGiftCertificate)
                                .Include(p => p.GiftCertificate)
                                .SelectFirstOrDefaultAsync(true),
                        }));
                foreach (var refundOrderToGiftCertificate in entity.RefundOrderToGiftCertificates)
                {
                    if (refundOrderToGiftCertificate?.OrderToGiftCertificate?.GiftCertificate != null)
                    {
                        refundOrderToGiftCertificate.OrderToGiftCertificate.GiftCertificate.Balance +=
                            refundOrderToGiftCertificate.Amount;
                    }
                    if (refundOrderToGiftCertificate != null)
                        refundOrderToGiftCertificate.OrderToGiftCertificate = null;
                }
            });
        }

        protected override Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderRefundDynamic, Order>> items)
        {
            return items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdAddedBy = entity.IdAddedBy;
                entity.DiscountTotal = dynamic.DiscountTotal;
                entity.OrderStatus = dynamic.OrderStatus;
                entity.ProductsSubtotal = dynamic.ProductsSubtotal;
                entity.ShippingTotal = dynamic.ShippingTotal;
                entity.TaxTotal = dynamic.TaxTotal;
                entity.Total = dynamic.Total;

                await _orderAddressMapper.UpdateEntityAsync(dynamic.ShippingAddress, entity.ShippingAddress);

                entity.IdCustomer = dynamic.Customer.Id;

                if (dynamic.PaymentMethod.Address != null && entity.PaymentMethod.BillingAddress == null)
                {
                    entity.PaymentMethod.BillingAddress = new OrderAddress { OptionValues = new List<OrderAddressOptionValue>() };
                }
                await _orderPaymentMethodMapper.UpdateEntityAsync(dynamic.PaymentMethod, entity.PaymentMethod);

                if (entity.RefundSkus == null)
                {
                    entity.RefundSkus = new List<RefundSku>();
                }
                entity.RefundSkus.MergeKeyed(dynamic.RefundSkus, sku => sku.IdSku, ordered => ordered.Sku.Id,
                    s => new RefundSku
                    {
                        Redeem = s.Redeem,
                        Quantity = s.Quantity,
                        RefundValue = s.RefundValue,
                        RefundPrice = s.RefundPrice,
                        RefundPercent = s.RefundPercent,
                        IdOrder = dynamic.Id,
                        IdSku = s.Sku.Id,
                    }, (sku, ordered) =>
                    {
                        sku.Redeem = ordered.Redeem;
                        sku.Quantity = ordered.Quantity;
                        sku.RefundValue = ordered.RefundValue;
                        sku.RefundPrice = ordered.RefundPrice;
                        sku.RefundPercent = ordered.RefundPercent;
                    });
                
                //new
                foreach (var refundOrderToGiftCertificate in dynamic.RefundOrderToGiftCertificates)
                {
                    var existItem = entity.RefundOrderToGiftCertificates.FirstOrDefault(pp => pp.IdOrder == refundOrderToGiftCertificate.IdOrder &&
                                        pp.IdGiftCertificate == refundOrderToGiftCertificate.IdGiftCertificate);
                    if(existItem==null)
                    { 
                        var newItem = new RefundOrderToGiftCertificate();
                        newItem.IdOrder = refundOrderToGiftCertificate.IdOrder;
                        newItem.IdGiftCertificate = refundOrderToGiftCertificate.IdGiftCertificate;
                        newItem.Amount = refundOrderToGiftCertificate.Amount;
                        newItem.OrderToGiftCertificate =
                            await
                                _orderToGiftCertificateRepository.Query(
                                    p =>
                                        p.IdOrder == refundOrderToGiftCertificate.IdOrder &&
                                        p.IdGiftCertificate == refundOrderToGiftCertificate.IdGiftCertificate)
                                    .Include(p => p.GiftCertificate)
                                    .SelectFirstOrDefaultAsync(true);
                        if (newItem.OrderToGiftCertificate?.GiftCertificate != null)
                        {
                            newItem.OrderToGiftCertificate.GiftCertificate.Balance += newItem.Amount;
                        }
                        entity.RefundOrderToGiftCertificates.Add(newItem);
                    }
                    else
                    {
                        var diff = existItem.Amount - refundOrderToGiftCertificate.Amount;
                        existItem.Amount = refundOrderToGiftCertificate.Amount;
                        if (existItem.OrderToGiftCertificate?.GiftCertificate != null)
                        {
                            existItem.OrderToGiftCertificate.GiftCertificate.Balance = existItem.OrderToGiftCertificate.GiftCertificate.Balance - diff >= 0 ?
                                existItem.OrderToGiftCertificate.GiftCertificate.Balance-diff : 0;
                            refundOrderToGiftCertificate.GCBalanceAfterUpdate= existItem.OrderToGiftCertificate.GiftCertificate.Balance;
                        }
                    }
                }

            });
        }
    }
}