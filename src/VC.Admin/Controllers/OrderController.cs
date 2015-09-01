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

namespace VC.Admin.Controllers
{
    [AdminAuthorize(PermissionType.Orders)]
    public class OrderController : BaseApiController
    {
        private readonly IOrderService orderService;
        private readonly IDynamicToModelMapper<OrderDynamic> _mapper;
        private readonly ILogger logger;

        public OrderController(IOrderService orderService,
            ILoggerProviderExtended loggerProvider, IDynamicToModelMapper<OrderDynamic> mapper)
        {
            this.orderService = orderService;
            _mapper = mapper;
            this.logger = loggerProvider.CreateLoggerDefault();
        }
        
        [HttpPost]
        public async Task<Result<PagedList<OrderListItemModel>>> GetOrders([FromBody]VOrderFilter filter)
        {
            var result = await orderService.GetOrdersAsync(filter);

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
                var model = orderService.CreatePrototypeFor<OrderManageModel>(1);//normal
                model.IdCustomer = 84920494;
                model.GCs = new List<GCListItemModel>() { new GCListItemModel(null) };
                model.SkuOrdereds = new List<SkuOrderedManageModel>() { new SkuOrderedManageModel(null) };
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
            else
            {
                return new OrderManageModel()
                {
                    IdObjectType = 1,//normal
                    IdCustomer = 84920494,
                    StatusCode = RecordStatusCode.Active,
                    OrderStatus = OrderStatus.Processed,
                    DateCreated = DateTime.Now,
                    GCs = new List<GCListItemModel>() { new GCListItemModel(null) },
                    SkuOrdereds = new List<SkuOrderedManageModel>() { new SkuOrderedManageModel(null) },
                    PreferredShipMethod = 1,
                    ShipDelayType = 0,
                    Shipping=new AddressModel(),
                };
            }

            var item = await orderService.SelectAsync(id);

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(item);

            return toReturn;
        }

        [HttpPost]
        public async Task<Result<OrderCalculateModel>> CalculateOrder([FromBody]OrderManageModel model)
        {
            var item = _mapper.FromModel(model);

            var orderContext = await orderService.CalculateOrder(item);

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
            if (Int32.TryParse(sUserId, out userId))
            {
                item.IdEditedBy = userId;
            }
            if (model.Id > 0)
                item = (await orderService.UpdateAsync(item));
            else
                item = (await orderService.InsertAsync(item));

            OrderManageModel toReturn = _mapper.ToModel<OrderManageModel>(item);

            //TODO - add sign up for newsletter(SignUpNewsletter)

            return toReturn;
        }
    }
}