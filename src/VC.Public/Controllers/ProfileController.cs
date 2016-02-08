using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.ModelBinding;
using Newtonsoft.Json;
using VC.Public.Models.Profile;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
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

namespace VC.Public.Controllers
{
    [CustomerAuthorize]
    public class ProfileController : PublicControllerBase
    {
        private const string TicketCommentMessageTempData = "ticket-comment-messsage";
        private const string ProductBaseUrl = "/product/";

        private readonly IStorefrontUserService _storefrontUserService;
        private readonly IDynamicMapper<AddressDynamic, Address> _addressConverter;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _paymentMethodConverter;
        private readonly IProductService _productService;
        private readonly IOrderService _orderService;
        private readonly IHelpService _helpService;
        private readonly IHealthwiseService _healthwiseService;

        public ProfileController(IHttpContextAccessor contextAccessor, IStorefrontUserService storefrontUserService,
            ICustomerService customerService, IDynamicMapper<AddressDynamic, Address> addressConverter,
            IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> paymentMethodConverter, IOrderService orderService,
            IProductService productService,
            IHelpService helpService,
            IHealthwiseService healthwiseService, IAppInfrastructureService infrastructureService,
            IAuthorizationService authorizationService, ICheckoutService checkoutService)
            : base(contextAccessor, customerService, infrastructureService, authorizationService, checkoutService)
        {
            _storefrontUserService = storefrontUserService;
            _addressConverter = addressConverter;
            _paymentMethodConverter = paymentMethodConverter;
            _orderService = orderService;
            _productService = productService;
            _helpService = helpService;
            _healthwiseService = healthwiseService;
        }

