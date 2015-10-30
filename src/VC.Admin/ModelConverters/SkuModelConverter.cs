using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

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