using System.Collections.Generic;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    internal struct EntityInfo
    {
        public EntityPrimaryKeyInfo PrimaryKey;
        public EntityUniqueIndexInfo UniqueIndex;
        public ICollection<EntityConditionalIndexInfo> ConditionalIndexes;
    }
}
