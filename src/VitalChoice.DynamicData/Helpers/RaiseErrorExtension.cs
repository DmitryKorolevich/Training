using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Helpers
{
    public static class RaiseErrorExtension
    {
        public static void RaiseValidationError<TEntity, TOptionValue, TOptionType, TCollectionItem>(
            this DynamicObject<TEntity, TOptionValue, TOptionType> obj,
            Expression<Func<DynamicObject<TEntity, TOptionValue, TOptionType>, ICollection<TCollectionItem>>>
                collectionExpression, int index, Expression<Func<TCollectionItem, object>> fieldExpression, string error)
            where TOptionType : OptionType, new()
            where TOptionValue : OptionValue<TOptionType>, new()
            where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        {

        }

        public static void RaiseValidationError<TEntity, TOptionValue, TOptionType>(
            this DynamicObject<TEntity, TOptionValue, TOptionType> obj,
            string modelCollectionName, int index, string modelFieldName, string error)
            where TOptionType : OptionType, new()
            where TOptionValue : OptionValue<TOptionType>, new()
            where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        {

        }
    }
}