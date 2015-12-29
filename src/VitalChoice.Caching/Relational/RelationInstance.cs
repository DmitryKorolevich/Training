using System;
using System.Collections.Generic;

namespace VitalChoice.Caching.Relational
{
    public struct RelationInstance
    {
        public RelationInfo RelationInfo { get; }
        public object RelatedObject { get; }
        public object ParentEntity { get; }
        public Type RelationObjectType { get; }

        public ICollection<RelationInstance> SubRelationInstances { get; }

        public RelationInstance(object parentEntity, object relatedObject, Type relationObjectType, ICollection<RelationInstance> relationInstances, RelationInfo relationInfo)
        {
            RelationInfo = relationInfo;
            ParentEntity = parentEntity;
            RelatedObject = relatedObject;
            RelationObjectType = relationObjectType;
            SubRelationInstances = relationInstances;
        }
    }
}