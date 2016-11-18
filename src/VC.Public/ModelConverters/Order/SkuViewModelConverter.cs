using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VC.Public.Models.Cart;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Business.Helpers;

namespace VC.Public.ModelConverters.Order
{
    public class SkuViewModelConverter : BaseModelConverter<CartSkuModel, SkuDynamic>
    {
        public override Task DynamicToModelAsync(CartSkuModel model, SkuDynamic dynamic)
        {
            model.InStock = dynamic.InStock();
            return TaskCache.CompletedTask;
        }

        public override Task ModelToDynamicAsync(CartSkuModel model, SkuDynamic dynamic)
        {
            return TaskCache.CompletedTask;
        }
    }
}