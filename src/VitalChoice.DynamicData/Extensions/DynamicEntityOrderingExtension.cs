using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.DynamicData.Extensions
{
    public class DynamicEntityOrderingExtension<TEntity, TD>
        where TEntity: DynamicDataEntity
    {
        public IOrderedQueryable<TEntity> OrderByValue<T, TOptionValue>(IQueryable<TEntity> entity, Func<TEntity, T> objectSelector, string fieldName, int? idObjectType)
            where T : DynamicDataEntity<TOptionValue> 
            where TOptionValue : OptionValue
        {
            return
                entity.OrderBy(
                    e =>
                        objectSelector(e)
                            .OptionValues.Where(v => v.IdOptionType == 2 || v.IdOptionType == 3)
                            .Select(v => v.Value)
                            .FirstOrDefault());
        }

        public IOrderedQueryable<TEntity> OrderByDescendingValue<T, TOptionValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector, string fieldName, int? idObjectType)
            where T : DynamicDataEntity<TOptionValue>
            where TOptionValue : OptionValue
        {
            var innerParam = Expression.Parameter(typeof(TEntity));

            Expression<Func<T, string>> orderByValue =
                e => e.OptionValues.Where(v => v.IdOptionType == 2 || v.IdOptionType == 3).Select(v => v.Value).FirstOrDefault();

            Expression<Func<TEntity, string>> orderBy =
                Expression.Lambda<Func<TEntity, string>>(Expression.Invoke(orderByValue, Expression.Invoke(objectSelector, innerParam)),
                    innerParam);

            return
                entity.OrderByDescending(orderBy);
        }

        public IOrderedQueryable<TEntity> OrderByValue<T, TOptionValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, ICollection<T>>> collectionSelector, Expression<Func<T, bool>> filterFunc, string fieldName,
            int? idObjectType)
            where T : DynamicDataEntity<TOptionValue>
            where TOptionValue : OptionValue
        {
            var innerParam = Expression.Parameter(typeof(TEntity));
            var collectionParam = Expression.Parameter(typeof(ICollection<T>));
            Expression<Func<T, string>> orderByValue =
                e => e.OptionValues.Where(v => v.IdOptionType == 2 || v.IdOptionType == 3).Select(v => v.Value).FirstOrDefault();
            Expression<Func<ICollection<T>, string>> select =
                Expression.Lambda<Func<ICollection<T>, string>>(Expression.Invoke(orderByValue,
                    Expression.Call(typeof(IEnumerable<T>).GetMethod("Where"), collectionParam, filterFunc)), collectionParam);
            Expression<Func<TEntity, string>> orderBy =
                Expression.Lambda<Func<TEntity, string>>(Expression.Invoke(select, Expression.Invoke(collectionSelector, innerParam)),
                    innerParam);
            return
                entity.OrderBy(orderBy);
        }

        public IOrderedQueryable<TEntity> OrderByDescendingValue<T, TOptionValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, ICollection<T>>> collectionSelector, Expression<Func<T, bool>> filterFunc, string fieldName,
            int? idObjectType)
            where T : DynamicDataEntity<TOptionValue>
            where TOptionValue : OptionValue
        {
            var innerParam = Expression.Parameter(typeof(TEntity));
            var collectionParam = Expression.Parameter(typeof(ICollection<T>));
            Expression<Func<T, string>> orderByValue =
                e => e.OptionValues.Where(v => v.IdOptionType == 2 || v.IdOptionType == 3).Select(v => v.Value).FirstOrDefault();
            Expression<Func<ICollection<T>, string>> select =
                Expression.Lambda<Func<ICollection<T>, string>>(Expression.Invoke(orderByValue,
                    Expression.Call(typeof(IEnumerable<T>).GetMethod("Where"), collectionParam, filterFunc)), collectionParam);
            Expression<Func<TEntity, string>> orderBy =
                Expression.Lambda<Func<TEntity, string>>(Expression.Invoke(select, Expression.Invoke(collectionSelector, innerParam)),
                    innerParam);
            return
                entity.OrderByDescending(orderBy);
        }
    }
}
