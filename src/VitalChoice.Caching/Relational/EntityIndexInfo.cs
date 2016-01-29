using System;
using Microsoft.Data.Entity.Metadata.Internal;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityIndexInfo : EntityValueInfo, IEquatable<EntityIndexInfo>
    {
        public EntityIndexInfo(string name, IClrPropertyGetter property, Type propertyType): base(name, property, propertyType)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
        }

        public bool Equals(EntityIndexInfo other)
        {
            return base.Equals(other);
        }
    }
}
