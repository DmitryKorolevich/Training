using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Mailings;
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
            ILoggerFactory loggerProvider, ICustomerService customerService,
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

        public async Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, bool loggedIn)
        {
            CartExtended cart;
            if (uid.HasValue)
            {
                var cartForCheck = await _cartRepository.Query(c => c.CartUid == uid.Value).SelectFirstOrDefaultAsync(false);
                if (cartForCheck?.IdCustomer != null && cartForCheck.IdOrder != null)
                {
                    if (loggedIn)
                        return await GetOrCreateCart(uid, cartForCheck.IdCustomer.Value);

                    //Create new cart and treat customer as Retail by default
                    var oldCart = await GetOrCreateCart(uid, cartForCheck.IdCustomer.Value);
                    oldCart.Order.Customer = new CustomerDynamic {IdObjectType = (int) CustomerType.Retail};
                    var newUid = Guid.NewGuid();
                    var newCart = new CartExtended
                    {
                        CartUid = newUid,
                        IdCustomer = null
                    };
                    UpdateCartEntity(oldCart, newCart);
                    await _cartRepository.InsertGraphAsync(newCart);
                    return await GetOrCreateCart(newUid, false);
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
            return await InitCartOrderModel(cart);
        }

        public async Task<CustomerCartOrder> GetOrCreateCart(Guid? uid, int idCustomer)
        {
            CustomerCartOrder result;
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
                                var customerCart =
                                    await
                                        _cartRepository.Query(c => c.IdCustomer == idCustomer)
                                            .SelectFirstOrDefaultAsync(true);

                                var dbCart = await _cartRepository.Query(c => c.CartUid == cart.CartUid).SelectFirstOrDefaultAsync(true);

                                if (customerCart?.IdOrder != null)
                                {
                                    var cartDataSource = new CustomerCartOrder
                                    {
                                        CartUid = cart.CartUid,
                                        Order = await _orderService.SelectAsync(customerCart.IdOrder.Value, true)
                                    };
                                    cartDataSource.Order.Customer = await _customerService.SelectAsync(idCustomer, true);

                                    var currentCart = await InitCartOrderModel(cart);

                                    cartDataSource.CartUid = cart.CartUid;
                                    cartDataSource.Order.Skus = currentCart.Order.Skus;
                                    cartDataSource.Order.Discount = currentCart.Order.Discount;
                                    cartDataSource.Order.GiftCertificates = currentCart.Order.GiftCertificates;
                                    cartDataSource.Order.PromoSkus = currentCart.Order.PromoSkus;

                                    cart.IdCustomer = idCustomer;
                                    cart.IdOrder = customerCart.IdOrder;
                                    dbCart.IdCustomer = idCustomer;
                                    dbCart.IdOrder = customerCart.IdOrder;
                                    customerCart.IdOrder = null;
                                    customerCart.IdCustomer = null;

                                    await _context.SaveChangesAsync();
                                    cartDataSource.Order =
                                        (await _orderService.CalculateStorefrontOrder(cartDataSource.Order, OrderStatus.Incomplete)).Order;
                                    await _orderService.UpdateAsync(cartDataSource.Order);
                                }
                            }
                            else
                            {
                                if (cart.IdCustomer != idCustomer && cart.IdOrder != null)
                                {
                                    var currectOrder = await _orderService.SelectAsync(cart.IdOrder.Value, true);
                                    var newCart = await CreateNew(idCustomer);
                                    var newCartOrder = await InitCartOrderModel(newCart);
                                    newCartOrder.Order.Customer = await _customerService.SelectAsync(idCustomer, true);
                                    newCartOrder.Order.Skus = currectOrder.Skus;
                                    newCartOrder.Order.PromoSkus = currectOrder.PromoSkus;
                                    newCartOrder.Order.Discount = currectOrder.Discount;
                                    newCartOrder.Order.GiftCertificates = currectOrder.GiftCertificates;

                                    newCartOrder.Order =
                                        (await _orderService.CalculateStorefrontOrder(newCartOrder.Order, OrderStatus.Incomplete)).Order;

                                    newCartOrder.Order = await _orderService.InsertAsync(newCartOrder.Order);

                                    var dbCart =
                                        await _cartRepository.Query(c => c.CartUid == newCart.CartUid).SelectFirstOrDefaultAsync(true);
                                    dbCart.IdOrder = newCartOrder.Order.Id;
                                    await _context.SaveChangesAsync();

                                    transaction.Commit();

                                    await FillProductContentDetails(newCartOrder);

                                    return newCartOrder;
                                }
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
                        var anonymCart = await InitCartOrderModel(cart);
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
                        await FillProductContentDetails(result);
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
                    if (cartOrder.Order.Customer?.Id > 0)
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
                        UpdateCartEntity(cartOrder, cart);
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

            if (cartOrder.Order.IdObjectType == (int) OrderType.AutoShip &&
                !cartOrder.Order.Skus.Any(x => (bool?) x.Sku.SafeData.AutoShipProduct ?? false))
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AutoShipOrderShouldContainAutoShip]);
            }

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
                        var customer =
                            await _customerRepository.Query(p => p.Id == cartOrder.Order.Customer.Id).SelectFirstOrDefaultAsync(false);
                        if (!string.IsNullOrEmpty(customer?.Email))
                        {
                            OrderDynamic mailOrder;
                            if (cartOrder.Order.IdObjectType == (int) OrderType.AutoShip)
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

        private async Task<CustomerCartOrder> InitCartOrderModel(CartExtended cart)
        {
            var newOrder = _orderService.Mapper.CreatePrototype((int) OrderType.Normal);
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
                                var productUrl =
                                    _productContentRep.Query(p => p.Id == s.Sku.IdProduct).Select(p => p.Url, false).FirstOrDefault();
                                var sku = _skuMapper.FromEntity(s.Sku, true);
                                sku.Product.Url = productUrl;
                                return new SkuOrdered
                                {
                                    Amount = s.Sku.Price,
                                    Sku = sku,
                                    Quantity = s.Quantity
                                };
                            }).ToList() ?? new List<SkuOrdered>();
            newOrder.ShippingAddress = _addressService.Mapper.CreatePrototype((int) AddressType.Shipping);
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

        private async Task FillProductContentDetails(CustomerCartOrder result)
        {
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
                    s.Amount = result.Order.Customer.IdObjectType == (int) CustomerType.Wholesale
                        ? skuAmounts[s.Sku.Id].WholesalePrice
                        : skuAmounts[s.Sku.Id].Price;
                });
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

        private static void UpdateCartEntity(CustomerCartOrder cartOrder, CartExtended cart)
        {
            cart.DiscountCode = cartOrder.Order.Discount?.Code;
            if (cart.GiftCertificates == null)
            {
                cart.GiftCertificates = new List<CartToGiftCertificate>();
            }
            cart.GiftCertificates.MergeKeyed(cartOrder.Order.GiftCertificates, c => c.IdGiftCertificate,
                co => co.GiftCertificate.Id,
                co => new CartToGiftCertificate
                {
                    Amount = co.Amount,
                    IdCart = cart.Id,
                    IdGiftCertificate = co.GiftCertificate.Id
                }, (certificate, order) => certificate.Amount = order.Amount);
            if (cart.Skus == null)
            {
                cart.Skus = new List<CartToSku>();
            }
            cart.Skus.MergeKeyed(cartOrder.Order.Skus, s => s.IdSku, so => so.Sku.Id, so => new CartToSku
            {
                Amount = so.Amount,
                IdCart = cart.Id,
                IdSku = so.Sku.Id,
                Quantity = so.Quantity
            }, (sku, ordered) => sku.Quantity = ordered.Quantity);
            if (cartOrder.Order.SafeData.ShipDelayType != null &&
                (int) cartOrder.Order.SafeData.ShipDelayType == (int) ShipDelayType.EntireOrder)
            {
                cart.ShipDelayDate = cartOrder.Order.Data.ShipDelayDate;
            }
            else
            {
                cart.ShipDelayDate = cartOrder.Order.Data.ShipDelayDate = null;
            }
            cart.ShippingUpgradeP = cartOrder.Order.SafeData.ShippingUpgradeP;
            cart.ShippingUpgradeNP = cartOrder.Order.SafeData.ShippingUpgradeNP;
            cart.IdCustomer = null;
            cart.IdOrder = null;
        }
    }
}