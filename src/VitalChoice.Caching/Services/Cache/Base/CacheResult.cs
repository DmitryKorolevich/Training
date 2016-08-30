using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Extensions;

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
                        Entity = (T) GetAttachedOrClone(cached.Entity, cached.Cache.Relations, cached.Cache.EntityInfo, stateManager);
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

        private static IEnumerable<object> GetAttachedOrCloneCollection(IEnumerable<object> entities, RelationInfo relationInfo,
            EntityInfo entityInfo,
            ICacheStateManager stateManager)
        {
            if (entities == null)
                yield break;
            foreach (var entity in entities)
            {
                bool cloned;
                var toTrack = stateManager.GetOrAddTracked(entityInfo, entity, out cloned);
                foreach (var relation in relationInfo.Relations)
                {
                    var value = relation.GetRelatedObject(entity);
                    if (value != null)
                    {
                        object newValue;
                        if (relation.IsCollection)
                        {
                            if (cloned)
                            {
                                if (!relation.CanSet)
                                {
                                    relation.SetOrUpdateRelatedObject(toTrack,
                                        GetAttachedOrCloneCollection((IEnumerable<object>)value, relation, relation.EntityInfo, stateManager));
                                }
                                else
                                {
                                    newValue =
                                        typeof(List<>).CreateGenericCollection(relation.RelationType,
                                                GetAttachedOrCloneCollection((IEnumerable<object>)value, relation, relation.EntityInfo,
                                                    stateManager))
                                            .CollectionObject;
                                    relation.SetOrUpdateRelatedObject(toTrack, newValue);
                                }
                            }
                            else
                            {
                                HashSet<object> itemList = new HashSet<object>((IEnumerable<object>)relation.GetRelatedObject(toTrack));
                                foreach (
                                    var newItemToTrack in
                                    GetAttachedOrCloneCollection((IEnumerable<object>)value, relation, relation.EntityInfo,
                                        stateManager))
                                {
                                    if (!itemList.Contains(newItemToTrack))
                                    {
                                        relation.AddItemToCollection(toTrack, newItemToTrack);
                                    }
                                }
                            }
                        }
                        else
                        {
                            newValue = GetAttachedOrClone(value, relation, relation.EntityInfo, stateManager);
                            if (newValue != relation.GetRelatedObject(toTrack))
                            {
                                relation.SetOrUpdateRelatedObject(toTrack, newValue);
                            }
                        }
                    }
                }
                yield return toTrack;
            }
        }

        private static object GetAttachedOrClone(object entity, RelationInfo relationInfo, EntityInfo entityInfo,
            ICacheStateManager stateManager)
        {
            if (entity == null)
                return null;
            bool cloned;
            var toTrack = stateManager.GetOrAddTracked(entityInfo, entity, out cloned);
            foreach (var relation in relationInfo.Relations)
            {
                var value = relation.GetRelatedObject(entity);
                if (value != null)
                {
                    object newValue;
                    if (relation.IsCollection)
                    {
                        if (cloned)
                        {
                            if (!relation.CanSet)
                            {
                                relation.SetOrUpdateRelatedObject(toTrack,
                                    GetAttachedOrCloneCollection((IEnumerable<object>) value, relation, relation.EntityInfo, stateManager));
                            }
                            else
                            {
                                newValue =
                                    typeof(List<>).CreateGenericCollection(relation.RelationType,
                                            GetAttachedOrCloneCollection((IEnumerable<object>) value, relation, relation.EntityInfo,
                                                stateManager))
                                        .CollectionObject;
                                relation.SetOrUpdateRelatedObject(toTrack, newValue);
                            }
                        }
                        else
                        {
                            HashSet<object> itemList = new HashSet<object>((IEnumerable<object>) relation.GetRelatedObject(toTrack));
                            foreach (
                                var newItemToTrack in
                                GetAttachedOrCloneCollection((IEnumerable<object>) value, relation, relation.EntityInfo,
                                    stateManager))
                            {
                                if (!itemList.Contains(newItemToTrack))
                                {
                                    relation.AddItemToCollection(toTrack, newItemToTrack);
                                }
                            }
                        }
                    }
                    else
                    {
                        newValue = GetAttachedOrClone(value, relation, relation.EntityInfo, stateManager);
                        if (newValue != relation.GetRelatedObject(toTrack))
                        {
                            relation.SetOrUpdateRelatedObject(toTrack, newValue);
                        }
                    }
                }
            }
            return toTrack;
        }
    }
}