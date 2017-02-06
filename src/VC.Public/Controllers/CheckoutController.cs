using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VC.Public.Helpers;
using VC.Public.Models.Auth;
using VC.Public.Models.Cart;
using VC.Public.Models.Checkout;
using VC.Public.Models.Profile;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Users;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using VC.Public.Models.Tracking;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Mailings;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Core.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Checkout;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;
using VitalChoice.Interfaces.Services.Affiliates;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.SharedWeb.Helpers;
using VitalChoice.Validation.Models;
using ApiException = VitalChoice.Ecommerce.Domain.Exceptions.ApiException;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;
using VitalChoice.ObjectMapping.Extensions;

namespace VC.Public.Controllers
{
    public class CheckoutController : CheckoutControllerBase
    {
        private const string CATALOG_PRODUCT_NAME = "pnc";

        private readonly IStorefrontUserService _userService;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _orderPaymentMethodConverter;
        private readonly IDynamicMapper<AddressDynamic, Address> _addressConverter;
        private readonly BrontoService _brontoService;
        private readonly ITransactionAccessor<EcommerceContext> _ecommerceTransactionAccessor;
        private readonly ITransactionAccessor<VitalChoiceContext> _vitalchoiceTransactionAccessor;
        private readonly IAffiliateService _affiliateService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly ICountryNameCodeResolver _countryNameCodeResolver;
        private readonly IEncryptedOrderExportService _exportService;
        private readonly ITokenService _tokenService;
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly IDiscountService _discountService;
        private readonly IGcService _gcService;

        public CheckoutController(IStorefrontUserService userService,
            ICustomerService customerService,
            IAffiliateService affiliateService,
            INotificationService notificationService,
            IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
            IOrderService orderService, IProductService productService,
            IAuthorizationService authorizationService, ICheckoutService checkoutService,
            IDynamicMapper<AddressDynamic, Address> addressConverter,
            IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> orderPaymentMethodConverter,
            IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper,
            BrontoService brontoService,
            ITransactionAccessor<EcommerceContext> ecommerceTransactionAccessor, ISettingService settingService,
            ILoggerFactory loggerProvider,
            ExtendedUserManager userManager, ICountryNameCodeResolver countryNameCodeResolver, ReferenceData referenceData,
            AppSettings appSettings, IEncryptedOrderExportService exportService, IObjectEncryptionHost encryptionHost,
            ITokenService tokenService, ITransactionAccessor<VitalChoiceContext> vitalchoiceTransactionAccessor,
            IDiscountService discountService,
            IGcService gcService)
            : base(
                customerService, referenceData, authorizationService, checkoutService, orderService,
                skuMapper, productMapper, settingService, userManager, appSettings)
        {
            _userService = userService;
            _paymentMethodConverter = paymentMethodConverter;
            _productService = productService;
            _addressConverter = addressConverter;
            _orderPaymentMethodConverter = orderPaymentMethodConverter;
            _brontoService = brontoService;
            _ecommerceTransactionAccessor = ecommerceTransactionAccessor;
            _countryNameCodeResolver = countryNameCodeResolver;
            _exportService = exportService;
            _encryptionHost = encryptionHost;
            _tokenService = tokenService;
            _vitalchoiceTransactionAccessor = vitalchoiceTransactionAccessor;
            _affiliateService = affiliateService;
            _notificationService = notificationService;
            _discountService = discountService;
            _gcService = gcService;
            _logger = loggerProvider.CreateLogger<CheckoutController>();
        }

        public async Task<IActionResult> Welcome(bool forgot = false)
        {
            if (await CustomerLoggedIn())
            {
                return RedirectToAction("AddUpdateShippingMethod");
            }

            ViewBag.ForgotPassSuccess = forgot;

            return View(new LoginModel());
        }

        [HttpGet]
        public async Task<Result<bool>> CheckCreditCard(int id)
        {
            if (await CustomerLoggedIn())
            {
                var internalId = GetInternalCustomerId();
                if (internalId.HasValue)
                {
                    return await CustomerService.GetCustomerCardExist(internalId.Value, id);
                }
            }
            return new Result<bool>(false);
        }

        [HttpPost]
        [CustomValidateAntiForgeryToken]
        public async Task<IActionResult> Welcome(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = null;
            try
            {
                user = await CustomerPasswordLogin(model);
                await HttpContext.SpinAuthorizationToken(_tokenService, null, user, _encryptionHost);
            }
            catch (WholesalePendingException)
            {
                return Redirect("/content/wholesale-review");
            }
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
            }

            return RedirectToAction("AddUpdateShippingMethod");
        }

        [HttpGet]
        [CustomerAuthorize]
        [CustomerStatusCheck]
        public async Task<IActionResult> GetBillingAddress(int id)
        {
            if (await CustomerLoggedIn())
            {
                if (id == 0)
                {
                    var cart = await GetCurrentCart();
                    var orderCard = cart?.Order?.PaymentMethod;

                    if (orderCard != null)
                    {
                        var billingInfoModel = await _addressConverter.ToModelAsync<AddUpdateBillingAddressModel>(orderCard.Address);
                        await _orderPaymentMethodConverter.UpdateModelAsync<BillingInfoModel>(billingInfoModel, orderCard);

                        return PartialView("_AddUpdateBillingAddress", billingInfoModel);
                    }

                    return PartialView("_AddUpdateBillingAddress", null);
                }

                var currentCustomer = await GetCurrentCustomerDynamic();

                CustomerPaymentMethodDynamic creditCard = currentCustomer.CustomerPaymentMethods
                    .FirstOrDefault(p => p.IdObjectType == (int)PaymentMethodType.CreditCard && p.Id == id);

                if (creditCard != null)
                {
                    var billingInfoModel = await _addressConverter.ToModelAsync<AddUpdateBillingAddressModel>(creditCard.Address);
                    await _paymentMethodConverter.UpdateModelAsync<BillingInfoModel>(billingInfoModel, creditCard);

                    return PartialView("_AddUpdateBillingAddress", billingInfoModel);
                }
            }
            return PartialView("_AddUpdateBillingAddress", null);
        }

        [HttpGet]
        public async Task<IActionResult> AddUpdateBillingAddress()
        {
            if (await IsCartEmpty())
            {
                return View("EmptyCart");
            }

            var addUpdateModel = new AddUpdateBillingAddressModel();
            var cart = await GetCurrentCart();
            var loggedIn = await EnsureLoggedIn(cart);

            if (loggedIn != null)
            {
                if (!loggedIn.Value)
                {
                    return RedirectToAction("AddUpdateBillingAddress");
                }

                if (cart.Order.ShippingAddress?.IdCountry == null)
                {
                    return RedirectToAction("AddUpdateShippingMethod");
                }

                var currentCustomer = cart.Order.Customer;

                if (cart.Order.PaymentMethod?.Address != null && cart.Order.PaymentMethod.Id != 0)
                {
                    await _addressConverter.UpdateModelAsync(addUpdateModel, cart.Order.PaymentMethod.Address);
                    await _orderPaymentMethodConverter.UpdateModelAsync<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                    addUpdateModel.Id = 0;
                }
                else
                {
                    var creditCards = currentCustomer.CustomerPaymentMethods
                        .Where(p => p.IdObjectType == (int)PaymentMethodType.CreditCard).ToList();

                    var firstCreditCard = creditCards.FirstOrDefault(x => (bool?)x.SafeData.Default == true) ??
                                          creditCards.FirstOrDefault();

                    if (firstCreditCard != null)
                    {
                        await _addressConverter.UpdateModelAsync(addUpdateModel, firstCreditCard.Address);
                        await _paymentMethodConverter.UpdateModelAsync<BillingInfoModel>(addUpdateModel, firstCreditCard);

                        addUpdateModel.Id = firstCreditCard.Id;
                    }
                }

                if (addUpdateModel.IdCountry == 0)
                {
                    addUpdateModel.IdCountry = ReferenceData.DefaultCountry.Id;
                }

                ViewBag.CreditCards = PopulateCreditCardsLookup(currentCustomer, cart.Order);
            }
            else
            {
                return RedirectToAction("Welcome");
            }

            return View(addUpdateModel);
        }

        public async Task<Result<bool>> GetIsUnsubscribed()
        {
            var loggedIn = await CustomerLoggedIn();
            if (loggedIn)
            {
                var currentCustomer = await GetCurrentCustomerDynamic();
                var email = currentCustomer.Email;
                if (!string.IsNullOrEmpty(email))
                {
                    return await _brontoService.GetIsUnsubscribed(email) ?? false;
                }
            }
            return false;
        }

