using System;
using System.Linq;
using System.Reflection;

namespace VitalChoice.DynamicData.Extensions
{
    public class MethodInfoData
    {
        public static readonly MethodInfo Select;
        public static readonly MethodInfo FirstOrDefault;
        public static readonly MethodInfo Where;
        public static readonly PropertyInfo StringLength;
        public static readonly MethodInfo Any;
        public static readonly MethodInfo All;
        public static readonly MethodInfo ConvertToInt32;
        public static readonly MethodInfo ConvertToInt64;
        public static readonly MethodInfo ConvertToDateTime;
        public static readonly MethodInfo ConvertToBoolean;
        public static readonly MethodInfo ConvertToDecimal;
        public static readonly MethodInfo ConvertToDouble;

        static MethodInfoData()
        {
            Select = typeof(Enumerable).GetMethods()
                .Single(
                    m =>
                        m.Name == nameof(Select) && m.GetParameters().Length == 2 &&
                        m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));

            FirstOrDefault = typeof(Enumerable).GetMethods()
                .Single(m => m.Name == nameof(FirstOrDefault) && m.GetParameters().Length == 1);

            Where = typeof(Enumerable).GetMethods()
                .Single(m => m.Name == nameof(Where) && m.GetParameters().Length == 2 &&
                             m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));

            StringLength = typeof(string).GetProperty("Length");

            Any = typeof(Enumerable).GetMethods()
                .Single(m => m.Name == nameof(Any) && m.GetParameters().Length == 2);

            All = typeof(Enumerable).GetMethods()
                .Single(m => m.Name == nameof(All) && m.GetParameters().Length == 2);

            ConvertToInt32 = typeof(Convert).GetMethod("ToInt32", new[] {typeof(string)});

            ConvertToInt64 = typeof(Convert).GetMethod("ToInt64", new[] {typeof(string)});

            ConvertToDateTime = typeof(Convert).GetMethod("ToDateTime", new[] {typeof(string)});

            ConvertToBoolean = typeof(Convert).GetMethod("ToBoolean", new[] {typeof(string)});

            ConvertToDecimal = typeof(Convert).GetMethod("ToDecimal", new[] {typeof(string)});

            ConvertToDouble = typeof(Convert).GetMethod("ToDouble", new[] {typeof(string)});
        }

        public static MethodInfo GetStringConversionMethod(Type destinationType)
        {
            if (destinationType == typeof(int))
            {
                return ConvertToInt32;
            }
            if (destinationType == typeof(long))
            {
                return ConvertToInt64;
            }
            if (destinationType == typeof(decimal))
            {
                return ConvertToDecimal;
            }
            if (destinationType == typeof(double))
            {
                return ConvertToDouble;
            }
            if (destinationType == typeof(bool))
            {
                return ConvertToBoolean;
            }
            if (destinationType == typeof(DateTime))
            {
                return ConvertToDateTime;
            }
            throw new NotSupportedException($"Type conversion from {typeof(string)} to {destinationType} is not supported.");
        }
    }
}