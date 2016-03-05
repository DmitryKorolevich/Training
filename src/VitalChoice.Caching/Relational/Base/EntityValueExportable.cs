using System;
using System.Runtime.Serialization;

namespace VitalChoice.Caching.Relational.Base
{
    [DataContract]
    public class EntityValueExportable
    {
        public EntityValueExportable(object value, string name)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            Value = value;
            Name = name;
        }

        [DataMember]
        public object Value { get; set; }

        [DataMember]
        public string Name { get; set; }

        public override string ToString()
        {
            return $"({Name}: {Value})";
        }
    }
}