        [HttpPost]
        [CustomValidateAntiForgeryToken]
        public async Task<IActionResult> AddUpdateBillingAddress(AddUpdateBillingAddressModel model)
        {
            Guid? cartUid;
            using (await LockCurrentCart(out cartUid))
            {
                if (await IsCartEmpty())
                {
                    return View("EmptyCart");
                }

                var cart = await GetCurrentCart(withMultipleShipmentsService: true);
                if (cart.Order?.ShippingAddress?.IdCountry == null)
                {
                    return RedirectToAction("AddUpdateShippingMethod");
                }

                var loggedIn = await EnsureLoggedIn(cart);
                if (!loggedIn.HasValue)
                {
                    return RedirectToAction("AddUpdateBillingAddress");
                }
                if (!loggedIn.Value)
                {
                    return RedirectToAction("Welcome");
                }

                if (IsCanadaShippingIssue(cart.Order.Customer, cart.Order))
                {
                    return RedirectToAction("AddUpdateShippingMethod", new { canadaissue = true });
                }

                using (await CartLocks.LockWhenAsync(() => cartUid.HasValue && cartUid.Value != cart.CartUid, () => cart.CartUid))
                {
                    if (!ModelState.IsValid)
                    {
                        ViewBag.CreditCards = PopulateCreditCardsLookup(cart.Order.Customer);
                        return View(model);
                    }
                    using (await OrderLocks.LockWhenAsync(() => cart.Order?.Id > 0, () => cart.Order.Id))
                    {
                        using (var transaction = _ecommerceTransactionAccessor.BeginTransaction())
                        {
                            try
                            {
                                ViewBag.CreditCards = PopulateCreditCardsLookup(cart.Order.Customer);

                                if (cart.Order.PaymentMethod?.Address == null || cart.Order.PaymentMethod.Id == 0)
                                {
                                    cart.Order.PaymentMethod =
                                        await _orderPaymentMethodConverter.FromModelAsync(model, (int)PaymentMethodType.CreditCard);
                                    cart.Order.PaymentMethod.Address =
                                        await _addressConverter.FromModelAsync(model, (int)AddressType.Billing);
                                }
                                else
                                {
                                    var orderPaymentId = cart.Order.PaymentMethod.Id;
                                    await
                                        _orderPaymentMethodConverter.UpdateObjectAsync(model, cart.Order.PaymentMethod,
                                            (int)PaymentMethodType.CreditCard);
                                    cart.Order.PaymentMethod.Id = orderPaymentId;
                                    var idAddress = 0;
                                    if (cart.Order.PaymentMethod.Address == null)
                                    {
                                        cart.Order.PaymentMethod.Address = new AddressDynamic { IdObjectType = (int)AddressType.Billing };
                                    }
                                    else
                                    {
                                        idAddress = cart.Order.PaymentMethod.Address.Id;
                                    }
                                    await
                                        _addressConverter.UpdateObjectAsync(model, cart.Order.PaymentMethod.Address,
                                            (int)AddressType.Billing);
                                    cart.Order.PaymentMethod.Address.Id = idAddress;
                                }
                                if (ObjectMapper.IsValuesMasked(typeof(OrderPaymentMethodDynamic),
                                    (string)cart.Order.PaymentMethod.Data.CardNumber,
                                    "CardNumber"))
                                {
                                    if (!await CustomerService.GetCustomerCardExist(cart.Order.Customer.Id, model.Id))
                                    {
                                        ModelState.AddModelError(string.Empty,
                                            "For security reasons. Please enter all credit card details for this card or please select a new one to continue.");

                                        return View(model);
                                    }
                                    if (model.Id > 0)
                                    {
                                        if (cart.Order.Customer.CustomerPaymentMethods?.All(p => p.Id != model.Id) ?? true)
                                        {
                                            ModelState.AddModelError(string.Empty,
                                                "For security reasons. Please enter all credit card details for this card or please select a new one to continue.");

                                            return View(model);
                                        }
                                        cart.Order.PaymentMethod.IdCustomerPaymentMethod = model.Id;
                                    }
                                }
                                else
                                {
                                    if (!await CustomerService.GetCustomerCardExist(cart.Order.Customer.Id, model.Id))
                                    {
                                        await _exportService.UpdateCustomerPaymentMethodsAsync(new List<CustomerCardData>
                                            {
                                                new CustomerCardData
                                                {
                                                    CardNumber = (string) cart.Order.PaymentMethod.Data.CardNumber,
                                                    IdCustomer = cart.Order.Customer.Id,
                                                    IdPaymentMethod = model.Id
                                                }
                                            });
                                    }
                                }
                                if (!await CheckoutService.UpdateCart(cart, withMultipleShipmentsService: true))
                                {
                                    ModelState.AddModelError(string.Empty, "Cannot update order");
                                    //if (loginTask != null)
                                    //{
                                    //    if (cart.Order.Customer != null && cart.Order.Customer.Id != 0)
                                    //    {
                                    //        await _userService.RemoveAsync(cart.Order.Customer.Id);
                                    //    }
                                    //}
                                    transaction.Rollback();
                                }
                                else
                                {
                                    transaction.Commit();
                                    return RedirectToAction("ReviewOrder");
                                }
                            }
                            catch (AppValidationException e)
                            {
                                transaction.Rollback();
                                var newMessages = e.Messages.Select(error => new MessageInfo
                                {
                                    Field = string.Empty,
                                    Message = error.Message,
                                    MessageLevel = error.MessageLevel
                                });
                                //if (loginTask != null)
                                //{
                                //    if (cart.Order.Customer != null && cart.Order.Customer.Id != 0)
                                //    {
                                //        var user = await _userService.GetAsync(cart.Order.Customer.Id);
                                //        user.Status = UserStatus.NotActive;
                                //        await _userService.UpdateAsync(user);
                                //    }
                                //}
                                throw new AppValidationException(newMessages);
                            }
                            catch
                            {
                                transaction.Rollback();
                                //if (loginTask != null)
                                //{
                                //    if (cart.Order.Customer != null && cart.Order.Customer.Id != 0)
                                //    {
                                //        var user = await _userService.GetAsync(cart.Order.Customer.Id);
                                //        user.Status = UserStatus.NotActive;
                                //        await _userService.UpdateAsync(user);
                                //    }
                                //}
                                throw;
                            }
                        }
                    }

                    return View(model);
                }
            }
        }

        #region NewStep3

        [HttpGet]
        public IActionResult AddUpdateShippingMethod(bool? canadaissue = false)
        {
            return View();
        }

        [HttpGet]
        [CustomerStatusCheck]
        public async Task<Result<UpdateShipmentsModel>> GetShipments()
        {
            if (await IsCartEmpty())
            {
                return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("EmptyCart", "Cart"));
            }

            var cart = await GetCurrentCart(withMultipleShipmentsService: true);

            var toReturn = new UpdateShipmentsModel();

            var loggedIn = await CustomerLoggedIn();
            if (loggedIn)
            {
                toReturn.Email = cart.Order.Customer.Email;
                toReturn.IdCustomerType = cart.Order.Customer.IdObjectType;
            }
            else
            {
                toReturn.CreateAccount = true;
                toReturn.IdCustomerType = (int)CustomerType.Retail;
            }

            var pnc = await _productService.GetSkuAsync(CATALOG_PRODUCT_NAME);
            if (pnc != null)
            {
                var pncModel = await SkuMapper.ToModelAsync<CartSkuModel>(pnc);
                toReturn.ShowSendCatalog = pncModel.InStock;
                toReturn.SendCatalog = pncModel.InStock;
            }

            toReturn.AllowAddMultipleShipments = cart.Order.IdObjectType != (int)OrderType.AutoShip;

            //order part
            var currentCustomer = cart.Order.Customer;
            var shippingMethodModel = new ShippingAddressModel()
            {
                AddressType = ShippingAddressType.Residential,
                FromOrder = true
            };

            var defaultShipping = currentCustomer?.ShippingAddresses.FirstOrDefault(x => (bool?)x.SafeData.Default == true);
            if (cart.Order.ShippingAddress != null && cart.Order.ShippingAddress.Id != 0 &&
                !string.IsNullOrEmpty(cart.Order.ShippingAddress.SafeData.FirstName))
            {
                await _addressConverter.UpdateModelAsync(shippingMethodModel, cart.Order.ShippingAddress);
            }
            else if (defaultShipping != null)
            {
                await _addressConverter.UpdateModelAsync(shippingMethodModel, defaultShipping);
            }

            if (shippingMethodModel.IdCountry == 0)
            {
                shippingMethodModel.IdCountry = ReferenceData.DefaultCountry.Id;
            }

            shippingMethodModel.IsGiftOrder = cart.Order.SafeData.GiftOrder;
            shippingMethodModel.GiftMessage = cart.Order.SafeData.GiftMessage;
            shippingMethodModel.PreferredShipMethodName = shippingMethodModel.PreferredShipMethod.HasValue ?
                ReferenceData.OrderPreferredShipMethod.FirstOrDefault(x => x.Key == (int)shippingMethodModel.PreferredShipMethod.Value)?.Text
                : null;
            toReturn.Shipments.Add(shippingMethodModel);

            var addresses = GetShippingAddresses(cart.Order, currentCustomer);
            var i = 0;
            foreach (var addressData in addresses)
            {
                var item = new AvalibleShippingAddressModel();
                item.Address = new ShippingAddressModel();
                item.Name = $"{addressData.Value.SafeData.FirstName} {addressData.Value.SafeData.LastName} " +
                            $"{addressData.Value.SafeData.Address1} {addressData.Key}";
                await _addressConverter.UpdateModelAsync(item.Address, addressData.Value);
                item.Address.FromOrder = item.Address.Id == cart.Order?.ShippingAddress?.Id;
                item.Address.IdCustomerShippingAddress = item.Address.Id == cart.Order?.ShippingAddress?.Id ? (int?)null : item.Address.Id;
                item.Address.Id = null;
                item.Address.PreferredShipMethodName = item.Address.PreferredShipMethod.HasValue
                    ? ReferenceData.OrderPreferredShipMethod.FirstOrDefault(
                        x => x.Key == (int)item.Address.PreferredShipMethod.Value)?.Text
                        : null;

                toReturn.AvalibleAddresses.Add(item);
            }

