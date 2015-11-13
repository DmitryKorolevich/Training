using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Attributes;

namespace VitalChoice.DynamicData.Delegates
{
    public struct GenericProperty
    {
        private Type _propertyType;

        public Type PropertyType
        {
            get { return _propertyType; }
            set
            {
                _propertyType = value;
                PropertyTypeInfo = value.GetTypeInfo();
            }
        }
        public TypeInfo PropertyTypeInfo { get; private set; }
        public Func<object, object> Get { get; set; }
        public Action<object, object> Set { get; set; }
        public MapAttribute Map { get; set; }
        public bool NotLoggedInfo { get; set; }
    }
}