using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Data;

namespace VitalChoice.Caching.Interfaces
{
    internal interface IRelationMapper
    {
        ICollection<RelationInfo> GetRelations(Type entityType);
        void MapUpdateRelations(Type entityType, ICollection<RelationInfo> relations);
    }
}
