using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.DynamicData.Extensions
{
    public static class DynamicEntityFilterExtension
    {
        public static bool WhenValueGreaterThan<TEntity, T>(this TEntity entity, string name, T filter)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValues<TEntity, T>(this TEntity entity, T filter)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValues<TEntity, T>(this TEntity entity, T filter, int? idObjectType)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValuesAny<TEntity, T>(this IEnumerable<TEntity> entity, T filter)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValuesAny<TEntity, T>(this IEnumerable<TEntity> entity, T filter, int? idObjectType)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValuesAll<TEntity, T>(this IEnumerable<TEntity> entity, T filter)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenValuesAll<TEntity, T>(this IEnumerable<TEntity> entity, T filter, int? idObjectType)
            where TEntity : DynamicDataEntity
        {
            return true;
        }
    }
}