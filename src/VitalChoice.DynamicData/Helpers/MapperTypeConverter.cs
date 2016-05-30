using System;
using System.Globalization;
using System.Reflection;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.DynamicData.Helpers
{
    public static class MapperTypeConverter
    {
        private static readonly string[] FormatStrings = {
            "yyyy-MM-ddTHH:mm:ss.fffffff", "yyyy-MM-ddTHH:mm:ss.ffffff", "yyyy-MM-ddTHH:mm:ss.fffff",
            "yyyy-MM-ddTHH:mm:ss.ffff", "yyyy-MM-ddTHH:mm:ss.fff", "yyyy-MM-ddTHH:mm:ss.ff", "yyyy-MM-ddTHH:mm:ss.f",
            "yyyy-MM-ddTHH:mm:ss"
        };

        public static object ConvertTo<TOptionValue, TOptionType>(TOptionValue value, FieldType typeId)
            where TOptionValue : OptionValue<TOptionType>
            where TOptionType : OptionType
        {
            if (string.IsNullOrEmpty(value.Value) && value.BigValue == null)
                return null;
            if (typeId == FieldType.LargeString)
            {
                if (value.BigValue != null)
                    return value.BigValue.Value;
                return value.Value;
            }
            return ConvertTo(value.Value, typeId);
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
                        return DateTime.ParseExact(value, FormatStrings, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal);
                    case FieldType.Int64:
                        return long.Parse(value, CultureInfo.InvariantCulture);
                    case FieldType.LargeString:
                        return value;
                    default:
                        throw new NotImplementedException($"Type conversion for Type:{typeId} is not implemented");
                }
            }
            catch (Exception e) when (!(e is NotImplementedException))
            {
                throw new ObjectConvertException($"\"{value}\" Cannot be converted to Type:{typeId}", e);
            }
        }

        public static string ConvertToOptionValue(object value, FieldType typeId)
        {
            if (value == null)
                return null;
            switch (typeId)
            {
                case FieldType.String:
                case FieldType.LargeString:
                    return value as string;
                case FieldType.DateTime:
                    var datetime = (DateTime)value;
                    return TrimZeros(datetime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
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
                    var str = value as string;
                    if (str != null && str.Length > 250)
                    {
                        if (option.BigValue == null)
                        {
                            option.BigValue = new BigStringValue
                            {
                                Value = str
                            };
                        }
                        else
                        {
                            option.BigValue.Value = str;
                        }
                        option.Value = null;
                    }
                    else
                    {
                        option.BigValue = null;
                        option.IdBigString = null;
                        option.Value = str;
                    }
                    break;
                case FieldType.DateTime:
                    var datetime = (DateTime) value;
                    option.Value = TrimZeros(datetime.ToString("yyyy-MM-ddTHH:mm:ss.fffffff"));
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

        private static string TrimZeros(string value)
        {
            if (value.Contains("."))
            {
                int seed = value.Length - 1;
                while (value[seed] == '0')
                {
                    seed--;
                }
                if (value[seed] == '.')
                {
                    return value.Substring(0, seed);
                }
                return value.Substring(0, seed + 1);
            }
            return value;
        }
    }
}