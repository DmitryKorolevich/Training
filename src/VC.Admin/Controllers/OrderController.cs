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
using VitalChoice.Interfaces.Services.Order;
using VC.Admin.Models.Order;

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
                    OrderStatus=VitalChoice.Domain.Entities.eCommerce.Orders.OrderStatus.Processed,
                    DateCreated=DateTime.Now,
                    IdOrderSource=1,
                    IdPaymentMethod=3,
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
                    OrderStatus=VitalChoice.Domain.Entities.eCommerce.Orders.OrderStatus.Shipped,
                    DateCreated=DateTime.Now,
                    IdOrderSource=2,
                    IdPaymentMethod=5,
                },
            };
            toReturn.Count = 2;

            return toReturn;
        }
    }
}