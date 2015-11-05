using System;
using System.Collections.Generic;
using System.Text;

namespace VitalChoice.Domain.Helpers
{
    internal static class Utility
    {
        public static string ToConcatenatedString<T>(this IEnumerable<T> source, Func<T, string> selector, string separator)
        {
            var result = new StringBuilder();
            bool needSeparator = false;

            foreach (var item in source)
            {
                if (needSeparator)
                    result.Append(separator);

                result.Append(selector(item));
                needSeparator = true;
            }

            return result.ToString();
        }

        public static LinkedList<T> ToLinkedList<T>(this IEnumerable<T> source)
        {
            return new LinkedList<T>(source);
        }
    }
}