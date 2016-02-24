using VC.Admin.Models.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VC.Admin.ModelConverters
{
    public class SkuModelConverter : BaseModelConverter<SKUManageModel, SkuDynamic>
    {
        public override void DynamicToModel(SKUManageModel model, SkuDynamic dynamic)
        {
            model.Active = dynamic.StatusCode == (int)RecordStatusCode.Active;
        }

        public override void ModelToDynamic(SKUManageModel model, SkuDynamic dynamic)
        {
            dynamic.StatusCode = model.Active ? (int)RecordStatusCode.Active : (int)RecordStatusCode.NotActive;
            dynamic.Code = dynamic?.Code.Trim();
            if (!dynamic.Data.AutoShipProduct)
            {
                dynamic.DictionaryData.Remove("AutoShipFrequency1");
                dynamic.DictionaryData.Remove("AutoShipFrequency2");
                dynamic.DictionaryData.Remove("AutoShipFrequency3");
                dynamic.DictionaryData.Remove("AutoShipFrequency6");
                dynamic.DictionaryData.Remove("OffPercent");
            }
        }
    }
}