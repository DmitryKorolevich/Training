using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace VitalChoice.DynamicData.Helpers
{
    public static class TypeExtension
    {
        public static Type TryGetElementType(this Type type, Type baseType)
        {
            if (type == null)
                return null;

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericTypeDefinition)
                return null;

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                return type.GenericTypeArguments.FirstOrDefault();

            if (baseType.GetTypeInfo().IsInterface)
            {
                var implementation =
                    typeInfo.ImplementedInterfaces.FirstOrDefault(
                        t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == baseType);
                return implementation?.GenericTypeArguments.FirstOrDefault();
            }
            var baseImplementation =
                type.GetBaseTypes()
                    .FirstOrDefault(
                        t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == baseType);
            return baseImplementation?.GenericTypeArguments.FirstOrDefault();
        }

        public static bool IsImplementGeneric(this Type type, Type baseType)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericTypeDefinition)
                return false;

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                return true;

            if (baseType.GetTypeInfo().IsInterface)
            {
                return typeInfo.ImplementedInterfaces.Any(
                    t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == baseType);
            }
            return type.GetBaseTypes().Any(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == baseType);
        }

        public static IEnumerable<Type> GetBaseTypes(this Type type)
        {
            type = type.GetTypeInfo().BaseType;

            while (type != null)
            {
                yield return type;

                type = type.GetTypeInfo().BaseType;
            }
        }
    }
}
