using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Extensions
{
    public static class DynamicEntityFilterExtension
    {
        public static bool When<TEntity, T>(this TEntity entity, T filter)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool When<TEntity, T>(this TEntity entity, T filter, int? idObjectType)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenAny<TEntity, T>(this ICollection<TEntity> entity, T filter)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenAny<TEntity, T>(this ICollection<TEntity> entity, T filter, int? idObjectType)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenAll<TEntity, T>(this ICollection<TEntity> entity, T filter)
            where TEntity : DynamicDataEntity
        {
            return true;
        }

        public static bool WhenAll<TEntity, T>(this ICollection<TEntity> entity, T filter, int? idObjectType)
            where TEntity : DynamicDataEntity
        {
            return true;
        }
    }
}
