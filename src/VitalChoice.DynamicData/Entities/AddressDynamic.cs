using VitalChoice.Domain.Entities.Settings;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public class AddressDynamic : MappedObject
    {
        public Country Country { get; set; }

        public State State { get; set; }

        public int IdCountry { get; set; }

        public string County { get; set; }

        public int? IdState { get; set; }
    }
}