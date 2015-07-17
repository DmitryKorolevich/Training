using System;
using System.Globalization;
using System.Reflection;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Delegates;

namespace VitalChoice.DynamicData.Helpers
{
    public static class MapperTypeConverter
    {
        public static void ThrowIfNotValid(Type modelType, Type dynamicType, object value, string propertyName,
            GenericProperty destProperty, bool toModelDirection)
        {
            if (value == null && !destProperty.PropertyType.IsImplementGeneric(typeof(Nullable<>)))
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

        public static object ConvertTo<TOptionValue, TOptionType>(TOptionValue value, FieldType typeId)
            where TOptionValue: OptionValue<TOptionType> 
            where TOptionType : OptionType
        {
            if (string.IsNullOrEmpty(value.Value) && value.BigValue == null)
                return null;
            return typeId == FieldType.LargeString ? value.BigValue?.Value : ConvertTo(value.Value, typeId);
        }

        public static object ConvertTo(string value, FieldType typeId)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            try
            {
                switch (typeId)
                {
                    case FieldType.String:
                        return value;
                    case FieldType.Bool:
                        return bool.Parse(value);
                    case FieldType.Int:
                        return int.Parse(value, CultureInfo.InvariantCulture);
                    case FieldType.Decimal:
                        return decimal.Parse(value, CultureInfo.InvariantCulture);
                    case FieldType.Double:
                        return double.Parse(value, CultureInfo.InvariantCulture);
                    case FieldType.DateTime:
                        return DateTime.Parse(value, CultureInfo.InvariantCulture);
                    case FieldType.Int64:
                        return long.Parse(value, CultureInfo.InvariantCulture);
                    case FieldType.LargeString:
                        return value;
                    default:
                        throw new NotImplementedException($"Type conversion for Type:{typeId} is not implemented");
                }
            }
            catch (Exception e) when(!(e is NotImplementedException))
            {
                throw new ObjectConvertException($"\"{value}\" Cannot be converted to Type:{typeId}", e);
            }
        }

        public static string ConvertToOptionValue(object value, FieldType typeId)
        {
            switch (typeId)
            {
                case FieldType.String:
                case FieldType.LargeString:
                    return value as string;
                default:
                    var valueType = value.GetType();
                    var underlyingType = valueType.UnwrapNullable();
                    var enumType = underlyingType.TryUnwrapEnum();
                    if (enumType != null)
                    {
                        return Convert.ToString(Convert.ChangeType(value, enumType),
                            CultureInfo.InvariantCulture);
                    }
                    return Convert.ToString(value, CultureInfo.InvariantCulture);
            }
        }

        public static void ConvertToOption<TOptionValue, TOptionType>(TOptionValue option, object value, FieldType typeId)
            where TOptionValue : OptionValue<TOptionType>
            where TOptionType : OptionType
        {
            switch (typeId)
            {
                case FieldType.String:
                    option.Value = value as string;
                    break;
                case FieldType.LargeString:
                    option.BigValue = new BigStringValue
                    {
                        Value = value as string
                    };
                    break;
                default:
                    var valueType = value.GetType();
                    var underlyingType = valueType.UnwrapNullable();
                    var enumType = underlyingType.TryUnwrapEnum();
                    if (enumType != null)
                    {
                        option.Value = Convert.ToString(Convert.ChangeType(value, enumType),
                            CultureInfo.InvariantCulture);
                        break;
                    }
                    option.Value = Convert.ToString(value, CultureInfo.InvariantCulture);
                    break;
            }
        }
    }
}