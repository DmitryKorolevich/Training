using System.Collections.Generic;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public struct CacheResult<T>
    {
        public CacheResult(T entity, CacheGetResult result)
        {
            Entity = entity;
            Result = result;
        }

        public CacheResult(CachedEntity<T> cached, ICacheStateManager stateManager, bool attach)
        {
            using (cached.Lock())
            {
                var needUpdate = cached.NeedUpdate;
                if (needUpdate)
                {
                    Result = CacheGetResult.Update;
                    Entity = default(T);
                }
                else
                {
                    Result = CacheGetResult.Found;
                    if (attach)
                    {
                        Entity = (T) AttachGraph(cached.Entity, cached.Cache.Relations, cached.Cache.EntityInfo, stateManager);
                    }
                    else
                    {
                        Entity = (T) cached.Entity.DeepCloneItem(cached.Cache.Relations);
                    }
                }
            }
        }

        public T Entity;

        public CacheGetResult Result;

        public static implicit operator T(CacheResult<T> result)
        {
            return result.Entity;
        }

        public static implicit operator CacheResult<T>(CacheGetResult result)
        {
            return new CacheResult<T>(default(T), result);
        }

        private static IEnumerable<object> AttachCollectionGraph(IEnumerable<object> entities, RelationInfo relationInfo, EntityInfo entityInfo,
            ICacheStateManager stateManager)
        {
            if (entities == null)
                yield break;
            foreach (var entity in entities)
            {
                var toTrack = stateManager.GetOrAddTracked(entityInfo, entity);
                foreach (var relation in relationInfo.Relations)
                {
                    var value = relation.GetRelatedObject(entity);
                    if (value != null)
                    {
                        object newValue;
                        if (relation.IsCollection)
                        {
                            newValue =
                                typeof(List<>).CreateGenericCollection(relation.RelationType,
                                    AttachCollectionGraph((IEnumerable<object>) value, relation, relation.EntityInfo, stateManager))
                                    .CollectionObject;
                        }
                        else
                        {
                            newValue = AttachGraph(value, relation, relation.EntityInfo, stateManager);
                        }
                        if (newValue != value)
                        {
                            relation.SetOrUpdateRelatedObject(entity, toTrack);
                        }
                    }
                }
                yield return entity;
            }
        }

        private static object AttachGraph(object entity, RelationInfo relationInfo, EntityInfo entityInfo, ICacheStateManager stateManager)
        {
            if (entity == null)
                return null;
            var item = stateManager.GetOrAddTracked(entityInfo, entity);
            foreach (var relation in relationInfo.Relations)
            {
                var value = relation.GetRelatedObject(entity);
                if (value != null)
                {
                    object newValue;
                    if (relation.IsCollection)
                    {
                        newValue =
                            typeof(List<>).CreateGenericCollection(relation.RelationType,
                                AttachCollectionGraph((IEnumerable<object>) value, relation, relation.EntityInfo, stateManager))
                                .CollectionObject;
                    }
                    else
                    {
                        newValue = AttachGraph(value, relation, relation.EntityInfo, stateManager);
                    }
                    if (newValue != value)
                    {
                        relation.SetOrUpdateRelatedObject(item, newValue);
                    }
                }
            }
            return item;
        }
    }
}