using System;
using System.Reflection;

namespace VitalChoice.Validation.Helpers
{
    public static class ReflectionHelper
    {
        public static TypeCode GetTypeCode(this Type type)
        {
#if ASPNET50
            return Type.GetTypeCode(type);
#else
            throw new NotImplementedException();
#endif
        }
    }
}