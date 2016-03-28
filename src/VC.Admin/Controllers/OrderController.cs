using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VitalChoice.Validation.Models;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using System.Security.Claims;
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
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;
using VC.Admin.ModelConverters;
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

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Orders)]
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IOrderRefundService _orderRefundService;
        private readonly OrderMapper _mapper;
        private readonly OrderRefundMapper _orderRefundMapper;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly ICustomerService _customerService;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly IOptions<AppOptions> _options;
        private readonly ICsvExportService<OrdersRegionStatisticItem, OrdersRegionStatisticItemCsvMap> _ordersRegionStatisticItemCSVExportService;
        private readonly ICsvExportService<OrdersZipStatisticItem, OrdersZipStatisticItemCsvMap> _ordersZipStatisticItemCSVExportService;
        private readonly ICsvExportService<VOrderWithRegionInfoItem, VOrderWithRegionInfoItemCsvMap> _vOrderWithRegionInfoItemCSVExportService;
        private readonly IProductService _productService;
        private readonly INotificationService _notificationService;
        private readonly IServiceCodeService _serviceCodeService;
        private readonly BrontoService _brontoService;
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly ILogger logger;

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
            IProductService productService,
            INotificationService notificationService,
            IServiceCodeService serviceCodeService,
            BrontoService brontoService,
            IObjectHistoryLogService objectHistoryLogService, IOptions<AppOptions> options)
        {
            _orderService = orderService;
            _orderRefundService = orderRefundService;
            _mapper = mapper;
            _orderRefundMapper = orderRefundMapper;
            _customerService = customerService;
            _addressMapper = addressMapper;
            _ordersRegionStatisticItemCSVExportService = ordersRegionStatisticItemCSVExportService;
            _ordersZipStatisticItemCSVExportService = ordersZipStatisticItemCSVExportService;
            _vOrderWithRegionInfoItemCSVExportService = vOrderWithRegionInfoItemCSVExportService;
            _productService = productService;
            _notificationService = notificationService;
            _serviceCodeService = serviceCodeService;
            _brontoService = brontoService;
            _objectHistoryLogService = objectHistoryLogService;
            _options = options;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            this.logger = loggerProvider.CreateLoggerDefault();
        }

        [HttpGet]
        public async Task<Result<bool>> GetIsBrontoSubscribed(string id)
        {
            return !(_brontoService.GetIsUnsubscribed(id) ?? false);
        }

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

        [HttpPost]
        public async Task<Result<bool>> UpdateOrderStatus(int id, int status, [FromBody] object model, int? orderpart = null)
        {
            var order = await _orderService.SelectAsync(id, false);

            if (order == null)
            {
                throw new AppValidationException("Id", "The given order doesn't exist.");
            }
            if (!orderpart.HasValue)
            {
                order.OrderStatus = (OrderStatus)status;
            }
            else if(orderpart == 1)//NP
            {
                order.NPOrderStatus = (OrderStatus)status;
            } if(orderpart == 2)//P
            {
                order.POrderStatus = (OrderStatus)status;
            }
            order = await _orderService.UpdateAsync(order);

            return order != null;
        }

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

        //[HttpPost]
        //public async Task<Result<PagedList<OrderListItemModel>>> GetOrders([FromBody]VOrderFilter filter)
        //{
        //    if (filter.To.HasValue)
        //    {
        //        filter.To = filter.To.Value.AddDays(1);
        //    }

        //    var result = await _orderService.GetOrdersAsync2(filter);

        //    var toReturn = new PagedList<OrderListItemModel>
        //    {
        //        Items = result.Items.Select(p => new OrderListItemModel(p)).ToList(),
        //        Count = result.Count,
        //    };

        //    return toReturn;
        //}

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
                model.GCs = new List<GCListItemModel>() {new GCListItemModel(null)};
                model.SkuOrdereds = new List<SkuOrderedManageModel>() {new SkuOrderedManageModel(null)};
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
            if (id != 0 && refreshprices && item.Skus != null)
            {
                var customer = await _customerService.SelectAsync(item.Customer.Id);
                foreach (var orderSku in item.Skus)
                {
                    if (orderSku.Sku != null)
                    {
                        orderSku.Amount = customer.IdObjectType == (int) CustomerType.Retail
                            ? orderSku.Sku.Price
                            : orderSku.Sku.WholesalePrice;
                    }
                }
            }

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(item);

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<OrderCalculateModel>> CalculateOrder([FromBody]OrderManageModel model)
        {
            var order = _mapper.FromModel(model);
            var orderContext = await _orderService.CalculateOrder(order, model.CombinedEditOrderStatus);

            OrderCalculateModel toReturn = new OrderCalculateModel(orderContext);

            return toReturn;
        }

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

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(order);

            if(!string.IsNullOrEmpty(model?.Customer.Email) && model.SignUpNewsletter.HasValue)
            {
                var unsubscribed = _brontoService.GetIsUnsubscribed(model.Customer.Email);
                if (model.SignUpNewsletter.Value && (!unsubscribed.HasValue || unsubscribed.Value))
                {
                    await _brontoService.Subscribe(model.Customer.Email);
                }
                if (!model.SignUpNewsletter.Value)
                {
                    if (!unsubscribed.HasValue)
                    {
                        //Resolve issue with showing the default value only the first time
                        await _brontoService.Subscribe(model.Customer.Email);
                        _brontoService.Unsubscribe(model.Customer.Email);
                    }
                    else if(!unsubscribed.Value)
                    {
                        _brontoService.Unsubscribe(model.Customer.Email);
                    }
                }
            }

            return toReturn;
        }

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
                        order.GiftCertificates=new List<GiftCertificateInOrder>();
                        order.Discount = null;
                        toReturn = _mapper.ToModel<OrderReshipManageModel>(order);
                        toReturn.KeyCode = "RESHIP";
                        toReturn.IdObjectType = (int)OrderType.Reship;
                        toReturn.IdOrderSource = toReturn.Id;
                        toReturn.OrderSourceDateCreated = toReturn.DateCreated;
                        toReturn.OrderSourceTotal = toReturn.Total;
                        toReturn.OrderNotes=String.Empty;
                        toReturn.ConfirmationEmailSent = false;
                        if (toReturn.SkuOrdereds != null && toReturn.PromoSkus != null)
                        {
                            toReturn.SkuOrdereds.AddRange(toReturn.PromoSkus.Select(p=>p.ConvertToBase()).ToList());
                            toReturn.PromoSkus=new List<PromoSkuOrderedManageModel>();
                        }
                        toReturn.SkuOrdereds?.ForEach(p =>
                        {
                            p.Price = 0;
                            p.Amount = 0;
                        });
                        toReturn.ReshipProblemSkus =
                            toReturn.SkuOrdereds?.Where(p=>p.IdSku.HasValue).Select(p => new ReshipProblemSkuModel()
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
                toReturn = _mapper.ToModel<OrderReshipManageModel>(item);
            }

            return toReturn;
        }

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

        [HttpPost]
        public async Task<Result<bool>> CancelOrder(int id, [FromBody] object model)
        {
            return await _orderService.CancelOrderAsync(id);
        }

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
            if (dynamic.IdObjectType == (int) OrderType.Reship)
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

        [HttpPost]
        public async Task<Result<ICollection<OrdersRegionStatisticItem>>> GetOrdersRegionStatistic([FromBody]OrderRegionFilter filter)
        {
            var toReturn = await _orderService.GetOrdersRegionStatisticAsync(filter);
            return toReturn.ToList();
        }

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

        [HttpPost]
        public async Task<Result<ICollection<OrdersZipStatisticItem>>> GetOrdersZipStatistic([FromBody]OrderRegionFilter filter)
        {
            var toReturn = await _orderService.GetOrdersZipStatisticAsync(filter);
            return toReturn.ToList();
        }

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


        [HttpPost]
        public async Task<Result<PagedList<VOrderWithRegionInfoItem>>> GetOrderWithRegionInfoItems([FromBody]OrderRegionFilter filter)
        {
            var toReturn = await _orderService.GetOrderWithRegionInfoItemsAsync(filter);
            return toReturn;
        }

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

        [HttpPost]
        public async Task<Result<decimal>> GetOrderWithRegionInfoAmount([FromBody]OrderRegionFilter filter)
        {
            var toReturn = await _orderService.GetOrderWithRegionInfoAmountAsync(filter);
            return toReturn;
        }

        [HttpGet]
        public async Task<Result<ICollection<GCOrderListItemModel>>> GetGCOrders(int id)
        {
            var toReturn = await _orderService.GetGCOrdersAsync(id);
            return toReturn.Select(p => new GCOrderListItemModel(p)).ToList();
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Customers)]
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

        [HttpPost]
        public async Task<Result<ServiceCodesReport>> GetServiceCodesReport([FromBody]ServiceCodesReportFilter filter)
        {
            if (filter.To.HasValue)
            {
                filter.To = filter.To.Value.AddDays(1);
            }
            return await _serviceCodeService.GetServiceCodesReportAsync(filter);
        }

        [HttpPost]
        public async Task<Result<OrderRefundCalculateModel>> CalculateRefundOrder([FromBody]OrderRefundManageModel model)
        {
            var order = _orderRefundMapper.FromModel(model);
            var orderContext = await _orderRefundService.CalculateRefundOrder(order);

            OrderRefundCalculateModel toReturn = new OrderRefundCalculateModel(orderContext);

            return toReturn;
        }

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
                        OrderRefundDynamic refund=new OrderRefundDynamic();
                        refund.DateCreated = DateTime.Now;
                        refund.IdObjectType = (int)OrderType.Refund;
                        refund.Customer = order.Customer;
                        refund.IdOrderSource = order.Id;
                        refund.ShippingAddress = order.ShippingAddress;
                        refund.ShippingAddress.Id = 0;
                        refund.OrderStatus=OrderStatus.Processed;
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
                    }
                }
            }

            var item = await _orderRefundService.SelectAsync(id);
            if (item != null)
            {
                toReturn = _orderRefundMapper.ToModel<OrderRefundManageModel>(item);
            }

            return toReturn;
        }

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
                await _orderRefundService.CalculateRefundOrder(order);

                if (model.Id == 0)
                {
                    order = await _orderRefundService.InsertAsync(order);
                }
            }

            OrderRefundManageModel toReturn = _orderRefundMapper.ToModel<OrderRefundManageModel>(order);

            return toReturn;
        }
        
        [HttpPost]
        public async Task<Result<bool>> CancelRefundOrder(int id, [FromBody] object model)
        {
            return await _orderRefundService.CancelRefundOrderAsync(id);
        }
    }
}