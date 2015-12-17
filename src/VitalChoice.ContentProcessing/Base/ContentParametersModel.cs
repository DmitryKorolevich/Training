using VitalChoice.DynamicData.TypeConverters;
using VitalChoice.Ecommerce.Domain.Attributes;

namespace VitalChoice.ContentProcessing.Base
{
    public class ContentParametersModel
    {
        public string Url { get; set; }

        //[ConvertWith(typeof(StringToBoolConverter))]
        public bool Preview { get; set; }
    }
}