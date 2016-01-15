using System.Runtime.Serialization;

namespace VitalChoice.Caching.Relational.Base
{
    [DataContract]
    public class EntityValueExportable
    {
        [DataMember]
        public object Value { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}