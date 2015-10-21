using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models;
using VC.Admin.Models.Product;
using VitalChoice.Business.Services;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Validation.Models;
using VitalChoice.Domain.Entities;
using VitalChoice.DynamicData.Entities;
using System;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Core.Services;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.Interfaces.Services.Products;
using System.Security.Claims;
using VitalChoice.Business.Services.Dynamic;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Interfaces.Services;
using VC.Admin.Models.Order;
using VitalChoice.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Orders;
using System.Threading;
using VC.Admin.Models.Customer;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Settings;
using VitalChoice.Interfaces.Services.Settings;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Orders)]
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IDynamicToModelMapper<OrderDynamic> _mapper;
        private readonly IDynamicToModelMapper<OrderAddressDynamic> _addressMapper;
        private readonly ICustomerService _customerService;
        private readonly IObjectHistoryLogService _objectHistoryLogService;
        private readonly ILogger logger;

        public OrderController(
            IOrderService orderService,
            ILoggerProviderExtended loggerProvider,
            IDynamicToModelMapper<OrderDynamic> mapper, ICustomerService customerService,
            IDynamicToModelMapper<OrderAddressDynamic> addressMapper,
            IObjectHistoryLogService objectHistoryLogService)
        {
            _orderService = orderService;
            _mapper = mapper;
            _customerService = customerService;
            _addressMapper = addressMapper;
            _objectHistoryLogService = objectHistoryLogService;
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
        public async Task<Result<bool>> UpdateOrderStatus(int id, int status)
        {
            var order = await _orderService.SelectAsync(id,false);

            if(order==null)
            {
                throw new AppValidationException("Id", "The given order doesn't exist.");
            }
            order.OrderStatus = (OrderStatus)status;
            order = await _orderService.UpdateAsync(order);

            return order!=null;
        }

        [HttpPost]
        public async Task<Result<bool>> MoveOrder(int id, int idcustomer)
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
        public async Task<Result<OrderManageModel>> GetOrder(int id)
        {
            if (id == 0)
            {
                var model = await _orderService.CreatePrototypeForAsync<OrderManageModel>((int) OrderType.Normal);
                model.GCs = new List<GCListItemModel>() {new GCListItemModel(null)};
                model.SkuOrdereds = new List<SkuOrderedManageModel>() {new SkuOrderedManageModel(null)};
                model.StatusCode = RecordStatusCode.Active;
                model.OrderStatus = OrderStatus.Processed;
                model.DateCreated = DateTime.Now;
                model.PreferredShipMethod = 1;
                model.ShipDelayType = 0;
                model.UpdateShippingAddressForCustomer = true;
                model.UpdateCardForCustomer = true;
                model.UpdateCheckForCustomer = true;
                model.UpdateOACForCustomer = true;
                return model;
            }

            var item = await _orderService.SelectAsync(id);

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

            var pOrder = false;
            var npOrder = false;
            foreach(var skuOrdered in item.Skus)
            {
                pOrder = pOrder || skuOrdered.ProductWithoutSkus.IdObjectType == (int)ProductType.Perishable;
                npOrder = npOrder || skuOrdered.ProductWithoutSkus.IdObjectType == (int)ProductType.NonPerishable;
            }
            if(pOrder && npOrder)
            {
                item.Data.POrderType = (int)POrderType.PNP;
            } else if(pOrder)
            {
                item.Data.POrderType = (int)POrderType.P;
            } else if(npOrder)
            {
                item.Data.POrderType = (int)POrderType.NP;
            }

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }
            item.Customer.IdEditedBy = userId;
            foreach (var address in item.Customer.Addresses)
            {
                address.IdEditedBy = userId;
            }
            foreach (var customerNote in item.Customer.CustomerNotes)
            {
                customerNote.IdEditedBy = userId;
            }

            await _customerService.UpdateAsync(item.Customer);

            var orderType = item.Data.MailOrder ? (int?)SourceOrderType.MailOrder : null;
            if (model.Id > 0)
            {
                var dbItem = (await _orderService.SelectAsync(item.Id));
                if(dbItem!=null && dbItem.DictionaryData.ContainsKey("OrderType"))
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
                        item.Data.OrderType = orderType.HasValue ? orderType.Value : dbItem.Data.OrderType;
                    }
                }
                else
                {
                    item.Data.OrderType = orderType.HasValue ? orderType.Value : (int)SourceOrderType.Phone;
                }
                var context = await _orderService.CalculateOrder(item);
                _orderService.UpdateOrderFromCalculationContext(item, context);
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
                item.PaymentMethod.Address.Id = 0;
                item.PaymentMethod.Id = 0;
                //var context = await _orderService.CalculateOrder(item);
                //_orderService.UpdateOrderFromCalculationContext(item, context);
                item = await _orderService.InsertAsync(item);
            }

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(item);

            //TODO: - add sign up for newsletter(SignUpNewsletter)

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<ObjectHistoryReportModel>> GetHistoryReport([FromBody]ObjectHistoryLogItemsFilter filter)
        {
            var toReturn = await _objectHistoryLogService.GetObjectHistoryReport(filter);

            if (toReturn.Main != null && !String.IsNullOrEmpty(toReturn.Main.Data))
            {
                var dynamic = (OrderDynamic)JsonConvert.DeserializeObject(toReturn.Main.Data, typeof(OrderDynamic));
                var model = _mapper.ToModel<OrderManageModel>(dynamic);
                toReturn.Main.Data = JsonConvert.SerializeObject(model, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Include,
                });
            }
            if (toReturn.Before != null && !String.IsNullOrEmpty(toReturn.Before.Data))
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
    }
}