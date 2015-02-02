using System;
using System.Linq;
using System.Reflection;
using Newtonsoft;
using Newtonsoft.Json;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Helpers;

namespace VitalChoice.Validation.Converters
{
    public class BasicEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var type = value.GetType();
            var enumType = type.GetTypeInfo().IsEnum ? type : Nullable.GetUnderlyingType(type);
            Type underlyingType = enumType.GetTypeInfo().GetCustomAttributes(typeof(SerializeAsCharAttribute), false).Any() ? typeof(char) : enumType.GetTypeInfo().UnderlyingSystemType;//TODO: TEST and compare with GetEnumUnderlyingType()

            switch (underlyingType.GetTypeCode())
            {
                case TypeCode.Char:
                    serializer.Serialize(writer, (char)(int)value);
                    break;
                case TypeCode.SByte:
                    serializer.Serialize(writer, (sbyte)value);
                    break;
                case TypeCode.Byte:
                    serializer.Serialize(writer, (byte)value);
                    break;
                case TypeCode.Int16:
                    serializer.Serialize(writer, (short)value);
                    break;
                case TypeCode.UInt16:
                    serializer.Serialize(writer, (ushort)value);
                    break;
                case TypeCode.Int32:
                    serializer.Serialize(writer, (int)value);
                    break;
                case TypeCode.UInt32:
                    serializer.Serialize(writer, (uint)value);
                    break;
                case TypeCode.Int64:
                    serializer.Serialize(writer, (long)value);
                    break;
                case TypeCode.UInt64:
                    serializer.Serialize(writer, (ulong)value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            bool nullable = !objectType.GetTypeInfo().IsEnum;
            var enumType = nullable ? Nullable.GetUnderlyingType(objectType) : objectType;
            Type underlyingType = enumType.GetTypeInfo().GetCustomAttributes(typeof(SerializeAsCharAttribute), false).Any() ? typeof(char) : enumType.UnderlyingSystemType;
            Type readAs = nullable ? typeof(Nullable<>).MakeGenericType(underlyingType) : underlyingType;
            var readedData = serializer.Deserialize(reader, readAs);
            return readedData == null ? null : Enum.Parse(enumType, Convert.ChangeType(readedData, enumType.GetTypeInfo().GetEnumUnderlyingType()).ToString());
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetTypeInfo().IsEnum ||
                   objectType.GetTypeInfo().IsValueType && objectType.GetTypeInfo().IsGenericType && objectType.GetGenericTypeDefinition() == typeof(Nullable<>) &&
                   Nullable.GetUnderlyingType(objectType).GetTypeInfo().IsEnum;
        }
    }
}