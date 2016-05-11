using System;
using System.Threading.Tasks;
using VC.Public.Models.Cart;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Order
{
    public class SkuViewModelConverter : BaseModelConverter<CartSkuModel, SkuDynamic>
    {
        public override Task DynamicToModelAsync(CartSkuModel model, SkuDynamic dynamic)
        {
            model.InStock = dynamic.IdObjectType == (int) ProductType.EGс || dynamic.IdObjectType == (int) ProductType.Gc ||
                            ((bool?) dynamic.SafeData.DisregardStock ?? false) || ((int?) dynamic.SafeData.Stock ?? 0) > 0;
            return Task.Delay(0);
        }

        public override Task ModelToDynamicAsync(CartSkuModel model, SkuDynamic dynamic)
        {
            return Task.Delay(0);
        }
    }
}