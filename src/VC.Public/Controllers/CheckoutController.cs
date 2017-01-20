﻿using System;
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
            ITokenService tokenService, ITransactionAccessor<VitalChoiceContext> vitalchoiceTransactionAccessor)
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
            _logger = loggerProvider.CreateLogger<CheckoutController>();
        }

        public async Task<IActionResult> Welcome(bool forgot = false)
        {
            if (await CustomerLoggedIn())
            {
                return RedirectToAction("AddUpdateBillingAddress");
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
                return await CustomerService.GetCustomerCardExist(internalId, id);
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

            return RedirectToAction("AddUpdateBillingAddress");
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

                        billingInfoModel.Email = cart.Order.Customer.Email;

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

                    billingInfoModel.Email = currentCustomer.Email;

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
            var loggedIn = await CustomerLoggedIn();

            if (loggedIn)
            {
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

                ViewBag.CreditCards = PopulateCreditCardsLookup(currentCustomer, cart.Order);

                addUpdateModel.Email = currentCustomer.Email;
                addUpdateModel.IdCustomerType = currentCustomer.IdObjectType;
            }
            else
            {
                if (cart.Order.PaymentMethod?.Address != null && cart.Order.PaymentMethod.Id != 0)
                {
                    await _addressConverter.UpdateModelAsync(addUpdateModel, cart.Order.PaymentMethod.Address);
                    await _orderPaymentMethodConverter.UpdateModelAsync<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                }
                addUpdateModel.Email = cart.Order.Customer?.Email;
                addUpdateModel.IdCustomerType = (int)CustomerType.Retail;
                addUpdateModel.Id = 0;
            }

            var pnc = await _productService.GetSkuAsync(CATALOG_PRODUCT_NAME);
            if (pnc != null)
            {
                var pncModel = await SkuMapper.ToModelAsync<CartSkuModel>(pnc);
                addUpdateModel.ShowSendCatalog = pncModel.InStock;
                addUpdateModel.SendCatalog = pncModel.InStock;
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
                var loggedIn = await CustomerLoggedIn();

                var cart = await GetCurrentCart();
                using (await CartLocks.LockWhenAsync(() => cartUid.HasValue && cartUid.Value != cart.CartUid, () => cart.CartUid))
                {
                    if (!ModelState.IsValid)
                    {
                        if (loggedIn)
                        {
                            ViewBag.CreditCards = PopulateCreditCardsLookup(cart.Order.Customer);
                        }
                        model.IdCustomerType = cart.Order?.Customer?.IdObjectType ?? (int)CustomerType.Retail;
                        return View(model);
                    }
                    using (await OrderLocks.LockWhenAsync(() => cart.Order?.Id > 0, () => cart.Order.Id))
                    {
                        Func<Task<ApplicationUser>> loginTask = null;
                        using (var userTransaction = _vitalchoiceTransactionAccessor.BeginTransaction())
                        {
                            using (var transaction = _ecommerceTransactionAccessor.BeginTransaction())
                            {
                                try
                                {
                                    if (loggedIn)
                                    {
                                        ViewBag.CreditCards = PopulateCreditCardsLookup(cart.Order.Customer);
                                    }
                                    else
                                    {
                                        var createResult = await EnsureCustomerCreated(model, cart.Order.Customer);
                                        cart.Order.Customer = createResult.Customer;
                                        loginTask = createResult.LoginTask;
                                    }

                                    if (model.SendCatalog)
                                    {
                                        var pnc = await _productService.GetSkuOrderedAsync(CATALOG_PRODUCT_NAME);
                                        if (pnc != null)
                                        {
                                            pnc.Quantity = 1;
                                            pnc.Amount = pnc.Sku.Price;
                                            cart.Order.Skus.AddKeyed(Enumerable.Repeat(pnc, 1), ordered => ordered.Sku.Code);
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
                                            wl.Quantity = 1;
                                            wl.Amount = wl.Sku.Price;
                                            cart.Order.Skus.AddKeyed(Enumerable.Repeat(wl, 1), ordered => ordered.Sku.Code);
                                        }
                                    }

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

                                            model.IdCustomerType = cart.Order.Customer.IdObjectType;
                                            return View(model);
                                        }
                                        if (model.Id > 0)
                                        {
                                            if (cart.Order.Customer.CustomerPaymentMethods?.All(p => p.Id != model.Id) ?? true)
                                            {
                                                ModelState.AddModelError(string.Empty,
                                                    "For security reasons. Please enter all credit card details for this card or please select a new one to continue.");

                                                model.IdCustomerType = cart.Order.Customer.IdObjectType;
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
                                    if (!await CheckoutService.UpdateCart(cart))
                                    {
                                        ModelState.AddModelError(string.Empty, "Cannot update order");
                                        //if (loginTask != null)
                                        //{
                                        //    if (cart.Order.Customer != null && cart.Order.Customer.Id != 0)
                                        //    {
                                        //        await _userService.RemoveAsync(cart.Order.Customer.Id);
                                        //    }
                                        //}
                                        userTransaction.Rollback();
                                        transaction.Rollback();
                                    }
                                    else
                                    {
                                        if (loginTask != null)
                                            await loginTask();
                                        transaction.Commit();
                                        userTransaction.Commit();

                                        if (!string.IsNullOrEmpty(model.Email))
                                        {
                                            _brontoService.PushSubscribe(model.Email, model.SendNews);
                                        }

                                        return RedirectToAction("AddUpdateShippingMethod");
                                    }
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

                    model.IdCustomerType = cart.Order?.Customer?.IdObjectType ?? (int)CustomerType.Retail;
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

            if (cart.Order.PaymentMethod?.Address?.IdCountry == null)
            {
                return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("AddUpdateBillingAddress"));
            }

            var toReturn = new UpdateShipmentsModel();

            var loggedIn = await EnsureLoggedIn(cart);
            if (loggedIn != null)
            {
                if (!loggedIn.Value)
                {
                    return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("AddUpdateShippingMethod"));
                }

                //order part
                var currentCustomer = cart.Order.Customer;
                var shippingMethodModel = new ShippingAddressModel()
                {
                    AddressType = ShippingAddressType.Residential,
                    FromOrder = true
                };

                var defaultShipping = currentCustomer.ShippingAddresses.FirstOrDefault(x => (bool?)x.SafeData.Default == true);
                if (cart.Order.ShippingAddress != null && cart.Order.ShippingAddress.Id != 0 &&
                    !string.IsNullOrEmpty(cart.Order.ShippingAddress.SafeData.FirstName))
                {
                    await _addressConverter.UpdateModelAsync(shippingMethodModel, cart.Order.ShippingAddress);
                }
                else if (defaultShipping != null)
                {
                    await _addressConverter.UpdateModelAsync(shippingMethodModel, defaultShipping);
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
                    item.Address=new ShippingAddressModel();
                    item.Name = $"{addressData.Value.SafeData.FirstName} {addressData.Value.SafeData.LastName} " +
                                $"{addressData.Value.SafeData.Address1} {addressData.Key}";
                    await _addressConverter.UpdateModelAsync(item.Address, addressData.Value);
                    item.Address.FromOrder = item.Address.Id == cart.Order?.ShippingAddress?.Id;
                    item.Address.IdCustomerShippingAddress = item.Address.Id == cart.Order?.ShippingAddress?.Id ? (int?)null : item.Address.Id;
                    item.Address.Id = null;
                    item.Address.PreferredShipMethodName = item.Address.PreferredShipMethod.HasValue
                        ? ReferenceData.OrderPreferredShipMethod.FirstOrDefault(
                            x => x.Key == (int) item.Address.PreferredShipMethod.Value)?.Text
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
            }
            else
            {
                return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("Welcome"));
            }

            return toReturn;
        }

        [HttpPost]
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

                var mainShipment = model.Shipments.FirstOrDefault(p => p.FromOrder);
                if (mainShipment == null)
                {
                    throw new AppValidationException("Main order shipping isn't specified");
                }

                var cart = await GetCurrentCart(withMultipleShipmentsService: true);
                using (await CartLocks.LockWhenAsync(() => cartUid.HasValue && cartUid.Value != cart.CartUid, () => cart.CartUid))
                {
                    using (await OrderLocks.LockWhenAsync(() => cart.Order?.Id > 0, () => cart.Order.Id))
                    {
                        var loggedIn = await EnsureLoggedIn(cart);
                        if (loggedIn != null)
                        {
                            //save main shipping to an order
                            if (cart.Order.ShippingAddress == null || cart.Order.ShippingAddress.Id == 0)
                            {
                                if (mainShipment.UseBillingAddress)
                                {
                                    cart.Order.ShippingAddress =
                                        await _addressConverter.FromModelAsync(mainShipment, (int)AddressType.Shipping);
                                    var billingMapped =
                                        await
                                            _addressConverter.ToModelAsync<AddUpdateBillingAddressModel>(
                                                cart.Order.PaymentMethod.Address);
                                    await _addressConverter.UpdateObjectAsync(billingMapped, cart.Order.ShippingAddress);
                                }
                                else
                                {
                                    cart.Order.ShippingAddress =
                                        await _addressConverter.FromModelAsync(mainShipment, (int)AddressType.Shipping);
                                }
                                cart.Order.ShippingAddress.Id = 0;
                            }
                            else
                            {
                                if (mainShipment.UseBillingAddress)
                                {
                                    var oldId = cart.Order.ShippingAddress.Id;
                                    await
                                        _addressConverter.UpdateObjectAsync(mainShipment, cart.Order.ShippingAddress,
                                            (int)AddressType.Shipping);
                                    var billingMapped =
                                        await
                                            _addressConverter.ToModelAsync<AddUpdateBillingAddressModel>(
                                                cart.Order.PaymentMethod.Address);
                                    await _addressConverter.UpdateObjectAsync(billingMapped, cart.Order.ShippingAddress);
                                    cart.Order.ShippingAddress.Id = oldId;
                                }
                                else
                                {
                                    var oldId = cart.Order.ShippingAddress.Id;
                                    await
                                        _addressConverter.UpdateObjectAsync(mainShipment, cart.Order.ShippingAddress,
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
                            Dictionary<int, AddressDynamic> customerShippingAddressForUpdate = new Dictionary<int, AddressDynamic>();
                            if (mainShipment.SaveToProfile && mainShipment.IdCustomerShippingAddress.HasValue)
                            {
                                customerShippingAddressForUpdate.Add(mainShipment.IdCustomerShippingAddress.Value, cart.Order.ShippingAddress);
                            }

                            //update additional shipments
                            var i = 2;
                            cart.Order.CartAdditionalShipments = new List<CartAdditionalShipmentModelItem>();
                            foreach (var modelShipment in model.Shipments.Where(p=>!p.FromOrder))
                            {
                                var item = new CartAdditionalShipmentModelItem {ShippingAddress = new AddressDynamic()};

                                await _addressConverter.UpdateObjectAsync(modelShipment, item.ShippingAddress,
                                    (int)AddressType.Shipping);
                                item.Id = modelShipment.IdShipment;
                                item.Name = $"Order #{i}";
                                item.IsGiftOrder = modelShipment.IsGiftOrder;
                                item.GiftMessage = modelShipment.GiftMessage;

                                if (modelShipment.SaveToProfile && modelShipment.IdCustomerShippingAddress.HasValue)
                                {
                                    if (!customerShippingAddressForUpdate.ContainsKey(modelShipment.IdCustomerShippingAddress.Value))
                                    {
                                        customerShippingAddressForUpdate.Add(modelShipment.IdCustomerShippingAddress.Value, item.ShippingAddress);
                                    }
                                }
                                i++;

                                cart.Order.CartAdditionalShipments.Add(item);
                            }
                            
                            await OrderService.CalculateStorefrontOrder(cart.Order, OrderStatus.Incomplete);
                            if (await CheckoutService.UpdateCart(cart, true))
                            {
                                var shippingAddresses = cart.Order.Customer.ShippingAddresses.ToList();
                                var update = false;
                                foreach (var itemForUpdate in customerShippingAddressForUpdate)
                                {
                                    var customerShippinhAddress = shippingAddresses.FirstOrDefault(x => x.Id == itemForUpdate.Key);
                                    if (customerShippinhAddress != null)
                                    {
                                        var index = shippingAddresses.IndexOf(customerShippinhAddress);
                                        var originalId = shippingAddresses[index].Id;
                                        var defaultAddr = (bool?) shippingAddresses[index].SafeData.Default ?? false;
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
                                    cart.Order.Customer.ShippingAddresses = new List<AddressDynamic> { cart.Order.ShippingAddress };

                                    cart.Order.Customer = await CustomerService.UpdateAsync(cart.Order.Customer);
                                }

                                if (IsCanadaShippingIssue(cart.Order.Customer, cart.Order))
                                {
                                    return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("AddUpdateShippingMethod", new { canadaissue = true }));
                                }

                                return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("ReviewOrder"), true);
                            }

                        }
                        else
                        {
                            return GetJsonRedirect<UpdateShipmentsModel>(Url.Action("Welcome"));
                        }

                        return model;
                    }
                }
            }
        }

        #endregion

        [HttpGet]
        [CustomerStatusCheck]
        public async Task<IActionResult> GetShippingAddress(int id)
        {
            var cart = await GetCurrentCart(withMultipleShipmentsService: true);
            var addresses = GetShippingAddresses(cart.Order, cart.Order?.Customer);
            var shipping = addresses[id].Value;

            var shippingModel = new AddUpdateShippingMethodModel();

            await _addressConverter.UpdateModelAsync<ShippingInfoModel>(shippingModel, shipping);

            shippingModel.ShipAddressIdToOverride = shipping.Id == cart.Order.ShippingAddress.Id ? null : (int?)shipping.Id;

            return PartialView("_AddUpdateShippingMethod", shippingModel);
        }

        //[HttpGet]
        //[CustomerStatusCheck]
        //public async Task<IActionResult> AddUpdateShippingMethod(bool? canadaissue = false)
        //{
        //    if (await IsCartEmpty())
        //    {
        //        return View("EmptyCart");
        //    }

        //    var cart = await GetCurrentCart();

        //    if (cart.Order.PaymentMethod?.Address?.IdCountry == null)
        //    {
        //        return RedirectToAction("AddUpdateBillingAddress");
        //    }

        //    var shippingMethodModel = new AddUpdateShippingMethodModel()
        //    {
        //        AddressType = ShippingAddressType.Residential
        //    };

        //    var loggedIn = await EnsureLoggedIn(cart);
        //    if (loggedIn != null)
        //    {
        //        if (!loggedIn.Value)
        //        {
        //            return RedirectToAction("AddUpdateShippingMethod");
        //        }
        //        var currentCustomer = cart.Order.Customer;

        //        var defaultShipping = currentCustomer.ShippingAddresses.FirstOrDefault(x => (bool?)x.SafeData.Default == true);
        //        if (cart.Order.ShippingAddress != null && cart.Order.ShippingAddress.Id != 0 &&
        //            !string.IsNullOrEmpty(cart.Order.ShippingAddress.SafeData.FirstName))
        //        {
        //            await _addressConverter.UpdateModelAsync(shippingMethodModel, cart.Order.ShippingAddress);
        //        }
        //        else if (defaultShipping != null)
        //        {
        //            await _addressConverter.UpdateModelAsync(shippingMethodModel, defaultShipping);
        //            shippingMethodModel.ShipAddressIdToOverride = defaultShipping.Id;
        //        }
        //        shippingMethodModel.IsGiftOrder = cart.Order.SafeData.GiftOrder;
        //        shippingMethodModel.GiftMessage = cart.Order.SafeData.GiftMessage;
        //        var addresses = GetShippingAddresses(cart.Order, currentCustomer);
        //        var i = 0;
        //        ViewBag.ShippingAddresses = addresses.ToDictionary(x => i++,
        //            y => $"{y.Value.SafeData.FirstName} {y.Value.SafeData.LastName} {y.Value.SafeData.Address1} {y.Key}");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Welcome");
        //    }

        //    return View(shippingMethodModel);
        //}

        [HttpPost]
        [CustomValidateAntiForgeryToken]
        public async Task<IActionResult> AddUpdateShippingMethod(AddUpdateShippingMethodModel model)
        {
            Guid? cartUid;
            using (await LockCurrentCart(out cartUid))
            {
                if (await IsCartEmpty())
                {
                    return View("EmptyCart");
                }

                if (model.UseBillingAddress)
                {
                    HashSet<string> propertyNames = new HashSet<string>(typeof(AddressModel).GetRuntimeProperties().Select(p => p.Name));
                    foreach (var item in ModelState)
                    {
                        if (propertyNames.Contains(item.Key))
                        {
                            ModelState.Remove(item.Key);
                        }
                    }
                }

                var cart = await GetCurrentCart(withMultipleShipmentsService: true);
                using (await CartLocks.LockWhenAsync(() => cartUid.HasValue && cartUid.Value != cart.CartUid, () => cart.CartUid))
                {
                    using (await OrderLocks.LockWhenAsync(() => cart.Order?.Id > 0, () => cart.Order.Id))
                    {
                        var loggedIn = await EnsureLoggedIn(cart);
                        if (loggedIn != null)
                        {
                            if (ModelState.IsValid)
                            {
                                if (cart.Order.ShippingAddress == null || cart.Order.ShippingAddress.Id == 0)
                                {
                                    if (model.UseBillingAddress)
                                    {
                                        cart.Order.ShippingAddress =
                                            await _addressConverter.FromModelAsync(
                                                await
                                                    _addressConverter.ToModelAsync<AddUpdateShippingMethodModel>(
                                                        cart.Order.PaymentMethod.Address),
                                                (int)AddressType.Shipping);
                                    }
                                    else
                                    {
                                        cart.Order.ShippingAddress =
                                            await _addressConverter.FromModelAsync(model, (int)AddressType.Shipping);
                                    }
                                    cart.Order.ShippingAddress.Id = 0;
                                }
                                else
                                {
                                    if (model.UseBillingAddress)
                                    {
                                        await
                                            _addressConverter.UpdateObjectAsync(model, cart.Order.ShippingAddress,
                                                (int)AddressType.Shipping);
                                        var billingMapped =
                                            await
                                                _addressConverter.ToModelAsync<AddUpdateShippingMethodModel>(
                                                    cart.Order.PaymentMethod.Address);
                                        await _addressConverter.UpdateObjectAsync(billingMapped, cart.Order.ShippingAddress);
                                        cart.Order.ShippingAddress.Id = model.Id;
                                    }
                                    else
                                    {
                                        var oldId = cart.Order.ShippingAddress.Id;
                                        await
                                            _addressConverter.UpdateObjectAsync(model, cart.Order.ShippingAddress,
                                                (int)AddressType.Shipping);
                                        cart.Order.ShippingAddress.Id = oldId;
                                    }
                                }

                                int? customerAddressIdToUpdate = null;
                                if (model.SaveToProfile && model.ShipAddressIdToOverride.HasValue)
                                {
                                    customerAddressIdToUpdate = model.ShipAddressIdToOverride;
                                }

                                if (model.UseBillingAddress)
                                {
                                    cart.Order.ShippingAddress.Data.PreferredShipMethod = PreferredShipMethod.Best;
                                }

                                cart.Order.Data.GiftOrder = model.IsGiftOrder;
                                if (model.IsGiftOrder)
                                {
                                    cart.Order.Data.GiftMessage = model.GiftMessage;
                                }
                                //cart.Order.Data.DeliveryInstructions = model.DeliveryInstructions;
                                await OrderService.CalculateStorefrontOrder(cart.Order, OrderStatus.Incomplete);
                                if (await CheckoutService.UpdateCart(cart, true))
                                {
                                    if (customerAddressIdToUpdate.HasValue)
                                    {
                                        var shippingAddresses = cart.Order.Customer.ShippingAddresses.ToList();

                                        var index =
                                            shippingAddresses.IndexOf(shippingAddresses.Single(x => x.Id == customerAddressIdToUpdate.Value));
                                        var originalId = shippingAddresses[index].Id;
                                        var defaultAddr = (bool?)shippingAddresses[index].SafeData.Default ?? false;
                                        cart.Order.ShippingAddress.Id = originalId;
                                        cart.Order.ShippingAddress.Data.Default = defaultAddr;
                                        shippingAddresses[index] = cart.Order.ShippingAddress;

                                        cart.Order.Customer.ShippingAddresses = shippingAddresses;

                                        cart.Order.Customer = await CustomerService.UpdateAsync(cart.Order.Customer);
                                    }
                                    else if (cart.Order.Customer.ShippingAddresses.Count == 0)
                                    {
                                        cart.Order.ShippingAddress.Id = 0;
                                        cart.Order.ShippingAddress.Data.Default = true;
                                        cart.Order.Customer.ShippingAddresses = new List<AddressDynamic> { cart.Order.ShippingAddress };

                                        cart.Order.Customer = await CustomerService.UpdateAsync(cart.Order.Customer);
                                    }

                                    if (IsCanadaShippingIssue(cart.Order.Customer, cart.Order))
                                    {
                                        return RedirectToAction("AddUpdateShippingMethod", new { canadaissue = true });
                                    }

                                    return RedirectToAction("ReviewOrder");
                                }
                            }

                            var currentCustomer = cart.Order.Customer;

                            var addresses = GetShippingAddresses(cart.Order, currentCustomer);
                            var i = 0;
                            ViewBag.ShippingAddresses = addresses.ToDictionary(x => i++,
                                y => $"{y.Value.SafeData.FirstName} {y.Value.SafeData.LastName} {y.Value.SafeData.Address1} {y.Key}");
                        }
                        else
                        {
                            return RedirectToAction("Welcome");
                        }

                        return View(model);
                    }
                }
            }
        }

        private bool IsCanadaShippingIssue(CustomerDynamic customer, OrderDynamic order)
            => customer.IdObjectType == (int)CustomerType.Retail && order.ShippingAddress.IdCountry != ReferenceData.DefaultCountry.Id;

        [HttpGet]
        [CustomerStatusCheck]
        //[CustomerAuthorize]
        public async Task<IActionResult> ReviewOrder()
        {
            if (await IsCartEmpty())
            {
                return View("EmptyCart");
            }
            var cart = await GetCurrentCart(withMultipleShipmentsService: true);
            if (cart.Order.PaymentMethod?.Address?.IdCountry == null)
            {
                return RedirectToAction("AddUpdateBillingAddress");
            }
            if (cart.Order.ShippingAddress?.IdCountry == null)
            {
                return RedirectToAction("AddUpdateShippingMethod");
            }
            var loggedIn = await EnsureLoggedIn(cart);
            if (loggedIn.HasValue)
            {
                if (!loggedIn.Value)
                {
                    return RedirectToAction("ReviewOrder");
                }
            }
            var reviewOrderModel = new ReviewOrderModel();

            await PopulateReviewModel(reviewOrderModel, cart);

            return View(reviewOrderModel);
        }

        [HttpPost]
        [CustomerStatusCheck]
        //[CustomerAuthorize]
        public async Task<Result<string>> ReviewOrder([FromBody] ViewCartModel model)
        {
            Guid? cartUid;
            using (await LockCurrentCart(out cartUid))
            {
                if (await IsCartEmpty())
                {
                    return Url.Action("ViewCart", "Cart");
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
                                return Url.Action("AddUpdateShippingMethod", "Checkout", new { canadaissue = true });
                            }

                            if (await CheckoutService.SaveOrder(cart))
                            {
                                HttpContext.Session.SetInt32(CheckoutConstants.ReceiptSessionOrderId, cart.Order.Id);
                                return Url.Action("Receipt", "Checkout");
                            }
                        }
                        else
                        {
                            return Url.Action("Welcome", "Checkout");
                        }
                        return new Result<string>(false);
                    }
                }
            }
        }

        [HttpGet]
        [CustomerStatusCheck]
        public async Task<IActionResult> Receipt()
        {
            var idOrder = HttpContext.Session.GetInt32(CheckoutConstants.ReceiptSessionOrderId);
            if (idOrder == null)
            {
                return View("EmptyCart");
            }
            var receiptModel = new ReceiptModel();

            var order = await PopulateReviewModel(receiptModel, idOrder.Value);

            receiptModel.OrderNumber = order.Id.ToString();
            receiptModel.OrderDate = order.DateCreated;

            if (order.Skus.Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                SelectMany(p => p.GcsGenerated).Any())
            {
                receiptModel.ShowEGiftEmailForm = true;
                receiptModel.EGiftSendEmail = new EGiftSendEmailModel();
                receiptModel.EGiftSendEmail.All = true;
                receiptModel.EGiftSendEmail.Codes = order.Skus.Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                    SelectMany(p => p.GcsGenerated).Select(p => new EGiftSendEmailCodeModel()
                    {
                        Code = p.Code,
                        Selected = true
                    }).ToList();
            }

            return View(receiptModel);
        }

        [HttpPost]
        [CustomerStatusCheck]
        public async Task<IActionResult> SendEGiftEmail(EGiftSendEmailModel model)
        {
            var idOrder = HttpContext.Session.GetInt32(CheckoutConstants.ReceiptSessionOrderId);
            if (!idOrder.HasValue)
            {
                model.Codes = new List<EGiftSendEmailCodeModel>();
                return PartialView("_SendEGiftEmail", model);
            }
            var order = await OrderService.SelectAsync(idOrder.Value);

            if (!model.All && (model.SelectedCodes == null || model.SelectedCodes.Count == 0))
            {
                ModelState.AddModelError("", "At least one E-Gift should be specified");
            }
            if (!ModelState.IsValid)
            {
                model.Codes = order.Skus.Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                   SelectMany(p => p.GcsGenerated).Select(p => new EGiftSendEmailCodeModel()
                   {
                       Code = p.Code,
                       Selected = false
                   }).ToList();
                return PartialView("_SendEGiftEmail", model);
            }

            var customer = await CustomerService.SelectAsync(order.Customer.Id);
            var emailModel = new EGiftNotificationEmail();
            emailModel.Sender = $"{customer.ProfileAddress.SafeData.FirstName} {customer.ProfileAddress.SafeData.LastName}";
            emailModel.Recipient = model.Recipient;
            emailModel.Email = model.Email;
            emailModel.Message = model.Message;
            emailModel.EGifts = order.Skus.Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
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
            newModel.Codes = order.Skus.Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
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
            if (currentCustomer.ShippingAddresses.Count > 0)
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

        private async Task<OrderDynamic> PopulateReviewModel(ReviewOrderModel reviewOrderModel, int idOrder)
        {
            var order = await OrderService.SelectAsync(idOrder, true);
            if (order.IdObjectType == (int)OrderType.AutoShip)
            {
                var id = (await OrderService.SelectAutoShipOrdersAsync(idOrder)).First();

                order = await OrderService.SelectAsync(id, true);
            }

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

            return order;
        }

        private struct CreateResult
        {
            public CustomerDynamic Customer;
            public Func<Task<ApplicationUser>> LoginTask;
        }

        private async Task<CreateResult> EnsureCustomerCreated(AddUpdateBillingAddressModel model, CustomerDynamic existing = null)
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
                    newCustomer = await CreateAccount(model);
                    loginTask = CreateLoginForNewGuest(newCustomer);
                }
                else
                {
                    newCustomer = await ReplaceAccount(model, existing);
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
                    newCustomer = await CreateAccount(model);
                }
                else
                {
                    newCustomer = await ReplaceAccount(model, existing);
                    if (newCustomer == null)
                    {
                        throw new ApiException("Customer couldn't be created");
                    }
                }
                loginTask = CreateLoginForNewActive(model, newCustomer.Id);
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

        private Func<Task<ApplicationUser>> CreateLoginForNewActive(AddUpdateBillingAddressModel model, int id) => async () =>
        {
            await _userService.SendSuccessfulRegistration(model.Email, model.FirstName, model.LastName);
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

        private async Task<CustomerDynamic> ReplaceAccount(AddUpdateBillingAddressModel model, CustomerDynamic existingCustomer)
        {
            if (existingCustomer == null)
                throw new ArgumentNullException(nameof(existingCustomer));
            UpdateAccount(model, existingCustomer);
            if (existingCustomer.ShippingAddresses == null)
                existingCustomer.ShippingAddresses = new List<AddressDynamic>();

            await _addressConverter.UpdateObjectAsync(model, existingCustomer.ProfileAddress, (int)AddressType.Profile);
            var shipping = existingCustomer.ShippingAddresses.FirstOrDefault();
            if (shipping == null)
            {
                shipping = _addressConverter.CreatePrototype((int)AddressType.Shipping);
                existingCustomer.ShippingAddresses.Add(shipping);
            }
            await _addressConverter.UpdateObjectAsync(model, shipping, (int)AddressType.Shipping);
            shipping.Data.Default = true;
            existingCustomer =
                await CustomerService.UpdateAsync(existingCustomer, model.GuestCheckout ? null : model.Password);
            return existingCustomer;
        }

        private void UpdateAccount(AddUpdateBillingAddressModel model, CustomerDynamic newCustomer)
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

        private async Task<CustomerDynamic> CreateAccount(AddUpdateBillingAddressModel model)
        {
            var newCustomer = CustomerService.Mapper.CreatePrototype((int)CustomerType.Retail);
            newCustomer.PublicId = Guid.NewGuid();
            UpdateAccount(model, newCustomer);
            newCustomer.ProfileAddress = await _addressConverter.FromModelAsync(model, (int)AddressType.Profile);
            newCustomer.ProfileAddress.Id = 0;
            var shipping = await _addressConverter.FromModelAsync(model, (int)AddressType.Shipping);
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