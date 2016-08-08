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
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Mailings;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.Core.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Cart;
using VitalChoice.Infrastructure.Domain.Transfer.Country;
using VitalChoice.Infrastructure.Identity.UserManagers;
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
        private readonly IStorefrontUserService _storefrontUserService;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _orderPaymentMethodConverter;
        private readonly IDynamicMapper<AddressDynamic, Address> _addressConverter;
        private readonly BrontoService _brontoService;
        private readonly ITransactionAccessor<EcommerceContext> _transactionAccessor;
        private readonly IAffiliateService _affiliateService;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly ICountryNameCodeResolver _countryNameCodeResolver;

        public CheckoutController(IStorefrontUserService storefrontUserService,
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
            ITransactionAccessor<EcommerceContext> transactionAccessor,
            IPageResultService pageResultService, ISettingService settingService, ILoggerProviderExtended loggerProvider,
            ExtendedUserManager userManager, ICountryNameCodeResolver countryNameCodeResolver, ReferenceData referenceData,
            AppSettings appSettings)
            : base(
                customerService, referenceData, authorizationService, checkoutService, orderService,
                skuMapper, productMapper, pageResultService, settingService, userManager, appSettings)
        {
            _storefrontUserService = storefrontUserService;
            _paymentMethodConverter = paymentMethodConverter;
            _productService = productService;
            _addressConverter = addressConverter;
            _orderPaymentMethodConverter = orderPaymentMethodConverter;
            _brontoService = brontoService;
            _transactionAccessor = transactionAccessor;
            _countryNameCodeResolver = countryNameCodeResolver;
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
                var currentCustomer = await GetCurrentCustomerDynamic();

                var creditCard = currentCustomer.CustomerPaymentMethods
                    .Single(p => p.IdObjectType == (int) PaymentMethodType.CreditCard && p.Id == id);

                var billingInfoModel = await _addressConverter.ToModelAsync<AddUpdateBillingAddressModel>(creditCard.Address);
                await _paymentMethodConverter.UpdateModelAsync<BillingInfoModel>(billingInfoModel, creditCard);

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
                        await _addressConverter.UpdateModelAsync(addUpdateModel, firstCreditCard.Address);
                        await _paymentMethodConverter.UpdateModelAsync<BillingInfoModel>(addUpdateModel, firstCreditCard);
                    }
                    else
                    {
                        await _addressConverter.UpdateModelAsync(addUpdateModel, cart.Order.PaymentMethod.Address);
                        await _orderPaymentMethodConverter.UpdateModelAsync<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                    }

                    await PopulateCreditCardsLookup();
                }
                else
                {
                    if (cart.Order.PaymentMethod?.Address != null && cart.Order.PaymentMethod.Id != 0)
                    {
                        await _addressConverter.UpdateModelAsync(addUpdateModel, cart.Order.PaymentMethod.Address);
                        await _orderPaymentMethodConverter.UpdateModelAsync<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                    }
                }

                addUpdateModel.Email = currentCustomer.Email;
            }
            else
            {
                if (cart.Order.PaymentMethod?.Address != null && cart.Order.PaymentMethod.Id != 0)
                {
                    await _addressConverter.UpdateModelAsync(addUpdateModel, cart.Order.PaymentMethod.Address);
                    await _orderPaymentMethodConverter.UpdateModelAsync<BillingInfoModel>(addUpdateModel, cart.Order.PaymentMethod);
                }
                addUpdateModel.Email = cart.Order.Customer?.Email;
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
                            await _orderPaymentMethodConverter.FromModelAsync(model, (int) PaymentMethodType.CreditCard);
                        cart.Order.PaymentMethod.Address = await _addressConverter.FromModelAsync(model, (int) AddressType.Billing);
                    }
                    else
                    {
                        await
                            _orderPaymentMethodConverter.UpdateObjectAsync(model, cart.Order.PaymentMethod,
                                (int) PaymentMethodType.CreditCard);
                        if (cart.Order.PaymentMethod.Address == null)
                        {
                            cart.Order.PaymentMethod.Address = new AddressDynamic {IdObjectType = (int) AddressType.Billing};
                        }
                        await _addressConverter.UpdateObjectAsync(model, cart.Order.PaymentMethod.Address, (int) AddressType.Billing);
                    }
                    if (ObjectMapper.IsValuesMasked(typeof(OrderPaymentMethodDynamic), (string)cart.Order.PaymentMethod.Data.CardNumber,
                        "CardNumber"))
                    {
                        //BUG: SECURITY!!! do not use real ID here, look to rework synthetic generated id with matching from customer profile
                        //BUG: The issue allows to replace payment ID and use card data from different customer (this usage would be hidden from admin UI)
                        cart.Order.PaymentMethod.IdCustomerPaymentMethod = model.Id;
                    }
                    if (await CheckoutService.UpdateCart(cart))
                    {
                        if (loginTask != null)
                            await loginTask();
                        transaction.Commit();

                        if (!string.IsNullOrEmpty(model.Email))
                        {
                            _brontoService.PushSubscribe(model.Email, model.SendNews);
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
                catch
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
        [CustomerStatusCheck]
        public async Task<IActionResult> GetShippingAddress(int id)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();
            var cart = await GetCurrentCart();
            var addresses = GetShippingAddresses(cart.Order, currentCustomer);
            var shipping = addresses[id].Value;

            var shippingModel = new AddUpdateShippingMethodModel();

            await _addressConverter.UpdateModelAsync<ShippingInfoModel>(shippingModel, shipping);

			shippingModel.ShipAddressIdToOverride = shipping.Id == cart.Order.ShippingAddress.Id ? null : (int?)shipping.Id;

			return PartialView("_AddUpdateShippingMethod", shippingModel);
        }

        [HttpGet]
        [CustomerStatusCheck]
        public async Task<IActionResult> AddUpdateShippingMethod()
        {
            if (await IsCartEmpty())
            {
                return View("EmptyCart");
            }

            var shippingMethodModel = new AddUpdateShippingMethodModel()
            {
                AddressType = ShippingAddressType.Residential
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
                    await _addressConverter.UpdateModelAsync(shippingMethodModel, cart.Order.ShippingAddress);
                }
                else if (defaultShipping != null)
                {
                    await _addressConverter.UpdateModelAsync(shippingMethodModel, defaultShipping);
					shippingMethodModel.ShipAddressIdToOverride = defaultShipping.Id;
				}
                shippingMethodModel.IsGiftOrder = cart.Order.SafeData.GiftOrder;
                shippingMethodModel.GiftMessage = cart.Order.SafeData.GiftMessage;
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
                foreach (var item in ModelState)
                {
                    if (propertyNames.Contains(item.Key))
                    {
                        ModelState.Remove(item.Key);
                    }
                }
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
                                    await _addressConverter.ToModelAsync<AddUpdateShippingMethodModel>(cart.Order.PaymentMethod.Address),
                                    (int) AddressType.Shipping);
                        }
                        else
                        {
                            cart.Order.ShippingAddress = await _addressConverter.FromModelAsync(model, (int) AddressType.Shipping);
                        }
                        cart.Order.ShippingAddress.Id = 0;
                    }
                    else
                    {
                        if (model.UseBillingAddress)
                        {
                            await _addressConverter.UpdateObjectAsync(model, cart.Order.ShippingAddress, (int) AddressType.Shipping);
                            var billingMapped = await _addressConverter.ToModelAsync<AddUpdateShippingMethodModel>(cart.Order.PaymentMethod.Address);
                            await _addressConverter.UpdateObjectAsync(billingMapped, cart.Order.ShippingAddress);
                            cart.Order.ShippingAddress.Id = model.Id;
                        }
                        else
                        {
                            var oldId = cart.Order.ShippingAddress.Id;
                            await _addressConverter.UpdateObjectAsync(model, cart.Order.ShippingAddress, (int) AddressType.Shipping);
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
                    if (await CheckoutService.UpdateCart(cart))
                    {
                        if (customerAddressIdToUpdate.HasValue)
                        {
                            var shippingAddresses = cart.Order.Customer.ShippingAddresses.ToList();

                            var index = shippingAddresses.IndexOf(shippingAddresses.Single(x => x.Id == customerAddressIdToUpdate.Value));
                            var originalId = shippingAddresses[index].Id;
                            var defaultAddr = shippingAddresses[index].Data.Default;
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
                            cart.Order.Customer.ShippingAddresses = new List<AddressDynamic> {cart.Order.ShippingAddress};

                            cart.Order.Customer = await CustomerService.UpdateAsync(cart.Order.Customer);
                        }
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

            if (order.Skus.Where(p => p.Sku.Product.IdObjectType == (int) ProductType.EGс).
                SelectMany(p => p.GcsGenerated).Any())
            {
                receiptModel.ShowEGiftEmailForm = true;
                receiptModel.EGiftSendEmail = new EGiftSendEmailModel();
                receiptModel.EGiftSendEmail.All = true;
                receiptModel.EGiftSendEmail.Codes = order.Skus.Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                    SelectMany(p => p.GcsGenerated).Select(p => new EGiftSendEmailCodeModel()
                    {
                        Code = p.Code,
                    }).ToList(); 
            }

            return View(receiptModel);
        }

        [HttpPost]
        [CustomerStatusCheck]
        public async Task<IActionResult> SendEGiftEmail(EGiftSendEmailModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var idOrder = HttpContext.Session.GetInt32(CheckoutConstants.ReceiptSessionOrderId);
            if (!idOrder.HasValue)
            {
                return PartialView("_SendEGiftEmail", model);
            }
            
            var order = await OrderService.SelectAsync(idOrder.Value);
            var customer = await CustomerService.SelectAsync(order.Customer.Id);
            var emailModel = new EGiftNotificationEmail();
            emailModel.Sender = $"{customer.ProfileAddress.SafeData.FirstName} {customer.ProfileAddress.SafeData.LastName}";
            emailModel.Recipient = model.Recipient;
            emailModel.Email = model.Email;
            emailModel.Message = model.Message;
            emailModel.EGifts = order.Skus.Where(p=>p.Sku.Product.IdObjectType==(int)ProductType.EGс).
                SelectMany(p => p.GcsGenerated).Where(p=>model.All || model.SelectedCodes.Contains(p.Code)).Select(p => new EGiftEmailModel()
                {
                    Code = p.Code,
                    Amount = p.Balance
                }).ToList();
            await _notificationService.SendEGiftNotificationEmailAsync(model.Email, emailModel);

            ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullySent];
            ModelState.Clear();
            var newModel = new EGiftSendEmailModel();
            newModel.All = false;
            newModel.Codes = order.Skus.Where(p => p.Sku.Product.IdObjectType == (int)ProductType.EGс).
                    SelectMany(p => p.GcsGenerated).Select(p => new EGiftSendEmailCodeModel()
                    {
                        Code = p.Code,
                    }).ToList();
            return PartialView("_SendEGiftEmail", newModel);
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
                    currentCustomer.ShippingAddresses.OrderByDescending(a => (bool?) a.SafeData.Default ?? false).Select(
                        a => new KeyValuePair<string, AddressDynamic>(((bool?) a.SafeData.Default ?? false) ? "(Default)" : string.Empty, a)));
            }
            return shippingAddresses;
        }

        private async Task<CustomerCartOrder> PopulateReviewModel(ReviewOrderModel reviewOrderModel, CustomerCartOrder cart)
        {
            await InitCartModelInternal(reviewOrderModel);

            var paymentMethod = cart.Order.PaymentMethod;
            reviewOrderModel.BillToAddress = paymentMethod.Address.PopulateBillingAddressDetails(_countryNameCodeResolver, cart.Order.Customer.Email);
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
            reviewOrderModel.BillToAddress = paymentMethod.Address.PopulateBillingAddressDetails(_countryNameCodeResolver, order.Customer.Email);
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
                shipping = _addressConverter.CreatePrototype((int) AddressType.Shipping);
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
            var newCustomer = CustomerService.Mapper.CreatePrototype((int) CustomerType.Retail);
            newCustomer.PublicId = Guid.NewGuid();
            UpdateAccount(model, newCustomer);
            newCustomer.ProfileAddress = await _addressConverter.FromModelAsync(model, (int) AddressType.Profile);
            newCustomer.ProfileAddress.Id = 0;
            var shipping = await _addressConverter.FromModelAsync(model, (int) AddressType.Shipping);
            shipping.Id = 0;
            shipping.Data.Default = true;
            newCustomer.ShippingAddresses = new List<AddressDynamic> {shipping};

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