using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Attributes;

namespace VitalChoice.DynamicData.Delegates
{
    public struct GenericProperty
    {
        public GenericGetDelegate Get;
        public GenericSetDelegate Set;
        public MapAttribute Map;

    }
}
