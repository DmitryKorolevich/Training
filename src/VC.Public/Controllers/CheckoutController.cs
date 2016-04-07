using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
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
using Microsoft.AspNet.Mvc.ModelBinding;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.Core.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.SharedWeb.Helpers;
using VitalChoice.Validation.Models;
using ApiException = VitalChoice.Ecommerce.Domain.Exceptions.ApiException;

namespace VC.Public.Controllers
{
    public class CheckoutController : CheckoutControllerBase
    {
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _orderPaymentMethodConverter;
        private readonly IDynamicMapper<AddressDynamic, Address> _addressConverter;
        private readonly ReferenceData _appInfrastructure;
        private readonly ICountryService _countryService;
        private readonly BrontoService _brontoService;
        private readonly ITransactionAccessor<EcommerceContext> _transactionAccessor;

        public CheckoutController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
            ICustomerService customerService,
            IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
            IOrderService orderService, IProductService productService, IAppInfrastructureService infrastructureService,
            IAuthorizationService authorizationService, ICheckoutService checkoutService,
            IDynamicMapper<AddressDynamic, Address> addressConverter, IAppInfrastructureService appInfrastructureService,
            IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> orderPaymentMethodConverter,
            IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<ProductDynamic, Product> productMapper, 
            ICountryService countryService,
            BrontoService brontoService,
            ITransactionAccessor<EcommerceContext> transactionAccessor,
            IPageResultService pageResultService, ISettingService settingService)
            : base(
                contextAccessor, customerService, infrastructureService, authorizationService, checkoutService, orderService,
                skuMapper, productMapper, pageResultService, settingService)
        {
            _storefrontUserService = storefrontUserService;
            _paymentMethodConverter = paymentMethodConverter;
            _productService = productService;
            _addressConverter = addressConverter;
            _orderPaymentMethodConverter = orderPaymentMethodConverter;
            _countryService = countryService;
            _brontoService = brontoService;
            _transactionAccessor = transactionAccessor;
            _appInfrastructure = appInfrastructureService.Get();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Welcome(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            ApplicationUser user = null;
            try
            {
                user = await _storefrontUserService.SignInAsync(model.Email, model.Password);
            }
            catch (WholesalePendingException e)
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
                var currentCustomer = await GetCurrentCustomerDynamic();

                var creditCard = currentCustomer.CustomerPaymentMethods
                    .Single(p => p.IdObjectType == (int) PaymentMethodType.CreditCard && p.Id == id);

                var billingInfoModel = _addressConverter.ToModel<AddUpdateBillingAddressModel>(creditCard.Address);
                _paymentMethodConverter.UpdateModel<BillingInfoModel>(billingInfoModel, creditCard);

                billingInfoModel.Email = currentCustomer.Email;

                return PartialView("_AddUpdateBillingAddress", billingInfoModel);
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
                var currentCustomer = await GetCurrentCustomerDynamic();

                var creditCards = currentCustomer.CustomerPaymentMethods
                    .Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard).ToList();

	            var firstCreditCard = creditCards.FirstOrDefault(x => (bool?) x.SafeData.Default == true) ??
	                                  creditCards.FirstOrDefault();

	            if (firstCreditCard != null)
                {
                    if (cart.Order.PaymentMethod?.Address == null || cart.Order.PaymentMethod.Id == 0)
                    {
                        _addressConverter.UpdateModel(addUpdateModel, firstCreditCard.Address);
                        _paymentMethodConverter.UpdateModel<BillingInfoModel>(addUpdateModel, firstCreditCard);
                    }
                    else
                    {
                        _addressConverter.UpdateModel(addUpdateModel, cart.Order.PaymentMethod.Address);
                        _orderPaymentMethodConverter.UpdateModel<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                    }

                    await PopulateCreditCardsLookup();
                }
                else
                {
                    if (cart.Order.PaymentMethod?.Address != null && cart.Order.PaymentMethod.Id != 0)
                    {
                        _addressConverter.UpdateModel(addUpdateModel, cart.Order.PaymentMethod.Address);
                        _orderPaymentMethodConverter.UpdateModel<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                    }
                }

                addUpdateModel.Email = currentCustomer.Email;
            }
            else
            {
                if (cart.Order.PaymentMethod?.Address != null && cart.Order.PaymentMethod.Id != 0)
                {
                    _addressConverter.UpdateModel(addUpdateModel, cart.Order.PaymentMethod.Address);
                    _orderPaymentMethodConverter.UpdateModel<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                }
                addUpdateModel.Email = cart.Order.Customer?.Email;
            }

