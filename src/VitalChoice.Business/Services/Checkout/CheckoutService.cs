using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Orders;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Checkout;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;

namespace VitalChoice.Business.Services.Checkout
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IEcommerceRepositoryAsync<Cart> _cartRepository;
        private readonly DiscountMapper _discountMapper;
        private readonly SkuMapper _skuMapper;
        private readonly ProductMapper _productMapper;
        private readonly OrderService _orderService;
        private readonly EcommerceContext _context;
        private readonly ILogger _logger;

        public CheckoutService(IEcommerceRepositoryAsync<Cart> cartRepository,
            DiscountMapper discountMapper,
            SkuMapper skuMapper, ProductMapper productMapper, OrderService orderService, EcommerceContext context, ILoggerProviderExtended loggerProvider)
        {
            _cartRepository = cartRepository;
            _discountMapper = discountMapper;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
            _orderService = orderService;
            _context = context;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<CustomerCart> GetOrCreateAnonymCart(Guid? uid)
        {
            Cart cart = null;
            if (uid.HasValue)
            {
                cart = await _cartRepository.Query(c => c.CartUid == uid.Value).Include(c => c.Discount)
                    .ThenInclude(d => d.OptionValues)
                    .Include(c => c.GiftCertificates)
                    .ThenInclude(g => g.GiftCertificate)
                    .Include(c => c.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.OptionValues)
                    .Include(c => c.Skus)
                    .ThenInclude(s => s.Sku)
                    .ThenInclude(s => s.Product)
                    .ThenInclude(p => p.OptionValues).SelectFirstOrDefaultAsync(false);
            }
            if (cart == null)
            {
                cart = await CreateNew();
            }

            return new CustomerCart
            {
                CartUid = cart.CartUid,
                GiftCertificates = cart.GiftCertificates?.Select(g => new GiftCertificateInOrder
                {
                    Amount = g.Amount,
                    GiftCertificate = g.GiftCertificate
                }).ToList(),
                Discount = await _discountMapper.FromEntityAsync(cart.Discount, true),
                Skus = await Task.WhenAll(cart.Skus?.Select(async s => new SkuOrdered
                {
                    Amount = s.Amount,
                    Sku = await _skuMapper.FromEntityAsync(s.Sku, true),
                    ProductWithoutSkus = await _productMapper.FromEntityAsync(s.Sku.Product, true),
                    Quantity = s.Quantity
                }))
            };
        }

        public async Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, int idCustomer)
        {
            Cart cart = null;
            if (uid.HasValue)
            {
                cart =
                    await _cartRepository.Query(c => c.CartUid == uid.Value && c.IdCustomer == idCustomer).SelectFirstOrDefaultAsync(false);

                if (cart.IdOrder == null)
                {
                    var anonymCart = await GetOrCreateAnonymCart(uid);
                    var newOrder = await _orderService.CreatePrototypeAsync((int) OrderType.Normal);
                    newOrder.OrderStatus = OrderStatus.Incomplete;
                    newOrder.GiftCertificates = anonymCart.GiftCertificates;
                    newOrder.Discount = anonymCart.Discount;
                    newOrder.Skus = anonymCart.Skus;
                }
            }
            if (cart == null)
            {
                cart = await CreateNew(idCustomer);
            }

            if (cart.IdOrder.HasValue && cart.IdOrder.Value > 0)
            {
                return new CustomerCartOrder
                {
                    CartUid = cart.CartUid,
                    Order = await _orderService.SelectAsync(cart.IdOrder.Value, true)
                };
            }
            return new CustomerCartOrder
            {
                CartUid = cart.CartUid,
                Order = await _orderService.CreatePrototypeAsync((int) OrderType.Normal)
            };
        }

        private async Task<Cart> CreateNew(int? idCustomer = null)
        {
            var cart = new Cart
            {
                CartUid = Guid.NewGuid(),
                IdCustomer = idCustomer
            };
            await _cartRepository.InsertAsync(cart);
            return cart;
        }

        public async Task<bool> UpdateCart(CustomerCart anonymCart)
        {
            if (anonymCart == null)
                throw new ArgumentNullException(nameof(anonymCart));
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    var cart =
                        await
                            _cartRepository.Query(c => c.CartUid == anonymCart.CartUid)
                                .Include(c => c.GiftCertificates)
                                .Include(c => c.Skus)
                                .SelectFirstOrDefaultAsync();

                    if (cart == null)
                        return false;
                    cart.IdDiscount = anonymCart.Discount?.Id;
                    cart.GiftCertificates.MergeKeyed(anonymCart.GiftCertificates, c => c.IdGiftCertificate, co => co.GiftCertificate.Id,
                        co => new CartToGiftCertificate
                        {
                            Amount = co.Amount,
                            IdCart = cart.Id,
                            IdGiftCertificate = co.GiftCertificate.Id
                        });
                    cart.Skus.MergeKeyed(anonymCart.Skus, s => s.IdSku, so => so.Sku.Id, so => new CartToSku
                    {
                        Amount = so.Amount,
                        IdCart = cart.Id,
                        IdSku = so.Sku.Id,
                        Quantity = so.Quantity
                    });
                    if (cart.IdOrder.HasValue)
                    {
                        var orderDynamic = await _orderService.SelectAsync(cart.IdOrder.Value);
                        if (orderDynamic != null)
                        {
                            orderDynamic.Discount = anonymCart.Discount;
                            orderDynamic.Skus = anonymCart.Skus;
                            orderDynamic.GiftCertificates = anonymCart.GiftCertificates;
                            await _orderService.UpdateAsync(orderDynamic);
                        }
                    }

                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> UpdateCart(CustomerCartOrder cartOrder)
        {
            if (cartOrder == null)
                throw new ArgumentNullException(nameof(cartOrder));
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    var cart =
                        await
                            _cartRepository.Query(c => c.CartUid == cartOrder.CartUid)
                                .Include(c => c.GiftCertificates)
                                .Include(c => c.Skus)
                                .SelectFirstOrDefaultAsync();

                    if (cart == null)
                        return false;
                    if (cartOrder.Order == null)
                        return false;
                    if (cartOrder.Order.Id == 0)
                    {
                        cartOrder.Order = await _orderService.InsertAsync(cartOrder.Order);
                    }
                    else
                    {
                        cartOrder.Order = await _orderService.UpdateAsync(cartOrder.Order);
                    }
                    cart.IdDiscount = cartOrder.Order.Discount?.Id;
                    cart.GiftCertificates.MergeKeyed(cartOrder.Order.GiftCertificates, c => c.IdGiftCertificate, co => co.GiftCertificate.Id,
                        co => new CartToGiftCertificate
                        {
                            Amount = co.Amount,
                            IdCart = cart.Id,
                            IdGiftCertificate = co.GiftCertificate.Id
                        });
                    cart.Skus.MergeKeyed(cartOrder.Order.Skus, s => s.IdSku, so => so.Sku.Id, so => new CartToSku
                    {
                        Amount = so.Amount,
                        IdCart = cart.Id,
                        IdSku = so.Sku.Id,
                        Quantity = so.Quantity
                    });
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    transaction.Rollback();
                    return false;
                }
            }
            return true;
        }

        public async Task<OrderDataContext> CalculateCart(CustomerCart anonymCart)
        {
            var cart = await _cartRepository.Query(c => c.CartUid == anonymCart.CartUid).SelectFirstOrDefaultAsync();
            OrderDynamic order;
            if (cart.IdOrder.HasValue)
            {
                order = await _orderService.SelectAsync(cart.IdOrder.Value, true);
            }
            else
            {
                order = await _orderService.CreatePrototypeAsync((int) OrderType.Normal);
            }
            order.Discount = anonymCart.Discount;
            order.GiftCertificates = anonymCart.GiftCertificates;
            order.Skus = anonymCart.Skus;
            return await _orderService.CalculateOrder(order);
        }

        public Task<OrderDataContext> CalculateCart(CustomerCartOrder cartOrder)
        {
            return _orderService.CalculateOrder(cartOrder.Order);
        }

        public Task<bool> SaveOrder(CustomerCartOrder cart)
        {
            throw new NotImplementedException();
        }
    }
}