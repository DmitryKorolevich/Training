using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Business.Services.Orders;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Checkout;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Settings;

namespace VitalChoice.Business.Services.Checkout
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IEcommerceRepositoryAsync<Cart> _cartRepository;
        private readonly DiscountMapper _discountMapper;
        private readonly SkuMapper _skuMapper;
        private readonly ProductMapper _productMapper;
        private readonly IOrderService _orderService;
        private readonly EcommerceContext _context;
        private readonly ICustomerService _customerService;
        private readonly IEcommerceRepositoryAsync<CartToSku> _skusRepository;
        private readonly IDynamicReadServiceAsync<AddressDynamic, OrderAddress> _addressService;
        private readonly ICountryService _countryService;
        private readonly IEcommerceRepositoryAsync<OrderToSku> _skuRepository;
        private readonly IRepositoryAsync<ProductContent> _productContentRep;
        private readonly ILogger _logger;

        public CheckoutService(IEcommerceRepositoryAsync<Cart> cartRepository,
            DiscountMapper discountMapper,
            SkuMapper skuMapper, ProductMapper productMapper, IOrderService orderService, EcommerceContext context,
            ILoggerProviderExtended loggerProvider, ICustomerService customerService, IEcommerceRepositoryAsync<CartToSku> skusRepository,
            IDynamicReadServiceAsync<AddressDynamic, OrderAddress> addressService, ICountryService countryService,
            IEcommerceRepositoryAsync<OrderToSku> skuRepository, IRepositoryAsync<ProductContent> productContentRep)
        {
            _cartRepository = cartRepository;
            _discountMapper = discountMapper;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
            _orderService = orderService;
            _context = context;
            _customerService = customerService;
            _skusRepository = skusRepository;
            _addressService = addressService;
            _countryService = countryService;
            _skuRepository = skuRepository;
            _productContentRep = productContentRep;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<CustomerCartOrder> GetOrCreateCart(Guid? uid)
        {
            Cart cart;
            if (uid.HasValue)
            {
                var cartForCheck = await _cartRepository.Query(c => c.CartUid == uid.Value).SelectFirstOrDefaultAsync(false);
                if (cartForCheck?.IdCustomer != null)
                {
                    return await GetOrCreateCart(uid, cartForCheck.IdCustomer.Value);
                }
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
                    .ThenInclude(p => p.OptionValues).SelectFirstOrDefaultAsync(false) ?? await CreateNew();
            }
            else
            {
                cart = await CreateNew();
            }
            var newOrder = await _orderService.CreatePrototypeAsync((int) OrderType.Normal);
            if (newOrder.Customer != null)
            {
                newOrder.Customer.IdObjectType = (int) CustomerType.Retail;
            }
            newOrder.StatusCode = (int) RecordStatusCode.Active;
            newOrder.OrderStatus = OrderStatus.Incomplete;
            newOrder.GiftCertificates = cart.GiftCertificates?.Select(g => new GiftCertificateInOrder
            {
                Amount = g.Amount,
                GiftCertificate = g.GiftCertificate
            }).ToList();
            newOrder.Discount = await _discountMapper.FromEntityAsync(cart.Discount, true);
            newOrder.Skus = cart.Skus?.Select(s =>
            {
                s.Sku.OptionTypes =
                    _productMapper.OptionTypes.Where(
                        _productMapper.GetOptionTypeQuery().WithObjectType(s.Sku.Product.IdObjectType).Query().CacheCompile()).ToList();
                s.Sku.Product.OptionTypes = s.Sku.OptionTypes;
                var productUrl = _productContentRep.Query(p => p.Id == s.Sku.IdProduct).Select(p => p.Url, false).FirstOrDefault();
                var product = _productMapper.FromEntity(s.Sku.Product, true);
                product.Url = productUrl;
                return new SkuOrdered
                {
                    Amount = s.Amount,
                    Sku = _skuMapper.FromEntity(s.Sku, true),
                    ProductWithoutSkus = product,
                    Quantity = s.Quantity
                };
            }).ToList();
            newOrder.ShippingAddress = await _addressService.CreatePrototypeAsync((int) AddressType.Shipping);
            newOrder.ShippingAddress.IdCountry = (await _countryService.GetCountriesAsync(new CountryFilter
            {
                CountryCode = "US"
            })).FirstOrDefault()?.Id ?? 0;
            return new CustomerCartOrder
            {
                CartUid = cart.CartUid,
                Order = newOrder
            };
        }

        public async Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, int idCustomer)
        {
            CustomerCartOrder result = null;
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    Cart cart;
                    if (uid.HasValue)
                    {
                        cart =
                            await _cartRepository.Query(c => c.CartUid == uid.Value).SelectFirstOrDefaultAsync(false) ??
                            await CreateNew(idCustomer);

                        result = new CustomerCartOrder
                        {
                            CartUid = uid.Value
                        };
                        if (cart.IdCustomer == null)
                        {
                            cart.IdCustomer = idCustomer;
                            await _cartRepository.UpdateAsync(cart);
                        }
                    }
                    else
                    {
                        cart =
                            await _cartRepository.Query(c => c.IdCustomer == idCustomer).SelectFirstOrDefaultAsync(false) ??
                            await CreateNew(idCustomer);
                        result = new CustomerCartOrder
                        {
                            CartUid = cart.CartUid
                        };
                    }
                    if (cart.IdOrder == null)
                    {
                        var anonymCart = await GetOrCreateCart(uid);

                        anonymCart.Order.Customer = await _customerService.SelectAsync(idCustomer);
                        anonymCart.Order = await _orderService.InsertAsync(anonymCart.Order);

                        cart = await _cartRepository.Query(c => c.CartUid == anonymCart.CartUid).SelectFirstOrDefaultAsync();
                        cart.IdOrder = anonymCart.Order.Id;
                        await _context.SaveChangesAsync();
                        anonymCart.Order.Customer = await _customerService.SelectAsync(idCustomer, true);
                        result.Order = anonymCart.Order;
                    }
                    else
                    {
                        result.Order = await _orderService.SelectAsync(cart.IdOrder.Value, true);
                        foreach (var skuOrdered in result.Order.Skus)
                        {
                            var productUrl = _productContentRep.Query(p => p.Id == skuOrdered.Sku.IdProduct).Select(p => p.Url, false).FirstOrDefault();
                            skuOrdered.ProductWithoutSkus.Url = productUrl;
                        }
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message, e);
                    transaction.Rollback();
                    return result;
                }
            }

            return result;
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
                    if (cartOrder.Order.Customer?.Id != 0)
                    {
                        if (cartOrder.Order.Id == 0)
                        {
                            cartOrder.Order = await _orderService.InsertAsync(cartOrder.Order);
                        }
                        else
                        {
                            cartOrder.Order = await _orderService.UpdateAsync(cartOrder.Order);
                        }
                        cart.IdDiscount = null;
                        cart.GiftCertificates.Clear();
                        cart.Skus.Clear();
                    }
                    else
                    {
                        cart.IdDiscount = cartOrder.Order.Discount?.Id;
                        cart.GiftCertificates.MergeUpdateKeyed(cartOrder.Order.GiftCertificates, c => c.IdGiftCertificate,
                            co => co.GiftCertificate.Id,
                            co => new CartToGiftCertificate
                            {
                                Amount = co.Amount,
                                IdCart = cart.Id,
                                IdGiftCertificate = co.GiftCertificate.Id
                            }, (certificate, order) => certificate.Amount = order.Amount);
                        cart.Skus.MergeUpdateKeyed(cartOrder.Order.Skus, s => s.IdSku, so => so.Sku.Id, so => new CartToSku
                        {
                            Amount = so.Amount,
                            IdCart = cart.Id,
                            IdSku = so.Sku.Id,
                            Quantity = so.Quantity
                        }, (sku, ordered) => sku.Quantity = ordered.Quantity);
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

        public Task<OrderDataContext> CalculateCart(CustomerCartOrder cartOrder)
        {
            return _orderService.CalculateOrder(cartOrder.Order);
        }

        public Task<bool> SaveOrder(CustomerCartOrder cart)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetCartItemsCount(Guid uid)
        {
            var cart =
                await
                    _cartRepository.Query(c => c.CartUid == uid).SelectFirstOrDefaultAsync(false);
            if (cart != null)
            {
                if (cart.IdOrder != null)
                {
                    var skusOrdered = await _skuRepository.Query(s => s.IdOrder == cart.IdOrder.Value).SelectAsync(false);
                    return skusOrdered.Sum(s => s.Quantity);
                }
                var skus = await _skusRepository.Query(s => s.IdCart == cart.Id).SelectAsync(false);
                return skus.Sum(s => s.Quantity);
            }
            return 0;
        }
    }
}