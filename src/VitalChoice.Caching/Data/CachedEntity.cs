using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Data
{
    internal class CachedEntity<TEntity>
        where TEntity : Entity
    {
        public CachedEntity(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; }
        public EntityPrimaryKey PrimaryKey { get; set; }

        public static implicit operator TEntity(CachedEntity<TEntity> cached)
        {
            return cached?.Entity;
        }
    }
}