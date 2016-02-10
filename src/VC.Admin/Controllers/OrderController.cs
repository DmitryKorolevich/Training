﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VC.Admin.Models.Product;
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

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Orders)]
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IDynamicMapper<OrderDynamic, Order> _mapper;
        private readonly IDynamicMapper<AddressDynamic, OrderAddress> _addressMapper;
        private readonly ICustomerService _customerService;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly ICsvExportService<OrdersRegionStatisticItem, OrdersRegionStatisticItemCsvMap> _ordersRegionStatisticItemCSVExportService;
        private readonly ICsvExportService<OrdersZipStatisticItem, OrdersZipStatisticItemCsvMap> _ordersZipStatisticItemCSVExportService;
        private readonly ICsvExportService<VOrderWithRegionInfoItem, VOrderWithRegionInfoItemCsvMap> _vOrderWithRegionInfoItemCSVExportService;
        private readonly IProductService _productService;
        private readonly TimeZoneInfo _pstTimeZoneInfo;
        private readonly ILogger logger;

        public OrderController(
            IOrderService orderService,
            ILoggerProviderExtended loggerProvider,
            IDynamicMapper<OrderDynamic, Order> mapper,
            ICustomerService customerService,
            IDynamicMapper<AddressDynamic, OrderAddress> addressMapper,
            ICsvExportService<OrdersRegionStatisticItem, OrdersRegionStatisticItemCsvMap> ordersRegionStatisticItemCSVExportService,
            ICsvExportService<OrdersZipStatisticItem, OrdersZipStatisticItemCsvMap> ordersZipStatisticItemCSVExportService,
            ICsvExportService<VOrderWithRegionInfoItem, VOrderWithRegionInfoItemCsvMap> vOrderWithRegionInfoItemCSVExportService,
            IProductService productService,
            IObjectHistoryLogService objectHistoryLogService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _customerService = customerService;
            _addressMapper = addressMapper;
            _ordersRegionStatisticItemCSVExportService = ordersRegionStatisticItemCSVExportService;
            _ordersZipStatisticItemCSVExportService = ordersZipStatisticItemCSVExportService;
            _vOrderWithRegionInfoItemCSVExportService = vOrderWithRegionInfoItemCSVExportService;
            _productService = productService;
            _objectHistoryLogService = objectHistoryLogService;
            _pstTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            this.logger = loggerProvider.CreateLoggerDefault();
        }

        [HttpPost]
        public async Task<Result<PagedList<ShortOrderListItemModel>>> GetShortOrders([FromBody]ShortOrderFilter filter)
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
            var result = await _orderService.GetOrdersAsync(filter);

            var toReturn = new PagedList<OrderListItemModel>
            {
                Items = result.Items.Select(p => new OrderListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<OrderManageModel>> GetOrder(int id, int? idcustomer = null, bool refreshprices=false)
        {
            if (id == 0)
            {
                var order = await _orderService.CreateNewNormalOrder(OrderStatus.Processed);
                if (idcustomer.HasValue)
                {
                    order.Data.OrderNotes = await _customerService.GetNewOrderNotesBasedOnCustomer(idcustomer.Value);
                }

                var model = _mapper.ToModel<OrderManageModel>(order);

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
            if(id!=0 && refreshprices && item.Skus!=null)
            {
                var customer = await _customerService.SelectAsync(item.Customer.Id);
                foreach(var orderSku in item.Skus)
                {
                    if(orderSku.Sku!=null)
                    {
                        orderSku.Amount = customer.IdObjectType == (int)CustomerType.Retail ? orderSku.Sku.Price : orderSku.Sku.WholesalePrice;
                    }
                }
            }

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(item);

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<OrderCalculateModel>> CalculateOrder([FromBody]OrderManageModel model)
        {
            var item = _mapper.FromModel(model);
            var orderContext = await _orderService.CalculateOrder(item);

            OrderCalculateModel toReturn = new OrderCalculateModel(orderContext);

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<OrderManageModel>> UpdateOrder([FromBody]OrderManageModel model)
        {
            if (!Validate(model))
                return null;

            var item = _mapper.FromModel(model);

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (int.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }
            item.Customer.IdEditedBy = userId;
            foreach (var address in item.Customer.ShippingAddresses)
            {
                address.IdEditedBy = userId;
            }
            item.Customer.ProfileAddress.IdEditedBy = userId;

            await _customerService.UpdateAsync(item.Customer);

            if (model.CombinedEditOrderStatus != OrderStatus.Cancelled && model.CombinedEditOrderStatus != OrderStatus.Exported && model.CombinedEditOrderStatus != OrderStatus.Shipped)
            {
                var orderType = item.Data.MailOrder ? (int?)SourceOrderType.MailOrder : null;
                if (model.Id > 0)
                {
                    var dbItem = (await _orderService.SelectAsync(item.Id));
                    if (dbItem != null && dbItem.DictionaryData.ContainsKey("OrderType"))
                    {
                        if (dbItem.Data.OrderType == (int?)SourceOrderType.MailOrder)
                        {
                            if (!orderType.HasValue)
                            {
                                orderType = (int)SourceOrderType.Phone;
                            }
                            item.Data.OrderType = orderType.Value;
                        }
                        else
                        {
                            item.Data.OrderType = orderType ?? dbItem.Data.OrderType;
                        }
                    }
                    else
                    {
                        item.Data.OrderType = orderType ?? (int)SourceOrderType.Phone;
                    }
                    var context = await _orderService.CalculateOrder(item);
                    model.ShouldSplit = context.SplitInfo?.ShouldSplit == true;
                    OrderModelConverter.SetOrderSplitStatuses(model, item);
                    item = await _orderService.UpdateAsync(item);
                }
                else
                {
                    if (!orderType.HasValue)
                    {
                        orderType = (int)SourceOrderType.Phone;
                    }
                    item.Data.OrderType = orderType.Value;
                    item.ShippingAddress.Id = 0;
                    if (item.PaymentMethod.Address != null)
                    {
                        item.PaymentMethod.Address.Id = 0;
                    }
                    item.PaymentMethod.Id = 0;
                    var context = await _orderService.CalculateOrder(item);
                    model.ShouldSplit = context.SplitInfo?.ShouldSplit == true;
                    OrderModelConverter.SetOrderSplitStatuses(model, item);
                    item = await _orderService.InsertAsync(item);
                }
            }

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(item);

            //TODO: - add sign up for newsletter(SignUpNewsletter)

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !string.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (OrderDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(OrderDynamic));
                var model = _mapper.ToModel<OrderManageModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !string.IsNullOrEmpty(toReturn.Before.Data))
            {
                var dynamic = (OrderDynamic)JsonConvert.DeserializeObject(toReturn.Before.Data, typeof(OrderDynamic));
                var model = _mapper.ToModel<OrderManageModel>(dynamic);
                toReturn.Before.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }

            return toReturn;
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
    }
}