﻿using System;
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
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.InventorySkus;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderMapper : DynamicMapper<OrderDynamic, Order, OrderOptionType, OrderOptionValue>
    {
        private readonly OrderAddressMapper _orderAddressMapper;
        private readonly CustomerMapper _customerMapper;
        private readonly DiscountMapper _discountMapper;
        private readonly OrderPaymentMethodMapper _orderPaymentMethodMapper;
        private readonly SkuMapper _skuMapper;
        private readonly IGcService _gcService;
        private readonly PromotionMapper _promotionMapper;
        private readonly IProductService _productService;
        private readonly IInventorySkuService _inventorySkuService;

        public OrderMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<OrderOptionType> orderRepositoryAsync, OrderAddressMapper orderAddressMapper,
            CustomerMapper customerMapper, DiscountMapper discountMapper,
            OrderPaymentMethodMapper orderPaymentMethodMapper, SkuMapper skuMapper,
            IProductService productService,
            IInventorySkuService inventorySkuService,
            PromotionMapper promotionMapper, IGcService gcService)
            : base(converter, converterService, orderRepositoryAsync)
        {
            _orderAddressMapper = orderAddressMapper;
            _customerMapper = customerMapper;
            _discountMapper = discountMapper;
            _orderPaymentMethodMapper = orderPaymentMethodMapper;
            _skuMapper = skuMapper;
            _productService = productService;
            _inventorySkuService = inventorySkuService;
            _promotionMapper = promotionMapper;
            _gcService = gcService;
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
                dynamic.POrderStatus = entity.POrderStatus;
                dynamic.NPOrderStatus = entity.NPOrderStatus;
                dynamic.ProductsSubtotal = entity.ProductsSubtotal;
                dynamic.ShippingTotal = entity.ShippingTotal;
                dynamic.TaxTotal = entity.TaxTotal;
                dynamic.Total = entity.Total;
                dynamic.IdOrderSource = entity.IdOrderSource;
                dynamic.ReshipProblemSkus = entity.ReshipProblemSkus?.Select(p=>new ReshipProblemSkuOrdered()
                {
                    IdSku = p.IdSku,
                    Code=p.Sku.Code,
                    IdOrder = p.IdOrder,
                }).ToList();

                dynamic.IsHealthwise = entity.HealthwiseOrder != null;

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
                    }));

                    if (dynamic.Skus != null && dynamic.Skus.Count != 0)
                    {
                        var productContents =
                            await _productService.SelectProductContents(dynamic.Skus.Select(p => p.Sku.IdProduct).Distinct().ToList());
                        foreach (var productContent in productContents)
                        {
                            var sku = dynamic.Skus.FirstOrDefault(p => p.Sku.IdProduct == productContent.Id);
                            if (sku != null)
                            {
                                sku.Sku.Product.Url = productContent.Url;
                            }
                        }
                    }
                }

                if (entity.PromoSkus != null)
                {
                    if (dynamic.PromoSkus == null)
                    {
                        dynamic.PromoSkus = new List<PromoOrdered>();
                    }

                    await dynamic.PromoSkus.AddRangeAsync(entity.PromoSkus.Select(async s => new PromoOrdered
                    {
                        Amount = s.Amount,
                        Quantity = s.Quantity,
                        Sku = await _skuMapper.FromEntityAsync(s.Sku, withDefaults),
                        Promotion = await _promotionMapper.FromEntityAsync(s.Promo, withDefaults),
                        Enabled = !s.Disabled
                    }));

                    if (dynamic.PromoSkus.Count != 0)
                    {
                        var productContents =
                            await _productService.SelectProductContents(dynamic.PromoSkus.Select(p => p.Sku.IdProduct).Distinct().ToList());
                        foreach (var productContent in productContents)
                        {
                            var sku = dynamic.PromoSkus.FirstOrDefault(p => p.Sku.IdProduct == productContent.Id);
                            if (sku != null)
                            {
                                sku.Sku.Product.Url = productContent.Url;
                            }
                        }
                    }
                }

                dynamic.GeneratedGcs = entity.GiftCertificatesGenerated?.Select(g => new GeneratedGiftCertificate
                {
                    Sku = _skuMapper.FromEntity(g.Sku, true),
                    Id = g.Id,
                    Balance = g.Balance,
                    Code = g.Code
                }).ToList() ?? new List<GeneratedGiftCertificate>();

                dynamic.AffiliateOrderPayment = entity.AffiliateOrderPayment;
            });
        }

        protected override async Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderDynamic, Order>> items)
        {
            var inventoryMap = await GetInventoryMap(items);

            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdAddedBy = entity.IdEditedBy;
                entity.DiscountTotal = dynamic.DiscountTotal;
                entity.OrderStatus = dynamic.OrderStatus;
                entity.POrderStatus = dynamic.POrderStatus;
                entity.NPOrderStatus = dynamic.NPOrderStatus;
                entity.ProductsSubtotal = dynamic.ProductsSubtotal;
                entity.ShippingTotal = dynamic.ShippingTotal;
                entity.TaxTotal = dynamic.TaxTotal;
                entity.Total = dynamic.Total;
                entity.IdOrderSource = dynamic.IdOrderSource;
                entity.ReshipProblemSkus = dynamic.ReshipProblemSkus?.Select(p => new ReshipProblemSku()
                {
                    IdSku = p.IdSku,
                }).ToList();

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
                    IdSku = s.Sku.Id,
                }));
                foreach (var orderToSku in entity.Skus)
                {
                    List<SkuToInventorySku> inventories;
                    inventoryMap.TryGetValue(orderToSku.IdSku, out inventories);
                    orderToSku.InventorySkus = inventories?.Select(p => new OrderToSkuToInventorySku()
                    {
                        IdSku = orderToSku.IdSku,
                        IdOrder = orderToSku.IdOrder,
                        IdInventorySku = p.IdInventorySku,
                        Quantity=p.Quantity,
                    }).ToList();
                }

                entity.PromoSkus = new List<OrderToPromo>(dynamic.PromoSkus.Select(s => new OrderToPromo
                {
                    Amount = s.Amount,
                    Quantity = s.Quantity,
                    IdOrder = dynamic.Id,
                    IdSku = s.Sku.Id,
                    IdPromo = s.Promotion?.Id,
                    Disabled = !s.Enabled
                }));
                foreach (var orderToPromo in entity.PromoSkus)
                {
                    List<SkuToInventorySku> inventories;
                    inventoryMap.TryGetValue(orderToPromo.IdSku, out inventories);
                    orderToPromo.InventorySkus = inventories?.Select(p => new OrderToPromoToInventorySku()
                    {
                        IdSku = orderToPromo.IdSku,
                        IdOrder = orderToPromo.IdOrder,
                        IdInventorySku = p.IdInventorySku,
                        Quantity=p.Quantity,
                    }).ToList();
                }

                entity.GiftCertificatesGenerated = dynamic.GeneratedGcs?.Select(g => new GiftCertificate
                {
                    IdSku = g.Sku.Id,
                    Balance = g.Balance,
                    Code = g.Id == 0 ? _gcService.GenerateGCCode().Result : g.Code,
                    Email = dynamic.Customer?.Email,
                    FirstName = dynamic.Customer?.ProfileAddress.SafeData.FirstName,
                    LastName = dynamic.Customer?.ProfileAddress.SafeData.LastName,
                    IdOrder = dynamic.Id == 0 ? null : (int?) dynamic.Id,
                    GCType = g.Sku.IdObjectType == (int) ProductType.EGс ? GCType.EGC : GCType.GC,
                    PublicId = Guid.NewGuid(),
                    Created = DateTime.Now,
                    StatusCode = RecordStatusCode.Active,
                    UserId = dynamic.Customer?.Id
                }).ToList() ?? new List<GiftCertificate>();
            });
        }

        protected override async Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderDynamic, Order>> items)
        {
            var inventoryMap = await GetInventoryMap(items);

            await items.ForEachAsync(async item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdAddedBy = entity.IdAddedBy;
                entity.DiscountTotal = dynamic.DiscountTotal;
                entity.OrderStatus = dynamic.OrderStatus;
                entity.POrderStatus = dynamic.POrderStatus;
                entity.NPOrderStatus = dynamic.NPOrderStatus;
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
                if (dynamic.PaymentMethod.Address != null && entity.PaymentMethod.BillingAddress == null)
                {
                    entity.PaymentMethod.BillingAddress = new OrderAddress {OptionValues = new List<OrderAddressOptionValue>()};
                }
                await _orderPaymentMethodMapper.UpdateEntityAsync(dynamic.PaymentMethod, entity.PaymentMethod);
                if (entity.Skus == null)
                {
                    entity.Skus = new List<OrderToSku>();
                }
                entity.Skus.MergeKeyed(dynamic.Skus.Where(s => (s.Sku?.Id ?? 0) != 0).ToArray(), sku => sku.IdSku, ordered => ordered.Sku.Id,
                    s => new OrderToSku
                    {
                        Amount = s.Amount,
                        Quantity = s.Quantity,
                        IdOrder = dynamic.Id,
                        IdSku = s.Sku.Id,
                        InventorySkus = new List<OrderToSkuToInventorySku>(),
                    }, (sku, ordered) =>
                    {
                        sku.Amount = ordered.Amount;
                        sku.Quantity = ordered.Quantity;
                    });
                foreach (var orderToSku in entity.Skus)
                {
                    List<SkuToInventorySku> inventories;
                    inventoryMap.TryGetValue(orderToSku.IdSku, out inventories);
                    orderToSku.InventorySkus.MergeKeyed(inventories ?? new List<SkuToInventorySku>(), p => p.IdInventorySku, p => p.IdInventorySku,
                        p => new OrderToSkuToInventorySku()
                        {
                            IdSku = orderToSku.IdSku,
                            IdOrder = orderToSku.IdOrder,
                            IdInventorySku = p.IdInventorySku,
                            Quantity=p.Quantity,
                        },
                        (p, rp) => {
                            p.Quantity = rp.Quantity;
                        });
                }

                if (entity.PromoSkus == null)
                {
                    entity.PromoSkus = new List<OrderToPromo>();
                }
                entity.PromoSkus.MergeKeyed(dynamic.PromoSkus.Where(s => (s.Sku?.Id ?? 0) != 0).ToArray(), sku => sku.IdSku,
                    ordered => ordered.Sku.Id, s => new OrderToPromo
                    {
                        Amount = s.Amount,
                        Quantity = s.Quantity,
                        IdOrder = dynamic.Id,
                        IdSku = s.Sku.Id,
                        IdPromo = s.Promotion?.Id,
                        Disabled = !s.Enabled,
                        InventorySkus = new List<OrderToPromoToInventorySku>(),
                    }, (sku, ordered) =>
                    {
                        sku.Amount = ordered.Amount;
                        sku.Quantity = ordered.Quantity;
                        sku.Disabled = !ordered.Enabled;
                        sku.IdPromo = ordered.Promotion?.Id;
                    });
                foreach (var orderToPromo in entity.PromoSkus)
                {
                    List<SkuToInventorySku> inventories;
                    inventoryMap.TryGetValue(orderToPromo.IdSku, out inventories);
                    orderToPromo.InventorySkus.MergeKeyed(inventories ?? new List<SkuToInventorySku>(), p => p.IdInventorySku, p => p.IdInventorySku,
                        p => new OrderToPromoToInventorySku()
                        {
                            IdSku = orderToPromo.IdSku,
                            IdOrder = orderToPromo.IdOrder,
                            IdInventorySku = p.IdInventorySku,
                            Quantity = p.Quantity,
                        },
                        (p, rp) => {
                            p.Quantity = rp.Quantity;
                        });
                }

                if (entity.GiftCertificatesGenerated == null)
                {
                    entity.GiftCertificatesGenerated = new List<GiftCertificate>();
                }

                entity.GiftCertificatesGenerated.MergeKeyed(dynamic.GeneratedGcs, g => g.Id, g => g.Id, src => new GiftCertificate
                {
                    IdSku = src.Sku.Id,
                    Balance = src.Balance,
                    Code = src.Id == 0 ? _gcService.GenerateGCCode().Result : src.Code,
                    Email = dynamic.Customer?.Email,
                    FirstName = dynamic.Customer?.ProfileAddress.SafeData.FirstName,
                    LastName = dynamic.Customer?.ProfileAddress.SafeData.LastName,
                    IdOrder = dynamic.Id == 0 ? null : (int?) dynamic.Id,
                    GCType = src.Sku.IdObjectType == (int) ProductType.EGс ? GCType.EGC : GCType.GC,
                    PublicId = Guid.NewGuid(),
                    Created = DateTime.Now,
                    StatusCode = RecordStatusCode.Active,
                    UserId = dynamic.Customer?.Id
                });
            });
        }

        private async Task<Dictionary<int, List<SkuToInventorySku>>> GetInventoryMap(ICollection<DynamicEntityPair<OrderDynamic, Order>> items)
        {
            var skuIds = items.Select(p => p.Dynamic).SelectMany(p => p?.Skus).Select(p => p.Sku.Id).ToList();
            skuIds.AddRange(items.Select(p => p.Dynamic).SelectMany(p => p?.PromoSkus).Select(p => p.Sku.Id).ToList());
            Dictionary<int, List<SkuToInventorySku>> inventoryMap;
            if (skuIds.Count > 0)
            {
                inventoryMap = await _inventorySkuService.GetAssignedInventorySkusAsync(skuIds);
            }
            else
            {
                inventoryMap = new Dictionary<int, List<SkuToInventorySku>>();
            }
            return inventoryMap;
        }
    }
}