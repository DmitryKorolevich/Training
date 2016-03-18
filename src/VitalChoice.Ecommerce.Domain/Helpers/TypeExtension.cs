using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class TypeExtension
    {
        public static bool IsType(this Type typeToCheck, Type type)
        {
            if (typeToCheck == null)
                throw new ArgumentNullException(nameof(typeToCheck));

            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return typeToCheck.IsAssignableFrom(type);
        }

        public static bool IsType(this Type typeToCheck, object data)
        {
            if (typeToCheck == null)
                throw new ArgumentNullException(nameof(typeToCheck));

            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return typeToCheck.IsInstanceOfType(data);
        }
        
        public static bool IsType<T>(this Type typeToCheck)
        {
            if (typeToCheck == null)
                throw new ArgumentNullException(nameof(typeToCheck));

            return typeToCheck.IsAssignableFrom(typeof(T));
        }

        public static bool IsImplement(this Type type, Type interfaceType)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (interfaceType == null)
                throw new ArgumentNullException(nameof(interfaceType));

            return type.GetInterfaces().Any(i => i == interfaceType);
        }

        public static bool IsImplement<T>(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetInterfaces().Any(i => i == typeof(T));
        }

        public static bool IsHaveAttribute(this Type type, Type attributeType, bool inherit = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (attributeType == null)
                throw new ArgumentNullException(nameof(attributeType));

            return type.GetTypeInfo().GetCustomAttributes(inherit).Any(a => a.GetType() == attributeType);
        }

        public static bool IsHaveAttribute<T>(this Type type, bool inherit = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetTypeInfo().GetCustomAttributes(inherit).Any(a => a is T);
        }

        public static T[] GetAttributes<T>(this Type type, bool inherit = false)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            return type.GetTypeInfo().GetCustomAttributes(inherit).Where(a => a is T).Cast<T>().ToArray();
        }

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

        public static TypeCode GetTypeCode(this Type type)
        {
            if (type == null)
                return TypeCode.Empty;
            if (type == typeof(bool))
                return TypeCode.Boolean;
            if (type == typeof(char))
                return TypeCode.Char;
            if (type == typeof(sbyte))
                return TypeCode.SByte;
            if (type == typeof(byte))
                return TypeCode.Byte;
            if (type == typeof(short))
                return TypeCode.Int16;
            if (type == typeof(ushort))
                return TypeCode.UInt16;
            if (type == typeof(int))
                return TypeCode.Int32;
            if (type == typeof(uint))
                return TypeCode.UInt32;
            if (type == typeof(long))
                return TypeCode.Int64;
            if (type == typeof(ulong))
                return TypeCode.UInt64;
            if (type == typeof(float))
                return TypeCode.Single;
            if (type == typeof(double))
                return TypeCode.Double;
            if (type == typeof(decimal))
                return TypeCode.Decimal;
            if (type == typeof(DateTime))
                return TypeCode.DateTime;
            if (type == typeof(string))
                return TypeCode.String;
            if (type.GetTypeInfo().IsEnum)
                return GetTypeCode(Enum.GetUnderlyingType(type));
            return TypeCode.Object;
        }

        public static Type UnwrapNullable(this Type type)
        {
            return type.IsImplementGeneric(typeof (Nullable<>))
                ? Nullable.GetUnderlyingType(type)
                : type;
        }

        public static Type TryUnwrapEnum(this Type type)
        {
            if (type.GetTypeInfo().IsEnum)
            {
                return Enum.GetUnderlyingType(type);
            }
            return null;
        }

        public static Type[] TryGetTypeArguments(this Type type, Type baseType)
        {
            if (type == null)
                return null;

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericTypeDefinition)
                return null;

            if (typeInfo.IsGenericType && type.GetGenericTypeDefinition() == baseType)
                return type.GenericTypeArguments;

            if (baseType.GetTypeInfo().IsInterface)
            {
                var implementation =
                    typeInfo.ImplementedInterfaces.FirstOrDefault(
                        t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == baseType);
                return implementation?.GenericTypeArguments;
            }
            var baseImplementation =
                type.GetBaseTypes()
                    .FirstOrDefault(
                        t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == baseType);
            return baseImplementation?.GenericTypeArguments;
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
