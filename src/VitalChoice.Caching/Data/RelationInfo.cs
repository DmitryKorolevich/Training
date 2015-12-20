using System;
using System.Collections.Generic;

namespace VitalChoice.Caching.Data
{
    internal class RelationInfo
    {
        public RelationInfo(Type entityType)
        {
            EntityType = entityType;
            Relations = new List<RelationInfo>();
        }

        public Type EntityType { get; }
        public List<RelationInfo> Relations { get; }
    }
}