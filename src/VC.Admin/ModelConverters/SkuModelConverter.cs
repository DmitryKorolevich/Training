using VC.Admin.Models.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VC.Admin.ModelConverters
{
    public class SkuModelConverter : IModelToDynamic<SKUManageModel, SkuMapped>
    {
        public void DynamicToModel(SKUManageModel model, SkuMapped dynamic)
        {
            model.Active = dynamic.StatusCode == RecordStatusCode.Active;
        }

        public void ModelToDynamic(SKUManageModel model, SkuMapped dynamic)
        {
            dynamic.StatusCode = model.Active ? RecordStatusCode.Active : RecordStatusCode.NotActive;
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