﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using VC.Public.Models.Profile;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Healthwise;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Ecommerce.Utils;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Help;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Validation.Models;
using VitalChoice.Interfaces.Services.Help;
using VitalChoice.SharedWeb.Models.Help;
using VitalChoice.Interfaces.Services.Healthwise;
using VitalChoice.Infrastructure.Domain.Transfer.Healthwise;
using VitalChoice.Infrastructure.Domain.Entities.Healthwise;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Checkout;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Content.ContentCrossSells;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.SharedWeb.Helpers;
using VitalChoice.SharedWeb.Models.Orders;

namespace VC.Public.Controllers
{
    [CustomerAuthorize]
    [CustomerStatusCheck]
    public class ProfileController : PublicControllerBase
    {
        private const string TicketCommentMessageTempData = "ticket-comment-messsage";
        private const string ProductBaseUrl = "/product/";

        private readonly IStorefrontUserService _storefrontUserService;
        private readonly IDynamicMapper<AddressDynamic, Address> _addressConverter;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _customerPaymentMethodConverter;
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _orderPaymentMethodConverter;
        private readonly ExtendedUserManager _userManager;
        private readonly IDynamicMapper<OrderDynamic, Order> _orderConverter;
        private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
        private readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
        private readonly IDynamicMapper<OrderDynamic, Order> _orderMapper;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IHelpService _helpService;
        private readonly IHealthwiseService _healthwiseService;
        private readonly ICountryService _countryService;
        private readonly IContentCrossSellService _contentCrossSellService;
        private readonly ILogger _logger;

        public ProfileController(IStorefrontUserService storefrontUserService,
            ICustomerService customerService, IDynamicMapper<AddressDynamic, Address> addressConverter,
            IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter,
            IDynamicMapper<OrderDynamic, Order> orderConverter,
            IOrderService orderService,
            IProductService productService,
            IHelpService helpService,
            IHealthwiseService healthwiseService, IAppInfrastructureService infrastructureService,
            IAuthorizationService authorizationService, ICheckoutService checkoutService,
            ILoggerProviderExtended loggerProvider,
            IPageResultService pageResultService, IDynamicMapper<SkuDynamic, Sku> skuMapper,
            IDynamicMapper<ProductDynamic, Product> productMapper, ICountryService countryService,
            IDynamicMapper<OrderDynamic, Order> orderMapper,
            IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> orderPaymentMethodConverter, ExtendedUserManager userManager, IContentCrossSellService contentCrossSellService)
            : base(customerService, infrastructureService, authorizationService, checkoutService, pageResultService, userManager)
        {
            _storefrontUserService = storefrontUserService;
            _addressConverter = addressConverter;
            _customerPaymentMethodConverter = paymentMethodConverter;
            _orderService = orderService;
            _orderConverter = orderConverter;
            _productService = productService;
            _helpService = helpService;
            _healthwiseService = healthwiseService;
            _skuMapper = skuMapper;
            _productMapper = productMapper;
            _countryService = countryService;
            _orderMapper = orderMapper;
            _orderPaymentMethodConverter = orderPaymentMethodConverter;
            _userManager = userManager;
            _contentCrossSellService = contentCrossSellService;
            _logger = loggerProvider.CreateLogger<ProfileController>();
        }

        private async Task<PagedListEx<AutoShipHistoryItemModel>> PopulateAutoShipHistoryModel(OrderFilter filter)
		{
			var infr = InfrastructureService.Data();
			var countries = await _countryService.GetCountriesAsync();

            var customer = await GetCurrentCustomerDynamic();

			filter.IdCustomer = customer.Id;
			filter.Sorting.SortOrder = FilterSortOrder.Desc;
			filter.Sorting.Path = VOrderSortPath.DateCreated;
			filter.OrderType = OrderType.AutoShip;

            var orders = await _orderService.GetFullAutoShipsAsync(filter);

            var helper = new AutoShipModelHelper(_skuMapper, _productMapper, _orderMapper, infr, countries);
            var ordersModel = new PagedListEx<AutoShipHistoryItemModel>
            {
                Items = await orders.Items.Select(async p => await helper.PopulateAutoShipItemModel(p)).ToListAsync(),
                Count = orders.Count,
                Index = filter.Paging.PageIndex
            };

            return ordersModel;
        }

