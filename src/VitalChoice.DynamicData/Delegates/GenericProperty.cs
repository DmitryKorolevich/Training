using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Helpers;

namespace VitalChoice.DynamicData.Delegates
{
    public struct GenericProperty
    {
        public Type PropertyType;
        public Func<object, object> Get;
        public VoidFunc<object, object> Set;
        public MapAttribute Map;

    }
}
