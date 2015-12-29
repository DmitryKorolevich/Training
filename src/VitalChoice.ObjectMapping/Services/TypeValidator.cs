using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.ObjectMapping.Services
{
    public class TypeValidator
    {
        public static void ThrowIfNotValid(Type modelType, Type dynamicType, object value, string propertyName,
            GenericProperty destProperty, bool toModelDirection)
        {
            if (value == null && destProperty.PropertyType.GetTypeInfo().IsValueType && !destProperty.PropertyType.IsImplementGeneric(typeof(Nullable<>)))
            {
                throw new ApiException(
                    $"Value is null while it should be a ValueType {destProperty.PropertyType}.\r\n [{modelType} <-> {dynamicType}]");
            }
            if (value != null && !destProperty.PropertyType.IsInstanceOfType(value))
            {
                throw new ApiException(
                    $"Value {value} of Type [{value.GetType()}] is not assignable to property {propertyName} with Type {destProperty.PropertyType}.\r\n [{modelType} {(toModelDirection ? "<-" : "->")} {dynamicType}]");
            }
        }
    }
}
