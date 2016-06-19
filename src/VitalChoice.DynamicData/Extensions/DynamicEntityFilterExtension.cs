using System.Collections.Generic;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.DynamicData.Extensions
{
    public static class DynamicEntityFilterExtension
    {
        public static bool WhenValueGreaterThan<TEntity, T>(this TEntity entity, string name, T filter, ValuesFilterType filterType, CompareBehaviour compareBehaviour)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValues<TEntity, T>(this TEntity entity, T filter, ValuesFilterType filterType, CompareBehaviour compareBehaviour)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValues<TEntity, T>(this TEntity entity, T filter, int? idObjectType, ValuesFilterType filterType, CompareBehaviour compareBehaviour)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValuesAny<TEntity, T>(this IEnumerable<TEntity> entity, T filter, ValuesFilterType filterType, CompareBehaviour compareBehaviour)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValuesAny<TEntity, T>(this IEnumerable<TEntity> entity, T filter, int? idObjectType, ValuesFilterType filterType, CompareBehaviour compareBehaviour)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValuesAll<TEntity, T>(this IEnumerable<TEntity> entity, T filter, ValuesFilterType filterType, CompareBehaviour compareBehaviour)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValuesAll<TEntity, T>(this IEnumerable<TEntity> entity, T filter, int? idObjectType, ValuesFilterType filterType, CompareBehaviour compareBehaviour)
            where TEntity : DynamicDataEntity
        {
            return true;
        }
    }
}