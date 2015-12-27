using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata;
using VitalChoice.Caching.Services;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Data
{
    internal struct CachedEntity<T>
        where T : Entity
    {
        public CachedEntity(T entity, ICollection<RelationInstance> relations, ICollection<RelationInfo> relationsInfo)
        {
            Relations = relations;
            RelationsInfo = relationsInfo;
            Entity = entity;
        }

        public T Entity { get; }
        public ICollection<RelationInstance> Relations { get; }
        public ICollection<RelationInfo> RelationsInfo { get; }

        public static implicit operator T(CachedEntity<T> cached)
        {
            return cached.Entity;
        }
    }
}