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

        static MethodInfoData()
        {
            Select = typeof(Enumerable).GetMethods()
                .Single(
                    m =>
                        m.Name == "Select" && m.GetParameters().Length == 2 &&
                        m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));

            FirstOrDefault = typeof(Enumerable).GetMethods()
                .Single(m => m.Name == "FirstOrDefault" && m.GetParameters().Length == 1);

            Where = typeof(Enumerable).GetMethods()
                .Single(m => m.Name == "Where" && m.GetParameters().Length == 2 &&
                        m.GetParameters()[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>));

            StringLength = typeof(string).GetProperty("Length");

            Any = typeof(Enumerable).GetMethods()
                .Single(m => m.Name == "Any" && m.GetParameters().Length == 2);

            All = typeof(Enumerable).GetMethods()
                .Single(m => m.Name == "All" && m.GetParameters().Length == 2);
        }
    }
}