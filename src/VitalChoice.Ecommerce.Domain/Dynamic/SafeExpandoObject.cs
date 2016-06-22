using System;
using System.Collections.Generic;
using System.Dynamic;

namespace VitalChoice.Ecommerce.Domain.Dynamic
{
    public class UnsafeDynamicObject : DynamicObject
    {
        public Dictionary<string, object> Dictionary { get; }

        public UnsafeDynamicObject()
        {
            Dictionary = new Dictionary<string, object>();
        }

        public UnsafeDynamicObject(Dictionary<string, object> dict)
        {
            Dictionary = dict;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return Dictionary.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            if (Dictionary.ContainsKey(binder.Name))
            {
                Dictionary[binder.Name] = value;
            }
            else
            {
                Dictionary.Add(binder.Name, value);
            }
            return true;
        }
    }

    public sealed class SafeDynamicObject : UnsafeDynamicObject
    {
        public SafeDynamicObject(UnsafeDynamicObject obj) : base(obj.Dictionary)
        {
        }

        public SafeDynamicObject()
        {
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            Dictionary.TryGetValue(binder.Name, out result);
            return true;
        }
    }
}