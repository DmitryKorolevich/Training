using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Delegates;

namespace VitalChoice.DynamicData
{
    public struct DynamicTypeCache
    {
        public Dictionary<string, GenericProperty> GenericProperties;
        public Type MapTo;
    }
}
