using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Indexes;

namespace VitalChoice.Caching.Relational
{
    internal struct EntityInfo
    {
        public EntityPrimaryKeyInfo PrimaryKey;
        public EntityUniqueIndexInfo UniqueIndex;
        public ConditionalIndexInfo[] ConditionalIndexes;
    }
}