        private async Task<BillingInfoModel> GetBillingDetailsInternal(int id, int orderId)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            BillingInfoModel model;
            if (id > 0)
            {
                var dynamic = currentCustomer.CustomerPaymentMethods
                .Single(p => p.IdObjectType == (int)PaymentMethodType.CreditCard && p.Id == id);
                model = await _addressConverter.ToModelAsync<BillingInfoModel>(dynamic.Address);
                await _customerPaymentMethodConverter.UpdateModelAsync(model, dynamic);
            }
            else if (orderId > 0)
            {
                var order = await _orderService.SelectAsync(orderId);
                model = await _addressConverter.ToModelAsync<BillingInfoModel>(order.PaymentMethod.Address);
                await _orderPaymentMethodConverter.UpdateModelAsync(model, order.PaymentMethod);
            }
            else
            {
                throw new ApiException();
            }

            return model;
        }

        private async Task<PagedListEx<OrderHistoryItemModel>> PopulateOrderHistoryModel(VOrderFilter filter)
        {
            var internalId = GetInternalCustomerId();

            filter.IdCustomer = internalId;
            filter.Sorting.SortOrder = FilterSortOrder.Desc;
            filter.Sorting.Path = VOrderSortPath.DateCreated;

            var orders = await _orderService.GetOrdersAsync(filter);

            var ordersModel = new PagedListEx<OrderHistoryItemModel>
            {
                Items = orders.Items.Select(p => new OrderHistoryItemModel()
                {
                    DateCreated = p.DateCreated,
                    Total = p.Total,
                    Id = p.Id,
                    OrderStatus = p.OrderStatus,
                    POrderStatus = p.POrderStatus,
                    NPOrderStatus = p.NPOrderStatus,
                    Healthwise = p.Healthwise,
                }).ToList(),
                Count = orders.Count,
                Index = filter.Paging.PageIndex
            };

            return ordersModel;
        }

        private async Task<BillingInfoModel> PopulateCreditCard(CustomerDynamic currentCustomer, int selectedId = 0)
        {
            var creditCards = new List<BillingInfoModel>();
            foreach (
                var creditCard in
                    currentCustomer.CustomerPaymentMethods.Where(p => p.IdObjectType == (int)PaymentMethodType.CreditCard))
            {
                var billingInfoModel = await _addressConverter.ToModelAsync<BillingInfoModel>(creditCard.Address);
                await _customerPaymentMethodConverter.UpdateModelAsync(billingInfoModel, creditCard);

                creditCards.Add(billingInfoModel);
            }

            ViewBag.CreditCards = null;

            BillingInfoModel model;
            if (creditCards.Count > 0)
            {
                model = selectedId > 0 ? creditCards.Single(x => x.Id == selectedId) : creditCards.FirstOrDefault(x => x.Default) ?? creditCards.FirstOrDefault();
                ViewBag.CreditCards = creditCards.ToJson();
            }
            else
            {
                model = new BillingInfoModel();
            }

            return model;
        }

        private async Task<ShippingInfoModel> PopulateShippingAddress(CustomerDynamic currentCustomer, int selectedId = 0)
        {
            var shippingAddresses = new List<ShippingInfoModel>();
            foreach (var shipping in currentCustomer.ShippingAddresses)
            {
                var shippingModel = await _addressConverter.ToModelAsync<ShippingInfoModel>(shipping);

                shippingAddresses.Add(shippingModel);
            }

            ViewBag.ShippingAddresses = null;
            ShippingInfoModel model;
            if (shippingAddresses.Count > 0)
            {
                model = selectedId > 0 ? shippingAddresses.Single(x => x.Id == selectedId) : shippingAddresses.First(x => x.Default);
                ViewBag.ShippingAddresses = shippingAddresses.ToJson();
            }
            else
            {
                model = new ShippingInfoModel();
            }

            return model;
        }

