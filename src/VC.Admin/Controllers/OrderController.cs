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
using VitalChoice.Workflow.Contexts;
using System.Threading;
using VC.Admin.Models.Customer;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Exceptions;

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Orders)]
    public class OrderController : BaseApiController
    {
        private readonly IOrderService _orderService;
        private readonly IEcommerceDynamicObjectService<OrderDynamic, Order, OrderOptionType, OrderOptionValue> _simpleOrderService;
        private readonly IDynamicToModelMapper<OrderDynamic> _mapper;
        private readonly ICustomerService _customerService;
        private readonly ILogger logger;

        public OrderController(IOrderService orderService, IEcommerceDynamicObjectService<OrderDynamic, Order, OrderOptionType, OrderOptionValue> simpleOrderService,
            ILoggerProviderExtended loggerProvider, IDynamicToModelMapper<OrderDynamic> mapper, ICustomerService customerService)
        {
            _orderService = orderService;
            _simpleOrderService = simpleOrderService;
            _mapper = mapper;
            _customerService = customerService;
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
            var order = await _simpleOrderService.SelectAsync(id,false);

            if(order==null)
            {
                throw new AppValidationException("Id", "The given order doesn't exist.");
            }
            order.OrderStatus = (OrderStatus)status;
            order = await _simpleOrderService.UpdateAsync(order);

            return order!=null;
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

            toReturn.Items = new List<OrderListItemModel>() {
                new OrderListItemModel(null)
                {
                    Company="Test company name",
                    StateCode="WA",
                    Customer="Test, Customer",
                    IdCustomer=7888921,
                    Id=12345,
                    Total= (decimal)55.12,
                    EditedByAgentId="KK",
                    DateEdited=DateTime.Now,
                    OrderNotes="Some notes",
                    OrderStatus=VitalChoice.Domain.Entities.eCommerce.Orders.OrderStatus.Incomplete,
                    DateCreated=DateTime.Now,
                    IdOrderSource=1,
                    IdPaymentMethod=1,
                },
                new OrderListItemModel(null)
                {
                    Company="Test company name2",
                    StateCode="WA",
                    Customer="Test2, Customer2",
                    IdCustomer=7888921,
                    Id=34567,
                    Total= (decimal)55.12,
                    EditedByAgentId="KA",
                    DateEdited=DateTime.Now,
                    OrderNotes=null,
                    OrderStatus=VitalChoice.Domain.Entities.eCommerce.Orders.OrderStatus.Processed,
                    DateCreated=DateTime.Now,
                    IdOrderSource=2,
                    IdPaymentMethod=2,
                },
                new OrderListItemModel(null)
                {
                    Company="Test company name4",
                    StateCode="WA",
                    Customer="Test4, Customer4",
                    IdCustomer=7888921,
                    Id=12345,
                    Total= (decimal)155.12,
                    EditedByAgentId="KK",
                    DateEdited=DateTime.Now,
                    OrderNotes="Some notes",
                    OrderStatus=VitalChoice.Domain.Entities.eCommerce.Orders.OrderStatus.Shipped,
                    DateCreated=DateTime.Now,
                    IdOrderSource=3,
                    IdPaymentMethod=3,
                },
                new OrderListItemModel(null)
                {
                    Company="Test company name5",
                    StateCode="WA",
                    Customer="Test5, Customer5",
                    IdCustomer=7888921,
                    Id=34567,
                    Total= (decimal)155.12,
                    EditedByAgentId="KA",
                    DateEdited=DateTime.Now,
                    OrderNotes=null,
                    OrderStatus=VitalChoice.Domain.Entities.eCommerce.Orders.OrderStatus.Cancelled,
                    DateCreated=DateTime.Now,
                    IdOrderSource=3,
                    IdPaymentMethod=4,
                },
                new OrderListItemModel(null)
                {
                    Company="Test company name5",
                    StateCode="WA",
                    Customer="Test5, Customer5",
                    IdCustomer=7888921,
                    Id=34567,
                    Total= (decimal)155.12,
                    EditedByAgentId="KA",
                    DateEdited=DateTime.Now,
                    OrderNotes=null,
                    OrderStatus=VitalChoice.Domain.Entities.eCommerce.Orders.OrderStatus.Exported,
                    DateCreated=DateTime.Now,
                    IdOrderSource=3,
                    IdPaymentMethod=5,
                },
                new OrderListItemModel(null)
                {
                    Company="Test company name5",
                    StateCode="WA",
                    Customer="Test5, Customer5",
                    IdCustomer=7888921,
                    Id=34567,
                    Total= (decimal)155.12,
                    EditedByAgentId="KA",
                    DateEdited=DateTime.Now,
                    OrderNotes=null,
                    OrderStatus=VitalChoice.Domain.Entities.eCommerce.Orders.OrderStatus.ShipDelayed,
                    DateCreated=DateTime.Now,
                    IdOrderSource=3,
                    IdPaymentMethod=6,
                },
                new OrderListItemModel(null)
                {
                    Company="Test company name5",
                    StateCode="WA",
                    Customer="Test5, Customer5",
                    IdCustomer=7888921,
                    Id=34567,
                    Total= (decimal)155.12,
                    EditedByAgentId="KA",
                    DateEdited=DateTime.Now,
                    OrderNotes=null,
                    OrderStatus=VitalChoice.Domain.Entities.eCommerce.Orders.OrderStatus.OnHold,
                    DateCreated=DateTime.Now,
                    IdOrderSource=3,
                    IdPaymentMethod=5,
                },
            };
            toReturn.Count = 4;

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<OrderManageModel>> GetOrder(int id)
        {
            if (id == 0)
            {
                var model = await _orderService.CreatePrototypeForAsync<OrderManageModel>((int) OrderType.RetailOrder);
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
            if (model.Id > 0)
            {
                item = (await _orderService.UpdateAsync(item));
            }
            else
            {
                item.Data.OrderType = (int)SourceOrderType.Phone;
                item.ShippingAddress.Id = 0;
                item.PaymentMethod.Address.Id = 0;
                item.PaymentMethod.Id = 0;
                item = (await _orderService.InsertAsync(item));
            }

            //update customer
            var dbCustomer = await _customerService.SelectAsync(item.Customer.Id);
            if (dbCustomer != null)
            {
                item.Customer.IdEditedBy = userId;
                foreach (var address in item.Customer.Addresses)
                {
                    address.IdEditedBy = userId;
                }
                foreach (var customerNote in item.Customer.CustomerNotes)
                {
                    customerNote.IdEditedBy = userId;
                }
                dbCustomer.CustomerNotes = item.Customer.CustomerNotes;
                dbCustomer.Files = item.Customer.Files;
                if(model.Id==0)
                {
                    dbCustomer.ApprovedPaymentMethods = item.Customer.ApprovedPaymentMethods;
                    dbCustomer.OrderNotes = item.Customer.OrderNotes;

                    var profileAddress = dbCustomer.Addresses.FirstOrDefault(p => p.IdObjectType == (int)AddressType.Profile);
                    if(profileAddress!=null)
                    {
                        dbCustomer.Addresses.Remove(profileAddress);
                    }
                    profileAddress= item.Customer.Addresses.FirstOrDefault(p => p.IdObjectType == (int)AddressType.Profile);
                    if (profileAddress != null)
                    {
                        dbCustomer.Addresses.Add(profileAddress);
                    }

                    if(model.UpdateShippingAddressForCustomer)
                    {
                        var shippingAddress = dbCustomer.Addresses.FirstOrDefault(p => p.IdObjectType == (int)AddressType.Shipping);
                        if (shippingAddress != null)
                        {
                            dbCustomer.Addresses.Remove(shippingAddress);
                        }
                        shippingAddress = item.Customer.Addresses.FirstOrDefault(p => p.IdObjectType == (int)AddressType.Shipping);
                        if (shippingAddress != null)
                        {
                            dbCustomer.Addresses.Add(profileAddress);
                        }
                    }

                    RemovePaymentMethodsFromDBCusomer(dbCustomer, item.PaymentMethod.IdObjectType, PaymentMethodType.CreditCard, model.UpdateCardForCustomer);
                    foreach(var card in item.Customer.CustomerPaymentMethods.Where(p=>p.IdObjectType==(int)PaymentMethodType.CreditCard))
                    {
                        dbCustomer.CustomerPaymentMethods.Add(card);
                    }
                    RemovePaymentMethodsFromDBCusomer(dbCustomer, item.PaymentMethod.IdObjectType, PaymentMethodType.Oac, model.UpdateOACForCustomer);
                    foreach (var card in item.Customer.CustomerPaymentMethods.Where(p => p.IdObjectType == (int)PaymentMethodType.Oac))
                    {
                        dbCustomer.CustomerPaymentMethods.Add(card);
                    }
                    RemovePaymentMethodsFromDBCusomer(dbCustomer, item.PaymentMethod.IdObjectType, PaymentMethodType.Check, model.UpdateCheckForCustomer);
                    foreach (var card in item.Customer.CustomerPaymentMethods.Where(p => p.IdObjectType == (int)PaymentMethodType.Check))
                    {
                        dbCustomer.CustomerPaymentMethods.Add(card);
                    }
                }
            }

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(item);

            //TODO - add sign up for newsletter(SignUpNewsletter)

            return toReturn;
        }

        private void RemovePaymentMethodsFromDBCusomer(CustomerDynamic customer, int? orderPaymentMethod, PaymentMethodType method, bool update)
        {
            if (orderPaymentMethod == (int)method && update)
            {
                var customerPaymentMethods = customer.CustomerPaymentMethods.Where(p => p.IdObjectType == (int)method).ToList();
                foreach (var customerPaymentMethod in customerPaymentMethods)
                {
                    customer.CustomerPaymentMethods.Remove(customerPaymentMethod);
                }
            }
        }
    }
}