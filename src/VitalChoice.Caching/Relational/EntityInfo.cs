using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Relational
{
    internal struct EntityInfo
    {
        public EntityPrimaryKeyInfo PrimaryKey;
        public EntityUniqueIndexInfo[] UniqueIndexes;
    }
}