            if (!string.IsNullOrEmpty(addUpdateModel.Email))
            {
                addUpdateModel.SendNews = !(_brontoService.GetIsUnsubscribed(addUpdateModel.Email) ?? false);
            }

            return View(addUpdateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUpdateBillingAddress(AddUpdateBillingAddressModel model)
        {
            if (await IsCartEmpty())
            {
                return View("EmptyCart");
            }
            var loggedIn = await CustomerLoggedIn();
            var cart = await GetCurrentCart(loggedIn);

            if (!ModelState.IsValid)
            {
                if (loggedIn)
                {
                    await PopulateCreditCardsLookup();
                }
                return View(model);
            }
            Func<Task<ApplicationUser>> loginTask = null;
            using (var transaction = _transactionAccessor.BeginTransaction())
            {
                try
                {
                    if (loggedIn)
                    {
                       await PopulateCreditCardsLookup();
                    }
                    else
                    {
                        var createResult = await EnsureCustomerCreated(model, cart.Order.Customer);
                        cart.Order.Customer = createResult.Customer;
                        loginTask = createResult.LoginTask;
                    }
                    if (model.SendCatalog)
                    {
                        var pnc = await _productService.GetSkuOrderedAsync("pnc");
                        if (pnc != null)
                        {
                            pnc.Quantity = 1;
                            pnc.Amount = pnc.Sku.Price;
                            cart.Order.Skus.AddKeyed(Enumerable.Repeat(pnc, 1), ordered => ordered.Sku.Code);
                        }
                    }
                    else
                    {
                        cart.Order.Skus.RemoveAll(s => s.Sku.Code.ToLower() == "pnc");
                    }
                    if (cart.Order.PaymentMethod?.Address == null || cart.Order.PaymentMethod.Id == 0)
                    {
                        cart.Order.PaymentMethod =
                            await _orderPaymentMethodConverter.FromModelAsync((BillingInfoModel) model, (int) PaymentMethodType.CreditCard);
                        cart.Order.PaymentMethod.Address = await _addressConverter.FromModelAsync(model, (int) AddressType.Billing);
                    }
                    else
                    {
                        await
                            _orderPaymentMethodConverter.UpdateObjectAsync((BillingInfoModel) model, cart.Order.PaymentMethod,
                                (int) PaymentMethodType.CreditCard);
                        if (cart.Order.PaymentMethod.Address == null)
                        {
                            cart.Order.PaymentMethod.Address = new AddressDynamic {IdObjectType = (int) AddressType.Billing};
                        }
                        await _addressConverter.UpdateObjectAsync(model, cart.Order.PaymentMethod.Address, (int) AddressType.Billing);
                    }
                    if (await CheckoutService.UpdateCart(cart))
                    {
                        if (loginTask != null)
                            await loginTask();
                        transaction.Commit();

                        if (!string.IsNullOrEmpty(model.Email))
                        {
                            var unsubscribed = _brontoService.GetIsUnsubscribed(model.Email);
                            if (model.SendNews && (!unsubscribed.HasValue || unsubscribed.Value))
                            {
                                await _brontoService.Subscribe(model.Email);
                            }
                            if (!model.SendNews)
                            {
                                if (!unsubscribed.HasValue)
                                {
                                    await _brontoService.Subscribe(model.Email);
                                    _brontoService.Unsubscribe(model.Email);
                                }
                                else if (!unsubscribed.Value)
                                {
                                    _brontoService.Unsubscribe(model.Email);
                                }
                            }
                        }

                        return RedirectToAction("AddUpdateShippingMethod");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Cannot update order");
                        if (loginTask != null)
                        {
                            if (cart.Order.Customer != null && cart.Order.Customer.Id != 0)
                            {
                                await _storefrontUserService.RemoveAsync(cart.Order.Customer.Id);
                            }
                        }
                        transaction.Rollback();
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
                    if (loginTask != null)
                    {
                        if (cart.Order.Customer != null && cart.Order.Customer.Id != 0)
                        {
                            await _storefrontUserService.RemoveAsync(cart.Order.Customer.Id);
                        }
                    }
                    throw new AppValidationException(newMessages);
                }
                catch (Exception)
                {
                    transaction.Rollback();
                    if (loginTask != null)
                    {
                        if (cart.Order.Customer != null && cart.Order.Customer.Id != 0)
                        {
                            await _storefrontUserService.RemoveAsync(cart.Order.Customer.Id);
                        }
                    }
                    throw;
                }
            }

            return View(model);
        }

        [HttpGet]
        [CustomerAuthorize]
        [CustomerStatusCheck]
        public async Task<IActionResult> GetShippingAddress(int id)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();
            var cart = await GetCurrentCart();
            var addresses = GetShippingAddresses(cart.Order, currentCustomer);
            var shipping = addresses[id].Value;

            var shippingModel = new AddUpdateShippingMethodModel();

            _addressConverter.UpdateModel<ShippingInfoModel>(shippingModel, shipping);

            return PartialView("_AddUpdateShippingMethod", shippingModel);
        }

        [HttpGet]
        [CustomerStatusCheck]
        //[CustomerAuthorize]
        public async Task<IActionResult> AddUpdateShippingMethod()
        {
            if (await IsCartEmpty())
            {
                return View("EmptyCart");
            }

            var shippingMethodModel = new AddUpdateShippingMethodModel()
            {
                AddressType = CheckoutAddressType.Residental
            };
            var cart = await GetCurrentCart();
            var loggedIn = await EnsureLoggedIn(cart);
            if (loggedIn != null)
            {
                if (!loggedIn.Value)
                {
                    return RedirectToAction("AddUpdateShippingMethod");
                }
                var currentCustomer = await GetCurrentCustomerDynamic();

                var defaultShipping = currentCustomer.ShippingAddresses.FirstOrDefault(x => (bool?) x.SafeData.Default == true);
                if (cart.Order.ShippingAddress != null && cart.Order.ShippingAddress.Id != 0 &&
                    !string.IsNullOrEmpty(cart.Order.ShippingAddress.SafeData.FirstName))
                {
                    _addressConverter.UpdateModel<ShippingInfoModel>(shippingMethodModel, cart.Order.ShippingAddress);
                }
                else if (defaultShipping != null)
                {
                    _addressConverter.UpdateModel<ShippingInfoModel>(shippingMethodModel, defaultShipping);
                }
                //else
                //{
                //    _addressConverter.UpdateModel<ShippingInfoModel>(shippingMethodModel, currentCustomer.ProfileAddress);
                //}
                shippingMethodModel.IsGiftOrder = cart.Order.SafeData.GiftOrder;
                shippingMethodModel.GiftMessage = cart.Order.SafeData.GiftMessage;
                shippingMethodModel.DeliveryInstructions = cart.Order.SafeData.DeliveryInstructions;
                var addresses = GetShippingAddresses(cart.Order, currentCustomer);
                var i = 0;
                ViewBag.ShippingAddresses = addresses.ToDictionary(x => i++,
                    y => $"{y.Value.Data.FirstName} {y.Value.Data.LastName} {y.Value.Data.Address1} {y.Key}");
            }
            else
            {
                return RedirectToAction("Welcome");
            }

            return View(shippingMethodModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUpdateShippingMethod(AddUpdateShippingMethodModel model)
        {
            if (await IsCartEmpty())
            {
                return View("EmptyCart");
            }

            if (model.UseBillingAddress)
            {
                HashSet<string> propertyNames = new HashSet<string>(typeof (AddressModel).GetRuntimeProperties().Select(p => p.Name));
                ModelState.RemoveAll(pair => propertyNames.Contains(pair.Key));
            }
            var cart = await GetCurrentCart();
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
                                    _addressConverter.ToModel<AddUpdateShippingMethodModel>(cart.Order.PaymentMethod.Address),
                                    (int) AddressType.Shipping);

                        }
                        else
                        {
                            cart.Order.ShippingAddress = await _addressConverter.FromModelAsync(model, (int) AddressType.Shipping);
                        }
                    }
                    else
                    {
                        if (model.UseBillingAddress)
                        {
                            await _addressConverter.UpdateObjectAsync(
                                _addressConverter.ToModel<AddUpdateShippingMethodModel>(cart.Order.PaymentMethod.Address),
                                cart.Order.ShippingAddress, (int) AddressType.Shipping);
                        }
                        else
                        {
                            await _addressConverter.UpdateObjectAsync(model, cart.Order.ShippingAddress, (int) AddressType.Shipping);
                        }
                    }
                    cart.Order.Data.GiftOrder = model.IsGiftOrder;
                    cart.Order.Data.GiftMessage = model.GiftMessage;
                    cart.Order.Data.DeliveryInstructions = model.DeliveryInstructions;
                    await OrderService.CalculateOrder(cart.Order, OrderStatus.Incomplete);
                    if (await CheckoutService.UpdateCart(cart))
                    {
                        return RedirectToAction("ReviewOrder");
                    }
                }

