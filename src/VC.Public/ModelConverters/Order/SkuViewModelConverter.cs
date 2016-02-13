using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Public.Models.Cart;
using VitalChoice.DynamicData.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Public.ModelConverters.Checkout
{
    public class SkuViewModelConverter : BaseModelConverter<CartSkuModel, SkuDynamic>
    {
        public override void DynamicToModel(CartSkuModel model, SkuDynamic dynamic)
        {
            model.DisplayName = dynamic.SafeData.DisplayName ?? String.Empty;
            if (dynamic.SafeData.SubTitle != null)
            {
                model.DisplayName += " " + dynamic.SafeData.SubTitle;
            }
            if (dynamic.SafeData.QTY != null)
            {
                model.DisplayName +=$" ({dynamic.SafeData.QTY})";
            }
        }

        public override void ModelToDynamic(CartSkuModel model, SkuDynamic dynamic)
        {
            throw new NotImplementedException();
        }
    }
}
