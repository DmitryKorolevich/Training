using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Data.Extensions;

namespace VitalChoice.Caching.Extensions
{
    internal static class KeyInfoExtensions
    {
        public static T[] Normalize<T>(this IEnumerable<T> infos)
            where T : EntityValueInfo
        {
            var result = infos.OrderBy(item => item.Name, StringComparer.Ordinal).ToArray();
            for (var index = 0; index < result.Length; index++)
            {
                result[index].ItemIndex = index;
            }
            return result;
        }

        public static void NormalizeUpdate<T>(this IEnumerable<T> infos)
            where T : EntityValueInfo
        {
            int index = 0;
            infos.OrderBy(item => item.Name, StringComparer.Ordinal).ForEach(item =>
            {
                item.ItemIndex = index;
                index++;
            });
        }

        public static void NormalizeUpdate<T>(this IReadOnlyCollection<KeyValuePair<T, T>> infos)
            where T : EntityValueInfo
        {
            int keyIndex = 0;
            infos.OrderBy(item => item.Key.Name, StringComparer.Ordinal).ForEach(item =>
            {
                item.Key.ItemIndex = keyIndex;
                keyIndex++;
            });
            var valueIndex = 0;
            infos.OrderBy(item => item.Value.Name, StringComparer.Ordinal).ForEach(item =>
            {
                item.Value.ItemIndex = valueIndex;
                valueIndex++;
            });
        }
    }
}