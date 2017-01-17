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
using VitalChoice.Ecommerce.Domain.Entities.Checkout;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
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

        public override Expression<Func<OrderOptionValue, int>> ObjectIdSelector => o => o.IdOrder;

        public override OrderDynamic CreatePrototype(int idObjectType)
        {
            var order = base.CreatePrototype(idObjectType);
            if (order.Customer == null)
            {
                order.Customer = _customerMapper.CreatePrototype((int) CustomerType.Retail);
            }
            return order;
        }

        public override OrderDynamic CreatePrototype()
        {
            var order = base.CreatePrototype();
            if (order.Customer == null)
            {
                order.Customer = _customerMapper.CreatePrototype((int)CustomerType.Retail);
            }
            return order;
        }

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderDynamic, Order>> items, bool withDefaults = false)
        {
            return items.ForEachAsync(async item =>
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
                dynamic.ReshipProblemSkus = entity.ReshipProblemSkus?.Select(p => new ReshipProblemSkuOrdered()
                {
                    IdSku = p.IdSku,
                    Code = p.Sku.Code,
                    IdOrder = p.IdOrder,
                }).ToList();

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
                        PAmount = g.PAmount,
                        NPAmount = g.NPAmount,
                        GiftCertificate = g.GiftCertificate
                    }));
                    dynamic.GiftCertificates.ForEach(p =>
                    {
                        p.GiftCertificate.Sku = null;
                        p.GiftCertificate.Order = null;
                    });
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
                        GcsGenerated = s.GeneratedGiftCertificates
                    }));

                    dynamic.Skus.ForEach(p =>
                    {
                        p.GcsGenerated?.ForEach(pp =>
                        {
                            pp.Sku = null;
                            pp.Order = null;
                        });
                    });

                    if (dynamic.Skus != null && dynamic.Skus.Count != 0 && dynamic.Skus.First().Sku?.Product != null)
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

                    if (dynamic.PromoSkus != null && dynamic.PromoSkus.Count != 0 && dynamic.PromoSkus.First().Sku?.Product != null)
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

                dynamic.AffiliateOrderPayment = entity.AffiliateOrderPayment;
                dynamic.OrderShippingPackages = entity.OrderShippingPackages?.Select(p => new OrderShippingPackageModelItem(p)).ToList() ??
                                                new List<OrderShippingPackageModelItem>();

                if (entity.HealthwiseOrder != null)
                {
                    dynamic.Data.IsHealthwise = true;
                }

                if (entity.CartAdditionalShipments != null)
                {
                    dynamic.CartAdditionalShipments=new List<CartAdditionalShipmentModelItem>();
                    foreach (var entityAdditionalShipment in entity.CartAdditionalShipments)
                    {
                        CartAdditionalShipmentModelItem shipment =new CartAdditionalShipmentModelItem();
                        shipment.Id = entityAdditionalShipment.Id;
                        shipment.Name = entityAdditionalShipment.Name;
                        shipment.IsGiftOrder = entityAdditionalShipment.IsGiftOrder;
                        shipment.GiftMessage = entityAdditionalShipment.GiftMessage;
                        shipment.ShippingAddress = await _orderAddressMapper.FromEntityAsync(entityAdditionalShipment.ShippingAddress, 
                            withDefaults);

                        if (entityAdditionalShipment.Skus != null)
                        {
                            await shipment.Skus.AddRangeAsync(entity.Skus.Select(async s => new SkuOrdered
                            {
                                Amount = s.Amount,
                                Quantity = s.Quantity,
                                Sku = await _skuMapper.FromEntityAsync(s.Sku, withDefaults),
                                GcsGenerated = s.GeneratedGiftCertificates
                            }));

                            if (shipment.Skus != null && shipment.Skus.Count != 0 && dynamic.Skus.First().Sku?.Product != null)
                            {
                                var productContents =
                                    await _productService.SelectProductContents(shipment.Skus.Select(p => p.Sku.IdProduct).Distinct().ToList());

                                foreach (var productContent in productContents)
                                {
                                    var sku = shipment.Skus.FirstOrDefault(p => p.Sku.IdProduct == productContent.Id);
                                    if (sku != null)
                                    {
                                        sku.Sku.Product.Url = productContent.Url;
                                    }
                                }
                            }
                        }
                    }
                }
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
                    PAmount = g.PAmount,
                    NPAmount = g.NPAmount,
                    IdOrder = dynamic.Id,
                    IdGiftCertificate = g.GiftCertificate.Id
                }));
                entity.IdDiscount = dynamic.Discount?.Id;
                entity.PaymentMethod =
                    await _orderPaymentMethodMapper.ToEntityAsync(dynamic.PaymentMethod);
                foreach (var sku in dynamic.Skus.Where(s => (s.GcsGenerated?.Count ?? 0) > 0))
                {
                    foreach (var gc in sku.GcsGenerated.Where(g => string.IsNullOrEmpty(g.Code)))
                    {
                        gc.Code = await _gcService.GenerateGCCode();
                    }
                }
                entity.Skus = new List<OrderToSku>(dynamic.Skus.Select(s => new OrderToSku
                {
                    Amount = s.Amount,
                    Quantity = s.Quantity,
                    IdOrder = dynamic.Id,
                    IdSku = s.Sku.Id,
                    GeneratedGiftCertificates = s.GcsGenerated
                }));
                foreach (var gc in entity?.Skus?.Where(p=>p.GeneratedGiftCertificates!=null).SelectMany(p=>p.GeneratedGiftCertificates))
                {
                    gc.UserId = entity.IdEditedBy;
                    gc.IdEditedBy = entity.IdEditedBy;
                }

                foreach (var orderToSku in entity.Skus)
                {
                    List<SkuToInventorySku> inventories;
                    inventoryMap.TryGetValue(orderToSku.IdSku, out inventories);
                    orderToSku.InventorySkus = inventories?.Select(p => new OrderToSkuToInventorySku
                    {
                        IdSku = orderToSku.IdSku,
                        IdOrder = orderToSku.IdOrder,
                        IdInventorySku = p.IdInventorySku,
                        Quantity = p.Quantity,
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
                        Quantity = p.Quantity,
                    }).ToList();
                }

                if (dynamic.CartAdditionalShipments != null)
                {
                    entity.CartAdditionalShipments = dynamic.CartAdditionalShipments.Select(s=> new CartAdditionalShipment()
                    {
                        Name = s.Name,
                        IsGiftOrder = s.IsGiftOrder,
                        GiftMessage = s.GiftMessage,
                        ShippingAddress = _orderAddressMapper.ToEntityAsync(s.ShippingAddress).Result,

                        Skus = new List<CartAdditionalShipmentToSku>(s.Skus.Select(p => new CartAdditionalShipmentToSku()
                        {
                            Amount = p.Amount,
                            Quantity = p.Quantity,
                            IdCartAdditionalShipment = s.Id,
                            IdSku = p.Sku.Id,
                        }))
                    }).ToList();
                }
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

                entity.ReshipProblemSkus.MergeKeyed(dynamic.ReshipProblemSkus, p => p.IdSku, pp => pp.IdSku, s => new ReshipProblemSku()
                {
                    IdOrder = dynamic.Id,
                    IdSku = s.IdSku,
                });

                await _orderAddressMapper.UpdateEntityAsync(dynamic.ShippingAddress, entity.ShippingAddress);

                entity.IdCustomer = dynamic.Customer.Id;
                if (dynamic.OrderStatus == OrderStatus.Incomplete ||
                    dynamic.POrderStatus == OrderStatus.Incomplete && dynamic.NPOrderStatus == OrderStatus.Incomplete)
                {
                    entity.GiftCertificates?.MergeKeyed(
                        dynamic.GiftCertificates,
                        g => g.IdGiftCertificate, gio => gio.GiftCertificate?.Id,
                        g => new OrderToGiftCertificate
                        {
                            Amount = 0, //g.Amount,
                            IdOrder = dynamic.Id,
                            IdGiftCertificate = g.GiftCertificate.Id
                        }, (g, dg) => g.Amount = 0);
                }
                else
                {
                    entity.GiftCertificates?.MergeKeyed(
                        dynamic.GiftCertificates,
                        g => g.IdGiftCertificate, gio => gio.GiftCertificate?.Id,
                        g => new OrderToGiftCertificate
                        {
                            Amount = g.Amount,
                            PAmount = g.PAmount,
                            NPAmount = g.NPAmount,
                            IdOrder = dynamic.Id,
                            IdGiftCertificate = g.GiftCertificate.Id,
                        }, (g, dg) =>
                        {
                            g.Amount = dg.Amount;
                            g.PAmount = dg.PAmount;
                            g.NPAmount = dg.NPAmount;
                            if (g.GiftCertificate != null && dg.GiftCertificate != null)
                            {
                                g.GiftCertificate.Balance = dg.GiftCertificate.Balance;
                            }
                        },
                        removed => removed.ForEach(gc => gc.GiftCertificate.Balance += gc.Amount));
                }

                entity.IdDiscount = dynamic.Discount?.Id;
                if (dynamic.PaymentMethod.Address != null && entity.PaymentMethod.BillingAddress == null)
                {
                    entity.PaymentMethod.BillingAddress = new OrderAddress {OptionValues = new List<OrderAddressOptionValue>()};
                }
                if (dynamic.PaymentMethod.Address == null)
                {
                    entity.PaymentMethod.BillingAddress = null;
                }
                await _orderPaymentMethodMapper.UpdateEntityAsync(dynamic.PaymentMethod, entity.PaymentMethod);
                if (entity.Skus == null)
                {
                    entity.Skus = new List<OrderToSku>();
                }
                foreach (var sku in dynamic.Skus.Where(s => (s.GcsGenerated?.Count ?? 0) > 0))
                {
                    foreach (var gc in sku.GcsGenerated.Where(g => string.IsNullOrEmpty(g.Code)))
                    {
                        gc.Code = await _gcService.GenerateGCCode();
                    }
                }
                entity.Skus.MergeKeyed(dynamic.Skus.Where(s => (s.Sku?.Id ?? 0) != 0).ToArray(), sku => sku.IdSku, ordered => ordered.Sku.Id,
                    s => new OrderToSku
                    {
                        Amount = s.Amount,
                        Quantity = s.Quantity,
                        IdOrder = dynamic.Id,
                        IdSku = s.Sku.Id,
                        InventorySkus = new List<OrderToSkuToInventorySku>(),
                        GeneratedGiftCertificates = s.GcsGenerated
                    }, (sku, ordered) =>
                    {
                        sku.Amount = ordered.Amount;
                        sku.Quantity = ordered.Quantity;
                        sku.GeneratedGiftCertificates.MergeKeyed(ordered.GcsGenerated, g => g.Id,
                            removed => removed.ForEach(r => r.StatusCode = RecordStatusCode.Deleted));

                        List<SkuToInventorySku> inventories;
                        inventoryMap.TryGetValue(sku.IdSku, out inventories);

                        sku.InventorySkus.MergeKeyed(inventories ?? new List<SkuToInventorySku>(), p => p.IdInventorySku,
                            p => p.IdInventorySku,
                            p => new OrderToSkuToInventorySku
                            {
                                IdSku = sku.IdSku,
                                IdOrder = sku.IdOrder,
                                IdInventorySku = p.IdInventorySku,
                                Quantity = p.Quantity,
                            },
                            (p, rp) => p.Quantity = rp.Quantity);
                    });

                foreach (var gc in entity?.Skus?.Where(p => p.GeneratedGiftCertificates != null).SelectMany(p => p.GeneratedGiftCertificates))
                {
                    gc.UserId = gc.Id == 0 ? entity.IdEditedBy : gc.UserId;
                    gc.IdEditedBy = entity.IdEditedBy;
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


                        List<SkuToInventorySku> inventories;
                        inventoryMap.TryGetValue(sku.IdSku, out inventories);

                        sku.InventorySkus.MergeKeyed(inventories ?? new List<SkuToInventorySku>(), p => p.IdInventorySku,
                            p => p.IdInventorySku,
                            p => new OrderToPromoToInventorySku
                            {
                                IdSku = sku.IdSku,
                                IdOrder = sku.IdOrder,
                                IdInventorySku = p.IdInventorySku,
                                Quantity = p.Quantity,
                            }, (p, rp) => p.Quantity = rp.Quantity);
                    });

                if (dynamic.CartAdditionalShipments != null)
                {
                    entity.CartAdditionalShipments.MergeKeyed(dynamic.CartAdditionalShipments, p => p.Id,
                        p => p.Id, s => new CartAdditionalShipment()
                        {
                            Name = s.Name,
                            IsGiftOrder = s.IsGiftOrder,
                            GiftMessage = s.GiftMessage,
                            ShippingAddress = _orderAddressMapper.ToEntityAsync(s.ShippingAddress).Result,

                            Skus = new List<CartAdditionalShipmentToSku>(s.Skus.Select(p => new CartAdditionalShipmentToSku()
                            {
                                Amount = p.Amount,
                                Quantity = p.Quantity,
                                IdCartAdditionalShipment = s.Id,
                                IdSku = p.Sku.Id,
                            })),
                        }, (dbItem, modelItem) =>
                        {
                            dbItem.Name = modelItem.Name;
                            dbItem.IsGiftOrder = modelItem.IsGiftOrder;
                            dbItem.GiftMessage = modelItem.GiftMessage;
                            _orderAddressMapper.UpdateEntityAsync(modelItem.ShippingAddress, dbItem.ShippingAddress).Wait();

                            dbItem.Skus.MergeKeyed(modelItem.Skus.Where(s => (s.Sku?.Id ?? 0) != 0).ToArray(), 
                                sku => sku.IdSku, ordered => ordered.Sku.Id,
                                s => new CartAdditionalShipmentToSku
                                {
                                    Amount = s.Amount,
                                    Quantity = s.Quantity,
                                    IdCartAdditionalShipment = dynamic.Id,
                                    IdSku = s.Sku.Id,
                                }, (sku, ordered) =>
                                {
                                    sku.Amount = ordered.Amount;
                                    sku.Quantity = ordered.Quantity;
                                });
                        });
                }
            });
        }

        private async Task<Dictionary<int, List<SkuToInventorySku>>> GetInventoryMap(
            ICollection<DynamicEntityPair<OrderDynamic, Order>> items)
        {
            var skuIds = items.Select(p => p.Dynamic).SelectMany(p => p.Skus).Select(p => p.Sku.Id);
            skuIds = skuIds.Union(items.Select(p => p.Dynamic).SelectMany(p => p.PromoSkus).Select(p => p.Sku.Id));
            var inventoryMap = await _inventorySkuService.GetAssignedInventorySkusAsync(skuIds);
            return inventoryMap;
        }
    }
}