        private async Task<PagedListEx<OrderHistoryItemModel>> PopulateHistoryModel(VOrderFilter filter)
        {
            var internalId = GetInternalCustomerId();

            filter.IdCustomer = internalId;
            filter.Sorting.SortOrder = SortOrder.Desc;
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

        private BillingInfoModel PopulateCreditCard(CustomerDynamic currentCustomer, int selectedId = 0)
        {
            var creditCards = new List<BillingInfoModel>();
            foreach (
                var creditCard in
                    currentCustomer.CustomerPaymentMethods.Where(p => p.IdObjectType == (int) PaymentMethodType.CreditCard))
            {
                var billingInfoModel = _addressConverter.ToModel<BillingInfoModel>(creditCard.Address);
                _paymentMethodConverter.UpdateModel(billingInfoModel, creditCard);

                creditCards.Add(billingInfoModel);
            }

            ViewBag.CreditCards = null;

            BillingInfoModel model = null;
            if (creditCards.Any())
            {
                model = selectedId > 0 ? creditCards.Single(x => x.Id == selectedId) : creditCards.First();
                ViewBag.CreditCards = creditCards.ToJson();
            }
            else
            {
                model = new BillingInfoModel();
            }

            return model;
        }

        private ShippingInfoModel PopulateShippingAddress(CustomerDynamic currentCustomer, int selectedId = 0)
        {
            var shippingAddresses = new List<ShippingInfoModel>();
            foreach (var shipping in currentCustomer.ShippingAddresses)
            {
                var shippingModel = _addressConverter.ToModel<ShippingInfoModel>(shipping);

                shippingAddresses.Add(shippingModel);
            }

            ViewBag.ShippingAddresses = null;
            ShippingInfoModel model;
            if (shippingAddresses.Any())
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
            ModelState["NewEmail"] = new ModelStateEntry();
            ModelState["ConfirmEmail"] = new ModelStateEntry();
        }

        public IActionResult Index()
        {
            return RedirectToAction("TopFavoriteItems");
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var context = ContextAccessor.HttpContext;

            var user = await _storefrontUserService.FindAsync(context.User.GetUserName());
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
                _addressConverter.ToModel<ChangeProfileModel>(currentCustomer.ProfileAddress);

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

            var newProfileAddress = _addressConverter.FromModel(model);
            newProfileAddress.IdObjectType = (int) AddressType.Profile;
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
                _addressConverter.ToModel<ChangeProfileModel>(customer.ProfileAddress);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeBillingInfo()
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            return View(PopulateCreditCard(currentCustomer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeBillingInfo(BillingInfoModel model)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            if (!ModelState.IsValid)
            {
                PopulateCreditCard(currentCustomer, model.Id);
                return View(model);
            }

            if (model.Id > 0)
            {
                var creditCardToUpdate =
                    currentCustomer.CustomerPaymentMethods.Single(
                        x => x.IdObjectType == (int) PaymentMethodType.CreditCard && x.Id == model.Id);
                currentCustomer.CustomerPaymentMethods.Remove(creditCardToUpdate);
            }

            var customerPaymentMethod = _paymentMethodConverter.FromModel(model);
            customerPaymentMethod.IdObjectType = (int) PaymentMethodType.CreditCard;
            customerPaymentMethod.Data.SecurityCode = model.SecurityCode;

            customerPaymentMethod.Address = _addressConverter.FromModel(model);
            customerPaymentMethod.Address.IdObjectType = (int) AddressType.Billing;

            currentCustomer.CustomerPaymentMethods.Add(customerPaymentMethod);
            try
            {
                currentCustomer = await CustomerService.UpdateAsync(currentCustomer);
            }
            catch (AppValidationException e)
            {
                foreach (var message in e.Messages)
                {
                    ModelState.AddModelError(message.Field, message.Message);
                }
                return View(PopulateCreditCard(currentCustomer, model.Id));
            }

            ViewBag.SuccessMessage = model.Id > 0
                ? InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated]
                : InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];

            ModelState.Clear();
            if (model.Id == 0)
            {
                ModelState.Add("Id", new ModelStateEntry
                {
                    RawValue =
                        model.Id = currentCustomer.CustomerPaymentMethods.Last(x => x.IdObjectType == (int) PaymentMethodType.CreditCard).Id
                });
            }
            return View(PopulateCreditCard(currentCustomer, model.Id));
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteBillingInfo(int id)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            var creditCardToDelete =
                currentCustomer.CustomerPaymentMethods.FirstOrDefault(
                    x => x.IdObjectType == (int) PaymentMethodType.CreditCard && x.Id == id);
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

            return View(PopulateShippingAddress(currentCustomer));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeShippingInfo([FromForm] ShippingInfoModel model)
        {
            var currentCustomer = await GetCurrentCustomerDynamic();

            if (!ModelState.IsValid)
            {
                PopulateShippingAddress(currentCustomer, model.Id);
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

            var newAddress = _addressConverter.FromModel(model);
            newAddress.IdObjectType = (int) AddressType.Shipping;

            currentCustomer.ShippingAddresses.Add(newAddress);

            currentCustomer = await CustomerService.UpdateAsync(currentCustomer);

            ViewBag.SuccessMessage = model.Id > 0
                ? InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyUpdated]
                : InfoMessagesLibrary.Data[InfoMessagesLibrary.Keys.EntitySuccessfullyAdded];

            if (model.Id == 0)
            {
                ModelState["Id"].RawValue = model.Id = currentCustomer.ShippingAddresses.Last().Id;
            }

            return View(PopulateShippingAddress(currentCustomer, model.Id));
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
                    var skus = await _productService.GetSkusAsync(filter);

                    foreach (var skuOrdered in lastOrder.Skus)
                    {
                        var skuInDB = skus.FirstOrDefault(p => p.SkuId == skuOrdered.Sku.Id);

                        var orderLineModel = new LastOrderLineModel()
                        {
                            ProductUrl = ProductBaseUrl + skuOrdered.ProductWithoutSkus.Url,
                            IconLink = skuOrdered.ProductWithoutSkus.SafeData.Thumbnail,
                            ProductName = skuOrdered.ProductWithoutSkus.Name,
                            PortionsCount = skuOrdered.Sku.Data.QTY,
                            Quantity = skuOrdered.Quantity,
                            SkuCode = skuOrdered.Sku.Code,
                            ProductSubTitle = skuInDB?.SubTitle,
                            SelectedPrice =
                                customer.IdObjectType == (int) CustomerType.Retail
                                    ? skuInDB?.Price?.ToString("C2")
                                    : skuInDB?.WholesalePrice?.ToString("C2"),
                        };
                        lines.Add(orderLineModel);
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

            var model = favorites.Items.Select(favorite => new FavoriteModel()
            {
                ProductName = favorite.ProductName,
                ProductSubTitle = favorite.ProductSubTitle,
                ProductThumbnail = favorite.ProductThumbnail,
                Url = ProductBaseUrl + favorite.Url
            }).ToList();

            ViewBag.MoreExist = !all && favorites.Count > filter.Paging.PageItemCount;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> OrderHistory()
        {
            var filter = new VOrderFilter();

            return View(await PopulateHistoryModel(filter));
        }

        public async Task<IActionResult> RefreshOrderHistory(VOrderFilter filter)
        {
            var model = await PopulateHistoryModel(filter);

            return PartialView("_OrderHistoryGrid", model);
        }

        [HttpGet]
        public async Task<IActionResult> HelpTickets(int idorder, bool ignore = false)
        {
            ICollection<HelpTicketListItemModel> toReturn = new List<HelpTicketListItemModel>();
            ViewBag.IdOrder = (int?) null;
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
                    return RedirectToAction("HelpTicket", new {idorder = idorder});
                }
                ViewBag.IdOrder = (int?) idorder;
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
            return RedirectToAction("HelpTickets", new {idorder = ticket.IdOrder});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHelpTicket(HelpTicketManageModel model)
        {
            await _helpService.DeleteHelpTicketAsync(model.Id);
            return RedirectToAction("HelpTickets", new {idorder = model.IdOrder});
        }

        [HttpPost]
        public async Task<IActionResult> UpdateHelpTicketComment(HelpTicketCommentManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();

            item = await _helpService.UpdateHelpTicketCommentAsync(item);

            var toReturn = new HelpTicketManageModel(await _helpService.GetHelpTicketAsync(item.IdHelpTicket));
            if (model.Id == 0)
            {
                TempData[TicketCommentMessageTempData] = "New comment was successfully added.";
            }
            else
            {
                TempData[TicketCommentMessageTempData] = "Comment was successfully updated.";
            }
            return RedirectToAction("HelpTicket", new {id = item.IdHelpTicket});
        }

        [HttpPost]
        public async Task<IActionResult> DeleteHelpTicketComment(HelpTicketCommentManageModel model)
        {
            var result = await _helpService.DeleteHelpTicketCommentAsync(model.Id, null);
            TempData[TicketCommentMessageTempData] = "Comment was successfully deleted.";
            return RedirectToAction("HelpTicket", new {id = model.IdHelpTicket});
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
                    orderModel.Total = healthWiseOrder.Order.ProductsSubtotal;
                    toReturn.Items.Add(orderModel);
                }
                if (toReturn.Items.Count > 0)
                {
                    toReturn.Count = toReturn.Items.Count;
                    toReturn.AverageAmount = toReturn.Items.Sum(p => p.Total)/toReturn.Count;
                }
            }

            return View(toReturn);
        }
    }
}