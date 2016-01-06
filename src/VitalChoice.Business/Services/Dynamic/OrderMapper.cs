﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Products;

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
        private readonly IProductService _productService;

        public OrderMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<OrderOptionType> orderRepositoryAsync, OrderAddressMapper orderAddressMapper,
            CustomerMapper customerMapper, DiscountMapper discountMapper,
            OrderPaymentMethodMapper orderPaymentMethodMapper, SkuMapper skuMapper, ProductMapper productMapper,
            IProductService productService)
            : base(converter, converterService, orderRepositoryAsync)
        {
            _orderAddressMapper = orderAddressMapper;
            _customerMapper = customerMapper;
            _discountMapper = discountMapper;
            _orderPaymentMethodMapper = orderPaymentMethodMapper;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
            _productService = productService;
        }

        protected override Expression<Func<OrderOptionValue, int>> ObjectIdReferenceSelector
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

                dynamic.IdAddedBy = entity.IdAddedBy;
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

                    if (dynamic.Skus != null && dynamic.Skus.Count != 0)
                    {
                        var productContents = await _productService.SelectProductContents(dynamic.Skus.Select(p => p.ProductWithoutSkus.Id).ToList());
                        foreach (var productContent in productContents)
                        {
                            var sku = dynamic.Skus.FirstOrDefault(p => p.ProductWithoutSkus.Id == productContent.Id);
                            if (sku != null)
                            {
                                sku.ProductWithoutSkus.Url = productContent.Url;
                            }
                        }
                    }
                }

                dynamic.AffiliateOrderPayment = entity.AffiliateOrderPayment;
            });
        }

        protected override async Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderDynamic, Order>> items)
        {
            await items.ForEachAsync(async item =>
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

                entity.IdAddedBy = entity.IdAddedBy;
                entity.DiscountTotal = dynamic.DiscountTotal;
                entity.OrderStatus = dynamic.OrderStatus;
                entity.ProductsSubtotal = dynamic.ProductsSubtotal;
                entity.ShippingTotal = dynamic.ShippingTotal;
                entity.TaxTotal = dynamic.TaxTotal;
                entity.Total = dynamic.Total;

                await _orderAddressMapper.UpdateEntityAsync(dynamic.ShippingAddress, entity.ShippingAddress);

                entity.IdCustomer = dynamic.Customer.Id;
                entity.GiftCertificates?.MergeKeyed(
                    dynamic.GiftCertificates,
                    g => g.IdGiftCertificate, gio => gio.GiftCertificate?.Id,
                    g => new OrderToGiftCertificate
                    {
                        Amount = g.Amount,
                        IdOrder = dynamic.Id,
                        IdGiftCertificate = g.GiftCertificate.Id
                    });
                entity.IdDiscount = dynamic.Discount?.Id;
                if(dynamic.PaymentMethod.Address!=null && entity.PaymentMethod.BillingAddress==null)
                {
                    entity.PaymentMethod.BillingAddress = new VitalChoice.Ecommerce.Domain.Entities.Addresses.OrderAddress();
                    entity.PaymentMethod.BillingAddress.OptionValues = new List<OrderAddressOptionValue>();
                }
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
                    entity.Skus.MergeKeyed(dynamic.Skus, s => s.IdSku, ds => ds.Sku?.Id,
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