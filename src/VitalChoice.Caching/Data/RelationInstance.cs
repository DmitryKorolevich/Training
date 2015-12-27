using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Caching.Services;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Data
{
    public struct RelationInstance
    {
        public object RelatedObject { get; }
        public object ParentEntity { get; }
        public Type RelationObjectType { get; }

        public ICollection<RelationInstance> SubRelationInstances { get; }

        public RelationInstance(object parentEntity, object relatedObject, Type relationObjectType, ICollection<RelationInstance> relationInstances)
        {
            ParentEntity = parentEntity;
            RelatedObject = relatedObject;
            RelationObjectType = relationObjectType;
            SubRelationInstances = relationInstances;
        }
    }
}