            //addiotional shipments
            if (cart.Order?.CartAdditionalShipments != null)
            {
                foreach (var cartAdditionalShipmentModelItem in cart.Order.CartAdditionalShipments)
                {
                    shippingMethodModel = new ShippingAddressModel();
                    await _addressConverter.UpdateModelAsync(shippingMethodModel, cartAdditionalShipmentModelItem.ShippingAddress);
                    shippingMethodModel.IdShipment = cartAdditionalShipmentModelItem.Id;
                    shippingMethodModel.IsGiftOrder = cartAdditionalShipmentModelItem.IsGiftOrder;
                    shippingMethodModel.GiftMessage = cartAdditionalShipmentModelItem.GiftMessage;
                    shippingMethodModel.PreferredShipMethodName = shippingMethodModel.PreferredShipMethod.HasValue ?
                        ReferenceData.OrderPreferredShipMethod.FirstOrDefault(x => x.Key == (int)shippingMethodModel.PreferredShipMethod.Value)?.Text
                        : null;
                    toReturn.Shipments.Add(shippingMethodModel);
                }
            }

            return toReturn;
        }

        [HttpPost]
        [CustomerStatusCheck]
        public async Task<Result<UpdateShipmentsModel>> UpdateShipments([FromBody]UpdateShipmentsModel model)
        {
            if (!Validate(model))
            {
                return null;
            }

            Guid? cartUid;
            using (await LockCurrentCart(out cartUid))
            {
                if (await IsCartEmpty())
                {
                    return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("EmptyCart", "Cart"));
                }

                var loggedIn = await CustomerLoggedIn();

                var mainShipment = model.Shipments.FirstOrDefault(p => p.FromOrder);
                if (mainShipment == null)
                {
                    throw new AppValidationException("Main order shipping isn't specified");
                }

                var cart = await GetCurrentCart(withMultipleShipmentsService: true);
                using (
                    await
                        CartLocks.LockWhenAsync(() => cartUid.HasValue && cartUid.Value != cart.CartUid,
                            () => cart.CartUid))
                {
                    using (await OrderLocks.LockWhenAsync(() => cart.Order?.Id > 0, () => cart.Order.Id))
                    {

                        using (var userTransaction = _vitalchoiceTransactionAccessor.BeginTransaction())
                        {
                            using (var transaction = _ecommerceTransactionAccessor.BeginTransaction())
                            {
                                try
                                {
                                    Func<Task<ApplicationUser>> loginTask = null;
                                    if (!loggedIn)
                                    {
                                        var createResult = await EnsureCustomerCreated(model, mainShipment, cart.Order.Customer);
                                        cart.Order.Customer = createResult.Customer;
                                        loginTask = createResult.LoginTask;
                                    }

                                    //save main shipping to an order
                                    if (cart.Order.ShippingAddress == null || cart.Order.ShippingAddress.Id == 0)
                                    {
                                        if (mainShipment.UseBillingAddress)
                                        {
                                            cart.Order.ShippingAddress = await
                                                    _addressConverter.FromModelAsync(mainShipment,
                                                        (int)AddressType.Shipping);
                                            var billingMapped = await
                                                    _addressConverter.ToModelAsync<AddUpdateBillingAddressModel>(
                                                        cart.Order.PaymentMethod.Address);
                                            await _addressConverter.UpdateObjectAsync(billingMapped,
                                                    cart.Order.ShippingAddress);
                                        }
                                        else
                                        {
                                            cart.Order.ShippingAddress = await
                                                    _addressConverter.FromModelAsync(mainShipment,
                                                        (int)AddressType.Shipping);
                                        }
                                        cart.Order.ShippingAddress.Id = 0;
                                    }
                                    else
                                    {
                                        if (mainShipment.UseBillingAddress)
                                        {
                                            var oldId = cart.Order.ShippingAddress.Id;
                                            await _addressConverter.UpdateObjectAsync(mainShipment,
                                                    cart.Order.ShippingAddress,
                                                    (int)AddressType.Shipping);
                                            var billingMapped = await
                                                    _addressConverter.ToModelAsync<AddUpdateBillingAddressModel>(
                                                        cart.Order.PaymentMethod.Address);
                                            await _addressConverter.UpdateObjectAsync(billingMapped,
                                                    cart.Order.ShippingAddress);
                                            cart.Order.ShippingAddress.Id = oldId;
                                        }
                                        else
                                        {
                                            var oldId = cart.Order.ShippingAddress.Id;
                                            await _addressConverter.UpdateObjectAsync(mainShipment,
                                                    cart.Order.ShippingAddress,
                                                    (int)AddressType.Shipping);
                                            cart.Order.ShippingAddress.Id = oldId;
                                        }
                                    }
                                    cart.Order.Data.GiftOrder = mainShipment.IsGiftOrder;
                                    if (mainShipment.IsGiftOrder)
                                    {
                                        cart.Order.Data.GiftMessage = mainShipment.GiftMessage;
                                    }
                                    if (mainShipment.UseBillingAddress)
                                    {
                                        cart.Order.ShippingAddress.Data.PreferredShipMethod = PreferredShipMethod.Best;
                                    }

                                    //customer addresses for update
                                    Dictionary<int, AddressDynamic> customerShippingAddressForUpdate =
                                        new Dictionary<int, AddressDynamic>();
                                    if (mainShipment.SaveToProfile && mainShipment.IdCustomerShippingAddress.HasValue)
                                    {
                                        customerShippingAddressForUpdate.Add(
                                            mainShipment.IdCustomerShippingAddress.Value, cart.Order.ShippingAddress);
                                    }

                                    //update additional shipments
                                    var i = 2;
                                    var dbShipments = cart.Order.CartAdditionalShipments ?? new List<CartAdditionalShipmentModelItem>();
                                    cart.Order.CartAdditionalShipments = new List<CartAdditionalShipmentModelItem>();
                                    foreach (var modelShipment in model.Shipments.Where(p => !p.FromOrder))
                                    {
                                        CartAdditionalShipmentModelItem item =
                                            dbShipments.FirstOrDefault(p => p.Id == modelShipment.IdShipment);
                                        if (item == null)
                                        {
                                            item = new CartAdditionalShipmentModelItem
                                            {
                                                ShippingAddress = new AddressDynamic()
                                            };
                                        }

                                        await _addressConverter.UpdateObjectAsync(modelShipment, item.ShippingAddress,
                                            (int)AddressType.Shipping);
                                        item.Id = modelShipment.IdShipment;
                                        item.Name = $"Order #{i}";
                                        item.IsGiftOrder = modelShipment.IsGiftOrder;
                                        item.GiftMessage = modelShipment.GiftMessage;

                                        if (modelShipment.SaveToProfile &&
                                            modelShipment.IdCustomerShippingAddress.HasValue)
                                        {
                                            if (!customerShippingAddressForUpdate.ContainsKey(
                                                    modelShipment.IdCustomerShippingAddress.Value))
                                            {
                                                customerShippingAddressForUpdate.Add(
                                                    modelShipment.IdCustomerShippingAddress.Value, item.ShippingAddress);
                                            }
                                        }
                                        i++;

                                        cart.Order.CartAdditionalShipments.Add(item);
                                    }

                                    //Move all skus on the removed shipments to the main order
                                    foreach (var shipmentForRemove in dbShipments.Where(p => !cart.Order.CartAdditionalShipments.Contains(p)))
                                    {
                                        if (shipmentForRemove.Skus?.Count > 0)
                                        {
                                            cart.Order.Skus.AddUpdateKeyed(shipmentForRemove.Skus, p => p.Sku.Id,
                                                (d, s) =>
                                                {
                                                    d.Quantity += s.Quantity;
                                                });
                                        }
                                    }

                                    if (model.SendCatalog)
                                    {
                                        var pnc = await _productService.GetSkuOrderedAsync(CATALOG_PRODUCT_NAME);

                                        if (pnc != null && pnc.Sku.InStock())
                                        {
                                            var exist = cart.Order.Skus.Any(p => p.Sku.Code.ToLower() == CATALOG_PRODUCT_NAME);
                                            exist = exist || cart.Order.CartAdditionalShipments.SelectMany(p => p.Skus).Any(p => p.Sku.Code.ToLower() == CATALOG_PRODUCT_NAME);

                                            if (!exist)
                                            {
                                                pnc.Quantity = 1;
                                                pnc.Amount = pnc.Sku.Price;
                                                cart.Order.Skus.AddKeyed(Enumerable.Repeat(pnc, 1), ordered => ordered.Sku.Code);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        cart.Order.Skus.RemoveAll(s => s.Sku.Code.ToLower() == CATALOG_PRODUCT_NAME);
                                    }

                                    var statistic = await CustomerService.GetCustomerOrderStatistics(new[] { cart.Order.Customer.Id });
                                    //new customer - add welcome letter
                                    if (statistic.FirstOrDefault() == null)
                                    {
                                        var wl = await _productService.GetSkuOrderedAsync(ProductConstants.WELCOME_LETTER_SKU);
                                        if (wl != null && wl.Sku.InStock())
                                        {
                                            var exist = cart.Order.Skus.Any(p => p.Sku.Code.ToLower() == ProductConstants.WELCOME_LETTER_SKU);
                                            exist = exist || cart.Order.CartAdditionalShipments.SelectMany(p => p.Skus).
                                                Any(p => p.Sku.Code.ToLower() == ProductConstants.WELCOME_LETTER_SKU);

                                            if (!exist)
                                            {
                                                wl.Quantity = 1;
                                                wl.Amount = wl.Sku.Price;
                                                cart.Order.Skus.AddKeyed(Enumerable.Repeat(wl, 1), ordered => ordered.Sku.Code);
                                            }
                                        }
                                    }

                                    await OrderService.CalculateStorefrontOrder(cart.Order, OrderStatus.Incomplete);
                                    if (await CheckoutService.UpdateCart(cart, true))
                                    {
                                        var shippingAddresses = cart.Order.Customer.ShippingAddresses.ToList();
                                        var update = false;
                                        foreach (var itemForUpdate in customerShippingAddressForUpdate)
                                        {
                                            var customerShippinhAddress =
                                                shippingAddresses.FirstOrDefault(x => x.Id == itemForUpdate.Key);
                                            if (customerShippinhAddress != null)
                                            {
                                                var index = shippingAddresses.IndexOf(customerShippinhAddress);
                                                var originalId = shippingAddresses[index].Id;
                                                var defaultAddr = (bool?)shippingAddresses[index].SafeData.Default ??
                                                                  false;
                                                shippingAddresses[index] = itemForUpdate.Value;
                                                shippingAddresses[index].Id = originalId;
                                                shippingAddresses[index].Data.Default = defaultAddr;

                                                update = true;
                                            }
                                        }
                                        if (update)
                                        {
                                            cart.Order.Customer.ShippingAddresses = shippingAddresses;
                                            cart.Order.Customer = await CustomerService.UpdateAsync(cart.Order.Customer);
                                        }

                                        if (cart.Order.Customer.ShippingAddresses.Count == 0)
                                        {
                                            cart.Order.ShippingAddress.Id = 0;
                                            cart.Order.ShippingAddress.Data.Default = true;
                                            cart.Order.Customer.ShippingAddresses = new List<AddressDynamic>
                                            {
                                                cart.Order.ShippingAddress
                                            };

                                            cart.Order.Customer = await CustomerService.UpdateAsync(cart.Order.Customer);
                                        }

                                        if (IsCanadaShippingIssue(cart.Order.Customer, cart.Order))
                                        {
                                            return
                                                GetJsonRedirect<UpdateShipmentsModel>(
                                                    Url.Action("AddUpdateShippingMethod", new { canadaissue = true }));
                                        }

                                        if (!string.IsNullOrEmpty(model.Email))
                                        {
                                            _brontoService.PushSubscribe(model.Email, model.SendNews);
                                        }

                                        if (loginTask != null)
                                            await loginTask();
                                        transaction.Commit();
                                        userTransaction.Commit();

                                        if (HttpContext.Request.Query.ContainsKey("mode") &&
                                            HttpContext.Request.Query["mode"] == "review")
                                        {
                                            return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("ReviewOrder"), true);
                                        }
                                        else
                                        {
                                            return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("AddUpdateBillingAddress"), true);
                                        }
                                    }
                                    else
                                    {
                                        userTransaction.Rollback();
                                        transaction.Rollback();
                                    }

                                    return model;
                                }
                                catch (AppValidationException e)
                                {
                                    transaction.Rollback();
                                    userTransaction.Rollback();
                                    var newMessages = e.Messages.Select(error => new MessageInfo
                                    {
                                        Field = string.Empty,
                                        Message = error.Message,
                                        MessageLevel = error.MessageLevel
                                    });
                                    //if (loginTask != null)
                                    //{
                                    //    if (cart.Order.Customer != null && cart.Order.Customer.Id != 0)
                                    //    {
                                    //        var user = await _userService.GetAsync(cart.Order.Customer.Id);
                                    //        user.Status = UserStatus.NotActive;
                                    //        await _userService.UpdateAsync(user);
                                    //    }
                                    //}
                                    throw new AppValidationException(newMessages);
                                }
                                catch
                                {
                                    transaction.Rollback();
                                    userTransaction.Rollback();
                                    //if (loginTask != null)
                                    //{
                                    //    if (cart.Order.Customer != null && cart.Order.Customer.Id != 0)
                                    //    {
                                    //        var user = await _userService.GetAsync(cart.Order.Customer.Id);
                                    //        user.Status = UserStatus.NotActive;
                                    //        await _userService.UpdateAsync(user);
                                    //    }
                                    //}
                                    throw;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region NewStep4

        [HttpGet]
        public IActionResult ReviewOrder()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> MoveSku()
        {
            return PartialView("_MoveSku");
        }

        [HttpGet]
        public async Task<IActionResult> CopySku()
        {
            return PartialView("_CopySku");
        }

        [HttpGet]
        [CustomerStatusCheck]
        public async Task<Result<MultipleOrdersReviewModel>> OrderReviewInitCartModel()
        {
            if (await IsCartEmpty())
            {
                return GetJsonRedirect<MultipleOrdersReviewModel>(Url.Action("EmptyCart", "Cart"));
            }
            var cart = await GetCurrentCart(withMultipleShipmentsService: true);
            if (cart.Order.ShippingAddress?.IdCountry == null)
            {
                return GetJsonRedirect<MultipleOrdersReviewModel>(Url.Action("AddUpdateShippingMethod", "Checkout"));
            }
            if (cart.Order.PaymentMethod?.Address?.IdCountry == null)
            {
                return GetJsonRedirect<MultipleOrdersReviewModel>(Url.Action("AddUpdateBillingAddress", "Checkout"));
            }

            var loggedIn = await EnsureLoggedIn(cart);
            if (loggedIn.HasValue)
            {
                if (!loggedIn.Value)
                {
                    return GetJsonRedirect<MultipleOrdersReviewModel>(Url.Action("ReviewOrder", "Checkout"));
                }
            }

            var toReturn = new MultipleOrdersReviewModel();
            await OrderReviewInitCartModelInternal(toReturn, cart);

            if (!ModelState.IsValid)
            {
                return ResultHelper.CreateErrorResult(ModelState, toReturn);
            }
            return toReturn;
        }

        private async Task OrderReviewInitCartModelInternal(MultipleOrdersReviewModel toReturn, CustomerCartOrder cart)
        {
            if (toReturn.GiftCertificateCodes.Count == 0)
            {
                toReturn.GiftCertificateCodes.Add(new CartGcModel() { Value = string.Empty });
            }
            toReturn.AutoShip = cart.Order.IdObjectType == (int)OrderType.AutoShip;
            var paymentMethod = cart.Order.PaymentMethod;
            toReturn.BillToAddress = paymentMethod.Address.PopulateBillingAddressDetails(_countryNameCodeResolver,
                cart.Order.Customer.Email);
            toReturn.CreditCardDetails = paymentMethod.PopulateCreditCardDetails(ReferenceData);

            var reviewModel = new ReviewUpdateOrderModel();
            reviewModel.Name = "Order #1";
            reviewModel.Main = true;
            reviewModel.OrderModel = new MultipleOrdersViewCartModel();
            PopulateReviewModel(reviewModel, cart.Order);

            var context = await OrderService.CalculateStorefrontOrder(cart.Order, OrderStatus.Incomplete);
            await FillModel(reviewModel.OrderModel, cart.Order, context, "i0.");
            if (reviewModel.OrderModel.GiftCertificateCodes.Count == 0)
            {
                reviewModel.OrderModel.GiftCertificateCodes.Add(new CartGcModel() { Value = string.Empty });
            }
            toReturn.Shipments.Add(reviewModel);

            var index = 1;
            foreach (var cartAdditionalShipmentModelItem in cart.Order.CartAdditionalShipments)
            {
                var order = OrderService.CreateNewNormalOrder(OrderStatus.Incomplete);
                order.ShippingAddress = cartAdditionalShipmentModelItem.ShippingAddress;
                order.Skus = cartAdditionalShipmentModelItem.Skus;
                order.PaymentMethod = cart.Order.PaymentMethod;
                order.Customer = cart.Order.Customer;
                order.Data.GiftMessage = cartAdditionalShipmentModelItem.IsGiftOrder ?
                    cartAdditionalShipmentModelItem.GiftMessage :
                    string.Empty;
                order.Data.ShipDelayType = cartAdditionalShipmentModelItem.ShipDelayDate.HasValue ? ShipDelayType.EntireOrder : ShipDelayType.None;
                order.Data.ShipDelayDate = cartAdditionalShipmentModelItem.ShipDelayDate;
                order.Data.ShippingUpgradeP = (ShippingUpgradeOption?)cartAdditionalShipmentModelItem.ShippingUpgradeP;
                order.Data.ShippingUpgradeNP = (ShippingUpgradeOption?)cartAdditionalShipmentModelItem.ShippingUpgradeNP;
                if (!string.IsNullOrEmpty(cartAdditionalShipmentModelItem.DiscountCode))
                {
                    order.Discount = await _discountService.GetByCode(cartAdditionalShipmentModelItem.DiscountCode);
                }
                if (cartAdditionalShipmentModelItem.GiftCertificateIds != null)
                {
                    var gcs = await _gcService.GetGiftCertificatesAsync(cartAdditionalShipmentModelItem.GiftCertificateIds);
                    order.GiftCertificates = gcs.Select(p => new GiftCertificateInOrder()
                    {
                        GiftCertificate = p,
                    }).ToList();
                }

                reviewModel = new ReviewUpdateOrderModel();
                reviewModel.IdShipment = cartAdditionalShipmentModelItem.Id;
                reviewModel.Name = cartAdditionalShipmentModelItem.Name;
                reviewModel.OrderModel = new MultipleOrdersViewCartModel();
                PopulateReviewModel(reviewModel, order);

                context = await OrderService.CalculateStorefrontOrder(order, OrderStatus.Incomplete);
                await FillModel(reviewModel.OrderModel, order, context, $"i{index}.");

                if (reviewModel.OrderModel.GiftCertificateCodes.Count == 0)
                {
                    reviewModel.OrderModel.GiftCertificateCodes.Add(new CartGcModel() { Value = string.Empty });
                }
                toReturn.Shipments.Add(reviewModel);
                index++;
            }
            HttpContext.SetCartUid(cart.CartUid);
        }

        private void PopulateReviewModel(ReviewUpdateOrderModel reviewOrderModel, OrderDynamic order)
        {
            var paymentMethod = order.PaymentMethod;
            reviewOrderModel.BillToAddress = paymentMethod.Address.PopulateBillingAddressDetails(_countryNameCodeResolver,
                order.Customer.Email);
            reviewOrderModel.CreditCardDetails = paymentMethod.PopulateCreditCardDetails(ReferenceData);

            var shippingAddress = order.ShippingAddress;
            reviewOrderModel.ShipToAddress = shippingAddress.PopulateShippingAddressDetails(_countryNameCodeResolver);
            reviewOrderModel.ShipToFirstName = shippingAddress.SafeData.FirstName;
            reviewOrderModel.ShipToLastName = shippingAddress.SafeData.LastName;
            reviewOrderModel.ShipToAddress1 = shippingAddress.SafeData.Address1;

            reviewOrderModel.DeliveryInstructions = shippingAddress.SafeData.DeliveryInstructions;
            reviewOrderModel.GiftMessage = order.SafeData.GiftMessage;
        }

        [HttpPost]
        [CustomerStatusCheck]
        public async Task<Result<MultipleOrdersReviewModel>> OrderReviewUpdateCart([FromBody] MultipleOrdersReviewModel model)
        {
            if (await IsCartEmpty())
            {
                return GetJsonRedirect<MultipleOrdersReviewModel>(Url.Action("EmptyCart", "Cart"));
            }

            Validate(model);

            var cart = await GetCurrentCart(withMultipleShipmentsService: true);

            bool canUpdate = true;
            if (model == null)
            {
                canUpdate = false;
                model = new MultipleOrdersReviewModel();
                await OrderReviewInitCartModelInternal(model, cart);
            }
            foreach (var reviewUpdateOrderModel in model.Shipments)
            {
                if (reviewUpdateOrderModel.OrderModel.ShipAsap && reviewUpdateOrderModel.OrderModel.ShippingDate.HasValue)
                {
                    reviewUpdateOrderModel.OrderModel.ShippingDate = null;
                }
            }

            using (await CartLocks.GetLockAsync(cart.CartUid))
            {
                var mainShipment = model.Shipments.FirstOrDefault(p => p.Main);
                if (mainShipment == null)
                {
                    throw new AppValidationException("Main order shipping isn't specified");
                }

                //get skus data and gc codes in all shipments
                var skuCodes = new List<string>();
                var gcCodes = new List<string>();
                foreach (var reviewUpdateOrderModel in model.Shipments)
                {
                    skuCodes.AddRange(reviewUpdateOrderModel.OrderModel.Skus.Select(p => p.Code));
                    gcCodes.AddRange(reviewUpdateOrderModel.OrderModel.GiftCertificateCodes
                        .Select(x => x.Value?.Trim().ToUpper())
                        .Where(v => !string.IsNullOrWhiteSpace(v)));
                }
                var skus = await _productService.GetSkusOrderedAsync(skuCodes.Distinct().ToList());
                //user current cart.Order gcs for correct calculation
                if (cart.Order.GiftCertificates != null)
                {
                    gcCodes.RemoveAll(p => cart.Order.GiftCertificates.Select(pp => pp.GiftCertificate.Code.ToUpper()).Contains(p));
                }
                var gcs = await _gcService.TryGetGiftCertificatesAsync(gcCodes.Distinct().ToList());
                //user current cart.Order gcs for correct calculation
                if (cart.Order.GiftCertificates != null)
                {
                    gcs.AddRange(cart.Order.GiftCertificates.Select(p => p.GiftCertificate));
                }

                //main shipment
                if (cart.Order.Skus != null)
                {
                    cart.Order.Skus.MergeKeyed(mainShipment.OrderModel.Skus.Where(s => s.Quantity > 0).ToArray(), ordered => ordered.Sku.Code,
                            skuModel => skuModel.Code,
                            skuModel =>
                            {
                                var ordered = skus.FirstOrDefault(p => p.Sku.Code == skuModel.Code);
                                if (ordered != null)
                                {
                                    ordered.Quantity = skuModel.Quantity;
                                    ordered.Amount = HasRole(RoleType.Wholesale) ? ordered.Sku.WholesalePrice : ordered.Sku.Price;
                                }
                                return ordered;
                            },
                            (ordered, skuModel) =>
                            {
                                ordered.Quantity = skuModel.Quantity;
                            });
                    cart.Order.Skus.RemoveAll(p => p == null);
                }
                cart.Order.Discount = await _discountService.GetByCode(mainShipment.OrderModel.DiscountCode);

                var inOrderGcs = gcs.Where(p => mainShipment.OrderModel.GiftCertificateCodes.Select(pp => pp.Value).Contains(p.Code)).ToList();
                cart.Order.GiftCertificates?.MergeKeyed(inOrderGcs,
                    gc => gc.GiftCertificate?.Code?.Trim().ToUpper(), code => code.Code?.Trim().ToUpper(),
                    code => new GiftCertificateInOrder
                    {
                        GiftCertificate = code
                    });
                //reorder based on UI ordering
                if (cart.Order.GiftCertificates != null)
                {
                    var data = cart.Order.GiftCertificates;
                    cart.Order.GiftCertificates = new List<GiftCertificateInOrder>();
                    foreach (var orderModelGiftCertificateCode in mainShipment.OrderModel.GiftCertificateCodes)
                    {
                        var gc = data.FirstOrDefault(p => p.GiftCertificate.Code.Trim().ToUpper() ==
                            orderModelGiftCertificateCode.Value.Trim().ToUpper());
                        if (gc != null)
                        {
                            cart.Order.GiftCertificates.Add(gc);
                        }
                    }
                }

                if (!mainShipment.OrderModel.ShipAsap)
                {
                    cart.Order.Data.ShipDelayType = ShipDelayType.EntireOrder;
                    cart.Order.Data.ShipDelayDate = mainShipment.OrderModel.ShippingDate;
                }
                else
                {
                    cart.Order.Data.ShipDelayType = ShipDelayType.None;
                    cart.Order.Data.ShipDelayDate = null;
                }

                if (ModelState.IsValid)
                {
                    cart.Order.Data.ShippingUpgradeP = mainShipment.OrderModel.ShippingUpgradeP;
                    cart.Order.Data.ShippingUpgradeNP = mainShipment.OrderModel.ShippingUpgradeNP;
                    var context = await OrderService.CalculateStorefrontOrder(cart.Order, OrderStatus.Incomplete);
                    await FillModel(mainShipment.OrderModel, cart.Order, context, "i0.");
                }

                var i = 1;
                foreach (var shipment in model.Shipments.Where(p => !p.Main))
                {
                    var item = cart.Order.CartAdditionalShipments.FirstOrDefault(p => p.Id == shipment.IdShipment);
                    if (item == null)
                    {
                        return GetJsonRedirect<MultipleOrdersReviewModel>(Url.Action("ReviewOrder"));
                    }
                    item.Name = $"Order #{(i + 1)}";
                    item.Skus = shipment.OrderModel.Skus.Where(s => s.Quantity > 0).Select(skuModel =>
                    {
                        var ordered = skus.FirstOrDefault(p => p.Sku.Code == skuModel.Code);
                        if (ordered != null)
                        {
                            ordered.Quantity = skuModel.Quantity;
                            ordered.Amount = HasRole(RoleType.Wholesale)
                                ? ordered.Sku.WholesalePrice
                                : ordered.Sku.Price;
                        }
                        return ordered;
                    }).ToList();
                    item.Skus.RemoveAll(p => p == null);

                    var currentOrder = OrderService.CreateNewNormalOrder(OrderStatus.Incomplete);
                    currentOrder.ShippingAddress = item.ShippingAddress;
                    currentOrder.Skus = item.Skus;
                    currentOrder.PaymentMethod = cart.Order.PaymentMethod;
                    currentOrder.Customer = cart.Order.Customer;

                    currentOrder.Discount = await _discountService.GetByCode(shipment.OrderModel.DiscountCode);
                    item.DiscountCode = shipment.OrderModel.DiscountCode;

                    inOrderGcs = gcs.Where(p => shipment.OrderModel.GiftCertificateCodes.Select(pp => pp.Value).Contains(p.Code)).ToList();
                    currentOrder.GiftCertificates?.MergeKeyed(inOrderGcs,
                        gc => gc.GiftCertificate?.Code?.Trim().ToUpper(), code => code.Code?.Trim().ToUpper(),
                        code => new GiftCertificateInOrder
                        {
                            GiftCertificate = code
                        });
                    //reorder based on UI ordering
                    if (currentOrder.GiftCertificates != null)
                    {
                        var data = currentOrder.GiftCertificates;
                        currentOrder.GiftCertificates = new List<GiftCertificateInOrder>();
                        foreach (var orderModelGiftCertificateCode in shipment.OrderModel.GiftCertificateCodes)
                        {
                            var gc = data.FirstOrDefault(p => p.GiftCertificate.Code.Trim().ToUpper() ==
                                orderModelGiftCertificateCode.Value.Trim().ToUpper());
                            if (gc != null)
                            {
                                currentOrder.GiftCertificates.Add(gc);
                            }
                        }
                    }

                    item.GiftCertificateIds = currentOrder.GiftCertificates?.Select(p => p.GiftCertificate.Id).ToList() ??
                        new List<int>();

                    if (!shipment.OrderModel.ShipAsap)
                    {
                        currentOrder.Data.ShipDelayType = ShipDelayType.EntireOrder;
                        currentOrder.Data.ShipDelayDate = shipment.OrderModel.ShippingDate;
                        item.ShipDelayDate = shipment.OrderModel.ShippingDate;
                    }
                    else
                    {
                        currentOrder.Data.ShipDelayType = ShipDelayType.None;
                        currentOrder.Data.ShipDelayDate = null;
                        item.ShipDelayDate = null;
                    }
                    currentOrder.Data.ShippingUpgradeP = shipment.OrderModel.ShippingUpgradeP;
                    currentOrder.Data.ShippingUpgradeNP = shipment.OrderModel.ShippingUpgradeNP;
                    item.ShippingUpgradeP = (int?)shipment.OrderModel.ShippingUpgradeP;
                    item.ShippingUpgradeNP = (int?)shipment.OrderModel.ShippingUpgradeNP;

                    var context = await OrderService.CalculateStorefrontOrder(currentOrder, OrderStatus.Incomplete);
                    await FillModel(shipment.OrderModel, currentOrder, context, $"i{i}.");

                    i++;
                }

                //remove deletes shipments
                var forRemove = new List<CartAdditionalShipmentModelItem>();
                foreach (var item in cart.Order.CartAdditionalShipments)
                {
                    var remove = true;
                    foreach (var shipment in model.Shipments.Where(p => !p.Main))
                    {
                        if (item.Id == shipment.IdShipment)
                        {
                            remove = false;
                            break;
                        }
                    }
                    if (remove)
                    {
                        forRemove.Add(item);
                    }
                }
                foreach (var cartAdditionalShipmentModelItem in forRemove)
                {
                    cart.Order.CartAdditionalShipments.Remove(cartAdditionalShipmentModelItem);
                }

                bool updateResult = true;
                if (canUpdate)
                {
                    updateResult = await CheckoutService.UpdateCart(cart, withMultipleShipmentsService: true);
                }

                if (!ModelState.IsValid)
                    return ResultHelper.CreateErrorResult(ModelState, model);
                if (!updateResult)
                    return new Result<MultipleOrdersReviewModel>(false, model);
                return model;
            }
        }

        //private void ParseFieldsCalculationErrors

        [HttpPost]
        [CustomerStatusCheck]
        //[CustomerAuthorize]
        public async Task<Result<object>> ReviewOrder([FromBody] MultipleOrdersReviewModel model)
        {
            Guid? cartUid;
            using (await LockCurrentCart(out cartUid))
            {
                if (await IsCartEmpty())
                {
                    return GetJsonRedirect<object>(Url.Action("ViewCart", "Cart"));
                }

                var cart = await GetCurrentCart(withMultipleShipmentsService: true);
                using (await CartLocks.LockWhenAsync(() => cartUid.HasValue && cartUid.Value != cart.CartUid, () => cart.CartUid))
                {
                    using (await OrderLocks.LockWhenAsync(() => cart.Order?.Id > 0, () => cart.Order.Id))
                    {
                        var loggedIn = await EnsureLoggedIn(cart);
                        if (loggedIn != null)
                        {
                            if (IsCanadaShippingIssue(cart.Order.Customer, cart.Order))
                            {
                                return GetJsonRedirect<object>(Url.Action("AddUpdateShippingMethod", new { canadaissue = true }));
                            }

                            var mainShipment = model.Shipments.FirstOrDefault(p => p.Main);
                            if (mainShipment == null)
                            {
                                throw new AppValidationException("Main order shipping isn't specified");
                            }

                            //get gc codes in all shipments
                            var gcCodes = new List<string>();
                            foreach (var reviewUpdateOrderModel in model.Shipments)
                            {
                                gcCodes.AddRange(reviewUpdateOrderModel.OrderModel.GiftCertificateCodes
                                    .Select(x => x.Value?.Trim().ToUpper())
                                    .Where(v => !string.IsNullOrWhiteSpace(v)));
                            }
                            //user current cart.Order gcs for correct calculation
                            if (cart.Order.GiftCertificates != null)
                            {
                                gcCodes.RemoveAll(p => cart.Order.GiftCertificates.Select(pp => pp.GiftCertificate.Code.ToUpper()).Contains(p));
                            }
                            var gcs = await _gcService.TryGetGiftCertificatesAsync(gcCodes.Distinct().ToList());
                            //user current cart.Order gcs for correct calculation
                            if (cart.Order.GiftCertificates != null)
                            {
                                gcs.AddRange(cart.Order.GiftCertificates.Select(p => p.GiftCertificate));
                            }

                            var inOrderGcs = gcs.Where(p => mainShipment.OrderModel.GiftCertificateCodes.Select(pp => pp.Value).Contains(p.Code)).ToList();
                            cart.Order.GiftCertificates?.MergeKeyed(inOrderGcs,
                                gc => gc.GiftCertificate?.Code?.Trim().ToUpper(), code => code.Code?.Trim().ToUpper(),
                                code => new GiftCertificateInOrder
                                {
                                    GiftCertificate = code
                                });
                            //reorder based on UI ordering
                            if (cart.Order.GiftCertificates != null)
                            {
                                var data = cart.Order.GiftCertificates;
                                cart.Order.GiftCertificates = new List<GiftCertificateInOrder>();
                                foreach (var orderModelGiftCertificateCode in mainShipment.OrderModel.GiftCertificateCodes)
                                {
                                    var gc = data.FirstOrDefault(p => p.GiftCertificate.Code.Trim().ToUpper() ==
                                        orderModelGiftCertificateCode.Value.Trim().ToUpper());
                                    if (gc != null)
                                    {
                                        cart.Order.GiftCertificates.Add(gc);
                                    }
                                }
                            }

                            var ordersForSaving = new List<OrderDynamic>()
                            {
                                cart.Order
                            };

                            foreach (var reviewUpdateOrderModel in model.Shipments)
                            {
                                if (reviewUpdateOrderModel.OrderModel.ShipAsap && reviewUpdateOrderModel.OrderModel.ShippingDate.HasValue)
                                {
                                    reviewUpdateOrderModel.OrderModel.ShippingDate = null;
                                }
                            }
                            foreach (var shipment in model.Shipments.Where(p => !p.Main))
                            {
                                var item = cart.Order.CartAdditionalShipments.FirstOrDefault(p => p.Id == shipment.IdShipment);
                                if (item == null)
                                {
                                    return GetJsonRedirect<MultipleOrdersReviewModel>(Url.Action("ReviewOrder"));
                                }

                                var currentOrder = OrderService.CreateNewNormalOrder(OrderStatus.Incomplete);
                                currentOrder.ShippingAddress = item.ShippingAddress.Clone<AddressDynamic, MappedObject>()
                                            .Clone<AddressDynamic, Entity>();
                                currentOrder.ShippingAddress.Id = 0;
                                currentOrder.Skus = item.Skus;
                                currentOrder.Data.GiftMessage = item.IsGiftOrder ? item.GiftMessage : string.Empty;
                                currentOrder.Data.GiftOrder = item.IsGiftOrder;
                                currentOrder.PaymentMethod = cart.Order.PaymentMethod.Clone<OrderPaymentMethodDynamic, MappedObject>()
                                        .Clone<OrderPaymentMethodDynamic, Entity>();
                                currentOrder.PaymentMethod.Id = 0;
                                currentOrder.PaymentMethod.IdOrderSource = cart.Order.Id;
                                if (currentOrder.PaymentMethod.Address != null)
                                {
                                    currentOrder.PaymentMethod.Address.Id = 0;
                                }
                                currentOrder.Customer = cart.Order.Customer;

                                currentOrder.Discount = await _discountService.GetByCode(shipment.OrderModel.DiscountCode);
                                item.DiscountCode = shipment.OrderModel.DiscountCode;

                                inOrderGcs = gcs.Where(p => shipment.OrderModel.GiftCertificateCodes.Select(pp => pp.Value).Contains(p.Code)).ToList();
                                currentOrder.GiftCertificates?.MergeKeyed(inOrderGcs,
                                    gc => gc.GiftCertificate?.Code?.Trim().ToUpper(), code => code.Code?.Trim().ToUpper(),
                                    code => new GiftCertificateInOrder
                                    {
                                        GiftCertificate = code
                                    });
                                //reorder based on UI ordering
                                if (currentOrder.GiftCertificates != null)
                                {
                                    var data = currentOrder.GiftCertificates;
                                    currentOrder.GiftCertificates = new List<GiftCertificateInOrder>();
                                    foreach (var orderModelGiftCertificateCode in shipment.OrderModel.GiftCertificateCodes)
                                    {
                                        var gc = data.FirstOrDefault(p => p.GiftCertificate.Code.Trim().ToUpper() ==
                                            orderModelGiftCertificateCode.Value.Trim().ToUpper());
                                        if (gc != null)
                                        {
                                            currentOrder.GiftCertificates.Add(gc);
                                        }
                                    }
                                }

                                item.GiftCertificateIds = currentOrder.GiftCertificates?.Select(p => p.GiftCertificate.Id).ToList() ??
                                    new List<int>();

                                if (!shipment.OrderModel.ShipAsap)
                                {
                                    currentOrder.Data.ShipDelayType = ShipDelayType.EntireOrder;
                                    currentOrder.Data.ShipDelayDate = shipment.OrderModel.ShippingDate;
                                    item.ShipDelayDate = shipment.OrderModel.ShippingDate;
                                }
                                else
                                {
                                    currentOrder.Data.ShipDelayType = ShipDelayType.None;
                                    currentOrder.Data.ShipDelayDate = null;
                                    item.ShipDelayDate = null;
                                }
                                currentOrder.Data.ShippingUpgradeP = shipment.OrderModel.ShippingUpgradeP;
                                currentOrder.Data.ShippingUpgradeNP = shipment.OrderModel.ShippingUpgradeNP;
                                item.ShippingUpgradeP = (int?)shipment.OrderModel.ShippingUpgradeP;
                                item.ShippingUpgradeNP = (int?)shipment.OrderModel.ShippingUpgradeNP;

                                ordersForSaving.Add(currentOrder);
                            }

                            var ids = await CheckoutService.SaveOrdersForTheSameCustomer(ordersForSaving, cart.Order.Customer, cart.CartUid);
                            string sIds = string.Join(",", ids);

                            HttpContext.Session.SetString(CheckoutConstants.ReceiptSessionOrderIds, sIds);

                            return GetJsonRedirect<object>(Url.Action("Receipt"));
                        }
                        else
                        {
                            return GetJsonRedirect<object>(Url.Action("Welcome"));
                        }
                        return null;
                    }
                }
            }
        }

        #endregion

        private bool IsCanadaShippingIssue(CustomerDynamic customer, OrderDynamic order)
            => customer.IdObjectType == (int)CustomerType.Retail && order.ShippingAddress.IdCountry != ReferenceData.DefaultCountry.Id;

        [HttpGet]
        [CustomerStatusCheck]
        public async Task<IActionResult> Receipt()
        {
            var ids = (HttpContext.Session.GetString(CheckoutConstants.ReceiptSessionOrderIds) ?? String.Empty).Split(',');
            var orderIds = new List<int>();
            foreach (var id in ids)
            {
                int result;
                if (Int32.TryParse(id, out result))
                {
                    orderIds.Add(result);
                }
            }

            if (orderIds.Count == 0)
            {
                return View("EmptyCart");
            }
            var toReturn = new MultipleReceiptModel();
            var orders = (await OrderService.SelectAsync(orderIds, true)).OrderBy(p => p.Id).ToList();
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].IdObjectType == (int)OrderType.AutoShip)
                {
                    var id = (await OrderService.SelectAutoShipOrdersAsync(orders[i].Id)).First();

                    orders[i] = await OrderService.SelectAsync(id, true);
                }

                ReceiptModel model = new ReceiptModel();
                await PopulateReviewModel(model, orders[i]);
                model.OrderNumber = orders[i].Id.ToString();
                model.OrderDate = orders[i].DateCreated;

                if (orders.Count > 1 && i % 2 == 0)
                {
                    model.AlternateColor = true;
                }

                toReturn.Receipts.Add(model);
            }


            if (orders.SelectMany(x => x.Skus).Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                SelectMany(p => p.GcsGenerated).Any())
            {
                toReturn.ShowEGiftEmailForm = true;
                toReturn.EGiftSendEmail = new EGiftSendEmailModel();
                toReturn.EGiftSendEmail.All = true;
                toReturn.EGiftSendEmail.Codes = orders.SelectMany(x => x.Skus).Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                    SelectMany(p => p.GcsGenerated).Select(p => new EGiftSendEmailCodeModel()
                    {
                        Code = p.Code,
                        Selected = true
                    }).ToList();
            }

            return View(toReturn);
        }

        [HttpPost]
        [CustomerStatusCheck]
        public async Task<IActionResult> SendEGiftEmail(EGiftSendEmailModel model)
        {
            var ids = (HttpContext.Session.GetString(CheckoutConstants.ReceiptSessionOrderIds) ?? String.Empty).Split(',');
            var orderIds = new List<int>();
            foreach (var id in ids)
            {
                int result;
                if (Int32.TryParse(id, out result))
                {
                    orderIds.Add(result);
                }
            }

            if (orderIds.Count == 0)
            {
                model.Codes = new List<EGiftSendEmailCodeModel>();
                return PartialView("_SendEGiftEmail", model);
            }

            var orders = (await OrderService.SelectAsync(orderIds, true)).OrderBy(p => p.Id).ToList();
            for (int i = 0; i < orders.Count; i++)
            {
                if (orders[i].IdObjectType == (int)OrderType.AutoShip)
                {
                    var id = (await OrderService.SelectAutoShipOrdersAsync(orders[i].Id)).First();

                    orders[i] = await OrderService.SelectAsync(id, true);
                }
            }

            if (!model.All && (model.SelectedCodes == null || model.SelectedCodes.Count == 0))
            {
                ModelState.AddModelError("", "At least one E-Gift should be specified");
            }
            if (!ModelState.IsValid)
            {
                model.Codes = orders.SelectMany(x => x.Skus).Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                   SelectMany(p => p.GcsGenerated).Select(p => new EGiftSendEmailCodeModel()
                   {
                       Code = p.Code,
                       Selected = false
                   }).ToList();
                return PartialView("_SendEGiftEmail", model);
            }

            var customer = await CustomerService.SelectAsync(orders.First().Customer.Id);
            var emailModel = new EGiftNotificationEmail();
            emailModel.Sender = $"{customer.ProfileAddress.SafeData.FirstName} {customer.ProfileAddress.SafeData.LastName}";
            emailModel.Recipient = model.Recipient;
            emailModel.Email = model.Email;
            emailModel.Message = model.Message;
            emailModel.EGifts = orders.SelectMany(x => x.Skus).Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                SelectMany(p => p.GcsGenerated)
                .Where(p => model.All || model.SelectedCodes.Contains(p.Code))
                .Select(p => new GiftEmailModel()
                {
                    Code = p.Code,
                    Amount = p.Balance
                }).ToList();
            await _notificationService.SendEGiftNotificationEmailAsync(model.Email, emailModel);

            ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullySent];
            ModelState.Clear();
            var newModel = new EGiftSendEmailModel();
            newModel.All = true;
            newModel.Codes = orders.SelectMany(x => x.Skus).Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                SelectMany(p => p.GcsGenerated).Select(p => new EGiftSendEmailCodeModel()
                {
                    Code = p.Code,
                    Selected = true
                }).ToList();
            return PartialView("_SendEGiftEmail", newModel);
        }

        [HttpGet]
        public IActionResult CanadaShippingNoticeView()
        {
            return PartialView("_CanadaShippingNotice");
        }

        [HttpGet]
        public IActionResult CanadaShippingIssueView()
        {
            return PartialView("_CanadaShippingIssue");
        }

        private List<KeyValuePair<string, AddressDynamic>> GetShippingAddresses(OrderDynamic order, CustomerDynamic currentCustomer)
        {
            List<KeyValuePair<string, AddressDynamic>> shippingAddresses = new List<KeyValuePair<string, AddressDynamic>>();
            if (order.ShippingAddress != null && order.ShippingAddress.Id != 0 &&
                !string.IsNullOrEmpty(order.ShippingAddress.SafeData.FirstName))
            {
                shippingAddresses.Add(new KeyValuePair<string, AddressDynamic>("(Currently On Order)", order.ShippingAddress));
            }
            if (currentCustomer?.ShippingAddresses.Count > 0)
            {
                shippingAddresses.AddRange(
                    currentCustomer.ShippingAddresses.OrderByDescending(a => (bool?)a.SafeData.Default ?? false).Select(
                        a => new KeyValuePair<string, AddressDynamic>(((bool?)a.SafeData.Default ?? false) ? "(Default)" : string.Empty, a)));
            }
            return shippingAddresses;
        }

        private async Task<CustomerCartOrder> PopulateReviewModel(ReviewOrderModel reviewOrderModel, CustomerCartOrder cart)
        {
            await InitCartModelInternal(reviewOrderModel);

            var paymentMethod = cart.Order.PaymentMethod;
            reviewOrderModel.BillToAddress = paymentMethod.Address.PopulateBillingAddressDetails(_countryNameCodeResolver,
                cart.Order.Customer.Email);
            reviewOrderModel.CreditCardDetails = paymentMethod.PopulateCreditCardDetails(ReferenceData);

            var shippingAddress = cart.Order.ShippingAddress;
            reviewOrderModel.ShipToAddress = shippingAddress.PopulateShippingAddressDetails(_countryNameCodeResolver);

            reviewOrderModel.DeliveryInstructions = shippingAddress.SafeData.DeliveryInstructions;
            reviewOrderModel.GiftMessage = cart.Order.SafeData.GiftMessage;

            return cart;
        }

        private async Task PopulateReviewModel(ReviewOrderModel reviewOrderModel, OrderDynamic order)
        {
            order.Customer = await CustomerService.SelectAsync(order.Customer.Id, true);
            var context = await OrderService.CalculateStorefrontOrder(order, OrderStatus.Processed);
            await FillModel(reviewOrderModel, order, context);

            var paymentMethod = order.PaymentMethod;
            reviewOrderModel.BillToAddress = paymentMethod.Address.PopulateBillingAddressDetails(_countryNameCodeResolver,
                order.Customer.Email);
            reviewOrderModel.CreditCardDetails = paymentMethod.PopulateCreditCardDetails(ReferenceData);

            var shippingAddress = order.ShippingAddress;
            reviewOrderModel.ShipToAddress = shippingAddress.PopulateShippingAddressDetails(_countryNameCodeResolver);

            reviewOrderModel.DeliveryInstructions = shippingAddress.SafeData.DeliveryInstructions;
            reviewOrderModel.GiftMessage = order.SafeData.GiftMessage;
        }

        private struct CreateResult
        {
            public CustomerDynamic Customer;
            public Func<Task<ApplicationUser>> LoginTask;
        }

        private async Task<CreateResult> EnsureCustomerCreated(UpdateShipmentsModel model,ShippingAddressModel mainShipment, 
            CustomerDynamic existing = null)
        {
            Func<Task<ApplicationUser>> loginTask;
            CustomerDynamic newCustomer;
            if (model.GuestCheckout)
            {
                var existingId = await CustomerService.TryGetActiveIdByEmailAsync(model.Email) ??
                                 await CustomerService.TryGetPhoneOnlyIdByEmailAsync(model.Email);
                if (existingId.HasValue)
                {
                    existing = await CustomerService.SelectAsync(existingId.Value, true);
                }
                if (existing == null || existing.StatusCode != (int)CustomerStatus.PhoneOnly)
                {
                    newCustomer = await CreateAccount(model, mainShipment);
                    loginTask = CreateLoginForNewGuest(newCustomer);
                }
                else
                {
                    newCustomer = await ReplaceAccount(model, mainShipment, existing);
                    if (newCustomer == null)
                    {
                        throw new ApiException("Customer couldn't be created");
                    }
                    loginTask = CreateLoginForExistingGuest(newCustomer);
                }
            }
            else
            {
                var existingId = await CustomerService.TryGetActiveIdByEmailAsync(model.Email) ??
                                 await CustomerService.TryGetPhoneOnlyIdByEmailAsync(model.Email);
                if (existingId.HasValue)
                {
                    existing = await CustomerService.SelectAsync(existingId.Value, true);
                }
                if (existing == null || existing.StatusCode != (int)CustomerStatus.PhoneOnly)
                {
                    newCustomer = await CreateAccount(model, mainShipment);
                }
                else
                {
                    newCustomer = await ReplaceAccount(model, mainShipment, existing);
                    if (newCustomer == null)
                    {
                        throw new ApiException("Customer couldn't be created");
                    }
                }
                loginTask = CreateLoginForNewActive(model, mainShipment, newCustomer.Id);
            }
            return new CreateResult
            {
                Customer = newCustomer,
                LoginTask = loginTask
            };
        }

        private Func<Task<ApplicationUser>> CreateLoginForExistingGuest(CustomerDynamic newCustomer) => async () =>
        {
            var user = await _userService.GetAsync(newCustomer.Id);
            user = await _userService.SignInNoStatusCheckingAsync(user);
            if (user == null)
            {
                throw new AppValidationException(
                    ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
            }
            return user;
        };

        private Func<Task<ApplicationUser>> CreateLoginForNewGuest(CustomerDynamic newCustomer) => async () =>
        {
            await _userService.SendActivationAsync(newCustomer.Id);
            var user = await _userService.GetAsync(newCustomer.Id);
            user = await _userService.SignInNoStatusCheckingAsync(user);
            if (user == null)
            {
                throw new AppValidationException(
                    ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
            }
            return user;
        };

        private Func<Task<ApplicationUser>> CreateLoginForNewActive(UpdateShipmentsModel model, ShippingAddressModel mainShipment, int id) => async () =>
        {
            await _userService.SendSuccessfulRegistration(model.Email, mainShipment.FirstName, mainShipment.LastName);
            var user = await _userService.SignInAsync(id, model.Password);
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
            }
            return user;
        };

        private async Task<ApplicationUser> CustomerPasswordLogin(LoginModel model)
        {
            var id = await CustomerService.TryGetActiveIdByEmailAsync(model.Email);
            if (!id.HasValue)
            {
                id = await CustomerService.TryGetNotActiveIdByEmailAsync(model.Email);
            }
            if (!id.HasValue)
            {
                throw new AppValidationException(
                    ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.IncorrectUserPassword]);
            }
            var user = await _userService.SignInAsync(id.Value, model.Password);
            await HttpContext.SpinAuthorizationToken(_tokenService, null, user, _encryptionHost);
            return user;
        }

        private async Task<bool?> EnsureLoggedIn(CustomerCartOrder cart)
        {
            if (await CustomerLoggedIn())
            {
                return true;
            }

            if ((bool?)cart.Order.Customer?.SafeData.Guest ?? false)
            {
                var user = await _userService.GetAsync(cart.Order.Customer.Id);
                user = await _userService.SignInNoStatusCheckingAsync(user);
                if (user == null)
                {
                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
                }
                return false;
            }
            return null;
        }

        private async Task<CustomerDynamic> ReplaceAccount(UpdateShipmentsModel model, ShippingAddressModel mainShipment, CustomerDynamic existingCustomer)
        {
            if (existingCustomer == null)
                throw new ArgumentNullException(nameof(existingCustomer));
            UpdateAccount(model, existingCustomer);
            if (existingCustomer.ShippingAddresses == null)
                existingCustomer.ShippingAddresses = new List<AddressDynamic>();

            await _addressConverter.UpdateObjectAsync(mainShipment, existingCustomer.ProfileAddress, (int)AddressType.Profile);
            var shipping = existingCustomer.ShippingAddresses.FirstOrDefault();
            if (shipping == null)
            {
                shipping = _addressConverter.CreatePrototype((int)AddressType.Shipping);
                existingCustomer.ShippingAddresses.Add(shipping);
            }
            await _addressConverter.UpdateObjectAsync(mainShipment, shipping, (int)AddressType.Shipping);
            shipping.Data.Default = true;
            existingCustomer =
                await CustomerService.UpdateAsync(existingCustomer, model.GuestCheckout ? null : model.Password);
            return existingCustomer;
        }

        private void UpdateAccount(UpdateShipmentsModel model, CustomerDynamic newCustomer)
        {
            newCustomer.IdDefaultPaymentMethod = (int)PaymentMethodType.CreditCard;
            if (model.GuestCheckout)
            {
                newCustomer.Data.Guest = true;
                newCustomer.StatusCode = (int)CustomerStatus.PhoneOnly;
            }
            else
            {
                newCustomer.StatusCode = (int)CustomerStatus.Active;
            }
            newCustomer.Email = model.Email;
            newCustomer.ApprovedPaymentMethods = new List<int> { (int)PaymentMethodType.CreditCard };
        }

        private async Task<CustomerDynamic> CreateAccount(UpdateShipmentsModel model,ShippingAddressModel mainShipment)
        {
            var newCustomer = CustomerService.Mapper.CreatePrototype((int)CustomerType.Retail);
            newCustomer.PublicId = Guid.NewGuid();
            UpdateAccount(model, newCustomer);
            newCustomer.ProfileAddress = await _addressConverter.FromModelAsync(mainShipment, (int)AddressType.Profile);
            newCustomer.ProfileAddress.Id = 0;
            var shipping = await _addressConverter.FromModelAsync(mainShipment, (int)AddressType.Shipping);
            shipping.Id = 0;
            shipping.Data.Default = true;
            newCustomer.ShippingAddresses = new List<AddressDynamic> { shipping };

            var cookies = Request.Cookies[AffiliateConstants.AffiliatePublicIdParam];
            if (!String.IsNullOrEmpty(cookies))
            {
                int idAffiliate = 0;
                if (Int32.TryParse(cookies, out idAffiliate))
                {
                    var affiliate = await _affiliateService.SelectAsync(idAffiliate);
                    if (affiliate != null)
                    {
                        newCustomer.IdAffiliate = affiliate.Id;
                    }
                }
            }

            newCustomer =
                await CustomerService.InsertAsync(newCustomer, model.GuestCheckout ? null : model.Password);
            return newCustomer;
        }
    }
}