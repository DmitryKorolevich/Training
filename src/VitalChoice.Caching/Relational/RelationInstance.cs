using System;
using System.Collections.Generic;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache;

namespace VitalChoice.Caching.Relational
{
    public struct RelationInstance
    {
        public readonly IInternalEntityCache CacheContainer;

        public readonly RelationInfo RelationInfo;
        public readonly CachedEntity RelatedObject;
        public readonly Type RelationObjectType;

        public RelationInstance(CachedEntity cachedEntity, Type relationObjectType, RelationInfo relationInfo,
            IInternalEntityCache cacheContainer)
        {
            RelationInfo = relationInfo;
            CacheContainer = cacheContainer;
            RelatedObject = cachedEntity;
            RelationObjectType = relationObjectType;
        }
    }
}