using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Mail;
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
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Checkout;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Settings;

namespace VitalChoice.Business.Services.Checkout
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IEcommerceRepositoryAsync<CartExtended> _cartRepository;
        private readonly SkuMapper _skuMapper;
        private readonly ProductMapper _productMapper;
        private readonly OrderMapper _orderMapper;
        private readonly IOrderService _orderService;
        private readonly EcommerceContext _context;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly IEcommerceRepositoryAsync<CartToSku> _cartToSkusRepository;
        private readonly IEcommerceRepositoryAsync<Sku> _skuRepository;
        private readonly IDynamicReadServiceAsync<AddressDynamic, OrderAddress> _addressService;
        private readonly ICountryService _countryService;
        private readonly IEcommerceRepositoryAsync<OrderToSku> _orderToSkuRepository;
        private readonly IEcommerceRepositoryAsync<OrderToPromo> _promoOrderedRepository;
        private readonly IRepositoryAsync<ProductContent> _productContentRep;
        private readonly IEcommerceRepositoryAsync<Customer> _customerRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;

        public CheckoutService(IEcommerceRepositoryAsync<CartExtended> cartRepository,
            SkuMapper skuMapper, ProductMapper productMapper, OrderMapper orderMapper,
            IOrderService orderService, EcommerceContext context,
            ILoggerProviderExtended loggerProvider, ICustomerService customerService,
            IEcommerceRepositoryAsync<CartToSku> cartToSkusRepository,
            IDynamicReadServiceAsync<AddressDynamic, OrderAddress> addressService, ICountryService countryService,
            IEcommerceRepositoryAsync<OrderToSku> orderToSkuRepository, IRepositoryAsync<ProductContent> productContentRep,
            IEcommerceRepositoryAsync<Sku> skuRepository, IEcommerceRepositoryAsync<OrderToPromo> promoOrderedRepository,
            IEcommerceRepositoryAsync<Customer> customerRepository,
            INotificationService notificationService, IDiscountService discountService)
        {
            _cartRepository = cartRepository;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
            _orderMapper = orderMapper;
            _orderService = orderService;
            _context = context;
            _customerService = customerService;
            _cartToSkusRepository = cartToSkusRepository;
            _addressService = addressService;
            _countryService = countryService;
            _orderToSkuRepository = orderToSkuRepository;
            _productContentRep = productContentRep;
            _skuRepository = skuRepository;
            _promoOrderedRepository = promoOrderedRepository;
            _customerRepository = customerRepository;
            _notificationService = notificationService;
            _discountService = discountService;
            _logger = loggerProvider.CreateLogger<CheckoutService>();
        }

        public IQueryFluent<CartExtended> BuildIncludes(IQueryFluent<CartExtended> query)
        {
            return query
                .Include(c => c.GiftCertificates)
                .ThenInclude(g => g.GiftCertificate)
                .Include(c => c.Skus)
                .ThenInclude(s => s.Sku)
                .ThenInclude(s => s.OptionValues)
                .Include(c => c.Skus)
                .ThenInclude(s => s.Sku)
                .ThenInclude(s => s.Product)
                .ThenInclude(p => p.OptionValues)
                .Include(c => c.Skus)
                .ThenInclude(s => s.Sku)
                .ThenInclude(s => s.Product)
                .ThenInclude(p => p.ProductsToCategories);
        }

        public async Task<CustomerCartOrder> GetOrCreateCart(Guid? uid)
        {
            CartExtended cart;
            if (uid.HasValue)
            {
                var cartForCheck = await _cartRepository.Query(c => c.CartUid == uid.Value).SelectFirstOrDefaultAsync(false);
                if (cartForCheck?.IdCustomer != null && cartForCheck.IdOrder != null)
                {
                    return await GetOrCreateCart(uid, cartForCheck.IdCustomer.Value);
                }
                if (cartForCheck == null)
                {
                    cart = await CreateNew();
                }
                else
                {
                    cart = await BuildIncludes(_cartRepository.Query(c => c.CartUid == uid.Value)).SelectFirstOrDefaultAsync(false) ??
                           await CreateNew();
                }
            }
            else
            {
                cart = await CreateNew();
            }
            return await InitCartOrder(cart);
        }

        private async Task<CustomerCartOrder> InitCartOrder(CartExtended cart)
        {
            var newOrder = await _orderService.Mapper.CreatePrototypeAsync((int) OrderType.Normal);
            newOrder.Data.OrderType = (int) SourceOrderType.Web;
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
            }).ToList() ?? new List<GiftCertificateInOrder>();
            if (!string.IsNullOrEmpty(cart.DiscountCode))
            {
                newOrder.Discount = await _discountService.GetByCode(cart.DiscountCode);
            }
            newOrder.Skus = cart.Skus?.Select(s =>
            {
                s.Sku.OptionTypes =
                    _skuMapper.FilterByType(s.Sku.Product.IdObjectType);
                s.Sku.Product.OptionTypes = _productMapper.FilterByType(s.Sku.Product.IdObjectType);
                var productUrl = _productContentRep.Query(p => p.Id == s.Sku.IdProduct).Select(p => p.Url, false).FirstOrDefault();
                var sku = _skuMapper.FromEntity(s.Sku, true);
                sku.Product.Url = productUrl;
                return new SkuOrdered
                {
                    Amount = s.Sku.Price,
                    Sku = sku,
                    Quantity = s.Quantity
                };
            }).ToList() ?? new List<SkuOrdered>();
            newOrder.ShippingAddress = await _addressService.Mapper.CreatePrototypeAsync((int) AddressType.Shipping);
            newOrder.ShippingAddress.IdCountry = (await _countryService.GetCountriesAsync(new CountryFilter
            {
                CountryCode = "US"
            })).FirstOrDefault()?.Id ?? 0;
            if (cart.ShipDelayDate != null)
            {
                newOrder.Data.ShipDelayType = ShipDelayType.EntireOrder;
                newOrder.Data.ShipDelayDate = cart.ShipDelayDate;
            }
            newOrder.Data.ShippingUpgradeP = cart.ShippingUpgradeP;
            newOrder.Data.ShippingUpgradeNP = cart.ShippingUpgradeNP;
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
                    CartExtended cart;
                    if (uid.HasValue)
                    {
                        cart = await BuildIncludes(_cartRepository.Query(c => c.CartUid == uid.Value)).SelectFirstOrDefaultAsync(false);
                        if (cart != null)
                        {
                            if (cart.IdCustomer == null)
                            {
                                var dbCart = await _cartRepository.Query(c => c.CartUid == cart.CartUid).SelectFirstOrDefaultAsync(true);
                                dbCart.IdCustomer = idCustomer;
                                await _context.SaveChangesAsync();
                            }
                        }
                        else
                        {
                            cart = await CreateNew(idCustomer);
                        }
                        result = new CustomerCartOrder
                        {
                            CartUid = cart.CartUid
                        };
                    }
                    else
                    {
                        cart =
                            await BuildIncludes(_cartRepository.Query(c => c.IdCustomer == idCustomer)).SelectFirstOrDefaultAsync(false) ??
                            await CreateNew(idCustomer);
                        result = new CustomerCartOrder
                        {
                            CartUid = cart.CartUid
                        };
                    }
                    if (cart.IdOrder == null)
                    {
                        var anonymCart = await InitCartOrder(cart);
                        var customer = await _customerService.SelectAsync(idCustomer, true);
                        anonymCart.Order.Customer = customer;
                        anonymCart.Order = await _orderService.InsertAsync(anonymCart.Order);

                        var dbCart = await _cartRepository.Query(c => c.CartUid == cart.CartUid).SelectFirstOrDefaultAsync(true);
                        dbCart.IdOrder = anonymCart.Order.Id;
                        await _context.SaveChangesAsync();

                        anonymCart.Order.Customer = customer;
                        result.Order = anonymCart.Order;
                    }
                    else
                    {
                        result.Order = await _orderService.SelectAsync(cart.IdOrder.Value, true);
                        result.Order.Customer = await _customerService.SelectAsync(idCustomer, true);
                        var products = result.Order.Skus.Select(s => s.Sku.IdProduct).Distinct().ToList();
                        var skus = result.Order.Skus.Select(s => s.Sku.Id).ToList();
                        var productUrls =
                            await
                                _productContentRep.Query(p => products.Contains(p.Id)).Distinct().SelectAsync(false);
                        var skuAmounts =
                            (await
                                _skuRepository.Query(s => skus.Contains(s.Id))
                                    .Distinct()
                                    .SelectAsync(false)).ToDictionary(s => s.Id);
                        result.Order.Skus.UpdateKeyed(productUrls, s => s.Sku.IdProduct, arg => arg.Id,
                            (s, p) =>
                            {
                                s.Sku.Product.Url = p.Url;
                                s.Amount = result.Order.Customer.IdObjectType == (int)CustomerType.Wholesale
                                    ? skuAmounts[s.Sku.Id].WholesalePrice
                                    : skuAmounts[s.Sku.Id].Price;
                            });
                    }
                    transaction.Commit();
                }
                catch (AppValidationException)
                {
                    transaction.Rollback();
                    throw;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }

            return result;
        }

        private async Task<CartExtended> CreateNew(int? idCustomer = null)
        {
            var newUid = Guid.NewGuid();
            var cart = new CartExtended
            {
                CartUid = newUid,
                IdCustomer = idCustomer
            };
            await _cartRepository.InsertGraphAsync(cart);
            return await BuildIncludes(_cartRepository.Query(c => c.CartUid == newUid)).SelectFirstOrDefaultAsync(false);
        }

        public async Task<bool> UpdateCart(CustomerCartOrder cartOrder)
        {
            if (cartOrder?.Order == null)
                return false;
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    var cart =
                        await
                            _cartRepository.Query(c => c.CartUid == cartOrder.CartUid)
                                .Include(c => c.GiftCertificates)
                                .Include(c => c.Skus)
                                .SelectFirstOrDefaultAsync(true);

                    if (cart == null)
                    {
                        return false;
                    }
                    if (cartOrder.Order.Customer?.Id != 0)
                    {
                        var customerBackup = cartOrder.Order.Customer;

                        if (cartOrder.Order.Id == 0)
                        {
                            cartOrder.Order = await _orderService.InsertAsync(cartOrder.Order);
                        }
                        else
                        {
                            cartOrder.Order = await _orderService.UpdateAsync(cartOrder.Order);
                        }
                        cartOrder.Order.Customer = customerBackup;
                        cart.IdCustomer = cartOrder.Order?.Customer?.Id;
                        cart.IdOrder = cartOrder.Order?.Id;
                        cart.DiscountCode = null;
                        cart.GiftCertificates?.Clear();
                        cart.Skus?.Clear();
                        cart.ShipDelayDate = null;
                        cart.ShippingUpgradeNP = null;
                        cart.ShippingUpgradeP = null;
                    }
                    else
                    {
                        cart.DiscountCode = cartOrder.Order.Discount?.Code;
                        cart.GiftCertificates?.MergeKeyed(cartOrder.Order.GiftCertificates, c => c.IdGiftCertificate,
                            co => co.GiftCertificate.Id,
                            co => new CartToGiftCertificate
                            {
                                Amount = co.Amount,
                                IdCart = cart.Id,
                                IdGiftCertificate = co.GiftCertificate.Id
                            }, (certificate, order) => certificate.Amount = order.Amount);
                        cart.Skus?.MergeKeyed(cartOrder.Order.Skus, s => s.IdSku, so => so.Sku.Id, so => new CartToSku
                        {
                            Amount = so.Amount,
                            IdCart = cart.Id,
                            IdSku = so.Sku.Id,
                            Quantity = so.Quantity
                        }, (sku, ordered) => sku.Quantity = ordered.Quantity);
                        cart.ShipDelayDate = cartOrder.Order.SafeData.ShipDelayType == ShipDelayType.EntireOrder
                            ? cartOrder.Order.Data.ShipDelayDate
                            : null;
                        cart.ShippingUpgradeP = cartOrder.Order.SafeData.ShippingUpgradeP;
                        cart.ShippingUpgradeNP = cartOrder.Order.SafeData.ShippingUpgradeNP;
                        cart.IdCustomer = null;
                        cart.IdOrder = null;
                    }
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
                catch (AppValidationException)
                {
                    transaction.Rollback();
                    throw;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return true;
        }

        public async Task<bool> SaveOrder(CustomerCartOrder cartOrder)
        {
            if (cartOrder?.Order == null)
                return false;

            cartOrder.Order = (await _orderService.CalculateStorefrontOrder(cartOrder.Order, OrderStatus.Processed)).Order;
            //set needed key code for orders from storefront
            cartOrder.Order.Data.KeyCode = "WEB ORDER";
            using (var transaction = _context.BeginTransaction())
            {
                try
                {
                    var cart =
                        await
                            _cartRepository.Query(c => c.CartUid == cartOrder.CartUid)
                                .Include(c => c.GiftCertificates)
                                .Include(c => c.Skus)
                                .SelectFirstOrDefaultAsync(true);

                    if (cart == null)
                        return false;

                    var sendOrderConfirm = false;
                    if (cartOrder.Order.Customer?.Id != 0)
                    {
                        if (cartOrder.Order.Id == 0)
                        {
                            throw new ApiException("Order haven't been created during checkout session.");
                        }

                        sendOrderConfirm = true;
                        cartOrder.Order.Data.ConfirmationEmailSent = true;
                        cartOrder.Order.DateCreated = DateTime.Now;
                        cartOrder.Order = await _orderService.UpdateAsync(cartOrder.Order);
                        cart.IdCustomer = cartOrder.Order?.Customer?.Id;
                        cart.IdOrder = null;
                        cart.DiscountCode = null;
                        cart.GiftCertificates?.Clear();
                        cart.Skus?.Clear();
                        cart.ShipDelayDate = null;
                        cart.ShippingUpgradeNP = null;
                        cart.ShippingUpgradeP = null;
                    }
                    await _context.SaveChangesAsync();
                    transaction.Commit();

                    if (sendOrderConfirm && cartOrder.Order?.Customer != null)
                    {
                        var customer = await _customerRepository.Query(p => p.Id == cartOrder.Order.Customer.Id).SelectFirstOrDefaultAsync(false);
                        if (!string.IsNullOrEmpty(customer?.Email))
                        {
                            OrderDynamic mailOrder;
                            if (cartOrder.Order.IdObjectType == (int)OrderType.AutoShip)
                            {
                                var ids = await _orderService.SelectAutoShipOrdersAsync(cartOrder.Order.Id);

                                mailOrder = await _orderService.SelectAsync(ids.First());
                            }
                            else
                            {
                                mailOrder = cartOrder.Order;
                            }

                            var emailModel = await _orderMapper.ToModelAsync<OrderConfirmationEmail>(mailOrder);
                            if (emailModel != null)
                            {
                                await _notificationService.SendOrderConfirmationEmailAsync(customer.Email, emailModel);
                            }
                        }
                    }
                }
                catch (AppValidationException)
                {
                    transaction.Rollback();
                    throw;
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
            return true;
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
                    var skusOrdered = await _orderToSkuRepository.Query(s => s.IdOrder == cart.IdOrder.Value).SelectAsync(false);
                    var promoOrdered = await _promoOrderedRepository.Query(s => s.IdOrder == cart.IdOrder.Value).SelectAsync(false);
                    return skusOrdered.Sum(s => s.Quantity) + promoOrdered.Sum(p => p.Quantity);
                }
                var skus = await _cartToSkusRepository.Query(s => s.IdCart == cart.Id).SelectAsync(false);
                return skus.Sum(s => s.Quantity);
            }
            return 0;
        }
    }
}