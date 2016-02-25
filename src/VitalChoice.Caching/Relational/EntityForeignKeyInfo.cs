using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata.Internal;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Relational
{
    public class EntityForeignKeyInfo : EntityValueGroupInfo<EntityValueInfo>, IEquatable<EntityForeignKeyInfo>
    {
        public bool Equals(EntityForeignKeyInfo other)
        {
            return base.Equals(other) && DependentType == other.DependentType;
        }

        public override int GetHashCode()
        {
            return (base.GetHashCode() * 397) ^ DependentType.GetHashCode();
        }

        public EntityForeignKeyInfo(IEnumerable<EntityValueInfo> keyInfos, Type dependentType) : base(keyInfos)
        {
            if (dependentType == null)
                throw new ArgumentNullException(nameof(dependentType));

            DependentType = dependentType;
        }

        public Type DependentType { get; }
    }

    public class EntityForeignKeyCollectionInfo : EntityForeignKeyInfo, IEquatable<EntityForeignKeyCollectionInfo>
    {
        public bool Equals(EntityForeignKeyCollectionInfo other)
        {
            return base.Equals(other);
        }

        public EntityForeignKeyCollectionInfo(string name, IClrPropertyGetter propertyGetter, Type dependentType)
            : base(Enumerable.Repeat(new EntityValueInfo(name, propertyGetter, dependentType), 1), dependentType)
        {
        }
    }
}