        private void CleanProfileEmailFields(ChangeProfileModel model)
        {
            model.ConfirmEmail = model.NewEmail = string.Empty;
            ModelState.Remove("NewEmail");
            ModelState.Remove("ConfirmEmail");
        }

        public IActionResult Index()
        {
            return RedirectToAction("TopFavoriteItems");
        }

        [HttpGet]
        public Task<IActionResult> ChangePassword()
        {
            return Task.FromResult<IActionResult>(View(new ChangePasswordModel()));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _storefrontUserService.FindAsync(_userManager.GetUserName(User));
            if (user == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindUser]);
            }

            await _storefrontUserService.UpdateWithPasswordChangeAsync(user, model.OldPassword, model.Password);

            ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated];

            return View(new ChangePasswordModel());
        }

        [HttpGet]
        public async Task<IActionResult> ChangeProfile()
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            var model =
                await _addressConverter.ToModelAsync<ChangeProfileModel>(currentCustomer.ProfileAddress);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeProfile(ChangeProfileModel model)
        {
            if (!ModelState.IsValid)
            {
                CleanProfileEmailFields(model);
                return View(model);
            }

            var customer = await GetCurrentCustomerDynamic();

            var oldEmail = customer.Email;

            var newProfileAddress = await _addressConverter.FromModelAsync(model, (int)AddressType.Profile);
            customer.ProfileAddress = newProfileAddress;
            customer.Email =
                newProfileAddress.Data.Email =
                    !string.IsNullOrWhiteSpace(model.NewEmail) && !string.IsNullOrWhiteSpace(model.ConfirmEmail)
                        ? model.NewEmail
                        : oldEmail;

            CleanProfileEmailFields(model);
            customer = await CustomerService.UpdateAsync(customer);

            if (oldEmail != customer.Email)
            {
                var user = await _storefrontUserService.GetAsync(GetInternalCustomerId());
                if (user == null)
                {
                    throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindLogin]);
                }
                await _storefrontUserService.RefreshSignInAsync(user);
            }

            ViewBag.SuccessMessage = InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated];

            model =
                await _addressConverter.ToModelAsync<ChangeProfileModel>(customer.ProfileAddress);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeBillingInfo()
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            return View(await PopulateCreditCard(currentCustomer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeBillingInfo(BillingInfoModel model)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            if (!ModelState.IsValid)
            {
                await PopulateCreditCard(currentCustomer, model.Id);
                return View(model);
            }

            if (model.Id > 0)
            {
                var creditCardToUpdate =
                    currentCustomer.CustomerPaymentMethods.Single(
                        x => x.IdObjectType == (int)PaymentMethodType.CreditCard && x.Id == model.Id);
                currentCustomer.CustomerPaymentMethods.Remove(creditCardToUpdate);
            }

            var otherAddresses = currentCustomer.CustomerPaymentMethods.Where(x => x.IdObjectType == (int)PaymentMethodType.CreditCard).ToList();
            if (model.Default)
            {
                foreach (var otherAddress in otherAddresses)
                {
                    otherAddress.Data.Default = false;
                }
            }

            if (otherAddresses.Count == 0)
            {
                model.Default = true;
            }

            var customerPaymentMethod = await _customerPaymentMethodConverter.FromModelAsync(model, (int)PaymentMethodType.CreditCard);
            customerPaymentMethod.Data.SecurityCode = model.SecurityCode;

            customerPaymentMethod.Address = await _addressConverter.FromModelAsync(model, (int)AddressType.Billing);

            currentCustomer.CustomerPaymentMethods.Add(customerPaymentMethod);
            try
            {
                currentCustomer = await CustomerService.UpdateAsync(currentCustomer);
            }
            catch (AppValidationException e)
            {
                foreach (var message in e.Messages)
                {
                    ModelState.AddModelError(string.Empty, message.Message);
                }

                await PopulateCreditCard(await GetCurrentCustomerDynamic(), model.Id);

                return View(model);
            }

            ViewBag.SuccessMessage = model.Id > 0
                ? InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated]
                : InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];

            ModelState.Clear();
            if (model.Id == 0)
            {
                model.Id = currentCustomer.CustomerPaymentMethods.Last(x => x.IdObjectType == (int) PaymentMethodType.CreditCard).Id;
            }
            return View(await PopulateCreditCard(currentCustomer, model.Id));
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteBillingInfo(int id)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            var creditCardToDelete =
                currentCustomer.CustomerPaymentMethods.FirstOrDefault(
                    x => x.IdObjectType == (int)PaymentMethodType.CreditCard && x.Id == id);
            if (creditCardToDelete == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }
            currentCustomer.CustomerPaymentMethods.Remove(creditCardToDelete);

            await CustomerService.UpdateAsync(currentCustomer);

            return true;
        }

        [HttpGet]
        public async Task<IActionResult> ChangeShippingInfo()
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            return View(await PopulateShippingAddress(currentCustomer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeShippingInfo([FromForm] ShippingInfoModel model)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            if (!ModelState.IsValid)
            {
                await PopulateShippingAddress(currentCustomer, model.Id);
                return View(model);
            }

            if (model.Id > 0)
            {
                var shippingAddressToUpdate = currentCustomer.ShippingAddresses.Single(x => x.Id == model.Id);
                currentCustomer.ShippingAddresses.Remove(shippingAddressToUpdate);
            }

            if (model.Default)
            {
                var otherAddresses = currentCustomer.ShippingAddresses;
                foreach (var otherAddress in otherAddresses)
                {
                    otherAddress.Data.Default = false;
                }
            }

            var newAddress = await _addressConverter.FromModelAsync(model, (int)AddressType.Shipping);

            currentCustomer.ShippingAddresses.Add(newAddress);

            currentCustomer = await CustomerService.UpdateAsync(currentCustomer);

            ViewBag.SuccessMessage = model.Id > 0
                ? InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated]
                : InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];

            if (model.Id == 0)
            {
                ModelState["Id"].RawValue = model.Id = currentCustomer.ShippingAddresses.Last().Id;
            }

            return View(await PopulateShippingAddress(currentCustomer, model.Id));
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteShippingInfo(int id)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            var shippingAddressToDelete = currentCustomer.ShippingAddresses.FirstOrDefault(x => x.Id == id);
            if (shippingAddressToDelete == null)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }

            currentCustomer.ShippingAddresses.Remove(shippingAddressToDelete);

            await CustomerService.UpdateAsync(currentCustomer);

            return true;
        }

        [HttpPost]
        public async Task<Result<bool>> SetDefaultShippingInfo(int id)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            var found = false;
            var addresses = currentCustomer.ShippingAddresses;
            foreach (var address in addresses)
            {
                if (address.Id == id)
                {
                    address.Data.Default = true;
                    found = true;
                }
                else
                {
                    address.Data.Default = false;
                }
            }

            if (!found)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }

            await CustomerService.UpdateAsync(currentCustomer);

            return true;
        }

        [HttpPost]
        public async Task<Result<bool>> SetDefaultCreditCard(int id)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            var found = false;
            var addresses = currentCustomer.CustomerPaymentMethods.Where(x => x.IdObjectType == (int)PaymentMethodType.CreditCard);
            foreach (var address in addresses)
            {
                if (address.Id == id)
                {
                    address.Data.Default = true;
                    found = true;
                }
                else
                {
                    address.Data.Default = false;
                }
            }

            if (!found)
            {
                throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
            }

            await CustomerService.UpdateAsync(currentCustomer);

            return true;
        }

        [HttpGet]
        public async Task<IActionResult> LastOrderPlaced()
        {
            var lines = new List<LastOrderLineModel>();

            var customer = await GetCurrentCustomerDynamic();
            if (customer != null)
            {
                var lastOrder = await _orderService.SelectLastOrderAsync(customer.Id);

                if (lastOrder != null)
                {
                    VProductSkuFilter filter = new VProductSkuFilter();
                    filter.Ids = lastOrder.Skus.Select(p => p.Sku.Id).ToList();
                    if (lastOrder.PromoSkus != null)
                    {
                        filter.Ids.AddRange(lastOrder.PromoSkus.Select(p => p.Sku.Id));
                    }
                    var skus = await _productService.GetSkusAsync(filter);

                    foreach (var skuOrdered in lastOrder.Skus)
                    {
                        //Ignore hidden in the storefront skus and products in this view
                        var skuInDB = skus.FirstOrDefault(p => p.Id == skuOrdered.Sku.Id && !p.Hidden && p.Product.IdVisibility.HasValue);

                        if (skuInDB != null)
                        {
                            var orderLineModel = new LastOrderLineModel()
                            {
                                ProductUrl = ProductBaseUrl + skuOrdered.Sku.Product.Url,
                                IconLink = skuOrdered.Sku.Product.SafeData.Thumbnail,
                                ProductName = skuOrdered.Sku.Product.Name,
                                PortionsCount = skuOrdered.Sku.Data.QTY,
                                Quantity = skuOrdered.Quantity,
                                SkuCode = skuOrdered.Sku.Code,
                                ProductSubTitle = skuInDB.Product.SafeData.SubTitle,
                                SelectedPrice =
                                    customer.IdObjectType == (int) CustomerType.Retail
                                        ? skuInDB.Price.ToString("C2")
                                        : skuInDB.WholesalePrice.ToString("C2"),
                            };
                            lines.Add(orderLineModel);
                        }
                    }
                }
            }

            return View(lines);
        }

        [HttpGet]
        public async Task<IActionResult> TopFavoriteItems(bool all = false)
        {
            var internalId = GetInternalCustomerId();

            var filter = new VCustomerFavoritesFilter()
            {
                IdCustomer = internalId,
                Paging = new Paging()
                {
                    PageIndex = 0,
                    PageItemCount = all ? ProductConstants.MAX_FAVORITES_COUNT : ProductConstants.DEFAULT_FAVORITES_COUNT
                }
            };

            var favorites = await _productService.GetCustomerFavoritesAsync(filter);

            List<FavoriteModel> model = new List<FavoriteModel>();
            if (favorites.Count > 0)
            {
                model = favorites.Items.Select(favorite => new FavoriteModel()
                {
                    ProductName = favorite.ProductName,
                    ProductSubTitle = favorite.ProductSubTitle,
                    ProductThumbnail = favorite.ProductThumbnail,
                    Url = ProductBaseUrl + favorite.Url
                }).ToList();
            }
            else
            {
                var addToCartSettings = await _contentCrossSellService.GetContentCrossSellsAsync(ContentCrossSellType.AddToCart);
                var productIds= await _productService.GetProductIdsBySkuIds(addToCartSettings.Select(p=>p.IdSku).ToList());
                foreach (var id in productIds.Select(p=>p.Value).Distinct())
                {
                    var product = await _productService.SelectTransferAsync(id);
                    model.Add(new FavoriteModel()
                    {
                        ProductName = product.ProductDynamic.Name,
                        ProductSubTitle = product.ProductDynamic.SafeData.SubTitle,
                        ProductThumbnail = product.ProductDynamic.SafeData.Thumbnail,
                        Url = ProductBaseUrl + product.ProductContent.Url
                    });
                }
            }

            ViewBag.MoreExist = !all && favorites.Count > filter.Paging.PageItemCount;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var filter = new VOrderFilter();

            return View(await PopulateOrderHistoryModel(filter));
        }

        public async Task<IActionResult> RefreshOrderHistory(VOrderFilter filter)
        {
            var model = await PopulateOrderHistoryModel(filter);

            return PartialView("_OrderHistoryGrid", model);
        }

        [HttpGet]
        public async Task<IActionResult> AutoShipHistory()
        {
            var filter = new OrderFilter();

            return View(await PopulateAutoShipHistoryModel(filter));
        }

        public async Task<IActionResult> RefreshAutoShipHistory()
        {
            var filter = new OrderFilter();
            var model = await PopulateAutoShipHistoryModel(filter);

            return PartialView("_AutoShipHistoryGrid", model);
        }

        [HttpGet]
        public async Task<IActionResult> AutoShipBillingDetails(int orderId)
        {
            var billingInfoModel = await GetBillingDetailsInternal(0, orderId);

            await PopulateCreditCardsLookup();

            var dict = (Dictionary<int, string>)ViewBag.CreditCards;
            dict.Add(orderId, "In Order");
            ViewBag.CreditCards = dict;
            ViewBag.OrderId = orderId;

            return PartialView("_AutoShipBillingDetails", billingInfoModel);
        }

        [HttpPost]
        public async Task<IActionResult> AutoShipBillingDetails(BillingInfoModel model, int orderId)
        {
            if (!ModelState.IsValid)
            {
                return PartialView("_BillingDetailsInner", model);
            }

            var order = await _orderService.SelectWithCustomerAsync(orderId);
            if (order.Customer.Id != GetInternalCustomerId())
            {
                throw new ApiException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AccessDenied], HttpStatusCode.Forbidden);
            }

            var addressId = order.PaymentMethod.Address.Id;
            await _orderPaymentMethodConverter.UpdateObjectAsync(model, order.PaymentMethod,
                               (int)PaymentMethodType.CreditCard);
            await _addressConverter.UpdateObjectAsync(model, order.PaymentMethod.Address, (int)AddressType.Billing);

            order.PaymentMethod.Address.Id = addressId;

            await _orderService.UpdateAsync(order);

            ViewBag.Success = true;
            return PartialView("_BillingDetailsInner", model);
        }

        [HttpGet]
        public async Task<IActionResult> GetBillingAddress(int paymentId, int orderId)
        {
            var billingInfoModel = await GetBillingDetailsInternal(paymentId, orderId);

            return PartialView("_BillingDetailsInner", billingInfoModel);
        }

        [HttpPost]
        public async Task<Result<bool>> ActivatePauseAutoShip(int id)
        {
            var internalId = GetInternalCustomerId();

            await _orderService.ActivatePauseAutoShipAsync(internalId, id);

            return true;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteAutoShip(int id)
        {
            var internalId = GetInternalCustomerId();

            await _orderService.DeleteAutoShipAsync(internalId, id);

            return true;
        }

        [HttpGet]
        public async Task<IActionResult> OrderInvoice(int id)
        {
            var internalId = GetInternalCustomerId();
            var order = await _orderService.SelectAsync(id, true);
            if (order != null && order.Customer.Id == internalId)
            {
                order.Customer = await CustomerService.SelectAsync(order.Customer.Id, true);
                var model = await _orderConverter.ToModelAsync<OrderViewModel>(order);

                return View(model);
            }
            else
            {
                return GetItemNotAccessibleResult();
            }
        }

        [HttpGet]
        public async Task<IActionResult> HelpTickets(int idorder, bool ignore = false)
        {
            ICollection<HelpTicketListItemModel> toReturn;
            ViewBag.IdOrder = (int?)null;
            var customerId = GetInternalCustomerId();
            var orderCustomerId = await _orderService.GetOrderIdCustomer(idorder);
            if (orderCustomerId == customerId)
            {
                VHelpTicketFilter filter = new VHelpTicketFilter();
                filter.IdOrder = idorder;
                filter.IdCustomer = customerId;
                var items = (await _helpService.GetHelpTicketsAsync(filter)).Items;
                if (items.Count == 0 && !ignore)
                {
                    return RedirectToAction("HelpTicket", new { idorder });
                }
                ViewBag.IdOrder = (int?)idorder;
                toReturn = items.Select(p => new HelpTicketListItemModel(p)).ToList();
            }
            else
            {
                return GetItemNotAccessibleResult();
            }
            return View(toReturn);
        }

        [HttpGet]
        public async Task<IActionResult> HelpTicket(int? id = null, int? idorder = null)
        {
            HelpTicketManageModel toReturn = null;
            if (id.HasValue)
            {
                var item = await _helpService.GetHelpTicketAsync(id.Value);
                var customerId = GetInternalCustomerId();
                if (item != null && item.Order.IdCustomer == customerId)
                {
                    toReturn = new HelpTicketManageModel(item);
                    if (TempData.ContainsKey(TicketCommentMessageTempData))
                    {
                        ViewBag.SuccessMessage = TempData[TicketCommentMessageTempData];
                    }
                }
                else
                {
                    return GetItemNotAccessibleResult();
                }
            }
            else if (idorder.HasValue)
            {
                toReturn = new HelpTicketManageModel(null)
                {
                    StatusCode = RecordStatusCode.Active,
                    Priority = TicketPriority.Low,
                    IdOrder = idorder.Value,
                };
            }
            return View(toReturn);
        }

        [HttpPost]
        public async Task<IActionResult> HelpTicket(HelpTicketManageModel model)
        {
            if (!Validate(model))
            {
                return View(model);
            }

            var ticket = model.Convert();
            ticket = await _helpService.UpdateHelpTicketAsync(ticket, null);
            return RedirectToAction("HelpTickets", new { idorder = ticket.IdOrder });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHelpTicket(HelpTicketManageModel model)
        {
            await _helpService.DeleteHelpTicketAsync(model.Id);
            return RedirectToAction("HelpTickets", new { idorder = model.IdOrder });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHelpTicketComment(HelpTicketCommentManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();

            item = await _helpService.UpdateHelpTicketCommentAsync(item);

            if (model.Id == 0)
            {
                TempData[TicketCommentMessageTempData] = "New comment was successfully added.";
            }
            else
            {
                TempData[TicketCommentMessageTempData] = "Comment was successfully updated.";
            }
            return RedirectToAction("HelpTicket", new { id = item.IdHelpTicket });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHelpTicketComment(HelpTicketCommentManageModel model)
        {
            await _helpService.DeleteHelpTicketCommentAsync(model.Id, null);
            TempData[TicketCommentMessageTempData] = "Comment was successfully deleted.";
            return RedirectToAction("HelpTicket", new { id = model.IdHelpTicket });
        }

        [HttpGet]
        public async Task<IActionResult> HealthWiseHistory()
        {
            HealthWiseHistoryModel toReturn = null;
            var internalId = GetInternalCustomerId();
            VHealthwisePeriodFilter filter = new VHealthwisePeriodFilter();
            filter.NotPaid = true;
            filter.IdCustomer = internalId;
            var periods = await _healthwiseService.GetVHealthwisePeriodsAsync(filter);
            periods = periods.OrderByDescending(p => p.StartDate).ToList();

            VHealthwisePeriod lastPeriodWithOrders = null;
            ICollection<HealthwiseOrder> orders = null;
            foreach (var period in periods)
            {
                orders = await _healthwiseService.GetHealthwiseOrdersAsync(period.Id);
                lastPeriodWithOrders = period;
                if (orders.Count > 0)
                {
                    break;
                }
            }
            if (lastPeriodWithOrders != null)
            {
                toReturn = new HealthWiseHistoryModel();
                toReturn.EndDate = lastPeriodWithOrders.EndDate;
                toReturn.Items = new List<HealthWiseHistoryOrderModel>();
                foreach (var healthWiseOrder in orders)
                {
                    HealthWiseHistoryOrderModel orderModel = new HealthWiseHistoryOrderModel();
                    orderModel.Id = healthWiseOrder.Id;
                    orderModel.DateCreated = healthWiseOrder.Order.DateCreated;
                    orderModel.OrderStatus = healthWiseOrder.Order.OrderStatus;
                    orderModel.POrderStatus = healthWiseOrder.Order.POrderStatus;
                    orderModel.NPOrderStatus = healthWiseOrder.Order.NPOrderStatus;
                    orderModel.DiscountedSubtotal = healthWiseOrder.Order.ProductsSubtotal - healthWiseOrder.Order.DiscountTotal;
                    toReturn.Items.Add(orderModel);
                }
                if (toReturn.Items.Count > 0)
                {
                    toReturn.Count = toReturn.Items.Count;
                    toReturn.AverageAmount = Math.Round(toReturn.Items.Sum(p => p.DiscountedSubtotal) / toReturn.Count, 2);
                }
            }

            return View(toReturn);
        }
    }
}
