using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Public.Models.Cart;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Checkout
{
    public class SkuToCartSkuConverter : BaseModelConverter<CartSkuModel, SkuDynamic>
    {
        public override void DynamicToModel(CartSkuModel model, SkuDynamic dynamic)
        {
            model.InStock = ((bool?) dynamic.SafeData.DisregardStock ?? false) || ((int?) dynamic.SafeData.Stock ?? 0) > 0;
        }

        public override void ModelToDynamic(CartSkuModel model, SkuDynamic dynamic)
        {
            throw new NotImplementedException();
        }
    }
}