                var currentCustomer = await GetCurrentCustomerDynamic();

                var addresses = GetShippingAddresses(cart.Order, currentCustomer);
                var i = 0;
                ViewBag.ShippingAddresses = addresses.ToDictionary(x => i++,
                    y => $"{y.Value.Data.FirstName} {y.Value.Data.LastName} {y.Value.Data.Address1} {y.Key}");
            }
            else
            {
                return RedirectToAction("Welcome");
            }

            return View(model);
        }

        [HttpGet]
        [CustomerStatusCheck]
        //[CustomerAuthorize]
        public async Task<IActionResult> ReviewOrder()
        {
            if (await IsCartEmpty())
            {
                return View("EmptyCart");
            }
            var cart = await GetCurrentCart();
            var loggedIn = await EnsureLoggedIn(cart);
            if (loggedIn != null)
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
            var cart = await GetCurrentCart();
            var loggedIn = await EnsureLoggedIn(cart);
            if (loggedIn != null)
            {
                if (await CheckoutService.SaveOrder(cart))
                {
                    ContextAccessor.HttpContext.Session.SetInt32(CheckoutConstants.ReceiptSessionOrderId, cart.Order.Id);
                    return Url.Action("Receipt", "Checkout");
                }
            }
            else
            {
                return Url.Action("Welcome", "Checkout");
            }
            return new Result<string>(false);
        }

        [HttpGet]
        [CustomerStatusCheck]
        public async Task<IActionResult> Receipt()
        {
            var idOrder = ContextAccessor.HttpContext.Session.GetInt32(CheckoutConstants.ReceiptSessionOrderId);
            if (idOrder == null)
            {
                return View("EmptyCart");
            }
            var receiptModel = new ReceiptModel();

            var order = await PopulateReviewModel(receiptModel, idOrder.Value);

            receiptModel.OrderNumber = order.Id.ToString();
            receiptModel.OrderDate = order.DateCreated;

            return View(receiptModel);
        }

        private List<KeyValuePair<string, AddressDynamic>> GetShippingAddresses(OrderDynamic order, CustomerDynamic currentCustomer)
        {
            List<KeyValuePair<string, AddressDynamic>> shippingAddresses = new List<KeyValuePair<string, AddressDynamic>>();
            if (order.ShippingAddress != null && order.ShippingAddress.Id != 0 &&
                !string.IsNullOrEmpty(order.ShippingAddress.SafeData.FirstName))
            {
                shippingAddresses.Add(new KeyValuePair<string, AddressDynamic>("(Currently On Order)", order.ShippingAddress));
            }
            if (currentCustomer.ShippingAddresses.Any())
            {
                shippingAddresses.AddRange(
                    currentCustomer.ShippingAddresses.OrderByDescending(a => (bool)a.Data.Default).Select(
                        a => new KeyValuePair<string, AddressDynamic>((bool)a.Data.Default ? "(Default)" : string.Empty, a)));
            }
            return shippingAddresses;
        }

        private async Task<CustomerCartOrder> PopulateReviewModel(ReviewOrderModel reviewOrderModel, CustomerCartOrder cart)
        {
            await InitCartModelInternal(reviewOrderModel);

            var countries = await _countryService.GetCountriesAsync();

            var paymentMethod = cart.Order.PaymentMethod;
            reviewOrderModel.BillToAddress = paymentMethod.Address.PopulateBillingAddressDetails(countries, cart.Order.Customer.Email);
            reviewOrderModel.CreditCardDetails = paymentMethod.PopulateCreditCardDetails(_appInfrastructure);

            var shippingAddress = cart.Order.ShippingAddress;
            reviewOrderModel.ShipToAddress = shippingAddress.PopulateShippingAddressDetails(countries);

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
            var context = await OrderService.CalculateOrder(order, OrderStatus.Processed);
            await FillModel(reviewOrderModel, order, context);

            var countries = await _countryService.GetCountriesAsync();

            var paymentMethod = order.PaymentMethod;
            reviewOrderModel.BillToAddress = paymentMethod.Address.PopulateBillingAddressDetails(countries, order.Customer.Email);
            reviewOrderModel.CreditCardDetails = paymentMethod.PopulateCreditCardDetails(_appInfrastructure);

            var shippingAddress = order.ShippingAddress;
            reviewOrderModel.ShipToAddress = shippingAddress.PopulateShippingAddressDetails(countries);

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
                existing = await CustomerService.GetByEmailAsync(model.Email);
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
                existing = await CustomerService.GetByEmailAsync(model.Email);
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
                loginTask = CreateLoginForNewActive(model);
            }
            return new CreateResult
            {
                Customer = newCustomer,
                LoginTask = loginTask
            };
        }

        private Func<Task<ApplicationUser>> CreateLoginForExistingGuest(CustomerDynamic newCustomer)
        {
            return async () =>
            {
                var user = await _storefrontUserService.GetAsync(newCustomer.Id);
                user = await _storefrontUserService.SignInNoStatusCheckingAsync(user);
                if (user == null)
                {
                    throw new AppValidationException(
                        ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
                }
                return user;
            };
        }

        private Func<Task<ApplicationUser>> CreateLoginForNewGuest(CustomerDynamic newCustomer)
        {
            return async () =>
            {
                await _storefrontUserService.SendActivationAsync(newCustomer.Email);
                var user = await _storefrontUserService.GetAsync(newCustomer.Id);
                user = await _storefrontUserService.SignInNoStatusCheckingAsync(user);
                if (user == null)
                {
                    throw new AppValidationException(
                        ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
                }
                return user;
            };
        }

        private Func<Task<ApplicationUser>> CreateLoginForNewActive(AddUpdateBillingAddressModel model)
        {
            return async () =>
            {
                await _storefrontUserService.SendSuccessfulRegistration(model.Email, model.FirstName, model.LastName);
                var user = await _storefrontUserService.SignInAsync(model.Email, model.Password);
                if (user == null)
                {
                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantSignIn]);
                }
                return user;
            };
        }

        private async Task<bool?> EnsureLoggedIn(CustomerCartOrder cart)
        {
            if (await CustomerLoggedIn())
            {
                return true;
            }

            if ((bool?) cart.Order.Customer?.SafeData.Guest ?? false)
            {
                var user = await _storefrontUserService.GetAsync(cart.Order.Customer.Id);
                user = await _storefrontUserService.SignInNoStatusCheckingAsync(user);
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

            await _addressConverter.UpdateObjectAsync(model, existingCustomer.ProfileAddress, (int) AddressType.Profile);
            var shipping = existingCustomer.ShippingAddresses.FirstOrDefault();
            if (shipping == null)
            {
                shipping = await _addressConverter.CreatePrototypeAsync((int) AddressType.Shipping);
                existingCustomer.ShippingAddresses.Add(shipping);
            }
            await _addressConverter.UpdateObjectAsync(model, shipping, (int) AddressType.Shipping);
            shipping.Data.Default = true;
            existingCustomer =
                await CustomerService.UpdateAsync(existingCustomer, model.GuestCheckout ? null : model.Password);
            return existingCustomer;
        }

        private void UpdateAccount(AddUpdateBillingAddressModel model, CustomerDynamic newCustomer)
        {
            newCustomer.IdDefaultPaymentMethod = (int) PaymentMethodType.CreditCard;
            if (model.GuestCheckout)
            {
                newCustomer.Data.Guest = true;
                newCustomer.StatusCode = (int) CustomerStatus.PhoneOnly;
            }
            else
            {
                newCustomer.StatusCode = (int) CustomerStatus.Active;
            }
            newCustomer.Email = model.Email;
            newCustomer.ApprovedPaymentMethods = new List<int> {(int) PaymentMethodType.CreditCard};
        }

        private async Task<CustomerDynamic> CreateAccount(AddUpdateBillingAddressModel model)
        {
            var newCustomer = await CustomerService.Mapper.CreatePrototypeAsync((int) CustomerType.Retail);
            newCustomer.PublicId = Guid.NewGuid();
            UpdateAccount(model, newCustomer);
            newCustomer.ProfileAddress = await _addressConverter.FromModelAsync(model, (int) AddressType.Profile);
            newCustomer.ProfileAddress.Id = 0;
            var shipping = await _addressConverter.FromModelAsync(model, (int) AddressType.Shipping);
            shipping.Id = 0;
            shipping.Data.Default = true;
            newCustomer.ShippingAddresses = new List<AddressDynamic> {shipping};
            newCustomer =
                await CustomerService.InsertAsync(newCustomer, model.GuestCheckout ? null : model.Password);
            return newCustomer;
        }
    }
}