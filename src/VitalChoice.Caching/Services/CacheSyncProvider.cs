using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Debuging;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services
{
    public class CacheSyncProvider : ICacheSyncProvider
    {
        protected readonly IInternalEntityCacheFactory CacheFactory;
        protected readonly IEntityInfoStorage KeyStorage;
        protected readonly ICacheServiceScopeFactoryContainer ScopeContainer;
        protected readonly ILogger Logger;

        public CacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage keyStorage, ILoggerFactory loggerFactory,
            ICacheServiceScopeFactoryContainer scopeContainer)
        {
            CacheFactory = cacheFactory;
            KeyStorage = keyStorage;
            ScopeContainer = scopeContainer;
            CacheDebugger.ScopeFactoryContainer = scopeContainer;
            Logger = loggerFactory.CreateLogger<CacheSyncProvider>();
        }

        public virtual void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {
            Logger.LogInfo(l => string.Join("\n", l), syncOperations.ToList());
        }

        public void AcceptChanges(IEnumerable<SyncOperation> syncOperations)
        {
            var syncGroups = syncOperations.Select(op =>
            {
                try
                {
                    return new SyncOp
                    {
                        SyncOperation = op,
                        EntityType = ReflectionHelper.ResolveType(op.Key.EntityType)
                    };
                }
                catch
                {
                    return new SyncOp
                    {
                        SyncOperation = op,
                        EntityType = typeof(object)
                    };
                }
            }).GroupBy(s => s.EntityType);

            foreach (var group in syncGroups)
            {
                var type = group.Key;
                var pkInfo = KeyStorage.GetPrimaryKeyInfo(type);
                if (pkInfo == null)
                    continue;
                var internalCache = CacheFactory.GetCache(type);
                var toAdd = new List<EntityKey>();
                var toAddForeignKeys = new List<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>>();
                foreach (var op in group)
                {
                    try
                    {
                        EntityKey pk;
                        switch (op.SyncOperation.SyncType)
                        {
                            case SyncType.Update:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                internalCache.MarkForUpdateByPrimaryKey(pk, null);
                                break;
                            case SyncType.Delete:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                internalCache.MarkForUpdateByPrimaryKey(pk, null);
                                internalCache.TryRemove(pk);
                                break;
                            case SyncType.Add:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                toAdd.Add(pk);
                                if (op.SyncOperation.ForeignKeys?.Count > 0)
                                {
                                    var foreignValues = new List<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>>();
                                    foreach (var foreignExportable in op.SyncOperation.ForeignKeys)
                                    {
                                        var dependentType = ReflectionHelper.ResolveType(foreignExportable.DependentType);
                                        var info = internalCache.EntityInfo.ForeignKeys.FirstOrDefault(
                                            f =>
                                                f.DependentType == dependentType &&
                                                foreignExportable.Values.Select(v => v.Name)
                                                    .All(n => f.InfoDictionary.ContainsKey(n)));
                                        if (info != null)
                                        {
                                            foreignValues.Add(new KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>(
                                                info, foreignExportable.ToForeignKey(info)));
                                        }
                                    }
                                    toAddForeignKeys.AddRange(foreignValues);
                                }
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.ToString());
                    }
                }
                if (toAdd.Count > 0)
                {
                    internalCache.MarkForAddListByPrimaryKey(toAdd, GetKeyList(toAddForeignKeys), null);
                }
            }
        }

        private IEnumerable<KeyValuePair<EntityForeignKeyInfo, ICollection<EntityForeignKey>>> GetKeyList(
            IEnumerable<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>>
                toAdd)
        {
            foreach (var item in toAdd.GroupBy(v => v.Key))
            {
                var keys = new HashSet<EntityForeignKey>();
                keys.AddRange(item.Select(g => g.Value));
                yield return new KeyValuePair<EntityForeignKeyInfo, ICollection<EntityForeignKey>>(item.Key, keys);
            }
        }

        public virtual void Dispose()
        {
        }

        private struct SyncOp
        {
            public Type EntityType;
            public SyncOperation SyncOperation;
        }
    }
}