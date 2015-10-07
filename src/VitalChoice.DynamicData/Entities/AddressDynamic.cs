using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public abstract class AddressDynamic : MappedObject
    {
        public int IdCountry { get; set; }

        public string County { get; set; }

        public int? IdState { get; set; }
    }
}