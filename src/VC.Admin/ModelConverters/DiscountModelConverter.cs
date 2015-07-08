using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VC.Admin.Models.Product;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class DiscountModelConverter : IModelToDynamicConverter<DiscountManageModel, DiscountDynamic>
    {
        public void DynamicToModel(DiscountManageModel model, DiscountDynamic dynamic)
        {
            model.ExpirationDate = model.ExpirationDate?.AddDays(-1);
        }

        public void ModelToDynamic(DiscountManageModel model, DiscountDynamic dynamic)
        {
            if (dynamic.StartDate.HasValue)
            {
                dynamic.StartDate = new DateTime(dynamic.StartDate.Value.Year, dynamic.StartDate.Value.Month, dynamic.StartDate.Value.Day);
            }
            if (dynamic.ExpirationDate != null)
            {
                dynamic.ExpirationDate = (new DateTime(dynamic.ExpirationDate.Value.Year, dynamic.ExpirationDate.Value.Month, dynamic.ExpirationDate.Value.Day)).AddDays(1);
            }
        }
    }
}
