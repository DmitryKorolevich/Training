using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Attributes;

namespace VitalChoice.DynamicData.Delegates
{
    public struct GenericProperty
    {
        public Type PropertyType { get; set; }
        public Func<object, object> Get { get; set; }
        public Action<object, object> Set { get; set; }
        public MapAttribute Map { get; set; }
        public bool NotLoggedInfo { get; set; }
    }
}