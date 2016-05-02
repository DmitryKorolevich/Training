using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Settings;
using Newtonsoft.Json;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.Domain.Transfer.Settings;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;
using VC.Admin.Models.Orders;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Business.CsvExportMaps.Orders;
using VitalChoice.Infrastructure.Domain.Constants;
using Microsoft.Net.Http.Headers;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Core.Infrastructure.Helpers;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VC.Admin.ModelConverters;
using VC.Admin.Models.Customers;
using VC.Admin.Models.Products;
using VitalChoice.Business.Mail;
using VitalChoice.Business.Services.Bronto;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.Caching.Extensions;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer.GiftCertificates;
using VitalChoice.SharedWeb.Models.Orders;
using VitalChoice.Business.Helpers;
using VitalChoice.SharedWeb.Helpers;
using System.Security.Claims;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Identity;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Business.Helpers;

namespace VC.Admin.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IOrderRefundService _orderRefundService;
        private readonly OrderMapper _mapper;
        private readonly OrderRefundMapper _orderRefundMapper;
        private readonly IDynamicMapper<AddressDynamic, Address> _addressMapper;
        private readonly IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> _customerPaymentMethodMapper;
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _orderPaymentMethodMapper;

        private readonly ICustomerService _customerService;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly IAppInfrastructureService _appInfrastructureService;
        private readonly ICountryService _countryService;
        private readonly ICsvExportService<OrdersRegionStatisticItem, OrdersRegionStatisticItemCsvMap> _ordersRegionStatisticItemCSVExportService;
        private readonly ICsvExportService<OrdersZipStatisticItem, OrdersZipStatisticItemCsvMap> _ordersZipStatisticItemCSVExportService;
        private readonly ICsvExportService<VOrderWithRegionInfoItem, VOrderWithRegionInfoItemCsvMap> _vOrderWithRegionInfoItemCSVExportService;
        private readonly ICsvExportService<OrdersAgentReportExportItem, OrdersAgentReportExportItemCsvMap> _ordersAgentReportExportItemCSVExportService;
        private readonly INotificationService _notificationService;
        private readonly BrontoService _brontoService;
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly IDynamicMapper<SkuDynamic, Sku> _skuMapper;
        private readonly IDynamicMapper<ProductDynamic, Product> _productMapper;
        private readonly IDynamicMapper<OrderDynamic, Order> _orderMapper;
        private readonly IOrderReportService _orderReportService;

        public OrderController(
            IOrderService orderService,
            IOrderRefundService orderRefundService,
            ILoggerProviderExtended loggerProvider,
            OrderMapper mapper,
            OrderRefundMapper orderRefundMapper,
            ICustomerService customerService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ICsvExportService<OrdersRegionStatisticItem, OrdersRegionStatisticItemCsvMap> ordersRegionStatisticItemCSVExportService,
            ICsvExportService<OrdersZipStatisticItem, OrdersZipStatisticItemCsvMap> ordersZipStatisticItemCSVExportService,
            ICsvExportService<VOrderWithRegionInfoItem, VOrderWithRegionInfoItemCsvMap> vOrderWithRegionInfoItemCSVExportService,
            ICsvExportService<OrdersAgentReportExportItem, OrdersAgentReportExportItemCsvMap> ordersAgentReportExportItemCSVExportService,
            INotificationService notificationService,
            BrontoService brontoService,
            IOrderReportService orderReportService,
            IObjectHistoryLogService objectHistoryLogService, IOptions<AppOptions> options, IAppInfrastructureService appInfrastructureService, ICountryService countryService, IDynamicMapper<OrderDynamic, Order> orderMapper, IDynamicMapper<ProductDynamic, Product> productMapper, IDynamicMapper<SkuDynamic, Sku> skuMapper, IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> orderPaymentMethodMapper, IDynamicMapper<CustomerPaymentMethodDynamic, CustomerPaymentMethod> customerPaymentMethodMapper, IDynamicMapper<AddressDynamic, Address> addressMapper1)
        {
            _orderService = orderService;
            _orderRefundService = orderRefundService;
            _mapper = mapper;
            _orderRefundMapper = orderRefundMapper;
            _customerService = customerService;
            _ordersRegionStatisticItemCSVExportService = ordersRegionStatisticItemCSVExportService;
            _ordersZipStatisticItemCSVExportService = ordersZipStatisticItemCSVExportService;
            _vOrderWithRegionInfoItemCSVExportService = vOrderWithRegionInfoItemCSVExportService;
            _ordersAgentReportExportItemCSVExportService = ordersAgentReportExportItemCSVExportService;
            _notificationService = notificationService;
            _brontoService = brontoService;
            _orderReportService = orderReportService;
            _objectHistoryLogService = objectHistoryLogService;
            _appInfrastructureService = appInfrastructureService;
            _countryService = countryService;
            _orderMapper = orderMapper;
            _productMapper = productMapper;
            _skuMapper = skuMapper;
            _orderPaymentMethodMapper = orderPaymentMethodMapper;
            _customerPaymentMethodMapper = customerPaymentMethodMapper;
            _addressMapper = addressMapper1;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            loggerProvider.CreateLoggerDefault();
        }

        #region BaseOrderLogic

        [HttpPost]
        public async Task<Result<PagedList<ShortOrderListItemModel>>> GetShortOrders([FromBody]OrderFilter filter)
        {
            var result = await _orderService.GetShortOrdersAsync(filter);

            var toReturn = new PagedList<ShortOrderListItemModel>
            {
                Items = result.Items.Select(p => new ShortOrderListItemModel(p)).ToList(),
                Count = result.Count,
            };
            return toReturn;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<bool>> UpdateOrderStatus(int id, int status, [FromBody] object model, int? orderpart = null)
        {
            var order = await _orderService.SelectAsync(id, false);

            if (order == null)
            {
                throw new AppValidationException("Id", "The given order doesn't exist.");
            }
            if (!orderpart.HasValue && order.OrderStatus.HasValue)
            {
                order.OrderStatus = (OrderStatus)status;
            }
            else if (orderpart == 1 && order.NPOrderStatus.HasValue)//NP
            {
                order.NPOrderStatus = (OrderStatus)status;
            }
            if (orderpart == 2 && order.POrderStatus.HasValue)//P
            {
                order.POrderStatus = (OrderStatus)status;
            }
            order = await _orderService.UpdateAsync(order);

            return order != null;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<bool>> MoveOrder(int id, int idcustomer, [FromBody] object model)
        {
            var order = await _orderService.SelectAsync(id, false);
            var customer = await _customerService.SelectAsync(idcustomer, false);

            if (order == null)
            {
                throw new AppValidationException("Id", "The given order doesn't exist.");
            }
            if (customer == null)
            {
                throw new AppValidationException("IdCustomer", "The given customer doesn't exist.");
            }
            order.Customer.Id = idcustomer;
            order = await _orderService.UpdateAsync(order);

            return order != null;
        }

        [HttpPost]
        public async Task<Result<PagedList<OrderListItemModel>>> GetOrders([FromBody]VOrderFilter filter)
        {
            if (filter.To.HasValue)
            {
                filter.To = filter.To.Value.AddDays(1);
            }

            var result = await _orderService.GetOrdersAsync(filter);

            var toReturn = new PagedList<OrderListItemModel>
            {
                Items = result.Items.Select(p => new OrderListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpGet]
        public async Task<Result<OrderManageModel>> GetOrder(int id, int? idcustomer = null, bool refreshprices = false)
        {
            if (id == 0)
            {
                var order = await _orderService.CreateNewNormalOrder(OrderStatus.Processed);
                if (idcustomer.HasValue)
                {
                    order.Data.OrderNotes = await _customerService.GetNewOrderNotesBasedOnCustomer(idcustomer.Value);
                }

                var model = _mapper.ToModel<OrderManageModel>(order);
                model.UseShippingAndBillingFromCustomer = true;
                model.GCs = new List<GCListItemModel>() { new GCListItemModel(null) };
                model.SkuOrdereds = new List<SkuOrderedManageModel>() { new SkuOrderedManageModel(null) };
                model.UpdateShippingAddressForCustomer = true;
                model.UpdateCardForCustomer = true;
                model.UpdateCheckForCustomer = true;
                model.UpdateOACForCustomer = true;
                model.UpdateWireTransferForCustomer = true;
                model.UpdateMarketingForCustomer = true;
                model.UpdateVCWellnessForCustomer = true;

                return model;
            }

            var item = await _orderService.SelectAsync(id);
            if (item.IdObjectType != (int)OrderType.Normal && item.IdObjectType != (int)OrderType.AutoShipOrder && item.IdObjectType != (int)OrderType.AutoShip &&
                item.IdObjectType != (int)OrderType.DropShip && item.IdObjectType != (int)OrderType.GiftList)
            {
                throw new AccessDeniedException();
            }
            if (id != 0 && refreshprices && item.Skus != null)
            {
                var customer = await _customerService.SelectAsync(item.Customer.Id);
                foreach (var orderSku in item.Skus)
                {
                    if (orderSku.Sku != null)
                    {
                        orderSku.Amount = customer.IdObjectType == (int)CustomerType.Retail
                            ? orderSku.Sku.Price
                            : orderSku.Sku.WholesalePrice;
                    }
                }
            }

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(item);

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<OrderCalculateModel>> CalculateOrder([FromBody]OrderManageModel model)
        {
            var order = _mapper.FromModel(model);
            var orderContext = await _orderService.CalculateOrder(order, model.CombinedEditOrderStatus);

            OrderCalculateModel toReturn = new OrderCalculateModel(orderContext);

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<OrderManageModel>> UpdateOrder([FromBody]OrderManageModel model)
        {
            if (!Validate(model))
                return null;

            var order = _mapper.FromModel(model);

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (int.TryParse(sUserId, out userId))
            {
                order.IdEditedBy = userId;
            }
            order.Customer.IdEditedBy = userId;
            foreach (var address in order.Customer.ShippingAddresses)
            {
                address.IdEditedBy = userId;
            }
            order.Customer.ProfileAddress.IdEditedBy = userId;

            await _customerService.UpdateAsync(order.Customer);

            var sendOrderConfirm = false;
            if (model.CombinedEditOrderStatus != OrderStatus.Cancelled && model.CombinedEditOrderStatus != OrderStatus.Exported && model.CombinedEditOrderStatus != OrderStatus.Shipped)
            {
                await _orderService.OrderTypeSetup(order);
                await _orderService.CalculateOrder(order, model.CombinedEditOrderStatus);

                if (!model.ConfirmationEmailSent &&
                    (model.CombinedEditOrderStatus == OrderStatus.Processed ||
                     model.CombinedEditOrderStatus == OrderStatus.ShipDelayed) &&
                    !string.IsNullOrEmpty(model.Customer.Email))
                {
                    sendOrderConfirm = true;
                    order.Data.ConfirmationEmailSent = true;
                }

                if (model.Id > 0)
                {
                    order = await _orderService.UpdateAsync(order);
                }
                else
                {
                    order = await _orderService.InsertAsync(order);
                    if (order.IdObjectType == (int)OrderType.AutoShip)
                    {
                        var ids = await _orderService.SelectAutoShipOrdersAsync(order.Id);

                        order = await _orderService.SelectAsync(ids.First());
                    }
                }
            }

            if (sendOrderConfirm && !string.IsNullOrEmpty(model.Customer?.Email))//&& order.IdObjectType != (int)OrderType.AutoShip
            {
                var emailModel = _mapper.ToModel<OrderConfirmationEmail>(order);
                if (emailModel != null)
                {
                    await _notificationService.SendOrderConfirmationEmailAsync(order.Customer.Email, emailModel);
                }
            }

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(order);

            if (!string.IsNullOrEmpty(model.Customer.Email) && model.SignUpNewsletter.HasValue)
            {
                _brontoService.PushSubscribe(model.Customer.Email, model.SignUpNewsletter.Value);
            }

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<bool>> CancelOrder(int id, [FromBody] object model)
        {
            return await _orderService.CancelOrderAsync(id);
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpGet]
        public async Task<Result<bool>> GetIsBrontoSubscribed(string id)
        {
            return !(await _brontoService.GetIsUnsubscribed(id) ?? false);
        }

        #endregion

        #region AutoShips

        [HttpPost]
        public async Task<Result<PagedList<AutoShipHistoryItemModel>>> GetAutoShips([FromBody] OrderFilter filter)
        {
            var infr = _appInfrastructureService.Data();
            var countries = await _countryService.GetCountriesAsync();

            filter.Sorting.SortOrder = VitalChoice.Infrastructure.Domain.Transfer.SortOrder.Desc;
            filter.Sorting.Path = VOrderSortPath.DateCreated;
            filter.OrderType = OrderType.AutoShip;

            var orders = await _orderService.GetFullAutoShipsAsync(filter);

            var helper = new AutoShipModelHelper(_skuMapper, _productMapper, _orderMapper, infr, countries);
            var ordersModel = new PagedList<AutoShipHistoryItemModel>
            {
                Items = orders.Items.Select(p => helper.PopulateAutoShipItemModel(p)).ToList(),
                Count = orders.Count
            };

            return ordersModel;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpGet]
        public async Task<Result<IList<CreditCardModel>>> GetAutoShipCreditCards([FromQuery] int orderId, [FromQuery] int customerId)
        {
            var model = new List<CreditCardModel>();

            var order = await _orderService.SelectAsync(orderId);
            var orderCreditCard = _addressMapper.ToModel<CreditCardModel>(order.PaymentMethod.Address);
            _orderPaymentMethodMapper.UpdateModel(orderCreditCard, order.PaymentMethod);

            orderCreditCard.IsSelected = true;
            model.Add(orderCreditCard);

            var customer = await _customerService.SelectAsync(customerId);
            var dynamics = customer.CustomerPaymentMethods
            .Where(p => p.IdObjectType == (int)PaymentMethodType.CreditCard).ToList();
            foreach (var item in dynamics)
            {
                var tempModel = _addressMapper.ToModel<CreditCardModel>(item.Address);
                _customerPaymentMethodMapper.UpdateModel(tempModel, item);

                model.Add(tempModel);
            }

            return model;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<CreditCardModel>> UpdateAutoShipBilling([FromBody]CreditCardModel model, [FromQuery] int orderId)
        {
            if (!Validate(model))
                return null;

            var order = await _orderService.SelectAsync(orderId, true);

            var addressId = order.PaymentMethod.Address.Id;
            await _orderPaymentMethodMapper.UpdateObjectAsync(model, order.PaymentMethod,
                               (int)PaymentMethodType.CreditCard);
            await _addressMapper.UpdateObjectAsync(model, order.PaymentMethod.Address, (int)AddressType.Billing);

            order.PaymentMethod.Address.Id = addressId;

            order = await _orderService.UpdateAsync(order);
            var orderCreditCard = _addressMapper.ToModel<CreditCardModel>(order.PaymentMethod.Address);
            _orderPaymentMethodMapper.UpdateModel(orderCreditCard, order.PaymentMethod);

            return orderCreditCard;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<bool>> ActivatePauseAutoShip([FromQuery]int customerId, [FromQuery]int id)
        {
            await _orderService.ActivatePauseAutoShipAsync(customerId, id);

            return true;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<bool>> DeleteAutoShip([FromQuery]int customerId, [FromQuery]int id)
        {
            await _orderService.DeleteAutoShipAsync(customerId, id);

            return true;
        }

        #endregion

        #region Reships

        [AdminAuthorize(PermissionType.Orders)]
        [HttpGet]
        public async Task<Result<OrderReshipManageModel>> GetReshipOrder(int id, int? idsource = null, int? idcustomer = null)
        {
            OrderReshipManageModel toReturn = null;
            if (id == 0)
            {
                if (idsource.HasValue)
                {
                    var order = await _orderService.SelectAsync(idsource.Value);
                    if (order != null)
                    {
                        order.GiftCertificates = new List<GiftCertificateInOrder>();
                        order.Discount = null;
                        toReturn = _mapper.ToModel<OrderReshipManageModel>(order);
                        toReturn.KeyCode = "RESHIP";
                        toReturn.IdObjectType = (int)OrderType.Reship;
                        toReturn.IdOrderSource = toReturn.Id;
                        toReturn.OrderSourceDateCreated = toReturn.DateCreated;
                        toReturn.OrderSourceTotal = toReturn.Total;
                        toReturn.OrderNotes = String.Empty;
                        toReturn.ConfirmationEmailSent = false;
                        if (toReturn.SkuOrdereds != null && toReturn.PromoSkus != null)
                        {
                            foreach (var promoSkuOrderedManageModel in toReturn.PromoSkus)
                            {
                                var existSku = toReturn.SkuOrdereds.FirstOrDefault(p => p.IdSku == promoSkuOrderedManageModel.IdSku);
                                if (existSku != null)
                                {
                                    existSku.QTY += promoSkuOrderedManageModel.QTY;
                                }
                                else
                                {
                                    toReturn.SkuOrdereds.Add(promoSkuOrderedManageModel.ConvertToBase());
                                }
                            }
                            toReturn.PromoSkus = new List<PromoSkuOrderedManageModel>();
                        }
                        toReturn.SkuOrdereds?.ForEach(p =>
                        {
                            p.Price = 0;
                            p.Amount = 0;
                        });
                        toReturn.ReshipProblemSkus =
                            toReturn.SkuOrdereds?.Where(p => p.IdSku.HasValue).Select(p => new ReshipProblemSkuModel()
                            {
                                IdSku = p.IdSku.Value,
                                Code = p.Code,
                                Used = true,
                            }).ToList();
                        toReturn.Id = 0;
                    }
                }
            }

            var item = await _orderService.SelectAsync(id);
            if (item != null)
            {
                if (item.IdObjectType != (int)OrderType.Reship)
                {
                    throw new AccessDeniedException();
                }
                toReturn = _mapper.ToModel<OrderReshipManageModel>(item);
            }

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<OrderReshipManageModel>> UpdateReshipOrder([FromBody]OrderReshipManageModel model)
        {
            if (!Validate(model))
                return null;

            var order = _mapper.FromModel(model);

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (int.TryParse(sUserId, out userId))
            {
                order.IdEditedBy = userId;
            }

            var sendOrderConfirm = false;
            if (model.CombinedEditOrderStatus != OrderStatus.Cancelled && model.CombinedEditOrderStatus != OrderStatus.Exported && model.CombinedEditOrderStatus != OrderStatus.Shipped)
            {
                await _orderService.OrderTypeSetup(order);
                await _orderService.CalculateOrder(order, model.CombinedEditOrderStatus);

                if (!model.ConfirmationEmailSent &&
                    (model.CombinedEditOrderStatus == OrderStatus.Processed ||
                     model.CombinedEditOrderStatus == OrderStatus.ShipDelayed) &&
                    !string.IsNullOrEmpty(model.Customer.Email))
                {
                    sendOrderConfirm = true;
                    order.Data.ConfirmationEmailSent = true;
                }

                if (model.Id > 0)
                {
                    order = await _orderService.UpdateAsync(order);
                }
                else
                {
                    order = await _orderService.InsertAsync(order);
                }
            }

            if (sendOrderConfirm && !string.IsNullOrEmpty(model.Customer?.Email))
            {
                var emailModel = _mapper.ToModel<OrderConfirmationEmail>(order);
                if (emailModel != null)
                {
                    await _notificationService.SendOrderConfirmationEmailAsync(model.Customer.Email, emailModel);
                }
            }

            OrderReshipManageModel toReturn = _mapper.ToModel<OrderReshipManageModel>(order);

            return toReturn;
        }

        #endregion

        #region Refunds

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<OrderRefundCalculateModel>> CalculateRefundOrder([FromBody]OrderRefundManageModel model)
        {
            var order = _orderRefundMapper.FromModel(model);
            var orderContext = await _orderRefundService.CalculateRefundOrder(order);

            OrderRefundCalculateModel toReturn = new OrderRefundCalculateModel(orderContext);

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpGet]
        public async Task<Result<OrderRefundManageModel>> GetRefundOrder(int id, int? idsource = null, int? idcustomer = null)
        {
            OrderRefundManageModel toReturn = null;
            if (id == 0)
            {
                if (idsource.HasValue)
                {
                    var order = await _orderService.SelectAsync(idsource.Value);
                    if (order != null)
                    {
                        OrderRefundDynamic refund = new OrderRefundDynamic();
                        refund.DateCreated = DateTime.Now;
                        refund.IdObjectType = (int)OrderType.Refund;
                        refund.Customer = order.Customer;
                        refund.IdOrderSource = order.Id;
                        refund.ShippingAddress = order.ShippingAddress;
                        refund.ShippingAddress.Id = 0;
                        refund.OrderStatus = OrderStatus.Processed;
                        AddressDynamic paymentAddress = null;
                        if (order?.PaymentMethod?.Address != null)
                        {
                            paymentAddress = order.PaymentMethod.Address;
                        }
                        else
                        {
                            var customer = order.Customer;
                            if (customer.ProfileAddress != null)
                            {
                                paymentAddress = customer.ProfileAddress;
                            }
                            else
                            {
                                customer = await _customerService.SelectAsync(order.Customer.Id);
                                paymentAddress = customer.ProfileAddress ?? new AddressDynamic();
                            }
                        }
                        paymentAddress.Id = 0;
                        refund.PaymentMethod = new OrderPaymentMethodDynamic()
                        {
                            Address = paymentAddress,
                            IdObjectType = (int)PaymentMethodType.Oac,
                        };
                        toReturn = _orderRefundMapper.ToModel<OrderRefundManageModel>(refund);
                        if (!toReturn.DisableShippingRefunded)
                        {
                            toReturn.ManualShippingTotal = order.ShippingTotal;
                        }
                    }
                }
            }

            var item = await _orderRefundService.SelectAsync(id);
            if (item != null)
            {
                if (item.IdObjectType != (int)OrderType.Refund)
                {
                    throw new AccessDeniedException();
                }
                toReturn = _orderRefundMapper.ToModel<OrderRefundManageModel>(item);
                toReturn.ManualShippingTotal = toReturn.ShippingTotal;
            }

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<OrderRefundManageModel>> AddRefundOrder([FromBody]OrderRefundManageModel model)
        {
            if (!Validate(model))
                return null;

            var order = _orderRefundMapper.FromModel(model);

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (int.TryParse(sUserId, out userId))
            {
                order.IdEditedBy = userId;
            }

            if (model.OrderStatus != OrderStatus.Cancelled && model.OrderStatus != OrderStatus.Exported && model.OrderStatus != OrderStatus.Shipped)
            {
                order.Data.OrderType = (int)SourceOrderType.Phone;
                await _orderRefundService.CalculateRefundOrder(order);

                if (model.Id == 0)
                {
                    order = await _orderRefundService.InsertAsync(order);
                }
            }

            OrderRefundManageModel toReturn = _orderRefundMapper.ToModel<OrderRefundManageModel>(order);

            return toReturn;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<bool>> CancelRefundOrder(int id, [FromBody] object model)
        {
            return await _orderRefundService.CancelRefundOrderAsync(id);
        }

        #endregion

        #region OrderUpdateHistory

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !string.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (OrderDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(OrderDynamic));
                var model = GetOrderManageModel(dynamic, toReturn.Main.Data);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !string.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (OrderDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(OrderDynamic));
                var model = GetOrderManageModel(dynamic, toReturn.Before.Data);
                toReturn.Before.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }

            return toReturn;
        }

        private object GetOrderManageModel(OrderDynamic dynamic, string data)
        {
            object model;
            if (dynamic.IdObjectType == (int)OrderType.Reship)
            {
                OrderReshipManageModel model2 = _mapper.ToModel<OrderReshipManageModel>(dynamic);
                model = model2;
            }
            else if (dynamic.IdObjectType == (int)OrderType.Refund)
            {
                var refund = (OrderRefundDynamic)JsonConvert.DeserializeObject(data, typeof(OrderRefundDynamic));
                model = _orderRefundMapper.ToModel<OrderRefundManageModel>(refund);
            }
            else
            {
                model = _mapper.ToModel<OrderManageModel>(dynamic);
            }
            return model;
        }

        #endregion

        #region Reports

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<ICollection<OrdersRegionStatisticItem>>> GetOrdersRegionStatistic([FromBody]OrderRegionFilter filter)
        {
            var toReturn = await _orderService.GetOrdersRegionStatisticAsync(filter);
            return toReturn.ToList();
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<FileResult> GetOrdersRegionStatisticReportFile([FromQuery]DateTime from, [FromQuery]DateTime to,
            [FromQuery]int? idcustomertype = null, [FromQuery]int? idordertype = null)
        {
            OrderRegionFilter filter = new OrderRegionFilter()
            {
                From = from,
                To = to,
                IdCustomerType = idcustomertype,
                IdOrderType = idordertype,
            };

            var items = await _orderService.GetOrdersRegionStatisticAsync(filter);

            var data = _ordersRegionStatisticItemCSVExportService.ExportToCsv(items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.REGIONAL_SALES_STATISTIC, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<ICollection<OrdersZipStatisticItem>>> GetOrdersZipStatistic([FromBody]OrderRegionFilter filter)
        {
            var toReturn = await _orderService.GetOrdersZipStatisticAsync(filter);
            return toReturn.ToList();
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<FileResult> GetOrdersZipStatisticReportFile([FromQuery]DateTime from, [FromQuery]DateTime to,
            [FromQuery]int? idcustomertype = null, [FromQuery]int? idordertype = null)
        {
            OrderRegionFilter filter = new OrderRegionFilter()
            {
                From = from,
                To = to,
                IdCustomerType = idcustomertype,
                IdOrderType = idordertype,
            };

            var items = await _orderService.GetOrdersZipStatisticAsync(filter);

            var data = _ordersZipStatisticItemCSVExportService.ExportToCsv(items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.REGIONAL_SALES_STATISTIC, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(data, "text/csv");
        }


        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<PagedList<VOrderWithRegionInfoItem>>> GetOrderWithRegionInfoItems([FromBody]OrderRegionFilter filter)
        {
            var toReturn = await _orderService.GetOrderWithRegionInfoItemsAsync(filter);
            return toReturn;
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpGet]
        public async Task<FileResult> GetOrderWithRegionInfoItemsReportFile([FromQuery]DateTime from, [FromQuery]DateTime to,
            [FromQuery]int? idcustomertype = null, [FromQuery]int? idordertype = null, [FromQuery]string region = null, [FromQuery]string zip = null)
        {
            OrderRegionFilter filter = new OrderRegionFilter()
            {
                From = from,
                To = to,
                IdCustomerType = idcustomertype,
                IdOrderType = idordertype,
                Region = region,
                Zip = zip,
            };
            filter.Paging = null;

            var data = await _orderService.GetOrderWithRegionInfoItemsAsync(filter);
            foreach (var item in data.Items)
            {
                item.DateCreated = TimeZoneInfo.ConvertTime(item.DateCreated, TimeZoneInfo.Local, _pstTimeZoneInfo);
            }

            var result = _vOrderWithRegionInfoItemCSVExportService.ExportToCsv(data.Items);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.REGIONAL_SALES_DETAILS_STATISTIC, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(result, "text/csv");
        }

        [AdminAuthorize(PermissionType.Reports)]
        [HttpPost]
        public async Task<Result<decimal>> GetOrderWithRegionInfoAmount([FromBody]OrderRegionFilter filter)
        {
            var toReturn = await _orderService.GetOrderWithRegionInfoAmountAsync(filter);
            return toReturn;
        }

        [HttpPost]
        public async Task<Result<OrdersAgentReport>> GetOrdersAgentReport([FromBody]OrdersAgentReportFilter filter)
        {
            filter.To = filter.To.AddDays(1);
            var toReturn = await _orderReportService.GetOrdersAgentReportAsync(filter);
            //correct dates for UI
            if (toReturn.FrequencyType == FrequencyType.Weekly || toReturn.FrequencyType == FrequencyType.Monthly)
            {
                for (int i = 0; i < toReturn.Periods.Count; i++)
                {
                    toReturn.Periods[i].To = toReturn.Periods[i].To.AddDays(-1);
                }
            }
            return toReturn;
        }

        [HttpGet]
        public async Task<FileResult> GetOrdersAgentReportFile([FromQuery]string from, [FromQuery]string to,
            [FromQuery]FrequencyType frequencytype, [FromQuery]int? idadminteam = null, [FromQuery]int? idadmin = null)
        {
            var dFrom = from.GetDateFromQueryStringInPst(_pstTimeZoneInfo);
            var dTo = to.GetDateFromQueryStringInPst(_pstTimeZoneInfo);
            if (!dFrom.HasValue || !dTo.HasValue)
            {
                return null;
            }

            OrdersAgentReportFilter filter = new OrdersAgentReportFilter()
            {
                From = dFrom.Value,
                To = dTo.Value.AddDays(1),
                FrequencyType = frequencytype,
                IdAdminTeam = idadminteam,
                IdAdmin = idadmin,
            };

            var superAdmin = HttpContext.RequestServices.GetService<IAppInfrastructureService>()
                .Data()
                .AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser)
                .Text;

            var fullReport = User.IsInRole(superAdmin.Normalize()) || User.HasClaim(x => x.Type == IdentityConstants.PermissionRoleClaimType && x.Value == ((int)PermissionType.Reports).ToString());

            var data = await _orderReportService.GetOrdersAgentReportAsync(filter);
            //correct dates for UI
            if (data.FrequencyType == FrequencyType.Weekly || data.FrequencyType == FrequencyType.Monthly)
            {
                for (int i = 0; i < data.Periods.Count; i++)
                {
                    data.Periods[i].To = data.Periods[i].To.AddDays(-1);
                }
            }
            foreach (var item in data.Periods)
            {
                item.From = TimeZoneInfo.ConvertTime(item.From, TimeZoneInfo.Local, _pstTimeZoneInfo);
                item.To = TimeZoneInfo.ConvertTime(item.To, TimeZoneInfo.Local, _pstTimeZoneInfo);
            }

            var items = _orderReportService.ConvertOrdersAgentReportToExportItems(data, fullReport);

            var result = _ordersAgentReportExportItemCSVExportService.ExportToCsv(items, false);

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = String.Format(FileConstants.ORDERS_AGENT_REPORT, DateTime.Now)
            };

            Response.Headers.Add("Content-Disposition", contentDisposition.ToString());
            return File(result, "text/csv");
        }

        #endregion

        #region GCOrders

        [HttpGet]
        public async Task<Result<ICollection<GCOrderListItemModel>>> GetGCOrders(int id)
        {
            var toReturn = await _orderService.GetGCOrdersAsync(id);
            return toReturn.Select(p => new GCOrderListItemModel(p)).ToList();
        }

        #endregion

        #region ImportOrders

        [HttpPost]
        [AdminAuthorize(PermissionType.Customers)]
        [AdminAuthorize(PermissionType.Orders)]
        public async Task<Result<bool>> ImportOrders()
        {
            var form = await Request.ReadFormAsync();

            var idCustomer = Int32.Parse(form["idcustomer"]);
            var idPaymentMethod = Int32.Parse(form["idpaymentmethod"]);
            OrderType orderType = (OrderType)Int32.Parse(form["ordertype"]);

            var parsedContentDisposition = ContentDispositionHeaderValue.Parse(form.Files[0].ContentDisposition);

            var contentType = form.Files[0].ContentType;
            using (var stream = form.Files[0].OpenReadStream())
            {
                var fileContent = stream.ReadFully();

                var sUserId = Request.HttpContext.User.GetUserId();
                int userId;
                var toReturn = false;
                if (int.TryParse(sUserId, out userId))
                {
                    toReturn = await _orderService.ImportOrders(fileContent, parsedContentDisposition.FileName, orderType, idCustomer, idPaymentMethod, userId);
                }

                return toReturn;
            }
        }

        #endregion

        #region OrderEmails

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<bool>> SendOrderConfirmationEmail(int id, [FromBody]OrderManualSendConfirmationModel model)
        {
            var order = await _orderService.SelectAsync(id, true);
            if (order == null || order.OrderStatus == OrderStatus.Cancelled)
            {
                return false;
            }

            var emailModel = _mapper.ToModel<OrderConfirmationEmail>(order);
            if (emailModel == null)
                return false;

            await _notificationService.SendOrderConfirmationEmailAsync(model.Email, emailModel);
            return true;
        }

        [AdminAuthorize(PermissionType.Orders)]
        [HttpPost]
        public async Task<Result<bool>> SendOrderShippingConfirmationEmail(int id, [FromBody]OrderManualSendConfirmationModel model)
        {
            var order = await _orderService.SelectAsync(id, true);
            if (order == null || order.OrderStatus == OrderStatus.Cancelled)
            {
                return false;
            }

            if (model.SendAll)
            {
                var emailModel = _mapper.ToModel<OrderShippingConfirmationEmail>(order);
                if (emailModel == null)
                    return false;
                await _notificationService.SendOrderShippingConfirmationEmailAsync(model.Email, emailModel);
            }
            else
            {
                if (model.SendP)
                {
                    order.SendSide = (int)POrderType.P;
                    var emailModel = _mapper.ToModel<OrderShippingConfirmationEmail>(order);
                    if (emailModel == null)
                        return false;

                    await _notificationService.SendOrderShippingConfirmationEmailAsync(model.Email, emailModel);
                }
                if (model.SendNP)
                {
                    order.SendSide = (int)POrderType.NP;
                    var emailModel = _mapper.ToModel<OrderShippingConfirmationEmail>(order);
                    if (emailModel == null)
                        return false;

                    await _notificationService.SendOrderShippingConfirmationEmailAsync(model.Email, emailModel);
                }
            }
            return true;
        }

        #endregion
    }
}