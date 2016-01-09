using System;
using Microsoft.Data.Entity.Metadata.Internal;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityKeyInfo : EntityValueInfo, IEquatable<EntityKeyInfo>
    {
        public EntityKeyInfo(string name, IClrPropertyGetter property): base(name, property)
        {
        }

        public bool Equals(EntityKeyInfo other)
        {
            return base.Equals(other);
        }
    }
}