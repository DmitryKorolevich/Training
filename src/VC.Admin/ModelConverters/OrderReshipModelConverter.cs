using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Admin.Models.Customer;
using VitalChoice.Business.Queries.Product;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Products;
using VC.Admin.Models.Orders;
using VC.Admin.Models.Products;
using VitalChoice.Business.Queries.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Business.Helpers;
using VitalChoice.Business.Services.Orders;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Interfaces.Services.Orders;

namespace VC.Admin.ModelConverters
{
    public class OrderReshipModelConverter : BaseModelConverter<OrderReshipManageModel, OrderDynamic>
    {
        private readonly OrderService _orderService;

        public OrderReshipModelConverter(OrderService orderService)
        {
            _orderService = orderService;
        }

        public override async Task DynamicToModelAsync(OrderReshipManageModel model, OrderDynamic dynamic)
        {
            model.ReshipProblemSkus = dynamic.ReshipProblemSkus?.Select(p => new ReshipProblemSkuModel()
            {
                IdSku = p.IdSku,
                Code = p.Code,
                Used = true,
            }).ToList();

            model.ReshipProblemSkus?.AddRange(
                dynamic.Skus.Where(p => model.ReshipProblemSkus!=null && !model.ReshipProblemSkus.Select(pp => pp.IdSku).Contains(p.Sku.Id))
                    .Select(p => new ReshipProblemSkuModel()
                    {
                        IdSku = p.Sku.Id,
                        Code = p.Sku.Code,
                        Used = false,
                    }).ToArray());

            if (dynamic.IdOrderSource.HasValue)
            {
                var source = await _orderService.SelectAsync(dynamic.IdOrderSource.Value,false, p => p);
                if (source != null)
                {
                    model.OrderSourceDateCreated = source.DateCreated;
                    model.OrderSourceTotal = source.Total;
                }
            }
        }

        public override Task ModelToDynamicAsync(OrderReshipManageModel model, OrderDynamic dynamic)
        {
            dynamic.ReshipProblemSkus = model.ReshipProblemSkus?.Where(p => p.Used).Select(p => new ReshipProblemSkuOrdered()
                {
                    IdSku = p.IdSku,
                }).ToList();
            return TaskCache.CompletedTask;
        }
    }
}