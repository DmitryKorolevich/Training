using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Order;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.DynamicData.Entities.Transfer;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.Domain.Helpers;

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderMapper : DynamicMapper<OrderDynamic, Order, OrderOptionType, OrderOptionValue>
    {
        private readonly OrderAddressMapper _orderAddressMapper;
        private readonly CustomerMapper _customerMapper;
        private readonly DiscountMapper _discountMapper;
        private readonly OrderPaymentMethodMapper _orderPaymentMethodMapper;
        private readonly SkuMapper _skuMapper;
        private readonly ProductMapper _productMapper;

        public OrderMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<OrderOptionType> orderRepositoryAsync, OrderAddressMapper orderAddressMapper,
            CustomerMapper customerMapper, DiscountMapper discountMapper,
            OrderPaymentMethodMapper orderPaymentMethodMapper, SkuMapper skuMapper, ProductMapper productMapper)
            : base(converter, converterService, orderRepositoryAsync)
        {
            _orderAddressMapper = orderAddressMapper;
            _customerMapper = customerMapper;
            _discountMapper = discountMapper;
            _orderPaymentMethodMapper = orderPaymentMethodMapper;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
        }

        public override Expression<Func<OrderOptionValue, int?>> ObjectIdSelector
        {
            get { return v => v.IdOrder; }
        }

        protected override async Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderDynamic, Order>> items, bool withDefaults = false)
        {
            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.DiscountTotal = entity.DiscountTotal;
                dynamic.OrderStatus = entity.OrderStatus;
                dynamic.ProductsSubtotal = entity.ProductsSubtotal;
                dynamic.ShippingTotal = entity.ShippingTotal;
                dynamic.TaxTotal = entity.TaxTotal;
                dynamic.Total = entity.Total;

                dynamic.ShippingAddress =
                    await _orderAddressMapper.FromEntityAsync(entity.ShippingAddress, withDefaults);
                dynamic.Customer = await _customerMapper.FromEntityAsync(entity.Customer, withDefaults);
                if (dynamic.Customer == null || dynamic.Customer.Id == 0)
                {
                    dynamic.Customer = new CustomerDynamic
                    {
                        Id = entity.IdCustomer
                    };
                }

                if (entity.GiftCertificates != null)
                {
                    dynamic.GiftCertificates.AddRange(entity.GiftCertificates.Select(g => new GiftCertificateInOrder
                    {
                        Amount = g.Amount,
                        GiftCertificate = g.GiftCertificate
                    }));
                }
                dynamic.Discount = await _discountMapper.FromEntityAsync(entity.Discount, withDefaults);
                dynamic.PaymentMethod =
                    await _orderPaymentMethodMapper.FromEntityAsync(entity.PaymentMethod, withDefaults);
                if (entity.Skus != null)
                {
                    await dynamic.Skus.AddRangeAsync(entity.Skus.Select(async s => new SkuOrdered
                    {
                        Amount = s.Amount,
                        Quantity = s.Quantity,
                        Sku = await _skuMapper.FromEntityAsync(s.Sku, withDefaults),
                        ProductWithoutSkus = await _productMapper.FromEntityAsync(s.Sku?.Product, withDefaults)
                    }));
                }
            });
        }

        protected override async Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderDynamic, Order>> items)
        {
            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.DiscountTotal = dynamic.DiscountTotal;
                entity.OrderStatus = dynamic.OrderStatus;
                entity.ProductsSubtotal = dynamic.ProductsSubtotal;
                entity.ShippingTotal = dynamic.ShippingTotal;
                entity.TaxTotal = dynamic.TaxTotal;
                entity.Total = dynamic.Total;

                entity.ShippingAddress =
                    await _orderAddressMapper.ToEntityAsync(dynamic.ShippingAddress);
                entity.IdCustomer = dynamic.Customer.Id;
                entity.GiftCertificates = new List<OrderToGiftCertificate>(dynamic.GiftCertificates.Select(g => new OrderToGiftCertificate
                {
                    Amount = g.Amount,
                    IdOrder = dynamic.Id,
                    IdGiftCertificate = g.GiftCertificate.Id
                }));
                entity.IdDiscount = dynamic.Discount?.Id;
                entity.PaymentMethod =
                    await _orderPaymentMethodMapper.ToEntityAsync(dynamic.PaymentMethod);
                entity.Skus = new List<OrderToSku>(dynamic.Skus.Select(s => new OrderToSku
                {
                    Amount = s.Amount,
                    Quantity = s.Quantity,
                    IdOrder = dynamic.Id,
                    IdSku = s.Sku.Id
                }));
            });
        }

        protected override async Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderDynamic, Order>> items)
        {
            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.DiscountTotal = dynamic.DiscountTotal;
                entity.OrderStatus = dynamic.OrderStatus;
                entity.ProductsSubtotal = dynamic.ProductsSubtotal;
                entity.ShippingTotal = dynamic.ShippingTotal;
                entity.TaxTotal = dynamic.TaxTotal;
                entity.Total = dynamic.Total;

                await _orderAddressMapper.UpdateEntityAsync(dynamic.ShippingAddress, entity.ShippingAddress);

                entity.IdCustomer = dynamic.Customer.Id;
                entity.GiftCertificates?.Merge(dynamic.GiftCertificates,
                    (g, gio) =>
                        g.IdGiftCertificate != gio.GiftCertificate?.Id,
                    g => new OrderToGiftCertificate
                    {
                        Amount = g.Amount,
                        IdOrder = dynamic.Id,
                        IdGiftCertificate = g.GiftCertificate.Id
                    });
                entity.IdDiscount = dynamic.Discount?.Id;
                await _orderPaymentMethodMapper.UpdateEntityAsync(dynamic.PaymentMethod, entity.PaymentMethod);
                Dictionary<int, SkuOrdered> keyedSkus = new Dictionary<int, SkuOrdered>();
                if (dynamic.Skus != null)
                {
                    keyedSkus = dynamic.Skus.Where(s => s.Sku?.Id > 0).ToDictionary(s => s.Sku.Id);
                }
                if (entity.Skus != null)
                {
                    //Update
                    foreach (var sku in entity.Skus)
                    {
                        SkuOrdered skuOrdered;
                        if (keyedSkus.TryGetValue(sku.IdSku, out skuOrdered))
                        {
                            sku.Amount = skuOrdered.Amount;
                            sku.Quantity = skuOrdered.Quantity;
                        }
                    }
                    //Add
                    entity.Skus.Merge(dynamic.Skus, (s, ds) => s.IdSku != ds.Sku?.Id,
                        s => new OrderToSku
                        {
                            Amount = s.Amount,
                            Quantity = s.Quantity,
                            IdOrder = dynamic.Id,
                            IdSku = s.Sku.Id
                        });
                }
            });
        }
    }
}