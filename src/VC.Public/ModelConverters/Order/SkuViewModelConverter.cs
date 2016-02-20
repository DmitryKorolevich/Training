using System;
using VC.Public.Models.Cart;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Order
{
    public class SkuViewModelConverter : BaseModelConverter<CartSkuModel, SkuDynamic>
    {
        public override void DynamicToModel(CartSkuModel model, SkuDynamic dynamic)
        {
            model.InStock = ((bool?)dynamic.SafeData.DisregardStock ?? false) || ((int?)dynamic.SafeData.Stock ?? 0) > 0;
        }

        public override void ModelToDynamic(CartSkuModel model, SkuDynamic dynamic)
        {
            throw new NotImplementedException();
        }
    }
}